# ADR-005 — Validated Decisions can be locked

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Status: Accepted**

## Context

A new analysis or better Candidate must not cancel a previously confirmed choice.

## Decision

The Lock is a domain invariant that prevents automatic replacement. Reidentification requires explicit unlocking.

## Consequences

- enforcement in the Domain;
- locking and unlocking HistoryEvents;
- conflicts are reported but never corrected automatically.
