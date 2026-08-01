using System.Net.Http.Headers;
using System.Text.Json;
using Guardian.Application.LibraryObservation;

namespace Guardian.Jellyfin.LibraryObservation;

public sealed class JellyfinLibraryObservationSource : ILibraryObservationSource
{
    private readonly HttpClient httpClient;
    private readonly string? accessToken;

    public JellyfinLibraryObservationSource(HttpClient httpClient, string? accessToken = null)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        this.accessToken = accessToken;
    }

    public async Task<IReadOnlyList<ObservedLibraryWork>> ReadWorksAsync(
        CancellationToken cancellationToken = default)
    {
        using HttpRequestMessage librariesRequest = CreateRequest(
            HttpMethod.Get,
            "Library/VirtualFolders");
        using HttpResponseMessage librariesResponse = await httpClient
            .SendAsync(librariesRequest, cancellationToken)
            .ConfigureAwait(false);
        librariesResponse.EnsureSuccessStatusCode();

        await using Stream librariesStream = await librariesResponse.Content
            .ReadAsStreamAsync(cancellationToken)
            .ConfigureAwait(false);
        using JsonDocument librariesDocument = await JsonDocument
            .ParseAsync(librariesStream, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        List<ObservedLibraryWork> works = [];
        foreach (JsonElement library in librariesDocument.RootElement.EnumerateArray())
        {
            string? libraryId = GetString(library, "ItemId") ?? GetString(library, "Id");
            if (string.IsNullOrWhiteSpace(libraryId))
            {
                continue;
            }

            const int pageSize = 500;
            for (int startIndex = 0; ; startIndex += pageSize)
            {
                string query = $"Items?ParentId={Uri.EscapeDataString(libraryId)}&Recursive=true" +
                    $"&IncludeItemTypes=Movie,Series&Fields=Path&StartIndex={startIndex}&Limit={pageSize}";
                using HttpRequestMessage itemsRequest = CreateRequest(HttpMethod.Get, query);
                using HttpResponseMessage itemsResponse = await httpClient
                    .SendAsync(itemsRequest, cancellationToken)
                    .ConfigureAwait(false);
                itemsResponse.EnsureSuccessStatusCode();

                await using Stream itemsStream = await itemsResponse.Content
                    .ReadAsStreamAsync(cancellationToken)
                    .ConfigureAwait(false);
                using JsonDocument itemsDocument = await JsonDocument
                    .ParseAsync(itemsStream, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);

                if (!itemsDocument.RootElement.TryGetProperty("Items", out JsonElement items))
                {
                    break;
                }

                foreach (JsonElement item in items.EnumerateArray())
                {
                    string? itemId = GetString(item, "Id");
                    string? name = GetString(item, "Name");
                    if (!string.IsNullOrWhiteSpace(itemId) && !string.IsNullOrWhiteSpace(name))
                    {
                        works.Add(new ObservedLibraryWork(libraryId, itemId, name));
                    }
                }

                int totalRecordCount = GetInt(itemsDocument.RootElement, "TotalRecordCount") ??
                    startIndex + items.GetArrayLength();
                if (startIndex + items.GetArrayLength() >= totalRecordCount || items.GetArrayLength() == 0)
                {
                    break;
                }
            }
        }

        return works;
    }

    private HttpRequestMessage CreateRequest(HttpMethod method, string relativePath)
    {
        HttpRequestMessage request = new(method, relativePath);
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.TryAddWithoutValidation("X-Emby-Token", accessToken);
            request.Headers.Authorization = new AuthenticationHeaderValue("MediaBrowser");
        }

        return request;
    }

    private static string? GetString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out JsonElement property) &&
        property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

    private static int? GetInt(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out JsonElement property) &&
        property.ValueKind == JsonValueKind.Number &&
        property.TryGetInt32(out int value)
            ? value
            : null;
}
