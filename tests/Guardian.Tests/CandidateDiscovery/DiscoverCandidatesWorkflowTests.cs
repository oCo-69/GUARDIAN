using Guardian.Application.CandidateDiscovery;
using Guardian.Application.LibraryObservation;
using Guardian.Domain.Identity;

namespace Guardian.Tests.CandidateDiscovery;

public sealed class DiscoverCandidatesWorkflowTests
{
    private static readonly SourceWorkId SourceWorkId =
        new(new Guid("10000000-0000-0000-0000-000000000001"));
    private static readonly WorkId WorkId =
        new(new Guid("20000000-0000-0000-0000-000000000001"));
    private static readonly CandidateId CandidateId =
        new(new Guid("30000000-0000-0000-0000-000000000001"));
    private static readonly ProviderIdentity ProviderEvidence =
        new("TMDb", "Series", "42");

    [Fact]
    public async Task ReturnsReviewableCandidatesWithoutGrantingAuthority()
    {
        DiscoveredCandidate candidate = new(
            CandidateId,
            WorkId,
            "Example Series",
            2024,
            "Series",
            ProviderEvidence,
            new CandidateProvenance("deterministic-test", "candidate-1"));
        DiscoverCandidatesWorkflow workflow = new(new StubSource(
            new CandidateDiscoverySourceResult([candidate])));

        DiscoverCandidatesResult result = await workflow.DiscoverAsync(CreateSourceWork());

        IdentityCandidate actual = Assert.Single(result.Candidates);
        Assert.Equal(DiscoverCandidatesStatus.Completed, result.Status);
        Assert.Equal(SourceWorkId, actual.SourceWorkId);
        Assert.Equal(WorkId, actual.WorkId);
        Assert.Equal("Example Series", actual.Title);
        Assert.Equal(2024, actual.Year);
        Assert.Equal("Series", actual.MediaType);
        Assert.Equal(ProviderEvidence, actual.ProviderEvidence);
        Assert.Equal("deterministic-test", actual.Provenance!.DiscoverySource);
    }

    [Fact]
    public async Task EmptySourceResultIsReportedExplicitly()
    {
        DiscoverCandidatesWorkflow workflow = new(new StubSource(
            CandidateDiscoverySourceResult.Empty()));

        DiscoverCandidatesResult result = await workflow.DiscoverAsync(CreateSourceWork());

        Assert.Equal(DiscoverCandidatesStatus.NoCandidates, result.Status);
        Assert.Empty(result.Candidates);
        Assert.False(result.HasCandidates);
    }

    [Fact]
    public async Task SourceFailureDoesNotProduceCandidates()
    {
        DiscoverCandidatesWorkflow workflow = new(new StubSource(
            CandidateDiscoverySourceResult.Failed(CandidateDiscoveryFailure.SourceUnavailable)));

        DiscoverCandidatesResult result = await workflow.DiscoverAsync(CreateSourceWork());

        Assert.Equal(DiscoverCandidatesStatus.Failed, result.Status);
        Assert.Equal(CandidateDiscoveryFailure.SourceUnavailable, result.Failure);
        Assert.Empty(result.Candidates);
    }

    [Fact]
    public async Task CandidateDiscoveryDoesNotCreateEditorialRecords()
    {
        DiscoverCandidatesWorkflow workflow = new(new StubSource(
            new CandidateDiscoverySourceResult(
                [new DiscoveredCandidate(
                    CandidateId,
                    WorkId,
                    "Example Series",
                    null,
                    "Series",
                    null,
                    new CandidateProvenance("deterministic-test"))])));

        DiscoverCandidatesResult result = await workflow.DiscoverAsync(CreateSourceWork());

        Assert.Single(result.Candidates);
        Assert.Equal(DiscoverCandidatesStatus.Completed, result.Status);
        Assert.Equal(CandidateDiscoveryFailure.None, result.Failure);
    }

    private static ObservedSourceWork CreateSourceWork() =>
        new(SourceWorkId, "library-a", "item-1", "Observed Series");

    private sealed class StubSource(CandidateDiscoverySourceResult result) : ICandidateDiscoverySource
    {
        public Task<CandidateDiscoverySourceResult> DiscoverAsync(
            ObservedSourceWork sourceWork,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);
    }
}
