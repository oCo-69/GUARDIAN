# Documentation GUARDIAN

Ce répertoire est le point d’entrée officiel de la documentation du projet.

## Parcours humain recommandé

1. [`01_BLUEPRINT.md`](01_BLUEPRINT.md) — mission, périmètre, invariants et vocabulaire.
2. [`adr/`](adr/README.md) — décisions d’architecture acceptées.
3. [`02_ARCHITECTURE.md`](02_ARCHITECTURE.md) — composants et dépendances.
4. [`03_DATABASE.md`](03_DATABASE.md) — mémoire persistante SQLite.
5. [`04_DEVELOPMENT.md`](04_DEVELOPMENT.md) — contribution, tests et livraison.
6. [`05_FEATURE_BACKLOG.md`](05_FEATURE_BACKLOG.md) — décomposition technique du travail planifié.

## Parcours Codex recommandé

1. [`00_CODEX_GUIDELINES.md`](00_CODEX_GUIDELINES.md) — méthode de travail de Codex.
2. [`01_BLUEPRINT.md`](01_BLUEPRINT.md) — autorité principale du projet.
3. les [ADR](adr/README.md) pertinents pour la tâche ;
4. les références techniques concernées parmi l’Architecture, la Base et le guide de Développement ;
5. [`05_FEATURE_BACKLOG.md`](05_FEATURE_BACKLOG.md) lorsque la tâche concerne la planification.

## Rôle et autorité

| Document | Sujet dont il est la référence | Statut |
|---|---|---|
| [`01_BLUEPRINT.md`](01_BLUEPRINT.md) | Mission, périmètre, invariants et contrat de conception | Normatif, autorité principale |
| [`adr/`](adr/README.md) | Décisions d’architecture durables et leur justification | Normatif lorsque l’ADR est accepté, sous l’autorité du Blueprint |
| [`02_ARCHITECTURE.md`](02_ARCHITECTURE.md) | Composants, responsabilités et dépendances | Normatif pour l’architecture technique |
| [`03_DATABASE.md`](03_DATABASE.md) | Modèle de données, transactions, migrations et sauvegarde | Normatif pour la persistance |
| [`04_DEVELOPMENT.md`](04_DEVELOPMENT.md) | Pratiques de développement, tests et contribution | Normatif pour le développement |
| [`00_CODEX_GUIDELINES.md`](00_CODEX_GUIDELINES.md) | Méthode de travail de Codex dans ce dépôt | Normatif pour Codex, sans pouvoir redéfinir le projet |
| [`05_FEATURE_BACKLOG.md`](05_FEATURE_BACKLOG.md) | Épics, fonctionnalités et suivi d’implémentation | Document de planification, non normatif |

En cas de contradiction, le Blueprint prévaut. Un ADR accepté précise le Blueprint sans pouvoir le contredire. Les références techniques s’appliquent ensuite dans leur domaine. Toute contradiction entre documents actifs doit être signalée et soumise à une décision humaine.

## Documents à la racine

- [`../README.md`](../README.md) présente le projet et dirige vers cette documentation.
- [`../MANIFESTO.md`](../MANIFESTO.md) expose l’intention fondatrice.
- [`../ROADMAP.md`](../ROADMAP.md) fixe l’ordre des jalons ; le backlog en détaille le travail sans modifier cet ordre.
- [`../CHANGELOG.md`](../CHANGELOG.md) consigne les changements notables.

## Archives

Les fichiers de [`../archives/`](../archives/README.md) conservent l’histoire du projet. Ils ne sont jamais normatifs et ne doivent pas servir à trancher une décision actuelle.
