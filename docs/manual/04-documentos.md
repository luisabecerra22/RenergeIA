# 4. Documentos — Planificación documental

> **Para qué sirve:** llevar el control de todos los documentos del proyecto (procedimientos de construcción, documentación HSE e ingeniería): en qué estado está cada uno, en manos de quién (Renergeia o el Cliente), cuántos días lleva sin atender y quién es el responsable interno.
> **Quién lo usa:** control documental / QA-QC, coordinadores de área (civil, mecánico, eléctrico, HSE, calidad), dirección de proyecto.
> **Dónde está:** menú del proyecto → **Documentos** (`/proyectos/{id}/documentos`).

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Planificación Documental** | El Excel `FO-SI-GC-002-1 - Planificación Documental` del proyecto. Es la fuente de la lista de documentos. Tiene las hojas **Construcción**, **HSE** e **Ingeniería**. |
| **Pestañas** | **Procedimientos** (hoja Construcción), **HSE** e **Ingeniería**. |
| **Código Cliente / Código Renergeia** | Los dos códigos del documento. La app los usa para reconocer el documento cuando se vuelve a cargar la planificación. |
| **Estado** | Pendiente Emitir · Pendiente Validación · Validado con Comentarios · Validado · Informativos · No Validado. |
| **Transmittal** | Número del envío oficial (ej. HSEEXT-129, COS5SO-GY-019). En el Excel viene en la columna **Observaciones**. |
| **Rondas de revisión** | Hasta 4 pares de *Fecha de entrega* (al cliente) y *Fecha de devolución* (del cliente). La columna **Días** de cada ronda cuenta los días entre ambas (o hasta hoy si aún no se devuelve). |
| **Días sin atender** | Días desde el último movimiento registrado del documento (la fecha más reciente entre emisión, entregas, devoluciones y validación) hasta hoy. |
| **En cancha de** | Quién debe mover el documento: **Cliente** (está pendiente de validación) o **Renergeia** (pendiente por emitir o no validado). |
| **Responsable interno** | Persona de Renergeia a cargo del documento. Se asigna **por área** (ej. Mecánico → Coordinador mecánico) o directamente en el documento. |
| **Redline / As-Built** | Solo Ingeniería: seguimiento de los planos con cambios en obra (Redline) y los planos finales construidos (As-Built), con su % de avance y aprobación de Interventoría. |

---

## Cómo funciona

1. **La planificación se carga desde el Excel** con el botón **Cargar planificación**. La app lee las tres hojas de una vez y muestra una vista previa antes de guardar.
2. **No se duplican documentos.** Cada fila del Excel se compara con los documentos que ya existen, en este orden: Código Renergeia + Código Cliente → Código Renergeia → Código Cliente → Nombre.
3. **Lo que la app calcula sola (no se digita):**
   - Días de cada ronda de revisión.
   - **Días sin atender** y el **semáforo**:

     | Color | Regla |
     |---|---|
     | 🟢 En plazo | Menos de 4 días sin movimiento |
     | 🟡 Amarillo | De 4 a 7 días |
     | 🔴 Rojo | 8 días o más |
     | ⚪ Sin fecha | Está pendiente pero no tiene ninguna fecha registrada |
     | Al día | Validado, Validado con comentarios o Informativo (no requiere atención) |

   - **En cancha de:** Pendiente Validación → Cliente · Pendiente Emitir / No Validado → Renergeia · Validado → OK para construcción.
4. **Lo que se digita en la app:** responsables por área, correcciones puntuales en la tabla (botón lápiz), validación de documentos y avance de Redline/As-Built.
5. **Si la app y el Excel no coinciden:** cuando un documento se editó en la app **después** de la última carga, al cargar de nuevo la app **conserva lo de la app** y muestra la diferencia; puedes marcar **Usar Excel** documento por documento.

---

## Paso a paso

### Cargar o actualizar la planificación documental

1. Abre el proyecto y entra a **Documentos**.
2. Haz clic en **Cargar planificación** y selecciona el Excel de la planificación documental (`.xlsx`).
3. Revisa la ventana **Cargar planificación documental**:
   - **Fecha de actualización del archivo** (celda "Fecha de actualización" del Excel) y la última cargada. Si sale **⚠ Archivo desactualizado**, verifica que sea la versión correcta.
   - La tabla por hoja: **Documentos**, **Nuevos**, **Actualizados**, **Sin cambios** y **Editados en la app**.
   - **Documentos editados en la app después de la última carga:** muestra campo por campo lo que hay en la app y lo que trae el Excel. Marca **Usar Excel** solo en los que quieras reemplazar.
   - **Ver cambios que se actualizarán:** detalle de lo que cambia.
   - **Datos a revisar en el Excel:** celdas que la app tuvo que interpretar (dos fechas en una celda, fechas mal escritas, áreas no reconocidas).
   - **Documentos de la app que no están en el archivo:** no se borran; revisa si sobran.
4. Haz clic en **Confirmar y actualizar** (o **Cargar de todas formas** si el archivo es anterior).
5. Aparece el mensaje *"Planificación cargada: X nuevos, Y actualizados…"* y la tabla se refresca.

### Asignar los responsables internos por área

1. En **Documentos**, haz clic en **Responsables**.
2. Para cada área (Civil, Mecánico, Eléctrico, General, Calidad, Ambiental, Seguridad, Comunicaciones) escribe **Cargo**, **Nombre** y **Correo**.
   - Usa el **mismo correo con el que la persona entra a RenergeIA**: así le funciona el filtro **Mis pendientes**.
   - Deja la fila vacía si el área no tiene responsable.
3. Haz clic en **Guardar**. La columna **Responsable interno** se llena con el responsable del área de cada documento.

> Si un documento necesita un responsable distinto al del área, edítalo (lápiz) y escribe el nombre en **Responsable interno**; ese manda sobre el del área.

### Revisar las alertas (qué está atrasado)

1. Elige la pestaña (**Procedimientos**, **HSE** o **Ingeniería**).
2. En la barra **Sin atender** verás los contadores **Rojo**, **Amarillo**, **En plazo** y **Sin fecha**, y **En cancha de Renergeia / Cliente**.
3. Haz clic en un contador para filtrar la tabla; la lista se ordena del más atrasado al menos atrasado. Haz clic de nuevo para quitar el filtro.
4. Pasa el mouse sobre el indicador de la columna **Sin atender** para ver la fecha del último movimiento y la observación del retraso.

### Ver mis pendientes

1. Haz clic en **Mis pendientes**.
2. La tabla muestra solo los documentos pendientes donde tú eres el responsable interno (por área o por documento).

### Editar un documento

1. Haz clic en el **lápiz** de la fila.
2. Modifica los campos en la tabla (estado, fechas, área, transmittal, responsable; en Ingeniería también Redline y As-Built).
3. Haz clic en **Guardar cambios** en la barra amarilla (o **Cancelar**).

> La edición queda registrada con tu usuario y la fecha. En la siguiente carga del Excel, si ese documento trae otros valores, la app te lo mostrará como "Editado en la app".

### Validar un documento

1. En la fila, haz clic en el botón **Validar** (ícono de sello) — aparece en los documentos que no están Validados ni son Informativos.
2. Confirma con el botón verde.
3. El documento pasa a **Validado**, toma la fecha de validación de hoy (si no tenía) y guarda quién lo validó.

### Agregar un documento que no está en la planificación

1. Haz clic en **Nuevo**: se crea una fila en la pestaña actual en modo edición.
2. Completa los datos y haz clic en **Guardar cambios**.

> Lo ideal es que el documento también se agregue al Excel de la planificación, para que ambas fuentes coincidan.

### Exportar

- **Exportar**: descarga un Excel con los documentos filtrados de la pestaña, incluyendo responsable interno, días sin atender y en cancha de quién.
- **Informe PDF**: abre un informe con indicadores por estado, área y fase; usa el diálogo de impresión para guardarlo como PDF.

### Eliminar un documento

1. Haz clic en la **papelera** de la fila y confirma con el botón rojo.

> ⚠ La eliminación es definitiva. Si el documento sigue en el Excel, volverá a aparecer como nuevo en la siguiente carga.

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| "No se encontraron documentos…" al cargar | El archivo no tiene las hojas Construcción, HSE o Ingeniería, o no tiene la fila de encabezados con "Código Cliente" | Verifica que sea el formato FO-SI-GC-002-1 y que no se hayan renombrado las hojas ni el encabezado "Código Cliente". |
| Aviso "No se encontró la hoja de …" | Falta una de las tres hojas | Se cargan las demás. Si la hoja existe con otro nombre, renómbrala a Construcción, HSE o Ingeniería. |
| ⚠ Archivo desactualizado | La fecha de actualización del Excel es anterior a la ya cargada | Busca la versión más reciente en SharePoint. Solo usa **Cargar de todas formas** si estás segura. |
| Aparecen documentos "Nuevos" que ya existían | Cambió el Código Renergeia y el Código Cliente a la vez, o el código está repetido en el Excel | Revisa los códigos en el Excel. Si quedó duplicado en la app, elimina la fila sobrante. |
| Un documento no se actualizó con el Excel | Se editó en la app después de la última carga | En la vista previa marca **Usar Excel** en ese documento y vuelve a confirmar. |
| "La celda trae 2 fechas… se tomó la más reciente" | En el Excel hay dos fechas escritas en la misma celda | Deja una sola fecha por celda en el Excel. |
| "Fecha mal escrita … se interpretó como …" | Falta una barra, ej. `03/042026` | Corrige la fecha en el Excel (`03/04/2026`). |
| "Área … no reconocida" | La columna Área tiene un valor fuera de la lista (ej. una fecha) | Usa la lista del Excel: Mecánico, Eléctrico, Civil, General, Calidad, Ambiental, Seguridad, Comunicaciones. |
| Muchos documentos en "Sin fecha" | Están pendientes por emitir y no tienen ninguna fecha | Registra al menos la fecha de emisión planeada/real para que empiecen a contar. |
| "Mis pendientes" sale en 0 aunque tengo documentos | El correo del responsable del área no coincide con tu usuario de RenergeIA | En **Responsables**, escribe el mismo correo con el que inicias sesión. |
| La columna Responsable interno dice "Sin asignar" | El área del documento no tiene responsable | Asigna uno en **Responsables** o directamente en el documento. |

---

## Relación con otros módulos

- **HSEQ – Calidad (Control documental):** gestiona los documentos del sistema de gestión (vigencia, aprobación); este módulo gestiona los entregables del proyecto.
- **Alertas / Control de ingreso:** documentos con vencimiento de personas, equipos y proveedores (otro tipo de documento).
- **Consolidado / gerencia:** el Excel exportado y el Informe PDF sirven de soporte para los informes semanales.

> ⚠ Por confirmar: las páginas de **versiones de archivo** de un documento (`/documentos/crear` y `/documentos/{id}`, para subir archivos Rev 0, Rev 1…) existen, pero hoy no hay un enlace desde la tabla. Definir si se enlazan o se retiran.

---

## Buenas prácticas

1. **Una sola fuente:** actualiza primero el Excel de la planificación y cárgalo; usa la edición en la app para correcciones puntuales o validaciones.
2. **Carga la planificación cada vez que se actualice** (idealmente semanal), siempre la versión más reciente.
3. **Lee "Datos a revisar en el Excel"** en cada carga y corrige el archivo fuente.
4. **Mantén los responsables por área al día** cuando cambie el equipo.
5. **Revisa el rojo primero** y, dentro de él, lo que está en cancha de Renergeia: es lo que depende de nosotros.
6. Escribe en el Excel la **Observación tiempo de retraso** cuando un documento se atrase: aparece en la app al pasar el mouse.
