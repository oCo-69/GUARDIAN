using Guardian.Domain.Identity;

namespace Guardian.Application.LibraryObservation;

public sealed class ObserveLibraryWorkflow
{
    private readonly ILibraryObservationSource source;

    public ObserveLibraryWorkflow(ILibraryObservationSource source)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
    }

    public async Task<LibraryObservationResult> ObserveAsync(
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ObservedLibraryWork> observedWorks =
            await source.ReadWorksAsync(cancellationToken).ConfigureAwait(false);

        ObservedSourceWork[] sourceWorks = observedWorks
            .Select(work => new ObservedSourceWork(
                SourceWorkId.FromStableReference($"{work.LibraryId}:{work.ExternalId}"),
                work.LibraryId,
                work.ExternalId,
                work.Name))
            .DistinctBy(work => work.SourceWork.Id)
            .ToArray();

        return new LibraryObservationResult(sourceWorks);
    }
}

public sealed record ObservedSourceWork(
    SourceWorkId Id,
    string LibraryId,
    string ExternalId,
    string Name)
{
    public SourceWork SourceWork => new(Id);
}

public sealed record LibraryObservationResult(IReadOnlyList<ObservedSourceWork> SourceWorks)
{
    public int Count => SourceWorks.Count;
}
