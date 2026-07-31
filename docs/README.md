# GUARDIAN Documentation

This directory is the official entry point for project documentation.

## Recommended human reading path

1. [`01_BLUEPRINT.md`](01_BLUEPRINT.md) — mission, scope, invariants, and vocabulary.
2. [`adr/`](adr/README.md) — accepted architecture decisions.
3. [`02_ARCHITECTURE.md`](02_ARCHITECTURE.md) — components and dependencies.
4. [`03_DATABASE.md`](03_DATABASE.md) — persistent SQLite memory.
5. [`04_DEVELOPMENT.md`](04_DEVELOPMENT.md) — contribution, testing, and delivery.
6. [`05_FEATURE_BACKLOG.md`](05_FEATURE_BACKLOG.md) — technical decomposition of planned work.

## Recommended Codex reading path

1. [`00_CODEX_GUIDELINES.md`](00_CODEX_GUIDELINES.md) — Codex working method.
2. [`01_BLUEPRINT.md`](01_BLUEPRINT.md) — primary project authority.
3. the [ADRs](adr/README.md) relevant to the task;
4. the affected technical references among Architecture, Database, and Development;
5. [`05_FEATURE_BACKLOG.md`](05_FEATURE_BACKLOG.md) when the task concerns planning.

## Role and authority

| Document | Authoritative subject | Status |
|---|---|---|
| [`01_BLUEPRINT.md`](01_BLUEPRINT.md) | Mission, scope, invariants, vocabulary, and design contract | Normative, primary authority |
| [`adr/`](adr/README.md) | Durable architecture decisions and their rationale | Normative when accepted, under the authority of the Blueprint |
| [`02_ARCHITECTURE.md`](02_ARCHITECTURE.md) | Components, responsibilities, and dependencies | Normative for technical architecture |
| [`03_DATABASE.md`](03_DATABASE.md) | Data model, transactions, migrations, and backup | Normative for persistence |
| [`04_DEVELOPMENT.md`](04_DEVELOPMENT.md) | Development, testing, and contribution practices | Normative for development |
| [`00_CODEX_GUIDELINES.md`](00_CODEX_GUIDELINES.md) | Codex working method in this repository | Normative for Codex, without authority to redefine the project |
| [`05_FEATURE_BACKLOG.md`](05_FEATURE_BACKLOG.md) | Epics, features, and implementation tracking | Non-normative planning document |

If documents conflict, the Blueprint prevails. An accepted ADR clarifies the Blueprint but cannot contradict it. Technical references then apply within their respective subjects. Any contradiction between active documents must be reported and submitted for a human decision.

## Canonical language and terminology

English is the sole language of the repository. New repository content must be written directly in English rather than translated after drafting. Bilingual documentation is not permitted.

Canonical domain terms are defined by the [Blueprint glossary](01_BLUEPRINT.md#glossary). Once adopted, a canonical term must be used consistently throughout active documentation, ADRs, source comments, tests, and commit messages. A new synonym must not be introduced for an existing concept.

## Root documents

- [`../README.md`](../README.md) introduces the project and directs readers to this documentation.
- [`../MANIFESTO.md`](../MANIFESTO.md) states the founding intent.
- [`../ROADMAP.md`](../ROADMAP.md) defines milestone order; the backlog details the work without changing that order.
- [`../CHANGELOG.md`](../CHANGELOG.md) records notable changes.

## Archives

Files under [`../archives/`](../archives/README.md) preserve project history. They are never normative and must not be used to resolve a current decision.
