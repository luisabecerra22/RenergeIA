# 8. HSEQ — Calidad

> **Para qué sirve:** llevar la gestión de calidad del proyecto: auditorías contra los requisitos de la norma ISO 9001:2015, puntos de inspección (PPIs), calibración de los equipos de medición, control de los documentos de calidad, no conformidades y acciones correctivas.
> **Quién lo usa:** inspectores y coordinadores de calidad (QA/QC), auditores internos, dirección de proyecto.
> **Dónde está:** menú del proyecto → **HSEQ** (`/proyectos/{id}/hseq`) → tarjeta **Calidad** → **Ver Calidad** (`/proyectos/{id}/hseq/calidad`). El registro general de no conformidades del proyecto está en el menú del proyecto → **No Conformidades** (`/proyectos/{id}/no-conformidades`).

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Auditoría ISO 9001** | Revisión del proyecto contra la lista de requisitos de ISO 9001:2015. Al crearla, la app carga todos los requisitos de la norma listos para evaluar. |
| **Cumplimiento (por requisito)** | Sin evaluar · ✅ Cumple · 🔄 En proceso · ❌ No cumple · ⬜ No aplica. |
| **Puntaje** | Valor de 0 a 2 de cada requisito. Se llena solo al elegir el cumplimiento (Cumple = 2, En proceso = 1, No cumple / No aplica / Sin evaluar = 0) y se puede corregir a mano. |
| **Seguimiento** | Estado de la acción que sale de un requisito: ⏳ Pendiente · 🔄 En proceso · ✅ Ejecutado. |
| **Estado de la auditoría** | **Borrador**, **En Proceso** o **Finalizada**. |
| **💡 Interpretación / Documentos auditables** | Ayudas incluidas en la app para cada requisito: qué pide la norma y qué documentos se suelen revisar como evidencia. |
| **PPI** | Programa / Punto de Inspección: una inspección planeada (con código, disciplina, frente y fecha). Estados: Pendiente · En Proceso · Aprobado · Rechazado · Requiere Corrección. |
| **Equipo de medición** | Instrumento que requiere calibración (código, serial, certificado, fechas). |
| **Documento HSEQ** | Documento del sistema de calidad (Procedimiento, Instructivo, Formato, Plan, ITP, Matriz, Dossier, Manual, Registro) con versión y vigencia. Estados: Vigente · En Revisión · Pendiente · Obsoleto · Vencido. |
| **NCR (No Conformidad de Calidad)** | Incumplimiento detectado en calidad. Severidad: Baja · Media · Alta · Crítica. Estados: Abierta · En Revisión · En Implementación · Cerrada · Cancelada. |
| **Acción correctiva** | Acción asociada a una no conformidad, con responsable, % de avance y fecha compromiso. |

---

## Cómo funciona

1. **Todo se registra por proyecto.** Cada pantalla muestra solo la información del proyecto abierto.
2. **Lo que la app calcula sola:**
   - **% de cumplimiento de la auditoría** = suma de puntajes ÷ (2 × número de requisitos evaluados) × 100. No cuentan los requisitos **Sin evaluar** ni **No aplica**. Se recalcula al cambiar cualquier cumplimiento o puntaje y se guarda al pulsar **Guardar cambios**, **Guardar borrador** o **Finalizar auditoría**.
   - **Semáforo de la auditoría:** 🟢 *Cumplimiento Alto* (85 % o más) · 🟡 *Requiere Atención* (70 % a 84 %) · 🔴 *Crítico* (menos de 70 %). En el listado de auditorías el chip se ve como Saludable / En Riesgo / Crítico con los mismos límites.
   - Gráficas de la auditoría: **Cumplimiento por Cláusula**, **Distribución de Estados** y **Seguimiento de Acciones**.
   - **Estado de calibración** de cada equipo, según la fecha de vencimiento:

     | Estado | Regla |
     |---|---|
     | Vigente | Vence en 30 días o más |
     | Próximo a Vencer | Vence en menos de 30 días |
     | Vencido | La fecha de vencimiento ya pasó |
     | Sin Certificado | No tiene fecha de vencimiento registrada |

   - Fechas en **rojo**: vencimiento de documentos ya pasado; fecha compromiso de NCR ya pasada sin cerrar; fecha compromiso de acciones correctivas ya pasada sin cerrar.
   - Contadores de acciones correctivas: **Vencidas** (abiertas con fecha compromiso pasada), **En Proceso**, **Cerradas**, **Abiertas**.
3. **Lo que se digita:** todos los formularios (auditorías, PPIs, equipos, documentos, NCR). Los **estados de PPIs, documentos y NCR son manuales**: la app no los cambia sola (por ejemplo, un documento con fecha vencida no pasa solo a "Vencido"; solo se pinta la fecha en rojo).
4. **Tablero de Calidad:** muestra tarjetas de indicadores y un panel de "Análisis Inteligente".

> ⚠ Por confirmar: el tablero de Calidad (`/hseq/calidad`) y el tablero HSEQ del proyecto (`/hseq`) muestran **cifras fijas de demostración** (el tablero HSEQ lo indica con el aviso "Fase 1 — Estructura base") y el panel de análisis está marcado como simulado. No reflejan los registros reales del proyecto; para cifras reales use los contadores de cada pantalla.

---

## Paso a paso

### Entrar a Calidad

1. Abre el proyecto y entra a **HSEQ**.
2. En la tarjeta **Calidad**, pulsa **Ver Calidad**.
3. Usa los botones de la parte superior para ir a cada sub-módulo: **PPIs**, **Calibración Equipos**, **Control Documental**, **No Conformidades**, **Acciones Correctivas**.

### Crear una auditoría ISO 9001

> ⚠ Por confirmar: la pantalla de auditorías ISO 9001 del proyecto (`/proyectos/{id}/hseq/calidad/checklist`) no tiene botón de acceso en el tablero de Calidad; hoy se abre escribiendo la dirección. Para auditorías corporativas por norma use el capítulo [11. HSEQ — Auditorías](11-hseq-auditorias.md).

1. Abre **Auditorías ISO 9001:2015** (`/proyectos/{id}/hseq/calidad/checklist`).
2. Pulsa **Nueva Auditoría**.
3. Diligencia **Auditor / Responsable** (obligatorio), **Fecha de Auditoría**, **Estado inicial** (Borrador o En Proceso) y, si quieres, **Observaciones generales**.
4. Pulsa **Crear y Diligenciar Checklist**. La app crea la auditoría con todos los requisitos en *Sin evaluar* y abre la pantalla de diligenciamiento.

### Diligenciar la auditoría

1. Revisa el bloque **Dashboard de Cumplimiento** (se puede ocultar haciendo clic en su encabezado).
2. En la tabla **Requisitos ISO 9001:2015**, para cada requisito:
   1. Si tienes dudas, pulsa el **💡** junto al requisito para ver la interpretación, o el **💡** de la columna Evidencia para ver los documentos auditables.
   2. Elige el **Cumplimiento**. El puntaje se llena solo; corrígelo si hace falta (0, 1 o 2).
   3. En **Evidencia**, adjunta el archivo (PDF, JPG, PNG, Word o Excel). Aparece el mensaje *"Evidencia '…' subida correctamente."* y los botones **Ver** y **x** (quitar).
   4. Escribe el **Hallazgo**, la **Oportunidad de Mejora**, el **Responsable**, el **Plazo** y el **Seguimiento**.
3. Las filas en *No cumple* se pintan de rojo y las *En proceso* de amarillo.
4. Guarda con frecuencia:
   - **Guardar cambios** → guarda y deja la auditoría **En Proceso**.
   - **Guardar borrador** → guarda y la deja en **Borrador**.
   - **Finalizar auditoría** → guarda y la marca **Finalizada** (*"Auditoría finalizada y guardada correctamente."*).
5. Para volver al listado usa **Listado Auditorías**.

> Importante: los cambios de la tabla (incluida la evidencia adjunta o quitada) solo quedan registrados en la auditoría cuando pulsas uno de los botones de guardar.

### Exportar una auditoría

1. **Exportar PDF:** abre la ventana de impresión del navegador; elige "Guardar como PDF".
2. **Exportar Excel:** descarga un archivo `.csv` (se abre en Excel) con cláusula, requisito, cumplimiento, puntaje, evidencia, hallazgo, oportunidad, responsable, plazo y seguimiento. También está en el listado (ícono de hoja de cálculo).

> ⚠ Por confirmar: la descarga **Exportar Excel** desde las auditorías del proyecto puede no generar el archivo correctamente. Si falla, use **Exportar PDF** o haga la auditoría desde el módulo corporativo (capítulo 11), cuya exportación usa otro mecanismo.

### Eliminar una auditoría

1. En el listado, pulsa el ícono de papelera.
2. Confirma con **Sí** (o **No** para cancelar). La eliminación es **definitiva** (no hay papelera).

### Registrar o editar un PPI

1. Entra a **PPIs** y pulsa **Nuevo PPI**.
2. Diligencia **Código**, **Descripción** y **Responsable** (obligatorios), y opcionalmente **Disciplina**, **Actividad WBS**, **Frente de Trabajo**, **Fecha Planeada**, **Estado** y **Observaciones**.
3. Pulsa **Guardar**.
4. Para cambiar el estado (por ejemplo, de Pendiente a Aprobado) pulsa el lápiz de la fila, ajusta y **Guardar**.
5. Filtra por estado con la lista **Todos los estados** o busca por código o descripción.

### Registrar un equipo de medición y su calibración

1. Entra a **Calibración Equipos** y pulsa **Nuevo Equipo**.
2. Diligencia **Código**, **Nombre del Equipo** y **Responsable** (obligatorios); además Serial, Marca, Modelo, N° Certificado, **Fecha Calibración**, **Fecha Vencimiento**, Ubicación y Observaciones.
3. Pulsa **Guardar**. La app calcula el estado y actualiza las tarjetas **Vigentes**, **Próximos a Vencer**, **Vencidos** y **Sin Certificado**.
4. Cuando el equipo se recalibre, pulsa el lápiz y actualiza las fechas y el número de certificado.

### Registrar un documento de calidad

1. Entra a **Control Documental** y pulsa **Nuevo Documento**.
2. Diligencia **Código**, **Nombre** y **Responsable** (obligatorios); Versión (por defecto 1.0), Tipo, Estado, Aprobado Por, Fecha Emisión, Fecha Vencimiento, **Ubicación SharePoint** y Disciplina.
3. Pulsa **Guardar**.
4. Al emitir una nueva versión o cuando el documento venza, edítalo con el lápiz y cambia la **Versión** y el **Estado**.

### Registrar una no conformidad de calidad (NCR)

1. Entra a **No Conformidades** (desde Calidad) y pulsa **Nueva NCR**.
2. Diligencia **Código**, **Título**, **Descripción** y **Responsable** (obligatorios); Severidad, Causa, Fecha Detección, **Fecha Compromiso**, Estado y Ubicación.
3. Pulsa **Guardar**.
4. Actualiza el estado con el lápiz a medida que avanza (En Revisión → En Implementación → Cerrada).

### Registrar una no conformidad general del proyecto

1. Menú del proyecto → **No Conformidades** → **+ Nueva NC** (o **+ Registrar primera NC** si no hay ninguna).
2. Diligencia **Número** (ej. NC-001), **Título**, **Descripción** y **Detectado Por** (obligatorios); Categoría (Calidad, Seguridad, Ambiental, Técnica, Documental), Severidad, Estado, Fecha Detección, Ubicación y **Fecha Cierre**.
3. Pulsa **Guardar**. Las tarjetas muestran **Total**, **Críticas**, **Abiertas** y **Cerradas**.
4. Usa **Editar** para actualizar o **Eliminar** → **Confirmar** para borrar.

> ⚠ Por confirmar: existen **dos registros de no conformidades que no están conectados**: el de Calidad (NCR, dentro de HSEQ) y el general del proyecto (menú **No Conformidades**). Definir con el equipo cuál es el registro oficial para no duplicar información.

### Consultar las acciones correctivas

1. Entra a **Acciones Correctivas**.
2. Revisa las tarjetas **Vencidas**, **En Proceso**, **Cerradas** y **Abiertas**, y la tabla con descripción, responsable, avance, fecha compromiso y estado. Puedes buscar por descripción o responsable.

> ⚠ Por confirmar: la pantalla dice que *"Las acciones correctivas se crean desde las No Conformidades"*, pero el formulario de NCR no tiene hoy una opción para crear acciones. Mientras tanto, esta pantalla es solo de consulta y normalmente aparecerá vacía.

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| Al crear la auditoría sale *"El auditor es obligatorio"* | El campo Auditor / Responsable está vacío | Escribe el nombre del auditor y vuelve a pulsar **Crear y Diligenciar Checklist**. |
| El formulario no guarda y aparece un mensaje en rojo bajo un campo (puede salir en inglés, p. ej. *"The … field is required"*) | Falta un campo marcado con * (Código, Descripción, Responsable, Nombre, Título…) | Completa los campos obligatorios y pulsa **Guardar**. |
| Sale *"Error al subir evidencia: …"* | El archivo pesa más de 10 MB o se interrumpió la carga | Reduce el tamaño del archivo (comprime el PDF o la foto) y vuelve a adjuntarlo. |
| La evidencia subida no aparece al volver a abrir la auditoría | No se pulsó **Guardar cambios** después de adjuntarla | Adjúntala de nuevo y pulsa **Guardar cambios**. |
| Un enlace **Ver** de evidencia antigua ya no abre el archivo | Los archivos se guardan en el servidor de la aplicación | ⚠ Por confirmar si las evidencias se conservan después de cada actualización de la plataforma; guarda siempre una copia en SharePoint. |
| El % de cumplimiento se ve en 0 % aunque hay requisitos llenos | Todos los requisitos llenos están en *No aplica* o *Sin evaluar*, o tienen puntaje 0 | Revisa el cumplimiento y el puntaje de cada requisito; recuerda que *No cumple* vale 0. |
| Sale *"Error al guardar: …"* | Se perdió la conexión o la sesión expiró | Recarga la página; si perdiste cambios, vuelve a diligenciarlos y guarda más seguido. |
| La descarga de **Exportar Excel** no se genera o sale *"Error al exportar: …"* | Falla conocida pendiente de confirmar | Usa **Exportar PDF** (impresión del navegador) mientras se revisa. |
| Un equipo aparece como **Sin Certificado** | No tiene Fecha Vencimiento | Edita el equipo y registra la fecha de vencimiento del certificado. |
| Un documento sigue en **Vigente** aunque la fecha está en rojo | El estado es manual | Edita el documento y cambia el Estado a **Vencido** o actualiza la versión y la fecha. |
| La pantalla de Acciones Correctivas está vacía | No hay forma de crear acciones desde la app todavía | Lleva el seguimiento en el campo Seguimiento de la auditoría o en la NCR (estado y fecha compromiso) mientras se habilita. |

---

## Relación con otros módulos

- **HSEQ del proyecto** (`/hseq`): puerta de entrada a Calidad, [Seguridad](07-hseq-seguridad.md), [Ambiental](09-hseq-ambiental.md) y [Social](10-hseq-social.md).
- **Auditorías corporativas** ([capítulo 11](11-hseq-auditorias.md)): usan la misma forma de diligenciar requisitos y el mismo cálculo de %. Las auditorías ISO 9001 creadas **dentro del proyecto no aparecen** en el tablero ni en el historial corporativo, y viceversa.
- **No Conformidades del proyecto** (menú del proyecto): registro general independiente de las NCR de Calidad (ver nota arriba).
- **Cronograma de Actividades** ([capítulo 2](02-cronograma-actividades.md)): el campo **Actividad WBS** del PPI es texto libre; no se enlaza automáticamente con la actividad del cronograma.
- **Documentos** ([capítulo 4](04-documentos.md)): el Control Documental de Calidad es un registro aparte de la planificación documental; no se sincronizan.

---

## Buenas prácticas

- Crea la auditoría el mismo día en que se realiza y guarda cada cierto tiempo con **Guardar cambios**.
- Marca **No aplica** solo cuando el requisito de verdad no aplica al proyecto: esos requisitos salen del cálculo del %.
- Para cada *No cumple* diligencia siempre **Hallazgo**, **Responsable** y **Plazo**; así el gráfico de Seguimiento de Acciones tiene sentido.
- Usa **Finalizar auditoría** solo cuando todos los requisitos estén evaluados (el contador "sin evaluar" debe quedar en 0).
- Guarda una copia de cada evidencia en SharePoint y usa el campo **Ubicación SharePoint** en los documentos.
- Revisa semanalmente la pantalla de calibración y recalibra antes de que un equipo pase a **Próximo a Vencer**.
- Usa códigos consecutivos y consistentes (PPI-001, NCR-001…) para facilitar la búsqueda.
