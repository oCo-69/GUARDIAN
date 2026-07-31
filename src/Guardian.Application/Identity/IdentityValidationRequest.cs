using Guardian.Domain.Identity;

namespace Guardian.Application.Identity;

public sealed record IdentityValidationRequest(
    SourceWorkId SourceWorkId,
    CandidateId CandidateId,
    EditorialAuthority? Authority);
