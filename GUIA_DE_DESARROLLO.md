# Guía de Desarrollo — RenergeIA

> Documento de referencia completo del proceso de construcción de la plataforma RenergeIA.
> Audiencia: equipo interno de Renergeia S.A.S. / desarrolladores que incorporen el proyecto.
> Última actualización: **1 de julio de 2026 — Motor de Auditoría HSEQ genérico multi-norma, Matriz de Riesgos IPERV con Inspecciones asistidas por IA, y módulo de Costos ampliado con Compromisos de Gasto**

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
23. [Control de versiones con GitHub](#control-de-versiones-con-github)
24. [Fases de desarrollo](#fases-de-desarrollo)
25. [Sistema de Diseño RenergeIA](#25-sistema-de-diseño-renergeia)
26. [Módulo HSEQ — Dashboards analíticos](#26-módulo-hseq--dashboards-analíticos)
27. [Módulo HSEQ — Calidad ISO 9001](#27-módulo-hseq--calidad-iso-9001)
28. [Módulo HSEQ — Ambiental ISO 14001](#28-módulo-hseq--ambiental-iso-14001)
29. [Módulo HSEQ — Social](#29-módulo-hseq--social)
30. [Módulo HSEQ — Motor de Auditoría Genérico (Corporativo)](#30-módulo-hseq--motor-de-auditoría-genérico-corporativo)
31. [Módulo HSEQ Seguridad — Matriz de Riesgos IPERV e Inspecciones con IA](#31-módulo-hseq-seguridad--matriz-de-riesgos-iperv-e-inspecciones-con-ia)
32. [Glosario](#glosario)

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
| Control de versiones | Git + GitHub | — |
| Exportación Excel | ClosedXML | 0.104.2 |

### Paquetes NuGet instalados

**RenergeIA.Infrastructure**
```
Microsoft.AspNetCore.Identity.EntityFrameworkCore  10.0.x
Microsoft.EntityFrameworkCore.SqlServer            10.0.x
Microsoft.EntityFrameworkCore.Tools                10.0.x
```

**RenergeIA.Web**
```
Microsoft.AspNetCore.Identity.UI      10.0.x
Microsoft.EntityFrameworkCore.Design  10.0.x
ClosedXML                             0.104.2   ← Exportación de archivos Excel (.xlsx)
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
    │   ├── Shared/
    │   │   └── Dashboard/                ← Sistema de Diseño — componentes reutilizables
    │   │       ├── GaugeCircular.razor   ← Gauge SVG animado (original, compatible)
    │   │       ├── GaugeKPI.razor        ← Gauge SVG v2 con tendencia
    │   │       ├── TarjetaKPI.razor      ← KPI con icono y valor grande (original)
    │   │       ├── ExecutiveCard.razor   ← Tarjeta ejecutiva v2 con tendencia
    │   │       ├── DonutKPI.razor        ← Donut CSS con valor central
    │   │       ├── SeccionDash.razor     ← Encabezado de sección con línea azul
    │   │       ├── ChartCard.razor       ← Tarjeta contenedora de gráficos Chart.js
    │   │       ├── AnalisisIA.razor      ← Panel IA (original, compatible)
    │   │       ├── AIPanel.razor         ← Panel IA v2 con slots Hallazgos/Alertas/Recomendaciones
    │   │       ├── DashboardLayout.razor ← Contenedor principal de módulo
    │   │       ├── PageHeader.razor      ← Cabecera de página con volver + acciones
    │   │       ├── FilterBar.razor       ← Barra de filtros / navegación de sub-módulos
    │   │       ├── StatusChip.razor      ← Badge semántico de estado (Saludable/Riesgo/Crítico)
    │   │       └── SmartTable.razor      ← Tabla con búsqueda, vacío y pie configurables
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
    │       ├── Dashboard/
    │       │   └── DashboardProyecto.razor
    │       └── HSEQ/
    │           ├── HSEQDashboard.razor           ← Hub central HSEQ (4 divisiones)
    │           ├── Seguridad/                    ← (ver sección 26)
    │           ├── Calidad/
    │           │   ├── CalidadDashboard.razor
    │           │   ├── ChecklistISO9001.razor
    │           │   ├── PPIs.razor
    │           │   ├── ControlCalibracion.razor
    │           │   ├── ControlDocumental.razor
    │           │   ├── NoConformidadesHSEQ.razor
    │           │   └── AccionesCorrectivasHSEQ.razor
    │           ├── Ambiental/
    │           │   ├── AmbientalDashboard.razor
    │           │   ├── ChecklistISO14001.razor
    │           │   ├── InspeccionesAmbientales.razor
    │           │   ├── GestionResiduos.razor
    │           │   ├── AspectosImpactos.razor
    │           │   ├── Derrames.razor
    │           │   ├── FaunaFlora.razor
    │           │   └── AccionesAmbientales.razor
    │           └── Social/
    │               ├── SocialDashboard.razor
    │               ├── Comunidades.razor
    │               ├── ReunionesComunitarias.razor
    │               ├── PQRModule.razor
    │               ├── CompromisosSociales.razor
    │               ├── ContratacionLocal.razor
    │               ├── ComprasLocales.razor
    │               └── ActasEvidencias.razor
    ├── Services/
    │   ├── InformeDiarioService.cs       ← KPIs, Curva S, Dashboard, filtrado por versión
    │   ├── DocumentoService.cs
    │   ├── CostoService.cs               ← cálculos de partidas y costos reales
    │   ├── HistogramaService.cs          ← lógica de histogramas planificado/real
    │   └── HomeDashboardService.cs       ← Dashboard del inicio (portafolio)
    ├── Program.cs
    ├── appsettings.json                  ← incluye AccuWeather:ApiKey
    └── wwwroot/
        ├── app.css                       ← Tema corporativo + Sistema de Diseño RenergeIA
        └── js/
            └── app.js                    ← Chart.js helpers + wbsResize + leafletMap + cap charts
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
| Git | Control de versiones local |
| Cuenta GitHub | Repositorio remoto del proyecto |

### Verificar instalación

```powershell
dotnet --version   # Debe mostrar 10.x.x
git --version      # Debe mostrar 2.x.x
```

### Clonar el repositorio (nuevos colaboradores)

```powershell
git clone https://github.com/[organización]/renergeia.git "Proyecto Agente"
cd "Proyecto Agente"
dotnet restore
dotnet ef database update --project RenergeIA.Infrastructure --startup-project RenergeIA.Web
dotnet watch run --urls http://localhost:5169 --project RenergeIA.Web
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
  },
  "Anthropic": {
    "ApiKey": "TU_CLAVE_ANTHROPIC_AQUI",
    "Model": "claude-sonnet-4-6"
  }
}
```

La clave `AccuWeather:ApiKey` es necesaria para el módulo de Clima. Se obtiene gratis (plan de desarrollo) en [developer.accuweather.com](https://developer.accuweather.com). Si no se configura, el módulo muestra un error descriptivo pero no rompe el resto de la aplicación.

La clave `Anthropic:ApiKey` es necesaria para el análisis automático de inspecciones con IA del módulo HSEQ Seguridad — Matriz de Riesgos (ver sección 31). `IAInspeccionService` la usa para llamar a la API de Anthropic (`https://api.anthropic.com/v1/messages`) y generar la valoración GTC 45 de un peligro a partir de una foto y/o descripción de la inspección. Se obtiene en [console.anthropic.com](https://console.anthropic.com). Sin esta clave, el botón de análisis con IA falla mostrando un error, pero el resto del módulo (captura manual de riesgos) sigue funcionando.

> **Pendiente:** actualizar `Model` a un identificador vigente (p. ej. `claude-sonnet-5`) — `claude-sonnet-4-6` es una versión anterior de Sonnet que sigue activa pero ya no es la más reciente en el momento de escribir esta guía.

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

### Rediseño con pestañas (5 sub-vistas)

`Costos.razor` pasó de ser una página monolítica a un **contenedor con pestañas** que delega en 5 componentes independientes. Esto redujo el archivo de ~674 a ~40 líneas propias:

| Pestaña | Componente | Función |
|---------|-----------|---------|
| Presupuesto | `CostosPresupuesto.razor` | Gestión de partidas y presupuesto EPC por disciplina (jerarquía padre-hijo) |
| Real / Ejecutado | `CostosEjecutado.razor` | Registro de costos reales incurridos, con adjunto de evidencia (factura, vale, recibo) |
| Compromisos | `CostosCompromisos.razor` | CRUD de compromisos de gasto (órdenes de compra) — **nuevo** |
| Dashboard | `CostosDashboard.razor` | Vista consolidada: presupuesto vs ejecutado vs compromisos |
| Comparativo | `CostosComparativo.razor` | Análisis lado a lado de las tres magnitudes anteriores |

`Costos.razor` solo mantiene el encabezado del proyecto, la barra de pestañas y un `switch` que renderiza el componente activo (`_tabActivo`), pasando `ProyectoId` como parámetro.

### Modelo de datos

```
Partida (jerarquía padre-hijo)
  ├── 1. Ingeniería          → $120,000,000
  │   ├── 1.1 Básica         → $50,000,000
  │   └── 1.2 Detalle        → $70,000,000
  ├── 2. Procura             → $800,000,000
  └── 3. Construcción        → $500,000,000

CostoReal (registros de costos incurridos)
  → ProyectoId, PartidaId, Descripción, Cantidad, PrecioUnitario, Fecha, AdjuntoUrl

CompromisoCosto (compromisos de gasto — órdenes de compra) ← NUEVO
  → ProyectoId, PartidaId?, Codigo, Proveedor, Valor, Fecha, FechaVencimiento,
    Estado (EstadoCompromiso), Prioridad, Observaciones
```

### Campos de Partida

- `Codigo`, `Descripcion`, `CantidadPresupuestada`, `Unidad`, `PrecioUnitario`
- `MontoPresupuestado` (calculado: cantidad × precio, ignorado por EF)
- `ValorEjecutado` (acumulado de `CostoReal` relacionados)
- `PadreId` (jerarquía multinivel)
- `SubPartidas` (colección de partidas hijas)

### CostoReal — campo nuevo

- `AdjuntoUrl` (`string?`, máx. 500 caracteres): ruta al comprobante digital (factura, vale, recibo) subido al registrar el costo real desde `CostosEjecutado.razor`.

### CompromisoCosto ← NUEVO

Representa un compromiso de compra (orden de compra, adquisición) vinculado al proyecto y, opcionalmente, a una partida específica. Permite ver el "pipeline" de gasto que aún no se ha ejecutado pero que ya está comprometido, complementando el presupuesto (planificado) y el `CostoReal` (ya incurrido):

```csharp
public class CompromisoCosto : EntidadBase
{
    public int ProyectoId { get; set; }
    public int? PartidaId { get; set; }           // Opcional: partida asociada
    public string Codigo { get; set; }
    public string Proveedor { get; set; }
    public decimal Valor { get; set; }             // Monto comprometido
    public DateTime Fecha { get; set; }
    public DateTime? FechaVencimiento { get; set; }
    public EstadoCompromiso Estado { get; set; }
    public string? Prioridad { get; set; }          // Normal, Alta, Urgente
    public string? Observaciones { get; set; }
}
```

`EstadoCompromiso`: `Pendiente` → `Aprobado` → `EnProceso` → `Pagado` | `Vencido` → `Cancelado`.

Relaciones en `OnModelCreating`: FK a `Proyecto` con `DeleteBehavior.Restrict`, FK a `Partida` con `DeleteBehavior.SetNull` (si se borra la partida, el compromiso queda sin partida asociada en vez de borrarse).

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
| `AddSeguridadV2` | 2026-06-26 | Tablas nuevas: `PlanesTrabajoHSE`, `PausasActivas`, `CapacitacionesPlanificadas`. Columnas nuevas en `InspeccionesSST` (Area, Responsable, ActosInseguros, CondicionesInseguras, AccionesGeneradas, ResponsableCierre, FechaCompromiso), `Capacitaciones` (Responsable, Area, Estado), `EntregasEPP` (Documento, Area, TipoEntrega, Talla) |
| `AddCamposISO9001` | 2026-06-29 | Renombra `Evidencia` → `OportunidadMejora` en `ItemsChecklist`; agrega `Clausula`, `EvidenciaUrl`, `Hallazgo`, `Plazo`, `Puntaje`, `Seguimiento`, `TituloClausula`; agrega `EstadoAuditoria`, `UsuarioId` en `ChecklistsAuditoria` |
| `AddSeguridadIPERV` | 2026-06-30 | Tablas nuevas: `BibliotecaPeligros` (corporativa, sin FK), `InspeccionesIA` (FK a Proyectos), `RiesgosIPERV` (FK a Proyectos e InspeccionesIA) |
| `AddCompromisoCosto` | 2026-06-30 | Tabla nueva `CompromisoCostos` (FK a Proyecto y Partida); agrega `AdjuntoUrl` en `CostosReales`; hace `ProyectoId` nullable (`SetNull`) en `ChecklistsAuditoria` para permitir auditorías corporativas sin proyecto |

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

## Control de versiones con GitHub

El proyecto usa **Git + GitHub** como sistema de control de versiones. El repositorio remoto centraliza el código y permite colaboración, respaldo y trazabilidad de cambios.

### Configuración inicial del repositorio (ya realizada)

```powershell
# Desde la carpeta raíz del proyecto
git init
git add .
git commit -m "Initial commit — RenergeIA v1.0"
git branch -M main
git remote add origin https://github.com/[organización]/renergeia.git
git push -u origin main
```

### Flujo de trabajo diario

```powershell
# 1. Antes de empezar — traer los últimos cambios
git pull origin main

# 2. Hacer cambios en el código...

# 3. Ver qué cambió
git status
git diff

# 4. Registrar los cambios
git add RenergeIA.Web/Components/Pages/HSEQ/Seguridad/EntregaEPP.razor
git add RenergeIA.Web/wwwroot/js/app.js
# (o agregar todos los cambios del día)
git add .

# 5. Crear el commit con mensaje descriptivo
git commit -m "feat: exportación Excel en EPP/Dotación con ClosedXML"

# 6. Subir al repositorio remoto
git push origin main
```

### Convención de mensajes de commit

Usar prefijos para facilitar la lectura del historial:

| Prefijo | Cuándo usarlo |
|---------|--------------|
| `feat:` | Nueva funcionalidad |
| `fix:` | Corrección de un error |
| `ui:` | Cambio visual / de estilos |
| `db:` | Nueva migración de base de datos |
| `refactor:` | Reorganización de código sin cambiar funcionalidad |
| `docs:` | Actualización de documentación |

**Ejemplos:**
```
feat: módulo HSEQ Seguridad — Plan de Trabajo, OTS, STC, Pausas Activas
fix: botón imprimir EPP ahora llama window.imprimirPagina
db: migración AddSeguridadV2 — tablas y columnas nuevas en HSEQ
ui: gráfico de barras Plan de Trabajo en Dashboard Seguridad
docs: guía de desarrollo actualizada con GitHub y módulo HSEQ
```

### Archivos que NO se suben a GitHub (.gitignore)

El archivo `.gitignore` en la raíz debe excluir:

```
# Build outputs
**/bin/
**/obj/

# Secretos y configuración local
**/appsettings.Development.json
**/appsettings.Local.json

# Base de datos local (si usas SQLite en desarrollo)
*.db
*.sqlite

# VS Code y Visual Studio
.vs/
.vscode/
*.user

# Archivos temporales de ngrok
ngrok.exe
```

> **Importante:** `appsettings.json` con la cadena de conexión real **no debe subirse** al repositorio. Cada desarrollador configura su propia conexión local. En producción (Fase 2), se usan variables de entorno de Azure.

### Ver el historial de cambios

```powershell
git log --oneline --graph    # Historial resumido con árbol
git log --since="1 week ago" # Solo la última semana
git diff HEAD~1              # Qué cambió en el último commit
```

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
- [x] Módulo HSEQ — sub-módulo **Seguridad** completo:
  - [x] Dashboard Seguridad con KPIs por sección y gráfico de barras (Chart.js) para Plan de Trabajo
  - [x] Plan de Trabajo HSE (CRUD con filtros por estado y mes)
  - [x] Inspecciones de Seguridad (filtradas por tipo = "Seguridad")
  - [x] OTS — Observaciones de Trabajo Seguro
  - [x] STC — Seguridad en Trabajo en Casa
  - [x] Indicadores de Seguridad (IF, IG, II desde incidentes reales)
  - [x] EPP / Dotación con exportación Excel (ClosedXML) e impresión
  - [x] Pausas Activas con meta 60/mes y % cumplimiento por persona
  - [x] Capacitaciones — **dashboard ejecutivo 5 niveles**: filtros, tarjeta personas, 8 gauges, 4 gráficos Chart.js (programadas vs ejecutadas por mes, HH, cumplimiento por área, por tema), timeline de vencidas, tabla inteligente con búsqueda, AnalisisIA con insights automáticos
  - [x] Incidentes y Accidentes
  - [x] ISO 45001 (Checklist de auditoría)
  - [x] Acciones Correctivas HSEQ
- [x] **Módulo HSEQ — sub-módulo Calidad** (ISO 9001):
  - [x] `HSEQDashboard.razor` — Hub central HSEQ con 4 tarjetas de división (Calidad, Seguridad, Ambiental, Social) y KPIs globales
  - [x] `CalidadDashboard.razor` — dashboard con ExecutiveCard KPIs y sub-navegación
  - [x] `ChecklistISO9001.razor` — auditoría de cumplimiento ISO 9001
  - [x] `PPIs.razor` — Puntos de Parada e Inspección
  - [x] `ControlCalibracion.razor` — calibración de equipos de medición
  - [x] `ControlDocumental.razor` — documentos de calidad
  - [x] `NoConformidadesHSEQ.razor` — no conformidades de calidad
  - [x] `AccionesCorrectivasHSEQ.razor` — acciones correctivas de calidad
- [x] **Módulo HSEQ — sub-módulo Ambiental** (ISO 14001):
  - [x] `AmbientalDashboard.razor` — dashboard con GaugeKPI y DashboardLayout
  - [x] `ChecklistISO14001.razor` — auditoría de cumplimiento ISO 14001
  - [x] `InspeccionesAmbientales.razor` — inspecciones de campo ambiental
  - [x] `GestionResiduos.razor` — registro y control de residuos
  - [x] `AspectosImpactos.razor` — matriz de aspectos e impactos ambientales
  - [x] `Derrames.razor` — registro de derrames y contingencias
  - [x] `FaunaFlora.razor` — monitoreo de biodiversidad
  - [x] `AccionesAmbientales.razor` — acciones correctivas ambientales
- [x] **Módulo HSEQ — sub-módulo Social**:
  - [x] `SocialDashboard.razor` — dashboard de gestión social del proyecto
  - [x] `Comunidades.razor` — registro de comunidades de área de influencia
  - [x] `ReunionesComunitarias.razor` — actas de reuniones con comunidades
  - [x] `PQRModule.razor` — Peticiones, Quejas y Reclamos
  - [x] `CompromisosSociales.razor` — seguimiento de compromisos sociales
  - [x] `ContratacionLocal.razor` — registro de mano de obra local
  - [x] `ComprasLocales.razor` — registro de compras a proveedores locales
  - [x] `ActasEvidencias.razor` — actas y evidencias de gestión social
- [x] **Sistema de Diseño RenergeIA** — identidad visual corporativa completa:
  - [x] CSS corporativo en `app.css`: variables, animación gauge (`rn-gauge-in`), badges semánticos, sección, chart card, panel IA, tabla
  - [x] `GaugeCircular.razor` — gauge SVG parametrizado con animación de entrada (v1)
  - [x] `GaugeKPI.razor` — gauge SVG v2 con soporte de tendencia (flecha arriba/abajo)
  - [x] `TarjetaKPI.razor` — KPI ejecutivo con icono y valor grande (v1)
  - [x] `ExecutiveCard.razor` — tarjeta ejecutiva v2 con icono, valor, tendencia y StatusChip
  - [x] `DonutKPI.razor` — donut CSS puro con valor central y badge
  - [x] `SeccionDash.razor` — encabezado de sección con decoración corporativa
  - [x] `ChartCard.razor` — tarjeta contenedora de gráficos con slot de filtros
  - [x] `AnalisisIA.razor` — panel de análisis con gradiente azul y slots de insights (v1)
  - [x] `AIPanel.razor` — panel IA v2 con slots independientes: Hallazgos, Alertas, Recomendaciones
  - [x] `DashboardLayout.razor` — contenedor principal `.rn-module-layout` para módulos
  - [x] `PageHeader.razor` — cabecera de página con botón volver, título, subtítulo y slot acciones
  - [x] `FilterBar.razor` — barra de navegación / filtros para sub-módulos
  - [x] `StatusChip.razor` — badge semántico de estado reutilizable (Saludable/En Riesgo/Riesgo/Crítico)
  - [x] `SmartTable.razor` — tabla con búsqueda integrada, estado vacío y pie configurables
  - [x] `_Imports.razor` actualizado con `@using RenergeIA.Web.Components.Shared.Dashboard`
  - [x] `SeguridadDashboard.razor` migrado a los nuevos componentes (eliminado RenderFragment local)
- [x] Control de versiones con GitHub (repositorio remoto configurado)
- [x] **Módulo de Costos ampliado**: rediseño en 5 pestañas (Presupuesto, Ejecutado, Compromisos, Dashboard, Comparativo); nueva entidad `CompromisoCosto` (órdenes de compra); `AdjuntoUrl` en `CostoReal` para evidencias digitales
- [x] **Módulo HSEQ — Motor de Auditoría Genérico (corporativo)**: `NormaChecklistService` centraliza auditorías de ISO 9001, ISO 14001, ISO 45001, Decreto 1072/2015 y Resolución 0312/2019, sin requerir un proyecto asociado; hub `/hseq/dashboard`, auditorías por norma, historial
- [x] **Módulo HSEQ Seguridad — Matriz de Riesgos IPERV**: metodología GTC 45 completa (ND/NE/NP/NC/NR, aceptabilidad I-V), biblioteca corporativa de peligros, dashboard SST, mapa de riesgos, acciones y evidencias
- [x] **Inspecciones con IA**: `IAInspeccionService` integra la API de Anthropic (Claude) para sugerir automáticamente peligros, clasificación GTC 45 y acciones correctivas a partir de una foto y descripción de campo
- [x] Checklist Resolución 0312/2019 (Estándares Mínimos SG-SST) en el sub-módulo Seguridad del proyecto

**Pendiente Fase 1:**
- [ ] Configurar `Anthropic:ApiKey` real en `appsettings.json` (actualmente placeholder; sin ella, el análisis con IA de la Matriz de Riesgos falla)
- [ ] Aplicar las migraciones `AddCamposISO9001`, `AddSeguridadIPERV` y `AddCompromisoCosto` pendientes (`dotnet ef database update`)
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

## 25. Sistema de Diseño RenergeIA

Implementado en esta sesión. Establece una identidad visual corporativa consistente y reutilizable en todos los módulos.

### Identidad visual

| Elemento | Valor |
|---------|-------|
| Fuente | Montserrat (pesos 400 y 700, Google Fonts) |
| Azul corporativo | `#183963` |
| Verde éxito | `#6ABF4B` |
| Gris neutro | `#D9D9D6` |
| Fondo | Blanco |
| Semaforización | Verde=Saludable, Azul=Informativo, Amarillo=En Riesgo, Naranja=Riesgo, Rojo=Crítico |

**Regla de color:** los colores comunican un estado operativo, nunca se usan de forma decorativa.

### Clases CSS en `app.css`

| Clase | Propósito |
|-------|-----------|
| `.rn-gauge-card` | Contenedor del gauge circular con hover elevado |
| `.rn-gauge-arc` | Arco SVG con animación de entrada `rn-gauge-in` (1.2 s) |
| `.rn-gauge-center` | Overlay absoluto con valor y subtexto |
| `.rn-kpi-card` | Tarjeta KPI ejecutiva (número grande + icono) |
| `.rn-badge-saludable/en-riesgo/riesgo/critico/informativo` | Badges semánticos pill |
| `.rn-section-header` | Encabezado de sección con borde azul inferior |
| `.rn-chart-card` | Tarjeta de gráfico con sombra suave |
| `.rn-ia-card` | Panel IA con gradiente azul `#183963 → #1a4e8f` |
| `.rn-ia-item` | Ítem de insight dentro del panel IA |
| `.rn-table` | Tabla con header azul corporativo y hover suave |

### Componentes Blazor (`Components/Shared/Dashboard/`)

#### `GaugeCircular.razor`

```razor
<GaugeCircular
    Titulo="Cumplimiento"
    ValorCentral="83%"
    SubTexto="18 de 22"
    Porcentaje="83"
    Estado="Saludable"
    Color="#6ABF4B"   <!-- opcional: sobrescribe color derivado de Estado -->
    SizePx="118"      <!-- opcional: tamaño del SVG en px -->
/>
```

El color se deriva automáticamente de `Estado` (Saludable=verde, En Riesgo=amarillo, Riesgo=naranja, Crítico=rojo, Informativo=azul). El parámetro `Color` lo sobrescribe cuando el color no sigue la semaforización estándar (p. ej., indicadores de conteo que son "Informativos" pero deben aparecer en gris cuando no hay datos).

La animación SVG funciona con `@keyframes rn-gauge-in { from { stroke-dasharray: 0 100; } }`. El punto "to" es el valor inline del elemento, por lo que CSS interpola automáticamente de 0 al valor real.

#### `TarjetaKPI.razor`

```razor
<TarjetaKPI Titulo="Horas Hombre" Valor="256h" Icono="bi-clock-fill"
            SubTexto="este mes" Estado="Informativo" />
```

#### `SeccionDash.razor`

```razor
<SeccionDash Titulo="Resumen Ejecutivo" Icono="bi-speedometer2">
    <!-- contenido del panel -->
</SeccionDash>
```

Renderiza un encabezado uppercase en gris con borde inferior azul de 2 px, seguido del `ChildContent`.

#### `ChartCard.razor`

```razor
<ChartCard Titulo="Programadas vs Ejecutadas" CanvasId="chartCapMes" Altura="230">
    <Filtros>
        <!-- botones de filtro opcionales sobre el gráfico -->
    </Filtros>
</ChartCard>
```

Crea el `<canvas id="...">` dentro de un `<div style="position:relative;height:Npx">`. Esta estructura es obligatoria para que `Chart.js` respete el alto con `maintainAspectRatio: false`.

#### `AnalisisIA.razor`

```razor
<AnalisisIA Titulo="Análisis Inteligente" Subtitulo="Generado por RenergeIA">
    <div class="rn-ia-item">
        <i class="bi-check-circle-fill" style="color:#6ABF4B"></i>
        <span>Texto del insight.</span>
    </div>
</AnalisisIA>
```

#### `GaugeKPI.razor` (v2)

Versión mejorada de `GaugeCircular` con soporte de tendencia:

```razor
<GaugeKPI Titulo="Cumplimiento ISO 14001"
          ValorCentral="93%"
          SubTexto="de requisitos"
          Porcentaje="93"
          Estado="Saludable"
          Tendencia="+5% vs mes anterior"
          TendenciaPositiva="true" />
```

#### `ExecutiveCard.razor` (v2)

Reemplaza a `TarjetaKPI` con soporte de tendencia y `StatusChip` integrado:

```razor
<ExecutiveCard Titulo="PPIs Ejecutados"
               Valor="16"
               Icono="bi-clipboard-check"
               SubTexto="de 23 planeados"
               Estado="En Riesgo"
               Tendencia="-2 esta semana"
               TendenciaPositiva="false" />
```

#### `DonutKPI.razor`

Donut implementado en CSS puro (sin SVG ni JS), con valor central:

```razor
<DonutKPI Titulo="Residuos Gestionados"
          ValorCentral="4.2t"
          SubTextoInterno="este mes"
          Porcentaje="72"
          Estado="Saludable" />
```

#### `DashboardLayout.razor`

Contenedor principal que aplica `.rn-module-layout`. Debe envolver todo el contenido de un módulo:

```razor
<DashboardLayout>
    <PageHeader ... />
    <FilterBar>...</FilterBar>
    <SeccionDash Titulo="KPIs">...</SeccionDash>
</DashboardLayout>
```

#### `PageHeader.razor`

Cabecera estándar de página con botón volver, título e icono, subtítulo y slot de acciones:

```razor
<PageHeader Titulo="Ambiental — ISO 14001"
            Icono="bi-tree-fill"
            Subtitulo="@(_proyecto.Nombre + " · " + DateTime.Now.ToString("dd/MM/yyyy"))"
            UrlVolver="@($"/proyectos/{ProyectoId}/hseq")"
            TextoVolver="HSEQ">
    <Acciones>
        <StatusChip Estado="Saludable" Grande="true" />
    </Acciones>
</PageHeader>
```

#### `FilterBar.razor`

Barra de navegación entre sub-módulos dentro de un módulo HSEQ:

```razor
<FilterBar>
    <span class="btn btn-success btn-sm">Dashboard</span>
    <a href="/...checklist" class="btn btn-outline-secondary btn-sm">Checklist ISO 14001</a>
    <a href="/...residuos"  class="btn btn-outline-secondary btn-sm">Residuos</a>
</FilterBar>
```

#### `StatusChip.razor`

Badge semántico reutilizable, acepta `Grande="true"` para versión más visible:

```razor
<StatusChip Estado="Saludable" />          <!-- badge pequeño -->
<StatusChip Estado="Crítico" Grande="true" />  <!-- badge grande en PageHeader -->
<StatusChip Estado="Informativo" Texto="Simulado" />  <!-- texto personalizado -->
```

Estados soportados: `"Saludable"`, `"En Riesgo"`, `"Riesgo"`, `"Crítico"`, `"Sin datos"`, `"Informativo"` (default).

#### `SmartTable.razor`

Tabla corporativa con búsqueda integrada, estado vacío y pie opcionales:

```razor
<SmartTable MostrarBusqueda="true"
            PlaceholderBusqueda="Buscar por comunidad..."
            Columnas="5"
            Vacio="@(!_items.Any())"
            MensajeVacio="No hay comunidades registradas."
            BusquedaCambiada="@(t => { _busqueda = t; StateHasChanged(); })">
    <Encabezados>
        <tr><th>Nombre</th><th>Municipio</th>...</tr>
    </Encabezados>
    <Filas>
        @foreach (var item in _itemsFiltrados)
        {
            <tr><td>@item.Nombre</td>...</tr>
        }
    </Filas>
</SmartTable>
```

#### `AIPanel.razor` (v2)

Panel IA con slots independientes para máxima flexibilidad:

```razor
<AIPanel Titulo="Análisis Ambiental" Subtitulo="Generado por RenergeIA" EsSimulado="true"
         EstadoGeneral="Cumplimiento ambiental en zona saludable.">
    <Hallazgos>
        <div class="rn-ia-item"><i class="bi-exclamation-triangle-fill" style="color:#ffc107"></i>
            <span>Residuos peligrosos superan límite del trimestre anterior.</span></div>
    </Hallazgos>
    <Alertas>
        <div class="rn-ia-item"><i class="bi-x-circle-fill" style="color:#dc3545"></i>
            <span>2 inspecciones programadas sin ejecutar este mes.</span></div>
    </Alertas>
    <Recomendaciones>
        <div class="rn-ia-item"><i class="bi-check-circle-fill" style="color:#6ABF4B"></i>
            <span>Priorizar disposición de aceites usados antes del cierre del período.</span></div>
    </Recomendaciones>
</AIPanel>
```

### Cómo agregar el namespace a una nueva página

El namespace `RenergeIA.Web.Components.Shared.Dashboard` ya está en `_Imports.razor`. Todos los componentes son accesibles en cualquier página Razor sin `@using` adicional.

### Patrón de datos para dashboards (4 niveles)

Todos los dashboards HSEQ siguen este patrón:

```
Nivel 1 — Filtros         : selects in-memory, sin nuevas consultas DB al filtrar
Nivel 2 — Gauges          : 8-12 GaugeCircular en grid responsive
Nivel 3 — Gráficos        : ChartCard con canvas → JS via IJSRuntime
Nivel 4 — Tabla           : búsqueda @bind:event="oninput" + tabs Planificadas/Ejecutadas
Nivel 5 — Análisis IA     : AnalisisIA con insights derivados de los datos cargados
```

**Control de renders de gráficos:**
```csharp
private bool _chartsReady;

// En ComputeMetrics() o CargarAsync():
_chartsReady = true;

// En OnAfterRenderAsync:
if (_chartsReady && _proyecto is not null)
{
    _chartsReady = false;
    await JS.InvokeVoidAsync("renderFuncion", "canvasId", ...datos...);
}
```

El flag `_chartsReady` evita que los gráficos se intenten renderizar antes de que el DOM esté listo y evita re-renders innecesarios.

---

## 26. Módulo HSEQ — Dashboards analíticos

### SeguridadDashboard.razor — actualizado

Ahora usa los componentes del Sistema de Diseño:

- `<SeccionDash>` reemplaza el encabezado `<h6>` manual
- `<GaugeCircular>` reemplaza el `RenderFragment GaugeCard(Gauge g)` local (50 líneas eliminadas)
- La lógica `Gauge` record y `Cpct`/`Epct` helpers se mantienen para calcular los datos; solo desapareció el HTML embebido en C#

**10 gauges:** Plan de Trabajo, Insp. Seguridad, OTS, STC, Hallazgos Abiertos, Acciones Vencidas, EPP/Dotación, Pausas Activas, Capacitaciones, ISO 45001.

### InspeccionesSST.razor — dashboard 4 niveles

Dashboard ejecutivo completo con:

**Nivel 2:** 12 GaugeCircular con datos cross-module (plan, inspecciones, acciones, EPP, pausas, capacitaciones, ISO)

**Nivel 3:** 8 gráficos Chart.js:
- `renderHallazgosPorArea` — barras horizontales coloreadas por altura relativa
- `renderActosVsCondiciones` — barras apiladas (Actos Inseguros vs Condiciones Inseguras)
- `renderEstadoHallazgos` — donut con plugin inline `afterDraw` para texto central
- `renderTendenciaInsp` — líneas (total inspecciones + hallazgos mes a mes)
- `renderInspPorMes` — barras verticales, último mes destacado en verde
- `renderParetoHallazgos` — mixto barra + línea, doble eje Y (conteo + % acumulado)
- `renderRankingInsp` — barras horizontales con filtro dinámico (Área / Tipo / Inspector)
- Heatmap CSS — tabla Área × {Seguridad, OTS, STC} con colores por cuartil

**Nivel 3 — Filtro de Ranking:**
```csharp
private async Task CambiarRankingAsync(string filtro)
{
    _rankingFiltro = filtro;
    ActualizarRanking(); // recomputa _labelsRanking y _valoresRanking
    await JS.InvokeVoidAsync("renderRankingInsp", "chartRanking", _labelsRanking, _valoresRanking, RankingTitulo);
}
```

**Nivel 4:** Tabla con 5 filtros + búsqueda + ordenamiento + exportación Excel (ClosedXML)

**Nivel 5:** AnalisisIA con insights sobre inspecciones, hallazgos, vencidas, temas críticos

### Capacitaciones.razor — dashboard 5 niveles (nuevo)

Transformación completa del módulo. Antes: 4 tarjetones simples + 2 tablas.
Ahora: dashboard ejecutivo completo.

**Datos:** `CapacitacionesPlanificadas` + `Capacitaciones`. Todos los cálculos son en memoria después de la carga inicial.

**Nivel 1 — Filtros:**
```
Área | Tema | Mes | Año | Estado
```
Los filtros aplican `ComputeMetrics()` sin relanzar consultas a BD. El método `MatchEstadoPlan(p, hoy)` encapsula la lógica para el estado "Vencida" (fecha < hoy AND no ejecutada/cancelada).

**Nivel 2 — Tarjeta Personas + 8 Gauges:**

Tarjeta "Personas Capacitadas" (gradiente azul, diseño especial):
- Total asistentes acumulados (`Sum(NumeroAsistentes)`)
- Capacitaciones este mes, promedio asistentes/sesión, áreas cubiertas
- HH acumuladas

8 GaugeCircular: Cumplimiento %, Planificadas, Ejecutadas, HH Capacitadas, Vencidas, Pendientes, Asistentes, Este Mes

**Nivel 3 — 4 gráficos Chart.js + timeline de vencidas:**

| Función JS | Tipo | Descripción |
|-----------|------|-------------|
| `renderCapMes` | Barras + línea, doble eje Y | Planificadas (navy) vs Ejecutadas (verde) + % cumplimiento (amarillo) |
| `renderCapHH` | Barras verticales | HH por mes, mes pico destacado en verde |
| `renderCapArea` | Barras horizontales | Cumplimiento % por área, color semafórico |
| `renderCapTemas` | Barras horizontales | Conteo por tema, opacidad proporcional al volumen |

Timeline de vencidas: visible solo si `_vencidas.Any()`. Tabla compacta con días de atraso.

**Fórmulas de negocio:**
```
Cumplimiento %   = Ejecutadas / Planificadas × 100
Horas Hombre     = Σ (DuracionHoras × NumeroAsistentes) por capacitación
Vencidas         = Planificadas con FechaPlanificada < hoy AND estado ≠ Ejecutada/Cancelada
Este Mes %       = Ejecutadas en mes/año actual / Planificadas en mes/año actual × 100
```

**Nivel 4 — Tabla inteligente:**
- Búsqueda en tiempo real (`@bind:event="oninput"`) sobre nombre, tema, área, instructor
- Tabs: Planificadas (con indicador de vencidas `bi-exclamation-circle-fill`) / Ejecutadas
- Clase `rn-table` para estilos corporativos

**Nivel 5 — AnalisisIA:**
Insights automáticos desde datos reales: cumplimiento global, HH, área mejor/peor, capacitaciones vencidas, temas frecuentes, alerta crítica si `_cumPct < 70`.

---

## 27. Módulo HSEQ — Calidad ISO 9001

**Ruta:** `/proyectos/{id}/hseq/calidad` → `CalidadDashboard.razor`

### Sub-módulos

| Ruta | Archivo | Propósito |
|------|---------|-----------|
| `/hseq/calidad` | `CalidadDashboard.razor` | Dashboard con KPIs ejecutivos y navegación |
| `/hseq/calidad/checklist` | `ChecklistISO9001.razor` | Auditoría de requisitos ISO 9001 |
| `/hseq/calidad/ppis` | `PPIs.razor` | Puntos de Parada e Inspección del contrato |
| `/hseq/calidad/calibracion` | `ControlCalibracion.razor` | Registro y vencimiento de calibración de equipos |
| `/hseq/calidad/documentos` | `ControlDocumental.razor` | Documentos de calidad del proyecto |
| `/hseq/calidad/no-conformidades` | `NoConformidadesHSEQ.razor` | No Conformidades de calidad |
| `/hseq/calidad/acciones` | `AccionesCorrectivasHSEQ.razor` | Acciones correctivas de calidad |

### Dashboard de Calidad

Usa `PageHeader` + `ExecutiveCard` para KPIs:
- Cumplimiento ISO 9001 (%)
- PPIs Ejecutados / Planeados
- No Conformidades Abiertas
- Acciones Correctivas Pendientes

---

## 28. Módulo HSEQ — Ambiental ISO 14001

**Ruta:** `/proyectos/{id}/hseq/ambiental` → `AmbientalDashboard.razor`

### Sub-módulos

| Ruta | Archivo | Propósito |
|------|---------|-----------|
| `/hseq/ambiental` | `AmbientalDashboard.razor` | Dashboard con GaugeKPI y navegación |
| `/hseq/ambiental/checklist` | `ChecklistISO14001.razor` | Auditoría de requisitos ISO 14001 |
| `/hseq/ambiental/inspecciones` | `InspeccionesAmbientales.razor` | Inspecciones ambientales de campo |
| `/hseq/ambiental/residuos` | `GestionResiduos.razor` | Registro de residuos sólidos, líquidos y peligrosos |
| `/hseq/ambiental/aspectos` | `AspectosImpactos.razor` | Matriz de aspectos e impactos ambientales |
| `/hseq/ambiental/derrames` | `Derrames.razor` | Registro de derrames y contingencias |
| `/hseq/ambiental/fauna-flora` | `FaunaFlora.razor` | Monitoreo de biodiversidad del entorno |
| `/hseq/ambiental/acciones` | `AccionesAmbientales.razor` | Acciones correctivas ambientales |

### Dashboard Ambiental

Usa `DashboardLayout` + `PageHeader` + `FilterBar` (navegación) + `GaugeKPI` para KPIs:
- Cumplimiento ISO 14001 (%)
- Inspecciones Realizadas
- Derrames del período
- Residuos gestionados (ton)

La arquitectura sigue el **patrón de 5 niveles** del Sistema de Diseño (ver sección 25).

---

## 29. Módulo HSEQ — Social

**Ruta:** `/proyectos/{id}/hseq/social` → `SocialDashboard.razor`

### Sub-módulos

| Ruta | Archivo | Propósito |
|------|---------|-----------|
| `/hseq/social` | `SocialDashboard.razor` | Dashboard de gestión social |
| `/hseq/social/comunidades` | `Comunidades.razor` | Registro de comunidades del área de influencia |
| `/hseq/social/reuniones` | `ReunionesComunitarias.razor` | Actas de reuniones con comunidades |
| `/hseq/social/pqr` | `PQRModule.razor` | Peticiones, Quejas y Reclamos |
| `/hseq/social/compromisos` | `CompromisosSociales.razor` | Seguimiento de compromisos sociales del contrato |
| `/hseq/social/contratacion-local` | `ContratacionLocal.razor` | Registro de mano de obra local contratada |
| `/hseq/social/compras-locales` | `ComprasLocales.razor` | Compras a proveedores locales |
| `/hseq/social/actas` | `ActasEvidencias.razor` | Actas y evidencias de gestión social |

### Dashboard Social

Usa `PageHeader` + `ExecutiveCard` para KPIs:
- Cumplimiento Social (%) — puede estar en estado Crítico si hay compromisos vencidos
- Reuniones Comunitarias realizadas
- PQR Abiertas
- Compromisos cumplidos / total

---

## 30. Módulo HSEQ — Motor de Auditoría Genérico (Corporativo)

A diferencia de los checklists por norma implementados en las secciones 27-29 (que viven **dentro de un proyecto**: `/proyectos/{id}/hseq/...`), este módulo es **corporativo**: las auditorías no requieren estar asociadas a un proyecto específico. Sirve para auditorías internas de la compañía, auditorías de cliente o de interventoría que evalúan procesos transversales (Compras, Producción, etc.).

**Ruta base:** `/hseq/...` (fuera del árbol `/proyectos/{id}/...`)

### Propósito

Antes existía un servicio (`ChecklistISO9001Service`) y una página por norma. Ahora `NormaChecklistService` centraliza la lógica para **cualquier norma soportada**, evitando duplicar código por cada nueva regulación que se agregue:

- **ISO 9001** — Gestión de Calidad
- **ISO 14001** — Gestión Ambiental
- **ISO 45001** — Gestión de Seguridad y Salud en el Trabajo
- **Decreto 1072/2015** — Sistema de Gestión de SST (normativa colombiana)
- **Resolución 0312/2019** — Estándares Mínimos SG-SST (normativa colombiana)
- **Cliente** / **Interventoría** — auditorías externas sobre el proyecto

### Entidades (compartidas con los checklists de calidad/ambiental/seguridad)

`ChecklistAuditoria` e `ItemChecklist` (ya existentes desde la sección 27) se ampliaron para soportar el modelo genérico:

**`ChecklistAuditoria`** — campos nuevos:

```csharp
public TipoNormaHSEQ TipoNorma { get; set; }        // ISO9001, ISO14001, ISO45001, Decreto1072, Resolucion0312, Cliente, Interventoria
public TipoAuditoriaHSEQ TipoAuditoria { get; set; } // Interna, Cliente, Interventoria
public string? ProcesoArea { get; set; }             // Ej: "Compras", "Producción"
public EstadoAuditoria EstadoAuditoria { get; set; } // Borrador, EnProceso, Finalizada
public string? UsuarioId { get; set; }               // Auditor responsable
public int? ProyectoId { get; set; }                 // NULLABLE — permite auditorías sin proyecto (SetNull)
```

**`ItemChecklist`** — campos nuevos (uno por cada requisito de la norma auditada):

```csharp
public string? Clausula { get; set; }              // Ej: "4.1"
public string? TituloClausula { get; set; }
public string? NumeroRequisito { get; set; }
public string? DescripcionRequisito { get; set; }
public decimal? Puntaje { get; set; }               // decimal(5,2)
public string? EvidenciaUrl { get; set; }            // máx. 500 caracteres
public string? Hallazgo { get; set; }
public string? OportunidadMejora { get; set; }       // Renombrado desde "Evidencia"
public string? Responsable { get; set; }
public DateTime? Plazo { get; set; }
public EstadoSeguimiento Seguimiento { get; set; }   // Pendiente, EnProceso, Ejecutado
public EstadoCumplimiento Estado { get; set; }        // Cumple, NoCumple, Parcial, NoAplica, EnProceso, SinEvaluar
```

### Enums nuevos

| Enum | Valores |
|------|---------|
| `TipoNormaHSEQ` | ISO9001, ISO14001, ISO45001, Decreto1072, Resolucion0312, Cliente, Interventoria |
| `EstadoAuditoria` | Borrador, EnProceso, Finalizada |
| `EstadoSeguimiento` | Pendiente, EnProceso, Ejecutado |
| `EstadoCumplimiento` (ampliado) | Cumple, NoCumple, Parcial, NoAplica, **EnProceso**, **SinEvaluar** |

### Servicio central: `NormaChecklistService`

| Método | Función |
|--------|---------|
| `ObtenerRequisitos(TipoNormaHSEQ)` | Retorna el catálogo de requisitos de la norma (delega en las clases `*ChecklistData` de solo lectura) |
| `GenerarItems(TipoNormaHSEQ)` | Crea los `ItemChecklist` completos listos para llenar, a partir del catálogo |
| `CalcularPorcentaje(IEnumerable<ItemChecklist>)` | % de cumplimiento: Cumple=100%, Parcial=50%, EnProceso=25%, resto=0% |
| `GuardarEvidenciaAsync(...)` | Sube evidencias a `/uploads/auditorias/{auditoriaId}/` |
| `GenerarCsv(...)` | Exporta la auditoría completa a CSV |
| `Meta` (diccionario estático) | Mapea `TipoNormaHSEQ` → (Título, ícono Bootstrap, color hex, versión del estándar) — usado para pintar la UI dinámicamente según la norma |

`ChecklistISO9001Service` (el servicio anterior, específico de ISO 9001) se mantiene por compatibilidad, pero `NormaChecklistService` es el punto de entrada para páginas nuevas.

### Catálogos de requisitos (`Web/Services/*ChecklistData.cs`)

Clases estáticas de solo lectura, cada una con un `record` de requisito (`NumClausula`/`NumGrupo`, `Titulo`, `SubClausula`, `Id`, `Req` (descripción), `Interp` (cómo evidenciarlo), `Docs` (documentos sugeridos)):

| Archivo | Norma | Requisitos aprox. |
|---------|-------|-------------------|
| `Iso9001ChecklistData.cs` | ISO 9001 (cláusulas 4-10) | 52+ |
| `Iso14001ChecklistData.cs` | ISO 14001 (cláusulas 4-10, enfoque ambiental) | ~50 |
| `Iso45001ChecklistData.cs` | ISO 45001 (cláusulas 4-10, enfoque SST) | ~50 |
| `Decreto1072ChecklistData.cs` | Decreto 1072/2015 SG-SST | ~126 |
| `Resolucion0312ChecklistData.cs` | Resolución 0312/2019 Estándares Mínimos | ~126 |

Todos cubren el mismo ciclo PHVA: Planificación (política, evaluación inicial, objetivos) → Implementación (gestión de peligros/aspectos, capacitación, vigilancia) → Verificación (indicadores, auditoría interna, revisión por la dirección) → Mejora continua (investigación de incidentes/no conformidades, acciones correctivas).

### Rutas y páginas (`Components/Pages/HSEQ/Global/`)

| Ruta | Archivo | Función |
|------|---------|---------|
| `/hseq/dashboard` | `HseqGlobalDashboard.razor` | Resumen corporativo con indicadores agregados de todas las normas |
| `/hseq/auditorias/norma/{normaSlug}` | `AuditoriasNorma.razor` | CRUD de auditorías para la norma seleccionada (iso9001, iso14001, iso45001, decreto1072, resolucion0312, cliente, interventoria) |
| — (componente hijo, no tiene ruta propia) | `MotorAuditoria.razor` | Componente reutilizable que renderiza el formulario de un ítem de checklist (cumplimiento, evidencia, hallazgo, oportunidad, responsable, plazo, seguimiento) — usado por `AuditoriasNorma.razor` |
| `/hseq/auditorias/historial` | `HistorialAuditorias.razor` | Historial de auditorías ejecutadas, con filtros y exportación |

### Navegación

`NavMenu.razor` agrega una sección de nivel superior **"HSEQ"** (corporativa, fuera del árbol de proyectos) con submenú expandible: Dashboard HSEQ y Auditorías (con una entrada por norma + Historial). El componente detecta la ruta activa (`EsRutaHseqGlobal`, `HseqSubActivo(...)`) y auto-expande el submenú correspondiente al navegar.

---

## 31. Módulo HSEQ Seguridad — Matriz de Riesgos IPERV e Inspecciones con IA

**Ruta:** `/proyectos/{id}/hseq/seguridad/matriz` → `MatrizRiesgosIndex.razor` (dentro del proyecto, a diferencia de la sección 30)

### Propósito

Implementa la metodología **GTC 45** (Guía Técnica Colombiana del ICONTEC) de identificación de peligros, evaluación y valoración de riesgos (IPERV — Identificación de Peligros, Evaluación y Valoración de Riesgos), con un diferencial: permite generar el análisis de riesgo **automáticamente con IA** a partir de una foto y una breve descripción de la inspección, usando la API de Anthropic (Claude).

Flujo completo:

```
1. Biblioteca de Peligros (catálogo corporativo, ~30 peligros típicos de proyectos FV)
        ↓ (referencia)
2. Nueva Inspección IA: el inspector sube foto + describe área/actividad/tarea
        ↓ (IAInspeccionService llama a la API de Anthropic)
3. Resultado IA: peligros identificados, clasificación GTC45, ND/NE/NC/NR, aceptabilidad, medidas sugeridas
        ↓ (el inspector revisa y aprueba)
4. Riesgo IPERV: queda registrado en la Matriz de Riesgos del proyecto, con trazabilidad a la inspección de origen
        ↓
5. Seguimiento: acciones correctivas, evidencias, mapa de riesgos (NP vs NC)
```

### Entidades

#### `BibliotecaPeligro` — catálogo corporativo (sin FK, es tabla de referencia)

```csharp
public class BibliotecaPeligro : EntidadBase
{
    public string Area { get; set; }
    public string Actividad { get; set; }
    public string Tarea { get; set; }
    public string Peligro { get; set; }
    public string Clasificacion { get; set; }        // GTC45: Locativo, Mecánico, Eléctrico, Químico, Biológico, Biomecánico, Psicosocial, Físico, Fenómenos naturales, Público
    public string EfectosPosibles { get; set; }
    public string? ControlFuente { get; set; }
    public string? ControlMedio { get; set; }
    public string? ControlIndividuo { get; set; }
    public string? MedidasIntervencion { get; set; }
    public string? EPPRecomendado { get; set; }
    public string? DocumentosAsociados { get; set; }
    public string? PermisosRequeridos { get; set; }
    public string? NivelRiesgoSugerido { get; set; }
    public bool Activo { get; set; } = true;
}
```

Poblada desde `BibliotecaPeligrosData.cs` — catálogo estático con ~30 peligros típicos de un proyecto EPC fotovoltaico, organizados por área: Obras Civiles, Estructuras, Módulos FV, Eléctricas AC, Salas Eléctricas, Subestación, Izajes, Trabajo en Alturas, Commissioning y O&M. Ejemplos: derrumbe en excavación (Locativo, NR sugerido 4000), atmósfera deficiente en espacio confinado (Químico, NR 6000), contacto eléctrico en AC (Eléctrico, NR 6000).

#### `InspeccionIA` — registro de la inspección de campo

```csharp
public class InspeccionIA : EntidadBase
{
    public int ProyectoId { get; set; }
    public string Area { get; set; }
    public string Actividad { get; set; }
    public string Tarea { get; set; }
    public string? Responsable { get; set; }
    public string? Ubicacion { get; set; }
    public string? Inspector { get; set; }
    public DateTime FechaInspeccion { get; set; }
    public string? ObservacionManual { get; set; }     // Notas del inspector
    public string? EvidenciaUrl { get; set; }           // Foto/video subido
    public EstadoValidacionIA EstadoValidacion { get; set; }
    public string? ValidadoPor { get; set; }
    public DateTime? FechaValidacion { get; set; }
    public string? ObservacionValidacion { get; set; }

    // Cache del resultado de IA (para no volver a llamar la API al recargar la página)
    public string? ResultadoIA { get; set; }            // JSON serializado de AnalisisIAResult
    public string? PeligrosIdentificados { get; set; }
    public string? NivelRiesgoSugerido { get; set; }
    public string? HallazgoRedactado { get; set; }
    public string? AccionCorrectivaSugerida { get; set; }

    public ICollection<RiesgoIPERV> RiesgosGenerados { get; set; } = [];
}
```

FK a `Proyecto` con `DeleteBehavior.Cascade`. Relación 1:N con `RiesgoIPERV` (una inspección puede generar varios riesgos aprobados).

#### `RiesgoIPERV` — el riesgo valorado según GTC 45

```csharp
public class RiesgoIPERV : EntidadBase
{
    public int ProyectoId { get; set; }
    public FuenteRiesgo FuenteOrigen { get; set; }      // InspeccionIA, InspeccionManual, Incidente, PermisoTrabajo, Capacitacion, AccionCorrectiva, Manual
    public int? InspeccionIAId { get; set; }             // Referencia a la inspección de origen (si aplica)

    // Identificación del peligro
    public string Area { get; set; }
    public string Actividad { get; set; }
    public string Tarea { get; set; }
    public bool EsRutinaria { get; set; }
    public string DescripcionPeligro { get; set; }
    public string ClasificacionPeligro { get; set; }
    public string EfectosPosibles { get; set; }

    // Controles existentes (jerarquía Fuente-Medio-Individuo)
    public string? ControlFuente { get; set; }
    public string? ControlMedio { get; set; }
    public string? ControlIndividuo { get; set; }

    // Valoración GTC 45
    public int ND { get; set; }              // Nivel Deficiencia: 10=MA, 6=A, 2=M, 0=B
    public int NE { get; set; }              // Nivel Exposición: 4=EC, 3=EF, 2=EO, 1=EEsp
    public int NP => ND * NE;                // Nivel Probabilidad (calculado)
    public int NC { get; set; }              // Nivel Consecuencia: 100=M, 60=MG, 25=G, 10=L
    public int NR => NP * NC;                // Nivel Riesgo (calculado)
    public string Aceptabilidad { get; set; } // I-V, derivado de NR

    // Medidas de intervención (jerarquía de controles)
    public string? Eliminacion { get; set; }
    public string? Sustitucion { get; set; }
    public string? ControlIngenieria { get; set; }
    public string? ControlAdministrativo { get; set; }
    public string? Senalizacion { get; set; }
    public string? EPP { get; set; }

    // Gestión
    public string? Responsable { get; set; }
    public DateTime? Plazo { get; set; }
    public string? EvidenciaUrl { get; set; }
    public string? Hallazgo { get; set; }
    public string? AccionCorrectiva { get; set; }

    public EstadoRiesgo Estado { get; set; }             // Activo, EnControl, Controlado, Eliminado
    public EstadoValidacionIA EstadoValidacion { get; set; }
}
```

FK a `Proyecto` (`Cascade`) y FK opcional a `InspeccionIA` (sin cascada, para no perder el riesgo si se elimina la inspección origen).

### Escala GTC 45 (valoración del riesgo)

| Variable | Significado | Valores |
|----------|-------------|---------|
| **ND** | Nivel de Deficiencia | 10=Muy Alto (MA), 6=Alto (A), 2=Medio (M), 0=Bajo (B) |
| **NE** | Nivel de Exposición | 4=Continua (EC), 3=Frecuente (EF), 2=Ocasional (EO), 1=Esporádica (EEsp) |
| **NP** | Nivel de Probabilidad | `ND × NE` |
| **NC** | Nivel de Consecuencia | 100=Mortal (M), 60=Muy Grave (MG), 25=Grave (G), 10=Leve (L) |
| **NR** | Nivel de Riesgo | `NP × NC` |

**Aceptabilidad** (derivada de NR):

| Nivel | Rango NR | Interpretación |
|-------|----------|----------------|
| I | ≥ 4000 | No Aceptable |
| II | 2000-3999 | No Aceptable |
| III | 1000-1999 | Mejorable |
| IV | 200-999 | Aceptable con control |
| V | < 200 | Aceptable |

### Enums nuevos

| Enum | Valores |
|------|---------|
| `EstadoRiesgo` | Activo, EnControl, Controlado, Eliminado |
| `EstadoValidacionIA` | PendienteValidacion, EnRevision, Aprobado, Rechazado |
| `FuenteRiesgo` | InspeccionIA, InspeccionManual, Incidente, PermisoTrabajo, Capacitacion, AccionCorrectiva, Manual |

### Servicio: `IAInspeccionService`

Integra la app con la **API de Anthropic (Claude)** para automatizar la valoración GTC 45 de una inspección:

| Método | Función |
|--------|---------|
| `AnalizarAsync(evidenciaUrl?, area, actividad, tarea, observacion?)` | Arma un prompt con el contexto GTC 45 (escalas ND/NE/NC, jerarquía de controles), adjunta la foto en base64 si existe, y hace `POST` a `https://api.anthropic.com/v1/messages` con la clave y el modelo configurados en `appsettings.json` (`Anthropic:ApiKey`, `Anthropic:Model`) |
| `ExtraerYParsearJSON(...)` | Busca el bloque JSON en la respuesta de texto de Claude (entre `{ }` o dentro de un bloque ` ```json `) |
| `LeerImagenBase64Async(...)` | Lee la foto local (jpg/png/gif/webp) y la convierte a base64 para enviarla en el mensaje |
| `SerializarResultado()` / `DeserializarResultado()` | Convierte el resultado a/desde JSON para guardarlo en caché en `InspeccionIA.ResultadoIA` |

`AnalisisIAResult` es el objeto que mapea la respuesta JSON de la IA: peligros identificados, clasificación, efectos posibles, controles existentes/faltantes, ND/NE/NC/NR (como texto, ej. `"A"`, `"EC"`, `"G"`), aceptabilidad, medidas de intervención, responsable y plazo sugeridos, hallazgo redactado, acción correctiva y EPP requerido — con propiedades calculadas para convertir los textos a los valores numéricos de la escala GTC 45 y derivar un color (`ColorNR`) para la UI.

Registrado en `Program.cs`:
```csharp
builder.Services.AddScoped<NormaChecklistService>();
builder.Services.AddScoped<IAInspeccionService>();
builder.Services.AddHttpClient(); // requerido por IAInspeccionService
```

> **Importante:** si `Anthropic:ApiKey` no está configurada, `AnalizarAsync` falla — el formulario de inspección debe seguir permitiendo captura 100% manual del riesgo (sin depender de la IA) para no bloquear el trabajo de campo.

### Rutas y páginas (`Components/Pages/HSEQ/Seguridad/MatrizRiesgos/`)

| Ruta (relativa a `/proyectos/{id}/hseq/seguridad/matriz`) | Archivo | Función |
|------|---------|---------|
| (raíz) | `MatrizRiesgosIndex.razor` | Contenedor/router del sub-módulo |
| — | `NavMatrizRiesgos.razor` | Barra de navegación: Dashboard, IPERV, Inspecciones, Biblioteca, Acciones, Evidencias, Mapa |
| `/dashboard` | `DashboardSST.razor` | KPIs de seguridad: riesgos activos, distribución por aceptabilidad, tendencias |
| `/iperv` | `MatrizIPERV.razor` | Tabla de riesgos identificados — filtrable por estado/NR, editable, con acciones de editar/eliminar/ver detalle |
| `/inspeccion-ia` | `NuevaInspeccionIA.razor` | Formulario de inspección + análisis IA: panel izquierdo con los datos de campo (área, actividad, tarea, responsable, inspector, fecha, observación, foto); panel derecho con el resultado de la IA en vivo (peligros, ND/NE/NC/NR, aceptabilidad, controles, medidas, hallazgo, acción) y un botón para generar/aprobar el riesgo IPERV a partir del análisis |
| `/biblioteca` | `BibliotecaPeligros.razor` | Consulta del catálogo de peligros genéricos, filtrable por área/actividad/tarea |
| `/acciones` | `AccionesMatriz.razor` | Gestión de acciones correctivas derivadas de riesgos activos |
| `/evidencias` | `EvidenciasMatriz.razor` | Galería/historial de evidencias fotográficas vinculadas a riesgos e inspecciones |
| `/mapa` | `MapaRiesgos.razor` | Mapa de riesgos: dispersión de NP (eje Y) vs NC (eje X), coloreado por nivel de aceptabilidad |

### Integración con otros módulos

- Las Inspecciones SST ya existentes (sección 26) pueden originar un `RiesgoIPERV` con `FuenteOrigen = InspeccionManual`.
- Un Incidente/Accidente puede originar un `RiesgoIPERV` con `FuenteOrigen = Incidente`, cerrando el ciclo entre "algo pasó" y "qué riesgo lo explica".

### Navegación

`NavSeguridad.razor` agrega la pestaña **"Matriz Riesgos"** → `/proyectos/{id}/hseq/seguridad/matriz`, junto a la nueva pestaña **"Res. 0312"** → `/proyectos/{id}/hseq/seguridad/resolucion0312` (`ChecklistResolucion0312.razor`, análogo a `ChecklistISO45001.razor` pero usando `Resolucion0312ChecklistData`).

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
| **GTC 45** | Guía Técnica Colombiana 45 del ICONTEC — metodología estándar para identificación de peligros, evaluación y valoración de riesgos laborales |
| **IPERV** | Identificación de Peligros, Evaluación y Valoración de Riesgos — matriz de riesgos de seguridad industrial basada en GTC 45 |
| **ND / NE / NP / NC / NR** | Variables de la valoración GTC 45: Nivel de Deficiencia, Nivel de Exposición, Nivel de Probabilidad (ND×NE), Nivel de Consecuencia, Nivel de Riesgo (NP×NC) |
| **Aceptabilidad (I-V)** | Clasificación del riesgo según su NR: I y II No Aceptable, III Mejorable, IV Aceptable con control, V Aceptable |
| **SG-SST** | Sistema de Gestión de Seguridad y Salud en el Trabajo — marco normativo colombiano (Decreto 1072/2015, Resolución 0312/2019) |
| **Decreto 1072/2015** | Decreto Único Reglamentario del Sector Trabajo en Colombia; define el SG-SST obligatorio para empresas |
| **Resolución 0312/2019** | Norma colombiana que define los Estándares Mínimos del SG-SST según el tamaño y riesgo de la empresa |
| **Anthropic / Claude** | Proveedor de modelos de lenguaje (LLM) usado por `IAInspeccionService` para analizar fotos/descripciones de inspecciones y sugerir la valoración GTC 45 de un riesgo |

---

*Guía actualizada el 1 de julio de 2026 — RenergeIA v1.0 en desarrollo activo. Motor de Auditoría HSEQ genérico (ISO 9001/14001/45001, Decreto 1072/2015, Resolución 0312/2019) a nivel corporativo. Matriz de Riesgos IPERV (GTC 45) con Inspecciones asistidas por IA (Anthropic/Claude). Módulo de Costos rediseñado en 5 pestañas con Compromisos de Gasto.*