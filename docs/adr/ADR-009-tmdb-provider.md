# ADR-009 — TMDb is an optional Candidate Provider

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Status: Accepted**

## Context

TMDb access is available and can accelerate identification, but Guardian must remain usable without it.

## Decision

TMDb is implemented as an optional adapter. It supplies Candidates and minimal metadata. Validation remains manual.

## Consequences

- token stored securely;
- fallback through browser search and direct entry;
- no Domain coupling to TMDb;
- future Providers remain possible.
