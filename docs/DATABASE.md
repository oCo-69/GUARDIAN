# Modèle de données SQLite

## Rôle

SQLite est la mémoire persistante de Guardian.

La base conserve les observations locales, les identités minimales, les décisions, les verrous, l’historique et l’état des constructions. Elle ne contient pas une copie complète de TMDb ou d’un autre fournisseur.

## Tables principales

### `schema_migrations`

```text
version
name
applied_at
checksum
```

### `source_roots`

```text
id
path
is_read_only
created_at
updated_at
```

Contrainte : une racine active de la v1 est toujours déclarée en lecture seule.

### `source_works`

```text
id
root_id
kind
local_title
local_year
stable_key
status
created_at
updated_at
```

`kind` : `series`, `movie`, `special`, `ambiguous`.

### `source_files`

```text
id
work_id
path
size
modified_at
fingerprint
season
episode
local_title
media_extension
created_at
updated_at
```

Contraintes :

- `path` unique dans une racine ;
- saison et épisode facultatifs ;
- aucune colonne ne doit supposer qu’un épisode possède sa propre identité de série.

### `provider_identities`

```text
id
provider
media_type
provider_id
canonical_title
year
source_url
created_at
```

Unicité :

```text
(provider, media_type, provider_id)
```

### `candidates`

```text
id
work_id
identity_id
score
evidence_json
provider_payload_cache
created_at
expires_at
```

Les candidats sont temporaires et ne sont jamais utilisés directement par le constructeur.

### `decisions`

```text
id
work_id
identity_id
status
is_locked
created_at
created_by
supersedes_id
reason
```

Règles :

- une décision validée est immuable ;
- la réidentification crée une nouvelle ligne ;
- `supersedes_id` relie la nouvelle décision à la précédente ;
- une seule décision courante par œuvre ;
- le verrou est contrôlé par le domaine.

### `history_events`

```text
id
work_id
event_type
payload_json
actor
app_version
created_at_utc
created_at_local
correlation_id
```

Append-only.

### `scan_snapshots`

```text
id
started_at
completed_at
status
summary_json
app_version
```

### `build_runs`

```text
id
started_at
completed_at
status
destination_root
summary_json
app_version
```

### `generated_items`

```text
id
build_id
source_file_id
decision_id
destination_path
link_type
checksum
status
created_at
```

### `settings`

```text
key
value_json
updated_at
```

Les secrets ne doivent pas être stockés en clair dans cette table.

## Transactions

Doivent être transactionnels :

- application d’un snapshot de scan ;
- validation d’une décision et création de son événement ;
- verrouillage ou déverrouillage et événement associé ;
- réidentification ;
- restauration ;
- enregistrement final d’un build.

Le système de fichiers et SQLite ne partagent pas une transaction technique unique. Guardian utilise donc une transaction logique :

1. préparer ;
2. valider les préconditions ;
3. construire dans un espace temporaire ;
4. vérifier ;
5. basculer ;
6. enregistrer le succès ;
7. nettoyer.

## Migrations

Chaque migration doit être :

- versionnée ;
- testée ;
- transactionnelle lorsque SQLite le permet ;
- précédée d’une sauvegarde lors d’une mise à niveau utilisateur ;
- non destructive pour les décisions et l’historique.

Une migration ne supprime jamais silencieusement une décision.

## Sauvegarde

La sauvegarde minimale comprend :

- le fichier SQLite cohérent ;
- la version du schéma ;
- les paramètres non secrets ;
- un manifeste de sauvegarde.

La bibliothèque virtuelle ne nécessite pas de sauvegarde puisqu’elle est reconstructible.

## Index recommandés

- `source_files(work_id)`
- `source_files(fingerprint)`
- `source_works(stable_key)`
- `decisions(work_id, created_at)`
- `history_events(work_id, created_at_utc)`
- `generated_items(build_id)`
- `generated_items(source_file_id)`
- `provider_identities(provider, media_type, provider_id)`
