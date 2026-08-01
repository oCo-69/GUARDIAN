using Guardian.Application.CandidateReview;
using Guardian.Application.LibraryObservation;
using Guardian.Domain.Identity;

namespace Guardian.Tests.CandidateReview;

public sealed class ReviewCandidatesWorkflowTests
{
    private static readonly SourceWorkId SourceWorkId =
        new(new Guid("10000000-0000-0000-0000-000000000001"));
    private static readonly WorkId FirstWorkId =
        new(new Guid("20000000-0000-0000-0000-000000000001"));
    private static readonly WorkId SecondWorkId =
        new(new Guid("20000000-0000-0000-0000-000000000002"));

    [Fact]
    public void CreatesCompleteUndecidedReviewContext()
    {
        ObservedSourceWork sourceWork = CreateSourceWork();
        IdentityCandidate first = CreateCandidate(
            new Guid("30000000-0000-0000-0000-000000000001"),
            FirstWorkId,
            "First Candidate");
        IdentityCandidate second = CreateCandidate(
            new Guid("30000000-0000-0000-0000-000000000002"),
            SecondWorkId,
            "Second Candidate");

        CandidateReviewContext context = ReviewCandidatesWorkflow.Create(
            sourceWork,
            [first, second]);

        Assert.Same(sourceWork, context.SourceWork);
        Assert.Equal([first, second], context.Candidates);
        Assert.Equal(
            CandidateReviewDecisionState.NoDecisionEstablished,
            context.DecisionState);
        Assert.Equal("First Candidate", context.Candidates[0].Title);
        Assert.NotNull(context.Candidates[0].Provenance);
    }

    [Fact]
    public void CreatesAnExplicitUndecidedContextWhenNoCandidatesExist()
    {
        CandidateReviewContext context = ReviewCandidatesWorkflow.Create(
            CreateSourceWork(),
            []);

        Assert.Empty(context.Candidates);
        Assert.Equal(
            CandidateReviewDecisionState.NoDecisionEstablished,
            context.DecisionState);
    }

    [Fact]
    public void RejectsCandidatesForAnotherSourceWork()
    {
        IdentityCandidate candidate = new(
            new CandidateId(new Guid("30000000-0000-0000-0000-000000000001")),
            new SourceWorkId(new Guid("10000000-0000-0000-0000-000000000099")),
            FirstWorkId,
            null,
            "Wrong Source",
            null,
            "Series",
            new CandidateProvenance("deterministic-test"));

        Assert.Throws<ArgumentException>(() =>
            ReviewCandidatesWorkflow.Create(CreateSourceWork(), [candidate]));
    }

    [Fact]
    public void DoesNotCreateEditorialRecords()
    {
        CandidateReviewContext context = ReviewCandidatesWorkflow.Create(
            CreateSourceWork(),
            [CreateCandidate(
                new Guid("30000000-0000-0000-0000-000000000001"),
                FirstWorkId,
                "Candidate")]);

        Assert.Equal(CandidateReviewDecisionState.NoDecisionEstablished, context.DecisionState);
    }

    private static ObservedSourceWork CreateSourceWork() =>
        new(SourceWorkId, "library-a", "item-1", "Observed Series");

    private static IdentityCandidate CreateCandidate(Guid candidateId, WorkId workId, string title) =>
        new(
            new CandidateId(candidateId),
            SourceWorkId,
            workId,
            new ProviderIdentity("TMDb", "Series", candidateId.ToString("N")),
            title,
            2024,
            "Series",
            new CandidateProvenance("deterministic-test", candidateId.ToString("N")));
}
