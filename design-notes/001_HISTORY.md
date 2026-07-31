# Design History

## Purpose

This file is an append-only history of closed design topics. It preserves concise context and conclusions without becoming a normative source.

An entry is added only after a topic has been explicitly closed, rejected, or deferred. Active reasoning remains in [`000_CURRENT_TOPIC.md`](000_CURRENT_TOPIC.md).

Historical entries may describe abandoned lines of reasoning, but their rejected or unresolved status must be explicit.

## Entry format

Each future entry uses the following fields:

### Topic

The subject that was explored.

### Date

The closing date in `YYYY-MM-DD` format.

### Context

The problem or question that initiated the exploration.

### Hypotheses explored

A concise summary of materially different directions considered.

### Conclusion or retained decision

The validated conclusion, explicit rejection, or unresolved outcome.

### Authoritative reference

A link to the authoritative document under `docs/`, or `None` when the topic was rejected or closed without a normative result.

### Closing commit

The related Git commit SHA once available, or `Pending` until the closing commit exists.

## Guardian's Fundamental Domain Language

### Date

2026-07-31

### Context

Guardian needed stable domain language before modeling or normative change. The exploration tested whether the domain centered on files, Works, identity, human Decisions, or trusted Knowledge.

### Hypotheses explored

The topic examined `Work`, `SourceWork`, `Observation`, `Candidate`, `Proposal`, `Decision`, `Knowledge`, `Projection`, `Evidence`, `Fact`, `Assertion`, accepted correspondence, provenance, and HistoryEvent.

### Conclusion or retained decision

The exploration is closed as sufficiently stable for normative impact mapping.

`Knowledge` emerged as the conceptual center: Guardian's current trusted editorial understanding derived from applicable human-validated Decisions and supported by traceable provenance. Explicit validation is the human action; `Decision` is its immutable editorial record; `HistoryEvent` records chronology without independently determining authority.

Applicability and supersession distinguish current Knowledge from the complete immutable Decision history. `Candidate` remains specialized to Provider-reference review. `Assertion` was merged into Candidate-before-validation and Knowledge-after-validation; `Fact` remains a reasoning term; `Proposal` is deferred until a concrete non-Provider reviewable interpretation requires it.

Future normative analysis is divided into Decision semantics, Knowledge semantics, Provider reference model, Observation provenance and retention, and Projection semantics. The project moved to impact mapping because further broad vocabulary refinement would add less value than testing these concepts against existing authority and migration constraints.

### Authoritative reference

None. This closure is a non-normative process milestone.

### Closing commit

Pending

## Decision Semantics Impact Mapping and ADR Preparation

### Date

2026-07-31

### Context

The retained vocabulary required a precise normative impact review before any authoritative change. The work mapped Decision, Knowledge, Provider references, Observation provenance, and Projection semantics across the Blueprint, Architecture, Database, and accepted ADRs.

### Hypotheses explored

The review tested whether the work belonged in one broad ADR or independent workstreams, whether Applicability required a separate ADR, and how accepted correspondence related to identity and Provider evidence.

### Conclusion or retained decision

Normative work was split into five workstreams. Decision semantics became the first ADR because editorial authority, immutable Decisions, subject, scope, outcome, Applicability, supersession, HistoryEvent distinction, and accepted correspondence provide the foundation required by later work.

ADR-011 was prepared and explicitly accepted together with coordinated Blueprint, Architecture, and Database clarifications. Applicability remains a cross-cutting Decision rule rather than a separate ADR.

### Authoritative reference

[ADR-011 — Editorial Decision Semantics](../docs/adr/ADR-011-editorial-decision-semantics.md)

### Closing commit

Pending
