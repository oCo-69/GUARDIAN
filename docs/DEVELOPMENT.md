# Guide de développement

## Prérequis prévus

- Windows 11 ;
- .NET SDK LTS retenu par le projet ;
- Visual Studio ou un éditeur compatible .NET ;
- Git ;
- SQLite embarqué par l’application ;
- compte TMDb facultatif pour tester le fournisseur.

La version précise du SDK devra être fixée dans `global.json` au démarrage du code.

## Démarrage du dépôt

Structure attendue :

```text
src/
tests/
tools/
docs/
archives/
```

Le code applicatif ne doit pas être placé à la racine.

## Règle de contribution

Avant de développer une fonctionnalité :

1. vérifier sa conformité au Blueprint ;
2. identifier les invariants concernés ;
3. créer un ADR lorsqu’une décision d’architecture nouvelle est nécessaire ;
4. écrire ou mettre à jour les tests ;
5. implémenter ;
6. vérifier qu’aucune écriture source n’est possible.

## Qualité C#

- nullable reference types activés ;
- warnings traités comme erreurs dans les projets principaux ;
- analyseurs .NET activés ;
- noms explicites ;
- petites méthodes ;
- dépendances injectées ;
- interfaces aux frontières techniques ;
- aucune logique métier dans le code-behind WPF ;
- opérations d’entrée/sortie asynchrones ;
- prise en charge de l’annulation ;
- messages utilisateur séparés des détails techniques.

## Tests minimaux

### Domaine

- impossible de valider sans identité ;
- impossible de construire depuis un candidat ;
- impossible de remplacer une décision verrouillée ;
- le déverrouillage crée un événement ;
- une restauration crée une nouvelle décision ;
- une suggestion ne modifie pas l’état validé.

### Parser

Tests fondés sur les conventions réelles observées :

```text
[01x01]
S01E01
Season 01
[24xFull]
[Jap&Eng]
[STEN]
```

Le parser doit exposer son niveau de confiance et ne pas deviner lorsque les données sont contradictoires.

### Construction

- source et destination sur le même volume ;
- volumes différents ;
- collision de noms ;
- destination inaccessible ;
- lien existant ;
- build annulé ;
- build partiellement échoué ;
- reconstruction déterministe ;
- source inchangée avant et après.

### SQLite

- création de base ;
- migrations successives ;
- rollback sur erreur ;
- décisions et historique préservés ;
- sauvegarde et restauration.

## Branches

Organisation simple recommandée :

- `main` : état stable ou démontrable ;
- branches courtes par fonctionnalité ;
- pull request pour toute intégration notable.

Éviter une branche `develop` longue tant que l’équipe est réduite.

## Commits

Format recommandé :

```text
type(scope): description
```

Exemples :

```text
feat(scanner): add read-only source discovery
fix(lock): reject automatic replacement of locked decisions
docs(blueprint): clarify TMDb candidate workflow
test(builder): cover cross-volume failure
```

Types principaux :

- `feat`
- `fix`
- `docs`
- `test`
- `refactor`
- `build`
- `chore`

## Pull requests

Une pull request doit indiquer :

- problème traité ;
- solution ;
- invariants concernés ;
- tests ajoutés ;
- impact sur la base ;
- impact sur la source ;
- captures pour les changements d’interface ;
- ADR associé, le cas échéant.

## Définition de terminé

Une tâche est terminée lorsque :

- elle compile sans avertissement bloquant ;
- les tests passent ;
- la source est protégée par conception et par test ;
- les erreurs sont compréhensibles ;
- la documentation active reste cohérente ;
- aucun secret n’est journalisé ;
- le changement est essayable.

## Versionnage

Le projet utilise le versionnage sémantique à partir de `1.0.0`.

Avant la version stable :

```text
1.0.0-alpha.1
1.0.0-alpha.2
1.0.0-beta.1
```

## Documentation

La documentation active doit rester courte.

Une information normative n’existe qu’à un seul endroit. Les documents historiques restent dans `archives/` et ne doivent pas être cités comme règle actuelle.
