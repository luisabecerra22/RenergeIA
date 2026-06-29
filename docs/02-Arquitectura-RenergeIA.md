# Arquitectura General — RenergeIA
**Versión:** 1.0 | **Fecha:** 2026-06-28 | **Estado:** Vigente

> Este documento define cómo está organizada la plataforma RenergeIA, cómo se conectan los módulos entre sí, y cuáles son las reglas de arquitectura que todo desarrollo nuevo debe respetar.

---

## 1. Visión general

RenergeIA no es una suma de pantallas independientes. Es un sistema integrado donde cada módulo aporta datos al siguiente nivel de análisis.

```
┌─────────────────────────────────────────────────────────────┐
│                    DASHBOARD GENERAL                        │
│           (vista ejecutiva de todos los proyectos)          │
└───────────────────────┬─────────────────────────────────────┘
                        │ alimentado por
          ┌─────────────┼──────────────────┐
          │             │                  │
   ┌──────▼──────┐ ┌────▼─────┐ ┌─────────▼───────┐
   │  Dashboard  │ │Dashboard │ │   Dashboard     │
   │  Proyecto A │ │Proyecto B│ │   Proyecto N    │
   └──────┬──────┘ └──────────┘ └─────────────────┘
          │ alimentado por
   ┌──────┴────────────────────────────────────┐
   │           MÓDULOS OPERATIVOS              │
   │  Informe Diario · WBS · Costos · HSEQ    │
   │  Documentos · Personal · Equipos · Clima  │
   └───────────────────────────────────────────┘
```

**Regla fundamental:** Ningún módulo opera como isla. Todo dato ingresado en un módulo debe poder aparecer en el dashboard del módulo, del área o del proyecto.

---

## 2. Stack tecnológico

| Capa | Tecnología | Versión |
|---|---|---|
| Frontend / UI | Blazor Server (ASP.NET Core) | .NET 10 |
| Estilos | Bootstrap 5 + CSS custom (`app.css`) | 5.3 |
| Gráficos | Chart.js (CDN) | 4.4.0 |
| Mapas | Leaflet.js (CDN) | 1.9.4 |
| Backend | C# — Servicios de aplicación en `RenergeIA.Web/Services/` | .NET 10 |
| ORM | Entity Framework Core + migraciones | 9.x |
| Base de datos | SQL Server (local desarrollo → Azure SQL en Fase 2) | — |
| Autenticación | ASP.NET Core Identity (PBKDF2) | — |
| Autorización | Policy-based RBAC (13 roles) | — |
| Exportación Excel | ClosedXML | 0.104.2 |
| PDF (pendiente) | QuestPDF | Fase 2 |
| Tiempo real | SignalR (alertas, pendiente) | Fase 2 |
| IA | Azure OpenAI / OpenAI API | Fase 3 |

---

## 3. Estructura de carpetas

```
Proyecto Agente/
├── docs/                          ← Arquitectura y guías (este documento)
├── RenergeIA.Core/                ← Entidades y contratos (Clean Architecture)
│   ├── Entities/                  ← Modelos de dominio (Proyecto, WBS, etc.)
│   └── Interfaces/                ← Contratos de repositorios
├── RenergeIA.Infrastructure/      ← Acceso a datos
│   ├── Data/
│   │   └── RenergeIADbContext.cs  ← DbContext principal
│   └── Migrations/                ← Migraciones EF Core
└── RenergeIA.Web/                 ← Aplicación Blazor Server
    ├── Components/
    │   ├── Layout/                ← MainLayout, NavMenu, LoginLayout
    │   ├── Pages/                 ← Páginas organizadas por módulo
    │   │   ├── Auth/
    │   │   ├── Clima/
    │   │   ├── Costos/
    │   │   ├── Dashboard/
    │   │   ├── Documentos/
    │   │   ├── Equipos/
    │   │   ├── HSEQ/
    │   │   │   ├── Ambiental/
    │   │   │   ├── Calidad/
    │   │   │   ├── Seguridad/
    │   │   │   └── Social/
    │   │   ├── Histogramas/
    │   │   ├── InformeDiario/
    │   │   ├── NoConformidades/
    │   │   ├── Personal/
    │   │   ├── Proyectos/
    │   │   ├── Restricciones/
    │   │   └── WBS/
    │   └── Shared/
    │       └── Dashboard/         ← Componentes reutilizables (Design System)
    ├── Services/                  ← Servicios de aplicación
    └── wwwroot/
        ├── app.css                ← Estilos globales + Design System
        └── app.js                 ← Funciones JS para Chart.js
```

---

## 4. Módulos actuales y estado

### Módulos Operativos (por proyecto)

| Módulo | Ruta | Estado | Alimenta |
|---|---|---|---|
| Dashboard | `/proyectos/{id}/dashboard` | ✅ Activo | Dashboard General |
| Informe Diario | `/proyectos/{id}/informes` | ✅ Activo | Dashboard, Reportes |
| WBS / Cronograma | `/proyectos/{id}/wbs` | ✅ Activo | Dashboard, IA |
| Personal | `/proyectos/{id}/personal` | ✅ Activo | Histogramas, Costos |
| Equipos | `/proyectos/{id}/equipos` | ✅ Activo | Histogramas |
| Documentos | `/proyectos/{id}/documentos` | ✅ Activo | Reportes |
| Costos | `/proyectos/{id}/costos` | ✅ Activo | Dashboard, Reportes |
| No Conformidades | `/proyectos/{id}/no-conformidades` | ✅ Activo | HSEQ, Reportes |
| Restricciones | `/proyectos/{id}/restricciones` | ✅ Activo | Dashboard |
| Histogramas | `/proyectos/{id}/histogramas` | ✅ Activo | Dashboard |
| Clima | `/proyectos/{id}/clima` | ✅ Activo | HSEQ, Informes |
| Alertas | `/proyectos/{id}/alertas` | 🔜 Próximamente | Todos |

### Módulo HSEQ (4 pilares)

| Sub-módulo | Ruta | Estado |
|---|---|---|
| HSEQ Dashboard general | `/hseq` | ✅ Activo |
| Seguridad (14 pantallas) | `/hseq/seguridad/*` | ✅ Activo |
| Calidad (7 pantallas) | `/hseq/calidad/*` | ✅ Activo |
| Ambiental (8 pantallas) | `/hseq/ambiental/*` | ✅ Activo |
| Social (8 pantallas) | `/hseq/social/*` | ✅ Activo |

### Módulos Futuros (Fase 3+)

| Módulo | Descripción |
|---|---|
| Maquinaria | Control de maquinaria y herramientas |
| Compras | Gestión de órdenes de compra |
| Bodega | Inventario y despacho de materiales |
| Gestión Contractual | Contratos, actas, garantías |
| Reportes | Centro de generación de informes PDF/Excel |
| IA | Panel de análisis inteligente global |

---

## 5. Flujo de datos entre módulos

Todo módulo operativo sigue este flujo de datos:

```
INGRESO DE DATOS
(operador en campo)
        ↓
MÓDULO OPERATIVO
(Informe, Inspección, Costo, etc.)
        ↓
BASE DE DATOS (SQL Server / EF Core)
        ↓
SERVICIO DE APLICACIÓN
(RenergeIA.Web/Services/)
        ↓
┌──────────────────────────────────┐
│  Dashboard del módulo            │  ← Vista detallada del módulo
│  Dashboard del área (HSEQ, etc.) │  ← Vista por área funcional
│  Dashboard del proyecto          │  ← Vista gerencial del proyecto
│  Dashboard General               │  ← Vista ejecutiva multi-proyecto
│  Reportes (PDF / Excel)          │  ← Exportación formal
│  Panel de IA                     │  ← Análisis inteligente (Fase 3)
└──────────────────────────────────┘
```

---

## 6. Sistema de roles (RBAC)

ASP.NET Core Identity con autorización basada en políticas. 13 roles definidos:

| Categoría | Roles |
|---|---|
| Gestión | Administrador, Gerente de Proyecto, Director |
| Técnica | Ingeniero Residente, Coordinador Técnico |
| HSEQ | Coordinador HSEQ, Inspector SST, Inspector Ambiental |
| Operativa | Supervisor de Obra, Operador de Campo |
| Soporte | Administrador de Documentos, Almacenista |
| Financiera | Analista de Costos |

Cada pantalla debe declarar `[Authorize(Policy = "...")]` o `[Authorize(Roles = "...")]` según corresponda.

---

## 7. Rutas principales

### Convención de rutas

```
/                          → Home (Dashboard General)
/proyectos                 → Lista de proyectos
/proyectos/{id}            → Detalle del proyecto
/proyectos/{id}/{modulo}   → Módulo del proyecto
/hseq                      → Dashboard HSEQ
/hseq/seguridad/{pantalla} → Sub-módulo Seguridad
/hseq/calidad/{pantalla}   → Sub-módulo Calidad
/hseq/ambiental/{pantalla} → Sub-módulo Ambiental
/hseq/social/{pantalla}    → Sub-módulo Social
/auth/login                → Inicio de sesión
/auth/logout               → Cierre de sesión
```

### Regla de rutas

> **Nunca cambiar rutas existentes sin validar** que no existan enlaces en NavMenu, otros componentes, o correos/documentos compartidos con el equipo. Un cambio de ruta rompe marcadores y accesos directos.

---

## 8. Convenciones de código

### Archivos Razor (`.razor`)

- **Nombre:** PascalCase, en español, descriptivo. Ej: `ListaProyectos.razor`, `SeguridadDashboard.razor`
- **Directiva de ruta:** siempre al inicio con `@page`
- **RenderMode:** `@rendermode InteractiveServer` para páginas con interactividad
- **Inyección:** `@inject` después de `@page` y `@rendermode`
- **Sección `@code`:** al final del archivo, separada del HTML

### Servicios (`.cs`)

- **Nombre:** `{Entidad}Service.cs`. Ej: `CostoService.cs`, `ProyectoService.cs`
- **Ubicación:** `RenergeIA.Web/Services/`
- **Patrón:** servicio inyectado en el componente vía DI, nunca acceso directo al DbContext desde el razor

### Componentes compartidos

- **Ubicación:** `RenergeIA.Web/Components/Shared/Dashboard/`
- **Nombre:** PascalCase descriptivo. Ej: `GaugeCircular.razor`, `PageHeader.razor`
- **Parámetros:** `[Parameter]` públicos con valores default
- **Namespace:** declarado en `_Imports.razor` para no repetir `@using` en cada página

---

## 9. Reglas de arquitectura — obligatorias

1. **Ningún módulo nuevo puede existir sin su dashboard propio.**
2. **El dashboard del módulo debe alimentar el dashboard del área.**
3. **El dashboard del área debe alimentar el Dashboard General.**
4. **Los datos no se duplican en la UI.** Si un dato ya existe en otro módulo, se lee del mismo servicio.
5. **Los componentes de UI son reutilizables.** Si creás el mismo HTML dos veces, convertilo en componente.
6. **Las rutas son estables.** No cambiar rutas de módulos activos sin coordinación.
7. **Todo módulo nuevo usa los componentes del Design System**, no crea su propio CSS.
8. **Los servicios encapsulan la lógica.** El código C# de negocio va en los servicios, no en el `@code` del razor.

---

*Documento generado: 2026-06-28. Actualizar cuando se agreguen módulos o cambie el stack.*
