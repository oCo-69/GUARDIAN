# ADR-003 — Aucune proposition n’est appliquée sans validation

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Statut : Accepted**

## Contexte

Une recherche automatique peut produire un résultat plausible mais incorrect.

## Décision

Tout résultat de fournisseur est un candidat. Seule une action explicite crée une décision validée.

## Conséquences

- le constructeur refuse les candidats ;
- le score de confiance aide à trier, jamais à décider ;
- les imports automatiques restent des propositions à vérifier explicitement.
