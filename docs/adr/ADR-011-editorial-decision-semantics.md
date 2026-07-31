# ADR-011 — Editorial Decision Semantics

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Status: Accepted**

## 1. Problem Statement

Guardian currently defines a Decision as a validated choice connecting a SourceWork to a ProviderIdentity. That definition is sufficient for the first identity workflow but conflates a specialized outcome with the general meaning of an editorial Decision.

The current authority also does not share precise semantics for:

- who exercises editorial authority;
- the distinction between explicit validation and the resulting Decision;
- the subject and scope of a Decision;
- the determinate outcome recorded by a Decision;
- which immutable Decisions currently apply;
- how a later Decision supersedes the effect of an earlier one;
- the distinction between a Decision and a HistoryEvent;
- how applicable Decisions contribute to current Knowledge.

Without these semantics, future editorial choices could acquire inconsistent meanings across identity, relationships, classification, references, history, and audit.

## 2. Context

Guardian proposes and the user decides. Provider results remain Candidates until explicit validation.

Validated Decisions are immutable. Reidentification and restoration preserve earlier Decisions rather than rewriting them. HistoryEvents are append-only, and Locks protect validated identity Decisions from automatic replacement.

Identity belongs at the Work level. A SourceWork is Guardian's local manifestation of a Work, while a ProviderIdentity is an external official reference. The existing identity workflow uses a validated Decision to connect a SourceWork to a ProviderIdentity.

These constraints remain valid. This ADR establishes shared editorial semantics beneath that specialized workflow without defining technical representation.

## 3. Decision

### Editorial authority

Editorial authority is the human authority to determine Guardian's accepted understanding of a subject within a defined scope.

Provider data, confidence, rules, imports, and automated analysis do not exercise editorial authority.

### Explicit validation

Explicit validation is the human action through which editorial authority is exercised for a determinate outcome.

Validation is not a Decision. It is the action that establishes the Decision.

### Immutable Decision

A Decision is the immutable record of an explicit human-validated editorial choice.

Every Decision answers six questions:

1. Who exercises authority?
2. About what subject?
3. Within which scope?
4. What editorial outcome?
5. Under which applicability conditions?
6. What provenance explains the Decision?

The Decision remains historically existent after its effect is superseded. Its recorded authority, subject, scope, outcome, applicability conditions, and provenance are not rewritten.

### Subject

The subject identifies what the editorial choice is about.

A subject must be sufficiently precise to distinguish the affected domain meaning from adjacent or unrelated meaning.

### Scope

The scope identifies the semantic boundary within which the outcome applies.

Scope must be sufficiently precise to determine whether two Decisions overlap, coexist, or may supersede one another.

### Determinate outcome

The outcome records the editorial meaning that was explicitly validated.

An outcome is determinate when Guardian can distinguish the accepted meaning from the Candidates, evidence, alternatives, or context considered during validation.

### Applicability

Applicability determines which immutable Decisions currently contribute to Knowledge.

Applicability is evaluated from a Decision's subject, scope, outcome, applicability conditions, and supersession relationships. Historical existence alone does not make a Decision currently applicable.

The semantic model is:

```text
Immutable Decisions
→ Applicability rules
→ Current Trusted Knowledge
```

Knowledge is not the simple accumulation of every historical Decision.

### Supersession

Supersession establishes that a later Decision replaces the current effect of an earlier Decision within an overlapping subject and scope.

Supersession changes Applicability. It does not modify, delete, or retroactively invalidate the earlier Decision.

An earlier Decision remains part of the immutable historical record while no longer contributing to current Knowledge.

### Decision and HistoryEvent

A Decision records an authoritative editorial choice.

A HistoryEvent records that a significant action or transition occurred in chronology.

A HistoryEvent may record validation or supersession, but chronology alone does not establish editorial authority or determine Applicability. A HistoryEvent does not replace the related Decision.

### Accepted correspondence

An accepted correspondence is Guardian's editorially accepted semantic connection between a SourceWork and a Work.

Provider references may supply evidence supporting that correspondence. No Provider reference, response, or confidence score establishes the correspondence without explicit human validation.

Provider-reference roles and mechanics are outside this ADR.

### Decisions and Knowledge

Knowledge is Guardian's current trusted editorial understanding insofar as it is derived from applicable Decisions and supported by traceable provenance.

This ADR uses Knowledge only as the consumer of applicable Decisions. It does not define Knowledge representation, persistence, or its complete future semantics.

## 4. Consequences

### Benefits

- explicit human authority remains the only source of accepted editorial choices;
- validation action, Decision record, and HistoryEvent chronology have distinct meanings;
- immutable history can coexist with changing current Knowledge;
- supersession no longer implies rewriting or invalidating historical Decisions;
- subject and scope provide a semantic basis for determining coexistence and supersession;
- accepted SourceWork-to-Work correspondence belongs to Guardian rather than to a Provider;
- identity Decisions remain a valid specialized use of the general semantics.

### Limitations

- this ADR does not determine how Decision semantics are represented technically;
- it does not establish every future Decision category;
- Applicability rules cannot be fully specialized until those categories are defined;
- accepted correspondence is defined semantically without defining Provider-reference cardinality;
- Knowledge is referenced without selecting a representation.

### Expected future work

Separate normative work will address:

- Knowledge semantics;
- the Provider reference model;
- Observation provenance and retention;
- Projection semantics;
- technical impact and migration when implementation requires them.

## 5. Compatibility Notes

### Blueprint

The current identity-specific definition of Decision becomes a specialized application of the general editorial semantics.

The user-authority, protective-locking, append-only-history, Work-level identity, safe-failure, and minimal-memory invariants remain in force.

The Blueprint requires clarification of:

- explicit validation versus Decision;
- Decision subject, scope, outcome, Applicability, and supersession;
- Decision versus HistoryEvent;
- accepted SourceWork-to-Work correspondence;
- applicable Decisions as inputs to current Knowledge.

### Architecture

The Domain remains responsible for Decision invariants. Application workflows continue to orchestrate explicit validation.

The architecture must preserve the semantic boundaries among validation, immutable Decision, HistoryEvent, Applicability, and current Knowledge. This note does not select components, services, ports, or representation.

### Database

Persistent memory must eventually be assessed against the six Decision questions and the need to explain Applicability and supersession.

This ADR does not define tables, columns, constraints, persistence strategy, legacy-data interpretation, or migration.

### Compatibility risks

- existing wording treats every Decision as an identity-specific SourceWork-to-ProviderIdentity choice;
- the current one-current-Decision-per-Work rule may be specific to identity scope;
- current Lock semantics are defined for validated identity Decisions and are not generalized here;
- existing persistence may not represent every semantic distinction introduced by this ADR;
- careless use of Knowledge could imply a representation that this ADR intentionally leaves undefined.

## 6. Deferred Topics

The following topics are intentionally outside this ADR:

- complete Knowledge semantics;
- Knowledge representation;
- persistence;
- migration;
- legacy-data interpretation;
- database structures;
- revocation;
- durable rejection policies;
- Provider-reference mechanics;
- Observation provenance and retention;
- Projection semantics;
- implementation services, classes, aggregates, APIs, or storage mechanisms.
