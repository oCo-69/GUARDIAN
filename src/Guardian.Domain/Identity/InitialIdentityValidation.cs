namespace Guardian.Domain.Identity;

internal static class InitialIdentityValidation
{
    public static InitialIdentityValidationRecords Establish(
        IdentityCandidate candidate,
        EditorialAuthority authority,
        DecisionId decisionId,
        HistoryEventId historyEventId,
        DateTimeOffset occurredAt)
    {
        ArgumentNullException.ThrowIfNull(candidate);
        ArgumentNullException.ThrowIfNull(authority);

        if (candidate.WorkId == default)
        {
            throw new ArgumentException("The Candidate must identify a Work.", nameof(candidate));
        }

        IdentityDecision decision = new(
            decisionId,
            candidate.SourceWorkId,
            candidate.WorkId,
            candidate.Id,
            authority,
            occurredAt,
            candidate.ProviderEvidence);

        IdentityValidationHistoryEvent historyEvent = new(
            historyEventId,
            decision.Id,
            candidate.SourceWorkId,
            candidate.Id,
            authority.ActorId,
            occurredAt);

        return new InitialIdentityValidationRecords(decision, historyEvent);
    }
}

internal sealed record InitialIdentityValidationRecords(
    IdentityDecision Decision,
    IdentityValidationHistoryEvent HistoryEvent);
