# 14. Dashboard del proyecto

> **Para qué sirve:** ver en una sola pantalla cómo va el proyecto: estado general, avance programado vs real a la fecha, brecha, índice de cumplimiento, plazo frente a la fecha contractual, actividades atrasadas y críticas, Curva S, avance por actividad de cualquier semana (pasada, actual o futura), presupuesto y costos, avance por disciplina y un plan de acción con las actividades que necesitan atención.
> **Quién lo usa:** dirección de proyecto, control de proyectos / planeación, gerencia, coordinadores de disciplina.
> **Dónde está:** menú lateral del proyecto → **Dashboard**, la tarjeta **Dashboard** en el detalle del proyecto, o un clic sobre el proyecto en el **Centro de control** (página de inicio) (`/proyectos/{id}/dashboard`).

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Actividad hoja** | Actividad del cronograma que no tiene sub-actividades. **Todos los cálculos usan solo actividades hoja** del cronograma vigente. |
| **Cronograma vigente** | La versión del cronograma marcada como vigente en el Cronograma de Actividades (WBS). |
| **Avance programado** | Porcentaje que debería llevar una actividad a la fecha de corte, según el tiempo transcurrido entre su inicio y su fin planeados (en línea recta). |
| **Avance real** | Avance registrado para la actividad en el Cronograma de Actividades y en los informes diarios cuando existen. |
| **Brecha de avance** | Avance real − avance programado, en **puntos porcentuales (p.p.)**: la resta directa de dos porcentajes. Negativa = atraso. Por ejemplo, real 40 % y programado 45 % = −5 p.p. |
| **Índice de cumplimiento** | Avance real ÷ avance programado. 1,00 = al día; menos de 1 = atraso; más de 1 = adelanto. Es un índice de **avance físico**: **no es el SPI de valor ganado**, porque el cronograma no trae costos por actividad. |
| **Estado del proyecto** | Saludable, En riesgo, Crítico, Por iniciar o Sin información. Es el mismo estado del **Centro de control** ([18](18-centro-de-control.md)), con las mismas reglas. |
| **Fecha contractual** | Fecha fin planeada registrada en la ficha del proyecto. |
| **Fecha proyectada** | Fecha fin del cronograma vigente (la última reprogramación aprobada). |
| **Curva S** | Gráfica del avance acumulado planificado vs real a lo largo del tiempo. |
| **Disciplina / Área** | Suministros, Ingeniería, Civil, Mecánica, Eléctrica, Puesta en marcha, Cierre de proyecto, etc., asignada a cada actividad **hoja** en el cronograma (las actividades con subactividades no llevan disciplina). En **Avance por actividad** se le llama *área*. |
| **Responsable del área** | Cargo y nombre registrados en **Documentos → Responsables** para ese proyecto. Hoy la app solo lo muestra para Civil, Mecánica y Eléctrica (las áreas de Documentos que corresponden a una disciplina del cronograma). |
| **CR / RC** | Marca de las actividades señaladas como críticas (ruta crítica) en el cronograma. |

---

## Cómo funciona

Nada se digita en el Dashboard (salvo las recomendaciones manuales, el histórico real y las categorías de la Curva S): **todo se calcula** con el cronograma vigente, los informes diarios y los costos del proyecto cada vez que se abre la página. El estado, el plazo y los costos salen del **mismo cálculo del Centro de control**, así que las dos pantallas siempre dicen lo mismo.

### Encabezado

- Bajo el título se ve el proyecto, la **fecha de corte**, la **versión del cronograma vigente** y la fecha del **último dato de avance**.
- A la derecha, la etiqueta del **estado del proyecto** (Saludable, En riesgo, Crítico, Por iniciar o Sin información) y debajo el **Motivo** (por ejemplo *brecha de avance de −4,2 p.p.* o *el cronograma vigente termina 45 días después de la fecha contractual*). Las reglas están en el capítulo [18](18-centro-de-control.md).

> ⚠ Por confirmar: los umbrales del estado (−3 p.p., −10 p.p., 30 días de retraso, bloqueo vencido) son una **propuesta pendiente de aprobación de gerencia**.

### Indicadores generales

**Primera fila (avance):**

- **AVANCE PROGRAMADO** y **AVANCE REAL**: avance de todas las actividades hoja del cronograma vigente **ponderado por la duración planeada de cada una** y por las **categorías con peso** de la Curva S (los hitos no pesan). Son los mismos valores del último punto de la Curva S con alcance "Todo el proyecto" y los mismos del Centro de control.
- **BRECHA DE AVANCE**: avance real − avance programado, en p.p. Debajo dice, por ejemplo, *vamos 4,2 puntos atrás* o *al día o adelantados*. El ícono **?** muestra la explicación con los números del proyecto.
- **ÍNDICE DE CUMPLIMIENTO**: avance real ÷ avance programado, con la frase *de cada 100 programado, hicimos N*. Si aún no hay avance programado muestra **No aplica**. El ícono **?** aclara que no es el SPI de valor ganado.
- Colores del avance real y la brecha: verde si la brecha es 0 o positiva; naranja entre 0 y −5 p.p.; rojo por debajo de −5 p.p.
- Colores del índice: verde desde 1,00; naranja de 0,90 a 0,99; rojo por debajo de 0,90.

**Segunda fila (actividades y plazo):**

- **ACT. ATRASADAS** (de cuántas en total), **ACT. CRÍTICAS** (brecha menor a −15 p.p.) y **ACT. FINALIZADAS** (avance de 100 %).
- **PLAZO**: *fecha contractual → fecha proyectada* y, en rojo, *+N días calendario sobre la fecha contractual*, o en verde *dentro de la fecha contractual*. Si el proyecto no tiene fecha fin contractual en su ficha, muestra **Sin información**.

### Clasificación de cada actividad

Se aplica en este orden (en las tarjetas, la dona, la lista de más atrasadas y el plan de acción):

| Estado | Regla |
|---|---|
| **Finalizada** | Avance real de 100 % o más |
| **Sin iniciar** | Avance real 0 % |
| **Crítica** | Brecha menor a −15 p.p. |
| **Atrasada** | Brecha menor a −5 p.p. y de hasta −15 |
| **En línea** | Cualquier otro caso |

> Importante: una actividad con 0 % de avance real cuenta como **Sin iniciar** aunque ya debería ir adelantada; no aparece como atrasada ni crítica. Para verlas, usa **Avance por actividad**: allí el conteo de sin iniciar dice cuántas *ya debían ir más de 5 %*.

### Curva S

- **Alcance:** el selector junto al título permite ver **Todo el proyecto** (todas las actividades hoja del cronograma vigente: contractual, ingeniería, suministros, construcción, puesta en marcha y cierre) o **Solo construcción** (únicamente las actividades hoja con disciplina Civil, Mecánica, Eléctrica o Construcción). El segundo alcance es el comparable con la curva de "Avance de construcción" del informe interno.
- **Categorías y pesos (botón junto a Actualizar):** la curva total es la **suma ponderada** de la curva de cada categoría, igual que en el informe interno de La Soberana: Suministro principal 25 %, Ingeniería del proyecto 10 %, Construcción de la planta 50 %, EPC Línea de transmisión y pruebas 15 %. Cada categoría se define con los **códigos WBS** de las ramas que incluye (separados por coma; una rama incluye todas sus subactividades) y los que excluye. Cada actividad hoja cae en la primera categoría que la incluye; las que no caen en ninguna **no entran** en la curva y la nota bajo la gráfica lo avisa. La casilla **Construcción** marca las categorías que forman el alcance "Solo construcción". **Contar actividades** muestra cuántas hojas caen en cada categoría antes de guardar. Si los pesos no suman 100 se normalizan y se avisa. Sin categorías configuradas, la curva pondera todas las hojas solo por duración.
- **Mapeo definido para La Soberana (26/09/2026):** Suministro = 2.6, 2.7 · Ingeniería = 2.4, 2.5 · Construcción = 2.8 y 2.9 (menos 2.8.7, 2.8.9, 2.8.10) más los hitos 1, 2.1, 2.2, 2.3 · Línea y pruebas = 2.8.7, 2.8.9, 2.8.10, 2.10, 2.11, 2.12, 2.13, 2.14, 2.15, 2.16 (los hitos finales van aquí por decisión de la usuaria, 28/09/2026).
- **Ponderación dentro de cada categoría:** cada actividad hoja pesa según su **duración planeada en días** (equivale al "Ponderado Actividad" del Excel interno). Los **hitos** (duración 0) no aportan peso; si una categoría solo tiene hitos, pesan igual. Todavía no se pondera por costo ni por horas-hombre.
- **Línea planificada:** un punto por semana desde el inicio más temprano hasta el fin más tardío de las actividades hoja (más el fin del proyecto, la fecha de hoy y la del primer informe). Cada punto es el avance lineal esperado de cada actividad a esa fecha, ponderado.
- **Línea real, en tres fuentes (de más a menos prioritaria por fecha):**
  1. **Histórico cargado** (opcional): una serie de % acumulado real de otra fuente (por ejemplo la curva S de un informe interno) pegada con el botón **Histórico real**. Solo se usa para las fechas que cubre.
  2. **Informes diarios**: desde el primer informe diario posterior al histórico, para cada fecha se toma, por actividad, el último avance acumulado informado y se pondera igual que el planificado. Si un informe fue registrado sobre una versión anterior del cronograma, la app lo cruza con la actividad vigente del mismo código WBS.
  3. **Avance del cronograma** (la fuente normal): el avance real registrado en cada actividad del Cronograma de Actividades, repartido desde su fecha real de inicio (o la planeada si no tiene) hasta hoy, con la misma ponderación. No hace falta cargar nada: la curva se alimenta del cronograma y de los informes diarios que existan.
- **Línea de Hoy y clic en la curva:** una línea roja vertical marca **Hoy**. Al hacer clic en cualquier punto de la curva aparece una línea azul punteada **Consultada** y la sección **Avance por actividad** salta a esa semana.
- **Histórico real (botón junto a Actualizar, opcional):** abre una ventana para pegar desde Excel dos columnas, **fecha** y **% real acumulado** (una fila por línea). Acepta fechas `09/02/2026`, `9-feb-26`, `2026-02-09` o seriales de Excel, y porcentajes `41,5%`, `41.5` o `0,415` (si todos los valores son ≤ 1 se toman como fracciones). Muestra una vista previa con puntos válidos, rango de fechas, último valor y filas ignoradas (encabezados se ignoran sin error), y avisa si algún valor baja respecto al anterior. Elige el **alcance** al que pertenece la serie (Todo el proyecto o Solo construcción); al **Guardar histórico** se reemplaza el histórico anterior de ese alcance. **Borrar histórico de este alcance** elimina la serie. El botón muestra en verde cuántos puntos hay cargados.
- Debajo de la gráfica, una nota explica cómo se calculan el planificado y el real (categorías, número de actividades hoja, hitos sin peso y período del histórico cargado).
- **Corte a hoy** (fecha de Colombia, UTC−5) muestra dos barras: **Avance Ejecutado** y **Avance Planeado**, que son exactamente el último punto de cada línea de la curva. Con el alcance "Todo el proyecto" coinciden con las tarjetas **AVANCE REAL** y **AVANCE PROGRAMADO**.

> Nota: la curva del informe interno se alimenta de otra fuente (cronograma en Excel/Project con su propio histórico), por lo que puede diferir de la de la app en las semanas anteriores al primer informe diario registrado.

### Avance por actividad

Sección debajo de la Curva S para ver **cómo iba, cómo va o cómo debería ir cada actividad en cualquier semana**, con la misma lógica de la curva.

- **Selector de semana:** flechas **‹ ›**, una barra deslizante y el botón **Hoy**. Muestra *Semana del dd/mm/aa al dd/mm/aa* y una etiqueta: **Semana actual · hoy dd/mm/aaaa**, **Pasada · hace N semanas** o **Futura · faltan N semanas**. Las semanas van de lunes a domingo; la semana actual se calcula a hoy y las demás a su domingo. También se cambia de semana con un clic en la Curva S.
- **Filtro por área:** un solo filtro con botones **Todas las áreas** y una por cada disciplina que tenga actividades (en el orden del Dashboard). Al elegir un área se muestra **Responsable del área** (cargo – nombre). Si el área no tiene responsable registrado dice *responsable no asignado*.
- **Resumen de la semana:** *Programado N % · Real N %*. Con **Todas las áreas** son los mismos valores de la Curva S a esa fecha; con un área, el promedio ponderado por duración de sus actividades. Además: terminadas, en ejecución, sin iniciar (y cuántas *ya debían ir más de 5 %*) y atrasadas o críticas. En una **semana futura** solo hay programado: *el avance real aún no existe*.
- **Tabla de actividades** (agrupada por área con su responsable cuando se ven todas): **Código**, **Actividad** (con la marca **RC** si es de ruta crítica), **Prog.**, **Real** (con minibarra), **Brecha** en p.p. y **Estado**:
  - Con avance real: **Terminada**, **Crítica** (brecha menor a −15 p.p.), **Atrasada** (menor a −5 p.p.), **Al día** o **Sin iniciar**.
  - En semanas futuras (sin real): **Debe terminar**, **Debe ir N %** o **No programada**.
- **Línea de tiempo** en la última columna: barra gris = duración planeada, verde = avance real a hoy, fondo sombreado = tiempo transcurrido, línea roja = **Hoy**, línea azul punteada = semana consultada. Arriba se ven los meses.
- **Cumplimiento por área (a hoy):** por área y responsable, cuántas actividades están **Pendientes**, **Atrasadas** (más de 5 p.p. por debajo de lo programado) y **Terminadas**. Siempre se calcula a hoy, aunque se esté viendo otra semana.
- **Próximas a iniciar en N días:** lista de actividades que empiezan entre mañana y N días (por defecto 30; el número se puede cambiar), con código, actividad, fecha de inicio y área.

### Presupuesto y costos

Tarjeta con los costos del proyecto, con las mismas cifras del Centro de control. Montos en **millones de pesos**; lo que está en dólares se convierte con la TRM del proyecto. El botón **Ver costos** lleva al módulo Costos.

- **PRESUPUESTO APROBADO**, **EJECUTADO (PAGADO)** (con el % del presupuesto), **COMPROMETIDO (POR PAGAR)** (órdenes de compra desde esta semana) y **COSTO ESTIMADO AL CIERRE** (en rojo con *+N sobre el presupuesto*, o en verde *dentro del presupuesto*).
- **Avance físico real vs presupuesto consumido:** dos barras. Si lo pagado supera el avance físico en más de 5 puntos, avisa *Se ha pagado más (N %) de lo que se ha avanzado (N %). Revisar si hay anticipos o sobrecostos.*; si no, *El gasto va en línea con el avance físico.*
- **Tabla por categoría** (partida principal del presupuesto): Presupuesto, Ejecutado, Comprometido, Sobrecosto y **Consumo** = (ejecutado + comprometido) ÷ presupuesto, en verde hasta 85 %, amarillo hasta 100 % y rojo por encima.
- **Costo estimado al cierre** = presupuesto + lo que el ejecutado y el comprometido de cada código superan su presupuesto. La nota al pie indica la fecha de corte del Seguimiento de pagos usado.
- Si el proyecto no tiene presupuesto cargado, la tarjeta lo dice y lleva a **Costos**.

### Gráficas y tablas

- **Avance por disciplina — Real vs Programado:** barras en verde (real) y azul (programado) de RenergeIA, siempre en este orden y solo para estas disciplinas: **Suministros, Ingeniería, Civil, Mecánica, Eléctrica, Puesta en marcha y Cierre de proyecto** (definido el 2026-09-26). Contractual, General y Construcción no se grafican. Una disciplina sin actividades hoja no aparece.
- **Estado de actividades:** dona con En Línea, Atrasadas, Críticas, Finalizadas y Sin iniciar.
- **Actividades más atrasadas:** hasta 10 actividades con brecha negativa, de la peor a la menos mala.
- **Plan de acción — Actividades críticas y atrasadas:** todas las críticas y atrasadas con código, actividad, disciplina, programado, real, desviación (brecha), estado y una **Recomendación**. La recomendación se arma con datos reales de cada actividad, en este orden, y se puede **corregir a mano**:
  1. **Situación frente al cronograma:** si venció, la fecha de fin, los días vencidos y el % que falta; si no ha vencido, los días que quedan y el ritmo semanal necesario para cerrar a tiempo; si no ha iniciado, desde cuándo debió empezar.
  2. **Restricciones abiertas** (Abierta o En gestión) ligadas a la actividad desde el Informe Diario: número, tipo, descripción, responsable y fecha de compromiso (avisa si el compromiso ya venció). Si no hay ninguna: *Sin restricción registrada: identificar la causa del atraso y registrarla en Restricciones.*
  3. **Última observación** escrita en el Informe Diario para esa actividad, con su fecha; si no hay observaciones pero el último informe tiene 7 días o más, avisa que falta actualizar el avance; si nunca ha tenido informe, aclara que el avance es el digitado en el cronograma.
  4. Si la actividad está marcada como **crítica (CR)**, recuerda que el atraso corre la fecha fin del proyecto.
  - **Corregir a mano:** el lápiz ✎ de cada fila abre un cuadro de texto. Al **Guardar**, esa recomendación reemplaza a la automática, queda guardada en la actividad (con quién y cuándo, etiqueta **Manual**) y se conserva en las reprogramaciones. **Volver a la automática** la borra. Mientras se edita, la automática se muestra debajo como referencia.
- **Detalle por disciplina:** mismas disciplinas y orden que la gráfica; programado, real, desviación (verde ≥ 0, amarillo hasta −5, rojo menor a −5), actividades atrasadas (incluye críticas) y total.

---

## Paso a paso

### Revisar el estado del proyecto

1. Entra a **Dashboard** del proyecto (o haz clic en el proyecto desde el Centro de control). Arriba ves la fecha de corte, la versión del cronograma, el último dato de avance y la etiqueta del estado con su **Motivo**.
2. Lee la primera fila de tarjetas: **AVANCE PROGRAMADO**, **AVANCE REAL**, **BRECHA DE AVANCE** y **ÍNDICE DE CUMPLIMIENTO**. Usa el ícono **?** si necesitas la explicación.
3. Lee la segunda fila: **ACT. ATRASADAS**, **ACT. CRÍTICAS**, **ACT. FINALIZADAS** y **PLAZO** (fecha contractual frente al fin del cronograma vigente).

### Analizar la Curva S

1. Ubica la tarjeta **Curva S — Avance real vs planificado**.
2. Elige el **Alcance** (Todo el proyecto o Solo construcción) y compara la línea real con la planificada: si la real está por debajo, el proyecto va atrasado.
3. Si necesitas el histórico de otra fuente, pulsa **Histórico real** y pega desde Excel la serie de fecha y % real acumulado de tu informe interno; guarda y la curva pasa a dibujar esos datos.
4. Revisa las barras de **Corte a hoy**.
5. Si acabas de registrar informes diarios o cambiar el cronograma, pulsa **Actualizar** (junto al título). Aparece *Actualizada* por unos segundos.

> Nota: **Actualizar** recalcula los datos y vuelve a dibujar la Curva S y las tarjetas. Para refrescar también las demás gráficas, recarga la página (F5).

### Ver el avance de las actividades en una semana

1. Haz clic en la Curva S sobre la fecha que te interesa, o baja a **Avance por actividad** y usa las flechas, la barra deslizante o **Hoy**.
2. Elige un área (por ejemplo **Civil**) para ver solo sus actividades y su **Responsable del área**; **Todas las áreas** vuelve a la vista completa.
3. Revisa el resumen (programado, real, terminadas, en ejecución, sin iniciar, atrasadas o críticas) y la columna **Estado**.
4. Para planear, mueve la semana hacia adelante: verás qué debería estar terminado (**Debe terminar**) o en qué % debería ir cada actividad (**Debe ir N %**).
5. Consulta **Cumplimiento por área (a hoy)** y **Próximas a iniciar en N días** (cambia el número de días si necesitas otra ventana).

### Revisar presupuesto y costos

1. Baja a **Presupuesto y costos**.
2. Compara **PRESUPUESTO APROBADO** con **COSTO ESTIMADO AL CIERRE**. Si está en rojo, hay códigos cuyo pagado + comprometido supera su presupuesto.
3. Compara las barras de **Avance físico real** y **Presupuesto consumido**, y lee el aviso.
4. En la tabla, busca las categorías con **Consumo** en amarillo o rojo.
5. Para el detalle, pulsa **Ver costos**.

### Identificar dónde está el atraso

1. En **Avance por disciplina** y **Detalle por disciplina**, ubica las disciplinas en rojo.
2. En **Actividades más atrasadas**, identifica las de mayor brecha.
3. En **Plan de acción**, revisa las actividades con estado **Crítica** (y las marcadas **CR**). Lee la recomendación automática y, si el equipo define otra acción, pulsa ✎ y escríbela: eso es lo que verá gerencia.
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
| *El cronograma vigente no tiene actividades hoja con fechas planeadas* en la Curva S | No hay cronograma vigente o sus actividades no tienen fechas. | Carga o activa un cronograma en Cronograma de Actividades. |
| La Curva S no aparece al abrir la página | La gráfica se dibuja cuando la página termina de conectarse; la app reintenta sola varias veces. | Espera unos segundos; si sigue en blanco, recarga la página (F5). |
| La Curva S quedaba en blanco tras guardar categorías | Las categorías se guardaron sin códigos WBS. | Ya no se permite guardar una categoría sin "Incluye"; si las categorías no coinciden con el cronograma, la curva se dibuja sin categorías y lo avisa. |
| Al pulsar **Actualizar** la página se queda bloqueada o se reconecta | La conexión de la página se cortaba cada 5 minutos en el servidor. | Corregido el 28/09/2026. Si vuelve a pasar, recarga la página (F5). |
| *Ninguna actividad hoja… tiene disciplina Civil, Mecánica, Eléctrica o Construcción* en la Curva S | El alcance "Solo construcción" no encontró actividades con esas disciplinas. | Asigna la disciplina a las actividades en el Cronograma o vuelve a "Todo el proyecto". |
| *No hay actividades hoja con disciplina Suministros, Ingeniería…* | Ninguna actividad hoja tiene una de las siete disciplinas que se grafican (Contractual, General y Construcción no cuentan). | Asigna la disciplina a las actividades en el cronograma. |
| El avance real está en 0 % aunque se trabaja en obra | No se ha actualizado el avance en el cronograma ni en el Informe Diario, o se registró sobre otra versión del cronograma. | Registra el avance en **Informe Diario** o en el cronograma y verifica que la versión vigente sea la correcta. |
| Hay muchas actividades **Sin iniciar** y pocas atrasadas, pero el proyecto se siente atrasado | Las actividades con 0 % no se clasifican como atrasadas (ver *Cómo funciona*). | Revisa en **Avance por actividad** cuántas *ya debían ir más de 5 %*. |
| Los datos no cambiaron después de registrar un informe | La página muestra lo calculado al abrirla. | Pulsa **Actualizar** o recarga la página. |
| El índice de cumplimiento muestra **No aplica** | A la fecha de corte todavía no hay avance programado (el proyecto no ha empezado según el cronograma). | Es esperado; el índice aparece cuando haya avance programado. |
| El índice y el avance no coinciden con el cálculo de planeación | La app pondera por **duración** de las actividades y por las **categorías con peso** de la Curva S; no pondera por costo ni por horas-hombre (el cronograma importado no los trae). | Revisa las **Categorías y pesos**; si la diferencia persiste, valida con planeación qué ponderación usa el informe interno. |
| **PLAZO** dice **Sin información** | La ficha del proyecto no tiene fecha fin contractual. | Regístrala en la ficha del proyecto. |
| En **Avance por actividad** dice *responsable no asignado* | El área no tiene responsable en Documentos, o es un área sin responsable (solo Civil, Mecánica y Eléctrica lo tienen). | Regístralo en **Documentos → Responsables** del proyecto. |
| En una semana futura la columna **Real** sale con "—" | El avance real de una fecha futura no existe. | Es esperado: la vista futura muestra lo que debería ir (**Debe ir N %**). |
| Un grupo dice **Sin disciplina** en Avance por actividad | Hay actividades hoja sin disciplina asignada. | Asígnala en el Cronograma (o usa **Completar disciplinas**). |
| **Presupuesto y costos** dice que el proyecto no tiene presupuesto | No se ha cargado el presupuesto en Costos. | Cárgalo en **Costos** (ver capítulo [5](05-costos.md)). |
| Aviso *Se ha pagado más … de lo que se ha avanzado* | Hay anticipos, pagos adelantados o sobrecostos. | Revisa en **Costos** los códigos con mayor consumo. |
| La recomendación dice *Sin restricción registrada* pero sí hay una restricción | La restricción no está ligada a la actividad: solo se cruzan las que se marcan en el **Informe Diario** al reportar esa actividad. | Registra el avance de la actividad en el Informe Diario marcando la restricción, o escribe la recomendación a mano con ✎. |
| Quiero volver a la recomendación automática | Alguien la corrigió a mano (etiqueta **Manual**). | Pulsa ✎ y luego **Volver a la automática**. |
| Las gráficas de disciplinas o estados no cambiaron tras **Actualizar** | **Actualizar** solo vuelve a dibujar la Curva S y las tarjetas. | Recarga la página (F5). |
| Tras una reprogramación, los números cambiaron mucho | El Dashboard usa siempre la **versión vigente** del cronograma, y la fecha proyectada es el fin de esa versión. | Es esperado. Revisa en el Cronograma cuál versión está activa como vigente. |

---

## Relación con otros módulos

- **Cronograma de Actividades (WBS)** ([2](02-cronograma-actividades.md)): de aquí salen las actividades hoja, sus fechas planeadas, su avance, disciplina, marca de crítica, la versión vigente y la fecha proyectada de fin.
- **Informe Diario** ([3](03-informe-diario.md)): avance real acumulado de cada actividad, observaciones y restricciones ligadas.
- **Documentos** ([4](04-documentos.md)): los **Responsables por área** que se muestran en Avance por actividad.
- **Costos** ([5](05-costos.md)): presupuesto, ejecutado y comprometido de la tarjeta **Presupuesto y costos**.
- **Restricciones:** alimentan las recomendaciones del plan de acción y los bloqueos que pesan en el estado del proyecto.
- **Centro de control** ([18](18-centro-de-control.md)): usa exactamente los mismos indicadores de este Dashboard para todos los proyectos; un clic en un proyecto abre este Dashboard.
- **Clima** ([13](13-clima.md)): usa el mismo cronograma vigente para las alertas operacionales de los próximos 3 días.

> Los indicadores de HSEQ están en sus propios dashboards.

---

## Buenas prácticas

- Registra el **avance todos los días** (Informe Diario o cronograma): sin avance reportado, el Dashboard muestra atraso aunque se esté trabajando, y el dato aparece desactualizado en el Centro de control.
- Mantén una sola versión **vigente** del cronograma y crea reprogramaciones solo cuando estén aprobadas: la fecha proyectada sale de esa versión.
- Asigna **disciplina** a todas las actividades hoja, para que la vista por disciplina y por área sea completa.
- Mantén al día los **Responsables por área** en Documentos.
- Revisa el **Plan de acción** y el **Cumplimiento por área** en la reunión semanal y registra las causas del atraso en **Restricciones**.
- Usa el **índice de cumplimiento** como alerta temprana: por debajo de **0,90** conviene tomar acciones. Recuerda que es un índice de avance físico, no de valor ganado.
- Antes de presentar el Dashboard, pulsa **Actualizar** o recarga la página para tener los datos al momento.
