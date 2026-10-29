# 17. Compromisos — Actas de reunión

> **Para qué sirve:** reunir en un solo lugar las actas de reunión (internas y externas, formato FO-SG-GC-021) y hacer seguimiento a cada compromiso desde la reunión en que nació hasta su cierre: quién lo tiene, cuándo vence, qué se ha gestionado y cuánto cumple cada persona.
> **Quién lo usa:** gerencia, dirección y coordinación de proyectos, líderes de área y cualquier persona de Renergeia con compromisos asignados en reuniones.
> **Dónde está:** menú superior → **Compromisos** (`/actas`). Desde ahí se llega al **Tablero de compromisos** (`/actas/compromisos`), al **Seguimiento por persona** (`/actas/seguimiento`) y al detalle de cada acta (`/actas/{id}`). Es un módulo corporativo: no está dentro de un proyecto.

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Acta interna / Acta externa** | Dos versiones de la misma reunión. La **interna** es de uso exclusivo del equipo Renergeia y trae el desarrollo completo y **todos** los compromisos. La **externa** es la que se comparte con el cliente: trae un resumen y solo una parte de los compromisos. |
| **Código del acta** | Identificador único del acta (por ejemplo, `COS6VL-ACT-SEG-2026-004`). La app reconoce un acta por su código. |
| **Compromiso** | Tarea acordada en una reunión, con texto, responsable, empresa, fecha límite y estado. Se identifica con el código del acta más su número (por ejemplo, `…-2026-004-03`) y conserva ese código hasta su cierre, aunque se revise en muchas reuniones. |
| **Continuidad** | Cuando una reunión posterior retoma un compromiso anterior, **no se crea un compromiso nuevo**: queda como una actualización en el historial del compromiso original. |
| **Propio / Recordatorio al cliente** | **Propio** = lo debe hacer Renergeia. **Recordatorio al cliente** = lo debe hacer el cliente (GreenYellow, Atera, Celsia) y Renergeia solo le hace seguimiento. |
| **Estado del compromiso** | **Pendiente**, **En seguimiento** o **Cerrado**. |
| **Estado del acta** | **Borrador**, **Con comentarios** o **Aprobada**. |
| **Responsable principal** | La persona a quien se le cuenta el compromiso cuando hay varios responsables ("Ana / Pedro"). Si no se ha definido, es el primero de la lista. |
| **Compartido** | Compromiso con varios responsables. Es **un solo** compromiso, no uno por persona. |
| **Por aclarar** | Compromiso marcado para confirmar algún dato (responsable, alcance, fecha). |
| **Fecha descriptiva** | Fecha escrita como texto ("Por definir", "Al terminar el montaje"). Se conserva tal cual, **sin inventar un vencimiento**. |
| **Proyecto del compromiso** | Ver *Cómo se asigna el proyecto* más abajo. Las actas sin proyecto se muestran como **General Renergeia**. |
| **Carga (lote)** | Cada subida de actas Word o de un paquete ZIP queda registrada como una carga, con respaldo, y se puede **revertir**. |
| **Actas IA** | El sistema anterior de actas. Sus actas y compromisos se migraron a este módulo; ese sistema sigue intacto hasta que se decida apagarlo. |

---

## Cómo funciona

### De dónde salen las actas

1. **Migración del sistema anterior (paquete ZIP):** las actas históricas de Actas IA se trajeron con **Importar paquete ZIP**. Antes de guardar se hace una **simulación** que muestra qué entraría y qué conflictos hay; nada se decide en silencio.
2. **Actas nuevas en Word, una a una:** con **Subir actas (Word)** se suben los documentos FO-SG-GC-021 a medida que se emiten. Hay vista previa y nada se guarda hasta confirmar.

### Reglas de la carga de actas Word

- **Complementa, nunca reescribe.** Si el acta ya existe, no se modifica: solo se le agregan los documentos y compromisos que le falten. Un compromiso que ya estaba cargado tampoco se toca (su estado, responsable o historial se conservan).
- **Los compromisos solo se toman del acta interna.** Si subes solo la externa, se guarda el documento, pero los compromisos entran cuando subas la interna. Si subes interna y externa juntas, se combinan en una sola acta.
- **Interna o externa:** la app lo decide por el nombre del archivo (si dice "externa" o "interna"); si el nombre no lo dice, la trata como interna cuando el documento trae la sección de desarrollo de la reunión.
- **Seguimiento de compromisos anteriores:** cada fila de esa tabla del Word queda como una actualización en el historial del compromiso de origen, **sin cambiar su estado**. Si la fila solo nombra un acta y no un número de compromiso, la app no adivina: queda visible en el acta y listada en la vista previa.
- **Proyectos nuevos:** si el acta trae un código de proyecto (COSxxx) que aún no está en el catálogo de actas, se agrega.
- **Fechas:** solo se convierten en fecha límite las fechas reales (por ejemplo, `15/11/2026` o "15 de noviembre de 2026"); las descriptivas quedan como texto y el compromiso sale como **Sin fecha límite**.
- **Límites:** hasta **20** archivos por carga, solo `.docx` y hasta 30 MB cada uno.

### Reglas del paquete ZIP (migración)

- Se puede volver a importar sin duplicar: las actas se reconocen por su código y los compromisos por su identificador.
- Los compromisos que ya tienen gestión hecha en la app quedan **protegidos**: la importación no los pisa.
- Si el paquete no trae el estado del tablero del sistema anterior, los compromisos entran como **pendientes** (sin los cierres anteriores) y la app lo advierte.
- Los documentos históricos (Word/PDF) ya guardados no se reemplazan, aunque el paquete traiga una versión distinta; la diferencia se lista.

### Reversión de una carga

Cada carga aparece en **Importaciones realizadas** con fecha, archivo y usuario. **Revertir** elimina lo que creó esa carga y restaura lo que modificó. **La reversión se bloquea** si sobre lo cargado ya hay gestiones (cierres, ediciones, actualizaciones) o comentarios hechos en la app, para no perder ese trabajo.

### Cómo se asigna el proyecto de un compromiso

- Si el acta tiene código de proyecto (COSxxx), ese es el proyecto.
- Si el acta es general o de varios proyectos, la app toma la **etiqueta al inicio del texto** del compromiso, por ejemplo `[Kimberly] Enviar…` o `[Aurora] …`, y la reconoce con el catálogo de proyectos de actas, los proyectos de RenergeIA y las etiquetas que ya traen código en otras actas (si una acta dice `[COS6AU - La Aurora]`, la app aprende que "Aurora" es COS6AU).
- `[General …]` o sin etiqueta → **General Renergeia**.
- Este proyecto se calcula al mostrar la información (no se guarda), y se usa en los filtros, la búsqueda y la agrupación del tablero y del seguimiento por persona.

### Semáforo de los compromisos

| Color / situación | Regla |
|---|---|
| 🔴 **Vencido** | Abierto y la fecha límite ya pasó |
| 🟠 **Vence pronto** | Abierto y vence en los próximos 7 días |
| 🟢 **En plazo** | Abierto y vence en más de 7 días |
| ⚪ **Sin fecha límite** | Abierto y sin fecha real (o con fecha descriptiva) |
| Gris tachado | **Cerrado** |

Las fechas se comparan con el día de hoy en hora de Colombia.

### Seguimiento por persona: cómo se calcula el % de cumplimiento

- **% de cumplimiento = compromisos cumplidos (cerrados) ÷ total de compromisos asignados.** Se cuenta todo lo asignado, no solo lo vencido.
- **Solo compromisos propios de Renergeia:** quedan fuera los recordatorios a clientes y los de proveedores (empresa distinta a Renergeia).
- **Cada compromiso cuenta solo para su responsable principal**, aunque sea compartido.
- **Nombres unificados automáticamente:** si en unas actas dice "Miguel Prada" y en otras "Miguel Angel Prada Muñoz", la app los toma como la misma persona (se muestra "También aparece en las actas como…"). Solo se unen cuando hay un único candidato posible; dos personas con el mismo nombre de pila no se mezclan.
- Responsables genéricos (un equipo, un área, "Por confirmar") van a **Equipos / sin responsable definido** y no entran en el % de nadie.
- **A tiempo / tarde:** de los cumplidos con fecha límite, cuántos se cerraron hasta esa fecha y cuántos después.
- Colores del %: verde desde 80 %, naranja de 50 % a 79 %, rojo por debajo de 50 %.

---

## Paso a paso

### Subir actas nuevas en Word

1. Entra a **Compromisos** y haz clic en **Subir actas (Word)**.
2. Selecciona uno o varios `.docx` (interna y externa de la misma reunión pueden ir juntas).
3. Revisa la vista previa: por cada acta, **Documentos** (Interna/Externa, "se guarda" o "ya estaba cargada"), **Compromisos** (nuevos y ya cargados, o "falta la interna") y **Qué pasará** (**Acta nueva** o **Ya existe — se complementa**). En las actas nuevas elige el estado con que queda: **Borrador**, **Con comentarios** o **Aprobada**.
4. Lee las secciones de abajo: **Conflictos para revisar**, **Seguimiento que quedará en el historial…**, **Seguimiento sin compromiso de origen en la app**, **Proyectos sin vincular a RenergeIA**, **Fechas descriptivas** y **Advertencias**.
5. Haz clic en **Guardar N acta(s)**. Luego puedes **Subir más actas** o **Cerrar**. Si te equivocaste de archivos, usa **Elegir otros archivos** antes de guardar.

### Importar un paquete ZIP del sistema anterior

1. Haz clic en **Importar paquete ZIP** y selecciona el ZIP.
2. Revisa el **Resultado de la simulación** (actas nuevas, sin cambios, con diferencias, compromisos nuevos, continuidades, archivos nuevos, protegidos) y las listas de conflictos y advertencias.
3. Haz clic en **Aplicar importación (con respaldo)** o en **Cancelar**.

### Revertir una carga

1. Al final de la página de **Compromisos**, en **Importaciones realizadas**, haz clic en **Revertir** en la carga que quieres deshacer.
2. Confirma con **Sí, revertir**. Si la app responde *"No se revirtió: … ya hay gestión(es)…"*, es porque alguien ya trabajó sobre esos compromisos (ver *Qué hacer en caso de…*).

### Consultar las actas

1. En **Compromisos** usa la búsqueda y los filtros de proyecto (incluye **General / corporativas**), área y estado.
2. Haz clic en una fila para abrir el acta: información general, **Asistentes**, **Resumen y desarrollo**, **Seguimiento de compromisos anteriores**, **Compromisos de esta reunión**, **Documentos oficiales** (botones para descargar el Word o PDF interno/externo) y **Comentarios de revisión**.
3. Si el acta tiene acta anterior, su código es un enlace para ir a ella.

### Gestionar compromisos en el tablero

1. Haz clic en **Tablero de compromisos**. Elige la pestaña **Compromisos propios** o **Recordatorios a clientes**.
2. Filtra por estado (**Abiertos** por defecto, **Vencidos**, **Próximos a vencer (7 días)**, **Sin fecha límite**, **Cerrados**…), proyecto o responsable, y agrupa **Por área**, **Por persona** o **Por proyecto**.
3. **Cerrar:** haz clic en el ícono de check de la tarjeta, escribe la **Nota de cierre (evidencia)** y **Tu nombre**, y pulsa **Cerrar compromiso**. El mismo ícono en un compromiso cerrado permite **Reabrir**.
4. **Editar (✎):** cambia el responsable principal, la categoría (columna del tablero), el tipo, el estado o la fecha límite. El texto y los responsables originales del acta no se borran. **Cambiar la fecha límite exige escribir el motivo** (debe existir un acuerdo explícito).
5. **Historial:** muestra todas las gestiones (de la app, de las actas y de la migración). Para registrar avance escribe la actualización, tu nombre y, si aplica, el acta de referencia, y pulsa **Guardar**.

### Ver la vista gerencial

1. En el tablero, abre la pestaña **Vista gerencial**.
2. Por responsable verás abiertos, vencidos, próximos a vencer, el compromiso más urgente, la fecha de la última gestión y cuántos están **por aclarar**. La lista empieza por quien tiene más vencidos.

### Revisar el seguimiento por persona

1. Haz clic en **Seguimiento por persona**.
2. El resumen muestra el ranking de personas: asignados, cumplidos, pendientes, vencidos, vencen en 7 días, % de cumplimiento y a tiempo / tarde. Haz clic en un encabezado para ordenar.
3. Filtra por proyecto, área o rango de fechas de las actas (**Quitar filtros** para volver).
4. Haz clic en una persona (o elígela en la lista) para ver su ficha: % de cumplimiento, desglose **Por proyecto** y las listas **Vencidos**, **Pendientes en plazo o sin fecha** y **Cumplidos**. La dirección de esa página se puede copiar y compartir.
5. Al final del resumen, **Ver** muestra los compromisos de **Equipos / sin responsable definido**.

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| Subí el acta y no aparecen sus compromisos | Se subió solo el acta **externa** (o el nombre del archivo dice "externa"). | Sube el Word de la **interna**; los compromisos entran en ese momento. |
| *"…no es un Word .docx…"* | El archivo es `.doc` antiguo, PDF u otro formato. | Ábrelo en Word y guárdalo como `.docx`. |
| Volví a subir un acta corregida y no cambió nada | La carga complementa y **nunca reescribe** un acta o compromiso ya cargado. | Haz el ajuste en el tablero (✎ o Historial). ⚠ Por confirmar: cómo corregir el texto de un compromiso o los datos generales de un acta ya cargada. |
| Aparece *Seguimiento sin compromiso de origen en la app* | La fila del Word nombra un acta sin número de compromiso, o el compromiso de origen no se ha cargado. | Sube primero el acta de origen o registra la actualización a mano en el **Historial**. |
| *"No se revirtió: … ya hay gestión(es)…"* | Alguien cerró, editó o actualizó compromisos de esa carga. | Revisa esas gestiones; la app no permite perderlas. Corrige con ✎ en vez de revertir. |
| Un compromiso tiene **Sin fecha límite** aunque el acta dice una fecha | La fecha venía como texto ("Por definir", "semana 3"). | Edítalo con ✎, pon la fecha acordada y el motivo. |
| Un compromiso sale en **General Renergeia** pero es de un proyecto | El acta no tiene código de proyecto y el texto no empieza con la etiqueta `[Proyecto]`. | En las próximas actas escribe la etiqueta al inicio, por ejemplo `[Villanueva] …`. ⚠ Por confirmar: no hay forma en pantalla de cambiar el proyecto de un compromiso ya cargado. |
| Una persona aparece dos veces en el seguimiento | Hay dos candidatos posibles para el nombre corto, o el nombre está escrito muy distinto. | Usa el nombre completo en las actas. ⚠ Por confirmar: no existe aún una pantalla para unir nombres a mano. |
| Una persona no aparece en el seguimiento | Sus compromisos son compartidos y ella no es la principal, o son recordatorios/de otra empresa. | Si debe contarle, edita el compromiso (✎) y cambia el **Responsable principal**. |
| No me deja cerrar o editar: *"Escribe tu nombre…"* | El campo **Tu nombre** está vacío. | Escribe tu nombre; queda como autor de la gestión. |
| *"El compromiso ya no existe…"* | Se revirtió la carga mientras tenías la página abierta. | Recarga la página. |
| Quiero agregar un comentario de revisión al acta | Los comentarios mostrados vienen del sistema anterior. | ⚠ Por confirmar: hoy no hay botón para agregar comentarios desde la app. |

---

## Relación con otros módulos

- **Proyectos** ([1](01-proyectos.md)): el código y nombre de los proyectos de RenergeIA se usan para reconocer las etiquetas `[Proyecto]` de los compromisos. Las actas tienen su propio catálogo de proyectos (con códigos y alias anteriores), que se vincula a los proyectos de la app solo cuando el código coincide exactamente. ⚠ Por confirmar: vínculo manual entre el catálogo de actas y los proyectos de la app (pendiente).
- **HSEQ Social** ([10](10-hseq-social.md)): sus compromisos con comunidades son un registro distinto; no se mezclan con este módulo.
- **Costos** ([5](05-costos.md)): los "Compromisos" de costos (órdenes de compra) no tienen relación con este módulo, aunque compartan la palabra.
- **WhatsApp (Helvis):** el historial de compromisos está preparado para recibir actualizaciones por WhatsApp, pero **hoy no se envían mensajes automáticos** de recordatorio. ⚠ Por confirmar: cuándo y cómo se activarán.
- **Actas IA (sistema anterior):** sigue funcionando sin cambios; los datos se trajeron por paquete ZIP. ⚠ Por confirmar: fecha en que se deja de usar y revisión final contra ese sistema.

---

## Buenas prácticas

- Sube **siempre el acta interna**; la externa sola no trae los compromisos.
- Sube las actas en orden cronológico: así el seguimiento de compromisos anteriores encuentra su compromiso de origen.
- En reuniones generales o multiproyecto, empieza cada compromiso con la etiqueta del proyecto: `[Kimberly] …`, `[Aurora] …`.
- Escribe fechas límite reales solo cuando haya acuerdo; si no lo hay, déjalo claro ("Por definir") en vez de inventar una fecha.
- Usa siempre el **nombre completo** del responsable y define un **responsable principal** cuando el compromiso sea compartido.
- Cierra cada compromiso con una **nota de evidencia** concreta (qué se entregó, a quién, dónde quedó).
- Registra los avances en el **Historial** en lugar de esperar a la siguiente reunión.
- Antes de **Revertir** una carga, confirma que nadie haya trabajado sobre ella; si ya hay gestión, corrige con ✎.
- Revisa la **Vista gerencial** y el **Seguimiento por persona** en el comité semanal, empezando por los vencidos.
