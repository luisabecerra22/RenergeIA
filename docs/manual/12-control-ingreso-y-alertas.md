# 12. Control de ingreso y alertas

> **Para qué sirve:** controlar qué proveedores, vehículos, maquinaria, equipos, herramientas y conductores/operadores pueden ingresar a la obra: sus documentos con vencimiento (SOAT, RTM, pólizas, licencia, ARL, examen médico, seguridad social), un semáforo de vencimientos y el flujo de aprobación por etapas (Compras → RRHH → HSE → Cliente → Proyecto).
> **Quién lo usa:** HSE, compras, recursos humanos, administración de obra, dirección de proyecto.
> **Dónde está:** menú lateral del proyecto → **Alertas**, o la tarjeta **Alertas** en el detalle del proyecto (`/proyectos/{id}/alertas`). La pantalla se titula **Alertas y Control de Documentos**.

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Proveedor** | Empresa dueña de los recursos o del personal (puede marcarse **Es Renergeia**). |
| **Recurso** | Vehículo, maquinaria, equipo o herramienta que ingresa a obra. Se identifica por su **Placa / Serial**. |
| **Persona** | Conductor, operador u otra persona externa. Puede asignarse como **Conductor / Operador** de un recurso. |
| **Dueño del documento** | El proveedor, el recurso o la persona a quien pertenece un documento. |
| **Tipo de documento** | Catálogo fijo por categoría (ver tabla abajo). Algunos **requieren vencimiento**. |
| **Vigente / histórico** | Al registrar un documento nuevo del mismo tipo para el mismo dueño, el anterior pasa a **histórico** (se ve atenuado) y solo el nuevo cuenta para las alertas. |
| **Semáforo de vencimiento** | Color según los días que faltan para vencer (ver *Cómo funciona*). |
| **Etapas de aprobación** | Revisión de cada recurso por **Compras**, **RRHH (Conductor/Operador)**, **HSE (Máquina/Vehículo)**, **Aprobación Cliente** y **Aprobación Proyecto**. |
| **Estado de etapa** | Pendiente · En Revisión · Con Comentarios · Aprobado · N/A. |
| **Estado general del recurso** | Resumen de sus etapas: Aprobado · En proceso · Con comentarios · Pendiente · Sin iniciar. |
| **Cuello de botella** | La etapa con el mayor promedio de días hasta aprobación. |

**Catálogo de documentos**

| Categoría | Con vencimiento (aparecen en alertas y matriz) | Sin vencimiento (se marcan como entregados) |
|---|---|---|
| Equipo (recurso) | SOAT · RTM (Revisión Técnico-Mecánica) · Póliza Responsabilidad Civil · Póliza Todo Riesgo | Licencia de Tránsito · Tarjeta de Registro de Maquinaria · Hoja de Vida de Equipo · Ficha Técnica · Registro de Mantenimiento P/C · Preoperacional · Declaración de Importación · Inspección de Equipo |
| Persona | Licencia de Conducción · ARL · Examen Médico Ocupacional · Seguridad Social | — |
| Proveedor | Póliza RC Contratista | RUT · Cámara de Comercio |

---

## Cómo funciona

### Lo que calcula la app sola

**Semáforo de vencimiento** (se calcula cada vez contra la fecha de hoy; no se digita):

| Color | Tarjeta | Regla |
|---|---|---|
| Verde | Vigentes | Faltan más de 30 días |
| Amarillo | Vence en 21-30 días | Faltan de 21 a 30 días |
| Naranja | Vence en 10-20 días | Faltan de 10 a 20 días |
| Rojo | Vence en <10 días | Faltan de 0 a 9 días (incluye el día del vencimiento) |
| Morado oscuro | Vencidos | La fecha ya pasó |
| Gris | Sin fecha | El documento no tiene fecha de vencimiento |

En las tablas, amarillo, naranja y rojo muestran el texto **Vence pronto**.

**Etapas de un recurso:** al crear un recurso (a mano o por Excel) la app crea sus cinco etapas en **Pendiente**. Si el recurso **no tiene conductor/operador** al crearlo, la etapa **RRHH** queda en **N/A**.

**Estado general del recurso** (en este orden):

1. Alguna etapa **Con Comentarios** → **Con comentarios** (naranja).
2. Todas **Aprobado** o **N/A** → **Aprobado** (verde).
3. Alguna **Aprobado** o **En Revisión** → **En proceso** (azul).
4. Si no → **Pendiente** (gris). Sin etapas → **Sin iniciar**.

**Fecha de aprobación:** al poner una etapa en **Aprobado**, si no tiene fecha, la app pone la de hoy. Se puede corregir a mano.

**Dashboard:** los días de cada etapa se cuentan desde que el recurso **ingresó al sistema** hasta la **fecha de aprobación**. Solo cuentan las etapas aprobadas y con fecha.

**Tiempo real:** si otra persona modifica proveedores, recursos, personas, documentos o etapas del mismo proyecto, tu pantalla se actualiza sola.

### Lo que se digita o se importa

- Proveedores, recursos, personas y sus documentos (a mano o con **Importar Excel**).
- Los estados, responsables, fechas y comentarios de cada etapa.

### Importación del Excel de Control de Ingreso

- Usa la hoja **REGISTRO** (si no existe, la primera hoja). Los encabezados deben estar en las **primeras 6 filas**.
- Columnas reconocidas (sin importar tildes ni mayúsculas): **Nombre** (proveedor), **NIT**, **Tipo de Recurso**, **Descripción del Equipo**, **Placa**, **Fecha Inicio Contrato**, **Fecha Fin Contrato**, **Nombre Conductor**, **Cédula Conductor**, **Venc. SOAT**, **Venc. RTM**, **Venc. Póliza RC**, **Venc. Póliza Todo Riesgo**, **Venc. Licencia**, **Venc. ARL**, **Venc. Examen Médico**, **Revisión de Seguridad Social**. Son obligatorias **Tipo de Recurso** y **Descripción del Equipo**.
- **Proveedor:** se reconoce por nombre; si no existe se crea (si el nombre contiene "Renergeia" queda marcado como Renergeia).
- **Recurso:** se reconoce por **placa**; si no existe se crea. El tipo se deduce del texto: contiene "vehículo" → Vehículo; "maquinaria" → Maquinaria; "herramienta" → Herramienta; cualquier otro → Equipo.
- **Conductor:** se reconoce por cédula y, si no, por nombre; si no existe se crea con rol *Conductor / Operador*. Si dice "No aplica" se ignora.
- **Documentos:** por cada fecha de vencimiento con valor se crea un documento vigente. Los textos "NO APLICA" o vacíos se ignoran.
- Se **omiten** las filas sin proveedor o sin descripción ni placa.
- Al final aparece el resumen: *Importado: X proveedores, X recursos, X personas y X documentos nuevos. X filas omitidas…*

> ⚠ Por confirmar: al reimportar el mismo Excel, los recursos existentes (misma placa) no se duplican, pero **sus datos no se actualizan** (descripción, proveedor, fechas de contrato) y **los documentos se vuelven a crear** sin pasar los anteriores a histórico. Revisa si aparecen documentos repetidos en la lista.

---

## Paso a paso

### Revisar las alertas de vencimiento

1. Entra a **Alertas** del proyecto. Se abre la pestaña **Alertas**.
2. Mira las cinco tarjetas de conteo (Vigentes, Vence en 21-30 días, Vence en 10-20 días, Vence en <10 días, Vencidos).
3. Elige la vista:
   - **Matriz:** una fila por recurso con su proveedor, tipo, placa y conductor, y una columna por documento con vencimiento (del equipo y de su conductor). Cada celda muestra la fecha de vencimiento con su color; **—** significa que no hay documento.
   - **Lista:** todos los documentos vigentes con fecha, ordenados del que vence primero, con **Categoría**, **Dueño**, **Documento**, **Vencimiento**, **Días** (negativos = vencido), **Estado** y **Responsable**.
4. Usa el buscador **Buscar por nombre, placa o documento...** para filtrar.

### Registrar un proveedor

1. Abre la pestaña **Proveedores** y pulsa **Nuevo Proveedor**.
2. Escribe el **Nombre** (obligatorio) y el **NIT**; marca **Es Renergeia** si aplica.
3. Pulsa **Guardar**.
4. Para modificarlo usa el lápiz; para eliminarlo, la papelera.

### Registrar un recurso (vehículo, maquinaria, equipo o herramienta)

1. Si tiene conductor u operador, **regístralo primero** en la pestaña **Personal** (así la etapa RRHH queda pendiente y no en N/A).
2. Abre la pestaña **Recursos** y pulsa **Nuevo Recurso**.
3. Completa **Tipo**, **Descripción** (obligatoria), **Placa / Serial**, **Proveedor**, **Conductor / Operador**, **Inicio Contrato** y **Fin Contrato**.
4. Pulsa **Guardar**. La app crea automáticamente sus cinco etapas de aprobación.

### Registrar una persona (conductor u operador)

1. Abre la pestaña **Personal** y pulsa **Nueva Persona**.
2. Completa **Nombre** (obligatorio), **Cédula**, **Rol** y **Proveedor**.
3. Pulsa **Guardar**.

### Cargar un documento

1. En **Proveedores**, **Recursos** o **Personal**, pulsa el botón **📄** (muestra cuántos documentos tiene) en la fila del dueño.
2. En el panel **Documentos — [nombre]**, elige el **Tipo de documento** (solo aparecen los de esa categoría).
3. Si vence, escribe la **Fecha vencimiento**. Si no vence, marca **Entregado**.
4. Opcional: adjunta el **Archivo** (hasta 50 MB), el **Responsable (nombre)**, el **Responsable (correo)** y **Observaciones**.
5. Pulsa **+ Agregar**.
6. El documento aparece en la tabla inferior. Si ya había uno vigente del mismo tipo, el anterior queda como **(histórico)**.
7. Cierra el panel con la **X** del encabezado.

### Renovar un documento vencido

1. Abre los documentos del dueño (botón **📄**).
2. Agrega un documento **del mismo tipo** con la nueva fecha de vencimiento y, si lo tienes, el archivo.
3. Pulsa **+ Agregar**. El documento viejo pasa a histórico y la alerta desaparece.

### Llevar el flujo de aprobación de un recurso

1. Abre la pestaña **Recursos**.
2. En la columna **Estado**, pulsa el botón con el estado general del recurso.
3. En el panel **Etapas — [recurso]**, para cada etapa:
   - cambia el **Estado** en la lista desplegable,
   - escribe el **Responsable**,
   - ajusta la **Fecha aprobación** si hace falta,
   - escribe los **Comentarios**.
4. Cada cambio **se guarda al salir del campo**; no hay botón Guardar.

### Importar el Excel de Control de Ingreso

1. Pulsa **Importar Excel** (arriba a la derecha, visible en todas las pestañas).
2. Elige el archivo `.xlsx` o `.xls` (hasta 15 MB).
3. Lee el mensaje de resultado (verde si importó, rojo si hubo error).
4. Revisa la pestaña **Alertas → Matriz** para confirmar los datos.

### Ver el dashboard de aprobaciones

1. Abre la pestaña **Dashboard**.
2. Revisa las tarjetas: **Recursos en seguimiento**, **Aprobado**, **En proceso**, **Con comentarios** y **Pendiente**.
3. En **Tiempos promedio por etapa** revisa promedio, máximo, mínimo y número de aprobadas. La etapa en rojo con la etiqueta **Cuello de botella** es la más lenta.
4. En **Distribución de estados por etapa** ves cuántos recursos hay en cada estado por etapa.

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| Mensaje *Selecciona un tipo de documento.* | Se pulsó **+ Agregar** sin elegir el tipo. | Elige el **Tipo de documento** y vuelve a agregar. |
| *Documento guardado, pero el archivo no se pudo subir: …* | El archivo no cumple las reglas de carga (tamaño, tipo) o falló la subida. | El registro ya quedó creado. Revisa el archivo; para adjuntarlo, agrega de nuevo el documento del mismo tipo con el archivo (el anterior queda histórico). |
| *No se reconoce el formato del archivo…* | El Excel no tiene los encabezados **Tipo de Recurso** y **Descripción del Equipo** en las primeras 6 filas. | Usa la plantilla de Control de Ingreso o corrige los encabezados. |
| *El archivo no tiene filas de datos.* | La hoja usada está vacía o solo tiene encabezados. | Verifica que los datos estén en la hoja **REGISTRO** (o en la primera hoja). |
| El resumen de importación muestra muchas *filas omitidas* | Filas sin proveedor (columna **Nombre**) o sin descripción ni placa. | Completa esas columnas en el Excel y vuelve a importar. |
| Un documento importado no aparece | La fecha estaba como texto no reconocible o decía "NO APLICA". | Corrige la fecha en el Excel (formato fecha) o cárgala a mano con el botón **📄**. |
| La etapa **RRHH** está en N/A aunque el recurso tiene conductor | El conductor se asignó después de crear el recurso. | Abre las **Etapas** del recurso y cambia RRHH a **Pendiente**. |
| No aparece la columna de un documento en la **Matriz** | Solo se muestran los documentos con vencimiento de equipos y personas; los de proveedor y los que no vencen no van en la matriz. | Consúltalos en la vista **Lista** o en el botón **📄** del dueño. |
| La matriz muestra **—** en los documentos del conductor | El recurso no tiene **Conductor / Operador** asignado. | Edita el recurso (lápiz) y asígnale el conductor. |
| *Error al importar: …* | Archivo dañado, protegido con contraseña o mayor a 15 MB. | Guarda una copia sin contraseña en `.xlsx` y vuelve a intentar. |
| Se borró un proveedor, recurso o persona por error | La papelera **no pide confirmación** y elimina también sus documentos (y, en recursos, sus etapas). | No hay papelera de recuperación: vuelve a crear el registro y sus documentos. |

> ⚠ Por confirmar: qué pasa al eliminar un proveedor que todavía tiene recursos o personas asociados (podría no permitirse o dejarlos sin proveedor). Antes de eliminar un proveedor, reasigna o elimina sus recursos y personas.

---

## Relación con otros módulos

- **Documentos del proyecto** (capítulo [4. Documentos](04-documentos.md)) es un módulo distinto: controla la planificación documental (procedimientos, HSE, ingeniería). Este capítulo controla los documentos de **ingreso a obra** de terceros.
- **HSEQ — Seguridad** ([7](07-hseq-seguridad.md)): la etapa **HSE (Máquina/Vehículo)** y documentos como ARL, examen médico y seguridad social apoyan el control SST de contratistas.
- **Histogramas** ([6](06-histogramas.md)): el control de ingreso registra personas y equipos externos, pero **no alimenta** los histogramas; son registros independientes.

---

## Buenas prácticas

- Registra **primero las personas**, luego los recursos con su conductor asignado.
- Escribe siempre la **placa o serial**: es lo que evita duplicados al importar el Excel.
- Para renovar un documento, **agrega uno nuevo del mismo tipo**; no elimines el anterior (se conserva como histórico).
- Adjunta el archivo del soporte y el **responsable** de cada documento.
- Revisa la pestaña **Alertas** al menos una vez por semana y actúa sobre los **rojos** y **vencidos** antes de permitir el ingreso.
- Escribe **comentarios** en las etapas con estado **Con Comentarios**, para que quien sigue sepa qué falta.
- Usa el **Dashboard** para detectar el cuello de botella y hablar con el área responsable.
- Evita reimportar el mismo Excel completo; carga solo las filas nuevas.
