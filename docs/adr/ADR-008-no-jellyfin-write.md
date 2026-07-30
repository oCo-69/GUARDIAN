# ADR-008 — Aucune écriture directe dans la base Jellyfin

**Statut : Accepted**

## Contexte

La base interne de Jellyfin évolue et une écriture externe pourrait la corrompre.

## Décision

Guardian agit par structure de fichiers, liens et NFO. Un lecteur Jellyfin peut auditer en lecture seule.

## Conséquences

- découplage des versions Jellyfin ;
- moindre risque de corruption ;
- résolution des conflits par reconstruction ou action utilisateur.
