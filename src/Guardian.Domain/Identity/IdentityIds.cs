namespace Guardian.Domain.Identity;

public readonly record struct SourceWorkId(Guid Value)
{
    public static SourceWorkId New() => new(Guid.NewGuid());
}

public readonly record struct WorkId(Guid Value)
{
    public static WorkId New() => new(Guid.NewGuid());
}

public readonly record struct CandidateId(Guid Value)
{
    public static CandidateId New() => new(Guid.NewGuid());
}

public readonly record struct DecisionId(Guid Value)
{
    public static DecisionId New() => new(Guid.NewGuid());
}

public readonly record struct HistoryEventId(Guid Value)
{
    public static HistoryEventId New() => new(Guid.NewGuid());
}
