namespace Guardian.Domain.Identity;

public sealed record EditorialAuthority
{
    public EditorialAuthority(string actorId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(actorId);
        ActorId = actorId;
    }

    public string ActorId { get; }
}
