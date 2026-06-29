# Design System — RenergeIA
**Versión:** 1.0 | **Fecha:** 2026-06-28 | **Estado:** Vigente

> Este documento es la fuente de verdad visual de RenergeIA. Todo nuevo desarrollo debe cumplir estas reglas antes de ser integrado a la plataforma.

---

## 1. Filosofía visual

RenergeIA es una plataforma de gestión de proyectos EPC fotovoltaicos. Su diseño debe reflejar:

- **Seriedad corporativa** — Es una herramienta de trabajo, no una app de consumo.
- **Claridad ejecutiva** — Los gerentes y directores deben leer el estado de un proyecto en menos de 10 segundos.
- **Minimalismo funcional** — Cada elemento visual existe porque comunica algo. Si no comunica, sobra.
- **Confianza técnica** — El diseño debe transmitir precisión y profesionalismo a los equipos de ingeniería.

**Regla de oro:** Si no sabes por qué un color, ícono o componente está ahí, es porque no debería estar.

---

## 2. Paleta de colores

### Colores corporativos principales

| Token CSS | Valor | Nombre | Uso |
|---|---|---|---|
| `--renergeia-blue` | `#183963` | Azul corporativo | Color principal, encabezados, botones primarios, links |
| `--renergeia-green` | `#6ABF4B` | Verde solar | Acciones positivas, éxito, progreso saludable |
| `--renergeia-gray` | `#D9D9D6` | Gris claro | Bordes, fondos secundarios, separadores |
| `--renergeia-black` | `#111921` | Negro corporativo | Texto principal en pantallas oscuras |

### Semáforo de estados (obligatorio en toda la plataforma)

| Estado | Color | Hex | Clase CSS | Cuándo usarlo |
|---|---|---|---|---|
| Saludable | Verde | `#6ABF4B` | `.rn-badge-saludable` | Indicador ≥ 90%, sin alertas críticas |
| En Riesgo | Amarillo | `#ffc107` | `.rn-badge-en-riesgo` | Indicador entre 70–89%, requiere atención |
| Riesgo | Naranja | `#fd7e14` | `.rn-badge-riesgo` | Indicador entre 50–69%, desviación notable |
| Crítico | Rojo | `#dc3545` | `.rn-badge-critico` | Indicador < 50%, acción inmediata requerida |
| Informativo | Azul | `#183963` | `.rn-badge-informativo` | Datos neutrales, sin calificación de estado |
| Sin datos | Gris | `#6c757d` | `.rn-badge-sin-datos` | Campo vacío, datos no disponibles |

### Regla estricta de uso de color

> **Prohibido usar colores por decoración.**
> Todo color debe comunicar: estado, categoría, prioridad o jerarquía.
> Si un color no tiene significado funcional, reemplazarlo por gris o blanco.

### Fondos y superficies

| Superficie | Color | Cuándo |
|---|---|---|
| Fondo de página | `#f8f9fa` (gris muy claro) | Fondo general del contenido |
| Fondo de tarjeta/card | `#ffffff` (blanco) | Todas las cards y panels |
| Fondo de sidebar | `#183963` (azul) | Navegación lateral |
| Fondo de encabezados de tabla | `#183963` (azul) | `<thead>` de todas las tablas |

---

## 3. Tipografía

### Fuente corporativa: Montserrat

Montserrat es la única fuente permitida en RenergeIA. Ya está configurada globalmente en `app.css` y se carga desde Google Fonts.

```css
--font-family-primary: 'Montserrat', sans-serif;
```

### Escala tipográfica

| Uso | Tamaño | Peso | Clase / Tag |
|---|---|---|---|
| Título de página | 1.5rem | 700 Bold | `<h1>` |
| Título de sección | 1.1rem | 700 Bold | `<h5>` o `.rn-section-header` |
| Valor KPI grande | 1.8–2.2rem | 700 Bold | `.rn-kpi-value`, `.rn-gauge-value` |
| Etiqueta de KPI | 0.75rem | 600 SemiBold | `.rn-gauge-title`, `.rn-kpi-title` |
| Texto de cuerpo | 0.875rem | 400 Regular | `<p>`, `<td>` |
| Texto secundario | 0.75rem | 400 Regular | `.text-muted`, subtextos |
| Badge / estado | 0.65rem | 700 Bold | `.rn-badge-*` |

### Reglas tipográficas

- Los títulos de página siempre en azul corporativo `#183963`.
- Los valores de KPI se colorean según el estado semafórico.
- Nunca usar italic en la interfaz.
- No usar fuentes distintas a Montserrat bajo ninguna circunstancia.

---

## 4. Espaciado

RenergeIA usa el sistema de espaciado de Bootstrap 5 (base 4px).

| Clase Bootstrap | Valor | Uso recomendado |
|---|---|---|
| `p-2` / `m-2` | 8px | Padding interno de badges y chips |
| `p-3` / `m-3` | 12px | Padding de cards pequeñas |
| `p-4` / `m-4` | 16px | Padding estándar de secciones |
| `gap-3` | 12px | Separación entre tarjetas KPI |
| `gap-4` | 16px | Separación entre secciones |
| `mb-4` | 16px | Separación entre bloques de contenido |

### Regla de espaciado

> Siempre usar las clases de Bootstrap para espaciado. Nunca agregar `margin` o `padding` inline sin justificación técnica. Los valores ad-hoc crean inconsistencia.

---

## 5. Iconografía

RenergeIA usa exclusivamente **Bootstrap Icons** (bi-*). Ya está incluido en el proyecto.

### Íconos estándar por módulo

| Módulo | Ícono |
|---|---|
| Inicio / Dashboard | `bi-house-door-fill` / `bi-speedometer2` |
| Proyectos | `bi-folder-fill` |
| WBS / Cronograma | `bi-diagram-3` |
| Informe Diario | `bi-journal-text` |
| Costos | `bi-currency-dollar` |
| Personal | `bi-people-fill` |
| Equipos | `bi-truck` |
| Documentos | `bi-folder2-open` |
| HSEQ | `bi-shield-check` |
| Seguridad | `bi-shield-fill` |
| Calidad | `bi-patch-check-fill` |
| Ambiental | `bi-tree-fill` |
| Social | `bi-heart-fill` |
| No Conformidades | `bi-exclamation-octagon` |
| Restricciones | `bi-shield-x` |
| Histogramas | `bi-bar-chart-steps` |
| Clima | `bi-cloud-sun` |
| Alertas | `bi-bell-fill` |
| IA / Análisis | `bi-cpu` |
| Reportes | `bi-file-earmark-bar-graph` |

### Reglas de iconografía

- Tamaño estándar: `fs-4` (1.5rem) para íconos de módulo, `fs-5` (1.25rem) para íconos en cards.
- No agregar íconos puramente decorativos. Si el ícono no ayuda a identificar rápidamente el elemento, omitirlo.
- El color del ícono siempre hereda el estado del elemento o usa `--renergeia-blue`.

---

## 6. Estados visuales

Todos los elementos interactivos y los datos deben mostrar su estado visualmente.

### Estados de datos

```
Saludable  → Verde   #6ABF4B   ██  (todo bien)
En Riesgo  → Amarillo #ffc107  ██  (atención requerida)
Riesgo     → Naranja  #fd7e14  ██  (desviación notable)
Crítico    → Rojo    #dc3545   ██  (acción inmediata)
Informativo→ Azul    #183963   ██  (neutro)
Sin datos  → Gris    #6c757d   ██  (no disponible)
```

### Reglas de semaforizació del gauge

| Valor | Estado asignado |
|---|---|
| ≥ 90% | Saludable |
| 70–89% | En Riesgo |
| 50–69% | Riesgo |
| < 50% | Crítico |

Estos umbrales son el **default**. Cada módulo puede sobrescribirlos según su contexto (ej: cumplimiento legal puede tener umbral 95%).

### Estados de interfaz

| Estado | Componente a usar | Cuándo |
|---|---|---|
| Cargando | `<LoadingState />` | Mientras se cargan datos |
| Sin datos | `<EmptyState />` | Cuando la consulta devuelve vacío |
| Error | `<ErrorState />` | Cuando la carga falla |

---

## 7. Gráficos — reglas de selección

La elección del tipo de gráfico no es estética. Cada tipo comunica un patrón específico.

| Tipo de gráfico | Componente | Cuándo usarlo |
|---|---|---|
| Gauge / Donut radial | `<GaugeCircular />` | Porcentaje de cumplimiento, avance |
| Barras verticales | `<ChartCard />` (bar) | Comparación entre categorías o períodos |
| Barras horizontales | `<ChartCard />` (horizontalBar) | Rankings, top-N, comparaciones largas |
| Línea de tendencia | `<ChartCard />` (line) | Evolución en el tiempo, tendencias |
| Donut / Pie | `<ChartCard />` (doughnut) | Distribución de partes de un todo |
| Pareto | `<ChartCard />` (bar + line) | Causas principales, priorización 80/20 |
| Heatmap | CSS + tabla | Concentración de riesgos, densidad |

### Reglas para gráficos

1. **Máximo 2 tipos de gráfico por sección.** Más de dos confunde la lectura.
2. **Los gráficos de barra siempre en azul corporativo** como color base. Usar el semáforo solo cuando el color comunica estado.
3. **Los ejes deben tener etiquetas legibles.** Nunca truncar etiquetas de eje.
4. **Altura mínima de canvas:** 200px. Altura recomendada: 220–280px.
5. **Leyendas:** Solo cuando el gráfico tiene 2 o más series de datos.
6. **No usar 3D, gradientes excesivos ni sombras en los gráficos.** Flat design.

---

## 8. Tarjetas (Cards)

### Jerarquía de tarjetas en RenergeIA

| Nivel | Componente | Contenido |
|---|---|---|
| KPI Visual | `<GaugeCircular />` | Un indicador con gauge y estado |
| KPI Texto | `<TarjetaKPI />` | Valor numérico con ícono y badge |
| Gráfico | `<ChartCard />` | Canvas de Chart.js con título |
| Sección | `<SeccionDash />` | Agrupa cards relacionadas |
| IA | `<AnalisisIA />` | Panel de análisis inteligente |

### Reglas de cards

- **Todas las cards tienen sombra sutil:** `box-shadow: 0 2px 8px rgba(24,57,99,.08)`
- **Hover effect:** `translateY(-2px)` + sombra más pronunciada
- **Border-radius:** `0.5rem` estándar
- **Fondo:** siempre blanco `#fff`
- **No usar `border` de color en cards.** El color va en el contenido, no en el borde de la card.

---

## 9. Reglas responsive

RenergeIA es prioritariamente una aplicación de escritorio (operadores y gerentes en computador). Sin embargo, debe ser legible en tablet.

### Breakpoints aplicados

| Vista | Ancho | Comportamiento |
|---|---|---|
| Desktop | ≥ 1200px | Layout completo, sidebar visible |
| Laptop | 992–1199px | Layout completo, sidebar compacto |
| Tablet | 768–991px | Sidebar colapsado, grillas de 2 columnas |
| Móvil | < 768px | Una columna, lectura vertical |

### Grilla de KPIs

```
Desktop (≥ 992px): col-xl-3 col-md-6   → 4 KPIs por fila
Tablet  (≥ 768px): col-md-6            → 2 KPIs por fila
Móvil   (< 768px): col-12             → 1 KPI por fila
```

### Grilla de gráficos

```
Desktop: col-xl-6   → 2 gráficos por fila
Tablet:  col-12     → 1 gráfico por fila
```

---

## 10. Animaciones

Las animaciones deben ser sutiles y funcionales. Nunca deben distraer.

| Elemento | Animación | Duración |
|---|---|---|
| Gauge al cargar | `rn-gauge-in` — stroke crece desde 0 | 1.2s ease |
| Cards al hacer hover | `translateY(-2px)` + sombra | 0.2s ease |
| Menú expandir/colapsar | Altura animada | 0.15s ease |
| Spinner de carga | Rotación continua | 0.75s linear infinite |

### Reglas de animación

- **No usar** `animation: none` para deshabilitar animaciones en producción.
- **No agregar** animaciones que duren más de 1.5 segundos.
- **No animar** más de 2 propiedades CSS simultáneamente en el mismo elemento.
- **Las animaciones deben respetar** `prefers-reduced-motion` cuando se implemente accesibilidad.

---

## 11. Coherencia visual — checklist obligatorio

Antes de integrar cualquier nueva pantalla, verificar:

- [ ] ¿Usa Montserrat como tipografía?
- [ ] ¿Los colores usados están en la paleta corporativa?
- [ ] ¿Todo estado de dato tiene su badge semafórico?
- [ ] ¿Los encabezados de tabla son azul `#183963`?
- [ ] ¿Las cards tienen la sombra estándar `rn-*`?
- [ ] ¿Los íconos son de Bootstrap Icons?
- [ ] ¿El tipo de gráfico corresponde al tipo de dato mostrado?
- [ ] ¿Existe un estado vacío (`<EmptyState />`) cuando no hay datos?
- [ ] ¿El layout usa Bootstrap 5 grid (no CSS manual)?
- [ ] ¿La página tiene `<PageHeader />` con título y breadcrumb?

---

*Documento generado: 2026-06-28. Próxima revisión al completar Fase 2 (componentes reutilizables).*
