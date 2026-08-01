namespace Guardian.Application.LibraryObservation;

public interface ILibraryObservationSource
{
    Task<IReadOnlyList<ObservedLibraryWork>> ReadWorksAsync(CancellationToken cancellationToken = default);
}

public sealed record ObservedLibraryWork(
    string LibraryId,
    string ExternalId,
    string Name);
