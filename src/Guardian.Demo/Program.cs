using System.Net;
using Guardian.Application.CandidateDiscovery;
using Guardian.Application.CandidateReview;
using Guardian.Application.EditorialDecision;
using Guardian.Application.LibraryObservation;
using Guardian.Domain.Identity;
using Guardian.Jellyfin.LibraryObservation;

Console.WriteLine("Guardian — End-to-End Editorial Demonstration");
Console.WriteLine();

using HttpClient httpClient = new(new DeterministicJellyfinHandler())
{
    BaseAddress = new Uri("https://deterministic-jellyfin.example/")
};

JellyfinLibraryObservationSource jellyfinSource = new(httpClient, "demo-token");
LibraryObservationResult observedLibrary = await new ObserveLibraryWorkflow(jellyfinSource)
    .ObserveAsync();
ObservedSourceWork observedSourceWork = observedLibrary.SourceWorks.Single();

Console.WriteLine("1. Library observed");
Console.WriteLine($"   SourceWork: {observedSourceWork.Name}");
Console.WriteLine($"   SourceWork identity: {observedSourceWork.Id.Value}");
Console.WriteLine();

CandidateDiscoverySourceResult discoverySourceResult = new(
[
    new DiscoveredCandidate(
        new CandidateId(new Guid("30000000-0000-0000-0000-000000000001")),
        new WorkId(new Guid("20000000-0000-0000-0000-000000000001")),
        "Stargate SG-1",
        1997,
        "Series",
        new ProviderIdentity("TMDb", "Series", "46298"),
        new CandidateProvenance("deterministic-demo", "candidate-stargate-sg1")),
    new DiscoveredCandidate(
        new CandidateId(new Guid("30000000-0000-0000-0000-000000000002")),
        new WorkId(new Guid("20000000-0000-0000-0000-000000000002")),
        "Stargate",
        1994,
        "Movie",
        new ProviderIdentity("TMDb", "Movie", "2161"),
        new CandidateProvenance("deterministic-demo", "candidate-stargate-movie")),
]);

DiscoverCandidatesResult discovered = await new DiscoverCandidatesWorkflow(
    new DeterministicCandidateSource(discoverySourceResult))
    .DiscoverAsync(observedSourceWork);

Console.WriteLine("2. Candidates discovered");
for (int index = 0; index < discovered.Candidates.Count; index++)
{
    IdentityCandidate candidate = discovered.Candidates[index];
    Console.WriteLine($"   [{index + 1}] {candidate.Title} ({candidate.Year}) — {candidate.MediaType}");
}

CandidateReviewContext review = ReviewCandidatesWorkflow.Create(
    observedSourceWork,
    discovered.Candidates);

Console.WriteLine();
Console.WriteLine("3. Review prepared");
Console.WriteLine($"   Decision status: {review.DecisionState}");

IdentityCandidate selectedCandidate = review.Candidates[0];
Console.WriteLine();
Console.WriteLine("4. User selection");
Console.WriteLine($"   Selected: {selectedCandidate.Title} ({selectedCandidate.Year})");

AcceptEditorialDecisionWorkflow acceptance = new(
    review,
    () => new DecisionId(new Guid("40000000-0000-0000-0000-000000000001")),
    () => new HistoryEventId(new Guid("50000000-0000-0000-0000-000000000001")),
    () => new DateTimeOffset(2026, 8, 1, 12, 0, 0, TimeSpan.Zero));
EditorialDecisionAcceptanceResult accepted = acceptance.Accept(
    selectedCandidate.Id,
    new EditorialAuthority("demo-human-editor"));

Console.WriteLine();
Console.WriteLine("5. Editorial decision accepted");
Console.WriteLine($"   Decision: {accepted.Decision!.Id.Value}");
Console.WriteLine($"   HistoryEvent: {accepted.HistoryEvent!.Id.Value}");

Console.WriteLine();
Console.WriteLine("6. Current understanding");
Console.WriteLine($"   Known correspondence: SourceWork {accepted.Knowledge.SourceWorkId.Value} → Work {accepted.Knowledge.AcceptedWorkId!.Value.Value}");
Console.WriteLine("   Knowledge state: Known");
Console.WriteLine();
Console.WriteLine("Demonstration completed successfully.");

internal sealed class DeterministicCandidateSource(CandidateDiscoverySourceResult result)
    : ICandidateDiscoverySource
{
    public Task<CandidateDiscoverySourceResult> DiscoverAsync(
        ObservedSourceWork sourceWork,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(result);
}

internal sealed class DeterministicJellyfinHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string content = request.RequestUri?.AbsolutePath.EndsWith(
            "/Library/VirtualFolders",
            StringComparison.Ordinal) == true
            ? "[{\"ItemId\":\"library-demo\"}]"
            : "{\"Items\":[{\"Id\":\"item-stargate-sg1\",\"Name\":\"Stargate SG-1\"}],\"TotalRecordCount\":1}";

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(content),
            RequestMessage = request,
        });
    }
}
