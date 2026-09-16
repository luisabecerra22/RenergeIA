# 14. Dashboard del proyecto

> **Para qué sirve:** ver en una sola pantalla cómo va el avance físico del proyecto: avance programado vs real a la fecha, desviación, SPI, actividades atrasadas y críticas, Curva S, avance por disciplina y un plan de acción con las actividades que necesitan atención.
> **Quién lo usa:** dirección de proyecto, control de proyectos / planeación, gerencia, coordinadores de disciplina.
> **Dónde está:** menú lateral del proyecto → **Dashboard**, o la tarjeta **Dashboard** en el detalle del proyecto (`/proyectos/{id}/dashboard`).

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Actividad hoja** | Actividad del cronograma que no tiene sub-actividades. **Todos los cálculos usan solo actividades hoja** del cronograma vigente. |
| **Cronograma vigente** | La versión del cronograma marcada como vigente en el Cronograma de Actividades (WBS). |
| **Avance programado** | Porcentaje que debería llevar una actividad hoy, según el tiempo transcurrido entre su inicio y su fin planeados (en línea recta). |
| **Avance real** | Último avance acumulado reportado en el Informe Diario para la actividad; si no tiene reportes, el avance real registrado en el cronograma. |
| **Desviación** | Avance real − avance programado, en puntos porcentuales. Negativa = atraso. |
| **SPI** | Índice de desempeño del cronograma = avance real ÷ avance programado. 1,00 = al día; menor a 1 = atraso. |
| **Curva S** | Gráfica del avance acumulado planificado vs real a lo largo del tiempo. |
| **Disciplina** | Civil, Eléctrica, Mecánica, etc., asignada a cada actividad en el cronograma. |
| **CR** | Marca de las actividades señaladas como críticas en el cronograma. |

---

## Cómo funciona

Nada se digita en el Dashboard: **todo se calcula** con el cronograma vigente y los informes diarios cada vez que se abre la página.

### Indicadores generales

- **AVANCE PROGRAMADO** y **AVANCE REAL**: promedio simple de todas las actividades hoja (todas pesan igual, sin importar duración ni costo).
- **DESVIACIÓN**: avance real − avance programado.
- **SPI GLOBAL**: avance real ÷ avance programado. Si el programado es 0, el SPI es 1,00.
- **ESTADO GENERAL** (también aparece como etiqueta junto al título):

| SPI | Estado | Texto bajo el SPI |
|---|---|---|
| 1,00 o más | **Al Día** (verde) | Al día o adelantado |
| 0,90 a 0,99 | **Leve Atraso** (naranja) | Leve atraso |
| 0,75 a 0,89 | **Atrasado** (rojo) | Atraso significativo |
| Menos de 0,75 | **Crítico** (rojo) | Atraso significativo |

- Colores de avance real y desviación: verde si la desviación es 0 o positiva; naranja entre 0 y −5; rojo por debajo de −5.

### Clasificación de cada actividad

Se aplica en este orden:

| Estado | Regla |
|---|---|
| **Finalizada** | Avance real de 100 % o más |
| **Sin iniciar** | Avance real 0 % |
| **Crítica** | Desviación menor a −15 puntos |
| **Atrasada** | Desviación menor a −5 puntos y de hasta −15 |
| **En línea** | Cualquier otro caso |

> Importante: una actividad con 0 % de avance real cuenta como **Sin iniciar** aunque ya debería ir adelantada; no aparece como atrasada ni crítica. Revisa en el Cronograma las actividades sin iniciar cuya fecha de inicio ya pasó.

### Curva S

- **Línea planificada:** un punto por semana, desde el inicio más temprano hasta el fin más tardío de las actividades hoja; cada punto es el promedio del avance programado de todas las actividades hoja en esa fecha.
- **Línea real:** para cada semana, el último valor disponible del promedio de avance acumulado de los informes diarios. Si no hay informes, la app dibuja una línea proporcional hasta el avance real actual. Las semanas futuras no tienen línea real.
- Se agrega un punto para **hoy** si no cae en una semana exacta.
- Debajo, **Corte a hoy** muestra dos barras: **Avance Ejecutado** (promedio del avance real de las actividades hoja en el cronograma) y **Avance Planeado** a hoy.

> ⚠ Por confirmar: el **Avance Ejecutado** de la barra "Corte a hoy" usa el avance real guardado en el cronograma, mientras que la tarjeta **AVANCE REAL** usa el último reporte del Informe Diario de cada actividad. Normalmente coinciden; si ves una diferencia, valida con planeación cuál manda.

### Gráficas y tablas

- **Avance por disciplina — Real vs Programado:** barras por disciplina (solo actividades con disciplina asignada).
- **Estado de actividades:** dona con En Línea, Atrasadas, Críticas, Finalizadas y Sin iniciar.
- **Actividades más atrasadas:** hasta 10 actividades con desviación negativa, de la peor a la menos mala.
- **Plan de acción — Actividades críticas y atrasadas:** todas las críticas y atrasadas con código, actividad, disciplina, programado, real, desviación, estado y una **Recomendación** automática:
  - Crítica: *Intervención urgente: revisar recursos, restricciones y productividad…*
  - Atrasada: *Seguimiento diario requerido. Validar restricciones y reasignar recursos…*
- **Detalle por disciplina:** programado, real, desviación (verde ≥ 0, amarillo hasta −5, rojo menor a −5), actividades atrasadas (incluye críticas) y total.

---

## Paso a paso

### Revisar el estado del proyecto

1. Entra a **Dashboard** del proyecto. Arriba ves la fecha y hora de la consulta y la etiqueta del estado general.
2. Lee la primera fila de tarjetas: **AVANCE PROGRAMADO**, **AVANCE REAL**, **DESVIACIÓN** y **SPI GLOBAL**.
3. Lee la segunda fila: **ACT. ATRASADAS** (de cuántas en total), **ACT. CRÍTICAS**, **ACT. FINALIZADAS** y **ESTADO GENERAL** (con cuántas van en línea y cuántas sin iniciar).

### Analizar la Curva S

1. Ubica la tarjeta **Curva S — Avance real vs planificado**.
2. Compara la línea real con la planificada: si la real está por debajo, el proyecto va atrasado.
3. Revisa las barras de **Corte a hoy**.
4. Si acabas de registrar informes diarios o cambiar el cronograma, pulsa **Actualizar** (junto al título). Aparece *Actualizada* por unos segundos.

> Nota: **Actualizar** recalcula los datos y vuelve a dibujar la Curva S y las tarjetas. Para refrescar también las demás gráficas, recarga la página (F5).

### Identificar dónde está el atraso

1. En **Avance por disciplina** y **Detalle por disciplina**, ubica las disciplinas en rojo.
2. En **Actividades más atrasadas**, identifica las de mayor desviación.
3. En **Plan de acción**, revisa las actividades con estado **Crítica** (y las marcadas **CR**) y aplica la recomendación.
4. Lleva esas actividades a la reunión de planeación semanal y registra las causas en **Restricciones**.

### Cuando el Dashboard está vacío

1. Si aparece *No hay actividades WBS configuradas para este proyecto.*, pulsa **Ir a WBS**.
2. Carga el cronograma (plantilla EPC, Excel, PDF o MS Project) en el **Cronograma de Actividades**.
3. Vuelve al Dashboard.

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| *No hay actividades WBS configuradas para este proyecto.* | El proyecto no tiene cronograma, o la versión vigente no tiene actividades activas. | Pulsa **Ir a WBS** y carga el cronograma o activa la versión correcta. |
| *No hay actividades con fechas programadas.* en la Curva S | Las actividades no tienen fechas planeadas utilizables. | Revisa las fechas de inicio y fin en el cronograma. |
| *No hay actividades con disciplina asignada.* | Ninguna actividad hoja tiene disciplina. | Asigna la disciplina a las actividades en el cronograma. |
| El avance real está en 0 % aunque se trabaja en obra | No se han registrado informes diarios con avance, o se registraron sobre otra versión del cronograma. | Registra el avance en **Informe Diario** y verifica que la versión vigente del cronograma sea la correcta. |
| Hay muchas actividades **Sin iniciar** y pocas atrasadas, pero el proyecto se siente atrasado | Las actividades con 0 % no se clasifican como atrasadas (ver *Cómo funciona*). | Filtra en el cronograma las actividades sin avance cuya fecha de inicio ya pasó. |
| Los datos no cambiaron después de registrar un informe | La página muestra lo calculado al abrirla. | Pulsa **Actualizar** o recarga la página. |
| Las gráficas de disciplinas o estados no cambiaron tras **Actualizar** | **Actualizar** solo vuelve a dibujar la Curva S. | Recarga la página (F5). |
| El SPI y el avance no coinciden con el cálculo de planeación | La app usa promedio simple de actividades hoja, sin ponderar por duración, costo o peso. | Tómalo como indicador de tendencia; para el avance ponderado oficial, valida con planeación. |
| Tras una reprogramación, los números cambiaron mucho | El Dashboard usa siempre la **versión vigente** del cronograma. | Es esperado. Revisa en el Cronograma cuál versión está activa como vigente. |

---

## Relación con otros módulos

- **Cronograma de Actividades (WBS)** ([2](02-cronograma-actividades.md)): de aquí salen las actividades hoja, sus fechas planeadas, disciplina, marca de crítica y la versión vigente.
- **Informe Diario** ([3](03-informe-diario.md)): de aquí sale el avance real acumulado de cada actividad.
- **Restricciones:** las recomendaciones del plan de acción invitan a revisar y registrar restricciones de las actividades atrasadas.
- **Clima** ([13](13-clima.md)): usa el mismo cronograma vigente para las alertas operacionales de los próximos 3 días.

> Este Dashboard muestra **avance físico**. Los indicadores de costos están en **Costos** ([5](05-costos.md)) y los de HSEQ en sus propios dashboards.

---

## Buenas prácticas

- Registra el **Informe Diario todos los días**: sin avance reportado, el Dashboard muestra atraso aunque se esté trabajando.
- Mantén una sola versión **vigente** del cronograma y crea reprogramaciones solo cuando estén aprobadas.
- Asigna **disciplina** a todas las actividades hoja, para que la vista por disciplina sea completa.
- Revisa el **Plan de acción** en la reunión semanal y registra las causas del atraso en **Restricciones**.
- Usa el SPI como alerta temprana: por debajo de **0,90** conviene tomar acciones.
- Antes de presentar el Dashboard, pulsa **Actualizar** o recarga la página para tener los datos al momento.
