using Guardian.Application.CandidateReview;
using Guardian.Application.EditorialDecision;
using Guardian.Application.Identity;
using Guardian.Application.LibraryObservation;
using Guardian.Domain.Identity;

namespace Guardian.Tests.EditorialDecision;

public sealed class AcceptEditorialDecisionWorkflowTests
{
    private static readonly SourceWorkId SourceWorkId =
        new(new Guid("10000000-0000-0000-0000-000000000001"));
    private static readonly WorkId WorkId =
        new(new Guid("20000000-0000-0000-0000-000000000001"));
    private static readonly CandidateId CandidateId =
        new(new Guid("30000000-0000-0000-0000-000000000001"));
    private static readonly DecisionId DecisionId =
        new(new Guid("40000000-0000-0000-0000-000000000001"));
    private static readonly HistoryEventId HistoryEventId =
        new(new Guid("50000000-0000-0000-0000-000000000001"));
    private static readonly DateTimeOffset DecisionTime =
        new(2026, 8, 1, 12, 0, 0, TimeSpan.Zero);
    private static readonly EditorialAuthority Authority = new("human-reviewer");

    [Fact]
    public void AcceptsSelectedCandidateAndCompletesReview()
    {
        AcceptEditorialDecisionWorkflow workflow = CreateWorkflow();

        EditorialDecisionAcceptanceResult result = workflow.Accept(CandidateId, Authority);

        Assert.True(result.ReviewCompleted);
        Assert.Equal(IdentityValidationStatus.Established, result.Validation.Status);
        Assert.NotNull(result.Decision);
        Assert.NotNull(result.HistoryEvent);
        Assert.Equal(SourceWorkId, result.Decision!.SourceWorkId);
        Assert.Equal(WorkId, result.Decision.AcceptedWorkId);
        Assert.Equal(result.Decision.Id, result.HistoryEvent!.DecisionId);
        Assert.True(result.Knowledge.IsKnown);
        Assert.Equal(WorkId, result.Knowledge.AcceptedWorkId);
        Assert.Equal(result.Decision.Id, result.Knowledge.SupportingDecisionId);
    }

    [Fact]
    public void InvalidSelectionDoesNotCreateEditorialRecords()
    {
        AcceptEditorialDecisionWorkflow workflow = CreateWorkflow();

        EditorialDecisionAcceptanceResult result = workflow.Accept(
            new CandidateId(new Guid("30000000-0000-0000-0000-000000000099")),
            Authority);

        Assert.False(result.ReviewCompleted);
        Assert.Equal(IdentityValidationStatus.Rejected, result.Validation.Status);
        Assert.Equal(IdentityValidationFailure.CandidateNotFound, result.Validation.Failure);
        Assert.Null(result.Decision);
        Assert.Null(result.HistoryEvent);
    }

    [Fact]
    public void MissingAuthorityDoesNotCreateEditorialRecords()
    {
        AcceptEditorialDecisionWorkflow workflow = CreateWorkflow();

        EditorialDecisionAcceptanceResult result = workflow.Accept(CandidateId, null!);

        Assert.False(result.ReviewCompleted);
        Assert.Equal(IdentityValidationFailure.InvalidAuthority, result.Validation.Failure);
        Assert.Null(result.Decision);
        Assert.Null(result.HistoryEvent);
    }

    private static AcceptEditorialDecisionWorkflow CreateWorkflow()
    {
        ObservedSourceWork sourceWork = new(SourceWorkId, "library-a", "item-1", "Observed Series");
        IdentityCandidate candidate = new(
            CandidateId,
            SourceWorkId,
            WorkId,
            new ProviderIdentity("TMDb", "Series", "42"),
            "Example Series",
            2024,
            "Series",
            new CandidateProvenance("deterministic-test", "candidate-1"));
        CandidateReviewContext context = ReviewCandidatesWorkflow.Create(sourceWork, [candidate]);
        return new AcceptEditorialDecisionWorkflow(
            context,
            () => DecisionId,
            () => HistoryEventId,
            () => DecisionTime);
    }
}
