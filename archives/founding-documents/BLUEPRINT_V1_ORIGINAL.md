# Guardian v1 — Architecture Blueprint (previous archive)

This file preserves the essential content of the Blueprint produced before documentation consolidation.

## Mission

Guardian connects the local organization of a collection to official identities, then generates a representation that Jellyfin can recognize correctly without touching the originals.

## Statement

> Guardian proposes. The user decides. Guardian remembers, locks, and rebuilds.

## Principles

1. Inviolable Source.
2. User authority.
3. Explicit validation.
4. Protective locking.
5. Append-only History.
6. Disposable outputs.
7. Minimal dependencies.
8. Safe failure.

## Modules

- Scanner
- Parser
- Work Grouper
- Identity Assistant
- Decision Service
- History Service
- Library Builder
- Audit Service
- Jellyfin Reader

## States

```text
Discovered → Unidentified → Needs review → Validated → Locked
```

Additional states: conflict, stale, error.

## Storage

SQLite preserves SourceRoots, Works, files, minimal identities, Decisions, Locks, HistoryEvents, scans, Builds, and settings.

## Projection

The output uses links, Jellyfin-compatible names, season directories, and minimal NFO files.

## Technology

C#/.NET, WPF, SQLite, and layered architecture.

## Evolution since this version

Active documentation now specifies that TMDb can be used as an optional Candidate Provider when API access is configured. Validation always remains explicit.
