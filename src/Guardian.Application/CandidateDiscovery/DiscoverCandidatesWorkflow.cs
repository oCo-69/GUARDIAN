using Guardian.Application.LibraryObservation;
using Guardian.Domain.Identity;

namespace Guardian.Application.CandidateDiscovery;

public sealed class DiscoverCandidatesWorkflow
{
    private readonly ICandidateDiscoverySource source;

    public DiscoverCandidatesWorkflow(ICandidateDiscoverySource source)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public async Task<DiscoverCandidatesResult> DiscoverAsync(
        ObservedSourceWork sourceWork,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(sourceWork);

        CandidateDiscoverySourceResult sourceResult = await source
            .DiscoverAsync(sourceWork, cancellationToken)
            .ConfigureAwait(false);

        if (sourceResult.Status == CandidateDiscoverySourceStatus.Failed)
        {
            return new DiscoverCandidatesResult(
                DiscoverCandidatesStatus.Failed,
                [],
                sourceResult.Failure);
        }

        IdentityCandidate[] candidates = sourceResult.Candidates
            .Select(candidate => new IdentityCandidate(
                candidate.CandidateId,
                sourceWork.Id,
                candidate.ProposedWorkId,
                candidate.ProviderEvidence,
                candidate.Title,
                candidate.Year,
                candidate.MediaType,
                candidate.Provenance))
            .ToArray();

        return new DiscoverCandidatesResult(
            candidates.Length == 0
                ? DiscoverCandidatesStatus.NoCandidates
                : DiscoverCandidatesStatus.Completed,
            candidates,
            CandidateDiscoveryFailure.None);
    }
}

public sealed record DiscoverCandidatesResult(
    DiscoverCandidatesStatus Status,
    IReadOnlyList<IdentityCandidate> Candidates,
    CandidateDiscoveryFailure Failure)
{
    public bool HasCandidates => Candidates.Count > 0;
}

public enum DiscoverCandidatesStatus
{
    Completed,
    NoCandidates,
    Failed,
}
