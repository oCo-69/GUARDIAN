# Guardian v1 — Blueprint d’architecture (archive antérieure)

Ce fichier conserve le contenu essentiel du blueprint produit avant la consolidation documentaire.

## Mission

Guardian relie l’organisation locale d’une collection aux identités officielles, puis génère une représentation que Jellyfin reconnaît correctement sans toucher aux originaux.

## Formule

> Guardian propose. L’utilisateur décide. Guardian mémorise, verrouille et reconstruit.

## Principes

1. Source inviolable.
2. Autorité utilisateur.
3. Validation explicite.
4. Verrouillage protecteur.
5. Historique append-only.
6. Sorties jetables.
7. Dépendances minimales.
8. Échec sûr.

## Modules

- Scanner
- Parser
- Work Grouper
- Identity Assistant
- Decision Service
- History Service
- Library Builder
- Audit Service
- Jellyfin Reader

## États

```text
Découverte → À identifier → À vérifier → Validée → Verrouillée
```

États complémentaires : conflit, obsolète, erreur.

## Stockage

SQLite conserve les racines, œuvres, fichiers, identités minimales, décisions, verrous, événements, scans, constructions et paramètres.

## Projection

La sortie utilise des liens, des noms compatibles avec Jellyfin, des dossiers de saisons et des NFO minimaux.

## Technologie

C#/.NET, WPF, SQLite et architecture en couches.

## Évolution depuis cette version

La documentation active précise désormais que TMDb peut être utilisé comme fournisseur optionnel de candidats lorsque son accès API est configuré. La validation reste toujours explicite.
