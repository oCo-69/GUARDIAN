# ADR-008 — No direct writes to the Jellyfin database

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Status: Accepted**

## Context

Jellyfin's internal database evolves, and an external write could corrupt it.

## Decision

Guardian acts through directory structure, links, and NFO files. An optional Jellyfin reader can audit in read-only mode.

## Consequences

- decoupling from Jellyfin versions;
- lower corruption risk;
- conflicts resolved through reconstruction or user action.
