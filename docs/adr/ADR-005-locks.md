# ADR-005 — Les décisions validées peuvent être verrouillées

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Statut : Accepted**

## Contexte

Une nouvelle analyse ou un meilleur candidat ne doit pas annuler un choix déjà confirmé.

## Décision

Le verrou est un invariant métier empêchant toute substitution automatique. Une réidentification exige un déverrouillage explicite.

## Conséquences

- contrôle dans le domaine ;
- événements de verrouillage et déverrouillage ;
- les conflits sont signalés mais non corrigés automatiquement.
