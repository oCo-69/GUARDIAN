# Guardian v1 — Blueprint

> [Documentation](README.md) · [ADR](adr/README.md)

## 1. Status

This document is the Guardian v1 design contract.

Every feature must respect the invariants, responsibilities, and boundaries defined here. When a new decision changes this contract, it must be documented by an ADR.

## 2. Mission

Guardian unambiguously connects the local organization of a collection to the official identities of its works, then builds a representation that Jellyfin can recognize correctly without touching the originals.

Guardian also preserves business relationships defined between multiple works, regardless of their type (movies, series, OVAs, specials, and others), source location, or distribution across libraries. This knowledge remains independent of the target platform so that it can be reconstructed and support other forms of projection.

Fundamental mappings:

```text
Series: SourceWork ↔ official series identity
Movie:  SourceWork ↔ official movie identity
```

Episodes inherit the identity of their series. They are not identified manually one by one.

## 3. v1 scope

### Included

- read-only scanning of one or more source directories;
- detection of series, movies, episodes, specials, and ambiguous cases;
- grouping episodes under a series work;
- Candidate search through TMDb when API access is configured;
- manual browser search as a fallback;
- direct identifier entry or URL paste;
- validation, locking, unlocking, and reidentification;
- append-only history;
- link-based Jellyfin virtual library;
- compatible names, season directories, and minimal NFO files;
- audits across the source, Guardian memory, and generated output.

### Excluded from the first release

- modification of the source library;
- direct modification of the Jellyfin database;
- a complete local encyclopedia;
- large-scale scraping;
- irreversible automatic validation;
- replacement of Jellyfin;
- exhaustive management of artwork, actors, synopses, and ratings;
- native TV features beyond those already provided by Jellyfin.

## 4. Non-negotiable invariants

### P1 — Inviolable source

Guardian never writes to source directories.

### P2 — User authority

Guardian may propose, never impose. Only explicit human validation creates a Decision; a Candidate never has editorial authority. [ADR-011](adr/ADR-011-editorial-decision-semantics.md) defines the governing Decision semantics.

### P3 — Work-level identity

A Series is identified once. Its Episodes inherit that identity and are attached through their season and episode numbers.

### P4 — Protective locking

A locked Decision cannot be replaced by a scan, import, rule, Provider, or Candidate.

### P5 — Append-only history

A previous Decision is never silently erased. Every change creates a new HistoryEvent and, when replacement occurs, a new Decision linked to the previous one. Immutability, Applicability, and supersession follow [ADR-011](adr/ADR-011-editorial-decision-semantics.md).

### P6 — Derived output

The virtual library, NFO files, manifests, and reports are derived. They must be deletable and reconstructible.

### P7 — Minimal memory

Guardian preserves the identities, Decisions, Locks, scans, Builds, and HistoryEvents it needs. It does not maintain a parallel encyclopedia.

### P8 — Safe failure

When uncertainty remains, Guardian isolates the affected case and requests a Decision. It does not degrade either the source or an existing valid Projection.

### P9 — Independent business relationships

Relationships between Works belong to the Guardian domain model, not to the projection platform.

## 5. Conceptual model

| Object | Responsibility |
|---|---|
| `SourceRoot` | Protected root containing the originals |
| `SourceWork` | Logical local work: Series, Movie, or ambiguous case |
| `SourceFile` | Original file identified by path and fingerprint |
| `EpisodeDescriptor` | Season, episode, and local title inferred from a file |
| `ProviderIdentity` | Provider, media type, and official identifier |
| `Candidate` | Unvalidated possible match |
| `Decision` | Explicit human-validated editorial choice governed by ADR-011 |
| `Lock` | Functional protection of a Decision |
| `HistoryEvent` | Timestamped record of an action or change |
| `GeneratedItem` | Item created in the Projection |
| `ScanSnapshot` | Logical state of a scan |
| `BuildRun` | Execution record for a Build |

## Glossary

### Source concepts

- **Source** — original media collection that Guardian observes without modifying.
- **SourceRoot** — configured read-only root containing part of the Source.
- **SourceWork** — logical local grouping of files representing a Work or a case that remains ambiguous.
- **SourceFile** — original media file observed within a SourceRoot.
- **Work** — media work serving as the unit of identity, regardless of the number of associated files.
- **Series** — Work composed of Episodes that inherit the same series identity.
- **Movie** — Work representing one movie, carried by one file or a group explicitly declared equivalent.
- **Episode** — element of a Series identified by season and episode numbers.

### Identity concepts

- **Provider** — optional adapter to an external source of identities and minimal metadata.
- **Candidate** — unvalidated possible match connecting a SourceWork to a potential ProviderIdentity.
- **ProviderIdentity** — minimal normalized official identity supplied by a Provider.

### Editorial concepts

- **Accepted correspondence** — editorially accepted semantic connection between a SourceWork and a Work; see [ADR-011](adr/ADR-011-editorial-decision-semantics.md).
- **Decision** — explicit human-validated editorial choice; its semantics are defined by [ADR-011](adr/ADR-011-editorial-decision-semantics.md).
- **Applicability** — rule determining which Decisions contribute to current Knowledge; see [ADR-011](adr/ADR-011-editorial-decision-semantics.md).
- **Knowledge** — current trusted editorial understanding reflecting applicable Decisions; detailed semantics are defined by [ADR-012](adr/ADR-012-knowledge-semantics.md).
- **Lock** — functional protection preventing automatic replacement of a Decision.
- **HistoryEvent** — append-only record of an action or significant change.

### Projection concepts

- **Projection** — derived, disposable, and reconstructible Jellyfin library.
- **Build** — execution that builds or rebuilds all or part of the Projection.
- **Audit** — comparison across the Source, Decisions, and Projection, optionally supplemented by a Jellyfin read.

## 6. Primary workflow

```text
Scan
  ↓
Group Works
  ↓
Search for Candidates
  ↓
Present
  ↓
User validation
  ↓
Optional Lock
  ↓
Build
  ↓
Audit
```

No Candidate can be used by the Builder before validation.

## 7. Identification rules

### Series

The Decision applies to the Series Work, generally inferred from a directory or logical group.

```text
G:\_MANGAS\Dororo
  ↕
TMDb / TV / 83100
```

All associated Episodes inherit this identity.

### Movies

The Decision applies to one file or to a group explicitly declared to represent the same Movie.

### Specials, OVAs, and ambiguous cases

Guardian may propose a category, but it does not silently transform a special into a Movie or Episode when that choice affects Jellyfin organization.

## 8. Providers and TMDb

TMDb is an optional Provider of Candidates and minimal metadata.

When a token is configured:

1. Guardian builds a query from the local title, probable year, and Work type.
2. The Provider returns Candidates.
3. Guardian presents them with distinguishing information.
4. The user selects and validates one.
5. Guardian records the normalized ProviderIdentity.

Without API access, Guardian must remain usable through an open browser search and by pasting a URL or identifier.

An API response is always a Candidate, never a Decision.

## 9. States

Primary states:

```text
Discovered → Unidentified → Needs review → Validated → Locked
```

Additional states:

- `Conflict`: the Projection or Jellyfin does not match the Decision;
- `Stale`: the Source has changed enough to require review;
- `Error`: an operation failed without degrading established work.

## 10. Locking

The Lock is enforced in the domain, not only in the interface.

- a locked Decision remains visible and auditable;
- a conflict can be reported without modifying the Decision;
- every reidentification requires explicit unlocking;
- locking and unlocking create HistoryEvents;
- restoring a previous Decision creates a new Decision;
- no automation bypasses the Lock.

## 11. History

History must answer four questions:

1. What changed?
2. When?
3. Why, or through which action?
4. What were the states before and after?

Minimum HistoryEvent types:

- scan;
- Candidate proposed;
- validation;
- locking;
- unlocking;
- reidentification;
- Build;
- conflict;
- restoration;
- error.

## 12. Virtual library

Example:

```text
G:\_JELLYFIN_MANGAS\
└── Dororo (2019) [tmdbid-83100]\
    ├── tvshow.nfo
    └── Season 01\
        ├── Dororo S01E01 - Daigo.mkv
        ├── Dororo S01E01 - Daigo.nfo
        └── ...
```

Rules:

- hard links on the same volume;
- symbolic links only after explicit choice and permission verification;
- no video copy by default;
- writes limited to the Guardian Projection root;
- temporary Build before switching when an operation affects multiple items;
- a manifest for every generated item;
- deterministic names;
- minimal NFO files intended to reinforce identity.

## 13. Application services

| Service | Essential contract |
|---|---|
| `ScanLibrary` | Reads Sources and produces a ScanSnapshot; never writes to the Source |
| `GroupWorks` | Groups files under logical Works |
| `SearchCandidates` | Returns Candidates; never creates a Decision |
| `ParseProviderReference` | Normalizes a URL or identifier |
| `ValidateDecision` | Creates a validated Decision and a HistoryEvent |
| `LockDecision` | Protects a validated Decision |
| `ReidentifyWork` | Refuses when locked; preserves the previous Decision |
| `RestoreDecision` | Creates a new Decision from History |
| `BuildWork` | Builds only a validated Work |
| `BuildLibrary` | Builds eligible Works and isolates failures |
| `AuditWork` | Compares the Source, Decision, Projection, and optionally Jellyfin |

## 14. v1 interface

Primary screens:

- Dashboard;
- Library;
- Identification;
- Work details;
- History;
- Settings.

Visible actions for each Work:

- identify;
- validate;
- lock or unlock;
- reidentify;
- enter a Provider reference;
- open the result;
- rebuild;
- audit;
- inspect History;
- restore a previous Decision.

## 15. Error handling

- an error affecting one Work does not block the others;
- a valid Projection is not removed before its replacement is ready;
- errors provide a clear message and copyable technical details;
- multi-step operations use logical transactions;
- SQLite writes use transactions;
- name conflicts, incompatible volumes, and insufficient permissions are detected before writing;
- every Build offers a dry-run mode.

## 16. Security and privacy

- no mandatory telemetry;
- no transmission of local paths to a Guardian server;
- Provider secrets stored through a secure operating-system mechanism;
- secrets excluded from logs and reports;
- simple backup of the local database;
- configurable and removable logs;
- no silent browser inspection;
- explicit privacy rules for project documents and screenshots.

## 17. Acceptance criteria for the first testable version

1. The application starts without a command line.
2. The user chooses a Source and destination.
3. The scan groups Episodes correctly and isolates Movies.
4. A Series is never identified Episode by Episode.
5. TMDb can propose Candidates when configured.
6. A URL or identifier can be entered manually.
7. Validation creates a persistent Decision.
8. A Lock prevents accidental replacement.
9. Reidentification preserves the previous Decision.
10. Reconstruction creates a Jellyfin directory structure, links, and minimal NFO files.
11. Deleting the complete Projection and rebuilding it produces the same logical result.
12. No operation modifies the Source.
13. An Audit distinguishes validated, locked, conflicting, and pending Works.

## 18. Governance

Before accepting a feature, verify that it:

- directly helps recognition or maintenance of the library;
- respects the inviolability of the Source;
- preserves user authority;
- respects Locks;
- produces reconstructible output;
- avoids unnecessarily duplicating an existing service;
- remains traceable;
- can fail without degrading established work.

When any answer is negative, the feature must be revised, isolated as an optional module, or rejected.
