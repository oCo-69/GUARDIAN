namespace Guardian.Domain.Identity;

public sealed class IdentityDecision
{
    public const string Category = "Identity";

    public const string Scope = "AcceptedCorrespondence";

    internal IdentityDecision(
        DecisionId id,
        SourceWorkId sourceWorkId,
        WorkId acceptedWorkId,
        CandidateId candidateId,
        EditorialAuthority authority,
        DateTimeOffset decidedAt,
        ProviderIdentity? providerEvidence,
        DecisionId? supersedesDecisionId = null)
    {
        Id = id;
        SourceWorkId = sourceWorkId;
        AcceptedWorkId = acceptedWorkId;
        CandidateId = candidateId;
        Authority = authority;
        DecidedAt = decidedAt;
        ProviderEvidence = providerEvidence;
        SupersedesDecisionId = supersedesDecisionId;
    }

    internal static IdentityDecision Rehydrate(
        DecisionId id,
        SourceWorkId sourceWorkId,
        WorkId acceptedWorkId,
        CandidateId candidateId,
        EditorialAuthority authority,
        DateTimeOffset decidedAt,
        ProviderIdentity? providerEvidence,
        DecisionId? supersedesDecisionId) =>
        new(id, sourceWorkId, acceptedWorkId, candidateId, authority, decidedAt, providerEvidence, supersedesDecisionId);

    public DecisionId Id { get; }

    public SourceWorkId SourceWorkId { get; }

    public WorkId AcceptedWorkId { get; }

    public CandidateId CandidateId { get; }

    public EditorialAuthority Authority { get; }

    public DateTimeOffset DecidedAt { get; }

    public ProviderIdentity? ProviderEvidence { get; }

    public DecisionId? SupersedesDecisionId { get; }
}
