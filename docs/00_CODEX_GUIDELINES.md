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

## 13. Response protocol

Discussion with the user may take place in French. The repository-facing language rule in [Documentation maintenance](#9-documentation-maintenance) always remains in force.

Every substantive response uses the following sections in this order:

1. `ARCHITECT REVIEW`
2. `EXECUTION SUMMARY`
3. `RESPONSE PROMPT`, only when a reply from ChatGPT is needed

### ARCHITECT REVIEW

This section contains exactly these subsections:

- `Reasoning to review:` summarizes only important reasoning, trade-offs, assumptions, rejected alternatives, and why the selected approach was preferred;
- `Potential weaknesses:` identifies unresolved risks, fragile assumptions, possible inconsistencies, and potential conflicts with Guardian's vision, domain language, architecture, or documentation authority; it states `None identified` when appropriate;
- `Questions for ChatGPT:` contains only questions requiring conceptual, architectural, editorial, or domain review; it states `None` when no review is needed.

Routine execution details do not belong in `ARCHITECT REVIEW`.

### EXECUTION SUMMARY

This section provides concise factual evidence:

- files created, modified, moved, or deleted;
- commands and checks performed;
- validation results;
- commit and push status when applicable.

Command results and checks are reported execution evidence. They must not be presented as independently verified by ChatGPT.

Long raw logs, full command transcripts, and very large diffs are omitted unless the user explicitly requests them. When approval requires a diff or complete file content, it is presented separately and completely without expanding `ARCHITECT REVIEW`.

### RESPONSE PROMPT

This section is included only when a reply from ChatGPT is needed.

It:

- is the final section of the response;
- contains one fenced `text` block and no other content;
- contains only the exact message intended for ChatGPT;
- preserves reasoning, reservations, decisions, unresolved questions, and concise execution results needed for the next architectural or editorial review;
- excludes greetings, user commentary, raw logs, routine details, and redundant information;
- has no text after the fenced block.

When no reply from ChatGPT is needed, the entire `RESPONSE PROMPT` section is omitted.

## 14. Uncertainty

When requirements are incomplete or conflicting, Codex preserves existing behavior, identifies the ambiguity, proposes the smallest safe options and requests a human decision.
