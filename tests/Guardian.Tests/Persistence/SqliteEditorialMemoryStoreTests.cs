using Guardian.Application.CandidateReview;
using Guardian.Application.CurrentUnderstanding;
using Guardian.Application.EditorialDecision;
using Guardian.Application.Identity;
using Guardian.Application.LibraryObservation;
using Guardian.Application.Persistence;
using Guardian.Domain.Identity;
using Guardian.Infrastructure;
using Microsoft.Data.Sqlite;

namespace Guardian.Tests.Persistence;

public sealed class SqliteEditorialMemoryStoreTests
{
    [Fact]
    public async Task DecisionAndHistoryRoundTripAndKnowledgeIsReconstructed()
    {
        string path = Path.Combine(Path.GetTempPath(), $"guardian-{Guid.NewGuid():N}.db");
        try
        {
            (CandidateReviewContext review, IdentityCandidate candidate) = CreateReview();
            AcceptEditorialDecisionWorkflow acceptance = new(review,
                () => new DecisionId(Guid.Parse("70000000-0000-0000-0000-000000000001")),
                () => new HistoryEventId(Guid.Parse("80000000-0000-0000-0000-000000000001")),
                () => new DateTimeOffset(2026, 8, 2, 10, 0, 0, TimeSpan.Zero));
            EditorialDecisionAcceptanceResult accepted = acceptance.Accept(
                candidate.Id,
                new EditorialAuthority("editor"));

            IdentityDecision decision = accepted.Decision ?? throw new InvalidOperationException();
            SqliteEditorialMemoryStore store = new(path);
            await store.SaveAsync(decision, accepted.HistoryEvent!);
            EditorialMemorySnapshot snapshot = await store.LoadAsync();
            IdentityCorrespondenceWorkflow restored = IdentityCorrespondenceWorkflow.Restore(
                snapshot.Decisions,
                snapshot.HistoryEvents);
            CurrentUnderstandingExplanation explanation = new ExplainCurrentUnderstandingWorkflow(restored)
                .Explain(review.SourceWork.Id);

            Assert.Single(snapshot.Decisions);
            Assert.Single(snapshot.HistoryEvents);
            Assert.True(explanation.IsKnown);
            Assert.Equal(candidate.WorkId, explanation.Knowledge.AcceptedWorkId);
            Assert.Equal(decision.Id, explanation.SupportingDecision!.Id);
            Assert.Equal(accepted.HistoryEvent!.Id, explanation.HistoryEvents.Single().Id);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            File.Delete(path);
        }
    }

    [Fact]
    public async Task DuplicateDecisionDoesNotOverwriteAndLeavesOneRecord()
    {
        string path = Path.Combine(Path.GetTempPath(), $"guardian-{Guid.NewGuid():N}.db");
        try
        {
            (CandidateReviewContext review, IdentityCandidate candidate) = CreateReview();
            AcceptEditorialDecisionWorkflow acceptance = new(review);
            EditorialDecisionAcceptanceResult accepted = acceptance.Accept(
                candidate.Id,
                new EditorialAuthority("editor"));
            IdentityDecision decision = accepted.Decision ?? throw new InvalidOperationException();
            SqliteEditorialMemoryStore store = new(path);
            await store.SaveAsync(accepted.Decision!, accepted.HistoryEvent!);

            await Assert.ThrowsAnyAsync<Exception>(() => store.SaveAsync(
                accepted.Decision!,
                new IdentityValidationHistoryEvent(
                    new HistoryEventId(Guid.NewGuid()),
                    decision.Id,
                    decision.SourceWorkId,
                    decision.CandidateId,
                    "editor",
                    decision.DecidedAt)));

            EditorialMemorySnapshot snapshot = await store.LoadAsync();
            Assert.Single(snapshot.Decisions);
            Assert.Single(snapshot.HistoryEvents);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            File.Delete(path);
        }
    }

    [Fact]
    public async Task HistoryEventConflictRollsBackTheNewDecision()
    {
        string path = Path.Combine(Path.GetTempPath(), $"guardian-{Guid.NewGuid():N}.db");
        try
        {
            (CandidateReviewContext review, IdentityCandidate candidate) = CreateReview();
            EditorialDecisionAcceptanceResult first = new AcceptEditorialDecisionWorkflow(review,
                () => new DecisionId(Guid.Parse("71000000-0000-0000-0000-000000000001")),
                () => new HistoryEventId(Guid.Parse("81000000-0000-0000-0000-000000000001")))
                .Accept(candidate.Id, new EditorialAuthority("editor"));
            EditorialDecisionAcceptanceResult second = new AcceptEditorialDecisionWorkflow(review,
                () => new DecisionId(Guid.Parse("71000000-0000-0000-0000-000000000002")),
                () => first.HistoryEvent!.Id)
                .Accept(candidate.Id, new EditorialAuthority("editor"));
            SqliteEditorialMemoryStore store = new(path);
            await store.SaveAsync(first.Decision!, first.HistoryEvent!);

            await Assert.ThrowsAnyAsync<Exception>(() => store.SaveAsync(
                second.Decision!, second.HistoryEvent!));

            EditorialMemorySnapshot snapshot = await store.LoadAsync();
            Assert.Single(snapshot.Decisions);
            Assert.Single(snapshot.HistoryEvents);
            Assert.Equal(first.Decision!.Id, snapshot.Decisions[0].Id);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            File.Delete(path);
        }
    }

    [Fact]
    public async Task SupersededDecisionsRestoreCurrentKnowledgeFromExplicitRelationship()
    {
        string path = Path.Combine(Path.GetTempPath(), $"guardian-{Guid.NewGuid():N}.db");
        try
        {
            SourceWorkId sourceId = new(Guid.Parse("63000000-0000-0000-0000-000000000001"));
            IdentityCandidate first = CreateCandidate(sourceId, "64000000-0000-0000-0000-000000000001", "65000000-0000-0000-0000-000000000001");
            IdentityCandidate second = CreateCandidate(sourceId, "64000000-0000-0000-0000-000000000002", "65000000-0000-0000-0000-000000000002");
            IdentityCorrespondenceWorkflow workflow = new(
                [new SourceWork(sourceId)], [first, second],
                () => new DecisionId(Guid.NewGuid()),
                () => new HistoryEventId(Guid.NewGuid()),
                () => new DateTimeOffset(2026, 8, 2, 10, 0, 0, TimeSpan.Zero));
            IdentityValidationResult established = workflow.Validate(new(
                sourceId, first.Id, new EditorialAuthority("editor")));
            IdentityRevisionResult revised = workflow.Revise(new(
                sourceId, second.Id, established.Decision!.Id, new EditorialAuthority("editor"), false));
            IdentityDecision decisionB = revised.Decision ?? throw new InvalidOperationException();

            SqliteEditorialMemoryStore store = new(path);
            await store.SaveAsync(established.Decision, established.HistoryEvent!);
            await store.SaveAsync(decisionB, revised.HistoryEvent!);
            EditorialMemorySnapshot snapshot = await store.LoadAsync();
            IdentityCorrespondenceWorkflow restored = IdentityCorrespondenceWorkflow.Restore(
                snapshot.Decisions,
                snapshot.HistoryEvents);

            Assert.Equal(decisionB.Id, restored.GetKnowledge(sourceId).SupportingDecisionId);
            Assert.Equal(second.WorkId, restored.GetKnowledge(sourceId).AcceptedWorkId);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            File.Delete(path);
        }
    }

    [Fact]
    public async Task EmptyDatabaseInitializesWithoutEditorialRecords()
    {
        string path = Path.Combine(Path.GetTempPath(), $"guardian-{Guid.NewGuid():N}.db");
        try
        {
            EditorialMemorySnapshot snapshot = await new SqliteEditorialMemoryStore(path).LoadAsync();
            Assert.Empty(snapshot.Decisions);
            Assert.Empty(snapshot.HistoryEvents);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            File.Delete(path);
        }
    }

    [Fact]
    public async Task MissingHistoryEventFailsRestorationExplicitly()
    {
        (CandidateReviewContext review, IdentityCandidate candidate) = CreateReview();
        EditorialDecisionAcceptanceResult accepted = new AcceptEditorialDecisionWorkflow(review).Accept(
            candidate.Id, new EditorialAuthority("editor"));

        Assert.Throws<InvalidOperationException>(() => IdentityCorrespondenceWorkflow.Restore(
            [accepted.Decision!], []));
        await Task.CompletedTask;
    }

    [Fact]
    public async Task MalformedStoredDecisionFailsExplicitly()
    {
        string path = Path.Combine(Path.GetTempPath(), $"guardian-{Guid.NewGuid():N}.db");
        try
        {
            SqliteEditorialMemoryStore store = new(path);
            await store.LoadAsync();
            await using SqliteConnection connection = new($"Data Source={path};Pooling=False");
            await connection.OpenAsync();
            await using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO decisions (id, source_work_id, accepted_work_id, candidate_id, category, scope, actor, decided_at) VALUES ('bad', 'bad', 'bad', 'bad', 'Identity', 'AcceptedCorrespondence', 'editor', 'bad');";
            await command.ExecuteNonQueryAsync();

            await Assert.ThrowsAnyAsync<Exception>(() => store.LoadAsync());
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            File.Delete(path);
        }
    }

    private static (CandidateReviewContext Review, IdentityCandidate Candidate) CreateReview()
    {
        SourceWorkId sourceId = new(Guid.Parse("60000000-0000-0000-0000-000000000001"));
        IdentityCandidate candidate = new(
            new CandidateId(Guid.Parse("61000000-0000-0000-0000-000000000001")),
            sourceId,
            new WorkId(Guid.Parse("62000000-0000-0000-0000-000000000001")),
            null,
            "Stargate SG-1",
            1997,
            "Series",
            new CandidateProvenance("test", "fixture"));
        ObservedSourceWork source = new(sourceId, "library", "item", "Stargate SG-1");
        return (ReviewCandidatesWorkflow.Create(source, [candidate]), candidate);
    }

    private static IdentityCandidate CreateCandidate(SourceWorkId sourceId, string candidateId, string workId) =>
        new(
            new CandidateId(Guid.Parse(candidateId)),
            sourceId,
            new WorkId(Guid.Parse(workId)),
            null,
            "Candidate",
            2026,
            "Series",
            new CandidateProvenance("test", "fixture"));
}
