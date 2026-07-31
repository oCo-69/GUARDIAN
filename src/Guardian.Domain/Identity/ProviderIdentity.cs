namespace Guardian.Domain.Identity;

public sealed record ProviderIdentity
{
    public ProviderIdentity(string provider, string mediaType, string externalId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(provider);
        ArgumentException.ThrowIfNullOrWhiteSpace(mediaType);
        ArgumentException.ThrowIfNullOrWhiteSpace(externalId);

        Provider = provider;
        MediaType = mediaType;
        ExternalId = externalId;
    }

    public string Provider { get; }

    public string MediaType { get; }

    public string ExternalId { get; }
}
