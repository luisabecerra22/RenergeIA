# 10. HSEQ — Social

> **Para qué sirve:** llevar la gestión social del proyecto con las comunidades del área de influencia: quiénes son, reuniones realizadas, peticiones, quejas y reclamos (PQR), compromisos adquiridos, mano de obra y compras locales, y el archivo de actas y evidencias.
> **Quién lo usa:** profesionales sociales y de relacionamiento comunitario, coordinación HSEQ, compras y contratación (para los registros locales), dirección de proyecto y auditores.
> **Dónde está:** menú del proyecto → **HSEQ** (`/proyectos/{id}/hseq`) → tarjeta **Social** → **Ver Social** (`/proyectos/{id}/hseq/social`).

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Comunidad** | Comunidad del área de influencia: nombre, municipio, departamento, líder comunal, contacto y número de pobladores. |
| **Reunión comunitaria** | Encuentro con una comunidad registrada: tema, objetivo, lugar, facilitador, asistentes, temas tratados y acuerdos. |
| **PQR** | Petición, Queja, Reclamo, Denuncia o Solicitud recibida. Estados: Abierta · En Gestión · Cerrada · Vencida. |
| **Fecha Límite Respuesta** | Fecha máxima para responder la PQR. Por defecto la app propone **15 días** después de hoy. |
| **Compromiso social** | Obligación adquirida con la comunidad (por acta, PMA, PAGA u otra fuente). Estados: Pendiente · En Cumplimiento · Cumplido · Vencido · Cancelado. |
| **Contratación local** | Persona contratada para el proyecto, marcada como **Local** o **Foráneo**. |
| **Compra local** | Compra de bienes o servicios en COP, marcada como proveedor **Local** o **Foráneo**. |
| **Acta / evidencia** | Registro de un documento de soporte: Acta de Reunión, Acta de Compromiso, Registro Fotográfico, Lista de Asistencia, Certificación u Otro, con enlace al archivo en SharePoint o Drive. |

---

## Cómo funciona

1. **Todo se registra por proyecto.**
2. **Primero las comunidades.** Las reuniones exigen elegir una comunidad; las PQR y los compromisos pueden asociarse a una comunidad o quedar "sin comunidad".
3. **Lo que la app calcula sola:**
   - **Reuniones:** **Reuniones Realizadas**, **Total Participantes** (suma de asistentes), **Comunidades Activas** (comunidades registradas) y **Última Reunión**.
   - **PQR:** conteo por tipo y **Vencidas** = PQR cuya fecha límite ya pasó y no están cerradas; esas filas se pintan de amarillo y la fecha en rojo. Si la fecha límite se deja vacía, la app usa hoy + 15 días.
   - **Compromisos:** **Total**, **Cumplidos**, **En Cumplimiento** y **Vencidos** (según el estado elegido). La fecha de cumplimiento se pinta de rojo si ya pasó y el compromiso no está Cumplido.
   - **Contratación local:** **Total Contrataciones**, **Mano de Obra Local**, **% Contratación Local** (locales ÷ total) y **Activos Actualmente** (sin fecha de retiro). El % se ve en verde si es **60 % o más** y en amarillo si es menor.
   - **Compras locales:** **Total Compras**, **A Proveedores Locales**, **Valor Total** y **Valor Local** en COP.
4. **Lo que se digita:** todos los formularios. **Los estados son manuales**: una PQR vencida o un compromiso vencido **no cambian de estado solos**; la app solo los resalta.
5. **Solo se crea y se elimina.** Ninguna pantalla social tiene botón de editar: para corregir un registro hay que eliminarlo y crearlo de nuevo.

> ⚠ Por confirmar: como las PQR no se pueden editar, la **Respuesta / Solución** y el cambio de estado a *Cerrada* solo pueden registrarse creando de nuevo la PQR. Confirmar con el equipo el procedimiento mientras se habilita la edición.

> ⚠ Por confirmar: el tablero Social (`/hseq/social`) muestra **cifras fijas de demostración** (61 % de cumplimiento, 11 PQR, 28 trabajadores locales, $45M COP, etc.) y su panel de análisis es simulado. No refleja los registros reales.

---

## Paso a paso

### Entrar a Social

1. Abre el proyecto → **HSEQ** → tarjeta **Social** → **Ver Social**.
2. Usa la barra superior: **Comunidades**, **Reuniones Comunitarias**, **PQR / Quejas / Reclamos**, **Compromisos Sociales**, **Contratación Local**, **Compras Locales**, **Actas y Evidencias**.

### Registrar una comunidad

1. Entra a **Comunidades** → **Nueva Comunidad**.
2. Diligencia **Nombre**, **Municipio** y **Departamento** (obligatorios); Líder Comunal, Teléfono, Email, N° Pobladores y Observaciones.
3. Pulsa **Guardar**. La comunidad aparece como tarjeta con sus datos y los botones **Reuniones** y **PQR** (llevan a las listas generales del proyecto).

### Registrar una reunión comunitaria

1. Entra a **Reuniones Comunitarias** → **Nueva Reunión**. (Si no hay comunidades, el botón está desactivado y aparece *"Primero registra al menos una comunidad"* con el enlace **Ir a Comunidades →**.)
2. Diligencia **Título / Tema**, **Comunidad**, **Lugar**, **Facilitador** y **Objetivo** (obligatorios); Fecha, Asistentes, **Temas Tratados** y **Acuerdos / Compromisos**.
3. Pulsa **Guardar**.
4. Si en la reunión se adquirieron compromisos, regístralos también en **Compromisos Sociales** y sube el acta en **Actas y Evidencias**.

### Registrar una PQR

1. Entra a **PQR / Quejas / Reclamos** → **Nueva PQR**.
2. Diligencia **Número**, **Solicitante** y **Descripción** (obligatorios); Tipo (por defecto *Queja*), Estado (por defecto *Abierta*), Comunidad, **Fecha Radicación**, **Fecha Límite Respuesta** (propuesta: hoy + 15 días) y Respuesta / Solución.
3. Pulsa **Guardar**.
4. Revisa a diario la tarjeta **Vencidas** y filtra por estado con **Todos los estados**; busca por número o solicitante.

### Registrar un compromiso social

1. Entra a **Compromisos Sociales** → **Nuevo Compromiso**.
2. Diligencia **Descripción** y **Responsable** (obligatorios); Estado, Comunidad, **Fuente / Origen** (ej. acta de reunión, PMA, PAGA), Fecha Compromiso, **Fecha Cumplimiento** y Observaciones.
3. Pulsa **Guardar**. La lista se ordena por fecha del compromiso.

### Registrar contratación local

1. Entra a **Contratación Local** → **Registrar Contratación**.
2. Diligencia **Nombre Persona**, **Cargo / Servicio** y **Municipio** (obligatorios); Empresa, **Fecha Ingreso**, **Fecha Retiro** (déjala vacía mientras la persona esté activa) y la casilla **Es local** (viene marcada).
3. Pulsa **Guardar**.

### Registrar una compra local

1. Entra a **Compras Locales** → **Registrar Compra**.
2. Diligencia **Proveedor**, **Descripción / Bien o Servicio** y **Municipio** (obligatorios); **Valor COP**, Fecha, N° Factura y la casilla **Proveedor local** (viene marcada).
3. Pulsa **Guardar**.

### Registrar un acta o evidencia

1. Entra a **Actas y Evidencias** → **Nueva Acta**.
2. Elige el **Tipo de Documento**; escribe **Título** y **Descripción** (obligatorios); Fecha, Comunidad / Parte, Responsable Elaboración, **Firmantes** (separados por coma) y **URL Archivo (SharePoint / Drive)**.
3. Pulsa **Guardar**. Si registraste URL, aparece un botón de enlace en la columna **Archivo** que abre el documento en otra pestaña.
4. Filtra por tipo con **Todos los tipos** o busca por título.

### Eliminar un registro

1. Pulsa el ícono de papelera en la fila (o en la tarjeta de la comunidad).
2. Confirma con **Sí** (o **No**). La eliminación es definitiva.

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| El botón **Nueva Reunión** está gris y sale *"Primero registra al menos una comunidad"* | No hay comunidades registradas en el proyecto | Pulsa **Ir a Comunidades →**, registra la comunidad y vuelve. |
| Pulso **Guardar** en la reunión y no pasa nada (el formulario sigue abierto) | La Comunidad quedó en *— Seleccionar —* | Elige la comunidad en la lista y vuelve a guardar. |
| El formulario no guarda y sale un mensaje en rojo bajo un campo (puede salir en inglés) | Falta un campo obligatorio (*) | Completa los campos marcados con * y pulsa **Guardar**. |
| Al elegir **— Sin comunidad —** después de haber elegido una comunidad aparece un error de validación en PQR o Compromisos | La lista no acepta volver a la opción vacía | Cierra el formulario con la **x**, vuelve a abrirlo y no selecciones comunidad. |
| Al eliminar una comunidad aparece la barra *"An unhandled error has occurred. Reload"* | La comunidad tiene reuniones (y posiblemente PQR o compromisos) asociados; la app no permite dejarlos sin comunidad | Pulsa **Reload**. Elimina primero las reuniones de esa comunidad o conserva la comunidad. ⚠ Por confirmar el comportamiento con PQR y compromisos asociados. |
| Una PQR sale en amarillo como vencida aunque ya la respondí | El estado sigue en Abierta / En Gestión (no hay edición) | Registra la PQR de nuevo con Estado **Cerrada** y la respuesta, y elimina la anterior (ver nota "Por confirmar"). |
| El **% Contratación Local** sale en amarillo | Menos del 60 % de las contrataciones registradas están marcadas como locales | Verifica que la casilla **Es local** esté bien marcada en cada registro; si el dato es real, escala a la gerencia. |
| La búsqueda de actas no encuentra por comunidad | La caja de búsqueda solo busca en el **título** | Usa el filtro por tipo y revisa la columna Comunidad / Parte, o incluye la comunidad en el título. |
| Un valor de compra se registró mal | No hay edición | Elimina la compra y regístrala de nuevo. |

---

## Relación con otros módulos

- **HSEQ del proyecto** (`/hseq`): entrada común a [Calidad](08-hseq-calidad.md), [Seguridad](07-hseq-seguridad.md), [Ambiental](09-hseq-ambiental.md) y Social.
- **Costos** ([capítulo 5](05-costos.md)): las **Compras Locales** son un registro aparte; no se cruzan con las órdenes de compra ni con el archivo de tesorería. Si una compra ya está en Costos, regístrala aquí solo para el indicador social.
- **Histogramas** ([capítulo 6](06-histogramas.md)): la **Contratación Local** no se alimenta de la nómina; se digita aquí.
- **Documentos** ([capítulo 4](04-documentos.md)) y SharePoint: las actas no se suben a la app; se guarda el enlace.
- **Auditorías** ([capítulo 11](11-hseq-auditorias.md)): las auditorías de Cliente o Interventoría pueden revisar la gestión social; las evidencias se toman de este módulo.

---

## Buenas prácticas

- Registra las comunidades antes de iniciar el relacionamiento; así las reuniones, PQR y compromisos quedan asociados.
- Numera las PQR con un consecutivo único (ej. PQR-001) y define siempre la **Fecha Límite Respuesta**.
- Después de cada reunión: registra la reunión, los compromisos que salieron y el acta con su enlace, el mismo día.
- Revisa cada semana las PQR **Vencidas** y los compromisos con fecha en rojo, y actualiza su estado.
- Los datos de contratación incluyen nombres de personas: registra solo lo necesario y no copies documentos de identidad ni datos sensibles en Observaciones.
- Verifica el enlace de SharePoint/Drive antes de guardar el acta (que abra y tenga permisos para el equipo).
- Como no hay edición, revisa bien cada formulario antes de pulsar **Guardar**.
