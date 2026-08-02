using Guardian.Domain.Identity;

namespace Guardian.Application.Persistence;

public sealed record EditorialMemorySnapshot(
    IReadOnlyList<IdentityDecision> Decisions,
    IReadOnlyList<IdentityValidationHistoryEvent> HistoryEvents,
    IReadOnlyDictionary<string, string> Metadata)
{
    public static EditorialMemorySnapshot Empty { get; } =
        new([], [], new Dictionary<string, string>());
}

public interface IEditorialMemoryStore
{
    Task SaveAsync(
        IdentityDecision decision,
        IdentityValidationHistoryEvent historyEvent,
        IReadOnlyDictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

    Task<EditorialMemorySnapshot> LoadAsync(CancellationToken cancellationToken = default);
}
