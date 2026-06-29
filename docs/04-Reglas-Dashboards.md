# Reglas para Dashboards — RenergeIA
**Versión:** 1.0 | **Fecha:** 2026-06-28 | **Estado:** Vigente

> Este documento define la estructura obligatoria de todos los dashboards de RenergeIA. Un dashboard que no sigue esta estructura no es un dashboard de RenergeIA.

---

## Por qué existe esta regla

Sin un estándar, cada módulo termina con su propio criterio de organización. El resultado es que el gerente que revisa el dashboard de Seguridad tiene que aprender una nueva interfaz cuando entra al de Calidad, y otra diferente en Costos. Eso rompe la experiencia y baja la confianza en el sistema.

**La regla es simple:** todos los dashboards tienen el mismo esqueleto. El contenido cambia, la estructura no.

---

## Estructura obligatoria — 5 niveles

```
┌──────────────────────────────────────────────────┐
│  NIVEL 1 — Filtros                               │
│  Proyecto · Período · Estado · Área              │
├──────────────────────────────────────────────────┤
│  NIVEL 2 — Resumen ejecutivo (KPIs visuales)     │
│  Gauges · Donuts · Tarjetas KPI                  │
├──────────────────────────────────────────────────┤
│  NIVEL 3 — Gráficos analíticos                   │
│  Barras · Líneas · Pareto · Heatmap · Ranking    │
├──────────────────────────────────────────────────┤
│  NIVEL 4 — Tabla inteligente                     │
│  Detalle operativo con búsqueda y estado         │
├──────────────────────────────────────────────────┤
│  NIVEL 5 — Análisis Inteligente IA               │
│  Resumen · Hallazgos · Alertas · Recomendaciones │
└──────────────────────────────────────────────────┘
```

---

## Nivel 1 — Filtros

### Propósito

Permitir al usuario acotar el análisis sin navegar a otra pantalla. Los filtros afectan todo el contenido del dashboard.

### Filtros estándar

| Filtro | Tipo | Aplica a |
|---|---|---|
| Proyecto | Selector | Todos los dashboards |
| Período (mes/trimestre/año) | Selector de fechas | Dashboards con datos históricos |
| Estado (Saludable/En Riesgo/Crítico) | Selector | Dashboards de seguimiento |
| Área / Módulo | Selector | Dashboard HSEQ general |

### Reglas de los filtros

- Los filtros van en la parte superior de la página, antes de cualquier dato.
- Un botón **"Aplicar"** y uno **"Limpiar"** son obligatorios cuando los filtros no son en tiempo real.
- El estado de los filtros debe mantenerse al navegar entre pestañas del mismo módulo.
- Los filtros no deben recargarse con el tiempo (sin auto-refresh que resetee la selección del usuario).

### Componente a usar

Usar `<FilterBar />` cuando esté disponible (Fase 2). Por ahora, usar un `<div class="card p-3 mb-4">` con los controles de Bootstrap.

---

## Nivel 2 — Resumen ejecutivo (KPIs visuales)

### Propósito

El gerente debe poder leer el estado del módulo en menos de 10 segundos mirando solo los KPIs. Sin leer texto.

### Tipos de KPI permitidos

| Tipo | Componente | Cuándo |
|---|---|---|
| Cumplimiento / Avance (%) | `<GaugeCircular />` | El dato es un porcentaje de 0–100% |
| Contadores con estado | `<TarjetaKPI />` | El dato es un número absoluto (ej: 5 incidentes) |
| Distribución en partes | Donut chart pequeño | El dato divide un total en categorías |

### Cuántos KPIs por dashboard

| Nivel del dashboard | KPIs recomendados |
|---|---|
| Dashboard de módulo | 4–6 KPIs |
| Dashboard de área (HSEQ, Proyectos) | 6–8 KPIs |
| Dashboard General | 4–6 KPIs macro |

### Layout estándar de KPIs

```razor
<SeccionDash Titulo="Resumen Ejecutivo" Icono="bi-speedometer2">
    <div class="row g-3">
        <div class="col-xl-3 col-md-6">
            <GaugeCircular Titulo="..." Porcentaje="..." Estado="..." />
        </div>
        <div class="col-xl-3 col-md-6">
            <GaugeCircular Titulo="..." Porcentaje="..." Estado="..." />
        </div>
        <div class="col-xl-3 col-md-6">
            <TarjetaKPI Titulo="..." Valor="..." Estado="..." />
        </div>
        <div class="col-xl-3 col-md-6">
            <TarjetaKPI Titulo="..." Valor="..." Estado="..." />
        </div>
    </div>
</SeccionDash>
```

### Reglas de KPIs

1. **Todo KPI tiene un estado semafórico.** Un número sin color no es un KPI ejecutivo.
2. **El valor central debe ser autoelocuente.** `"87%"` sin contexto no dice nada. Usar `SubTexto` para contextualizar: `"de 120 actividades"`.
3. **No apilar más de 2 filas de KPIs.** Si se necesitan más de 8, usar pestañas o secciones.
4. **El color del KPI refleja el estado real del dato**, no la preferencia visual del desarrollador.

---

## Nivel 3 — Gráficos analíticos

### Propósito

Mostrar tendencias, comparaciones, distribuciones y patrones que los KPIs numéricos no pueden comunicar solos.

### Guía de selección de gráfico

| Pregunta | Tipo de gráfico | Implementación |
|---|---|---|
| ¿Cómo avanza X en el tiempo? | Línea (line) | `<ChartCard />` tipo `line` |
| ¿Cuánto hay de cada categoría? | Barras verticales (bar) | `<ChartCard />` tipo `bar` |
| ¿Cómo se comparan A, B, C? | Barras horizontales | `<ChartCard />` tipo `horizontalBar` |
| ¿Qué parte del total es X? | Donut / Pie | `<ChartCard />` tipo `doughnut` |
| ¿Cuáles son las causas principales? | Pareto (barras + línea) | `<ChartCard />` combinado |
| ¿Dónde se concentran los riesgos? | Heatmap | Tabla CSS con colores semafóricos |
| ¿Cuáles son los top N? | Barras horizontales | `<RankingCard />` (Fase 2) |

### Layout estándar de gráficos

```razor
<SeccionDash Titulo="Análisis" Icono="bi-bar-chart-fill">
    <div class="row g-3">
        <div class="col-xl-6">
            <ChartCard Titulo="Tendencia mensual" CanvasId="chart-tendencia" Altura="250" />
        </div>
        <div class="col-xl-6">
            <ChartCard Titulo="Distribución por área" CanvasId="chart-dist" Altura="250" />
        </div>
    </div>
</SeccionDash>
```

### Reglas de gráficos en dashboards

1. **Máximo 4 gráficos por sección analítica.** Si se necesitan más, agrupar en sub-secciones.
2. **Todos los gráficos deben tener título claro**, sin abreviaciones ambiguas.
3. **El eje Y siempre empieza en 0**, salvo excepciones técnicas justificadas.
4. **Los gráficos de tendencia muestran mínimo 4 períodos** (4 semanas, 4 meses, etc.)
5. **Los colores de las barras siguen el sistema**: azul `#183963` como base, semáforo solo cuando comunica estado.
6. **Cada canvas necesita un ID único** en toda la aplicación para evitar conflictos de Chart.js.

---

## Nivel 4 — Tabla inteligente

### Propósito

Detalle operativo. El usuario que necesita profundidad puede ir aquí. **La tabla nunca es lo primero.**

### Reglas de tablas en dashboards

1. **La tabla va siempre al final**, debajo de KPIs y gráficos.
2. **La tabla tiene encabezado azul corporativo** (`thead` con clase `table-dark` o el override de `app.css`).
3. **La tabla tiene una columna de "Estado"** con badge semafórico cuando los datos tienen estados.
4. **La tabla tiene búsqueda o filtro propio** cuando tiene más de 10 filas.
5. **La tabla tiene paginación** cuando puede exceder 20 filas.
6. **Las acciones de la tabla** (Editar, Ver, Eliminar) van en la última columna.
7. **Las columnas de fecha** usan formato `dd/MM/yyyy`.
8. **Los valores monetarios** usan formato `$ #,##0`.
9. **Los porcentajes** usan formato `0.0%`.

### Layout estándar de tabla

```razor
<SeccionDash Titulo="Detalle Operativo" Icono="bi-table">
    <div class="table-responsive">
        <table class="table table-hover align-middle rn-table">
            <thead class="table-dark">
                <tr>
                    <th>Columna 1</th>
                    <th>Columna 2</th>
                    <th>Estado</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var item in _lista)
                {
                    <tr>
                        <td>@item.Campo1</td>
                        <td>@item.Campo2</td>
                        <td><span class="rn-badge rn-badge-saludable">@item.Estado</span></td>
                        <td>
                            <a href="..." class="btn btn-sm btn-outline-primary">Ver</a>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    </div>
</SeccionDash>
```

---

## Nivel 5 — Análisis Inteligente IA

### Propósito

Síntesis automática de los datos del dashboard. El usuario no tiene que interpretar los gráficos; el sistema ya lo hizo.

### Contenido estándar del panel IA

El panel `<AnalisisIA />` debe mostrar (en este orden cuando esté implementado):

1. **Resumen ejecutivo** — 1 o 2 frases que describen el estado general del módulo.
2. **Hallazgos principales** — Lista de los 3–5 datos más relevantes del período.
3. **Alertas activas** — Ítems que requieren atención inmediata.
4. **Tendencias** — Qué está mejorando, qué está empeorando.
5. **Recomendaciones** — Acciones concretas sugeridas.
6. **Riesgos identificados** — Factores que podrían deteriorar los indicadores.

### Estado actual (Fase 1 — simulado)

Por ahora el contenido del panel IA es estático y se escribe manualmente basándose en los datos del módulo. Esto está bien: la estructura existe, el contenido vendrá de la IA real en Fase 3.

```razor
<AnalisisIA Titulo="Análisis Inteligente" Subtitulo="Módulo Costos · Proyecto Alpha">
    <p class="fw-bold mb-1">Estado general: <span class="text-warning">En Riesgo</span></p>
    <ul class="mb-2">
        <li>El costo ejecutado supera en 8% el presupuesto original en la partida civil.</li>
        <li>3 órdenes de cambio pendientes de aprobación por un total de $45M.</li>
    </ul>
    <p class="fw-bold mb-1">Recomendaciones:</p>
    <ul class="mb-0">
        <li>Revisar partidas de obra civil con el residente antes del cierre del período.</li>
        <li>Aprobar o rechazar las OC pendientes antes del 05/07.</li>
    </ul>
</AnalisisIA>
```

---

## Tipos de dashboard en RenergeIA

### Dashboard de módulo

Foco: un solo tipo de dato (solo costos, solo inspecciones, solo capacitaciones).
Audiencia: coordinador o especialista del área.
Profundidad: los 5 niveles completos.

### Dashboard de área

Foco: visión consolidada de un área (HSEQ, Financiero, Técnico).
Audiencia: jefe de área, director técnico.
Profundidad: KPIs macro de cada sub-módulo + alertas activas + enlace a módulo detallado.

### Dashboard del proyecto

Foco: estado general de un proyecto específico.
Audiencia: gerente de proyecto, cliente.
Profundidad: KPIs de avance, costo, HSEQ + semáforo global + alertas prioritarias.

### Dashboard General (Home)

Foco: todos los proyectos activos.
Audiencia: director, gerencia, comité.
Profundidad: tarjeta por proyecto con semáforo + indicadores macro + acceso rápido.

---

## Checklist de dashboard — antes de publicar

- [ ] ¿Tiene filtros en el nivel 1?
- [ ] ¿Los KPIs del nivel 2 tienen estado semafórico?
- [ ] ¿Hay mínimo 2 KPIs visuales (gauge o donut)?
- [ ] ¿Los gráficos del nivel 3 usan el tipo correcto para el dato?
- [ ] ¿La tabla del nivel 4 está al final?
- [ ] ¿La tabla tiene encabezado azul corporativo?
- [ ] ¿Existe el panel IA del nivel 5 (aunque sea con contenido simulado)?
- [ ] ¿Todos los componentes son del Design System (`rn-*`)?
- [ ] ¿El dashboard se ve bien en 1280px de ancho mínimo?

---

*Documento generado: 2026-06-28. Aplicar desde Fase 2 (estandarización de dashboards existentes).*
