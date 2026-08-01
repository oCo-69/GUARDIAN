namespace Guardian.Domain.Identity;

public sealed record IdentityCandidate(
    CandidateId Id,
    SourceWorkId SourceWorkId,
    WorkId WorkId,
    ProviderIdentity? ProviderEvidence,
    string? Title = null,
    int? Year = null,
    string? MediaType = null,
    CandidateProvenance? Provenance = null);

public sealed record CandidateProvenance
{
    public CandidateProvenance(string discoverySource, string? sourceReference = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(discoverySource);
        DiscoverySource = discoverySource;
        SourceReference = sourceReference;
    }

    public string DiscoverySource { get; }

    public string? SourceReference { get; }

}
