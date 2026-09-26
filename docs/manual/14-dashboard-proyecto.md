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
| **Disciplina** | Suministros, Ingeniería, Civil, Mecánica, Eléctrica, Puesta en marcha, Cierre de proyecto, etc., asignada a cada actividad **hoja** en el cronograma (las actividades con subactividades no llevan disciplina). |
| **CR** | Marca de las actividades señaladas como críticas en el cronograma. |

---

## Cómo funciona

Nada se digita en el Dashboard: **todo se calcula** con el cronograma vigente y los informes diarios cada vez que se abre la página.

### Indicadores generales

- **AVANCE PROGRAMADO** y **AVANCE REAL**: avance de todas las actividades hoja del cronograma vigente **ponderado por la duración planeada de cada una** (los hitos no pesan). Son los mismos valores del último punto de la Curva S con alcance "Todo el proyecto".
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

- **Alcance:** el selector junto al título permite ver **Todo el proyecto** (todas las actividades hoja del cronograma vigente: contractual, ingeniería, suministros, construcción, commissioning y cierre) o **Solo construcción** (únicamente las actividades hoja con disciplina Civil, Mecánica, Eléctrica o Construcción). El segundo alcance es el comparable con la curva de "Avance de construcción" del informe interno.
- **Ponderación:** cada actividad hoja pesa según su **duración planeada en días**. Una actividad de seis meses pesa más que una de dos días, y los **hitos** (duración 0) no aportan peso. Con esto la curva toma la forma de S clásica: arranque lento, máximo ritmo cuando más frentes están activos y cierre suave. Todavía no se pondera por costo ni por horas-hombre (el cronograma no trae esa información).
- **Línea planificada:** un punto por semana desde el inicio más temprano hasta el fin más tardío de las actividades hoja (más el fin del proyecto, la fecha de hoy y la del primer informe). Cada punto es el avance lineal esperado de cada actividad a esa fecha, ponderado por duración.
- **Línea real, en tres tramos (de más a menos confiable):**
  1. **Histórico cargado** (tramo sólido): la serie de % acumulado real de tu informe interno, cargada con el botón **Histórico real**. Es la única forma de que la app conozca el avance real de las fechas anteriores a los informes diarios.
  2. **Informes diarios** (tramo sólido): desde el primer informe diario posterior al histórico, para cada fecha se toma, por actividad, el último avance acumulado informado y se pondera igual que el planificado. Si un informe fue registrado sobre una versión anterior del cronograma, la app lo cruza con la actividad vigente del mismo código WBS.
  3. **Estimación** (tramo **punteado**): solo donde no hay dato: el puente entre el fin del histórico y el avance actual del cronograma (o el primer informe), o, si no hay nada cargado, el avance actual de cada actividad repartido linealmente desde su inicio. La nota bajo la gráfica avisa cuando existe un tramo estimado.
- **Histórico real (botón junto a Actualizar):** abre una ventana para pegar desde Excel dos columnas, **fecha** y **% real acumulado** (una fila por línea). Acepta fechas `09/02/2026`, `9-feb-26`, `2026-02-09` o seriales de Excel, y porcentajes `41,5%`, `41.5` o `0,415` (si todos los valores son ≤ 1 se toman como fracciones). Muestra una vista previa con puntos válidos, rango de fechas, último valor y filas ignoradas (encabezados se ignoran sin error), y avisa si algún valor baja respecto al anterior. Elige el **alcance** al que pertenece la serie (Todo el proyecto o Solo construcción); al **Guardar** se reemplaza el histórico anterior de ese alcance. **Borrar histórico** elimina la serie del alcance elegido. El botón muestra en verde cuántos puntos hay cargados.
- Debajo de la gráfica, una nota indica cuántas actividades hoja entran en el cálculo, cuántos hitos hay sin peso y desde qué fecha hay informes.
- **Corte a hoy** (fecha de Colombia, UTC−5) muestra dos barras: **Avance Ejecutado** y **Avance Planeado**, que son exactamente el último punto de cada línea de la curva. Con el alcance "Todo el proyecto" coinciden con las tarjetas **AVANCE REAL** y **AVANCE PROGRAMADO**.

> Nota: la curva del informe interno se alimenta de otra fuente (cronograma en Excel/Project con su propio histórico), por lo que puede diferir de la de la app en las semanas anteriores al primer informe diario registrado.

### Gráficas y tablas

- **Avance por disciplina — Real vs Programado:** barras en verde (real) y azul (programado) de RenergeIA, siempre en este orden y solo para estas disciplinas: **Suministros, Ingeniería, Civil, Mecánica, Eléctrica, Puesta en marcha y Cierre de proyecto** (definido el 2026-09-26). Contractual, General y Construcción no se grafican. Una disciplina sin actividades hoja no aparece.
- **Estado de actividades:** dona con En Línea, Atrasadas, Críticas, Finalizadas y Sin iniciar.
- **Actividades más atrasadas:** hasta 10 actividades con desviación negativa, de la peor a la menos mala.
- **Plan de acción — Actividades críticas y atrasadas:** todas las críticas y atrasadas con código, actividad, disciplina, programado, real, desviación, estado y una **Recomendación** automática:
  - Crítica: *Intervención urgente: revisar recursos, restricciones y productividad…*
  - Atrasada: *Seguimiento diario requerido. Validar restricciones y reasignar recursos…*
- **Detalle por disciplina:** mismas disciplinas y orden que la gráfica; programado, real, desviación (verde ≥ 0, amarillo hasta −5, rojo menor a −5), actividades atrasadas (incluye críticas) y total.

---

## Paso a paso

### Revisar el estado del proyecto

1. Entra a **Dashboard** del proyecto. Arriba ves la fecha y hora de la consulta y la etiqueta del estado general.
2. Lee la primera fila de tarjetas: **AVANCE PROGRAMADO**, **AVANCE REAL**, **DESVIACIÓN** y **SPI GLOBAL**.
3. Lee la segunda fila: **ACT. ATRASADAS** (de cuántas en total), **ACT. CRÍTICAS**, **ACT. FINALIZADAS** y **ESTADO GENERAL** (con cuántas van en línea y cuántas sin iniciar).

### Analizar la Curva S

1. Ubica la tarjeta **Curva S — Avance real vs planificado**.
2. Elige el **Alcance** (Todo el proyecto o Solo construcción) y compara la línea real con la planificada: si la real está por debajo, el proyecto va atrasado.
3. Si la línea real tiene un tramo punteado, pulsa **Histórico real** y pega desde Excel la serie de fecha y % real acumulado de tu informe interno; guarda y la curva pasa a dibujar esos datos.
4. Revisa las barras de **Corte a hoy**.
5. Si acabas de registrar informes diarios o cambiar el cronograma, pulsa **Actualizar** (junto al título). Aparece *Actualizada* por unos segundos.

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
| *Ninguna actividad hoja… tiene disciplina Civil, Mecánica, Eléctrica o Construcción* en la Curva S | El alcance "Solo construcción" no encontró actividades con esas disciplinas. | Asigna la disciplina a las actividades en el Cronograma o vuelve a "Todo el proyecto". |
| *No hay actividades con fechas programadas.* en la Curva S | Las actividades no tienen fechas planeadas utilizables. | Revisa las fechas de inicio y fin en el cronograma. |
| *No hay actividades hoja con disciplina Suministros, Ingeniería…* | Ninguna actividad hoja tiene una de las siete disciplinas que se grafican (Contractual, General y Construcción no cuentan). | Asigna la disciplina a las actividades en el cronograma. |
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
