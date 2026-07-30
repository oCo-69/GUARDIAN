# ADR-002 — Une série est identifiée au niveau de l’œuvre

**Statut : Accepted**

## Contexte

Identifier chaque épisode manuellement répète la même décision et augmente le risque d’incohérence.

## Décision

Une décision de série relie une œuvre source à une identité officielle. Les épisodes héritent de cette identité et utilisent leurs numéros de saison et d’épisode.

## Conséquences

- l’interface demande une seule validation par série ;
- le parser produit des descripteurs d’épisodes ;
- les anomalies restent traitées explicitement.
