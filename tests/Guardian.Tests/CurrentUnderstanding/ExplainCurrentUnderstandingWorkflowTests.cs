using Guardian.Application.CurrentUnderstanding;
using Guardian.Application.Identity;
using Guardian.Domain.Identity;

namespace Guardian.Tests.CurrentUnderstanding;

public sealed class ExplainCurrentUnderstandingWorkflowTests
{
    private static readonly SourceWorkId SourceWorkId =
        new(new Guid("10000000-0000-0000-0000-000000000001"));
    private static readonly WorkId WorkId =
        new(new Guid("20000000-0000-0000-0000-000000000001"));
    private static readonly CandidateId CandidateId =
        new(new Guid("30000000-0000-0000-0000-000000000001"));
    private static readonly DecisionId DecisionId =
        new(new Guid("40000000-0000-0000-0000-000000000001"));
    private static readonly HistoryEventId HistoryEventId =
        new(new Guid("50000000-0000-0000-0000-000000000001"));
    private static readonly DateTimeOffset DecisionTime =
        new(2026, 8, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void ExplainsKnownCurrentUnderstandingAndItsEditorialEvidence()
    {
        IdentityCorrespondenceWorkflow correspondence = CreateWorkflow();
        correspondence.Validate(new IdentityValidationRequest(
            SourceWorkId,
            CandidateId,
            new EditorialAuthority("human-reviewer")));

        CurrentUnderstandingExplanation explanation =
            new ExplainCurrentUnderstandingWorkflow(correspondence).Explain(SourceWorkId);

        Assert.True(explanation.IsKnown);
        Assert.Equal(WorkId, explanation.Knowledge.AcceptedWorkId);
        Assert.Single(explanation.Decisions);
        Assert.Single(explanation.HistoryEvents);
        Assert.NotNull(explanation.SupportingDecision);
        Assert.Equal(DecisionId, explanation.SupportingDecision!.Id);
        Assert.Equal("human-reviewer", explanation.SupportingDecision.Authority.ActorId);
        Assert.Equal(DecisionTime, explanation.SupportingDecision.DecidedAt);
        Assert.Equal(
            explanation.SupportingDecision.Id,
            explanation.HistoryEvents.Single().DecisionId);
        Assert.Equal(
            "TMDb",
            explanation.SupportingDecision.ProviderEvidence!.Provider);
    }

    [Fact]
    public void ExplainsUnknownUnderstandingWithoutEditorialEvidence()
    {
        IdentityCorrespondenceWorkflow correspondence = CreateWorkflow();

        CurrentUnderstandingExplanation explanation =
            new ExplainCurrentUnderstandingWorkflow(correspondence).Explain(SourceWorkId);

        Assert.False(explanation.IsKnown);
        Assert.Null(explanation.Knowledge.AcceptedWorkId);
        Assert.Null(explanation.SupportingDecision);
        Assert.Empty(explanation.Decisions);
        Assert.Empty(explanation.HistoryEvents);
    }

    [Fact]
    public void ExplanationRetainsHistoricalRecordsWithoutChangingThem()
    {
        IdentityCorrespondenceWorkflow correspondence = CreateWorkflow();
        correspondence.Validate(new IdentityValidationRequest(
            SourceWorkId,
            CandidateId,
            new EditorialAuthority("human-reviewer")));

        CurrentUnderstandingExplanation explanation =
            new ExplainCurrentUnderstandingWorkflow(correspondence).Explain(SourceWorkId);

        Assert.Equal(correspondence.Decisions, explanation.Decisions);
        Assert.Equal(correspondence.HistoryEvents, explanation.HistoryEvents);
    }

    private static IdentityCorrespondenceWorkflow CreateWorkflow()
    {
        IdentityCandidate candidate = new(
            CandidateId,
            SourceWorkId,
            WorkId,
            new ProviderIdentity("TMDb", "Series", "42"),
            "Example Series",
            2024,
            "Series",
            new CandidateProvenance("deterministic-test", "candidate-1"));

        return new IdentityCorrespondenceWorkflow(
            [new SourceWork(SourceWorkId)],
            [candidate],
            () => DecisionId,
            () => HistoryEventId,
            () => DecisionTime);
    }
}
