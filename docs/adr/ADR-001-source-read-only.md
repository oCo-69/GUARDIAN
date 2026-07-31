# ADR-001 — Source is always read-only

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Status: Accepted**

## Context

The user's original collection is the local source of truth. An accidental rename, move, or deletion would be difficult to reverse.

## Decision

Guardian has no write operation for SourceRoots. Transformations are performed in a separate destination.

## Consequences

- Source access uses read-only abstractions;
- tests verify integrity before and after operations;
- any feature requiring a Source write is rejected.
