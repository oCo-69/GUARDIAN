# Technical Architecture

> [Documentation](README.md) · [Blueprint](01_BLUEPRINT.md) · [ADR](adr/README.md)

## Objective

The architecture must make the [Blueprint](01_BLUEPRINT.md) invariants difficult to violate.

The Domain depends on neither WPF, SQLite, TMDb, nor Jellyfin. Technical dependencies are injected behind interfaces.

## Target solution

```text
Guardian.sln
├── Guardian.Domain
├── Guardian.Application
├── Guardian.Infrastructure
├── Guardian.Providers.Tmdb
├── Guardian.Jellyfin
├── Guardian.Desktop
└── Guardian.Tests
```

## Responsibilities

### Guardian.Domain

Contains:

- SourceWorks and SourceFiles;
- official identities;
- Candidates;
- Decisions;
- Locks;
- states;
- domain events;
- transition rules;
- invariants.

References no technical project.

### Guardian.Application

Orchestrates use cases:

- scanning;
- grouping;
- searching;
- validation;
- locking;
- reidentification;
- restoration;
- building;
- auditing.

Depends on the Domain and abstract interfaces.

### Guardian.Infrastructure

Implements:

- SQLite;
- file-system access;
- fingerprints;
- link creation;
- Build transactions;
- logging;
- local settings and secrets.

### Guardian.Providers.Tmdb

Contains the TMDb adapter.

It returns Candidates and minimal details. It cannot validate a Decision.

### Guardian.Jellyfin

Contains:

- naming conventions;
- the Projection model;
- NFO generation;
- optional read-only Jellyfin access;
- correspondence audits.

It never writes directly to the Jellyfin database.

### Guardian.Desktop

Windows WPF application:

- navigation;
- state display;
- user commands;
- configuration;
- error and History visualization.

The interface does not implement domain rules.

### Guardian.Tests

Contains:

- Domain unit tests;
- parser tests;
- SQLite migration tests;
- integration tests using a fictional library;
- Build tests;
- regression tests.

## Dependency flow

```text
Desktop ───────────────┐
Providers ─────────────┤
Infrastructure ────────┼──> Application ───> Domain
Jellyfin ──────────────┘
```

The Domain is at the center and depends on nothing else.

## Scan pipeline

```text
SourceRoot
  ↓ read-only
File Discovery
  ↓
Fingerprint
  ↓
Filename Parser
  ↓
Work Grouper
  ↓
ScanSnapshot
  ↓
Persistence
```

A scan produces observations. It never replaces a validated Decision.

## Identification pipeline

```text
SourceWork
  ↓
Search Query
  ↓
Provider Adapter
  ↓
Candidates
  ↓
User Selection
  ↓
Validation
  ↓
Decision + HistoryEvent
```

The semantic boundaries among validation, Decision, HistoryEvent, Applicability, and current Knowledge are defined by [ADR-011](adr/ADR-011-editorial-decision-semantics.md).

An accepted correspondence between a SourceWork and a Work is domain meaning under ADR-011. Provider references may support it but do not establish it without explicit validation.

Knowledge meaning and the rule that editorial authority resides exclusively in applicable Decisions are defined by [ADR-012](adr/ADR-012-knowledge-semantics.md). Architecture does not define or require a Knowledge representation.

## Build pipeline

```text
Validated Decision
  ↓
Build Plan
  ↓
Dry Run / Validation
  ↓
Temporary Projection
  ↓
Verification
  ↓
Safe Switch
  ↓
Generated Manifest + HistoryEvent
```

## Concurrency

v1 may use a single operation queue for writes.

Rules:

- only one active Build per destination root;
- short SQLite transactions;
- cancellation supported before the switch;
- a scan can be prepared in the background, but its results are applied atomically;
- HistoryEvents are ordered.

## Fingerprints

A stable fingerprint must recognize a file across successive scans.

v1 may combine:

- normalized path;
- size;
- modification time;
- optional partial fingerprint.

A complete fingerprint is calculated only when it provides real value.

## Configuration

Non-secret settings are stored in SQLite or in a local schema-versioned file.

Secrets, including the TMDb token, must be protected through appropriate Windows mechanisms and must never appear in logs.

## Logging

Two levels:

- clear user-facing message;
- structured technical details.

Every log must exclude secrets and support path anonymization for diagnostic reports.

## Design constraints

- nullable reference types enabled;
- static analysis enabled;
- asynchronous methods for input/output;
- `CancellationToken` on long-running operations;
- no direct SQLite access from the interface;
- no Provider call from the Domain;
- no Decision mutation outside the dedicated service;
- as a consequence of ADR-011, no automatic process exercises editorial authority;
- no Build from an unvalidated Candidate.
