# ADR-007 — Projection is disposable and reconstructible

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Status: Accepted**

## Context

The destination must not become a fragile second source of truth.

## Decision

The virtual library is entirely derived from the Source and Guardian Decisions.

## Consequences

- deterministic names;
- Build manifests;
- deletion and reconstruction are possible;
- projected videos do not require backup.
