# Roadmap

Cette Roadmap fixe l’ordre des jalons du projet. Le [`Feature Backlog`](docs/05_FEATURE_BACKLOG.md) décompose leur implémentation sans modifier leur priorité ni leur portée.

## v1.0.0-alpha.1 — Fondation

Objectif : première application lançable.

- solution .NET ;
- interface WPF minimale ;
- configuration source et destination ;
- SQLite et migrations ;
- scanner en lecture seule ;
- parser initial ;
- regroupement séries et films ;
- tableau de bord et liste des œuvres.

## v1.0.0-alpha.2 — Identification

- fournisseur TMDb optionnel ;
- stockage sécurisé du jeton ;
- recherche de candidats ;
- recherche navigateur de repli ;
- collage d’URL et saisie d’identifiant ;
- aperçu ;
- validation persistante.

## v1.0.0-alpha.3 — Protection

- verrouillage et déverrouillage ;
- réidentification ;
- historique append-only ;
- comparaison avant/après ;
- restauration d’une décision antérieure ;
- actions forcées explicites.

## v1.0.0-beta.1 — Construction

- plan de construction ;
- mode simulation ;
- arborescence Jellyfin ;
- liens physiques ;
- liens symboliques optionnels ;
- NFO minimaux ;
- construction temporaire et basculement sûr ;
- reconstruction par œuvre ou complète.

## v1.0.0-beta.2 — Audit

- cohérence source / décision / projection ;
- détection des éléments absents ou modifiés ;
- conflits ;
- rapports ;
- lecteur Jellyfin optionnel en lecture seule.

## v1.0.0 — Stable

- installateur Windows ;
- sauvegarde et restauration ;
- migrations robustes ;
- tests de non-régression ;
- documentation utilisateur ;
- journalisation configurable ;
- validation sur une bibliothèque réelle représentative.

## Après la v1

Les évolutions ne seront intégrées que si elles servent la mission principale.

Pistes possibles :

- fournisseurs supplémentaires ;
- amélioration des règles de regroupement ;
- Guardian Companion pour le confort de navigation web desktop ;
- intelligence de découverte fondée sur les liens déjà présents dans la collection.

Ces pistes ne doivent jamais fragiliser le cœur : mémoire, verrouillage, reconstruction et source inviolable.
