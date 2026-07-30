# ADR-001 — Source toujours en lecture seule

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Statut : Accepted**

## Contexte

La collection originale représente la source de vérité de l’utilisateur. Une erreur de renommage, déplacement ou suppression serait difficilement réversible.

## Décision

Guardian ne possède aucune opération d’écriture sur les racines sources. Les transformations sont réalisées dans une destination séparée.

## Conséquences

- les accès source utilisent des abstractions en lecture seule ;
- les tests vérifient l’intégrité avant et après les opérations ;
- toute fonctionnalité exigeant une écriture source est rejetée.
