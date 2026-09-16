# Traspaso y estado del proyecto — LÉEME PRIMERO

> **Propósito:** que cualquier persona o cuenta de IA (Claude u otra) retome el trabajo sin perder el hilo.
> Aquí está **dónde vamos, qué está hecho, qué queda pendiente y hacia dónde queremos llegar**.
> **Última actualización:** 2026-09-16.

---

## 0. Cómo retomar con una cuenta de Claude nueva (pasos)

1. Abrir esta carpeta del repo con Claude Code. Al iniciar, Claude lee automáticamente `CLAUDE.md` (y `AGENTS.md` / `GEMINI.md`, que son idénticos).
2. Leer **este archivo completo** (`TRASPASO-Y-ESTADO.md`) — da el panorama.
3. Según en qué se vaya a trabajar, leer la guía detallada correspondiente:
   - Plataforma .NET RenergeIA → `GUIA_DE_DESARROLLO.md` y `docs/`.
   - Plataforma de evaluaciones → `evaluacion-hse/README.md`.
4. Revisar la sección **“Registro de aprendizajes”** de `CLAUDE.md`: contiene los gotchas del entorno (deploys, Windows sin Python, etc.).
5. Antes de ejecutar cambios, respetar las **convenciones** (sección 6): confirmar antes de aplicar, trabajar contra producción, y mantener los tres `.md` sincronizados.

> ⚠️ La “memoria” automática de Claude **no se comparte entre cuentas**. Toda la trazabilidad viva debe quedar en **archivos del repo** (este documento + las guías + el registro de aprendizajes), no en la memoria de una cuenta.

---

## 1. Qué hay en este repositorio (son DOS proyectos)

El repo `github.com/luisabecerra22/RenergeIA` (rama `main`, **público**) contiene dos plataformas distintas:

| Proyecto | Carpeta | Stack | Proyecto GCP | Servicio Cloud Run | URL de producción |
|---|---|---|---|---|---|
| **RenergeIA** (gestión EPC) | raíz (`RenergeIA.Web/`, `.Core/`, `.Infrastructure/`) | .NET 10 + Blazor Server + PostgreSQL | `renergeia-app` (#577313322290) | `renergeia-web` | https://renergeia-web-577313322290.us-central1.run.app |
| **Evaluaciones HSE** | `evaluacion-hse/` | Next.js 15 + TypeScript + Firestore | `renergeia-evaluaciones` (#64204106653) | `evaluacion-hse` | https://evaluacion-hse-64204106653.us-central1.run.app |

> Son **proyectos GCP diferentes**. No mezclar credenciales ni comandos de deploy entre uno y otro (ver sección 4).

---

## 2. Estado actual (qué está hecho y funcionando)

### Evaluaciones HSE (lo más reciente — sept. 2026)
- Plataforma en producción con panel admin de 6 pestañas: **Resultados, Evaluaciones, Dashboard, Asistencia, Personal, Usuarios**.
- **Roles**: `admin` (ve todo) y `area` (ve solo su área: `hse`, `rrhh`).
- **Pestaña Personal** (creada en esta etapa): planta de personal (importar Excel, CRUD, buscar y filtrar por área/cargo/proyecto/trabajo), **matriz de asistencia ✓/✗** por evaluación con **% de asistencia**, toggles para mostrar/ocultar evaluaciones (recalculan el %), filtro ✓/✗ por evaluación, botón **Actualizar** y **Exportar a Excel**.
- La matriz marca ✓ si la persona tiene **asistencia registrada O presentó la evaluación** (cruce por cédula).
- Ajustes UI: tabla ordenada alfabéticamente, fuente Montserrat en todos los controles de formulario, se retiró el aviso de “certificado enviado por correo”.
- Último commit: `633d3ce` en `main` (ya en GitHub).

### RenergeIA (.NET) — resumen
- Plataforma EPC con módulos de Proyectos, WBS (con versiones de cronograma), Informe Diario, Documentos, Costos, Histogramas, Clima, HSEQ (Calidad/Ambiental/Social/Seguridad), Dashboard, Alertas.
- Detalle completo en `GUIA_DE_DESARROLLO.md` (32 secciones) y `docs/`.
- **Manual de usuario** (para quien usa la app): `docs/manual/README.md`, un capítulo por módulo con paso a paso y "Qué hacer en caso de…". Sus notas "⚠ Por confirmar" son decisiones o fallas pendientes de revisar.
- **Documentos (sept. 2026):** carga de la Planificación Documental FO-SI-GC-002-1 por proyecto sin duplicar, alertas de días sin atender (amarillo 4 / rojo 8), responsables internos por área, validación y Redline/As-Built.
- Versión en producción al momento de este traspaso: v38 (Cloud Run `renergeia-web`).

---

## 3. Cuentas y credenciales

### Evaluaciones HSE (panel admin)
| Usuario | Rol | Área |
|---|---|---|
| `renergeia-administrador` | admin | — |
| `hseq` | area | hse |
| `rrhh` | area | rrhh |

> **Las contraseñas NO se guardan en este repo** (es público). Están en el manual interno:
> `docs/Manual-Plataforma-Evaluaciones-HSE.pdf` (ese archivo está en `.gitignore`, solo existe local).
> Para cambiar una contraseña: pestaña **Usuarios** (admin) o **Mi cuenta** (cada quien).

### GCP
- Autenticación local con la cuenta Google dueña de los proyectos GCP.
- En Windows, `gcloud` requiere `CLOUDSDK_PYTHON` apuntando al Python embebido del SDK:
  `...\google-cloud-sdk\platform\bundledpython\python.exe`.

---

## 4. Cómo desplegar cada proyecto

### Evaluaciones HSE (Next.js) — ⚠️ NO usar `--source .`
`--source .` toma el `Dockerfile` .NET de la raíz y falla (empuja al registry equivocado). Flujo correcto:
```bash
# 1) build de la imagen en el registry del proyecto correcto
gcloud builds submit \
  --tag us-central1-docker.pkg.dev/renergeia-evaluaciones/cloud-run-source-deploy/evaluacion-hse \
  --project renergeia-evaluaciones --quiet
# 2) deploy de esa imagen
gcloud run deploy evaluacion-hse \
  --image us-central1-docker.pkg.dev/renergeia-evaluaciones/cloud-run-source-deploy/evaluacion-hse:latest \
  --region us-central1 --project renergeia-evaluaciones --allow-unauthenticated --port 8080 --quiet
```
(Se ejecuta desde `evaluacion-hse/`; hay un `.gcloudignore` que reduce el upload.)

### RenergeIA (.NET) — publicar SIEMPRE antes
```bash
dotnet publish RenergeIA.Web -c Release -o publish
gcloud run deploy renergeia-web --source . --project renergeia-app \
  --region us-central1 --allow-unauthenticated --port 8080 --quiet
```

> Tras cada deploy, **verificar en la URL de producción** (no asumir éxito).

---

## 5. PENDIENTES (lo que falta)

### Evaluaciones HSE
- [ ] **Envío del certificado por correo.** Hoy la persona solo lo **descarga** en pantalla; el aviso de “enviado por correo” se retiró de la UI. Falta configurar el envío real (Resend: `RESEND_API_KEY`, `MAIL_FROM`) y reactivar el aviso.

### RenergeIA (.NET) — CI/CD (para que el deploy no dependa del PC local)
Estado: la **service account `github-actions-deploy@renergeia-app.iam.gserviceaccount.com` YA está creada**. Falta (retomar exactamente desde aquí, no recrear la cuenta):
- [ ] Otorgar roles IAM a esa cuenta sobre `renergeia-app`: `roles/run.admin`, `roles/artifactregistry.writer`, `roles/iam.serviceAccountUser`, `roles/cloudbuild.builds.editor`.
- [ ] Generar la key JSON (`gcloud iam service-accounts keys create`).
- [ ] Guardarla como secreto de GitHub `GCP_SA_KEY` (`gh secret set` o web), y borrar el JSON local.
- [ ] Crear `.github/workflows/deploy.yml` que haga `dotnet publish` + deploy a Cloud Run en cada push a `main`.
- **Nota:** el paso de roles IAM fue **bloqueado por el clasificador de seguridad**; la usuaria debe autorizarlo/ejecutarlo (su terminal o la consola web de GCP → IAM & Admin).

### RenergeIA (.NET) — Documentos
- [ ] Lectura automática de la planificación documental desde **SharePoint** (requiere registrar una app en Microsoft 365 / Graph con permiso de solo lectura).
- [ ] Notificaciones por correo a los responsables de documentos en rojo.
- [ ] Confirmar con la usuaria la regla cuando app y Excel difieren (hoy gana lo editado en la app, con opción "Usar Excel").

### Documentación / repo
- [ ] Revisar con la usuaria los "⚠ Por confirmar" del manual de usuario (`docs/manual/`) y completar el manual a medida que cambien los módulos.
- [ ] El repo es **público** → el manual con contraseñas queda **fuera** (en `.gitignore`). Si se quiere versionar el manual, pasar a un repo **privado** o quitarle la tabla de contraseñas.

---

## 6. Convenciones de trabajo (importantes)

- **Confirmar antes de aplicar cambios.** La usuaria pidió expresamente: avisar qué se va a hacer y esperar su confirmación antes de ejecutar.
- **Trabajar contra producción**, no localhost, cuando así lo pida (es su flujo habitual en Evaluaciones HSE).
- **Los tres archivos `CLAUDE.md`, `AGENTS.md`, `GEMINI.md` deben ser idénticos.** Si se cambia uno, replicar en los otros dos.
- **Registrar aprendizajes** no triviales en la sección “Registro de aprendizajes” de esos tres archivos (formato con fecha y “Por qué importa”).
- Colores de marca: azul `#183963`, verde `#6ABF4B`, gris `#D9D9D6`, oscuro `#111921`.

---

## 7. Hacia dónde queremos llegar (rumbo)

Objetivo general: una plataforma corporativa de Renergeia que centralice la gestión EPC (RenergeIA .NET) y el cumplimiento HSE/SST con evaluaciones, asistencia y trazabilidad del personal (Evaluaciones HSE).

Rumbo cercano concreto (ya identificado):
1. Cerrar el **envío de certificados por correo** en Evaluaciones HSE.
2. Terminar el **CI/CD** de RenergeIA para desplegar desde GitHub Actions (sin depender del PC).

> 🔎 **Por confirmar con la usuaria** (dejar que ella defina/priorice el rumbo de mediano plazo): reportes/indicadores adicionales en Evaluaciones HSE, recordatorios automáticos a quienes no han presentado, integración entre ambas plataformas, y cualquier módulo nuevo. Antes de asumir metas nuevas, **preguntar**.
