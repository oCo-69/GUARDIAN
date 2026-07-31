# ADR-012 — Knowledge Semantics

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Status: Accepted**

## 1. Problem Statement

[ADR-011](ADR-011-editorial-decision-semantics.md) establishes that applicable Decisions contribute to current Knowledge. It intentionally does not define the complete meaning or boundaries of Knowledge.

Without a shared definition, Knowledge could be confused with:

- the authority residing in applicable Decisions;
- raw information or Provider data;
- immutable history;
- workflow and audit conditions;
- a technical representation;
- a Projection for a consumer.

Guardian requires a representation-independent semantic definition before architecture, persistence, or future editorial capabilities can rely on Knowledge consistently.

## 2. Context

ADR-011 remains authoritative for Decision, editorial authority, explicit validation, immutability, subject, scope, Applicability, supersession, accepted correspondence, and Decision versus HistoryEvent.

This ADR consumes those semantics without redefining them.

The inherited relationship is:

```text
Immutable Decisions
→ Applicability
→ Current Trusted Knowledge
```

Knowledge concerns a semantic subject within a scope. It expresses Guardian's current editorial understanding rather than every observation, possibility, Decision, or historical transition associated with that subject.

## 3. Decision

Knowledge never owns editorial authority.

Editorial authority resides exclusively in applicable Decisions.

Knowledge reflects the current editorial understanding produced by those Decisions.

### What Knowledge is

Knowledge is Guardian's current trusted editorial understanding of a semantic subject within a scope, determined by a coherent set of applicable Decisions and explainable through their provenance.

Knowledge is:

- current;
- trusted;
- explainable;
- representation-independent;
- implementation-independent.

### What contributes to Knowledge

Only applicable Decisions supply the authoritative editorial outcomes reflected in Knowledge.

One applicable Decision may determine a known editorial outcome. Several applicable Decisions may jointly determine one outcome when their semantic effects are compatible.

For any subject and scope, applicable Decisions contribute to Knowledge only when they determine no more than one coherent editorial outcome.

An accepted correspondence between a SourceWork and a Work is part of Knowledge while the Decision establishing that correspondence remains applicable.

Provenance explains why a known outcome is trusted. Provenance does not exercise editorial authority and does not establish Knowledge independently of applicable Decisions.

### What does not contribute to Knowledge

The following do not independently establish Knowledge:

- Observations;
- Candidates;
- raw Provider information;
- confidence scores;
- non-applicable Decisions;
- HistoryEvents;
- unsupported claims;
- Projections.

Storage, repetition, visibility, technical availability, or successful generation does not grant editorial authority to information.

### Known and Unknown

Knowledge has exactly two semantic states for a subject and scope:

- `Known`;
- `Unknown`.

Knowledge is `Known` when the applicable Decisions determine one coherent editorial outcome that is explainable through their provenance.

Knowledge is `Unknown` when no coherent determinate editorial outcome is established.

Absence of an applicable Decision results in Unknown Knowledge. Unknown Knowledge does not imply that no historical Decision exists.

Workflow, audit, access, and provenance conditions do not create additional Knowledge states.

### Conflict

Conflict is not a Knowledge state.

When applicable Decisions prescribe incompatible outcomes within overlapping subject and scope, they do not establish coherent Knowledge. Knowledge for that subject and scope is Unknown.

The conflicting Decisions remain governed by ADR-011 and the Decision lifecycle. This ADR does not define conflict detection or correction.

### Unsupported material

`Unsupported` is not a Knowledge concept.

It describes an audit or provenance concern. When applicable Decisions and their provenance cannot explain a claimed outcome, the criterion for Known Knowledge is not satisfied.

### Semantic guarantees

Knowledge provides these guarantees:

- **Authority reflection:** it reflects editorial authority residing exclusively in applicable Decisions;
- **Current meaning:** it reflects present Applicability rather than all historical Decisions;
- **Trust:** a known outcome originates in explicit human-validated Decisions;
- **Coherence:** Knowledge never contains conflicting editorial outcomes within the same subject and scope;
- **Explainability:** a known outcome can be traced to its governing applicable Decisions and their provenance;
- **Revisability:** changed Applicability may change Knowledge without modifying immutable historical records;
- **Representation independence:** no storage or presentation form defines its meaning;
- **Implementation independence:** no component, service, or algorithm defines its authority.

### Explicit limits

Knowledge does not define:

- Decision or Applicability semantics;
- authority independent of Decisions;
- immutable history;
- Provider-reference roles;
- Observation retention;
- Projection behavior;
- representation;
- persistence;
- reconstruction;
- synchronization;
- conflict-resolution behavior;
- implementation structures.

Reconstructibility is not a Knowledge guarantee. It belongs to representation, Projection, persistence, or audit requirements where those guarantees are defined.

## 4. Consequences

### Benefits

- Guardian has one canonical meaning for current trusted editorial understanding;
- Knowledge and its representations cannot become independent sources of editorial authority;
- Known and Unknown remain semantic rather than workflow states;
- conflicting applicable Decisions cannot be presented as coherent Knowledge;
- several compatible Decisions may support one coherent outcome;
- accepted correspondence has a clear place in current Knowledge;
- future representations can change without changing domain meaning.

### Limitations

- this ADR does not define how Knowledge is represented or obtained technically;
- it does not define composition rules for future Decision categories;
- it does not define how conflicts are detected or corrected;
- it does not define provenance retention or audit behavior;
- it does not define Provider-reference or Projection semantics.

### Expected future work

Separate normative work may address:

- Provider reference roles;
- Observation provenance and retention;
- Projection semantics and reconstructibility;
- Decision-lifecycle conflict correction;
- Knowledge representation only if a later impact review demonstrates a need.

## 5. Compatibility Notes

### ADR-011

This ADR specializes only the Knowledge semantics intentionally deferred by ADR-011.

It preserves the authority residing in Decisions, immutability, subject, scope, Applicability, supersession, accepted correspondence, and Decision versus HistoryEvent.

### Blueprint

The Blueprint's concise Knowledge definition remains compatible. It requires only a reference to this ADR and a high-level statement of Knowledge's architectural purpose.

The `Known` and `Unknown` states, the authority statement, and all other detailed Knowledge semantics remain in this ADR rather than the Blueprint.

### Architecture

Knowledge remains domain meaning rather than a component, service, pipeline stage, cache, or read model.

Architecture must preserve the boundaries among Decisions, Knowledge, HistoryEvents, Observations, Candidates, and Projections without selecting a Knowledge representation.

### Database

No Database change is required.

This ADR creates no persistence obligation, schema concept, stored state, reconstruction requirement, or migration concern. Persistence cannot define Knowledge or make its representations authoritative.

### Compatibility risks

- `Known` and `Unknown` could be misread as required stored statuses;
- explainability could be misread as a retention design;
- joint contribution could be misread as an unspecified merging algorithm;
- accepted correspondence could be misread as an authority separate from Decisions;
- Knowledge could be confused with a Projection or technical read model.

These interpretations are excluded by this ADR.

## 6. Deferred Topics

The following topics are intentionally outside this ADR:

- Decision semantics;
- Applicability semantics;
- Provider-reference mechanics;
- Observation provenance and retention;
- Projection semantics;
- persistence;
- migration;
- database schemas;
- caches, views, or materialization;
- reconstruction and synchronization;
- conflict detection and correction algorithms;
- entities, aggregates, services, ports, APIs, or other implementation structures.
