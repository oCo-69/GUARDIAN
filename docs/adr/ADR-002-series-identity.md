# ADR-002 — A Series is identified at the Work level

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Status: Accepted**

## Context

Manually identifying every Episode repeats the same Decision and increases the risk of inconsistency.

## Decision

A Series Decision connects a SourceWork to an official identity. Episodes inherit this identity and use their season and episode numbers.

## Consequences

- the interface requests one validation per Series;
- the parser produces EpisodeDescriptors;
- anomalies remain explicit.
