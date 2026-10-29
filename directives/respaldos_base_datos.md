# Respaldos de la base de datos (RenergeIA)

Activado el 2026-10-29. Todo corre solo en Google Cloud; no depende de la IA ni de ningún equipo encendido.

## Qué está configurado

| Capa | Qué hace | Cuándo | Cuánto se guarda | Dónde |
|------|----------|--------|------------------|-------|
| 1. Copias de Cloud SQL | Foto completa de la instancia `renergeia-db` | Todos los días a las **00:00 hora Colombia** (05:00 UTC) | Últimas **30** copias | Dentro de Cloud SQL |
| 1b. Recuperación a un momento exacto (PITR) | Permite volver a **cualquier minuto** | Continuo | Últimos **7 días** | Dentro de Cloud SQL |
| 2. Exportación aparte | Archivo `.sql.gz` de la base `RenergeIA` | Todos los días a la **01:00 hora Colombia** | **90 días** (nadie puede borrarlos antes; se eliminan solos el día 91) | `gs://renergeia-app-respaldos-db/AAAA/MM/renergeia_AAAA-MM-DD_HHMM.sql.gz` |

La capa 2 existe porque las copias de la capa 1 se pierden si alguien elimina la instancia completa.

Piezas de la capa 2 (proyecto `renergeia-app`, región `us-central1`):
- Bucket `renergeia-app-respaldos-db`: retención de 90 días (sin bloquear), regla de ciclo de vida que borra a los 91 días, acceso público bloqueado.
- Workflow `respaldo-db-diario`. Su código es `infra/respaldos/respaldo-db-diario.yaml`. Arma el nombre del archivo con la fecha de Colombia y reintenta si la instancia está ocupada (409).
- Cloud Scheduler `respaldo-db-diario` (`0 1 * * *`, America/Bogota). Llama al workflow.
- Cuenta de servicio `respaldo-db@renergeia-app.iam.gserviceaccount.com`. Solo tiene el rol propio `respaldoExportSQL` (export + get), `workflows.invoker` y `logging.logWriter`.
- La cuenta de servicio de la instancia Cloud SQL puede escribir (objectCreator) y leer (objectViewer) en el bucket.

Para redesplegar el workflow después de editarlo:
```bash
gcloud workflows deploy respaldo-db-diario --source=infra/respaldos/respaldo-db-diario.yaml --location=us-central1 --service-account=respaldo-db@renergeia-app.iam.gserviceaccount.com --project renergeia-app
```

## Revisión rápida (¿se están haciendo?)
```bash
gcloud sql backups list --instance renergeia-db --project renergeia-app --limit 5
gcloud storage ls -l -r gs://renergeia-app-respaldos-db/ | tail -5
gcloud workflows executions list respaldo-db-diario --location=us-central1 --project renergeia-app --limit 5
```
Para hacer un respaldo extra en este momento, por ejemplo antes de una carga masiva:
```bash
gcloud scheduler jobs run respaldo-db-diario --location=us-central1 --project renergeia-app
```

## Cómo recuperar información

**Regla de oro:** NUNCA restaurar encima de la base `RenergeIA` en uso. Si se hace, se pierde todo lo bueno que se registró después de la copia. Siempre se restaura en una copia aparte, se copian solo los datos perdidos y luego se borra la copia. Antes de cualquier recuperación se debe confirmar con la usuaria qué se perdió y desde cuándo.

### Opción A: volver a un minuto exacto (últimos 7 días)
Sirve cuando se sabe más o menos a qué hora ocurrió el borrado. Se clona la instancia al minuto anterior al error, en una instancia NUEVA. Esa instancia cuesta mientras exista. La hora va en UTC (Colombia + 5 h).
```bash
gcloud sql instances clone renergeia-db renergeia-db-recuperacion --point-in-time="2026-11-05T20:40:00Z" --project renergeia-app
```

### Opción B: copia de una noche (últimos 30 días)
```bash
gcloud sql backups list --instance renergeia-db --project renergeia-app
# Primero se crea una instancia vacía renergeia-db-recuperacion (mismo tier y versión, POSTGRES_15), luego:
gcloud sql backups restore <ID_COPIA> --restore-instance=renergeia-db-recuperacion --backup-instance=renergeia-db --project renergeia-app
```

### Opción C: archivo exportado (últimos 90 días; sirve aunque la instancia ya no exista)
Se importa a una base TEMPORAL dentro de la misma instancia. Esto no cuesta otra instancia.
```bash
gcloud sql databases create RenergeIA_recuperacion --instance renergeia-db --project renergeia-app
gcloud sql import sql renergeia-db gs://renergeia-app-respaldos-db/AAAA/MM/<archivo>.sql.gz --database=RenergeIA_recuperacion --project renergeia-app
```

### Después de A, B o C
1. Comparar la copia con la base actual y sacar SOLO las filas perdidas. Por ejemplo, con `INSERT ... SELECT` entre bases usando `dblink`, o exportando esas tablas a CSV.
2. Revisar con la usuaria lo recuperado en la app.
3. Eliminar la instancia o la base temporal (`gcloud sql instances delete renergeia-db-recuperacion` / `gcloud sql databases delete RenergeIA_recuperacion`). Así no sigue costando ni se confunde con la real.

## Notas
- La retención de 90 días del bucket NO está bloqueada. Un dueño del proyecto todavía podría quitarla. Bloquearla (`gcloud storage buckets update gs://renergeia-app-respaldos-db --lock-retention-period`) es IRREVERSIBLE y solo se hace si la usuaria lo decide.
- Las copias de Cloud SQL y el PITR protegen la base completa, incluidos los archivos guardados en ella (actas Word/PDF en bytea). Las evidencias que se guardan como archivos en el disco del contenedor NO quedan cubiertas.
- Costo estimado: menos de US$ 2–3 al mes con el tamaño actual (~1 MB exportado sin comprimir el 2026-10-29). Va a la misma facturación de `renergeia-app`.
- Pendiente (capa 3): bitácora en la app de quién cambió o borró qué (valor anterior y nuevo), para recuperar un registro puntual sin restaurar.
