# ADR-003 — No Candidate is applied without validation

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Status: Accepted**

## Context

An automatic search can produce a plausible but incorrect result.

## Decision

Every Provider result is a Candidate. Only an explicit user action creates a validated Decision.

## Consequences

- the Builder rejects Candidates;
- confidence scores help with ordering but never decide;
- automatic imports remain Candidates requiring explicit review.
