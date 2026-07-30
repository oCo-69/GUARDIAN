# ADR-009 — TMDb est un fournisseur optionnel de candidats

> [Documentation](../README.md) · [Blueprint](../01_BLUEPRINT.md) · [ADR](README.md)

**Statut : Accepted**

## Contexte

Un accès TMDb est disponible et peut accélérer l’identification, mais Guardian doit rester utilisable sans lui.

## Décision

TMDb est implémenté comme adaptateur optionnel. Il fournit des candidats et des métadonnées minimales. La validation reste manuelle.

## Conséquences

- jeton stocké de façon sécurisée ;
- solution de repli par navigateur et saisie directe ;
- aucun couplage du domaine à TMDb ;
- ajout futur d’autres fournisseurs possible.
