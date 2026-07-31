namespace Guardian.Domain.Identity;

public sealed record IdentityValidationHistoryEvent(
    HistoryEventId Id,
    DecisionId DecisionId,
    SourceWorkId SourceWorkId,
    CandidateId CandidateId,
    string ActorId,
    DateTimeOffset OccurredAt);
