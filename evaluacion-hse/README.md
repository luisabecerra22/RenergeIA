# Sistema de Evaluaciones HSE — Renergeia

Plataforma web para digitalizar las evaluaciones de capacitación en salud, seguridad
y ambiente (SG-SST) de Renergeia. Basada en el formato **FO-SG-SS-100-1**.

## Funcionalidades

- **Participante:** se registra (correo, nombre, apellido, cédula, cargo), presenta la
  evaluación de conocimiento y la retroalimentación del capacitador. Recibe la
  calificación al instante (escala 0–5, aprueba con 3,0). También puede **registrar asistencia**
  a una capacitación desde la página principal.
- **Aprobado:** descarga un **certificado PDF** desde la pantalla de resultado.
  > ⚠️ El envío del certificado por correo está **pendiente** (el aviso se retiró de la UI).
- **Reprobado:** debe repetir la capacitación; puede reintentar tras **24 horas**.
- **Panel de administración** con pestañas: **Resultados**, **Evaluaciones**, **Dashboard**,
  **Asistencia**, **Personal** y **Usuarios**.
- **Roles:** `admin` (ve y gestiona todo, incluida la pestaña Usuarios) y `area` (ve solo su
  propia área; áreas actuales: `hse`, `rrhh`). El aislamiento por área se aplica en cada pestaña.
- **Pestaña Personal:** planta de personal (importar desde Excel, agregar/editar/eliminar,
  buscar y filtrar por área/cargo/proyecto/trabajo) y una **matriz de asistencia** que cruza
  cada persona con cada evaluación (✓/✗) y su **% de asistencia**. La matriz marca ✓ si la
  persona tiene asistencia registrada **o** presentó la evaluación (cruce por cédula). Botón
  **Actualizar** (recarga datos) y **Exportar a Excel** (matriz + hojas de ausentes).

## Cuentas (producción)

| Usuario | Rol | Área |
|---|---|---|
| `renergeia-administrador` | admin | — |
| `hseq` | area | hse |
| `rrhh` | area | rrhh |

Las contraseñas se documentan en el manual interno (`docs/Manual-Plataforma-Evaluaciones-HSE.pdf`).

## Stack

- **Next.js 15** (App Router, TypeScript) — un solo contenedor sirve el formulario y el panel.
- **Firestore** en producción / **JSON local** en desarrollo (seleccionable con `DB_BACKEND`).
- **pdf-lib** para el certificado, **Resend** para el correo, **jose + bcrypt** para el acceso admin.
- Despliegue en **Google Cloud Run** (escala a cero).

## Desarrollo local

```bash
npm install
cp .env.example .env.local     # ya viene un .env.local listo para pruebas
npm run seed                   # crea la evaluación inicial y el admin
npm run dev                    # http://localhost:3000
```

Admin de prueba: usuario `admin`, contraseña `Renergeia2026*` (cámbiala en producción).

## Despliegue en Google Cloud (Cloud Run + Firestore)

Proyecto GCP: **`renergeia-evaluaciones`** (#64204106653) · servicio Cloud Run: **`evaluacion-hse`**
· URL: **https://evaluacion-hse-64204106653.us-central1.run.app**

> ⚠️ **No usar `gcloud run deploy --source .`**: desde este subdirectorio toma el `Dockerfile`
> .NET de la raíz del repo y falla al empujar la imagen a `renergeia-app` (artifact registry
> denied). Un `.gcloudignore` en esta carpeta reduce el tamaño del upload.

Flujo correcto (dos pasos: build de la imagen y luego deploy con `--image`):

```bash
# 1) Construir y publicar la imagen en el Artifact Registry del proyecto correcto
gcloud builds submit \
  --tag us-central1-docker.pkg.dev/renergeia-evaluaciones/cloud-run-source-deploy/evaluacion-hse \
  --project renergeia-evaluaciones --quiet

# 2) Desplegar esa imagen en Cloud Run
gcloud run deploy evaluacion-hse \
  --image us-central1-docker.pkg.dev/renergeia-evaluaciones/cloud-run-source-deploy/evaluacion-hse:latest \
  --region us-central1 --project renergeia-evaluaciones \
  --allow-unauthenticated --port 8080 --quiet
```

En Windows, exportar antes `CLOUDSDK_PYTHON` al Python empaquetado del SDK.

## Variables de entorno

| Variable | Descripción |
|---|---|
| `DB_BACKEND` | `json` (local) o `firestore` (producción) |
| `GOOGLE_CLOUD_PROJECT` | ID del proyecto GCP (producción) |
| `SESSION_SECRET` | Secreto para firmar la sesión del admin |
| `ADMIN_USERNAME` / `ADMIN_PASSWORD` | Credenciales del primer admin (para el seed) |
| `RESEND_API_KEY` | API key de Resend (envío de certificados) |
| `MAIL_FROM` | Remitente verificado, ej. `no-reply@btodigital.com` |
| `APP_BASE_URL` | URL pública de la app |
