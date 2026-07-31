namespace Guardian.Domain.Identity;

public sealed record IdentityCandidate(
    CandidateId Id,
    SourceWorkId SourceWorkId,
    WorkId WorkId,
    ProviderIdentity? ProviderEvidence);
