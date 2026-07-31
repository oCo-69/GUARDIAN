using Guardian.Domain.Identity;

namespace Guardian.Application.Identity;

public enum IdentityValidationStatus
{
    Established,
    NoChange,
    Rejected,
    RequiresSupersession,
}

public enum IdentityValidationFailure
{
    None,
    SourceWorkNotFound,
    CandidateNotFound,
    CandidateSourceWorkMismatch,
    CandidateDoesNotIdentifyWork,
    InvalidAuthority,
}

public sealed record IdentityValidationResult(
    IdentityValidationStatus Status,
    IdentityValidationFailure Failure,
    IdentityDecision? Decision,
    IdentityValidationHistoryEvent? HistoryEvent,
    AcceptedCorrespondenceKnowledge Knowledge)
{
    public bool Changed => Status == IdentityValidationStatus.Established;
}
