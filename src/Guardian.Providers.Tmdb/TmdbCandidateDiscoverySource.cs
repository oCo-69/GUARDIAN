using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Guardian.Application.CandidateDiscovery;
using Guardian.Application.LibraryObservation;
using Guardian.Domain.Identity;

namespace Guardian.Providers.Tmdb;

public sealed class TmdbCandidateDiscoverySource : ICandidateDiscoverySource
{
    private readonly HttpClient httpClient;
    private readonly string accessToken;

    public TmdbCandidateDiscoverySource(HttpClient httpClient, string accessToken)
    {
        this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);
        this.accessToken = accessToken;
    }

    public async Task<CandidateDiscoverySourceResult> DiscoverAsync(
        ObservedSourceWork sourceWork,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceWork);

        List<DiscoveredCandidate> candidates = [];
        foreach ((string path, string mediaType) in SearchEndpoints(sourceWork.Name))
        {
            CandidateDiscoverySourceResult result = await SearchAsync(
                sourceWork,
                path,
                mediaType,
                cancellationToken).ConfigureAwait(false);

            if (result.Status == CandidateDiscoverySourceStatus.Failed)
            {
                return result;
            }

            candidates.AddRange(result.Candidates);
        }

        return candidates.Count == 0
            ? CandidateDiscoverySourceResult.Empty()
            : new CandidateDiscoverySourceResult(candidates);
    }

    private async Task<CandidateDiscoverySourceResult> SearchAsync(
        ObservedSourceWork sourceWork,
        string path,
        string mediaType,
        CancellationToken cancellationToken)
    {
        using HttpRequestMessage request = new(HttpMethod.Get, path);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        using HttpResponseMessage response = await httpClient
            .SendAsync(request, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            CandidateDiscoveryFailure failure = response.StatusCode is
                HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden
                ? CandidateDiscoveryFailure.SourceRejected
                : CandidateDiscoveryFailure.SourceUnavailable;
            return CandidateDiscoverySourceResult.Failed(failure);
        }

        try
        {
            await using Stream stream = await response.Content
                .ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);
            using JsonDocument document = await JsonDocument
                .ParseAsync(stream, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            if (!document.RootElement.TryGetProperty("results", out JsonElement results))
            {
                return CandidateDiscoverySourceResult.Failed(
                    CandidateDiscoveryFailure.SourceUnavailable);
            }

            DiscoveredCandidate[] candidates = results
                .EnumerateArray()
                .Select((result, index) => ParseCandidate(sourceWork, result, mediaType, path, index))
                .Where(candidate => candidate is not null)
                .Select(candidate => candidate!)
                .ToArray();

            return candidates.Length == 0
                ? CandidateDiscoverySourceResult.Empty()
                : new CandidateDiscoverySourceResult(candidates);
        }
        catch (JsonException)
        {
            return CandidateDiscoverySourceResult.Failed(
                CandidateDiscoveryFailure.SourceUnavailable);
        }
    }

    private static DiscoveredCandidate? ParseCandidate(
        ObservedSourceWork sourceWork,
        JsonElement result,
        string mediaType,
        string endpoint,
        int index)
    {
        if (!result.TryGetProperty("id", out JsonElement idProperty) ||
            !idProperty.TryGetInt32(out int externalId))
        {
            return null;
        }

        string? title = GetString(result, mediaType == "Series" ? "name" : "title");
        if (string.IsNullOrWhiteSpace(title))
        {
            return null;
        }

        string? date = GetString(result, mediaType == "Series" ? "first_air_date" : "release_date");
        int? year = int.TryParse(date?[..Math.Min(4, date.Length)], out int parsedYear)
            ? parsedYear
            : null;
        string externalReference = externalId.ToString(System.Globalization.CultureInfo.InvariantCulture);
        string candidateReference = $"tmdb:candidate:{mediaType}:{externalReference}:{sourceWork.Id.Value}";
        string workReference = $"tmdb:work:{mediaType}:{externalReference}";

        return new DiscoveredCandidate(
            new CandidateId(StableGuid(candidateReference)),
            new WorkId(StableGuid(workReference)),
            title,
            year,
            mediaType,
            new ProviderIdentity("TMDb", mediaType, externalReference),
            new CandidateProvenance("TMDb", endpoint));
    }

    private static IEnumerable<(string Path, string MediaType)> SearchEndpoints(string query)
    {
        string encodedQuery = Uri.EscapeDataString(query);
        yield return ($"3/search/tv?query={encodedQuery}&include_adult=false&language=en-US&page=1", "Series");
        yield return ($"3/search/movie?query={encodedQuery}&include_adult=false&language=en-US&page=1", "Movie");
    }

    private static string? GetString(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out JsonElement property) &&
        property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;

    private static Guid StableGuid(string value)
    {
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return new Guid(hash.AsSpan(0, 16));
    }
}
