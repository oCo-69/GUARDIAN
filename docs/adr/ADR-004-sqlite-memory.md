# ADR-004 — SQLite conserve la mémoire minimale

**Statut : Accepted**

## Contexte

Guardian doit persister les décisions sans maintenir une encyclopédie locale.

## Décision

SQLite conserve les œuvres locales, identités minimales, décisions, verrous, événements, scans et constructions.

## Conséquences

- base légère et sauvegardable ;
- schéma versionné ;
- absence de duplication exhaustive des données fournisseur.
