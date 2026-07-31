using Guardian.Application.Identity;
using Guardian.Domain.Identity;

namespace Guardian.Tests.Identity;

public sealed class InitialIdentityCorrespondenceTests
{
    private static readonly SourceWorkId SourceId = new(new Guid("10000000-0000-0000-0000-000000000001"));
    private static readonly SourceWorkId OtherSourceId = new(new Guid("10000000-0000-0000-0000-000000000002"));
    private static readonly WorkId WorkId = new(new Guid("20000000-0000-0000-0000-000000000001"));
    private static readonly WorkId OtherWorkId = new(new Guid("20000000-0000-0000-0000-000000000002"));
    private static readonly CandidateId CandidateId = new(new Guid("30000000-0000-0000-0000-000000000001"));
    private static readonly CandidateId OtherCandidateId = new(new Guid("30000000-0000-0000-0000-000000000002"));
    private static readonly DecisionId DecisionId = new(new Guid("40000000-0000-0000-0000-000000000001"));
    private static readonly HistoryEventId EventId = new(new Guid("50000000-0000-0000-0000-000000000001"));
    private static readonly DateTimeOffset ValidationTime = new(2026, 7, 31, 12, 0, 0, TimeSpan.Zero);
    private static readonly ProviderIdentity ProviderEvidence = new("TMDb", "Series", "42");
    private static readonly EditorialAuthority Authority = new("human-reviewer");

    [Fact]
    public void CandidateAloneLeavesCorrespondenceUnknown()
    {
        IdentityCorrespondenceWorkflow workflow = CreateWorkflow();

        AcceptedCorrespondenceKnowledge knowledge = workflow.GetKnowledge(SourceId);

        Assert.False(knowledge.IsKnown);
        Assert.Null(knowledge.AcceptedWorkId);
        Assert.Null(knowledge.SupportingDecisionId);
        Assert.Empty(workflow.Decisions);
        Assert.Empty(workflow.HistoryEvents);
    }

    [Fact]
    public void ProviderIdentityAloneLeavesCorrespondenceUnknown()
    {
        IdentityCandidate candidate = new(CandidateId, SourceId, WorkId, ProviderEvidence);
        IdentityCorrespondenceWorkflow workflow = CreateWorkflow(candidate);

        AcceptedCorrespondenceKnowledge knowledge = workflow.GetKnowledge(SourceId);

        Assert.False(knowledge.IsKnown);
        Assert.Empty(workflow.Decisions);
    }

    [Fact]
    public void ExplicitValidationEstablishesOneExplainableKnownCorrespondence()
    {
        IdentityCorrespondenceWorkflow workflow = CreateWorkflow();

        IdentityValidationResult result = workflow.Validate(
            new IdentityValidationRequest(SourceId, CandidateId, Authority));

        Assert.Equal(IdentityValidationStatus.Established, result.Status);
        Assert.True(result.Changed);
        IdentityDecision decision = Assert.Single(workflow.Decisions);
        IdentityValidationHistoryEvent historyEvent = Assert.Single(workflow.HistoryEvents);
        Assert.Same(decision, result.Decision);
        Assert.Same(historyEvent, result.HistoryEvent);
        Assert.Equal(
            AcceptedCorrespondenceEvaluationStatus.Evaluated,
            workflow.EvaluateKnowledge(SourceId).Status);
        Assert.Equal(SourceId, decision.SourceWorkId);
        Assert.Equal(WorkId, decision.AcceptedWorkId);
        Assert.Equal(DecisionId, decision.Id);
        Assert.Equal(decision.Id, historyEvent.DecisionId);
        Assert.Equal(EventId, historyEvent.Id);
        Assert.NotEqual(decision.Id.Value, historyEvent.Id.Value);
        Assert.True(result.Knowledge.IsKnown);
        Assert.Equal(SourceId, result.Knowledge.SourceWorkId);
        Assert.Equal(WorkId, result.Knowledge.AcceptedWorkId);
        Assert.Equal(decision.Id, result.Knowledge.SupportingDecisionId);
    }

    [Fact]
    public void ExplicitValidationWithoutProviderEvidenceEstablishesKnownCorrespondence()
    {
        IdentityCandidate candidate = new(CandidateId, SourceId, WorkId, null);
        IdentityCorrespondenceWorkflow workflow = CreateWorkflow(candidate);

        IdentityValidationResult result = workflow.Validate(
            new IdentityValidationRequest(SourceId, CandidateId, Authority));

        Assert.Equal(IdentityValidationStatus.Established, result.Status);
        IdentityDecision decision = Assert.Single(workflow.Decisions);
        IdentityValidationHistoryEvent historyEvent = Assert.Single(workflow.HistoryEvents);
        Assert.Null(decision.ProviderEvidence);
        Assert.Equal(SourceId, decision.SourceWorkId);
        Assert.Equal(WorkId, decision.AcceptedWorkId);
        Assert.Equal(decision.Id, result.Knowledge.SupportingDecisionId);
        Assert.True(result.Knowledge.IsKnown);
        Assert.Equal(WorkId, result.Knowledge.AcceptedWorkId);
        Assert.Equal(decision.Id, historyEvent.DecisionId);
    }

    [Fact]
    public void AcceptedWorkIsNotReplacedByProviderIdentity()
    {
        IdentityCorrespondenceWorkflow workflow = CreateWorkflow();

        IdentityValidationResult result = workflow.Validate(
            new IdentityValidationRequest(SourceId, CandidateId, Authority));

        Assert.Equal(WorkId, result.Decision!.AcceptedWorkId);
        Assert.Equal(ProviderEvidence, result.Decision.ProviderEvidence);
        Assert.NotEqual(
            result.Decision.AcceptedWorkId.Value.ToString(),
            result.Decision.ProviderEvidence!.ExternalId);
    }

    [Fact]
    public void ExactRepeatIsNoChangeAndCreatesNoRecords()
    {
        IdentityCorrespondenceWorkflow workflow = CreateWorkflow();
        IdentityValidationRequest request = new(SourceId, CandidateId, Authority);
        IdentityValidationResult first = workflow.Validate(request);

        IdentityValidationResult repeated = workflow.Validate(request);

        Assert.Equal(IdentityValidationStatus.NoChange, repeated.Status);
        Assert.False(repeated.Changed);
        Assert.Same(first.Decision, repeated.Decision);
        Assert.Null(repeated.HistoryEvent);
        Assert.Single(workflow.Decisions);
        Assert.Single(workflow.HistoryEvents);
        Assert.Equal(first.Knowledge, repeated.Knowledge);
    }

    [Fact]
    public void MissingSourceWorkIsRejectedWithoutPartialAuthority()
    {
        IdentityCandidate candidate = new(CandidateId, OtherSourceId, WorkId, ProviderEvidence);
        IdentityCorrespondenceWorkflow workflow = CreateWorkflow(candidate);

        IdentityValidationResult result = workflow.Validate(
            new IdentityValidationRequest(OtherSourceId, CandidateId, Authority));

        AssertRejectedWithoutRecords(result, workflow, IdentityValidationFailure.SourceWorkNotFound);
    }

    [Fact]
    public void MissingCandidateIsRejectedWithoutPartialAuthority()
    {
        IdentityCorrespondenceWorkflow workflow = CreateWorkflow();

        IdentityValidationResult result = workflow.Validate(
            new IdentityValidationRequest(SourceId, OtherCandidateId, Authority));

        AssertRejectedWithoutRecords(result, workflow, IdentityValidationFailure.CandidateNotFound);
    }

    [Fact]
    public void CandidateForAnotherSourceWorkIsRejectedAndIsNotAnExactRepeat()
    {
        IdentityCandidate mismatched = new(OtherCandidateId, OtherSourceId, WorkId, ProviderEvidence);
        IdentityCorrespondenceWorkflow workflow = CreateWorkflow(additionalCandidate: mismatched);

        IdentityValidationResult result = workflow.Validate(
            new IdentityValidationRequest(SourceId, OtherCandidateId, Authority));

        AssertRejectedWithoutRecords(
            result,
            workflow,
            IdentityValidationFailure.CandidateSourceWorkMismatch);
    }

    [Fact]
    public void CandidateWithoutWorkIsRejectedEvenWhenProviderEvidenceExists()
    {
        IdentityCandidate invalid = new(CandidateId, SourceId, default, ProviderEvidence);
        IdentityCorrespondenceWorkflow workflow = CreateWorkflow(invalid);

        IdentityValidationResult result = workflow.Validate(
            new IdentityValidationRequest(SourceId, CandidateId, Authority));

        AssertRejectedWithoutRecords(
            result,
            workflow,
            IdentityValidationFailure.CandidateDoesNotIdentifyWork);
    }

    [Fact]
    public void MissingAuthorityIsRejectedWithoutPartialAuthority()
    {
        IdentityCorrespondenceWorkflow workflow = CreateWorkflow();

        IdentityValidationResult result = workflow.Validate(
            new IdentityValidationRequest(SourceId, CandidateId, null));

        AssertRejectedWithoutRecords(result, workflow, IdentityValidationFailure.InvalidAuthority);
    }

    [Fact]
    public void InvalidAuthorityCannotBeConstructed()
    {
        Assert.Throws<ArgumentException>(() => new EditorialAuthority(" "));
    }

    [Fact]
    public void DifferentCandidateRequiresIncrementBAndPreservesHistory()
    {
        IdentityCandidate revisionCandidate = new(
            OtherCandidateId,
            SourceId,
            OtherWorkId,
            new ProviderIdentity("TMDb", "Series", "84"));
        IdentityCorrespondenceWorkflow workflow = CreateWorkflow(additionalCandidate: revisionCandidate);
        IdentityValidationResult first = workflow.Validate(
            new IdentityValidationRequest(SourceId, CandidateId, Authority));

        IdentityValidationResult revision = workflow.Validate(
            new IdentityValidationRequest(SourceId, OtherCandidateId, Authority));

        Assert.Equal(IdentityValidationStatus.RequiresSupersession, revision.Status);
        Assert.False(revision.Changed);
        Assert.Same(first.Decision, revision.Decision);
        Assert.Null(revision.HistoryEvent);
        Assert.Single(workflow.Decisions);
        Assert.Single(workflow.HistoryEvents);
        Assert.Equal(WorkId, revision.Knowledge.AcceptedWorkId);
    }

    [Fact]
    public void IdentityDecisionHasNoIntrinsicApplicabilityOrPublicMutationPath()
    {
        IdentityCorrespondenceWorkflow workflow = CreateWorkflow();
        IdentityDecision original = workflow.Validate(
            new IdentityValidationRequest(SourceId, CandidateId, Authority)).Decision!;

        Assert.Null(typeof(IdentityDecision).GetProperty("IsApplicable"));
        Assert.Empty(typeof(IdentityDecision).GetConstructors());
        Assert.All(
            typeof(IdentityDecision).GetProperties(),
            property => Assert.False(property.CanWrite));
        Assert.Equal(WorkId, original.AcceptedWorkId);
        Assert.Same(original, Assert.Single(workflow.Decisions));
    }

    [Fact]
    public void ExposedDecisionAndHistoryCollectionsCannotMutateRetainedRecords()
    {
        IdentityCorrespondenceWorkflow workflow = CreateWorkflow();
        workflow.Validate(new IdentityValidationRequest(SourceId, CandidateId, Authority));
        IList<IdentityDecision> decisions = Assert.IsAssignableFrom<IList<IdentityDecision>>(
            workflow.Decisions);
        IList<IdentityValidationHistoryEvent> historyEvents =
            Assert.IsAssignableFrom<IList<IdentityValidationHistoryEvent>>(workflow.HistoryEvents);

        Assert.Throws<NotSupportedException>(() => decisions.Clear());
        Assert.Throws<NotSupportedException>(() => historyEvents.Clear());
        Assert.Single(workflow.Decisions);
        Assert.Single(workflow.HistoryEvents);
    }

    [Fact]
    public void ZeroApplicableDecisionsProducesUnknown()
    {
        AcceptedCorrespondenceEvaluation evaluation = AcceptedCorrespondenceEvaluator.Evaluate(
            SourceId,
            []);

        Assert.Equal(AcceptedCorrespondenceEvaluationStatus.Evaluated, evaluation.Status);
        Assert.False(evaluation.Knowledge.IsKnown);
    }

    [Fact]
    public void ExactlyOneApplicableDecisionProducesKnown()
    {
        IdentityDecision decision = CreateValidatedDecision(
            SourceId,
            CandidateId,
            WorkId,
            ProviderEvidence);

        AcceptedCorrespondenceEvaluation evaluation = AcceptedCorrespondenceEvaluator.Evaluate(
            SourceId,
            [decision]);

        Assert.Equal(AcceptedCorrespondenceEvaluationStatus.Evaluated, evaluation.Status);
        Assert.True(evaluation.Knowledge.IsKnown);
        Assert.Equal(WorkId, evaluation.Knowledge.AcceptedWorkId);
        Assert.Equal(decision.Id, evaluation.Knowledge.SupportingDecisionId);
    }

    [Fact]
    public void MultipleApplicableDecisionsAreUnsupportedRegardlessOfOrder()
    {
        IdentityDecision first = CreateValidatedDecision(SourceId, CandidateId, WorkId, ProviderEvidence);
        IdentityDecision second = CreateValidatedDecision(SourceId, OtherCandidateId, OtherWorkId, null);

        AcceptedCorrespondenceEvaluation forward = AcceptedCorrespondenceEvaluator.Evaluate(
            SourceId,
            [first, second]);
        AcceptedCorrespondenceEvaluation reversed = AcceptedCorrespondenceEvaluator.Evaluate(
            SourceId,
            [second, first]);

        Assert.Equal(
            AcceptedCorrespondenceEvaluationStatus.UnsupportedMultipleApplicableDecisions,
            forward.Status);
        Assert.Equal(forward.Status, reversed.Status);
        Assert.False(forward.Knowledge.IsKnown);
        Assert.Equal(forward.Knowledge, reversed.Knowledge);
    }

    [Fact]
    public void UnrelatedCollectionOrderDoesNotChangeSingleApplicableOutcome()
    {
        IdentityDecision relevant = CreateValidatedDecision(SourceId, CandidateId, WorkId, ProviderEvidence);
        IdentityDecision unrelated = CreateValidatedDecision(OtherSourceId, OtherCandidateId, OtherWorkId, null);

        AcceptedCorrespondenceEvaluation forward = AcceptedCorrespondenceEvaluator.Evaluate(
            SourceId,
            [relevant, unrelated]);
        AcceptedCorrespondenceEvaluation reversed = AcceptedCorrespondenceEvaluator.Evaluate(
            SourceId,
            [unrelated, relevant]);

        Assert.Equal(AcceptedCorrespondenceEvaluationStatus.Evaluated, forward.Status);
        Assert.Equal(forward.Knowledge, reversed.Knowledge);
        Assert.Equal(relevant.Id, forward.Knowledge.SupportingDecisionId);
    }

    [Fact]
    public void KnownCannotBeConstructedThroughPublicApi()
    {
        Assert.Empty(typeof(AcceptedCorrespondenceKnowledge).GetConstructors());
        Assert.DoesNotContain(
            typeof(AcceptedCorrespondenceKnowledge).GetMethods(),
            method => method.IsPublic && method.IsStatic && method.Name == "Known");
    }

    [Fact]
    public void DuplicateDecisionIdentityCannotRecordAlteredEditorialContent()
    {
        IdentityCandidate firstCandidate = new(CandidateId, SourceId, WorkId, ProviderEvidence);
        IdentityCandidate secondCandidate = new(OtherCandidateId, OtherSourceId, OtherWorkId, null);
        IdentityCorrespondenceWorkflow workflow = new(
            [new SourceWork(SourceId), new SourceWork(OtherSourceId)],
            [firstCandidate, secondCandidate],
            () => DecisionId,
            () => EventId,
            () => ValidationTime);
        workflow.Validate(new IdentityValidationRequest(SourceId, CandidateId, Authority));

        Assert.Throws<InvalidOperationException>(() => workflow.Validate(
            new IdentityValidationRequest(OtherSourceId, OtherCandidateId, Authority)));
        IdentityDecision retained = Assert.Single(workflow.Decisions);
        Assert.Equal(SourceId, retained.SourceWorkId);
        Assert.Equal(WorkId, retained.AcceptedWorkId);
        Assert.Single(workflow.HistoryEvents);
        Assert.False(workflow.GetKnowledge(OtherSourceId).IsKnown);
    }

    private static IdentityCorrespondenceWorkflow CreateWorkflow(
        IdentityCandidate? candidate = null,
        IdentityCandidate? additionalCandidate = null)
    {
        List<IdentityCandidate> candidates =
        [
            candidate ?? new IdentityCandidate(CandidateId, SourceId, WorkId, ProviderEvidence),
        ];

        if (additionalCandidate is not null)
        {
            candidates.Add(additionalCandidate);
        }

        return new IdentityCorrespondenceWorkflow(
            [new SourceWork(SourceId)],
            candidates,
            () => DecisionId,
            () => EventId,
            () => ValidationTime);
    }

    private static void AssertRejectedWithoutRecords(
        IdentityValidationResult result,
        IdentityCorrespondenceWorkflow workflow,
        IdentityValidationFailure failure)
    {
        Assert.Equal(IdentityValidationStatus.Rejected, result.Status);
        Assert.Equal(failure, result.Failure);
        Assert.False(result.Changed);
        Assert.Null(result.Decision);
        Assert.Null(result.HistoryEvent);
        Assert.Empty(workflow.Decisions);
        Assert.Empty(workflow.HistoryEvents);
        Assert.False(result.Knowledge.IsKnown);
    }

    private static IdentityDecision CreateValidatedDecision(
        SourceWorkId sourceWorkId,
        CandidateId candidateId,
        WorkId workId,
        ProviderIdentity? providerEvidence)
    {
        IdentityCorrespondenceWorkflow workflow = new(
            [new SourceWork(sourceWorkId)],
            [new IdentityCandidate(candidateId, sourceWorkId, workId, providerEvidence)],
            () => DecisionId.New(),
            () => HistoryEventId.New(),
            () => ValidationTime);

        return workflow.Validate(new IdentityValidationRequest(sourceWorkId, candidateId, Authority)).Decision!;
    }
}
