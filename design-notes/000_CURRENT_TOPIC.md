# Current Topic — Guardian's Fundamental Domain Language

**Status:** Active exploration

**Authority:** Non-normative

**Implementation consequence:** None

## Purpose

This topic seeks the smallest, most accurate vocabulary for describing Guardian's domain before any domain model is proposed.

The discussion is about meaning, not software structure. It must not define entities, aggregates, classes, database structures, ports, or implementation details.

## Established project constraints

The following constraints already exist in authoritative documentation and are not being reconsidered here:

- the Source is inviolable;
- Guardian may present Candidates, but only explicit user validation creates a Decision;
- identity belongs at the Work level;
- Locks protect validated Decisions from automatic replacement;
- History is append-only;
- a Projection is derived and reconstructible;
- Provider data remains external and minimal;
- relationships between Works belong to Guardian rather than to a projection platform.

The authoritative wording is defined by the [Blueprint](../docs/01_BLUEPRINT.md) and accepted [ADRs](../docs/adr/README.md).

## Current hypotheses

The following statements are working hypotheses, not validated conclusions:

- a Work may be the subject about which Guardian preserves meaning;
- an Observation may describe what Guardian detects without assigning authoritative meaning;
- a Proposal may be a general interpretation offered for human review;
- a Candidate may remain the narrower canonical term for a possible ProviderIdentity;
- a Decision may be an explicit human act rather than the durable state that results from it;
- Knowledge may describe the durable, trusted understanding produced and revised through Decisions;
- a Projection may be an external, reconstructible expression of that understanding.

## Candidate vocabulary

| Term | Candidate meaning | Current status |
|---|---|---|
| `Work` | The media work whose identity or relationships are being understood. | Already present in the Blueprint; exact centrality remains under discussion. |
| `Observation` | A detected fact that does not yet carry authoritative interpretation. | Used descriptively in current documentation; not yet adopted as a foundational domain term. |
| `Proposal` | A general interpretation offered for explicit human review. | Not adopted; its relationship to `Candidate` must be resolved. |
| `Decision` | An explicit human act that accepts, rejects, completes, or revises an interpretation. | Canonical term today; its event-like or state-like meaning requires clarification. |
| `Knowledge` | The durable trusted understanding established by Decisions. | Not adopted and absent from the current canonical glossary. |
| `Projection` | A derived and reconstructible expression for a target platform. | Canonical term today; its relationship to any future Knowledge concept requires clarification. |

## Central distinction under examination

A Decision may be an event: a human act occurring at a particular time.

Knowledge may instead be a durable state: the current trusted understanding produced by one or more Decisions and explained by their History.

This distinction raises the question of whether Guardian primarily preserves Decisions, preserves trusted Knowledge, or preserves both for different purposes.

## Mission-language candidates

The current leading formulation is:

> Guardian preserves trusted knowledge about what the works in a media collection are and how they relate to one another.

This wording is a candidate only. It has not been promoted into the Blueprint and does not replace Guardian's official mission.

## Unresolved questions

The concise action list is maintained in [`003_TODO.md`](003_TODO.md). The active discussion must resolve:

- which concept represents the durable center of the domain;
- whether `Decision` names an event, a retained record, a current state, or more than one of these;
- whether `Knowledge` is precise enough to become a canonical term;
- whether `Proposal` adds necessary meaning beyond `Candidate`;
- how Observation, Proposal, Decision, Knowledge, and Projection differ without overlap;
- which mission wording expresses Guardian's purpose rather than its software behavior;
- which terms should become canonical across documentation, code, tests, and user-facing language.

## Explicit boundary

No term in this document becomes canonical through discussion alone. Adoption requires explicit human validation and promotion into the appropriate authoritative documentation under `docs/`.
