namespace Guardian.Domain.Identity;

public sealed record AcceptedCorrespondenceKnowledge
{
    private AcceptedCorrespondenceKnowledge(
        SourceWorkId sourceWorkId,
        WorkId? acceptedWorkId,
        DecisionId? supportingDecisionId)
    {
        SourceWorkId = sourceWorkId;
        AcceptedWorkId = acceptedWorkId;
        SupportingDecisionId = supportingDecisionId;
    }

    public SourceWorkId SourceWorkId { get; }

    public WorkId? AcceptedWorkId { get; }

    public DecisionId? SupportingDecisionId { get; }

    public bool IsKnown => AcceptedWorkId.HasValue && SupportingDecisionId.HasValue;

    internal static AcceptedCorrespondenceKnowledge Unknown(SourceWorkId sourceWorkId) =>
        new(sourceWorkId, null, null);

    internal static AcceptedCorrespondenceKnowledge Known(IdentityDecision decision) =>
        new(decision.SourceWorkId, decision.AcceptedWorkId, decision.Id);
}

public enum AcceptedCorrespondenceEvaluationStatus
{
    Evaluated,
    UnsupportedMultipleApplicableDecisions,
}

public sealed record AcceptedCorrespondenceEvaluation(
    AcceptedCorrespondenceEvaluationStatus Status,
    AcceptedCorrespondenceKnowledge Knowledge);

public static class AcceptedCorrespondenceEvaluator
{
    public static AcceptedCorrespondenceEvaluation Evaluate(
        SourceWorkId sourceWorkId,
        IEnumerable<IdentityDecision> applicableDecisions)
    {
        ArgumentNullException.ThrowIfNull(applicableDecisions);

        IdentityDecision[] relevantDecisions = applicableDecisions
            .Where(decision => decision.SourceWorkId == sourceWorkId)
            .ToArray();

        return relevantDecisions.Length switch
        {
            0 => new(
                AcceptedCorrespondenceEvaluationStatus.Evaluated,
                AcceptedCorrespondenceKnowledge.Unknown(sourceWorkId)),
            1 => new(
                AcceptedCorrespondenceEvaluationStatus.Evaluated,
                AcceptedCorrespondenceKnowledge.Known(relevantDecisions[0])),
            _ => new(
                AcceptedCorrespondenceEvaluationStatus.UnsupportedMultipleApplicableDecisions,
                AcceptedCorrespondenceKnowledge.Unknown(sourceWorkId)),
        };
    }
}
