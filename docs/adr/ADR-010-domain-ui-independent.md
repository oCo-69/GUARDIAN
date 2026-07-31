# ADR-010 — The Domain is independent of the user interface

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Status: Accepted**

## Context

Safety and Decision rules must remain testable and stable independently of WPF.

## Decision

The Domain and Application services do not depend on the graphical interface.

## Consequences

- unit tests without a user interface;
- future interfaces remain possible;
- code-behind is limited to presentation.
