# Codex Guidelines

> [Documentation index](README.md) · [Blueprint](01_BLUEPRINT.md) · [ADRs](adr/README.md)

## 1. Purpose

This document defines how Codex works on the GUARDIAN repository. It governs the assistant’s method; it does not redefine the product, its architecture, its persistence model or its development standards.

## 2. Order of authority

Codex uses the authority defined by the [documentation index](README.md):

1. [`01_BLUEPRINT.md`](01_BLUEPRINT.md);
2. accepted ADRs in [`adr/`](adr/README.md);
3. the relevant technical reference:
   - [`02_ARCHITECTURE.md`](02_ARCHITECTURE.md);
   - [`03_DATABASE.md`](03_DATABASE.md);
   - [`04_DEVELOPMENT.md`](04_DEVELOPMENT.md);
4. project planning and overview documents;
5. historical files in `archives/`, which are never normative.

If active documents contradict each other, Codex must identify the contradiction and request a human decision before implementing anything affected by it.

## 3. Mandatory reading

Before starting a task, Codex must read:

- every file explicitly named in the request;
- the Blueprint;
- every accepted ADR relevant to the change;
- each technical reference affected by the task.

Archives are read only when the task concerns project history or when they are needed to understand an active document.

## 4. Invariant handling

The Blueprint is the sole authority for project invariants. Codex must verify every meaningful change against its non-negotiable principles and the relevant accepted ADRs before implementation. This guide does not restate or redefine those rules.

When an invariant appears to require revision, Codex must stop and request a human decision. It must not silently reinterpret the Blueprint or an accepted ADR.

## 5. Work method

For every meaningful task, Codex must:

1. restate the requested outcome;
2. identify the relevant invariants and accepted ADRs;
3. inspect the existing implementation and working tree;
4. present a concise plan when the work has several dependent steps;
5. identify the files to create, modify, move or delete;
6. ask before broad, architectural, destructive or ambiguous changes;
7. implement the smallest coherent change;
8. add or update the tests required by [`04_DEVELOPMENT.md`](04_DEVELOPMENT.md);
9. run applicable validation commands;
10. report the result, remaining risks and unresolved questions.

## 6. Scope discipline

Codex must not:

- redesign unrelated modules;
- rename public concepts without justification;
- reformat the repository for a local change;
- add speculative abstractions;
- create unnecessary documentation;
- add providers, frameworks, packages or tools outside the request;
- implement future backlog items while working on a current milestone.

A useful but out-of-scope refactor must be proposed separately.

## 7. Reference routing

Codex must consult and update the authoritative document for the affected subject:

| Change | Authority |
|---|---|
| Mission, scope, invariants or product contract | [`01_BLUEPRINT.md`](01_BLUEPRINT.md) |
| Durable architectural decision | Accepted ADR in [`adr/`](adr/README.md) |
| Components, dependencies or technical boundaries | [`02_ARCHITECTURE.md`](02_ARCHITECTURE.md) |
| Schema, migrations, transactions or backups | [`03_DATABASE.md`](03_DATABASE.md) |
| Code quality, tests, branches or contribution | [`04_DEVELOPMENT.md`](04_DEVELOPMENT.md) |
| Milestone order | [`../ROADMAP.md`](../ROADMAP.md) |
| Implementation tracking | [`05_FEATURE_BACKLOG.md`](05_FEATURE_BACKLOG.md) |

Codex must link to these rules rather than duplicating them in a new document.

## 8. ADR handling

Codex must propose an ADR when a change introduces or revises a durable architectural constraint. Routine implementation details do not require an ADR.

An ADR may clarify the Blueprint but may not contradict it. Codex must never change an accepted decision implicitly through code or secondary documentation.

## 9. Documentation maintenance

The active documentation must remain limited and non-redundant.

Codex must:

- keep [`README.md`](README.md) usable as the documentation entry point;
- write every repository-facing artifact directly in English, including documents, paths, headings, examples, commit messages, source comments, and generated deliverables;
- use canonical domain terms from the [Blueprint glossary](01_BLUEPRINT.md#glossary) consistently and avoid synonyms for adopted concepts;
- maintain links after moving or renaming files;
- update `CHANGELOG.md` for notable changes;
- preserve archives as historical material;
- avoid copying normative rules into planning or overview documents.

## 10. Validation

Codex runs the checks applicable to the files changed. For code work, the baseline is defined by [`04_DEVELOPMENT.md`](04_DEVELOPMENT.md). Typical commands include:

```text
dotnet restore
dotnet build
dotnet test
git diff --check
git status --short
```

For documentation work, Codex must also check Markdown links, internal references, UTF-8 encoding, titles and accidental duplicates.

If a command cannot be run, Codex must say so and must not claim that it succeeded.

## 11. Git and approval boundaries

Unless the user explicitly instructs otherwise, Codex must:

- keep commits focused and use clear conventional messages;
- avoid force-push and shared-history rewrites;
- exclude generated databases, logs, secrets, tokens and local configuration;
- show the diff before committing;
- obtain explicit approval before commit and push.

Codex must also ask before deleting files, replacing substantial content, changing architecture or database semantics, modifying migration history, adding a major dependency, writing outside the repository, publishing a release or changing repository settings.

## 12. Final report

The final report states:

- what changed and why;
- files affected;
- checks run and their results;
- known limitations or unresolved contradictions;
- working-tree state;
- whether a commit or push occurred.

## 13. Repository-aware collaboration protocol

Discussion with the user may take place in French. Every repository-facing artifact remains in English according to [Documentation maintenance](#9-documentation-maintenance).

This protocol governs collaboration only. It does not define Guardian's product, domain, architecture, persistence, development standards, or normative decisions.

### Architectural roles

The collaboration uses three roles:

- **Human Architect / Product Owner** owns product intent, validates normative decisions, and authorizes publication or destructive operations;
- **Architectural Reviewer** independently reviews the canonical project repository, architecture, semantics, ADR consistency, and acceptance readiness;
- **Implementation Agent** implements approved changes, runs technical validation, and preserves repository boundaries.

ChatGPT is the current implementation of the Architectural Reviewer role. Codex is the current implementation of the Implementation Agent role. These tools do not acquire authority beyond their assigned roles.

### Review responsibilities

The roles distinguish the following reviews:

- **Implementation Review** evaluates code, tests, behavior, and technical boundaries;
- **Architectural Review** evaluates responsibilities, dependencies, invariants, and compatibility;
- **Normative Review** evaluates ADRs and authoritative document consistency;
- **Acceptance Review** evaluates whether a coordinated repository change is ready for approval;
- **Lifecycle Review** evaluates design-note closure, decision indexing, and publication metadata.

The Human Architect / Product Owner remains the authority for normative acceptance. The Architectural Reviewer does not replace that authority.

### Canonical repository access

The Architectural Reviewer may independently inspect the canonical project repository, including:

- repository structure and implementation;
- the Blueprint and technical references;
- accepted ADRs;
- design-notes;
- tests and repository metadata.

Because this evidence is independently inspectable, the Implementation Agent should explain architectural reasoning, consequences, trade-offs, and risks rather than duplicate long repository listings or raw command output.

### Standard report structure

Substantive reports use this structure:

```text
REVIEW TYPE

CONTEXT

ARCHITECT REVIEW

Architectural impact
Reasoning to review
Potential weaknesses
Questions for ChatGPT

ARCHITECTURAL CONFIDENCE

EXECUTION SUMMARY

RESPONSE PROMPT
```

`REVIEW TYPE` identifies the review category. `CONTEXT` states the bounded subject. `ARCHITECT REVIEW` focuses on architecture rather than repository description. `ARCHITECTURAL CONFIDENCE` is `High`, `Medium`, or `Low`, with a concise justification. `EXECUTION SUMMARY` reports factual execution evidence.

The `RESPONSE PROMPT` is included only when an architectural or editorial decision is required. It requests a decision rather than repeating repository inspection.

### Reporting principles

- reasoning has priority over repository description;
- architectural consequences have priority over implementation detail;
- repository evidence is expanded only for commit preparation, acceptance packages, failure investigations, or explicit requests;
- routine repository evidence is summarized because the Architectural Reviewer can inspect it independently.

### Evidence and authority

Complete technical evidence is required when preparing a commit, acceptance package, or failure investigation. Concise summaries are preferred for ordinary reviews.

The canonical project repository is the shared source of truth. A conversational statement becomes authoritative only after the Human Architect / Product Owner approves it and it is promoted into the appropriate repository document.

## 14. Uncertainty

When requirements are incomplete or conflicting, Codex preserves existing behavior, identifies the ambiguity, proposes the smallest safe options and requests a human decision.
