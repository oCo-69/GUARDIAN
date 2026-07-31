using Guardian.Domain.Identity;

namespace Guardian.Application.Identity;

public sealed class IdentityCorrespondenceWorkflow
{
    private readonly Dictionary<SourceWorkId, SourceWork> sourceWorks;
    private readonly Dictionary<CandidateId, IdentityCandidate> candidates;
    private readonly List<InitialIdentityValidationRecords> validationRecords = [];
    private readonly Func<DecisionId> nextDecisionId;
    private readonly Func<HistoryEventId> nextHistoryEventId;
    private readonly Func<DateTimeOffset> currentTime;

    public IdentityCorrespondenceWorkflow(
        IEnumerable<SourceWork> sourceWorks,
        IEnumerable<IdentityCandidate> candidates,
        Func<DecisionId>? nextDecisionId = null,
        Func<HistoryEventId>? nextHistoryEventId = null,
        Func<DateTimeOffset>? currentTime = null)
    {
        ArgumentNullException.ThrowIfNull(sourceWorks);
        ArgumentNullException.ThrowIfNull(candidates);

        this.sourceWorks = sourceWorks.ToDictionary(sourceWork => sourceWork.Id);
        this.candidates = candidates.ToDictionary(candidate => candidate.Id);
        this.nextDecisionId = nextDecisionId ?? DecisionId.New;
        this.nextHistoryEventId = nextHistoryEventId ?? HistoryEventId.New;
        this.currentTime = currentTime ?? (() => DateTimeOffset.UtcNow);
    }

    public IReadOnlyList<IdentityDecision> Decisions =>
        Array.AsReadOnly(validationRecords.Select(records => records.Decision).ToArray());

    public IReadOnlyList<IdentityValidationHistoryEvent> HistoryEvents =>
        Array.AsReadOnly(validationRecords.Select(records => records.HistoryEvent).ToArray());

    public AcceptedCorrespondenceEvaluation EvaluateKnowledge(SourceWorkId sourceWorkId) =>
        AcceptedCorrespondenceEvaluator.Evaluate(
            sourceWorkId,
            CurrentApplicableDecisions(sourceWorkId));

    public AcceptedCorrespondenceKnowledge GetKnowledge(SourceWorkId sourceWorkId) =>
        EvaluateKnowledge(sourceWorkId).Knowledge;

    public IdentityValidationResult Validate(IdentityValidationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        AcceptedCorrespondenceKnowledge currentKnowledge = GetKnowledge(request.SourceWorkId);

        if (!sourceWorks.ContainsKey(request.SourceWorkId))
        {
            return Rejected(IdentityValidationFailure.SourceWorkNotFound, currentKnowledge);
        }

        if (!candidates.TryGetValue(request.CandidateId, out IdentityCandidate? candidate))
        {
            return Rejected(IdentityValidationFailure.CandidateNotFound, currentKnowledge);
        }

        if (candidate.SourceWorkId != request.SourceWorkId)
        {
            return Rejected(IdentityValidationFailure.CandidateSourceWorkMismatch, currentKnowledge);
        }

        if (candidate.WorkId == default)
        {
            return Rejected(IdentityValidationFailure.CandidateDoesNotIdentifyWork, currentKnowledge);
        }

        if (request.Authority is null || string.IsNullOrWhiteSpace(request.Authority.ActorId))
        {
            return Rejected(IdentityValidationFailure.InvalidAuthority, currentKnowledge);
        }

        IdentityDecision? applicableDecision = CurrentApplicableDecisions(request.SourceWorkId)
            .SingleOrDefault();

        if (applicableDecision is not null)
        {
            if (applicableDecision.CandidateId == candidate.Id &&
                applicableDecision.AcceptedWorkId == candidate.WorkId)
            {
                return new IdentityValidationResult(
                    IdentityValidationStatus.NoChange,
                    IdentityValidationFailure.None,
                    applicableDecision,
                    null,
                    currentKnowledge);
            }

            return new IdentityValidationResult(
                IdentityValidationStatus.RequiresSupersession,
                IdentityValidationFailure.None,
                applicableDecision,
                null,
                currentKnowledge);
        }

        InitialIdentityValidationRecords records = InitialIdentityValidation.Establish(
            candidate,
            request.Authority,
            nextDecisionId(),
            nextHistoryEventId(),
            currentTime());

        Record(records);

        AcceptedCorrespondenceEvaluation evaluation = EvaluateKnowledge(request.SourceWorkId);

        return new IdentityValidationResult(
            IdentityValidationStatus.Established,
            IdentityValidationFailure.None,
            records.Decision,
            records.HistoryEvent,
            evaluation.Knowledge);
    }

    public IdentityRevisionResult Revise(IdentityRevisionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        AcceptedCorrespondenceKnowledge currentKnowledge = GetKnowledge(request.SourceWorkId);

        if (!sourceWorks.ContainsKey(request.SourceWorkId))
        {
            return RevisionRejected(IdentityRevisionFailure.SourceWorkNotFound, currentKnowledge);
        }

        if (!candidates.TryGetValue(request.CandidateId, out IdentityCandidate? candidate))
        {
            return RevisionRejected(IdentityRevisionFailure.CandidateNotFound, currentKnowledge);
        }

        if (candidate.SourceWorkId != request.SourceWorkId)
        {
            return RevisionRejected(IdentityRevisionFailure.CandidateSourceWorkMismatch, currentKnowledge);
        }

        if (candidate.WorkId == default)
        {
            return RevisionRejected(IdentityRevisionFailure.CandidateDoesNotIdentifyWork, currentKnowledge);
        }

        if (request.Authority is null || string.IsNullOrWhiteSpace(request.Authority.ActorId))
        {
            return RevisionRejected(IdentityRevisionFailure.InvalidAuthority, currentKnowledge);
        }

        if (request.DecisionCategory != IdentityDecision.Category)
        {
            return RevisionRejected(IdentityRevisionFailure.DecisionCategoryMismatch, currentKnowledge);
        }

        if (request.DecisionScope != IdentityDecision.Scope)
        {
            return RevisionRejected(IdentityRevisionFailure.DecisionScopeMismatch, currentKnowledge);
        }

        IdentityDecision[] applicable = CurrentApplicableDecisions(request.SourceWorkId).ToArray();

        if (applicable.Length == 0)
        {
            return new IdentityRevisionResult(
                IdentityRevisionStatus.RequiresCoherentCurrentDecision,
                IdentityRevisionFailure.NoCurrentCorrespondence,
                null,
                null,
                currentKnowledge);
        }

        if (applicable.Length != 1)
        {
            return new IdentityRevisionResult(
                IdentityRevisionStatus.RequiresCoherentCurrentDecision,
                IdentityRevisionFailure.None,
                null,
                null,
                currentKnowledge);
        }

        IdentityDecision currentDecision = applicable[0];

        if (currentDecision.Id != request.CurrentDecisionId)
        {
            return RevisionRejected(IdentityRevisionFailure.SupersededDecisionNotApplicable, currentKnowledge, currentDecision);
        }

        if (candidate.WorkId == currentDecision.AcceptedWorkId)
        {
            return new IdentityRevisionResult(
                IdentityRevisionStatus.NoChange,
                IdentityRevisionFailure.None,
                currentDecision,
                null,
                currentKnowledge);
        }

        if (request.IsCurrentDecisionLocked)
        {
            return new IdentityRevisionResult(
                IdentityRevisionStatus.Locked,
                IdentityRevisionFailure.None,
                currentDecision,
                null,
                currentKnowledge);
        }

        InitialIdentityValidationRecords records = InitialIdentityValidation.Revise(
            candidate,
            currentDecision,
            request.Authority,
            nextDecisionId(),
            nextHistoryEventId(),
            currentTime());

        Record(records);

        AcceptedCorrespondenceEvaluation evaluation = EvaluateKnowledge(request.SourceWorkId);

        return new IdentityRevisionResult(
            IdentityRevisionStatus.Revised,
            IdentityRevisionFailure.None,
            records.Decision,
            records.HistoryEvent,
            evaluation.Knowledge);
    }

    private IEnumerable<IdentityDecision> CurrentApplicableDecisions(SourceWorkId sourceWorkId) =>
        validationRecords
            .Select(records => records.Decision)
            .Where(decision => decision.SourceWorkId == sourceWorkId)
            .Where(decision => !validationRecords.Any(
                other => other.Decision.SupersedesDecisionId == decision.Id));

    private void Record(InitialIdentityValidationRecords records)
    {
        ArgumentNullException.ThrowIfNull(records);

        if (records.HistoryEvent.DecisionId != records.Decision.Id)
        {
            throw new ArgumentException(
                "The HistoryEvent must reference its corresponding Decision.",
                nameof(records));
        }

        if (validationRecords.Any(existing => existing.Decision.Id == records.Decision.Id))
        {
            throw new InvalidOperationException("A Decision with the same identity already exists.");
        }

        if (validationRecords.Any(existing => existing.HistoryEvent.Id == records.HistoryEvent.Id))
        {
            throw new InvalidOperationException("A HistoryEvent with the same identity already exists.");
        }

        validationRecords.Add(records);
    }

    private static IdentityValidationResult Rejected(
        IdentityValidationFailure failure,
        AcceptedCorrespondenceKnowledge knowledge) =>
        new(IdentityValidationStatus.Rejected, failure, null, null, knowledge);

    private static IdentityRevisionResult RevisionRejected(
        IdentityRevisionFailure failure,
        AcceptedCorrespondenceKnowledge knowledge,
        IdentityDecision? decision = null) =>
        new(IdentityRevisionStatus.Rejected, failure, decision, null, knowledge);
}
