# Design Notes Collaboration Protocol

## Purpose

The `design-notes/` directory is Guardian's Git-tracked workspace for design exploration, refinement, and historical synthesis.

Everything in this directory is non-normative. No file under `design-notes/` defines an official product, domain, architecture, persistence, development, or technical rule. The authoritative sources remain the official documents under [`docs/`](../docs/README.md).

If `design-notes/` and `docs/` disagree, `docs/` always prevails. Speculative content from this directory cannot justify implementation work by itself.

Codex-specific repository rules are defined in [`docs/00_CODEX_GUIDELINES.md`](../docs/00_CODEX_GUIDELINES.md) and are not duplicated here.

## File roles

| File | Role |
|---|---|
| [`000_CURRENT_TOPIC.md`](000_CURRENT_TOPIC.md) | Contains only the topic currently being explored. It may be rewritten as the discussion evolves. |
| [`001_HISTORY.md`](001_HISTORY.md) | Preserves an append-only, concise synthesis after a topic is closed or explicitly abandoned. |
| [`002_DECISIONS.md`](002_DECISIONS.md) | Indexes explicitly validated decisions only after their normative promotion. |
| [`003_TODO.md`](003_TODO.md) | Lists unresolved questions and future design actions without duplicating the active reasoning. |
| `HANDSHAKE.md` | Defines this collaboration protocol and nothing about Guardian's product or technical design. |

## Single-location rule

An idea belongs in only one design-notes file at each stage of its lifecycle:

- active reasoning belongs in `000_CURRENT_TOPIC.md`;
- unresolved questions and deferred actions belong in `003_TODO.md`;
- a closed topic receives one concise record in `001_HISTORY.md`;
- a validated and normatively promoted decision receives one index entry in `002_DECISIONS.md`.

Files must link to one another when context is needed instead of copying the same reasoning.

## Lifecycle of an idea

1. A new idea becomes the active subject in `000_CURRENT_TOPIC.md`.
2. Discussion and refinement update that file.
3. The human project owner explicitly validates, rejects, or defers the conclusion.
4. A validated rule is promoted into the appropriate authoritative document under `docs/`.
5. `001_HISTORY.md` receives a concise historical record.
6. `002_DECISIONS.md` receives an index entry when the conclusion qualifies as a decision.
7. Resolved items are removed from `003_TODO.md`.
8. `000_CURRENT_TOPIC.md` is reset for the next topic.
9. A focused commit is proposed.

Validation in conversation alone does not create an official rule. Normative promotion is required.

## Normative promotion

A product, domain, architecture, persistence, development, or technical rule becomes official only when it is integrated into the appropriate authoritative document under `docs/`.

The normative source and its related `002_DECISIONS.md` entry must be introduced in the same documentary change. A Decision Index entry must never claim validated status before its authoritative source exists.

## Decision identifiers

Decision Index entries use stable, sequential identifiers:

```text
DEC-0001
DEC-0002
DEC-0003
```

Identifiers are never reused or renumbered. Every entry includes:

- identifier;
- date;
- concise title;
- validated status;
- short consequence;
- authoritative reference;
- related Git commit SHA once available.

The Decision Index points to authority; it never duplicates the normative decision.

## Commit preparation

When a topic is ready to close:

1. verify explicit human validation;
2. update the appropriate normative source;
3. update the relevant design-notes lifecycle files without duplication;
4. remove resolved TODO items;
5. reset the current topic when promotion is complete;
6. verify links, English-only content, canonical terminology, encoding, and whitespace;
7. present the complete diff;
8. obtain explicit approval before staging, committing, or pushing.

Commits should remain focused on one stabilized topic.

## Prompt exchange convention

The collaboration uses a symmetric copy boundary:

- a message intended for Codex ends with `PROMPT FOR CODEX`;
- Codex responses follow the authoritative [response protocol](../docs/00_CODEX_GUIDELINES.md#13-response-protocol);
- when a response from ChatGPT is needed, the final `RESPONSE PROMPT` block is the only text that needs to be copied.

This exchange convention controls conversation handoff only. It does not create or validate a project decision.
