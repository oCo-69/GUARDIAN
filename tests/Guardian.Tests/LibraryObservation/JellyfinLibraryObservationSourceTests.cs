using System.Net;
using Guardian.Application.LibraryObservation;
using Guardian.Jellyfin.LibraryObservation;

namespace Guardian.Tests.LibraryObservation;

public sealed class JellyfinLibraryObservationSourceTests
{
    [Fact]
    public async Task ReadsMovieAndSeriesItemsFromEveryVirtualLibrary()
    {
        RecordingHandler handler = new();
        using HttpClient client = new(handler)
        {
            BaseAddress = new Uri("https://jellyfin.example/")
        };
        JellyfinLibraryObservationSource source = new(client, "test-token");

        IReadOnlyList<ObservedLibraryWork> works =
            await source.ReadWorksAsync();

        Assert.Collection(
            works,
            first =>
            {
                Assert.Equal("library-a", first.LibraryId);
                Assert.Equal("item-1", first.ExternalId);
                Assert.Equal("First Series", first.Name);
            },
            second =>
            {
                Assert.Equal("library-b", second.LibraryId);
                Assert.Equal("item-2", second.ExternalId);
                Assert.Equal("Second Movie", second.Name);
            });

        Assert.Equal(3, handler.Requests.Count);
        Assert.All(handler.Requests, request =>
        {
            Assert.Equal("test-token", request.Headers.GetValues("X-Emby-Token").Single());
            Assert.Equal("MediaBrowser", request.Headers.Authorization?.Scheme);
        });
        Assert.Contains("ParentId=library-a", handler.Requests[1].RequestUri!.Query);
        Assert.Contains("ParentId=library-b", handler.Requests[2].RequestUri!.Query);
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public List<HttpRequestMessage> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Requests.Add(request);
            string content = Requests.Count switch
            {
                1 => "[{\"ItemId\":\"library-a\"},{\"ItemId\":\"library-b\"}]",
                2 => "{\"Items\":[{\"Id\":\"item-1\",\"Name\":\"First Series\"}]}",
                _ => "{\"Items\":[{\"Id\":\"item-2\",\"Name\":\"Second Movie\"}]}",
            };

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(content),
                RequestMessage = request,
            });
        }
    }
}
