# Guardian v1 — Blueprint

> [Documentation](README.md) · [ADR](adr/README.md)

## 1. Statut

Ce document est le contrat de conception de Guardian v1.

Toute fonctionnalité doit respecter les invariants, responsabilités et limites définis ici. Lorsqu’une décision nouvelle modifie ce contrat, elle doit être documentée par un ADR.

## 2. Mission

Guardian relie sans ambiguïté l’organisation locale d’une collection aux identités officielles des œuvres, puis construit une représentation que Jellyfin reconnaît correctement, sans toucher aux originaux.

Correspondances fondamentales :

```text
Série : œuvre source ↔ identité officielle d’une série
Film  : œuvre source ↔ identité officielle d’un film
```

Les épisodes héritent de l’identité de leur série. Ils ne sont pas identifiés manuellement un par un.

## 3. Périmètre de la v1

### Inclus

- scan d’un ou plusieurs répertoires sources en lecture seule ;
- détection des séries, films, épisodes, spéciaux et cas ambigus ;
- regroupement des épisodes sous une œuvre série ;
- recherche de candidats via TMDb lorsque l’accès API est configuré ;
- recherche manuelle par navigateur comme solution de repli ;
- saisie directe d’un identifiant ou collage d’une URL ;
- validation, verrouillage, déverrouillage et réidentification ;
- historique append-only ;
- bibliothèque virtuelle Jellyfin par liens ;
- noms compatibles, dossiers de saisons et NFO minimaux ;
- audit entre source, mémoire Guardian et sortie générée.

### Exclus de la première livraison

- modification de la bibliothèque source ;
- modification directe de la base Jellyfin ;
- encyclopédie locale complète ;
- scraping massif ;
- validation automatique irréversible ;
- remplacement de Jellyfin ;
- gestion exhaustive des affiches, acteurs, synopsis et notes ;
- fonctions TV natives autres que celles déjà offertes par Jellyfin.

## 4. Invariants non négociables

### P1 — Source inviolable

Guardian n’écrit jamais dans les répertoires sources.

### P2 — Autorité utilisateur

Guardian peut proposer, jamais imposer. Une proposition ne devient une décision qu’après validation explicite.

### P3 — Identité au niveau de l’œuvre

Une série est identifiée une fois. Ses épisodes héritent de cette identité et sont rattachés par leurs numéros de saison et d’épisode.

### P4 — Verrouillage protecteur

Une décision verrouillée ne peut être remplacée par un scan, un import, une règle, un fournisseur ou une suggestion.

### P5 — Historique append-only

Une décision antérieure n’est pas effacée silencieusement. Toute évolution crée un nouvel événement et, lorsqu’il y a remplacement, une nouvelle décision liée à la précédente.

### P6 — Sortie dérivée

La bibliothèque virtuelle, les NFO, les manifestes et les rapports sont dérivés. Ils doivent pouvoir être supprimés puis recréés.

### P7 — Mémoire minimale

Guardian conserve les identités, décisions, verrous, scans, constructions et événements nécessaires. Il ne maintient pas une encyclopédie parallèle.

### P8 — Échec sûr

En cas d’incertitude, Guardian isole le cas concerné et demande une décision. Il ne dégrade ni la source ni une projection valide existante.

## 5. Modèle conceptuel

| Objet | Responsabilité |
|---|---|
| `SourceRoot` | Racine protégée contenant les originaux |
| `SourceWork` | Œuvre logique locale : série, film ou cas ambigu |
| `SourceFile` | Fichier original identifié par chemin et empreinte |
| `EpisodeDescriptor` | Saison, épisode et titre local déduits du fichier |
| `ProviderIdentity` | Fournisseur, type et identifiant officiel |
| `Candidate` | Proposition non validée |
| `Decision` | Choix validé reliant une œuvre locale à une identité |
| `Lock` | Protection fonctionnelle d’une décision |
| `HistoryEvent` | Trace horodatée d’une action ou d’un changement |
| `GeneratedItem` | Élément créé dans la projection |
| `ScanSnapshot` | État logique d’un scan |
| `BuildRun` | Exécution d’une construction |

## Glossaire

### Source concepts

- **Source** — collection originale de médias que Guardian observe sans la modifier.
- **SourceRoot** — racine configurée en lecture seule contenant une partie de la Source.
- **SourceWork** — regroupement logique local de fichiers représentant une œuvre ou un cas encore ambigu.
- **SourceFile** — fichier média original observé dans un SourceRoot.
- **Work** — œuvre média servant d’unité d’identité, indépendamment du nombre de fichiers associés.
- **Series** — Work composée d’épisodes qui héritent d’une même identité de série.
- **Movie** — Work représentant un film unique, portée par un fichier ou un groupe déclaré équivalent.
- **Episode** — élément d’une Series repéré par ses numéros de saison et d’épisode.

### Identity concepts

- **Provider** — adaptateur optionnel vers une source externe d’identités et de métadonnées minimales.
- **Candidate** — proposition non validée reliant un SourceWork à une ProviderIdentity possible.
- **ProviderIdentity** — identité officielle minimale et normalisée fournie par un Provider.
- **Decision** — choix validé reliant un SourceWork à une ProviderIdentity.
- **Lock** — protection fonctionnelle empêchant le remplacement automatique d’une Decision.
- **HistoryEvent** — trace append-only d’une action ou d’un changement significatif.

### Projection concepts

- **Projection** — bibliothèque Jellyfin dérivée, jetable et reconstructible.
- **Build** — exécution qui construit ou reconstruit tout ou partie de la Projection.
- **Audit** — comparaison entre la Source, les Decision et la Projection, éventuellement complétée par une lecture de Jellyfin.

## 6. Workflow principal

```text
Scanner
  ↓
Regrouper les œuvres
  ↓
Rechercher des candidats
  ↓
Présenter
  ↓
Validation utilisateur
  ↓
Verrouillage facultatif
  ↓
Construction
  ↓
Audit
```

Aucun candidat ne peut être utilisé par le constructeur avant validation.

## 7. Règles d’identification

### Séries

La décision porte sur l’œuvre série, généralement issue d’un dossier ou d’un groupe logique.

```text
G:\_MANGAS\Dororo
  ↕
TMDb / TV / 83100
```

Tous les épisodes associés héritent de cette identité.

### Films

La décision porte sur un fichier ou un groupe explicitement déclaré comme représentant le même film.

### Spéciaux, OVA et ambiguïtés

Guardian peut proposer une catégorie, mais ne transforme pas silencieusement un spécial en film ou en épisode lorsque ce choix affecte l’organisation Jellyfin.

## 8. Fournisseurs et TMDb

TMDb est un fournisseur optionnel de candidats et de métadonnées minimales.

Lorsqu’un jeton est configuré :

1. Guardian construit une requête à partir du titre local, de l’année probable et du type d’œuvre.
2. Le fournisseur retourne des candidats.
3. Guardian les présente avec leurs informations distinctives.
4. L’utilisateur choisit et valide.
5. Guardian enregistre l’identité normalisée.

Sans API, Guardian doit rester utilisable grâce à une recherche ouverte dans le navigateur et au collage d’une URL ou d’un identifiant.

Une réponse d’API est toujours une proposition, jamais une validation.

## 9. États

États principaux :

```text
Découverte → À identifier → À vérifier → Validée → Verrouillée
```

États complémentaires :

- `Conflit` : la projection ou Jellyfin ne correspond pas à la décision ;
- `Obsolète` : la source a suffisamment changé pour nécessiter une revue ;
- `Erreur` : une opération a échoué sans dégrader les acquis.

## 10. Verrouillage

Le verrou est appliqué dans le domaine, pas seulement dans l’interface.

- une décision verrouillée reste visible et auditable ;
- un conflit peut être signalé sans modifier la décision ;
- toute réidentification exige un déverrouillage explicite ;
- verrouillage et déverrouillage créent des événements ;
- restaurer une ancienne décision crée une nouvelle décision ;
- aucune automatisation ne contourne le verrou.

## 11. Historique

L’historique doit répondre à quatre questions :

1. Qu’est-ce qui a changé ?
2. Quand ?
3. Pourquoi ou par quelle action ?
4. Quel était l’état avant et après ?

Événements minimaux :

- scan ;
- proposition ;
- validation ;
- verrouillage ;
- déverrouillage ;
- réidentification ;
- construction ;
- conflit ;
- restauration ;
- erreur.

## 12. Bibliothèque virtuelle

Exemple :

```text
G:\_JELLYFIN_MANGAS\
└── Dororo (2019) [tmdbid-83100]\
    ├── tvshow.nfo
    └── Season 01\
        ├── Dororo S01E01 - Daigo.mkv
        ├── Dororo S01E01 - Daigo.nfo
        └── ...
```

Règles :

- liens physiques sur un même volume ;
- liens symboliques uniquement après choix explicite et vérification des droits ;
- aucune copie vidéo par défaut ;
- écriture limitée à la racine de projection Guardian ;
- construction temporaire avant basculement lorsqu’une opération concerne plusieurs éléments ;
- manifeste de chaque élément généré ;
- noms déterministes ;
- NFO minimaux destinés à renforcer l’identité.

## 13. Services applicatifs

| Service | Contrat essentiel |
|---|---|
| `ScanLibrary` | Lit les sources et produit un snapshot ; aucune écriture source |
| `GroupWorks` | Regroupe les fichiers sous des œuvres logiques |
| `SearchCandidates` | Retourne des propositions ; ne crée aucune décision |
| `ParseProviderReference` | Normalise une URL ou un identifiant |
| `ValidateDecision` | Crée une décision validée et un événement |
| `LockDecision` | Protège une décision validée |
| `ReidentifyWork` | Refuse si verrouillé ; conserve l’ancienne décision |
| `RestoreDecision` | Crée une nouvelle décision à partir de l’historique |
| `BuildWork` | Construit uniquement une œuvre validée |
| `BuildLibrary` | Construit les œuvres éligibles et isole les échecs |
| `AuditWork` | Compare source, décision, projection et éventuellement Jellyfin |

## 14. Interface de la v1

Écrans principaux :

- Tableau de bord ;
- Bibliothèque ;
- Identification ;
- Détail d’une œuvre ;
- Historique ;
- Paramètres.

Actions visibles par œuvre :

- identifier ;
- valider ;
- verrouiller ou déverrouiller ;
- réidentifier ;
- saisir une référence fournisseur ;
- ouvrir le résultat ;
- reconstruire ;
- auditer ;
- consulter l’historique ;
- restaurer une décision antérieure.

## 15. Gestion des erreurs

- une erreur sur une œuvre ne bloque pas les autres ;
- une projection valide n’est pas supprimée avant que sa remplaçante soit prête ;
- les erreurs ont un message clair et un détail technique copiable ;
- les opérations multi-étapes utilisent des transactions logiques ;
- les écritures SQLite utilisent des transactions ;
- les conflits de noms, volumes incompatibles et droits insuffisants sont détectés avant écriture ;
- toute construction propose un mode simulation.

## 16. Sécurité et confidentialité

- aucune télémétrie obligatoire ;
- aucun envoi des chemins locaux vers un serveur Guardian ;
- secrets de fournisseurs stockés via un mécanisme sécurisé du système ;
- secrets absents des journaux et rapports ;
- sauvegarde simple de la base locale ;
- journaux nettoyables et configurables ;
- aucune lecture silencieuse du navigateur ;
- confidentialité explicite des documents et captures du projet.

## 17. Critères d’acceptation de la première version essayable

1. L’application démarre sans ligne de commande.
2. L’utilisateur choisit source et destination.
3. Le scan regroupe correctement les épisodes et isole les films.
4. Une série n’est jamais identifiée épisode par épisode.
5. TMDb peut proposer des candidats lorsqu’il est configuré.
6. Une URL ou un identifiant peut être saisi manuellement.
7. La validation crée une décision persistante.
8. Le verrou empêche toute substitution accidentelle.
9. La réidentification conserve l’ancienne décision.
10. La reconstruction crée une arborescence Jellyfin, des liens et des NFO minimaux.
11. Une suppression complète de la projection suivie d’une reconstruction produit le même résultat logique.
12. Aucune opération ne modifie la source.
13. Un audit distingue les œuvres validées, verrouillées, conflictuelles et à traiter.

## 18. Gouvernance

Avant d’accepter une fonctionnalité, vérifier qu’elle :

- aide directement la reconnaissance ou la maintenance de la bibliothèque ;
- respecte l’inviolabilité de la source ;
- préserve l’autorité de l’utilisateur ;
- respecte les verrous ;
- produit une sortie reconstructible ;
- évite de refaire inutilement un service existant ;
- reste traçable ;
- peut échouer sans dégrader les acquis.

Lorsqu’une réponse est négative, la fonctionnalité doit être modifiée, isolée comme module optionnel ou rejetée.
