# GUARDIAN

**Guardian est un compagnon local pour Jellyfin.**

Il relie l’organisation réelle d’une collection multimédia aux identités officielles des œuvres, mémorise les choix validés par l’utilisateur, protège ces décisions et construit une bibliothèque virtuelle que Jellyfin peut reconnaître correctement.

> **Guardian propose. L’utilisateur décide. Guardian mémorise, verrouille et reconstruit.**

## Le problème

Une collection personnelle peut avoir une organisation parfaitement cohérente pour son propriétaire, tout en restant difficile à interpréter pour Jellyfin.

Exemple de fichier source :

```text
Blue Gender - [01x01][Jap&Eng][STEN][570p DVDRip] - One Day.mkv
```

Guardian ne renomme pas ce fichier. Il construit une représentation dérivée :

```text
Blue Gender (1999) [tmdbid-...]
└── Season 01
    └── Blue Gender S01E01 - One Day.mkv
```

La vidéo projetée est un lien vers l’original. La source reste intacte.

## Ce que Guardian est

Guardian est :

- une application Windows locale ;
- une mémoire des correspondances entre médias locaux et identités officielles ;
- un assistant d’identification ;
- un générateur de bibliothèque virtuelle Jellyfin ;
- un outil de contrôle, d’historique et de reconstruction.

## Ce que Guardian n’est pas

Guardian n’est pas :

- un remplacement de Jellyfin ;
- une nouvelle base encyclopédique ;
- un gestionnaire qui renomme ou déplace les originaux ;
- un moteur qui applique silencieusement ses suppositions ;
- un outil qui écrit directement dans la base de données Jellyfin.

## Principes essentiels

1. **Source inviolable** — aucun original n’est renommé, déplacé, réécrit ou supprimé.
2. **Autorité utilisateur** — une suggestion n’est jamais une décision.
3. **Validation explicite** — une identité doit être validée avant d’être utilisée.
4. **Verrouillage protecteur** — une décision verrouillée ne peut pas être remplacée automatiquement.
5. **Historique conservé** — les changements restent explicables et restaurables.
6. **Sortie reconstructible** — la bibliothèque virtuelle peut être supprimée puis recréée.
7. **Échec sûr** — en cas d’incertitude, Guardian s’arrête sur le cas concerné.

## Première version essayable

La première version doit permettre de :

- choisir une bibliothèque source et une destination ;
- scanner la collection ;
- regrouper les épisodes par série ;
- isoler les films et les cas ambigus ;
- rechercher des candidats TMDb ;
- valider une identité ;
- verrouiller ou réidentifier une œuvre ;
- reconstruire une œuvre dans une bibliothèque virtuelle ;
- consulter l’historique et les conflits ;
- vérifier que la source n’a jamais été modifiée.

## Architecture documentaire

La documentation active est volontairement limitée :

| Document | Rôle |
|---|---|
| [MANIFESTO.md](MANIFESTO.md) | Pourquoi Guardian existe |
| [docs/BLUEPRINT.md](docs/BLUEPRINT.md) | Règles et contrat de conception |
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | Organisation technique |
| [docs/DATABASE.md](docs/DATABASE.md) | Modèle SQLite |
| [docs/DEVELOPMENT.md](docs/DEVELOPMENT.md) | Développement et contribution |
| [ROADMAP.md](ROADMAP.md) | Étapes du projet |
| [CHANGELOG.md](CHANGELOG.md) | Évolution des versions |

Les documents qui ont servi à la réflexion initiale sont conservés dans [`archives/`](archives/README.md), mais ils ne constituent plus la référence normative.

## Structure cible

```text
GUARDIAN/
├── README.md
├── MANIFESTO.md
├── ROADMAP.md
├── CHANGELOG.md
├── docs/
│   ├── BLUEPRINT.md
│   ├── ARCHITECTURE.md
│   ├── DATABASE.md
│   ├── DEVELOPMENT.md
│   └── adr/
├── archives/
├── src/
├── tests/
└── tools/
```

## Statut

Le projet est en phase de fondation de la **v1.0-alpha 1**.

L’objectif immédiat est une application Windows essayable, centrée sur le scan, le regroupement des œuvres, l’identification TMDb assistée et la persistance locale des décisions.
