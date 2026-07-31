# SQLite Data Model

> [Documentation](README.md) · [Blueprint](01_BLUEPRINT.md) · [ADR](adr/README.md)

## Role

SQLite is Guardian's persistent memory.

The database preserves local observations, minimal identities, Decisions, Locks, History, and Build state. It does not contain a complete copy of TMDb or another Provider.

The data model must remain compatible with the Decision semantics defined by [ADR-011](adr/ADR-011-editorial-decision-semantics.md). ADR-011 does not prescribe a schema change.

## Primary tables

### `schema_migrations`

```text
version
name
applied_at
checksum
```

### `source_roots`

```text
id
path
is_read_only
created_at
updated_at
```

Constraint: an active v1 SourceRoot is always declared read-only.

### `source_works`

```text
id
root_id
kind
local_title
local_year
stable_key
status
created_at
updated_at
```

`kind`: `series`, `movie`, `special`, `ambiguous`.

### `source_files`

```text
id
work_id
path
size
modified_at
fingerprint
season
episode
local_title
media_extension
created_at
updated_at
```

Constraints:

- `path` is unique within a SourceRoot;
- season and episode are optional;
- no column may assume that an Episode owns a separate series identity.

### `provider_identities`

```text
id
provider
media_type
provider_id
canonical_title
year
source_url
created_at
```

Uniqueness:

```text
(provider, media_type, provider_id)
```

### `candidates`

```text
id
work_id
identity_id
score
evidence_json
provider_payload_cache
created_at
expires_at
```

Candidates are temporary and are never used directly by the Builder.

### `decisions`

```text
id
work_id
identity_id
status
is_locked
created_at
created_by
supersedes_id
reason
```

Rules:

- a validated Decision is immutable;
- reidentification creates a new row;
- `supersedes_id` links the new Decision to the previous one;
- only one current Decision exists per Work;
- the Lock is enforced by the Domain.

### `history_events`

```text
id
work_id
event_type
payload_json
actor
app_version
created_at_utc
created_at_local
correlation_id
```

Append-only.

### `scan_snapshots`

```text
id
started_at
completed_at
status
summary_json
app_version
```

### `build_runs`

```text
id
started_at
completed_at
status
destination_root
summary_json
app_version
```

### `generated_items`

```text
id
build_id
source_file_id
decision_id
destination_path
link_type
checksum
status
created_at
```

### `settings`

```text
key
value_json
updated_at
```

Secrets must not be stored in plaintext in this table.

## Transactions

The following operations must be transactional:

- applying a ScanSnapshot;
- validating a Decision and creating its HistoryEvent;
- locking or unlocking and creating the associated HistoryEvent;
- reidentification;
- restoration;
- final Build recording.

The file system and SQLite do not share one technical transaction. Guardian therefore uses a logical transaction:

1. prepare;
2. validate preconditions;
3. build in a temporary space;
4. verify;
5. switch;
6. record success;
7. clean up.

## Migrations

Every migration must be:

- versioned;
- tested;
- transactional when SQLite permits;
- preceded by a backup during a user upgrade;
- non-destructive to Decisions and History.

A migration never silently deletes a Decision.

## Backup

The minimum backup contains:

- the consistent SQLite file;
- the schema version;
- non-secret settings;
- a backup manifest.

The virtual library does not require backup because it is reconstructible.

## Recommended indexes

- `source_files(work_id)`
- `source_files(fingerprint)`
- `source_works(stable_key)`
- `decisions(work_id, created_at)`
- `history_events(work_id, created_at_utc)`
- `generated_items(build_id)`
- `generated_items(source_file_id)`
- `provider_identities(provider, media_type, provider_id)`
