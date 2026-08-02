using Guardian.Application.Persistence;
using Guardian.Domain.Identity;
using Microsoft.Data.Sqlite;

namespace Guardian.Infrastructure;

public sealed class SqliteEditorialMemoryStore : IEditorialMemoryStore
{
    private readonly string connectionString;

    public SqliteEditorialMemoryStore(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        string? directory = Path.GetDirectoryName(Path.GetFullPath(databasePath));
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Pooling = false,
        }.ToString();
    }

    public async Task SaveAsync(
        IdentityDecision decision,
        IdentityValidationHistoryEvent historyEvent,
        IReadOnlyDictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(decision);
        ArgumentNullException.ThrowIfNull(historyEvent);
        if (historyEvent.DecisionId != decision.Id)
        {
            throw new ArgumentException("The HistoryEvent must reference the Decision.", nameof(historyEvent));
        }

        await using SqliteConnection connection = new(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await EnsureSchemaAsync(connection, cancellationToken).ConfigureAwait(false);
        await using SqliteTransaction transaction = (SqliteTransaction)await connection
            .BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await ExecuteAsync(connection, transaction, """
                INSERT INTO decisions
                (id, source_work_id, accepted_work_id, candidate_id, category, scope, actor, decided_at,
                 provider, provider_media_type, provider_external_id, supersedes_id)
                VALUES ($id, $source, $work, $candidate, $category, $scope, $actor, $at,
                        $provider, $provider_media_type, $provider_external_id, $supersedes)
                """, cancellationToken,
                ("$id", decision.Id.Value.ToString()),
                ("$source", decision.SourceWorkId.Value.ToString()),
                ("$work", decision.AcceptedWorkId.Value.ToString()),
                ("$candidate", decision.CandidateId.Value.ToString()),
                ("$category", IdentityDecision.Category),
                ("$scope", IdentityDecision.Scope),
                ("$actor", decision.Authority.ActorId),
                ("$at", decision.DecidedAt.ToString("O")),
                ("$provider", decision.ProviderEvidence?.Provider),
                ("$provider_media_type", decision.ProviderEvidence?.MediaType),
                ("$provider_external_id", decision.ProviderEvidence?.ExternalId),
                ("$supersedes", decision.SupersedesDecisionId?.Value.ToString())).ConfigureAwait(false);

            await ExecuteAsync(connection, transaction, """
                INSERT INTO history_events
                (id, decision_id, source_work_id, candidate_id, actor, occurred_at, superseded_decision_id)
                VALUES ($id, $decision, $source, $candidate, $actor, $at, $superseded)
                """, cancellationToken,
                ("$id", historyEvent.Id.Value.ToString()),
                ("$decision", historyEvent.DecisionId.Value.ToString()),
                ("$source", historyEvent.SourceWorkId.Value.ToString()),
                ("$candidate", historyEvent.CandidateId.Value.ToString()),
                ("$actor", historyEvent.ActorId),
                ("$at", historyEvent.OccurredAt.ToString("O")),
                ("$superseded", historyEvent.SupersededDecisionId?.Value.ToString())).ConfigureAwait(false);

            if (metadata is not null)
            {
                foreach ((string key, string value) in metadata)
                {
                    await ExecuteAsync(connection, transaction,
                        "INSERT INTO guardian_metadata (key, value) VALUES ($key, $value) " +
                        "ON CONFLICT(key) DO UPDATE SET value = excluded.value;",
                        cancellationToken, ("$key", key), ("$value", value)).ConfigureAwait(false);
                }
            }

            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<EditorialMemorySnapshot> LoadAsync(CancellationToken cancellationToken = default)
    {
        await using SqliteConnection connection = new(connectionString);
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await EnsureSchemaAsync(connection, cancellationToken).ConfigureAwait(false);

        List<IdentityDecision> decisions = [];
        await using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "SELECT id, source_work_id, accepted_work_id, candidate_id, actor, decided_at, provider, provider_media_type, provider_external_id, supersedes_id FROM decisions;";
            await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                ProviderIdentity? evidence = reader.IsDBNull(6)
                    ? null
                    : new ProviderIdentity(reader.GetString(6), reader.GetString(7), reader.GetString(8));
                decisions.Add(IdentityDecision.Rehydrate(
                    new DecisionId(Guid.Parse(reader.GetString(0))),
                    new SourceWorkId(Guid.Parse(reader.GetString(1))),
                    new WorkId(Guid.Parse(reader.GetString(2))),
                    new CandidateId(Guid.Parse(reader.GetString(3))),
                    new EditorialAuthority(reader.GetString(4)),
                    DateTimeOffset.Parse(reader.GetString(5), null, System.Globalization.DateTimeStyles.RoundtripKind),
                    evidence,
                    reader.IsDBNull(9) ? null : new DecisionId(Guid.Parse(reader.GetString(9)))));
            }
        }

        List<IdentityValidationHistoryEvent> historyEvents = [];
        await using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "SELECT id, decision_id, source_work_id, candidate_id, actor, occurred_at, superseded_decision_id FROM history_events;";
            await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                historyEvents.Add(new IdentityValidationHistoryEvent(
                    new HistoryEventId(Guid.Parse(reader.GetString(0))),
                    new DecisionId(Guid.Parse(reader.GetString(1))),
                    new SourceWorkId(Guid.Parse(reader.GetString(2))),
                    new CandidateId(Guid.Parse(reader.GetString(3))),
                    reader.GetString(4),
                    DateTimeOffset.Parse(reader.GetString(5), null, System.Globalization.DateTimeStyles.RoundtripKind),
                    reader.IsDBNull(6) ? null : new DecisionId(Guid.Parse(reader.GetString(6)))));
            }
        }

        Dictionary<string, string> metadata = [];
        await using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = "SELECT key, value FROM guardian_metadata;";
            await using SqliteDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
            while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                metadata[reader.GetString(0)] = reader.GetString(1);
            }
        }

        return new EditorialMemorySnapshot(decisions, historyEvents, metadata);
    }

    private static async Task EnsureSchemaAsync(SqliteConnection connection, CancellationToken cancellationToken)
    {
        await ExecuteAsync(connection, null, """
            CREATE TABLE IF NOT EXISTS schema_migrations (version INTEGER PRIMARY KEY, applied_at TEXT NOT NULL);
            CREATE TABLE IF NOT EXISTS decisions (
                id TEXT PRIMARY KEY, source_work_id TEXT NOT NULL, accepted_work_id TEXT NOT NULL,
                candidate_id TEXT NOT NULL, category TEXT NOT NULL, scope TEXT NOT NULL, actor TEXT NOT NULL,
                decided_at TEXT NOT NULL, provider TEXT NULL, provider_media_type TEXT NULL,
                provider_external_id TEXT NULL, supersedes_id TEXT NULL REFERENCES decisions(id));
            CREATE TABLE IF NOT EXISTS history_events (
                id TEXT PRIMARY KEY, decision_id TEXT NOT NULL REFERENCES decisions(id),
                source_work_id TEXT NOT NULL, candidate_id TEXT NOT NULL, actor TEXT NOT NULL,
                occurred_at TEXT NOT NULL, superseded_decision_id TEXT NULL);
            CREATE TABLE IF NOT EXISTS guardian_metadata (key TEXT PRIMARY KEY, value TEXT NOT NULL);
            INSERT OR IGNORE INTO schema_migrations(version, applied_at) VALUES (1, $now);
            """, cancellationToken, ("$now", DateTimeOffset.UtcNow.ToString("O"))).ConfigureAwait(false);
    }

    private static async Task ExecuteAsync(
        SqliteConnection connection,
        SqliteTransaction? transaction,
        string sql,
        CancellationToken cancellationToken,
        params (string Name, object? Value)[] parameters)
    {
        await using SqliteCommand command = connection.CreateCommand();
        command.CommandText = sql;
        command.Transaction = transaction;
        foreach ((string name, object? value) in parameters)
        {
            command.Parameters.AddWithValue(name, value ?? DBNull.Value);
        }

        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
    }
}
