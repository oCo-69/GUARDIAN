# ADR-006 — Append-only History

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Status: Accepted**

## Context

Diagnosis and restoration require an understanding of how Decisions evolved.

## Decision

HistoryEvents are appended without rewriting. A restoration creates a new Decision instead of erasing History.

## Consequences

- complete traceability;
- before-and-after comparison;
- growth may be managed through future archival, never through silent deletion.
