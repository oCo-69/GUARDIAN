using Guardian.Application.LibraryObservation;
using Guardian.Domain.Identity;

namespace Guardian.Application.CandidateDiscovery;

public interface ICandidateDiscoverySource
{
    Task<CandidateDiscoverySourceResult> DiscoverAsync(
        ObservedSourceWork sourceWork,
        CancellationToken cancellationToken = default);
}

public sealed record CandidateDiscoverySourceResult(
    IReadOnlyList<DiscoveredCandidate> Candidates,
    CandidateDiscoverySourceStatus Status = CandidateDiscoverySourceStatus.Completed,
    CandidateDiscoveryFailure Failure = CandidateDiscoveryFailure.None)
{
    public static CandidateDiscoverySourceResult Empty() =>
        new([], CandidateDiscoverySourceStatus.NoCandidates);

    public static CandidateDiscoverySourceResult Failed(CandidateDiscoveryFailure failure) =>
        new([], CandidateDiscoverySourceStatus.Failed, failure);
}

public sealed record DiscoveredCandidate(
    CandidateId CandidateId,
    WorkId ProposedWorkId,
    string Title,
    int? Year,
    string? MediaType,
    ProviderIdentity? ProviderEvidence,
    CandidateProvenance Provenance);

public enum CandidateDiscoverySourceStatus
{
    Completed,
    NoCandidates,
    Failed,
}

public enum CandidateDiscoveryFailure
{
    None,
    SourceUnavailable,
    SourceRejected,
}
