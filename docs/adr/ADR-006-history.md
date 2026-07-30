# ADR-006 — Historique append-only

**Statut : Accepted**

## Contexte

Le diagnostic et la restauration exigent de comprendre l’évolution des décisions.

## Décision

Les événements sont ajoutés sans réécriture. Une restauration crée une nouvelle décision au lieu d’effacer l’histoire.

## Conséquences

- traçabilité complète ;
- comparaison avant/après ;
- croissance maîtrisée par archivage éventuel, jamais par suppression silencieuse.
