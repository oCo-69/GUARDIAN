# ADR-004 — SQLite preserves minimal memory

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Status: Accepted**

## Context

Guardian must persist Decisions without maintaining a local encyclopedia.

## Decision

SQLite preserves SourceWorks, minimal identities, Decisions, Locks, HistoryEvents, scans, and Builds.

## Consequences

- lightweight and backup-friendly database;
- versioned schema;
- no exhaustive duplication of Provider data.
