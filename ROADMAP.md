# Roadmap

This Roadmap defines the order of project milestones. The [`Feature Backlog`](docs/05_FEATURE_BACKLOG.md) decomposes their implementation without changing their priority or scope.

## v1.0.0-alpha.1 — Foundation

Objective: first launchable application.

- .NET solution;
- minimal WPF interface;
- source and destination configuration;
- SQLite and migrations;
- read-only scanner;
- initial parser;
- series and movie grouping;
- dashboard and work list.

## v1.0.0-alpha.2 — Identification

- optional TMDb provider;
- secure token storage;
- candidate search;
- fallback browser search;
- URL paste and identifier entry;
- preview;
- persistent validation.

## v1.0.0-alpha.3 — Protection

- locking and unlocking;
- reidentification;
- append-only history;
- before-and-after comparison;
- restoration of a previous decision;
- explicit forced actions.

## v1.0.0-beta.1 — Build

- build plan;
- dry-run mode;
- Jellyfin directory structure;
- hard links;
- optional symbolic links;
- minimal NFO files;
- temporary build and safe switch;
- per-work or complete reconstruction.

## v1.0.0-beta.2 — Audit

- source / decision / projection consistency;
- detection of missing or modified items;
- conflicts;
- reports;
- optional read-only Jellyfin reader.

## v1.0.0 — Stable

- Windows installer;
- backup and restore;
- robust migrations;
- regression tests;
- user documentation;
- configurable logging;
- validation against a representative real-world library.

## After v1

Changes will be integrated only when they serve the primary mission.

Possible directions:

- additional providers;
- improved grouping rules;
- Guardian Companion for more convenient desktop web navigation;
- discovery intelligence based on relationships already present in the collection.

These directions must never weaken the core: memory, locking, reconstruction, and an inviolable source.
