using Guardian.Application.CandidateReview;
using Guardian.Application.Identity;
using Guardian.Domain.Identity;

namespace Guardian.Application.EditorialDecision;

public sealed class AcceptEditorialDecisionWorkflow
{
    private readonly CandidateReviewContext reviewContext;
    private readonly IdentityCorrespondenceWorkflow correspondenceWorkflow;

    public AcceptEditorialDecisionWorkflow(
        CandidateReviewContext reviewContext,
        Func<DecisionId>? nextDecisionId = null,
        Func<HistoryEventId>? nextHistoryEventId = null,
        Func<DateTimeOffset>? currentTime = null)
    {
        this.reviewContext = reviewContext ?? throw new ArgumentNullException(nameof(reviewContext));
        correspondenceWorkflow = new IdentityCorrespondenceWorkflow(
            [reviewContext.SourceWork.SourceWork],
            reviewContext.Candidates,
            nextDecisionId,
            nextHistoryEventId,
            currentTime);
    }

    public EditorialDecisionAcceptanceResult Accept(
        CandidateId selectedCandidateId,
        EditorialAuthority authority)
    {
        IdentityValidationResult validation = correspondenceWorkflow.Validate(
            new IdentityValidationRequest(
                reviewContext.SourceWork.Id,
                selectedCandidateId,
                authority));

        return new EditorialDecisionAcceptanceResult(
            validation,
            validation.Status == IdentityValidationStatus.Established);
    }
}

public sealed record EditorialDecisionAcceptanceResult(
    IdentityValidationResult Validation,
    bool ReviewCompleted)
{
    public IdentityDecision? Decision => Validation.Decision;

    public IdentityValidationHistoryEvent? HistoryEvent => Validation.HistoryEvent;

    public AcceptedCorrespondenceKnowledge Knowledge => Validation.Knowledge;
}
