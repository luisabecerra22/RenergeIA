# 3. Informe Diario

> **Para qué sirve:** registra lo que pasó cada día en obra: personal en sitio, resumen, observaciones y, sobre todo, el **porcentaje de avance de cada actividad** del cronograma vigente. Con esos datos la app calcula la desviación frente a lo programado, actualiza el avance del Cronograma de Actividades y alimenta el Dashboard, la Curva S y el Centro de Control. Cada informe pasa por un flujo de revisión y aprobación y se puede exportar a PDF.
> **Quién lo usa:** ingenieros residentes y supervisores de campo (elaboran), director de obra o control de proyectos (revisan y aprueban) y gerencia (consulta).
> **Dónde está:**
> - Menú lateral → proyecto → **Informe Diario** → `/proyectos/{id}/informes`
> - Nuevo informe: `/proyectos/{id}/informes/crear`
> - Ver informe: `/proyectos/{id}/informes/{informe}` · Editar: `/proyectos/{id}/informes/{informe}/editar`

---

## Conceptos clave

- **Informe diario:** el reporte de un día de obra. **Solo puede haber un informe por proyecto y por fecha.**
- **N° de Informe:** consecutivo de tres dígitos (001, 002…) que la app asigna al crear el informe.
- **Estados del informe:**

  | Estado | Significado | Qué se puede hacer |
  |---|---|---|
  | **Borrador** | En elaboración | Editar, eliminar, **Enviar a Revisión** |
  | **En Revisión** | Enviado al revisor | **Aprobar** o **Rechazar** |
  | **Aprobado** | Aceptado | Consultar y exportar PDF |
  | **Rechazado** | Devuelto con motivo | **Volver a Borrador** para corregir |

- **Prog. % (avance programado):** lo que la actividad debería llevar **en la fecha del informe**, según sus fechas planeadas (distribución lineal: 0 % al inicio y 100 % al final).
- **Avance Real %:** el porcentaje **acumulado** que la actividad lleva a la fecha del informe (no lo hecho solo ese día).
- **Desv. (desviación):** Avance Real % − Prog. %.
- **Estado de la actividad (punto de color en la columna Est.):**

  | Punto | Estado | Desviación |
  |---|---|---|
  | Verde | Adelantado | +5 % o más |
  | Azul | Al Día | de 0 % a menos de +5 % |
  | Naranja | Atrasado | de −15 % a menos de 0 % |
  | Rojo | Crítico | peor que −15 % |

- **Actividad padre (fila con 📁):** agrupa subactividades. Su avance real y su avance programado son el **promedio** de sus hijas y no se digitan.
- **CR:** insignia roja de las actividades marcadas como críticas en el cronograma.

---

## Cómo funciona

### De dónde salen las actividades
- El informe lista automáticamente **todas las actividades activas de la versión vigente** del Cronograma de Actividades, ordenadas por código WBS, con su código, disciplina, fechas de inicio y fin, y duración en días.
- Si el cronograma no tiene actividades activas, la tabla muestra "No hay actividades WBS activas en este proyecto." con el enlace **Ir a WBS**.

### Qué calcula la app sola
- **N° de Informe:** el número más alto usado en el proyecto + 1.
- **Prog. %** de cada actividad según la fecha del informe. Si cambias la **Fecha**, se recalcula al instante.
- **Avance y programado de las actividades padre** (promedio de sus hijas), **Desviación**, **punto de estado** y los contadores **Adelantados**, **Al Día**, **Atrasados** y **Críticos** (solo cuentan las actividades hoja).
- **Al guardar el informe** (aunque quede en Borrador), por cada actividad hoja:
  - Guarda el avance acumulado. **El avance nunca retrocede:** si escribes un valor menor que el último reportado en una fecha anterior, se conserva el anterior.
  - Calcula la desviación, los días de atraso y el estado de avance.
  - **Actualiza el Avance Real de la actividad en el Cronograma de Actividades** y recalcula el promedio de las actividades padre.
- **Revisado por / Fecha revisión:** se llenan con el usuario y la fecha al aprobar o rechazar.

### Qué se digita y qué no
| Se digita | No se digita |
|---|---|
| Fecha, Personal total en sitio, Resumen de actividades del día, Observaciones generales, Comentarios adicionales | N° de Informe (al crear), Prog. %, Desv., Est., avance de las actividades padre, contadores |
| Por actividad hoja: **Avance Real %** (acumulado), **Pers.** (personas) y **Hrs.** (horas trabajadas) | Avance real del cronograma (se actualiza solo desde el informe) |

> ⚠ **Ojo con el avance en 0:** cada informe nuevo muestra el Avance Real % de todas las actividades en 0. Si dejas una actividad en 0, se conserva el último avance que tenía **en informes anteriores**. Pero si la actividad **nunca ha tenido informe** (por ejemplo, justo después de una reprogramación o de importar un .mpp con % completado), dejarla en 0 **pone su avance del cronograma en 0**. Escribe siempre el % acumulado real de las actividades en curso.

> ⚠ Por confirmar: el avance del cronograma se actualiza al **guardar** el informe, incluso en Borrador, y no al aprobarlo. Rechazar o eliminar un informe no revierte ese avance. Validar con el equipo si el avance debe contar solo desde la aprobación.

> ⚠ Por confirmar: las pantallas del Informe Diario no tienen campos para **clima** ni **fotografías**, aunque la base de datos contempla ambos. El clima se consulta y guarda en el módulo **Clima** (cap. 13), sin enlazarse a un informe. Mientras tanto, anota las novedades de clima en **Observaciones generales**.

> ⚠ Por confirmar: el detalle muestra la **Versión** del informe (v1), pero hoy el número no aumenta al corregir un informe rechazado. Además, cualquier usuario con sesión puede aprobar o rechazar, porque el código no restringe por rol.

---

## Paso a paso

### Consultar la lista de informes
1. Entra a **Informe Diario** del proyecto.
2. Filtra con la lista de estados: **Todos los estados**, **Borrador**, **En Revisión**, **Aprobado** o **Rechazado**.
3. La tabla muestra: **Fecha**, **N° Informe**, **Actividades** (cantidad de registros de avance), **Personal**, **Estado**, **Creado por** y **Acciones** (**Ver**, ícono PDF y, solo en Borrador, **Editar** y **Eliminar**). Los informes más recientes salen primero.

### Crear el informe del día
1. Pulsa **+ Nuevo informe** (o **Crear primer informe**).
2. En **Información general del informe**:
   - **Fecha***: viene con la fecha de hoy. Cámbiala si reportas otro día. El Prog. % se recalcula.
   - **N° de Informe**: generado automáticamente, no editable al crear.
   - **Personal total en sitio**: total de personas en obra.
   - **Resumen de actividades del día**, **Observaciones generales** (novedades, incidentes, clima) y **Comentarios adicionales**.
3. En **Avances del Día (N actividades)**, usa **Buscar por código o nombre...** para ubicar actividades rápido.
4. En cada actividad hoja trabajada, escribe:
   - **Avance Real %**: el porcentaje **acumulado** a hoy (0 a 100).
   - **Pers.**: personas en esa actividad.
   - **Hrs.**: horas trabajadas (admite medias horas).
5. Revisa los contadores **Adelantados / Al Día / Atrasados / Críticos** y los puntos de color.
6. Pulsa **Crear informe** (abajo) o **Guardar cambios** (arriba). El informe queda en **Borrador** y la app abre su detalle.

### Editar un informe en Borrador
1. En la lista pulsa **Editar**, o en el detalle pulsa **Editar** (solo aparece en Borrador).
2. Corrige los datos o los avances. En edición sí se puede cambiar el **N° de Informe**.
3. Pulsa **Guardar cambios**.

### Enviar a revisión
1. Abre el informe (**Ver**).
2. Verifica los datos y pulsa **Enviar a Revisión**. El estado pasa a **En Revisión** y ya no se puede editar.

### Aprobar un informe
1. Abre un informe **En Revisión**.
2. Revisa el resumen y la tabla de avances.
3. Pulsa **Aprobar**. Quedan registrados **Revisado por** y **Fecha revisión**.

### Rechazar un informe
1. Abre un informe **En Revisión** y pulsa **Rechazar**.
2. En **Rechazar informe**, escribe el **Motivo del rechazo**.
3. Pulsa **Confirmar rechazo** (o **Cancelar**). Sin motivo, el rechazo no se registra.
4. El informe queda **Rechazado** y muestra "Motivo de rechazo: …".

### Corregir un informe rechazado
1. Abre el informe rechazado y lee el motivo.
2. Pulsa **Volver a Borrador**. El motivo se borra.
3. Pulsa **Editar**, corrige y guarda.
4. Pulsa **Enviar a Revisión** otra vez.

### Exportar a PDF
1. En el detalle (o en el formulario) pulsa **Exportar PDF**, o en la lista pulsa el ícono PDF rojo (abre el detalle y lanza la impresión sola).
2. En la ventana de impresión del navegador elige **Guardar como PDF**. La hoja sale en horizontal.
3. El PDF incluye el encabezado con el logo, "Informe Diario de Obra N°…", el proyecto y la fecha; el personal en sitio, el resumen y las observaciones; la tabla de avances, y el pie "Documento confidencial" con la fecha de generación.

### Eliminar un informe en Borrador
1. En la lista pulsa **Eliminar** en un informe en Borrador.
2. Confirma con **Sí, eliminar** (o **Cancelar**).

> ⚠ Por confirmar: eliminar un borrador que ya tiene avances guardados puede ser rechazado por la base de datos y mostrar un error general. Tampoco revierte el avance que ese informe escribió en el cronograma. Si te equivocaste de datos, es preferible editar el borrador.

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| "Ya existe un informe para el dd/mm/aaaa. Solo puede haber un informe por día." | Ya hay un informe de esa fecha en el proyecto. | Busca ese informe en la lista y edítalo (si está en Borrador) o corrige la fecha. |
| "Ya existe un informe con el N° '…'." | Al editar pusiste un número que ya usa otro informe. | Usa otro número o deja el que tenía. |
| "No hay actividades WBS activas en este proyecto." | El cronograma está vacío, todas las actividades están inactivas o no hay versión vigente con actividades. | Pulsa **Ir a WBS** y carga el cronograma o activa las actividades (cap. 2). |
| Una actividad que existe en el cronograma no aparece en el informe | Está inactiva o pertenece a una versión histórica. | Actívala (▶) en la versión vigente o revisa qué versión está vigente. |
| No puedo escribir el avance de una fila | Es una actividad padre (📁): su avance es el promedio de sus hijas. | Registra el avance en sus subactividades. |
| Escribí un avance menor y el cronograma sigue mostrando el anterior | El avance acumulado nunca retrocede frente a informes de fechas anteriores. | Si el avance anterior fue un error, corrige el informe de esa fecha (si sigue en Borrador) o informa a control de proyectos. |
| Tras guardar, una actividad quedó con avance 0 % en el cronograma | Era su primer informe (por ejemplo, después de una reprogramación) y se dejó en 0. | Edita el informe (si está en Borrador) y escribe el % acumulado real. |
| No veo el botón **Editar** o **Eliminar** | El informe no está en Borrador; al abrir la dirección de edición, la app te devuelve a la lista. | Si está Rechazado, pulsa **Volver a Borrador**. Si está En Revisión o Aprobado, pide al revisor que lo rechace para corregirlo. |
| Al pulsar **Confirmar rechazo** no pasa nada | El motivo está vacío. | Escribe el motivo del rechazo. |
| Aparece "Error al guardar: …" | Error al grabar en la base de datos. | Revisa tu conexión, recarga la página (lo digitado sin guardar se pierde) e intenta de nuevo. Si persiste, informa al administrador con el mensaje. |
| El campo **Personal total en sitio** marca un error | El valor está fuera del rango permitido (0 a 9999). | Escribe un número entero entre 0 y 9999. |
| El PDF sale cortado o vertical | Configuración de impresión del navegador. | En la ventana de impresión elige orientación horizontal, escala "Ajustar" y destino **Guardar como PDF**. |

---

## Relación con otros módulos

- **Cronograma de Actividades (cap. 2):** es la fuente de las actividades (solo las activas de la versión vigente), sus fechas y su disciplina. A su vez, el informe **actualiza el Avance Real** de cada actividad y de sus padres. Los avances quedan ligados a la versión, por eso esa versión ya no se puede borrar con **Reiniciar plantilla**.
- **Dashboard (cap. 14):** la Curva S real, los KPIs y los resúmenes por disciplina salen de los avances registrados en los informes.
- **Centro de Control (cap. 1):** el avance global, el SPI, las actividades atrasadas, la clasificación (Saludable, En Riesgo, Crítico) y la columna **Último Registro** vienen de estos informes.
- **Clima (cap. 13):** registra las condiciones meteorológicas del proyecto por separado.
- **Histogramas (cap. 6):** el personal real del histograma sale de la nómina, **no** del campo *Personal total en sitio* del informe.

---

## Buenas prácticas

- **Haz el informe el mismo día** y envíalo a revisión antes de terminar la jornada. Así el Centro de Control y el Dashboard están al día.
- **Reporta siempre el % acumulado**, no lo hecho en el día. Escribe el valor en todas las actividades en curso, aunque no hayan avanzado, para que el detalle y el PDF sean fáciles de leer.
- **Revisa con especial cuidado el primer informe después de una reprogramación:** ninguna actividad tiene aún historial, y un 0 borra su avance.
- **Registra personas y horas por actividad.** Sirven para analizar rendimientos.
- **Usa Observaciones generales** para clima, incidentes, visitas y novedades que expliquen desviaciones.
- **Rechaza siempre con un motivo concreto** ("Falta avance de hincado bloque 3", "Personal no coincide con el control de ingreso") para que la corrección sea rápida.
- **No uses Eliminar para corregir:** edita el borrador.
- **Antes del primer informe, revisa que el cronograma vigente tenga fechas y disciplinas correctas:** el Prog. % y la desviación dependen de ellas.
