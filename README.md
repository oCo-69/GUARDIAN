# GUARDIAN

**Guardian is a local companion for Jellyfin.**

It connects the actual organization of a media collection to the official identities of its works, remembers user-validated choices, protects those decisions, and builds a virtual library that Jellyfin can recognize correctly.

> **Guardian proposes. The user decides. Guardian remembers, locks, and rebuilds.**

## The problem

A personal collection can be perfectly coherent for its owner while remaining difficult for Jellyfin to interpret.

Example source file:

```text
Blue Gender - [01x01][Jap&Eng][STEN][570p DVDRip] - One Day.mkv
```

Guardian does not rename this file. It builds a derived representation:

```text
Blue Gender (1999) [tmdbid-...]
└── Season 01
    └── Blue Gender S01E01 - One Day.mkv
```

The projected video is a link to the original. The source remains intact.

## What Guardian is

Guardian is:

- a local Windows application;
- a memory of the mappings between local media and official identities;
- an identification assistant;
- a Jellyfin virtual library generator;
- a tool for control, history, and reconstruction.

## What Guardian is not

Guardian is not:

- a replacement for Jellyfin;
- a new encyclopedic database;
- a manager that renames or moves original media;
- an engine that silently applies its assumptions;
- a tool that writes directly to the Jellyfin database.

## Essential principles

1. **Inviolable source** — no original is renamed, moved, rewritten, or deleted.
2. **User authority** — a Candidate is never a Decision.
3. **Explicit validation** — an identity must be validated before it can be used.
4. **Protective locking** — a locked decision cannot be replaced automatically.
5. **Preserved history** — changes remain explainable and restorable.
6. **Reconstructible output** — the virtual library can be deleted and rebuilt.
7. **Safe failure** — when uncertainty remains, Guardian stops on the affected case.

## First testable version

The first version must allow the user to:

- choose a source library and a destination;
- scan the collection;
- group episodes by series;
- isolate movies and ambiguous cases;
- search for TMDb candidates;
- validate an identity;
- lock or reidentify a work;
- rebuild a work in a virtual library;
- inspect history and conflicts;
- verify that the source has never been modified.

## Documentation architecture

The official entry point is [`docs/README.md`](docs/README.md). It defines the reading order, the role of each document, and its level of authority.

The [Manifesto](MANIFESTO.md) states the founding intent, the [Roadmap](ROADMAP.md) orders the milestones, and the [Changelog](CHANGELOG.md) records notable changes.

Documents that supported the initial design work are preserved in [`archives/`](archives/README.md). They are not normative.

## Target structure

```text
GUARDIAN/
├── README.md
├── MANIFESTO.md
├── ROADMAP.md
├── CHANGELOG.md
├── docs/
│   ├── README.md
│   ├── 00_CODEX_GUIDELINES.md
│   ├── 01_BLUEPRINT.md
│   ├── 02_ARCHITECTURE.md
│   ├── 03_DATABASE.md
│   ├── 04_DEVELOPMENT.md
│   ├── 05_FEATURE_BACKLOG.md
│   └── adr/
├── archives/
├── src/
├── tests/
└── tools/
```

## Status

The project is in the **v1.0.0-alpha.1** foundation phase.

The immediate objective is a testable Windows application focused on scanning, work grouping, assisted TMDb identification, and local persistence of decisions.
