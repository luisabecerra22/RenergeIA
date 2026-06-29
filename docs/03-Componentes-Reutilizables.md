# Componentes Reutilizables — RenergeIA
**Versión:** 1.0 | **Fecha:** 2026-06-28 | **Estado:** Vigente

> Este documento es el catálogo oficial de componentes de RenergeIA. Antes de escribir HTML nuevo, revisar si ya existe un componente que lo resuelva.

---

## Regla general

> **No duplicar componentes.**
> Si ya existe un componente que hace lo que necesitás, usarlo.
> Si el componente existente necesita una pequeña variación, agregar un parámetro opcional.
> Solo crear un componente nuevo si no existe nada equivalente.

Todos los componentes viven en:
```
RenergeIA.Web/Components/Shared/Dashboard/
```

Y están disponibles globalmente gracias a `_Imports.razor`:
```razor
@using RenergeIA.Web.Components.Shared.Dashboard
```

---

## Componentes EXISTENTES (ya implementados)

### 1. GaugeCircular

**Archivo:** `GaugeCircular.razor`
**Para qué sirve:** Mostrar un KPI de cumplimiento o avance en forma de círculo animado con semáforo de color.

**Parámetros:**

| Parámetro | Tipo | Default | Descripción |
|---|---|---|---|
| `Titulo` | `string` | `""` | Etiqueta debajo del gauge |
| `ValorCentral` | `string` | `"0"` | Texto grande en el centro (ej: `"87%"`) |
| `SubTexto` | `string` | `""` | Texto pequeño bajo el valor (ej: `"de 120 actividades"`) |
| `Porcentaje` | `int` | `0` | Número 0–100 que controla el arco |
| `Estado` | `string` | `"Informativo"` | Saludable / En Riesgo / Riesgo / Crítico / Informativo |
| `Color` | `string?` | `null` | Override de color (si no se pasa, lo toma del Estado) |
| `SizePx` | `int` | `118` | Tamaño del gauge en píxeles |

**Ejemplo de uso:**
```razor
<GaugeCircular
    Titulo="Avance Físico"
    ValorCentral="87%"
    SubTexto="de 120 actividades"
    Porcentaje="87"
    Estado="Saludable" />
```

---

### 2. TarjetaKPI

**Archivo:** `TarjetaKPI.razor`
**Para qué sirve:** Mostrar un indicador numérico ejecutivo con ícono, valor y badge de estado. Ideal para KPIs que no necesitan gauge pero sí comunicar estado.

**Parámetros:**

| Parámetro | Tipo | Default | Descripción |
|---|---|---|---|
| `Titulo` | `string` | `""` | Nombre del indicador |
| `Valor` | `string` | `"0"` | Valor principal (ej: `"$125M"`, `"48"`, `"100%"`) |
| `Icono` | `string` | `""` | Clase Bootstrap Icons (ej: `"bi-currency-dollar"`) |
| `SubTexto` | `string` | `""` | Texto explicativo secundario |
| `Estado` | `string` | `"Informativo"` | Saludable / En Riesgo / Riesgo / Crítico / Informativo |
| `Color` | `string?` | `null` | Override de color |

**Ejemplo de uso:**
```razor
<TarjetaKPI
    Titulo="Incidentes del Mes"
    Valor="2"
    Icono="bi-exclamation-triangle-fill"
    SubTexto="meta: 0"
    Estado="En Riesgo" />
```

---

### 3. ChartCard

**Archivo:** `ChartCard.razor`
**Para qué sirve:** Contenedor estándar para gráficos Chart.js. Provee el título, filtros opcionales y el canvas con altura controlada.

**Parámetros:**

| Parámetro | Tipo | Default | Descripción |
|---|---|---|---|
| `Titulo` | `string` | `""` | Título del gráfico |
| `CanvasId` | `string` | `""` | ID único del canvas (requerido para Chart.js) |
| `Altura` | `int` | `220` | Altura del canvas en píxeles |
| `Filtros` | `RenderFragment?` | `null` | Slot para controles de filtro |
| `ChildContent` | `RenderFragment?` | `null` | Contenido adicional bajo el canvas |

**Ejemplo de uso:**
```razor
<ChartCard Titulo="Avance Semanal" CanvasId="chart-avance" Altura="250">
    <Filtros>
        <select class="form-select form-select-sm" style="width:auto">
            <option>Últimas 4 semanas</option>
            <option>Último mes</option>
        </select>
    </Filtros>
</ChartCard>
```

---

### 4. SeccionDash

**Archivo:** `SeccionDash.razor`
**Para qué sirve:** Agrupa un conjunto de tarjetas o contenido bajo un encabezado de sección con ícono. Equivale a los niveles 1–5 de la estructura de dashboards.

**Parámetros:**

| Parámetro | Tipo | Default | Descripción |
|---|---|---|---|
| `Titulo` | `string` | `""` | Texto del encabezado de sección |
| `Icono` | `string` | `""` | Clase Bootstrap Icons para el encabezado |
| `ChildContent` | `RenderFragment?` | `null` | Contenido de la sección |

**Ejemplo de uso:**
```razor
<SeccionDash Titulo="Resumen Ejecutivo" Icono="bi-speedometer2">
    <div class="row g-3">
        <div class="col-xl-3 col-md-6">
            <GaugeCircular ... />
        </div>
    </div>
</SeccionDash>
```

---

### 5. AnalisisIA

**Archivo:** `AnalisisIA.razor`
**Para qué sirve:** Panel de Análisis Inteligente. Actualmente muestra contenido estático/simulado. En Fase 3 se conectará a OpenAI.

**Parámetros:**

| Parámetro | Tipo | Default | Descripción |
|---|---|---|---|
| `Titulo` | `string` | `"Análisis Inteligente"` | Título del panel |
| `Subtitulo` | `string` | `"Generado por RenergeIA"` | Subtítulo informativo |
| `ChildContent` | `RenderFragment?` | `null` | Contenido del análisis (listas, textos, alertas) |

**Ejemplo de uso:**
```razor
<AnalisisIA Titulo="Análisis Inteligente" Subtitulo="Módulo Seguridad">
    <ul class="mb-0">
        <li>La tasa de cumplimiento de capacitaciones superó el 90% por tercer mes consecutivo.</li>
        <li>Se detectaron 2 inspecciones vencidas en el área eléctrica.</li>
        <li>Recomendación: programar auditoría interna antes del 15 de julio.</li>
    </ul>
</AnalisisIA>
```

---

## Componentes PENDIENTES (a crear en Fase 2)

Los siguientes componentes están definidos en el Design System pero aún no han sido implementados. **No recrear funcionalidad equivalente en HTML inline**; esperar a que se construyan como componentes reutilizables.

### 6. PageHeader *(pendiente)*

**Para qué sirve:** Cabecera estándar de cada página. Muestra el título, ícono, breadcrumb y botones de acción principales.

**Parámetros planificados:**

| Parámetro | Tipo | Descripción |
|---|---|---|
| `Titulo` | `string` | Título principal de la página |
| `Icono` | `string` | Bootstrap Icon del módulo |
| `Subtitulo` | `string?` | Descripción breve o breadcrumb |
| `Acciones` | `RenderFragment?` | Botones (Nuevo, Exportar, etc.) |

**Uso esperado:**
```razor
<PageHeader Titulo="Capacitaciones" Icono="bi-mortarboard-fill" Subtitulo="HSEQ › Seguridad">
    <Acciones>
        <button class="btn btn-primary btn-sm">+ Nueva Capacitación</button>
    </Acciones>
</PageHeader>
```

---

### 7. FilterBar *(pendiente)*

**Para qué sirve:** Barra de filtros del nivel 1 de dashboards. Proyecto, fecha, estado, área.

**Parámetros planificados:**

| Parámetro | Tipo | Descripción |
|---|---|---|
| `ChildContent` | `RenderFragment` | Controles de filtro |
| `OnAplicar` | `EventCallback` | Callback al presionar "Aplicar" |
| `OnLimpiar` | `EventCallback` | Callback al presionar "Limpiar" |

---

### 8. StatusChip *(pendiente)*

**Para qué sirve:** Badge/chip reutilizable de estado semafórico. Versión inline de los badges `.rn-badge-*` para usar dentro de tablas, listas o textos.

**Parámetros planificados:**

| Parámetro | Tipo | Descripción |
|---|---|---|
| `Estado` | `string` | Saludable / En Riesgo / Riesgo / Crítico / Informativo |
| `Texto` | `string?` | Override del texto (si no se pasa, usa el Estado) |

**Uso esperado:**
```razor
<StatusChip Estado="Crítico" />
<StatusChip Estado="Saludable" Texto="Al día" />
```

---

### 9. AlertCard *(pendiente)*

**Para qué sirve:** Tarjeta de alerta compacta con ícono, prioridad, mensaje y acción.

**Parámetros planificados:**

| Parámetro | Tipo | Descripción |
|---|---|---|
| `Titulo` | `string` | Título de la alerta |
| `Mensaje` | `string` | Descripción breve |
| `Prioridad` | `string` | Alta / Media / Baja |
| `Fecha` | `string?` | Fecha de la alerta |
| `OnAccion` | `EventCallback?` | Botón de acción |

---

### 10. EmptyState *(pendiente)*

**Para qué sirve:** Pantalla estándar cuando no hay datos que mostrar. Reemplaza el patrón `@if (!lista.Any()) { <p>No hay datos</p> }` que existe en varios módulos.

**Parámetros planificados:**

| Parámetro | Tipo | Descripción |
|---|---|---|
| `Mensaje` | `string` | Texto principal ("No hay registros") |
| `Submensaje` | `string?` | Texto secundario de ayuda |
| `Icono` | `string` | Bootstrap Icon a mostrar |
| `Accion` | `RenderFragment?` | Botón de acción (ej: "Crear primero") |

**Uso esperado:**
```razor
@if (!_lista.Any())
{
    <EmptyState
        Mensaje="No hay inspecciones registradas"
        Submensaje="Crea la primera inspección para comenzar"
        Icono="bi-clipboard-x">
        <Accion>
            <button class="btn btn-primary">+ Nueva Inspección</button>
        </Accion>
    </EmptyState>
}
```

---

### 11. LoadingState *(pendiente)*

**Para qué sirve:** Indicador de carga estándar mientras se obtienen datos.

**Uso esperado:**
```razor
@if (_cargando)
{
    <LoadingState Mensaje="Cargando inspecciones..." />
}
```

---

### 12. ErrorState *(pendiente)*

**Para qué sirve:** Pantalla de error estándar cuando la carga de datos falla.

**Uso esperado:**
```razor
@if (_error)
{
    <ErrorState Mensaje="No se pudo cargar la información" OnReintentar="CargarDatos" />
}
```

---

### 13. SectionTitle *(pendiente)*

**Para qué sirve:** Versión más liviana que `SeccionDash` para separadores de sección dentro de una misma página.

---

### 14. SmartTable *(pendiente)*

**Para qué sirve:** Tabla reutilizable con columnas configurables, paginación básica y búsqueda. Reemplaza tablas repetidas en todos los módulos.

---

### 15. RankingCard *(pendiente)*

**Para qué sirve:** Tarjeta Top-N (Top 5 causas de NC, Top 10 actividades críticas, etc.) con barra de progreso relativa.

---

## Cómo agregar un nuevo componente

1. Crear el archivo `.razor` en `RenergeIA.Web/Components/Shared/Dashboard/`
2. Definir todos los `[Parameter]` con valores default
3. Agregar los estilos CSS en `app.css` bajo la sección `SISTEMA DE DISEÑO RENERGEIA` con prefijo `rn-`
4. Documentarlo en este archivo con su tabla de parámetros y ejemplo de uso
5. Verificar que esté listado en `_Imports.razor`

---

*Documento generado: 2026-06-28. Actualizar cada vez que se cree o modifique un componente.*
