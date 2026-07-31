using Guardian.Domain.Identity;

namespace Guardian.Application.Identity;

public enum IdentityRevisionStatus
{
    Revised,
    NoChange,
    Locked,
    Rejected,
    RequiresCoherentCurrentDecision,
}

public enum IdentityRevisionFailure
{
    None,
    SourceWorkNotFound,
    CandidateNotFound,
    CandidateSourceWorkMismatch,
    CandidateDoesNotIdentifyWork,
    InvalidAuthority,
    NoCurrentCorrespondence,
    SupersededDecisionNotApplicable,
    DecisionCategoryMismatch,
    DecisionScopeMismatch,
}

public sealed record IdentityRevisionResult(
    IdentityRevisionStatus Status,
    IdentityRevisionFailure Failure,
    IdentityDecision? Decision,
    IdentityValidationHistoryEvent? HistoryEvent,
    AcceptedCorrespondenceKnowledge Knowledge)
{
    public bool Changed => Status == IdentityRevisionStatus.Revised;
}
