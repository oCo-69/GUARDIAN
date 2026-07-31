# Decision Index

## Purpose

This file is a non-normative index of explicitly validated and normatively promoted decisions.

It helps readers locate official decisions but never defines or duplicates them. The authoritative rule must exist in the linked document under [`docs/`](../docs/README.md).

No entry may exist without an authoritative reference. If this index conflicts with its linked source, the authoritative source prevails.

## Identifier convention

Entries use stable sequential identifiers beginning with `DEC-0001`, followed by `DEC-0002`, `DEC-0003`, and so on.

Identifiers are never reused, reordered, or renumbered.

## Required entry fields

Every future entry contains:

- stable decision identifier;
- date in `YYYY-MM-DD` format;
- concise title;
- `Validated` status;
- short consequence;
- link to the authoritative document;
- related Git commit SHA once available.

## Entry format

```text
## DEC-NNNN — Concise title

- Date: YYYY-MM-DD
- Status: Validated
- Consequence: Short statement
- Authoritative reference: Relative link under docs/
- Related commit: Full Git commit SHA or Pending
```

The normative source and Decision Index entry must be introduced in the same documentary change. The commit field may remain `Pending` only until that change is committed, after which a follow-up may record the SHA.

## DEC-0001 — Editorial Decision Semantics

- Date: 2026-07-31
- Status: Validated
- Consequence: Editorial authority, immutable Decisions, Applicability, supersession, HistoryEvent distinction, and accepted correspondence now share one normative semantic foundation.
- Authoritative reference: [ADR-011 — Editorial Decision Semantics](../docs/adr/ADR-011-editorial-decision-semantics.md)
- Related commit: Pending
