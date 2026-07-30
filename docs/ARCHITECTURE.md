# Architecture technique

## Objectif

L’architecture doit rendre les invariants du Blueprint difficiles à violer.

Le domaine ne dépend ni de WPF, ni de SQLite, ni de TMDb, ni de Jellyfin. Les dépendances techniques sont injectées derrière des interfaces.

## Solution cible

```text
Guardian.sln
├── Guardian.Domain
├── Guardian.Application
├── Guardian.Infrastructure
├── Guardian.Providers
├── Guardian.Jellyfin
├── Guardian.Desktop
└── Guardian.Tests
```

## Responsabilités

### Guardian.Domain

Contient :

- œuvres et fichiers sources ;
- identités officielles ;
- candidats ;
- décisions ;
- verrous ;
- états ;
- événements métier ;
- règles de transition ;
- invariants.

Ne référence aucun projet technique.

### Guardian.Application

Orchestre les cas d’usage :

- scan ;
- regroupement ;
- recherche ;
- validation ;
- verrouillage ;
- réidentification ;
- restauration ;
- construction ;
- audit.

Dépend du domaine et d’interfaces abstraites.

### Guardian.Infrastructure

Implémente :

- SQLite ;
- accès au système de fichiers ;
- empreintes ;
- création de liens ;
- transactions de construction ;
- journalisation ;
- paramètres et secrets locaux.

### Guardian.Providers

Contient les adaptateurs vers les fournisseurs externes.

Premier module prévu :

```text
Guardian.Providers.Tmdb
```

Il retourne des candidats et des détails minimaux. Il ne peut pas valider une décision.

### Guardian.Jellyfin

Contient :

- conventions de nommage ;
- modèle de projection ;
- génération NFO ;
- lecture optionnelle de Jellyfin en lecture seule ;
- audit de correspondance.

Il n’écrit jamais directement dans la base Jellyfin.

### Guardian.Desktop

Application WPF Windows :

- navigation ;
- affichage des états ;
- commandes utilisateur ;
- configuration ;
- visualisation des erreurs et de l’historique.

L’interface n’implémente pas les règles métier.

### Guardian.Tests

Contient :

- tests unitaires du domaine ;
- tests du parser ;
- tests de migrations SQLite ;
- tests d’intégration sur bibliothèque fictive ;
- tests de construction ;
- tests de non-régression.

## Flux de dépendances

```text
Desktop ───────────────┐
Providers ─────────────┤
Infrastructure ────────┼──> Application ───> Domain
Jellyfin ──────────────┘
```

Le domaine est au centre et ne dépend de rien.

## Pipeline de scan

```text
SourceRoot
  ↓ lecture seule
File Discovery
  ↓
Fingerprint
  ↓
Filename Parser
  ↓
Work Grouper
  ↓
ScanSnapshot
  ↓
Persistence
```

Le scan produit des observations. Il ne remplace aucune décision validée.

## Pipeline d’identification

```text
SourceWork
  ↓
Search Query
  ↓
Provider Adapter
  ↓
Candidates
  ↓
User Selection
  ↓
Validation
  ↓
Decision + HistoryEvent
```

## Pipeline de construction

```text
Validated Decision
  ↓
Build Plan
  ↓
Dry Run / Validation
  ↓
Temporary Projection
  ↓
Verification
  ↓
Safe Switch
  ↓
Generated Manifest + HistoryEvent
```

## Gestion de la concurrence

La v1 peut fonctionner avec une file d’opérations unique pour les écritures.

Règles :

- un seul build actif par racine de destination ;
- transactions SQLite courtes ;
- annulation prise en charge avant le basculement ;
- le scan peut être préparé en arrière-plan mais ses résultats sont appliqués de façon atomique ;
- les événements sont ordonnés.

## Empreintes

Une empreinte stable doit permettre de reconnaître un fichier malgré les scans successifs.

La v1 peut combiner :

- chemin normalisé ;
- taille ;
- date de modification ;
- empreinte partielle optionnelle.

Une empreinte complète n’est calculée que lorsqu’elle apporte une valeur réelle.

## Configuration

Les paramètres non secrets sont stockés dans SQLite ou dans un fichier local versionné par schéma.

Les secrets, notamment le jeton TMDb, doivent être protégés avec les mécanismes Windows appropriés et ne jamais apparaître dans les journaux.

## Journalisation

Deux niveaux :

- message utilisateur clair ;
- détail technique structuré.

Chaque journal doit éviter les secrets et permettre l’anonymisation des chemins pour les rapports de diagnostic.

## Contraintes de conception

- nullable reference types activés ;
- analyse statique activée ;
- méthodes asynchrones pour les entrées/sorties ;
- `CancellationToken` sur les opérations longues ;
- pas d’accès direct à SQLite depuis l’interface ;
- pas d’appel fournisseur depuis le domaine ;
- pas de mutation de décision en dehors du service dédié ;
- pas de construction à partir d’un candidat non validé.
