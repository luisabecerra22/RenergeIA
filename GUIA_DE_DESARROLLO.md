# Guía de Desarrollo — RenergeIA

> Documento de referencia completo del proceso de construcción de la plataforma RenergeIA.
> Audiencia: equipo interno de Renergeia S.A.S. / desarrolladores que incorporen el proyecto.
> Última actualización: **24 de junio de 2026**

---

## Tabla de contenido

1. [Contexto del proyecto](#1-contexto-del-proyecto)
2. [Visión general de la arquitectura](#2-visión-general-de-la-arquitectura)
3. [Stack tecnológico](#3-stack-tecnológico)
4. [Estructura de la solución](#4-estructura-de-la-solución)
5. [Preparación del entorno de desarrollo](#5-preparación-del-entorno-de-desarrollo)
6. [Etapa 1 — Creación de la solución y proyectos](#etapa-1--creación-de-la-solución-y-proyectos)
7. [Etapa 2 — Modelo de datos (Core)](#etapa-2--modelo-de-datos-core)
8. [Etapa 3 — Infraestructura, EF Core e Identity](#etapa-3--infraestructura-ef-core-e-identity)
9. [Etapa 4 — Autenticación y roles RBAC](#etapa-4--autenticación-y-roles-rbac)
10. [Etapa 5 — Módulo de Proyectos](#etapa-5--módulo-de-proyectos)
11. [Etapa 6 — Módulo WBS (Work Breakdown Structure)](#etapa-6--módulo-wbs-work-breakdown-structure)
12. [Etapa 7 — Módulo de Informe Diario](#etapa-7--módulo-de-informe-diario)
13. [Etapa 8 — Módulo de Documentos](#etapa-8--módulo-de-documentos)
14. [Etapa 9 — Dashboard analítico](#etapa-9--dashboard-analítico)
15. [Etapa 10 — Módulo de Costos y Partidas](#etapa-10--módulo-de-costos-y-partidas)
16. [Etapa 11 — Módulo de Histogramas](#etapa-11--módulo-de-histogramas)
17. [Etapa 12 — Control de versiones del cronograma](#etapa-12--control-de-versiones-del-cronograma)
18. [Etapa 13 — Módulo de Clima](#etapa-13--módulo-de-clima)
19. [Mejoras UI en el módulo WBS](#mejoras-ui-en-el-módulo-wbs)
20. [Módulos adicionales del modelo](#módulos-adicionales-del-modelo)
21. [Migraciones de base de datos](#migraciones-de-base-de-datos)
22. [Cómo correr el proyecto](#cómo-correr-el-proyecto)
23. [Fases de desarrollo](#fases-de-desarrollo)
24. [Glosario](#glosario)

---

## 1. Contexto del proyecto

### ¿Qué es RenergeIA?

RenergeIA es una **plataforma web interna** diseñada para gestionar de extremo a extremo los proyectos EPC (Engineering, Procurement & Construction) fotovoltaicos de **Renergeia S.A.S.**, empresa con operaciones en Colombia, Panamá, Ecuador e Italia.

### ¿Por qué existe?

Antes de RenergeIA, la gestión del ciclo de vida de un proyecto se distribuía entre múltiples herramientas desconectadas:

| Necesidad | Herramienta anterior |
|-----------|---------------------|
| Cronograma | Microsoft Project |
| Presupuesto / costos | Excel |
| Documentos | SharePoint |
| Comunicación interna | WhatsApp / correo |
| Informes diarios | Formatos Word/PDF manuales |

Esto provocaba pérdida de información, reprocesos, falta de trazabilidad y demoras en la toma de decisiones. RenergeIA centraliza todo en un único sistema con roles, alertas automáticas e inteligencia artificial (en fases futuras).

### Alcance final planeado

- **18 módulos** funcionales
- **13 roles RBAC** (control de acceso basado en roles)
- **20 alertas automáticas**
- **17 tipos de reportes**
- Despliegue en Azure (Fase 2)
- IA predictiva con Azure OpenAI (Fase 2)

---

## 2. Visión general de la arquitectura

La aplicación sigue el patrón **Clean Architecture** dividido en 3 capas principales:

```
┌─────────────────────────────────────────┐
│          RenergeIA.Web                  │
│   (Blazor Server — UI + servicios)      │
│   Páginas .razor, Layouts, Auth UI      │
└────────────────┬────────────────────────┘
                 │ depende de
┌────────────────▼────────────────────────┐
│        RenergeIA.Infrastructure         │
│   EF Core DbContext, Identity,          │
│   Migraciones, Seeder                   │
└────────────────┬────────────────────────┘
                 │ depende de
┌────────────────▼────────────────────────┐
│           RenergeIA.Core                │
│   Entidades, Enums, Helpers             │
│   (sin dependencias externas)           │
└─────────────────────────────────────────┘
```

**Regla fundamental:** las capas superiores pueden depender de las inferiores, pero nunca al revés. `Core` no conoce ni a `Infrastructure` ni a `Web`.

### Flujo de una petición en Blazor Server

```
Usuario (navegador)
      │  WebSocket (SignalR)
      ▼
Blazor Server (proceso en servidor)
      │
      ▼
Servicio de aplicación (.cs) en RenergeIA.Web/Services/
      │
      ▼
DbContext (EF Core)
      │
      ▼
SQL Server (base de datos)
```

A diferencia de una SPA, en Blazor Server **todo el código C# corre en el servidor**. El navegador solo recibe diferencias del DOM a través de la conexión WebSocket. Esto simplifica la autenticación y el acceso a datos.

---

## 3. Stack tecnológico

| Capa | Tecnología | Versión |
|------|-----------|---------|
| Framework | .NET | 10.0 |
| Frontend | Blazor Server (ASP.NET Core) | 10.0 |
| UI | Bootstrap | 5 (CDN) |
| Iconos | Bootstrap Icons | 1.11.3 (CDN) |
| Gráficos | Chart.js | 4.4.0 (CDN) |
| Mapas | Leaflet.js | 1.9.4 (CDN) |
| ORM | Entity Framework Core | 10.0 |
| Base de datos (local) | SQL Server (Express o Developer) | 2019+ |
| Base de datos (nube, Fase 2) | Azure SQL | — |
| Autenticación | ASP.NET Core Identity | 10.0 |
| Interoperabilidad JS | IJSRuntime (Blazor built-in) | — |
| Reportes PDF | QuestPDF | Fase 2 |
| Almacenamiento archivos | Sistema local → Azure Blob (Fase 2) | — |
| Clima | Open-Meteo API | Gratis, sin clave |
| IA | Azure OpenAI / OpenAI API | Fase 2 |
| Control de versiones | Git | — |

### Paquetes NuGet instalados

**RenergeIA.Infrastructure**
```
Microsoft.AspNetCore.Identity.EntityFrameworkCore  10.0.x
Microsoft.EntityFrameworkCore.SqlServer            10.0.x
Microsoft.EntityFrameworkCore.Tools                10.0.x
```

**RenergeIA.Web**
```
Microsoft.AspNetCore.Identity.UI    10.0.x
Microsoft.EntityFrameworkCore.Design  10.0.x
```

---

## 4. Estructura de la solución

```
Proyecto Agente/
│
├── RenergeIA.slnx                        ← Archivo de solución
│
├── RenergeIA.Core/                       ← Capa de dominio
│   ├── Entities/
│   │   ├── EntidadBase.cs
│   │   ├── Proyecto.cs
│   │   ├── CronogramaVersion.cs          ← NUEVO: versiones del cronograma
│   │   ├── ActividadWBS.cs
│   │   ├── InformeDiario.cs
│   │   ├── RegistroAvanceDiario.cs
│   │   ├── RegistroClima.cs
│   │   ├── PersonalProyecto.cs
│   │   ├── DocumentoPersona.cs
│   │   ├── Equipo.cs
│   │   ├── RegistroAvanceEquipo.cs
│   │   ├── RegistroAvancePersonal.cs
│   │   ├── RegistroAvanceRestriccion.cs
│   │   ├── RegistroHorometro.cs
│   │   ├── Fotografia.cs
│   │   ├── Partida.cs
│   │   ├── CostoReal.cs
│   │   ├── Restriccion.cs
│   │   ├── NoConformidad.cs
│   │   ├── AccionCorrectiva.cs
│   │   ├── Mantenimiento.cs
│   │   ├── Alerta.cs
│   │   ├── Documento.cs
│   │   ├── VersionDocumento.cs
│   │   ├── PlantillaHistograma.cs        ← NUEVO: histograma planificado
│   │   ├── ItemHistograma.cs             ← NUEVO: ítem mensual histograma
│   │   ├── HistogramaReal.cs             ← NUEVO: histograma de ejecución real
│   │   └── ItemHistogramaReal.cs         ← NUEVO: ítem mensual histograma real
│   ├── Enums/
│   │   ├── EstadoProyecto.cs
│   │   ├── EstadoActividad.cs
│   │   ├── EstadoAvance.cs
│   │   ├── EstadoInforme.cs
│   │   ├── EstadoDocumento.cs
│   │   ├── EstadoNoConformidad.cs
│   │   ├── EstadoRestriccion.cs
│   │   ├── CategoriaAlerta.cs
│   │   ├── CondicionClimatica.cs
│   │   ├── Disciplina.cs
│   │   ├── DisciplinaDocumento.cs
│   │   ├── SeveridadNoConformidad.cs     ← NUEVO
│   │   ├── TipoDocumento.cs
│   │   ├── TipoEquipo.cs
│   │   ├── TipoMantenimiento.cs
│   │   ├── TipoPersonal.cs
│   │   └── TipoHistograma.cs
│   └── Helpers/
│       └── EnumDisplay.cs
│
├── RenergeIA.Infrastructure/
│   ├── Data/
│   │   └── RenergeIADbContext.cs
│   ├── Identity/
│   │   ├── ApplicationUser.cs
│   │   ├── Roles.cs
│   │   └── DatabaseSeeder.cs
│   └── Migrations/                       ← Historial completo de cambios BD
│       ├── 20260607211442_InitialCreate
│       ├── 20260608001941_AddActivoWBS
│       ├── 20260609123730_AgregarDisciplinaYRelacionClima
│       ├── 20260609125338_AgregarCamposActividadWBS
│       ├── 20260611160217_AgregarModuloDocumentos
│       ├── 20260621203109_AgregarJerarquiaPartidas
│       ├── 20260621203614_AgregarHistogramas
│       ├── 20260621214119_AgregarPorcentajeEjecutadoPartida
│       ├── 20260621221117_ReemplazarPorcentajePorValorEjecutado
│       ├── 20260622000523_AgregarMesInicialHistograma
│       ├── 20260622223258_AgregarHistogramaReal
│       ├── 20260623133018_AgregarAnioInicialHistograma
│       └── AgregarCronogramaVersion      ← NUEVO
│
└── RenergeIA.Web/
    ├── Components/
    │   ├── App.razor                     ← Carga Chart.js + js/app.js
    │   ├── Routes.razor
    │   ├── _Imports.razor
    │   ├── RedirectToLogin.razor
    │   ├── Layout/
    │   │   ├── MainLayout.razor
    │   │   ├── LoginLayout.razor
    │   │   ├── NavMenu.razor
    │   │   └── ReconnectModal.razor
    │   └── Pages/
    │       ├── Auth/Login.razor, Logout.razor
    │       ├── Home.razor
    │       ├── Proyectos/
    │       │   ├── ListaProyectos.razor
    │       │   ├── NuevoProyecto.razor
    │       │   ├── EditarProyecto.razor
    │       │   └── DetalleProyecto.razor
    │       ├── WBS/
    │       │   ├── ListaWBS.razor        ← Rediseñado con versiones, resize, etc.
    │       │   └── FormWBS.razor
    │       ├── InformeDiario/
    │       │   ├── ListaInformesDiarios.razor
    │       │   ├── CrearInformeDiario.razor
    │       │   └── DetalleInformeDiario.razor
    │       ├── Documentos/
    │       │   ├── ListaDocumentos.razor
    │       │   ├── CrearDocumento.razor
    │       │   └── DetalleDocumento.razor
    │       ├── Personal/
    │       │   ├── ListaPersonal.razor
    │       │   └── FormPersonal.razor
    │       ├── Equipos/
    │       │   ├── ListaEquipos.razor
    │       │   └── FormEquipo.razor
    │       ├── Costos/
    │       │   └── Costos.razor
    │       ├── Histogramas/
    │       │   └── Histogramas.razor
    │       ├── Clima/
    │       │   └── Clima.razor               ← NUEVO: mapa Leaflet + AccuWeather API
    │       ├── NoConformidades/
    │       │   └── NoConformidades.razor
    │       ├── Restricciones/
    │       │   └── Restricciones.razor
    │       └── Dashboard/
    │           └── DashboardProyecto.razor
    ├── Services/
    │   ├── InformeDiarioService.cs       ← KPIs, Curva S, Dashboard, filtrado por versión
    │   ├── DocumentoService.cs
    │   ├── CostoService.cs               ← cálculos de partidas y costos reales
    │   ├── HistogramaService.cs          ← lógica de histogramas planificado/real
    │   └── HomeDashboardService.cs       ← Dashboard del inicio (portafolio)
    ├── Program.cs
    ├── appsettings.json                  ← incluye AccuWeather:ApiKey
    └── wwwroot/
        ├── app.css
        └── js/
            └── app.js                    ← Chart.js helpers + wbsResize + leafletMap
```

---

## 5. Preparación del entorno de desarrollo

### Herramientas necesarias

| Herramienta | Propósito |
|------------|-----------|
| .NET SDK 10 | Compilar y ejecutar la aplicación |
| SQL Server Express/Developer | Base de datos local |
| SQL Server Management Studio (SSMS) | Visualizar la base de datos |
| Visual Studio Code o Visual Studio 2022+ | Editor de código |
| Git | Control de versiones |

### Verificar instalación

```powershell
dotnet --version   # Debe mostrar 10.x.x
git --version      # Debe mostrar 2.x.x
```

---

## Etapa 1 — Creación de la solución y proyectos

### Comandos ejecutados

```powershell
mkdir "Proyecto Agente"
cd "Proyecto Agente"

dotnet new sln -n RenergeIA
dotnet new classlib -n RenergeIA.Core -f net10.0
dotnet new classlib -n RenergeIA.Infrastructure -f net10.0
dotnet new blazorserver -n RenergeIA.Web -f net10.0

dotnet sln add RenergeIA.Core
dotnet sln add RenergeIA.Infrastructure
dotnet sln add RenergeIA.Web

dotnet add RenergeIA.Infrastructure reference RenergeIA.Core
dotnet add RenergeIA.Web reference RenergeIA.Core
dotnet add RenergeIA.Web reference RenergeIA.Infrastructure
```

---

## Etapa 2 — Modelo de datos (Core)

### Clase base

```csharp
public abstract class EntidadBase
{
    public int Id { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaModificacion { get; set; }
}
```

### Entidades principales

#### Proyecto

Representa un proyecto EPC fotovoltaico con su ciclo de vida completo. Tiene colecciones hacia todas las entidades hijas: `ActividadesWBS`, `InformesDiarios`, `Documentos`, `Partidas`, `NoConformidades`, `Restricciones`, `PersonalProyecto`, `Equipos`, `Alertas`.

Campos clave: `Codigo`, `Nombre`, `Cliente`, `Pais`, `CapacidadKWp`, `PresupuestoContractual`, `FechaInicioPlaneada`, `FechaFinPlaneada`, `Estado (EstadoProyecto)`.

Campos de ubicación climática (agregados en migración `AgregarUbicacionClimatica`): `Departamento?`, `Municipio?`, `Latitud?`, `Longitud?`, `AccuWeatherLocationKey?`. Estos campos permiten al módulo de Clima localizar el proyecto en el mapa y consultar la API de AccuWeather.

#### CronogramaVersion ← NUEVO

Gestiona las versiones del cronograma de un proyecto (línea base + reprogramaciones):

```csharp
public class CronogramaVersion : EntidadBase
{
    public int ProyectoId { get; set; }
    public string Nombre { get; set; }            // "Actividades Inicial", "Actividades Reprogramación 1"...
    public int NumeroVersion { get; set; }         // 0 = inicial, 1 = primera reprog., etc.
    public bool EsVigente { get; set; }            // Solo una puede ser true por proyecto
    public string? MotivoReprogramacion { get; set; }
    public string? Observaciones { get; set; }
    public string CreadoPor { get; set; }
    public ICollection<ActividadWBS> Actividades { get; set; }
}
```

#### ActividadWBS

Actividad dentro de la Estructura de Desglose de Trabajo. Soporta jerarquía padre-hijo y ahora pertenece a una versión del cronograma:

```csharp
public class ActividadWBS : EntidadBase
{
    public int ProyectoId { get; set; }
    public int? CronogramaVersionId { get; set; } // ← NUEVO: versión a la que pertenece
    public int? ActividadPadreId { get; set; }

    public string CodigoWBS { get; set; }          // Ej: "1.8.2.5"
    public string Nombre { get; set; }
    public int NivelWBS { get; set; }
    public Disciplina? Disciplina { get; set; }
    public DateTime FechaInicioPlaneada { get; set; }
    public DateTime FechaFinPlaneada { get; set; }
    public decimal AvancePlanificado { get; set; }
    public decimal AvanceReal { get; set; }        // Editable inline en la tabla WBS
    public decimal CantidadTotal { get; set; }
    public string? Unidad { get; set; }
    public decimal CantidadEjecutadaAcumulada { get; set; }
    public bool EsCritica { get; set; }
    public bool Activo { get; set; } = true;
    public string? FrenteTrabajo { get; set; }
}
```

#### InformeDiario

Registro diario de campo. Tiene flujo de aprobación (Borrador → Enviado → Aprobado/Rechazado) y soporte de versiones del mismo informe. Relaciona `RegistrosAvance`, `Fotografias`, `RegistrosClima`.

#### Partida

Ítem del presupuesto contractual con soporte de jerarquía (partidas padre-hijo):

```csharp
public class Partida : EntidadBase
{
    public int ProyectoId { get; set; }
    public int? PadreId { get; set; }             // Jerarquía de partidas
    public string Codigo { get; set; }
    public string Descripcion { get; set; }
    public decimal CantidadPresupuestada { get; set; }
    public string? Unidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal ValorEjecutado { get; set; }   // Acumulado ejecutado
    public decimal MontoPresupuestado => CantidadPresupuestada * PrecioUnitario; // calculado
}
```

#### PlantillaHistograma + ItemHistograma

Histograma de recursos/valores planificados mensualmente:

```csharp
public class PlantillaHistograma : EntidadBase
{
    public int ProyectoId { get; set; }
    public string Nombre { get; set; }
    public TipoHistograma Tipo { get; set; }    // Personal, Equipos, Costos, etc.
    public int MesInicial { get; set; }          // 1-12
    public int AnioInicial { get; set; }
    public ICollection<ItemHistograma> Items { get; set; }
}

public class ItemHistograma : EntidadBase
{
    // 12 columnas: Mes1…Mes12 (decimal) — valores planificados por mes
    public string Descripcion { get; set; }
}
```

#### HistogramaReal + ItemHistogramaReal

Contraparte real del histograma planificado, para comparación visual:

```csharp
public class HistogramaReal : EntidadBase
{
    public int ProyectoId { get; set; }
    public string Nombre { get; set; }
    public TipoHistograma Tipo { get; set; }
    public int MesInicial { get; set; }
    public int AnioInicial { get; set; }
    public ICollection<ItemHistogramaReal> Items { get; set; }
}
```

### Enumeraciones del dominio

| Enum | Valores principales |
|------|---------------------|
| `EstadoProyecto` | Planificacion, EnEjecucion, Suspendido, Completado, Cancelado |
| `EstadoActividad` | Pendiente, EnProgreso, Completada, Suspendida, Cancelada |
| `EstadoInforme` | Borrador, EnRevision, Aprobado, Rechazado |
| `EstadoDocumento` | Borrador, EnRevision, Aprobado, Obsoleto |
| `EstadoNoConformidad` | Abierta, EnAnalisis, EnImplementacion, Cerrada |
| `EstadoRestriccion` | Abierta, EnGestion, Resuelta, Aceptada |
| `EstadoAvance` | Adelantado, EnTiempo, Atrasado, Critico |
| `SeveridadNoConformidad` | Baja, Media, Alta, Critica |
| `Disciplina` | Civil, Estructural, Mecanica, Electrica, Instrumentacion, HSE, Ambiental, Otro |
| `TipoHistograma` | Personal, Equipos, Costos, Materiales, Otro |

---

## Etapa 3 — Infraestructura, EF Core e Identity

### DbContext (extracto relevante)

```csharp
public class RenergeIADbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Proyecto> Proyectos => Set<Proyecto>();
    public DbSet<CronogramaVersion> CronogramasVersion => Set<CronogramaVersion>(); // NUEVO
    public DbSet<ActividadWBS> ActividadesWBS => Set<ActividadWBS>();
    public DbSet<InformeDiario> InformesDiarios => Set<InformeDiario>();
    public DbSet<RegistroAvanceDiario> RegistrosAvanceDiario => Set<RegistroAvanceDiario>();
    public DbSet<Partida> Partidas => Set<Partida>();
    public DbSet<CostoReal> CostosReales => Set<CostoReal>();
    public DbSet<PlantillaHistograma> PlantillasHistograma => Set<PlantillaHistograma>();
    public DbSet<ItemHistograma> ItemsHistograma => Set<ItemHistograma>();
    public DbSet<HistogramaReal> HistogramasReales => Set<HistogramaReal>();
    public DbSet<ItemHistogramaReal> ItemsHistogramaReal => Set<ItemHistogramaReal>();
    // ... resto de DbSets
}
```

**Configuraciones de relaciones importantes en `OnModelCreating`:**

- `ActividadWBS` → auto-referencia padre-hijo con `DeleteBehavior.Restrict`
- `ActividadWBS` → `CronogramaVersion` con `DeleteBehavior.Restrict`
- `CronogramaVersion` → `Proyecto` con `DeleteBehavior.Cascade`
- `Partida` → auto-referencia padre-hijo con `DeleteBehavior.Restrict`
- `InformeDiario` → `InformeDiarioAnterior` con `DeleteBehavior.NoAction`
- `RegistroClima` → `InformeDiario` con `DeleteBehavior.NoAction`
- `PlantillaHistograma` → `Items` con `DeleteBehavior.Cascade`
- `HistogramaReal` → `Items` con `DeleteBehavior.Cascade`

### Cadena de conexión y configuración de APIs

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=RenergeIA;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "AccuWeather": {
    "ApiKey": "TU_CLAVE_ACCUWEATHER_AQUI"
  }
}
```

La clave `AccuWeather:ApiKey` es necesaria para el módulo de Clima. Se obtiene gratis (plan de desarrollo) en [developer.accuweather.com](https://developer.accuweather.com). Si no se configura, el módulo muestra un error descriptivo pero no rompe el resto de la aplicación.

---

## Etapa 4 — Autenticación y roles RBAC

Se implementó autenticación completa con ASP.NET Core Identity. 13 roles definidos en `Roles.cs`. Seed automático al iniciar la aplicación crea roles y usuario admin (`admin@renergeia.com` / `Admin123!`).

Protección de páginas: `@attribute [Authorize]` en todos los componentes privados. Redirección automática al login mediante `RedirectToLogin.razor`.

| Rol | Acceso principal |
|-----|-----------------|
| Administrador | Acceso total |
| DirectorGeneral | Reportes ejecutivos, dashboards |
| GerenteProyecto | Gestión completa del proyecto asignado |
| IngenierosResidente | WBS, informes diarios, avances de campo |
| InspectorCalidad | No conformidades, acciones correctivas |
| CoordinadorHSE | Gestión HSE |
| AdministradorContrato | Presupuesto, contratos |
| JefeAlmacen | Inventario de materiales y equipos |
| SupervisorCampo | Informes diarios, personal, equipos en sitio |
| ControlCostos | Costos reales vs. presupuesto |
| Documentador | Gestión documental |
| Consultor | Solo lectura |
| Subcontratista | Registro de avance de sus actividades |

---

## Etapa 5 — Módulo de Proyectos

CRUD completo: listar, crear, editar, ver detalle.

| Ruta | Función |
|------|---------|
| `/proyectos` | Listado en tarjetas con badge de estado |
| `/proyectos/nuevo` | Formulario de creación |
| `/proyectos/{id}` | Hub central con accesos a todos los submódulos |
| `/proyectos/{id}/editar` | Edición de datos |

El `DetalleProyecto` actúa como hub central con accesos directos a: WBS, Informes, Documentos, Personal, Equipos, Costos, Histogramas, No Conformidades, Restricciones y Dashboard.

---

## Etapa 6 — Módulo WBS (Work Breakdown Structure)

### Rutas

| Ruta | Función |
|------|---------|
| `/proyectos/{id}/wbs` | Vista árbol de actividades (`ListaWBS.razor`) |
| `/proyectos/{id}/wbs/nueva` | Crear actividad (`FormWBS.razor`) |
| `/proyectos/{id}/wbs/{actId}/editar` | Editar actividad |

### Plantilla EPC fotovoltaico estándar

Cuando un proyecto no tiene actividades, se ofrece cargar automáticamente una plantilla pre-configurada con las 110 actividades típicas de un proyecto EPC solar. Cubre:

- **1.1** Hitos Principales del Contrato (5 sub-actividades)
- **1.2** Hitos Generales del Contrato
- **1.3** Permisos de construcción
- **1.4** Estudios de Ingeniería
- **1.5** Pull Out Test
- **1.6** Ingeniería de detalle
- **1.7** Suministros (3 sub-actividades, con 1.7.3 con 17 sub-actividades de tercer nivel)
- **1.8** Construcción Planta (15 sub-actividades, varias con sub-sub-actividades)
- **1.9** Culminación Sustancial
- **1.10** Cierre
- **1.11** Aceptación Provisional

La carga se hace por niveles (1→4) para respetar las claves foráneas padre-hijo.

Al cargar la plantilla, se crea automáticamente la versión de cronograma "Actividades Inicial".

### Cálculo de Avance Planificado

Se calcula dinámicamente en tiempo real según la fecha actual y las fechas planeadas de la actividad:

```csharp
AvancePlanificado = (Hoy - FechaInicio) / (FechaFin - FechaInicio) × 100
// Acotado entre 0% y 100%
```

### Cálculo de Desviación

```
Desviación = AvanceReal − AvancePlanificado
```
- Positivo (verde): adelantado
- Negativo (rojo): atrasado

### Estado operativo de cada actividad

| Estado | Condición |
|--------|-----------|
| Pendiente | Aún no ha comenzado (hoy < fecha inicio) |
| En Progreso | Desviación ≥ −5% |
| Atrasada | Desviación entre −5% y −15% |
| Crítica | Desviación < −15% |
| Finalizada | AvanceReal ≥ 100% |
| Sin Fechas | No tiene fechas configuradas |

---

## Etapa 7 — Módulo de Informe Diario

| Ruta | Función |
|------|---------|
| `/proyectos/{id}/informes` | Lista con filtros y badge de estado |
| `/proyectos/{id}/informes/crear` | Crear / editar informe |
| `/proyectos/{id}/informes/{infId}` | Ver detalle |

### Flujo de aprobación

```
Borrador → Enviado → Aprobado
                └→ Rechazado → (corrección) → Enviado (V2, V3...)
```

### InformeDiarioService

Servicio central que encapsula toda la lógica del módulo:

- **`CalcularAvanceEsperado(inicio, fin, fechaInforme)`** — distribución lineal del trabajo
- **`ActualizarCalculosAsync(registro, actividad)`** — calcula avance acumulado, desviación, días de atraso, estado
- **`KPIsProyectoAsync(proyectoId)`** — KPIs globales basados en últimos registros por actividad
- **`ObtenerDashboardCompletoAsync(proyectoId)`** — dashboard completo filtrado por **versión vigente del cronograma**
- **`DatosCurvaSAsync(proyectoId)`** — datos para gráfico de Curva S

> **Importante:** desde la implementación del control de versiones, `ObtenerDashboardCompletoAsync` solo considera las actividades de la **versión vigente** del cronograma. Esto garantiza que el dashboard siempre refleja el cronograma contractualmente activo.

Las actividades cargadas en el formulario de Informe Diario también se filtran por versión vigente.

---

## Etapa 8 — Módulo de Documentos

| Ruta | Función |
|------|---------|
| `/proyectos/{id}/documentos` | Lista con filtros por tipo, disciplina, estado |
| `/proyectos/{id}/documentos/crear` | Nuevo documento |
| `/proyectos/{id}/documentos/{docId}` | Ver y gestionar versiones |

Cada documento puede tener múltiples `VersionDocumento`. La versión activa es la más reciente aprobada.

---

## Etapa 9 — Dashboard analítico

**Ruta:** `/proyectos/{id}/dashboard` → `DashboardProyecto.razor`

### KPIs principales (primera fila)

| KPI | Cálculo |
|-----|---------|
| Avance Programado | Promedio de avances esperados de todas las actividades activas a hoy |
| Avance Real | Promedio de avances reales (último registro o `AvanceReal` directo) |
| Desviación | Real − Programado |
| SPI Global | Avance Real / Avance Programado |

### KPIs de actividades (segunda fila)

Conteo de actividades: Atrasadas, Críticas, Finalizadas, Estado General.

### Gráficos (Chart.js via `IJSRuntime`)

- **Curva S**: línea comparativa avance real vs planificado por fecha de informe
- **Barras por disciplina**: avance real vs programado agrupado por disciplina
- **Donut de estados**: distribución En Línea / Atrasadas / Críticas / Finalizadas / Sin iniciar
- **Barras horizontales**: top 10 actividades más atrasadas por desviación

### Dashboard del inicio (portafolio)

`HomeDashboardService` consolida KPIs de todos los proyectos en ejecución y los muestra en la página de inicio.

---

## Etapa 10 — Módulo de Costos y Partidas

**Ruta:** `/proyectos/{id}/costos` → `Costos.razor`

**Servicio:** `CostoService.cs`

### Modelo de datos

```
Partida (jerarquía padre-hijo)
  ├── 1. Ingeniería          → $120,000,000
  │   ├── 1.1 Básica         → $50,000,000
  │   └── 1.2 Detalle        → $70,000,000
  ├── 2. Procura             → $800,000,000
  └── 3. Construcción        → $500,000,000

CostoReal (registros de costos incurridos)
  → ProyectoId, PartidaId, Descripción, Cantidad, PrecioUnitario, Fecha
```

### Campos de Partida

- `Codigo`, `Descripcion`, `CantidadPresupuestada`, `Unidad`, `PrecioUnitario`
- `MontoPresupuestado` (calculado: cantidad × precio, ignorado por EF)
- `ValorEjecutado` (acumulado de `CostoReal` relacionados)
- `PadreId` (jerarquía multinivel)
- `SubPartidas` (colección de partidas hijas)

### CostoService

- Carga de partidas con jerarquía completa
- Cálculo de porcentaje de ejecución: `ValorEjecutado / MontoPresupuestado × 100`
- Acumulado de costos reales por partida

---

## Etapa 11 — Módulo de Histogramas

**Ruta:** `/proyectos/{id}/histogramas` → `Histogramas.razor`

**Servicio:** `HistogramaService.cs`

### Propósito

Visualizar la distribución temporal de recursos (personal, equipos, costos) mes a mes, tanto planificada como ejecutada, para comparación y control.

### Modelo de datos

```
PlantillaHistograma
  ├── ProyectoId
  ├── Nombre
  ├── Tipo (Personal, Equipos, Costos, ...)
  ├── MesInicial (1-12)
  ├── AnioInicial
  └── Items: List<ItemHistograma>
       └── Descripcion, Mes1...Mes12 (decimal)

HistogramaReal (contraparte del planificado)
  ├── ProyectoId, Nombre, Tipo, MesInicial, AnioInicial
  └── Items: List<ItemHistogramaReal>
       └── Descripcion, Mes1...Mes12 (decimal)
```

### Visualización

- Tabla mensual con columnas dinámicas (los 12 meses del proyecto)
- Gráfico de barras comparativo: planificado vs real por mes
- Totales y subtotales por tipo de recurso

---

## Etapa 12 — Control de versiones del cronograma

Esta es una de las funcionalidades más importantes del sistema para **trazabilidad contractual**.

### Concepto

Permite mantener un historial de versiones del cronograma:

```
Actividades Inicial (v0)       → EsVigente: false (pasa a histórica)
Actividades Reprogramación 1   → EsVigente: false
Actividades Reprogramación 2   → EsVigente: true  ← cronograma activo
```

Solo **una versión puede estar vigente** por proyecto en cualquier momento.

### Flujo de uso

1. Al cargar la plantilla EPC (o al acceder a un proyecto con actividades existentes), se crea automáticamente **"Actividades Inicial"** (NumeroVersion = 0, EsVigente = true).

2. Cuando hay un cambio de alcance, retraso significativo u otra causa contractual, el usuario hace clic en **"+ Crear reprogramación"**.

3. El sistema muestra un modal pidiendo el **motivo de reprogramación** (campo obligatorio).

4. Al confirmar:
   - La versión vigente pasa a `EsVigente = false` (histórica)
   - Se crea una nueva `CronogramaVersion` con el número siguiente
   - Se duplican **todas las actividades** de la versión anterior:
     - Se copian en **2 pasadas**: primero sin padres (para obtener nuevos IDs), luego se mapean los `ActividadPadreId` usando el diccionario `oldId → newId`
   - La nueva versión queda como vigente

### Regla de avance real

Cada reprogramación **hereda el AvanceReal acumulado** de la versión anterior. Así no se pierden los registros de ejecución ya realizados.

### Selector de versión en la UI

En la parte superior del módulo WBS aparece un desplegable:

```
Cronograma: [ Actividades Reprogramación 2 ★ ▼ ]   ● Vigente
```

- `★` indica la versión activa en el selector
- Badge verde **Vigente** o gris **Histórica**
- Al seleccionar una versión histórica se muestra un banner 🔒 y todos los campos quedan en modo solo lectura

### Modo solo lectura para versiones históricas

| Elemento | Versión Vigente | Versión Histórica |
|----------|----------------|------------------|
| Av. Real | Editable (input) | Solo lectura (texto) |
| Fechas | Editables (date picker) | Deshabilitadas |
| Botón Editar | Visible | Oculto |
| Botón Activo/Inactivo | Visible | Oculto |
| Guardar cambios | Visible | Oculto |
| + Crear reprogramación | Visible | Oculto |
| + Nueva actividad | Visible | Oculto |

### Impacto en otros módulos

- **Dashboard**: solo considera actividades de la versión vigente
- **Informe Diario**: solo carga actividades de la versión vigente
- **FormWBS** (nueva actividad): asigna automáticamente la versión vigente a la actividad creada

### Auto-migración de datos existentes

Si un proyecto ya tiene actividades sin `CronogramaVersionId` (datos anteriores a la implementación), al entrar al módulo WBS el sistema crea automáticamente "Actividades Inicial" y asigna todas las actividades existentes a esa versión. Esto se hace con `ExecuteUpdateAsync` para eficiencia.

---

## Etapa 13 — Módulo de Clima

**Ruta:** `/proyectos/{id}/clima` → `Clima.razor`

### Propósito

Registrar automáticamente las condiciones climáticas del sitio de construcción directamente desde AccuWeather, asociadas a la ubicación geográfica exacta del proyecto. El historial climático queda almacenado en la BD para trazabilidad y análisis de impacto en avance de obra.

### Dependencias externas

| Librería / API | Versión | Clave requerida | Uso |
|----------------|---------|-----------------|-----|
| Leaflet.js | 1.9.4 (CDN) | No | Mapa interactivo para seleccionar coordenadas |
| Open-Meteo API | REST v1 | **No (gratis)** | Clima actual + pronóstico 16 días |

Ambas se cargan en `App.razor`:
```html
<!-- CSS -->
<link rel="stylesheet" href="https://unpkg.com/leaflet@1.9.4/dist/leaflet.css" />

<!-- JS (antes de blazor.web.js) -->
<script src="https://unpkg.com/leaflet@1.9.4/dist/leaflet.js"></script>
<script src="js/app.js"></script>
```

> **Nota:** el campo `AccuWeatherLocationKey` en `Proyectos` quedó en la BD de una versión anterior pero ya no se usa. Puede dejarse sin valor.

### Flujo de uso

```
1. Abrir módulo Clima del proyecto
2. [A] Ingresar País, Departamento, Municipio, Dirección (texto libre)
3. [B] Hacer clic en el mapa Leaflet  ──┐
   ─ O bien ─                           │ Ambos métodos actualizan
   Escribir Latitud + Longitud manualmente ─┘ los campos y mueven el marcador
4. [C] Confirmar ubicación → guarda lat/lng/municipio/departamento en BD
5. [D] Consultar clima y pronóstico → llama Open-Meteo con lat/lng
        → muestra clima actual + pronóstico 7 días (tarjetas) + tabla 16 días
        → guarda automáticamente el clima actual en RegistroClima
6. [E] Ver historial climático (últimos 100 registros guardados)
```

### Mapa Leaflet (JavaScript Interop)

La función `leafletMap` en `app.js` implementa tres operaciones:

| Función JS | Llamada desde C# | Propósito |
|-----------|-----------------|-----------|
| `leafletMap.init(divId, lat, lng, dotNetRef)` | `OnAfterRenderAsync` | Inicializa el mapa; si hay coordenadas previas, coloca marcador y centra |
| `leafletMap.setMarker(lat, lng)` | Al cambiar inputs manuales | Mueve el marcador sin recargar el mapa |
| `leafletMap.destroy()` | `DisposeAsync` | Destruye la instancia Leaflet al salir de la página |

Cuando el usuario hace clic en el mapa, Leaflet llama de vuelta a C# vía:
```javascript
dotNetRef.invokeMethodAsync('OnMapClick', lat, lng);
```

El método `[JSInvokable] OnMapClick(double lat, double lng)` actualiza los campos del formulario en Blazor.

### API Open-Meteo utilizada

```
GET https://api.open-meteo.com/v1/forecast
    ?latitude={lat}&longitude={lng}
    &current=temperature_2m,relative_humidity_2m,apparent_temperature,
             precipitation,rain,weather_code,wind_speed_10m
    &daily=weather_code,temperature_2m_max,temperature_2m_min,
           precipitation_sum,precipitation_probability_max,wind_speed_10m_max
    &timezone=auto
    &forecast_days=16
```

Devuelve condiciones actuales + array de 16 días con temperatura máx/mín, precipitación acumulada, probabilidad de lluvia y viento máximo.

### Códigos WMO → `CondicionClimatica`

| Código WMO | Descripción | Condición mapeada |
|-----------|-------------|------------------|
| 0, 1 | Despejado / Mayormente despejado | Soleado |
| 2 | Parcialmente nublado | ParcialmenteNublado |
| 3 | Nublado | Nublado |
| 45, 48 | Niebla | Niebla |
| 51-57, 61-63, 80-81 | Llovizna / Lluvia / Chubascos | Lluvia |
| 65, 82 | Lluvia fuerte / Chubascos fuertes | LluviaFuerte |
| 95-99 | Tormenta eléctrica / con granizo | Tormenta |

### Lógica "Apto para obra"

Cada día del pronóstico recibe un badge verde/rojo según:
```
Apto = probabilidad lluvia < 60%
    AND precipitación < 5 mm
    AND viento < 40 km/h
    AND sin código de tormenta (95, 96, 99)
```

### Datos guardados en `RegistroClima`

Cada consulta al clima crea un registro automático con: `ProyectoId`, `Fecha` (hora local), `Condicion`, `TemperaturaMaxima`, `TemperaturaMinima` (ambas = temp. actual), `HumedadRelativa`, `VelocidadViento`, `PrecipitacionMm`, `AfectoActividades = false`, `Observaciones = "Open-Meteo | {DescripcionCondicion}"`.

El pronóstico (16 días) **no se guarda** en BD ya que cambia a diario y se consulta en tiempo real.

---

## Mejoras UI en el módulo WBS

### Columnas redimensionables (tipo Excel / MS Project)

Implementado con **JavaScript Interop** (`IJSRuntime`). La función `wbsResize.init` en `app.js`:

1. Agrega un `<div class="wbs-resize-handle">` al borde derecho de cada `<th>`
2. En `mousedown` inicia el drag; en `mousemove` actualiza el ancho del `<th>`; en `mouseup` guarda en `localStorage`
3. En `OnAfterRenderAsync` de Blazor se re-llama `wbsResize.init` para restaurar anchos después de cada re-render
4. Los anchos se persisten en `localStorage` bajo la clave `wbs-col-widths`

**Anchos mínimos:**
- Columna Actividad: 300px
- Resto de columnas: 60px

La tabla usa `table-layout: fixed` para que los anchos explícitos sean respetados.

### Av. Planificado

- Se eliminó la barra de progreso gris
- Solo muestra el valor numérico en **negrita negra** (`100.0%`)
- Calculado dinámicamente en el servidor según la fecha actual

### Av. Real editable inline

- Campo `<input type="number">` directamente en la celda de la tabla
- Guarda en base de datos al perder el foco (`@onchange`)
- Validación: rango 0-100, sin negativos (tanto en HTML `min="0"` como en servidor `Math.Clamp`)
- Usa `InvariantCulture` para evitar problemas con separador decimal en entornos en español (coma vs punto)

### Botón "Guardar cambios"

- Botón verde en la barra de herramientas
- Guarda todos los cambios pendientes en EF Core (`Db.SaveChangesAsync()`)
- Muestra spinner mientras guarda y mensaje "✓ Cambios guardados" por 3 segundos
- Solo visible en la versión vigente

---

## Módulos adicionales del modelo

Estos módulos tienen entidades definidas y páginas UI básicas implementadas:

### Personal (`PersonalProyecto`)

`/proyectos/{id}/personal` — Listado y formulario de alta.

Campos: nombre, tipo (Propio/Subcontratado/Visitante), empresa, cargo, fechas de asignación. Relacionado con `DocumentoPersona` para gestión de documentos de vinculación.

### Equipos (`Equipo`)

`/proyectos/{id}/equipos` — Listado y formulario de alta.

Campos: tipo de equipo, placa/serial, empresa propietaria, fechas de ingreso/salida. Relacionado con `RegistroHorometro` (horas de uso) y `Mantenimiento`.

### No Conformidades (`NoConformidad`)

`/proyectos/{id}/noconformidades` — Listado de desviaciones de calidad.

Estados: Abierta → En Análisis → En Implementación → Cerrada. Cada NC puede tener una o más `AccionesCorrectivas`.

### Restricciones (`Restriccion`)

`/proyectos/{id}/restricciones` — Impedimentos que bloquean actividades.

Estados: Abierta → En Gestión → Resuelta / Aceptada.

---

## Migraciones de base de datos

### Historial completo

| Migración | Fecha | Contenido |
|-----------|-------|-----------|
| `InitialCreate` | 2026-06-07 | Tablas base: Proyectos, ActividadesWBS, InformesDiarios, RegistrosAvance, Personal, Equipos, Partidas, Costos, Restricciones, NoConformidades, Alertas, Fotografias, Identity |
| `AddActivoWBS` | 2026-06-08 | Campo `Activo` en `ActividadesWBS` |
| `AgregarDisciplinaYRelacionClima` | 2026-06-09 | Campo `Disciplina` en WBS; entidad `RegistroClima` |
| `AgregarCamposActividadWBS` | 2026-06-09 | `CantidadTotal`, `Unidad`, `CantidadEjecutadaAcumulada`, `EsCritica`, `FrenteTrabajo` |
| `AgregarModuloDocumentos` | 2026-06-11 | Tablas `Documentos` y `VersionesDocumento` con `DisciplinaDocumento` |
| `AgregarJerarquiaPartidas` | 2026-06-21 | Jerarquía padre-hijo en `Partidas` |
| `AgregarHistogramas` | 2026-06-21 | Tablas `PlantillasHistograma` e `ItemsHistograma` con columnas Mes1-Mes12 |
| `AgregarPorcentajeEjecutadoPartida` | 2026-06-21 | Campo de ejecución en `Partidas` |
| `ReemplazarPorcentajePorValorEjecutado` | 2026-06-21 | Reemplaza % por valor monetario ejecutado en `Partidas` |
| `AgregarMesInicialHistograma` | 2026-06-22 | Campo `MesInicial` en `PlantillasHistograma` |
| `AgregarHistogramaReal` | 2026-06-22 | Tablas `HistogramasReales` e `ItemsHistogramaReal` |
| `AgregarAnioInicialHistograma` | 2026-06-23 | Campo `AnioInicial` en histogramas |
| `AgregarCronogramaVersion` | 2026-06-23 | Tabla `CronogramasVersion`; campo `CronogramaVersionId` en `ActividadesWBS` |
| `AgregarUbicacionClimatica` | 2026-06-23 | Campos `Departamento`, `Municipio`, `Latitud`, `Longitud`, `AccuWeatherLocationKey` en `Proyectos` |

### Comandos de migración

```powershell
# Crear una nueva migración (ejecutar desde la raíz de la solución)
dotnet ef migrations add NombreDeLaMigracion `
    --project RenergeIA.Infrastructure `
    --startup-project RenergeIA.Web

# Aplicar migraciones pendientes a la base de datos
dotnet ef database update `
    --project RenergeIA.Infrastructure `
    --startup-project RenergeIA.Web

# Ver el historial de migraciones
dotnet ef migrations list `
    --project RenergeIA.Infrastructure `
    --startup-project RenergeIA.Web
```

> **Importante:** ejecutar siempre desde la carpeta raíz de la solución (`Proyecto Agente`), nunca desde dentro de un proyecto individual. Si la app está corriendo, detenerla primero (`Stop-Process -Id <PID> -Force`).

---

## Cómo correr el proyecto

### Primera vez (configuración inicial)

```powershell
cd "C:\Users\Luisa_Becerra\OneDrive - Renergeia LLC\Escritorio\RenergeIA - Agente\Proyecto Agente"

dotnet restore

dotnet ef database update `
    --project RenergeIA.Infrastructure `
    --startup-project RenergeIA.Web

dotnet run --project RenergeIA.Web
```

### Ejecución normal

```powershell
dotnet run --project RenergeIA.Web
```

Abrir en el navegador: `https://localhost:5001` o `http://localhost:5000`

### Si hay error "archivo bloqueado por otro proceso"

```powershell
# Matar proceso dotnet que tiene los archivos bloqueados
Get-Process dotnet | Stop-Process -Force
# Luego volver a correr
dotnet run --project RenergeIA.Web
```

### Credenciales por defecto

| Campo | Valor |
|-------|-------|
| Email | `admin@renergeia.com` |
| Contraseña | `Admin123!` |

---

## Fases de desarrollo

### Fase 1 — Operativa ← EN PROGRESO

**Completado:**
- [x] Estructura base de la solución (Clean Architecture 3 capas)
- [x] Modelo de datos completo (25+ entidades)
- [x] Autenticación con Identity y 13 roles RBAC
- [x] Módulo de Proyectos (CRUD completo)
- [x] Módulo WBS con plantilla EPC fotovoltaico estándar (110 actividades)
- [x] Columnas redimensionables en WBS (drag & drop tipo Excel, con localStorage)
- [x] Edición inline de Avance Real en WBS
- [x] Botón "Guardar cambios" con feedback visual
- [x] Control de versiones del cronograma (Inicial + Reprogramaciones)
- [x] Duplicación de jerarquía padre-hijo en reprogramaciones
- [x] Modo solo lectura para versiones históricas del cronograma
- [x] Módulo de Informes Diarios con flujo de aprobación
- [x] Módulo de Documentos con control de versiones
- [x] Dashboard analítico por proyecto (KPIs + 4 tipos de gráficos Chart.js)
- [x] Dashboard de portafolio (página de inicio)
- [x] Módulo de Costos y Partidas con jerarquía
- [x] Módulo de Histogramas (planificado + real, gráfico comparativo)
- [x] Módulo de Personal
- [x] Módulo de Equipos
- [x] Módulo de No Conformidades (estructura)
- [x] Módulo de Restricciones (estructura)
- [x] Módulo de Clima con mapa Leaflet y API AccuWeather (ubicación + historial climático)

**Pendiente Fase 1:**
- [ ] Gestión de usuarios desde la UI (CRUD de cuentas)
- [ ] Carga de archivos físicos de documentos
- [ ] Generación de reportes PDF (QuestPDF)
- [ ] Sistema de Alertas automáticas con notificaciones
- [ ] Comparativo de versiones de cronograma (Inicial vs Reprogramación N)
- [ ] Registro de clima desde el formulario de Informe Diario (vinculado al módulo de Clima)
- [ ] Horómetros y mantenimiento de equipos

### Fase 2 — Analítica (meses 5-8)

- [ ] Migración a Azure SQL Database
- [ ] Almacenamiento de archivos en Azure Blob Storage
- [ ] Reportes automáticos por email
- [ ] Integración con Azure OpenAI para resúmenes inteligentes
- [ ] Valor Ganado (EVM): CPI, SPI avanzado, EAC

### Fase 3 — Avanzada (meses 9-14)

- [ ] IA predictiva de retrasos y sobrecostos
- [ ] App móvil (MAUI Blazor Hybrid) para registro en campo sin internet
- [ ] API pública para integración con sistemas de clientes
- [ ] Tablero ejecutivo multi-proyecto

---

## Glosario

| Término | Definición |
|---------|-----------|
| **EPC** | Engineering, Procurement & Construction — contrato llave en mano donde el contratista es responsable de ingeniería, compra de equipos y construcción |
| **WBS** | Work Breakdown Structure — Estructura de Desglose de Trabajo; descomposición jerárquica de todas las actividades |
| **RBAC** | Role-Based Access Control — los permisos se asignan a roles, no directamente a usuarios |
| **KWp** | Kilowattpeak — medida de potencia máxima de un sistema fotovoltaico en condiciones estándar |
| **Curva S** | Gráfico del avance acumulado a lo largo del tiempo; la forma de "S" es característica de proyectos con lento inicio, fase intensa y cierre gradual |
| **EVM** | Earned Value Management — metodología que integra alcance, tiempo y costo |
| **SPI** | Schedule Performance Index — índice de desempeño del cronograma: AvanceReal / AvanceProgramado |
| **KPI** | Key Performance Indicator — indicador clave de desempeño |
| **Línea base** | Versión del cronograma aprobada contractualmente como referencia de control |
| **Reprogramación** | Nueva versión del cronograma aprobada que reemplaza a la anterior como referencia vigente, conservando los avances reales acumulados |
| **EF Core** | Entity Framework Core — ORM de Microsoft para .NET que mapea clases C# a tablas SQL |
| **ORM** | Object-Relational Mapper — capa que traduce entre objetos C# y tablas de base de datos |
| **Blazor Server** | Framework donde la UI se renderiza en el servidor y se sincroniza con el navegador via WebSocket (SignalR) |
| **IJSRuntime** | Servicio de Blazor para ejecutar código JavaScript desde C# (interoperabilidad) |
| **Clean Architecture** | Patrón de arquitectura en capas con dependencias hacia el interior: Web → Infrastructure → Core |
| **O&M** | Operación y Mantenimiento — fase posterior a la construcción del parque solar |
| **HSE** | Health, Safety & Environment — Salud, Seguridad y Medio Ambiente |
| **No Conformidad (NC)** | Desviación detectada respecto a un requisito de calidad, seguridad o contrato |
| **LocalStorage** | Almacenamiento del navegador que persiste entre sesiones; se usa para guardar anchos de columnas WBS |
| **Leaflet.js** | Librería JavaScript open-source para mapas interactivos; se usa en el módulo de Clima para seleccionar coordenadas del proyecto |
| **LocationKey** | Identificador único de AccuWeather para una ubicación geográfica; se obtiene con el endpoint Geoposition Search y se usa para consultar condiciones climáticas |
| **Open-Meteo** | API meteorológica REST completamente gratuita y sin clave; provee clima actual y pronóstico de hasta 16 días usando modelos GFS/ECMWF |
| **WMO Code** | Código estándar de la Organización Meteorológica Mundial para describir condiciones climáticas (0=Despejado, 95=Tormenta, etc.) |
| **No Conformidad (NC)** | Desviación detectada respecto a un requisito de calidad, seguridad o contrato; tiene severidad (Baja/Media/Alta/Crítica) y estados de resolución |

---

*Guía actualizada el 24 de junio de 2026 — RenergeIA v1.0 en desarrollo activo.*

Prueba de que voy a subir el archivo