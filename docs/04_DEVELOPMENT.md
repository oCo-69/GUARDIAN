# Development Guide

> [Documentation](README.md) · [Blueprint](01_BLUEPRINT.md) · [ADR](adr/README.md)

## Prerequisites

- Windows 11;
- .NET 8 SDK selected by `global.json`;
- Visual Studio or a .NET-compatible editor;
- Git;
- SQLite embedded by the application;
- optional TMDb account for Provider testing.

## Repository layout

Expected structure:

```text
src/
tests/
tools/
docs/
archives/
```

Application code must not be placed at the repository root.

## Contribution rule

Before developing a feature:

1. verify compliance with the [Blueprint](01_BLUEPRINT.md);
2. identify the affected invariants;
3. create an ADR when a new architecture decision is required;
4. write or update tests;
5. implement;
6. verify that no Source write is possible.

## C# quality

- nullable reference types enabled;
- warnings treated as errors in primary projects;
- .NET analyzers enabled;
- explicit names;
- small methods;
- injected dependencies;
- interfaces at technical boundaries;
- no domain logic in WPF code-behind;
- asynchronous input/output operations;
- cancellation support;
- user-facing messages separated from technical details.

## Minimum tests

### Domain

- validation without an identity is impossible;
- building from a Candidate is impossible;
- replacing a locked Decision is impossible;
- unlocking creates a HistoryEvent;
- restoration creates a new Decision;
- a Candidate does not modify validated state.

### Parser

Tests are based on observed real-world conventions:

```text
[01x01]
S01E01
Season 01
[24xFull]
[Jap&Eng]
[STEN]
```

The parser must expose its confidence level and must not guess when data conflicts.

### Build

- Source and destination on the same volume;
- different volumes;
- name collision;
- inaccessible destination;
- existing link;
- cancelled Build;
- partially failed Build;
- deterministic reconstruction;
- unchanged Source before and after.

### SQLite

- database creation;
- successive migrations;
- rollback on error;
- preserved Decisions and History;
- backup and restore.

## Branches

Recommended simple organization:

- `main`: stable or demonstrable state;
- short-lived branches per feature;
- pull request for every notable integration.

Avoid a long-lived `develop` branch while the team remains small.

## Commits

Recommended format:

```text
type(scope): description
```

Examples:

```text
feat(scanner): add read-only source discovery
fix(lock): reject automatic replacement of locked decisions
docs(blueprint): clarify TMDb Candidate workflow
test(builder): cover cross-volume failure
```

Primary types:

- `feat`
- `fix`
- `docs`
- `test`
- `refactor`
- `build`
- `chore`

Commit messages must be written in English.

## Pull requests

A pull request must state:

- problem addressed;
- solution;
- affected invariants;
- tests added;
- database impact;
- Source impact;
- screenshots for interface changes;
- associated ADR, when applicable.

## Definition of done

A task is complete when:

- it builds without blocking warnings;
- tests pass;
- the Source is protected by design and by tests;
- errors are understandable;
- active documentation remains coherent;
- no secret is logged;
- the change is testable.

## Versioning

The project uses semantic versioning starting at `1.0.0`.

Before the stable release:

```text
1.0.0-alpha.1
1.0.0-alpha.2
1.0.0-beta.1
```

## Documentation

Active documentation must remain concise.

English is the sole repository language. New content must be written directly in English, and canonical domain terms from the [Blueprint glossary](01_BLUEPRINT.md#glossary) must be used consistently.

Normative information exists in exactly one place. Historical documents remain under `archives/` and must not be cited as current rules.
