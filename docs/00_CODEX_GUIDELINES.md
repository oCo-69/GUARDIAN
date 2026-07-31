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
- write every repository document, source comment, and commit message directly in English;
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

## 13. Uncertainty

When requirements are incomplete or conflicting, Codex preserves existing behavior, identifies the ambiguity, proposes the smallest safe options and requests a human decision.
