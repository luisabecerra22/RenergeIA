# 18. Centro de control (Inicio)

> **Para qué sirve:** ver en una sola pantalla cómo va todo el portafolio de proyectos EPC: cuáles están sanos, en riesgo o críticos y por qué, el avance real frente al programado, qué proyectos terminarían después de su fecha contractual, el sobrecosto estimado, los bloqueos abiertos y un resumen ejecutivo listo para gerencia.
> **Quién lo usa:** gerencia, dirección de proyectos, control de proyectos y control de costos.
> **Dónde está:** es la página de **Inicio** de la aplicación: menú lateral → **Inicio**, o al entrar a la app (`/`). Es una vista corporativa: no está dentro de un proyecto.

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Portafolio** | Todos los proyectos activos de la app. Los proyectos **Cancelados** o **Completados** no aparecen. |
| **Avance programado / real** | Los mismos valores de la Curva S de cada proyecto a la fecha de hoy (ponderados por duración de las actividades y por las categorías con peso). Son idénticos a las tarjetas del Dashboard del proyecto ([14](14-dashboard-proyecto.md)). |
| **Brecha (p.p.)** | Avance real − avance programado, en **puntos porcentuales**: la resta directa de dos porcentajes. Real 40 % y programado 45 % = **−5,0 p.p.** Negativa = atraso. |
| **Índice de cumplimiento** | Avance real ÷ avance programado (1,00 = al día; menos de 1 = atraso). Se muestra en el Dashboard de cada proyecto. **No es el SPI de valor ganado**: los cronogramas no traen costos por actividad, así que es un índice de avance físico. |
| **Fecha contractual** | Fecha fin planeada registrada en la ficha del proyecto. |
| **Fecha proyectada** | Fecha fin del **cronograma vigente** (la última reprogramación aprobada). |
| **Retraso de plazo** | Fecha proyectada − fecha contractual, en **días calendario**. |
| **Presupuesto, ejecutado y comprometido** | Salen del módulo Costos de cada proyecto, **en pesos** (lo que está en dólares se convierte con la TRM del proyecto). Ejecutado = lo pagado; comprometido = lo que falta pagar según el Seguimiento de pagos. |
| **Sobrecosto** | Suma, código por código del presupuesto, de lo que el ejecutado + comprometido **supera** el presupuesto de ese código. Un código que gasta menos no compensa a otro que se pasa. |
| **Costo estimado al cierre** | Presupuesto + sobrecosto. |
| **Bloqueo** | Restricción del proyecto en estado **Abierta** o **En gestión** (módulo Restricciones). Está **vencida** si su fecha de compromiso ya pasó. |
| **Principal bloqueo** | El bloqueo que más pesa de cada proyecto: primero los vencidos, luego el de fecha de compromiso más cercana y, a igualdad, el más antiguo. |
| **Estado** | Clasificación del desempeño de cada proyecto: **Saludable**, **En riesgo**, **Crítico**, **Por iniciar** o **Sin información** (reglas abajo). |
| **Calidad del dato** | Qué tan reciente es el último avance registrado: **Actualizado**, **Desactualizado**, **Sin datos recientes** o **Sin información**. Se muestra **aparte** del estado. |
| **p.p. / M** | p.p. = puntos porcentuales. "$1.250 M" = 1.250 millones de pesos. |

---

## Cómo funciona

Nada se digita en el Centro de control: todo se calcula cada vez que se abre la página, con los mismos cálculos del **Dashboard de cada proyecto** (las dos pantallas siempre muestran las mismas cifras). Arriba a la derecha se ve la **fecha de corte** (hoy, hora Colombia) y la hora del cálculo.

### Estado de cada proyecto

Se evalúa en este orden; el primero que se cumple define el estado. El **Motivo** se ve al pasar el mouse sobre la etiqueta del estado y en el Dashboard del proyecto.

| Estado | Regla |
|---|---|
| **Sin información** | El proyecto no tiene cronograma cargado. No entra en los indicadores generales. |
| **Por iniciar** | La fecha de inicio del proyecto (o, si no la tiene, la primera actividad del cronograma) es **futura**. Si la fecha de inicio ya pasó y no hay avance, **no** es "por iniciar": se evalúa como atraso. |
| **Crítico** | El cronograma vigente termina **más de 30 días** después de la fecha contractual, **o** la brecha es de **−10 p.p. o peor**, **o** hay un **bloqueo vencido** y la brecha es de **−3 p.p. o peor**. |
| **En riesgo** | El cronograma vigente termina **después** de la fecha contractual (1 a 30 días), **o** la brecha es de **−3 p.p. o peor**. |
| **Saludable** | Avance y plazo dentro de las tolerancias. |

> ⚠ Por confirmar: estos umbrales (−3 p.p., −10 p.p., 30 días, bloqueo vencido con −3 p.p.) son una **propuesta pendiente de aprobación de gerencia**. La misma página lo indica en la nota al pie: *Criterios (propuestos, pendientes de aprobación)*. No los presente como política aprobada hasta que gerencia los confirme.

### Calidad del dato

Se mira la fecha del **último dato de avance**: el más reciente entre la última modificación de una actividad del cronograma vigente y el último Informe Diario.

| Calidad | Regla |
|---|---|
| **Actualizado** | Último dato de hace **7 días o menos**. |
| **Desactualizado** | Último dato de hace **8 a 21 días**. |
| **Sin datos recientes** | Último dato de hace **más de 21 días**, o ninguno. |
| **Sin información** | El proyecto no tiene cronograma. |

La calidad del dato **no cambia el estado**: un proyecto sin datos recientes no se presenta como saludable ni como crítico por esa razón, pero la etiqueta avisa que sus cifras pueden estar viejas.

> ⚠ Por confirmar: cualquier modificación de una actividad del cronograma (no solo el avance) cuenta como "dato de avance". Confirmar con control de proyectos si debe contar solo el registro de avance.

### Tarjetas

| Tarjeta | Qué muestra |
|---|---|
| **PROYECTOS** | Número de proyectos activos y cuántos están en ejecución (Saludable, En riesgo o Crítico), por iniciar y sin información. |
| **AVANCE GENERAL** | Avance real promedio del portafolio, con el programado y la **brecha** en p.p. Solo incluye los proyectos **en ejecución** con cronograma. El promedio se **pondera por presupuesto** (un proyecto grande pesa más); si alguno de esos proyectos no tiene presupuesto cargado, todos pesan igual y la ayuda lo dice. El ícono **?** explica qué proyectos entraron y cómo se ponderó. |
| **FECHAS LÍMITE** | Cuántos proyectos **terminarían tarde** (fecha proyectada después de la contractual), con el nombre y los días de cada uno (ej. *La Soberana +45 d*), y cuántos tienen entrega en **menos de 6 meses**. |
| **DESVIACIÓN DEL PRESUPUESTO** | Sobrecosto total estimado (en rojo si es mayor que cero), el % que representa sobre el presupuesto total y cuántos proyectos tienen presupuesto cargado. |
| **BLOQUEOS ABIERTOS** | Total de restricciones abiertas o en gestión y cuántas tienen la fecha de compromiso vencida. |

### Gráficas

- **ESTADO DE SALUD DEL PORTAFOLIO:** dona con el número de proyectos por estado (verde Saludable, amarillo En riesgo, rojo Crítico, azul claro Por iniciar, gris Sin información), el total en el centro y el % de cada porción. **Un clic en una porción o en su nombre de la leyenda filtra** la página por ese estado; otro clic en el mismo estado quita el filtro. La dona siempre muestra todo el portafolio.
- **PRESUPUESTO APROBADO VS COSTO ESTIMADO AL CIERRE:** por proyecto con presupuesto, tres barras: presupuesto aprobado (gris), ejecutado a la fecha (azul) y costo estimado al cierre (verde si queda dentro del presupuesto, rojo si hay sobrecosto). Clic en una barra abre el Dashboard de ese proyecto.
- **AVANCE REAL VS PROGRAMADO POR PROYECTO:** por proyecto con cronograma, el programado a hoy (gris) y el real, coloreado según el estado (verde saludable, amarillo en riesgo, rojo crítico). Al pasar el mouse se ve la brecha. **Clic en una barra abre el Dashboard del proyecto.**

Estas dos gráficas de barras y la tabla respetan el filtro de estado activo.

### Tabla de proyectos

Ordenada de lo más grave a lo menos grave (Crítico, En riesgo, Saludable, Por iniciar, Sin información y, dentro de cada estado, la peor brecha primero). **Clic en una fila abre el Dashboard del proyecto.**

| Columna | Contenido |
|---|---|
| **Proyecto** | Nombre, código y capacidad (kWp o MWp). |
| **Estado** | Etiqueta del estado; el motivo aparece al pasar el mouse. |
| **Dato** | Calidad del dato. |
| **Prog.** / **Real** / **Brecha** | Avance programado, real y brecha en p.p. ("Sin inf." si no hay cronograma; "—" en la brecha de un proyecto por iniciar). |
| **Fecha contractual** | Fecha fin de la ficha del proyecto. |
| **Plazo** | **A tiempo**, o en rojo **+N d** con la fecha proyectada debajo. |
| **Costo al cierre** | Costo estimado al cierre en millones de pesos y, si hay, el sobrecosto en rojo. "Sin inf." si no hay presupuesto. |
| **Principal bloqueo** | Descripción del principal bloqueo, su responsable y, si aplica, *vencido hace N d*. |
| **Último dato** | Fecha del último dato de avance. |

### Resumen ejecutivo

Texto armado con los datos, listo para copiar en un informe:

- Por cada proyecto **Crítico** o **En riesgo**: su estado y motivo, avance real frente a programado, sobrecosto estimado (o, si no tiene presupuesto, cuánto lleva pagado o comprometido *sin presupuesto cargado*), el principal bloqueo con su número, días de vencido y el **plan** registrado. Si no hay restricciones abiertas, dice que no hay restricciones que expliquen la desviación y que hay que registrarlas.
- Los proyectos **Saludables**, con un aviso si alguno no tiene datos de los últimos 7 días.
- Los **Por iniciar**, con su fecha de inicio, y los **Sin información**, que no entran en los indicadores generales.

Al final, una nota recuerda los criterios (propuestos) y cómo se calcula el costo al cierre.

---

## Paso a paso

### Revisar el portafolio en la reunión semanal

1. Entra a la app: el **Centro de control** es la primera página. Verifica la fecha de **Corte** arriba a la derecha.
2. Lee las cinco tarjetas: cuántos proyectos hay, avance general y brecha, fechas límite, desviación del presupuesto y bloqueos.
3. En la dona **ESTADO DE SALUD DEL PORTAFOLIO**, haz clic en **Crítico** (o **En riesgo**) para ver solo esos proyectos.
4. En la tabla, revisa el **Motivo** (pasando el mouse por el estado), el **Plazo**, el **Costo al cierre** y el **Principal bloqueo**.
5. Revisa la columna **Dato**: si dice **Desactualizado** o **Sin datos recientes**, pide al equipo del proyecto que actualice el avance antes de sacar conclusiones.
6. Haz clic en la fila (o en la barra de la gráfica) para abrir el **Dashboard del proyecto** y ver el detalle por actividad y por área.
7. Quita el filtro con **quitar** (junto a *Filtro activo*), eligiendo **Todos los estados** en la lista, o con otro clic sobre el mismo estado de la dona.

### Usar el resumen ejecutivo

1. Baja a **RESUMEN EJECUTIVO**.
2. Copia los párrafos de los proyectos críticos y en riesgo en el informe o el correo de gerencia.
3. Si un proyecto dice *No hay restricciones abiertas que expliquen la desviación*, pide al equipo que registre las restricciones en **Restricciones** del proyecto, con responsable, fecha de compromiso y plan.

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| *Calculando indicadores del portafolio...* tarda | La página recalcula todos los proyectos (cronograma, curva, costos y restricciones) al abrirse. | Espera unos segundos. |
| *No se pudieron calcular los indicadores del portafolio* | Error al leer los datos de algún proyecto. | Recarga la página; si persiste, repórtalo a la administración de la plataforma. |
| Un proyecto aparece **Sin información** | No tiene cronograma cargado. | Carga el cronograma en **Cronograma de Actividades** del proyecto. |
| Un proyecto que ya debía empezar aparece **En riesgo** o **Crítico** y no **Por iniciar** | Su fecha de inicio ya pasó y no tiene avance suficiente: eso es atraso. | Es lo esperado. Registra el avance real o reprograma si corresponde. |
| **Plazo** dice "Sin inf." | Falta la fecha fin contractual en la ficha del proyecto o el cronograma. | Complétala en la ficha del proyecto. |
| **Costo al cierre** dice "Sin inf." | El proyecto no tiene presupuesto cargado en Costos. | Carga el presupuesto (capítulo [5](05-costos.md)). |
| El resumen dice *pagados o comprometidos sin presupuesto cargado* | El proyecto tiene pagos, pero no presupuesto: no se puede hablar de sobrecosto. | Carga el presupuesto del proyecto. |
| La ayuda del **AVANCE GENERAL** dice *con el mismo peso* | Algún proyecto en ejecución no tiene presupuesto. | Carga el presupuesto de todos los proyectos para que el promedio se pondere. |
| El avance o el estado no coincide con lo que se ve en el Dashboard del proyecto | No debería pasar: ambas pantallas usan el mismo cálculo. | Recarga las dos páginas (F5) para comparar con el mismo corte. Si sigue distinto, repórtalo. |
| El **Dato** sale **Desactualizado** | No se ha registrado avance (Informe Diario o cronograma) en más de 7 días. | Pide la actualización al equipo del proyecto. |
| **Bloqueos abiertos** muestra restricciones que ya se resolvieron | No se marcaron como levantadas. | En **Restricciones** del proyecto pulsa **✓ Levantar**. |
| Los montos de un proyecto en dólares se ven raros | La TRM del proyecto está desactualizada. | Actualiza la TRM en Costos → Presupuesto. |

---

## Relación con otros módulos

- **Dashboard del proyecto** ([14](14-dashboard-proyecto.md)): mismo cálculo de estado, avance, plazo y costos; cada clic en un proyecto lleva allí.
- **Cronograma de Actividades y Restricciones** ([2](02-cronograma-actividades.md)): fechas, avance, versión vigente (fecha proyectada) y bloqueos.
- **Informe Diario** ([3](03-informe-diario.md)): avance real y fecha del último dato.
- **Costos** ([5](05-costos.md)): presupuesto, ejecutado y comprometido; el ejecutado se recalcula con el Seguimiento de pagos.
- **Proyectos** ([1](01-proyectos.md)): fechas de inicio y fin contractual, capacidad, estado (Cancelado / Completado salen del portafolio) y TRM.

---

## Buenas prácticas

- **Mantén al día los datos de cada proyecto** (avance semanal, Seguimiento de pagos, restricciones): el Centro de control solo es tan bueno como los datos que recibe.
- **Lee siempre la columna Dato** antes de juzgar un estado: un "Saludable" con dato desactualizado puede no serlo.
- **Registra las restricciones con fecha de compromiso, responsable y plan**: así aparecen como bloqueo, explican la desviación en el resumen ejecutivo y alertan cuando se vencen.
- **Completa la fecha contractual y el presupuesto** de todos los proyectos, para que el plazo, el sobrecosto y el promedio ponderado del portafolio sean correctos.
- **No presentes los umbrales de estado como política aprobada** hasta que gerencia los confirme.
- Para analizar un proyecto, **ve a su Dashboard**: el Centro de control muestra el panorama, el detalle está en cada proyecto.
