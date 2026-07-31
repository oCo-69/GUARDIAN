using Guardian.Domain.Identity;

namespace Guardian.Application.Identity;

public sealed record IdentityRevisionRequest(
    SourceWorkId SourceWorkId,
    CandidateId CandidateId,
    DecisionId CurrentDecisionId,
    EditorialAuthority? Authority,
    bool IsCurrentDecisionLocked,
    string DecisionCategory = IdentityDecision.Category,
    string DecisionScope = IdentityDecision.Scope);
