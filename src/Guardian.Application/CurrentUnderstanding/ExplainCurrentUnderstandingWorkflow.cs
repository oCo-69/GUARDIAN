using Guardian.Application.Identity;
using Guardian.Domain.Identity;

namespace Guardian.Application.CurrentUnderstanding;

public sealed class ExplainCurrentUnderstandingWorkflow
{
    private readonly IdentityCorrespondenceWorkflow correspondenceWorkflow;

    public ExplainCurrentUnderstandingWorkflow(IdentityCorrespondenceWorkflow correspondenceWorkflow)
    {
        this.correspondenceWorkflow = correspondenceWorkflow ??
            throw new ArgumentNullException(nameof(correspondenceWorkflow));
    }

    public CurrentUnderstandingExplanation Explain(SourceWorkId sourceWorkId)
    {
        AcceptedCorrespondenceKnowledge knowledge =
            correspondenceWorkflow.GetKnowledge(sourceWorkId);
        IdentityDecision[] decisions = correspondenceWorkflow.Decisions
            .Where(decision => decision.SourceWorkId == sourceWorkId)
            .ToArray();
        IdentityValidationHistoryEvent[] historyEvents = correspondenceWorkflow.HistoryEvents
            .Where(historyEvent => historyEvent.SourceWorkId == sourceWorkId)
            .ToArray();

        return new CurrentUnderstandingExplanation(
            sourceWorkId,
            knowledge,
            Array.AsReadOnly(decisions),
            Array.AsReadOnly(historyEvents));
    }
}

public sealed record CurrentUnderstandingExplanation
{
    internal CurrentUnderstandingExplanation(
        SourceWorkId sourceWorkId,
        AcceptedCorrespondenceKnowledge knowledge,
        IReadOnlyList<IdentityDecision> decisions,
        IReadOnlyList<IdentityValidationHistoryEvent> historyEvents)
    {
        SourceWorkId = sourceWorkId;
        Knowledge = knowledge;
        Decisions = decisions;
        HistoryEvents = historyEvents;
    }

    public SourceWorkId SourceWorkId { get; }

    public AcceptedCorrespondenceKnowledge Knowledge { get; }

    public IReadOnlyList<IdentityDecision> Decisions { get; }

    public IReadOnlyList<IdentityValidationHistoryEvent> HistoryEvents { get; }

    public IdentityDecision? SupportingDecision =>
        Knowledge.SupportingDecisionId is DecisionId supportingDecisionId
            ? Decisions.SingleOrDefault(decision => decision.Id == supportingDecisionId)
            : null;

    public bool IsKnown => Knowledge.IsKnown;
}
