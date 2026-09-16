# 2. Cronograma de Actividades (WBS) y Restricciones

> **Para qué sirve:** arma y controla el cronograma del proyecto como una estructura de desglose de trabajo (WBS): fechas planeadas, disciplina, avance real frente al planificado y estado de cada actividad. Guarda versiones (inicial y reprogramaciones) para no perder la línea base. El submódulo **Restricciones** registra los impedimentos que frenan la ejecución y su plan para levantarlos.
> **Quién lo usa:** control de proyectos y planeación (carga y reprogramaciones), ingenieros residentes (fechas, disciplinas y seguimiento) y gerencia (consulta).
> **Dónde está:**
> - Menú lateral → proyecto → **Cronograma de Actividades** → `/proyectos/{id}/wbs`
> - Nueva actividad: `/proyectos/{id}/wbs/nueva` · Editar actividad: `/proyectos/{id}/wbs/{actividad}/editar`
> - Menú lateral → proyecto → **Restricciones** → `/proyectos/{id}/restricciones`

---

## Conceptos clave

- **Actividad WBS:** una línea del cronograma con **código WBS** (1, 1.2, 1.2.3…), nombre, fechas de inicio y fin planeadas, disciplina y avance.
- **Nivel:** la profundidad de la actividad en el árbol. El **nivel 1 es un título de sección**: se muestra en gris, en mayúsculas y sin avance, fechas, disciplina ni estado.
- **Actividad hoja:** actividad sin subactividades. Es la única en la que se registra avance real.
- **Actividad padre:** tiene subactividades. Su avance real es el **promedio automático** de sus subactividades activas (se ve con una flechita ↑).
- **Avance Planificado:** porcentaje que *debería* llevar la actividad hoy según sus fechas: 0 % antes del inicio, 100 % desde la fecha de fin y proporcional a los días transcurridos en medio.
- **Avance Real:** porcentaje ejecutado de verdad. Normalmente llega desde el **Informe Diario**.
- **Desviación:** Avance Real − Avance Planificado. En verde (+) si va adelantada o al día; en rojo (−) si va atrasada.
- **Estado operativo** (lo calcula la app):

  | Estado | Cuándo |
  |---|---|
  | Pendiente | Avance real 0 % y todavía no llega la fecha de inicio |
  | En Progreso | Desviación de −5 % o mejor |
  | Atrasada | Desviación entre −5 % y −15 % |
  | Crítica | Desviación peor que −15 % |
  | Finalizada | Avance real de 100 % |
  | Sin Fechas | La actividad no tiene fechas |

- **Disciplina:** Mecánica, Civil, Eléctrica, Contractual, Hot Commissioning, Dossier, General o Suministros.
- **Activa / Inactiva:** una actividad inactiva queda fuera del conteo, del promedio de su padre y del Informe Diario, pero no se borra.
- **Versión del cronograma:** foto completa del cronograma. La primera se llama **Actividades Inicial**; las siguientes, **Actividades Reprogramación N** o **Actividades Plantilla EPC N**. Solo una es **Vigente** (★); las demás son **Históricas** y solo se pueden consultar.
- **Plantilla EPC:** cronograma estándar de un proyecto EPC fotovoltaico que trae la app (Timing Template de MS Project). Se ubica automáticamente desde la fecha de inicio planeada del proyecto.
- **Restricción:** impedimento (meteorológico, administrativo, técnico, financiero o de permisos) que frena el avance. Tiene responsable, fecha de compromiso, impacto y plan de acción.

---

## Cómo funciona

### Qué calcula la app sola
- **Avance Planificado, Desviación y Estado** de cada actividad, todos los días, con la fecha de hoy.
- **Avance real de las actividades padre:** promedio de sus hijas activas. Se recalcula al cambiar un avance y al abrir la versión vigente.
- **Tarjetas de resumen** (sin contar los títulos de nivel 1): **Activas**, **Completadas** (Finalizadas), **En progreso** y **Con problemas** (Atrasadas + Críticas).
- **Códigos WBS al reordenar:** al mover una actividad con ▲ ▼ se renumeran ella, sus hermanas y todas sus subactividades.
- **Plantilla EPC:** toma el cronograma estándar, omite la tarea raíz y corre todas las fechas para que empiece en la **Fecha inicio planeada** del proyecto. Todos los avances quedan en 0 %.
- **Importación de Project (.mpp):** conserva la jerarquía, los códigos (número de esquema), las fechas de inicio y fin y el **% completado** de cada tarea.
- **Protección del historial:** si una versión ya tiene avances del Informe Diario, **Reiniciar plantilla** no la borra; crea una versión nueva y deja la anterior como Histórica.
- **Autorreparación:** si ninguna versión queda marcada como vigente, al abrir la pantalla la app marca como vigente la más reciente.

### De dónde salen los datos
- **Estructura y fechas:** plantilla EPC, archivo de MS Project (.mpp), Excel, PDF (leído con IA) o creación manual.
- **Avance real:** principalmente del **Informe Diario** (cada informe actualiza el avance de las actividades). También se puede digitar directamente en la tabla del cronograma vigente.

### Qué se digita y qué no
| Se digita | No se digita |
|---|---|
| Fechas de inicio y fin planeadas, disciplina, avance real de actividades hoja (si no se usa el Informe Diario), activar/desactivar, orden, datos de la actividad en el formulario | Avance Planificado, Desviación, Estado, avance de las actividades padre, tarjetas de resumen, códigos al reordenar |

### Reglas importantes
- **Solo la versión vigente se edita.** En una histórica todos los campos están bloqueados y aparece 🔒 "Esta es una versión histórica del cronograma. Solo consulta".
- **Los botones de importar (Excel, PDF, .mpp) y Cargar plantilla EPC solo aparecen cuando el proyecto no tiene actividades ni versiones.** Para cambiar un cronograma ya cargado se usa **+ Crear reprogramación** (con o sin archivo .mpp).
- **Nunca se duplica "Actividades Inicial":** si ya existe, la carga inicial desde .mpp la reutiliza.
- **Una fecha de inicio posterior a la de fin (o una fin anterior al inicio) no se guarda** en la tabla.
- **El avance real se limita a 0–100 %**, con un decimal.
- **El Informe Diario usa solo las actividades activas de la versión vigente.**

---

## Paso a paso

### Cargar el cronograma por primera vez con la plantilla EPC
1. Verifica que la **Fecha inicio planeada** del proyecto sea correcta (capítulo 1), porque la plantilla arranca en esa fecha.
2. Entra a **Cronograma de Actividades**. Verás "No hay actividades aún".
3. Pulsa **Cargar plantilla EPC** y espera a que termine.
4. Se crea la versión **Actividades Inicial** (vigente) con las actividades estándar, agrupadas bajo títulos de nivel 1.
5. Ajusta fechas y disciplinas según tu proyecto y desactiva (⏸) las actividades que no apliquen.

### Cargar el cronograma inicial desde MS Project (.mpp)
1. En la pantalla vacía pulsa **Cronograma Project (.mpp)** y elige el archivo (máximo 60 MB).
2. Espera el mensaje verde "Cronograma cargado desde Project: N actividades."
3. Revisa la estructura, las fechas y el avance importados. El % completado de Project entra como avance real.

### Importar el cronograma desde Excel
1. Prepara el Excel. La app lee **la primera hoja**; la **fila 1 debe tener los encabezados** y los datos empiezan en la fila 2. Columnas reconocidas:
   - **Código** (también "WBS" o "ID"): código WBS (1, 1.1, 1.1.1…). Si falta, la app numera 1.1, 1.2…
   - **Nombre** (también "Tarea" o "Descripción"): **obligatoria**.
   - **Duración** (también "Días" o "Duration"): en días. Si falta, se toma 1 día.
   - **Nivel** (también "Level"): si falta, el nivel sale del código (1 = nivel 1, 1.1 = nivel 2…).
2. En la pantalla vacía pulsa **Cronograma Excel** y elige el archivo (.xlsx o .xls, máximo 10 MB).
3. En la ventana **Actividades detectadas en el Excel** revisa la tabla (Código, Actividad, Nivel, Duración (días)).
4. Pulsa **Importar N actividades** (o **Cancelar**).
5. Revisa y ajusta las fechas: las actividades de nivel 1 se encadenan una detrás de otra desde la fecha de inicio del proyecto según su duración, y las de los demás niveles arrancan en la fecha de inicio del proyecto con su propia duración.

> ⚠ Por confirmar: la app identifica la columna de código buscando, entre otras, las letras "id" dentro del encabezado. Por eso un encabezado como **"Actividad"** o **"Nombre de la actividad"** se toma como columna de código y aparece el error "No se encontró una columna de 'Actividad' o 'Nombre'…". Mientras se corrige, titula la columna del nombre **"Nombre"** o **"Tarea"**.

### Importar el cronograma desde PDF (con IA)
1. En la pantalla vacía pulsa **Cronograma PDF** y elige el archivo (máximo 10 MB). Debe tener texto seleccionable, no ser una imagen escaneada.
2. Espera el aviso "Analizando cronograma… Extrayendo actividades del PDF con IA".
3. En **Actividades detectadas en el PDF** revisa con cuidado códigos, niveles y duraciones: la IA puede equivocarse.
4. Pulsa **Importar N actividades**. Las fechas se calculan igual que en la importación desde Excel.

> ⚠ Por confirmar: la lectura de PDF depende del servicio de IA Gemini. Si la facturación del servicio no está habilitada, la importación falla con un error de Gemini (por ejemplo, 429). En ese caso usa Excel o .mpp.

### Crear una actividad manualmente
1. Pulsa **+ Nueva actividad** (solo disponible en la versión vigente).
2. Completa **Código WBS*** (máx. 20 caracteres, ej. 1.8.2.3), **Nombre***, **Actividad padre** (o "— Sin padre (nivel raíz) —"), **Nivel WBS** (1 a 10, debe coincidir con el código), **Responsable**, **Inicio planeado*** y **Fin planeado***.
3. Opcional: **Disciplina**, **Estado** y la casilla **Actividad activa en el proyecto**. Los controles deslizantes de avance normalmente se dejan en 0.
4. Pulsa **Crear actividad**. La actividad aparece en la tabla ubicada según su código.

> La columna **Estado** de la tabla se calcula sola con las fechas y el avance; el estado que se elige en el formulario no la cambia.

### Editar fechas, disciplina y avance en la tabla
1. Abre la versión **Vigente**.
2. Cambia **Inicio Plan.** o **Fin Plan.** con el calendario. El inicio no puede quedar después del fin.
3. Elige la **Disciplina** en la lista.
4. En actividades hoja, escribe el **Av. Real** (0 a 100). El avance del padre se recalcula solo.
5. Cada cambio se guarda al salir del campo. Si quieres asegurarte, pulsa **Guardar cambios** y espera "✓ Cambios guardados".
6. Para ajustar el ancho de las columnas, arrastra el borde derecho del encabezado. El ancho queda recordado en tu navegador.

### Editar o eliminar una actividad desde el formulario
1. En la fila, pulsa **Editar**.
2. Modifica los datos y pulsa **Guardar cambios**.
3. Para borrarla, pulsa **Eliminar actividad** y confirma con **Sí, eliminar**.

> ⚠ Por confirmar: eliminar una actividad que tiene subactividades o avances del Informe Diario puede ser rechazado por la base de datos y mostrar un error general. En esos casos es preferible **desactivarla** (⏸).

### Activar o desactivar actividades
1. Pulsa **⏸** en la fila para desactivar la actividad, o **▶** para activarla.
2. Usa las pestañas **● Activas (N)** y **○ Inactivas (N)** para ver cada grupo.

### Reordenar actividades
1. Pulsa **▲** (Subir) o **▼** (Bajar) en la fila. Solo se mueve entre sus hermanas activas.
2. La app renumera los códigos WBS de las hermanas y de todas sus subactividades.

### Crear una reprogramación (nueva versión)
1. En la versión vigente pulsa **+ Crear reprogramación**.
2. Lee el aviso: se crea una nueva versión y la actual quedará como **Histórica**.
3. Escribe el **Motivo de reprogramación*** (obligatorio).
4. Opcional: en **Cargar cronograma desde Project (opcional)** elige el **.mpp** reprogramado. Verás "✓ archivo — N actividades leídas del Project" (con **quitar** puedes descartarlo).
   - **Con .mpp:** la nueva versión se crea con las actividades del Project.
   - **Sin archivo:** se copian todas las actividades de la versión vigente (fechas, disciplinas, avances, activas e inactivas) para que las ajustes.
5. Pulsa **Crear reprogramación**. La nueva versión, "Actividades Reprogramación N", queda vigente y seleccionada.

### Consultar versiones y activar una histórica como vigente
1. En **Cronograma:** abre la lista y elige la versión (la vigente tiene ★). Junto a la lista se ve **● Vigente** u **○ Histórica** y el motivo de la reprogramación.
2. Para volver a usar una versión histórica, pulsa **★ Activar como vigente**.
3. Confirma con **✓ Sí, activar**. La versión que estaba vigente pasa a Histórica.

### Reiniciar la plantilla
1. En la versión vigente pulsa **Reiniciar plantilla**.
2. La ventana **Reiniciar Plantilla** muestra uno de dos casos:
   - **La versión tiene registros de avance del Informe Diario:** no se borra nada. Pulsa **Crear nueva versión con plantilla EPC**. Se crea "Actividades Plantilla EPC N" como vigente, desde la fecha de inicio del proyecto, y la versión anterior queda Histórica con todos sus avances.
   - **La versión no tiene avances:** pulsa **Sí, reiniciar todo**. Se eliminan **todas** las actividades y la versión, y vuelves a la pantalla inicial para cargar la plantilla, importar o crear desde cero. **No se puede deshacer.**
3. Pulsa **Cancelar** si no quieres continuar.

> ⚠ Por confirmar: si al reiniciar sin avances existen otras versiones históricas, después de recargar la página la más reciente de ellas quedará como vigente. Validar si ese es el comportamiento esperado.

### Consultar la Curva S
La Curva S (avance real frente al planificado) no está en esta pantalla. Está en el **Dashboard** del proyecto (capítulo 14), con el botón **Actualizar**. Su línea planificada sale de las fechas de la versión vigente y la línea real, de los Informes Diarios.

### Registrar una restricción
1. Entra a **Restricciones** y pulsa **+ Nueva Restricción** (o **+ Registrar primera restricción**).
2. Completa **Número*** (ej. RES-001), **Tipo** (Meteorológica, Administrativa, Técnica, Financiera o Permisos), **Estado** (Abierta por defecto), **Fecha Identificación**, **Descripción***, **Responsable**, **Fecha Compromiso**, **Fecha Levantamiento**, **Impacto** y **Plan de Acción**.
3. Pulsa **Guardar**.
4. Revisa las tarjetas **TOTAL**, **ABIERTAS**, **EN GESTIÓN** y **LEVANTADAS**.

### Hacer seguimiento, levantar o eliminar una restricción
1. En la tabla (#, Tipo, Descripción, Responsable, Estado, F. Compromiso, Acciones) pulsa **Editar** para actualizar el estado (por ejemplo, a *En Gestión*) o el plan.
2. Cuando se resuelva, pulsa **✓ Levantar**: queda *Levantada* con la fecha de hoy como fecha de levantamiento. Este botón solo aparece en las restricciones Abiertas o En Gestión.
3. Para borrarla, pulsa **Eliminar** y luego **Confirmar** (o **×** para cancelar). Se borra definitivamente.

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| No veo los botones **Guardar cambios**, **+ Crear reprogramación**, **Reiniciar plantilla** ni **+ Nueva actividad**, y los campos están bloqueados | Estás viendo una versión **Histórica**. | Elige la versión con ★ en **Cronograma:**, o pulsa **★ Activar como vigente** si realmente quieres trabajar sobre esa versión. |
| No aparecen los botones **Cronograma Excel / PDF / Project** | El proyecto ya tiene actividades o versiones; esos botones solo salen en la pantalla vacía. | Usa **+ Crear reprogramación** (con .mpp si quieres traer Project) o **Reiniciar plantilla**. |
| Cambio una fecha y al recargar vuelve al valor anterior | El inicio quedó después del fin (o el fin antes del inicio) y la app no guardó el cambio. | Cambia primero la fecha que libera el rango (por ejemplo, amplía el fin) y luego la otra. |
| No puedo escribir el avance real de una actividad | Es una actividad padre (su avance es el promedio de sus hijas) o un título de nivel 1. | Registra el avance en las subactividades. |
| "No se encontró una columna de 'Actividad' o 'Nombre' en la primera fila del Excel." | Los encabezados no están en la fila 1 o la columna del nombre se llama "Actividad". | Pon los encabezados en la fila 1 y titula la columna del nombre **Nombre** o **Tarea**. |
| "El archivo Excel está vacío o no tiene datos." / "No se encontraron actividades en el Excel." | La primera hoja no tiene filas de datos o la columna de nombre está vacía. | Deja el cronograma en la **primera hoja** del libro, con los datos desde la fila 2. |
| "No se pudo extraer texto del PDF. Asegúrate de que no sea una imagen escaneada." | PDF escaneado o solo con imágenes. | Exporta el cronograma como PDF de texto desde el programa original, o usa Excel o .mpp. |
| "La clave de Gemini (GEMINI_API_KEY) no está configurada…" o "Error de Gemini (…)" | El servicio de IA no está disponible o no tiene cuota. | Informa al administrador y mientras tanto usa Excel o .mpp. |
| "Error al leer el archivo de Project: …" / "El archivo de Project no contiene actividades." | Archivo .mpp dañado, de otro formato o sin tareas. | Ábrelo en MS Project, guárdalo de nuevo como .mpp y vuelve a cargarlo. |
| "El motivo de reprogramación es obligatorio." | Intentaste crear la reprogramación sin motivo. | Escribe el motivo y pulsa **Crear reprogramación** de nuevo. |
| **Reiniciar plantilla** ofrece "Crear nueva versión con plantilla EPC" en vez de borrar | La versión tiene avances del Informe Diario y la app protege ese historial. | Es el comportamiento correcto: crea la versión nueva. La anterior queda Histórica y se puede reactivar. |
| "No se pudo reiniciar: …" o "No se pudo crear la versión: …" | Error al borrar o crear (por ejemplo, datos enlazados). | Recarga la página y revisa en qué versión quedaste. Si persiste, informa al administrador con el mensaje. |
| "Error al cargar la plantilla EPC: …" | No se encontró la plantilla estándar en la app. | Informa al administrador. Mientras tanto, importa un .mpp. |
| Al guardar una restricción aparece "El número es obligatorio" o "La descripción es obligatoria" | Faltan campos obligatorios. | Completa **Número** y **Descripción**. |

---

## Relación con otros módulos

- **Proyectos (cap. 1):** la **Fecha inicio planeada** del proyecto ubica la plantilla EPC y las importaciones de Excel y PDF.
- **Informe Diario (cap. 3):** muestra solo las **actividades activas de la versión vigente**. Al guardar un informe se actualiza el **avance real** de cada actividad y se recalculan los padres. Los avances quedan ligados a la versión, y por eso no se puede borrar una versión con avances.
- **Dashboard (cap. 14) y Centro de Control (cap. 1):** la Curva S, el SPI y las actividades atrasadas usan la versión vigente y los avances reportados.
- **Costos (cap. 5):** las partidas de costos pueden estar enlazadas a actividades WBS. Al reiniciar una versión sin avances se desenlazan antes de borrar.
- **Clima (cap. 13):** complementa las restricciones de tipo meteorológico.

---

## Buenas prácticas

- **Una sola fuente de avance:** cuando el equipo empiece a usar el Informe Diario, reporta el avance **por el informe** y no a mano en la tabla. El informe vuelve a escribir el avance real de cada actividad.
- **Nunca edites la línea base: reprograma.** Usa **+ Crear reprogramación** con un motivo claro, así la versión anterior queda como evidencia.
- **Desactiva en lugar de eliminar** las actividades que no aplican. Conservan su historial y dejan de afectar promedios e informes.
- **Asigna disciplina a todas las actividades hoja.** Los resúmenes por disciplina del Informe Diario y del Dashboard dependen de ella.
- **Revisa siempre la vista previa** al importar desde PDF (con IA) o Excel antes de pulsar **Importar**.
- **Prefiere .mpp sobre PDF** cuando el cronograma venga de MS Project: conserva fechas reales, jerarquía y % completado.
- **Cuida los títulos de nivel 1:** agrupan secciones y no reciben avance. Registra el trabajo en niveles 2 o inferiores.
- **Reordena con cuidado:** ▲ ▼ cambia los códigos WBS. Avisa al equipo si alguien usa esos códigos en otros documentos.
- **Restricciones:** registra cada una con responsable y **Fecha Compromiso**, revísalas en el comité semanal y pulsa **✓ Levantar** apenas se resuelvan.
