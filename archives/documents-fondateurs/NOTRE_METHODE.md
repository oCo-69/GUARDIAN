# Notre Méthode — Archive

La méthode initiale consistait à :

1. partir de problèmes réels observés dans la bibliothèque ;
2. éviter les transformations irréversibles ;
3. produire des prototypes courts ;
4. vérifier les résultats sur une collection représentative ;
5. documenter les décisions ;
6. faire évoluer l’architecture lorsque les prototypes révélaient une limite.

Cette approche a conduit du script PowerShell à la conception d’une application Windows structurée.

La méthode actuelle conserve cet esprit, avec une discipline supplémentaire :

```text
Problème → invariant → ADR éventuel → test → implémentation → validation
```
