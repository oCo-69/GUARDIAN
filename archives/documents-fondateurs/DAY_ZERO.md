# Day Zero — Archive

Guardian est né d’un problème concret : une importante bibliothèque Jellyfin fonctionnait selon une organisation locale cohérente, mais certaines conventions de noms et décisions de métadonnées étaient mal comprises ou fragilisées par les migrations.

Les premiers travaux portaient sur :

- l’audit de la base Jellyfin ;
- les champs verrouillés ;
- les migrations SQLite ;
- la restauration des plugins ;
- les rapports de santé ;
- des scripts PowerShell ;
- l’amélioration de la navigation web.

Puis une question plus profonde est apparue :

> Comment permettre à Jellyfin de comprendre la collection sans modifier cette collection ?

La réponse a d’abord pris la forme de NFO, puis de renommages virtuels, puis d’une bibliothèque dérivée par liens.

Guardian est alors devenu non plus un script d’entretien, mais une couche d’interprétation et finalement une mémoire d’identité média.
