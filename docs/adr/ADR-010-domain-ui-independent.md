# ADR-010 — Le cœur métier est indépendant de l’interface

**Statut : Accepted**

## Contexte

Les règles de sécurité et de décision doivent rester testables et stables, indépendamment de WPF.

## Décision

Le domaine et les services applicatifs ne dépendent pas de l’interface graphique.

## Conséquences

- tests unitaires sans UI ;
- possibilité d’une autre interface future ;
- code-behind limité à la présentation.
