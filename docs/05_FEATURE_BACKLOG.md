# Feature Backlog

> [Documentation index](README.md) · [Blueprint](01_BLUEPRINT.md) · [Roadmap](../ROADMAP.md)

> This document is the implementation backlog for GUARDIAN.
>
> It complements the roadmap by decomposing the project into technical epics,
> features and milestones. It is intentionally implementation-oriented.

---

## v1.0.0-alpha.1 — Foundation

### EPIC 1 — Solution Structure

#### Goal
Create the project skeleton.

- [x] Create `Guardian.sln`
- [x] Create `Guardian.Domain`
- [x] Create `Guardian.Application`
- [x] Create `Guardian.Infrastructure`
- [x] Create `Guardian.Jellyfin`
- [x] Create `Guardian.Providers.Tmdb`
- [x] Create `Guardian.Desktop`
- [x] Create `Guardian.Tests`

**Acceptance criteria**

- Solution builds successfully.
- Projects follow the dependency rules from [`02_ARCHITECTURE.md`](02_ARCHITECTURE.md).

---

### EPIC 2 — Core Domain

#### Goal

Implement Guardian's business model.

#### Features

- [ ] SourceRoot
- [ ] SourceWork
- [ ] Series
- [ ] Movie
- [ ] Episode
- [ ] ProviderIdentity
- [ ] Candidate
- [ ] Decision
- [ ] Lock
- [ ] HistoryEvent
- [ ] Projection

**Acceptance criteria**

- No infrastructure dependency.
- Unit tests for all business rules.

---

### EPIC 3 — SQLite Memory

#### Goal

Create Guardian's persistent memory.

#### Features

- [ ] Initial schema
- [ ] Migration engine
- [ ] Repository layer
- [ ] Lock persistence
- [ ] History persistence
- [ ] Migration tests

---

### EPIC 4 — Library Scanner

#### Goal

Read the source library without modifying it.

#### Features

- [ ] Discover source roots
- [ ] Recursive scan
- [ ] Detect series
- [ ] Detect movies
- [ ] Group episodes
- [ ] Incremental scan

**Acceptance criteria**

- Source remains read-only.
- Scan is repeatable.

---

### EPIC 5 — Filename Parser

#### Goal

Extract useful information from filenames.

#### Features

- [ ] Detect season
- [ ] Detect episode
- [ ] Detect language tags
- [ ] Detect quality tags
- [ ] Detect release tags
- [ ] Confidence score

---

### EPIC 6 — Provider Layer (TMDb)

#### Goal

Search for candidate identities.

#### Features

- [ ] TMDb adapter
- [ ] Candidate search
- [ ] Candidate ranking
- [ ] Rate-limit handling
- [ ] Retry policy

**Invariant**

TMDb proposes.
The user validates.

---

### EPIC 7 — Validation Workflow

#### Goal

Validate identities manually.

#### Features

- [ ] Candidate list
- [ ] Validation
- [ ] Reject candidate
- [ ] Manual identifier
- [ ] Lock decision

---

### EPIC 8 — Projection Builder

#### Goal

Build the disposable Jellyfin projection.

#### Features

- [ ] Create folders
- [ ] Create hard links
- [ ] Generate NFO
- [ ] Build report
- [ ] Rebuild projection

**Invariant**

Projection is always reconstructible.

---

### EPIC 9 — Jellyfin Companion

#### Goal

Prepare integration without modifying Jellyfin.

#### Features

- [ ] Projection conventions
- [ ] Read-only audit
- [ ] Health report
- [ ] Missing media report

---

### EPIC 10 — Desktop Application

#### Goal

Deliver the first Windows application.

#### Features

- [ ] Home screen
- [ ] Scan view
- [ ] Candidate view
- [ ] Validation screen
- [ ] History
- [ ] Build screen
- [ ] Audit screen
- [ ] Settings

---

## Unscheduled work

These tasks are not assigned to a milestone. The [Roadmap](../ROADMAP.md) remains authoritative for milestone order and scope.

- [ ] Advanced parser
- [ ] Batch decision review
- [ ] Improved conflict resolution
- [ ] Projection performance
- [ ] Rich search
- [ ] Keyboard shortcuts
- [ ] Localization
- [ ] Automatic updates

### Cross-work relationships

- [ ] Model relationships between works independently of source organization and target media libraries.
- [ ] Discover and maintain relationships through provider metadata and explicit user decisions.
- [ ] Persist these relationships as Guardian-owned knowledge.
- [ ] Project applicable relationships automatically using the grouping capabilities of the target platform (Jellyfin Collections today).
- [ ] Reconstruct these projected groupings after a target library is recreated.
- [ ] Keep the relationship model independent of any target platform so that other projections remain possible.

This capability is not assigned to a milestone. Its implementation must preserve the Blueprint invariants and accepted ADRs; no technical mechanism is selected by this backlog entry.

Examples include narrative franchises, sagas, shared universes and other relationships between works. The Blueprint intentionally does not prescribe a domain model for these relationships. The appropriate domain concepts should emerge from real-world use cases rather than premature abstraction.

**Invariant**

Guardian may prepare and group hundreds of Candidates for batch decision review. Each validation remains an explicit user action. Automatic or implicit validation is forbidden, as required by [ADR-003](adr/ADR-003-user-validation.md).

---

## v1.0.0 — Stable

### Planned work

- [ ] Complete documentation
- [ ] Full regression suite
- [ ] Packaging
- [ ] Installer
- [ ] User manual

---

## Rules

When implementing features:

1. Respect the Blueprint.
2. Respect accepted ADRs.
3. Never violate project invariants.
4. Implement the smallest coherent change.
5. Add tests with every business rule.
6. Keep documentation synchronized.
7. Ask for approval before commit and push.

This backlog evolves throughout the project. Completed items should be checked rather than removed to preserve project history.
