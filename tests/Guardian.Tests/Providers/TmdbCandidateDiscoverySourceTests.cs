using System.Net;
using Guardian.Application.CandidateDiscovery;
using Guardian.Application.LibraryObservation;
using Guardian.Domain.Identity;
using Guardian.Providers.Tmdb;

namespace Guardian.Tests.Providers;

public sealed class TmdbCandidateDiscoverySourceTests
{
    [Fact]
    public async Task MapsTvAndMovieSearchResultsToReviewableCandidates()
    {
        RecordingHandler handler = new();
        using HttpClient client = new(handler)
        {
            BaseAddress = new Uri("https://api.themoviedb.org/")
        };
        TmdbCandidateDiscoverySource source = new(client, "test-access-token");

        IReadOnlyList<DiscoveredCandidate> candidates = (await source.DiscoverAsync(
            new ObservedSourceWork(
                new(new Guid("10000000-0000-0000-0000-000000000001")),
                "library-a",
                "item-1",
                "Stargate"))).Candidates;

        Assert.Equal(2, candidates.Count);
        Assert.Equal("Stargate SG-1", candidates[0].Title);
        Assert.Equal(1997, candidates[0].Year);
        Assert.Equal("Series", candidates[0].MediaType);
        ProviderIdentity tvEvidence = Assert.IsType<ProviderIdentity>(candidates[0].ProviderEvidence);
        Assert.Equal("TMDb", tvEvidence.Provider);
        Assert.Equal("100", tvEvidence.ExternalId);
        Assert.Equal("TMDb", candidates[0].Provenance.DiscoverySource);
        Assert.Equal("Stargate", candidates[1].Title);
        Assert.Equal("Movie", candidates[1].MediaType);
        Assert.Equal("200", candidates[1].ProviderEvidence!.ExternalId);
        Assert.Equal(2, handler.Requests.Count);
        Assert.All(handler.Requests, request =>
        {
            Assert.Equal("Bearer", request.Headers.Authorization?.Scheme);
            Assert.Equal("test-access-token", request.Headers.Authorization?.Parameter);
        });
    }

    [Fact]
    public async Task EmptySearchResultsRemainExplicitlyEmpty()
    {
        using HttpClient client = new(new RecordingHandler(empty: true))
        {
            BaseAddress = new Uri("https://api.themoviedb.org/")
        };
        TmdbCandidateDiscoverySource source = new(client, "test-access-token");

        CandidateDiscoverySourceResult result = await source.DiscoverAsync(CreateSourceWork());

        Assert.Equal(CandidateDiscoverySourceStatus.NoCandidates, result.Status);
        Assert.Empty(result.Candidates);
    }

    [Fact]
    public async Task UnauthorizedResponsesAreReportedWithoutCandidates()
    {
        using HttpClient client = new(new RecordingHandler(HttpStatusCode.Unauthorized))
        {
            BaseAddress = new Uri("https://api.themoviedb.org/")
        };
        TmdbCandidateDiscoverySource source = new(client, "test-access-token");

        CandidateDiscoverySourceResult result = await source.DiscoverAsync(CreateSourceWork());

        Assert.Equal(CandidateDiscoverySourceStatus.Failed, result.Status);
        Assert.Equal(CandidateDiscoveryFailure.SourceRejected, result.Failure);
        Assert.Empty(result.Candidates);
    }

    [Fact]
    public void RejectsAnEmptyAccessToken()
    {
        using HttpClient client = new();

        Assert.Throws<ArgumentException>(() => new TmdbCandidateDiscoverySource(client, " "));
    }

    private static ObservedSourceWork CreateSourceWork() =>
        new(new SourceWorkId(new Guid("10000000-0000-0000-0000-000000000001")), "library-a", "item-1", "Stargate");

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode statusCode;
        private readonly bool empty;

        public RecordingHandler(HttpStatusCode statusCode = HttpStatusCode.OK, bool empty = false)
        {
            this.statusCode = statusCode;
            this.empty = empty;
        }

        public List<HttpRequestMessage> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Requests.Add(request);
            string content = empty
                ? "{\"results\":[]}"
                : request.RequestUri?.AbsolutePath.EndsWith("/tv", StringComparison.Ordinal) == true
                    ? "{\"results\":[{\"id\":100,\"name\":\"Stargate SG-1\",\"first_air_date\":\"1997-07-27\"}]}"
                    : "{\"results\":[{\"id\":200,\"title\":\"Stargate\",\"release_date\":\"1994-10-28\"}]}";

            return Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content),
                RequestMessage = request,
            });
        }
    }
}
