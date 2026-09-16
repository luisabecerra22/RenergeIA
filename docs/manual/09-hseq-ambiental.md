# 9. HSEQ — Ambiental

> **Para qué sirve:** registrar la gestión ambiental del proyecto: auditorías contra ISO 14001:2015, inspecciones ambientales de campo, residuos generados, matriz de aspectos e impactos, derrames, avistamientos de fauna y flora, y consulta de acciones correctivas ambientales.
> **Quién lo usa:** profesionales e inspectores ambientales, coordinación HSEQ, auditores, dirección de proyecto.
> **Dónde está:** menú del proyecto → **HSEQ** (`/proyectos/{id}/hseq`) → tarjeta **Ambiental** → **Ver Ambiental** (`/proyectos/{id}/hseq/ambiental`).

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Auditoría ISO 14001** | Revisión del proyecto contra los requisitos de ISO 14001:2015. Funciona igual que la auditoría ISO 9001 de Calidad (cumplimiento, puntaje, evidencia, hallazgo, seguimiento). |
| **Inspección ambiental** | Visita de campo con código, tipo (texto libre: residuos, ruido, suelos…), frente, inspector, estado y número de hallazgos encontrados y cerrados. Estados: Programada · Realizada · Cancelada · Vencida. |
| **Tipo de residuo** | Aprovechable · Peligroso · Orgánico · Escombros · No Aprovechable · Otro. La cantidad se registra en **kg**. |
| **Aspecto ambiental** | Actividad del proyecto que interactúa con el ambiente (ej. generación de residuos sólidos). |
| **Impacto** | Cambio que produce en el ambiente (ej. contaminación del suelo). |
| **Magnitud / Frecuencia / Duración** | Calificaciones de 1 a 5 del aspecto. |
| **Nivel de significancia** | Magnitud × Frecuencia × Duración (lo calcula la app). |
| **Derrame** | Evento de sustancia derramada (aceite, combustible…), con volumen en litros, causa raíz y acciones inmediatas. Usa los estados de no conformidad: Abierta · En Revisión · En Implementación · Cerrada · Cancelada. |
| **Estado de conservación** | Categoría de la especie, escrita con su sigla (ej. **LC** preocupación menor, **VU** vulnerable, **EN** en peligro, **CR** en peligro crítico). |

---

## Cómo funciona

1. **Todo se registra por proyecto** y cada pantalla muestra solo los datos del proyecto abierto.
2. **Lo que la app calcula sola:**
   - **Auditoría ISO 14001:** % de cumplimiento = suma de puntajes ÷ (2 × requisitos evaluados) × 100, sin contar *Sin evaluar* ni *No aplica*. Semáforo: 🟢 85 % o más · 🟡 70 % a 84 % · 🔴 menos de 70 %.
   - **Aspectos e impactos:** nivel = M × F × D y su clasificación:

     | Nivel | Clasificación | Color |
     |---|---|---|
     | 12 o más | **Significativo** | Rojo (Crítico) |
     | 8 a 11 | **Moderado** | Amarillo (En Riesgo) |
     | Menos de 8 | No significativo | Verde |

     La tabla se ordena del nivel más alto al más bajo.
   - **Residuos:** total en kg por cada tipo (tarjetas superiores; los peligrosos en rojo) y **Total mostrado** al pie de la tabla según el filtro.
   - **Inspecciones:** **Total Inspecciones**, **Realizadas**, **Total Hallazgos** y **Hallazgos Abiertos** (= hallazgos encontrados − hallazgos cerrados de todas las inspecciones).
   - **Derrames:** **Derrames Registrados**, **Total Litros Derramados** y **Último Derrame** (si no hay, la tarjeta muestra "Sin derrames").
   - **Fauna y Flora:** **Total Registros**, **Fauna**, **Flora** y **Con Estado Especial** (registros con estado de conservación escrito y distinto de "LC").
   - **Acciones ambientales:** **Total Acciones**, **Cerradas**, **En Proceso** y **Vencidas** (fecha compromiso pasada y no cerrada; la fila se pinta de rojo).
3. **Lo que se digita:** todos los formularios. Los **estados son manuales** (una inspección programada no pasa sola a "Vencida").
4. **Solo se crea y se elimina.** En Inspecciones, Residuos, Aspectos, Derrames y Fauna y Flora **no hay botón de editar**: para corregir un registro hay que eliminarlo y crearlo de nuevo.

> ⚠ Por confirmar: el tablero Ambiental (`/hseq/ambiental`) muestra **cifras fijas de demostración** (75 % de residuos aprovechables, 12 inspecciones, 2.4 t, etc.) y su panel de "Análisis Inteligente" es simulado. No se alimenta de los registros reales; use los contadores de cada pantalla.

---

## Paso a paso

### Entrar a Ambiental

1. Abre el proyecto → **HSEQ** → tarjeta **Ambiental** → **Ver Ambiental**.
2. Usa la barra superior: **Inspecciones**, **Residuos**, **Aspectos e Impactos**, **Derrames**, **Fauna y Flora**, **Acciones Correctivas**.

### Hacer una auditoría ISO 14001

> ⚠ Por confirmar: la pantalla de auditorías ISO 14001 del proyecto (`/proyectos/{id}/hseq/ambiental/checklist`) no tiene botón en el tablero Ambiental; hoy se abre escribiendo la dirección. Para auditorías corporativas ISO 14001 ver [capítulo 11](11-hseq-auditorias.md).

1. Abre `/proyectos/{id}/hseq/ambiental/checklist` y pulsa **Nueva Auditoría**.
2. Diligencia **Auditor / Responsable** (obligatorio), **Fecha de Auditoría**, **Estado inicial** y **Observaciones generales**.
3. Pulsa **Crear y Diligenciar Checklist**.
4. Diligencia cada requisito igual que en Calidad (ver [capítulo 8, "Diligenciar la auditoría"](08-hseq-calidad.md)): cumplimiento, puntaje, evidencia (hasta 10 MB), hallazgo, oportunidad de mejora, responsable, plazo y seguimiento. Usa los **💡** para ver la interpretación y los documentos auditables.
5. Guarda con **Guardar cambios**, **Guardar borrador** o **Finalizar auditoría**. Exporta con **Exportar PDF** o **Exportar Excel** (archivo `.csv` con nombre `ISO14001_fecha_auditor`).

### Registrar una inspección ambiental

1. Entra a **Inspecciones** → **Nueva Inspección**.
2. Diligencia **Código**, **Tipo Inspección** e **Inspector** (obligatorios); Frente de Trabajo, Fecha Inspección, Estado (por defecto *Programada*), **Hallazgos Encontrados**, **Hallazgos Cerrados** y Observaciones.
3. Pulsa **Guardar**.
4. Busca por tipo o inspector con la caja de búsqueda.

### Registrar residuos

1. Entra a **Residuos** → **Registrar Residuo**.
2. Elige el **Tipo de Residuo** y escribe la **Descripción** (obligatoria); agrega **Cantidad (kg)**, Frente de Trabajo, **Gestor Autorizado**, Fecha Registro y **Destino Final / Observaciones**.
3. Pulsa **Guardar**. Las tarjetas por tipo se actualizan.
4. Filtra por tipo con **Todos los tipos** para ver el total en kg de ese tipo al pie de la tabla.

### Agregar un aspecto e impacto ambiental

1. Entra a **Aspectos e Impactos** → **Nuevo Aspecto**.
2. Escribe el **Aspecto Ambiental** y el **Impacto Asociado** (obligatorios); Actividad y Medio Afectado (suelo, agua, aire, fauna…).
3. Califica **Magnitud (1-5)**, **Frecuencia (1-5)** y **Duración (1-5)**.
4. Escribe las **Medidas de Manejo** y Observaciones.
5. Pulsa **Guardar**. La app calcula el **Nivel** y lo clasifica (tarjetas **Aspectos Identificados**, **Significativos (≥ 12)**, **Moderados (8–11)**).

### Registrar un derrame

1. Entra a **Derrames** → **Registrar Derrame**.
2. Diligencia **Número**, **Sustancia** y **Reportado Por** (obligatorios); **Volumen (L)**, Frente de Trabajo, Fecha Evento, Estado (por defecto *Abierta*), **Causa Raíz** y **Acciones Inmediatas**.
3. Pulsa **Guardar**.

### Registrar fauna o flora

1. Entra a **Fauna y Flora** → **Nuevo Registro**.
2. Elige **Tipo** (Fauna o Flora) y escribe la **Especie** (nombre común, obligatorio); Nombre Científico, **Estado de Conservación** (sigla: LC, VU, EN, CR…), Ubicación, Fecha Registro, Registrado Por, **Acciones de Manejo** (ej. rescate y relocalización) y Observaciones.
3. Pulsa **Guardar**.
4. Filtra con **Fauna + Flora / Solo Fauna / Solo Flora** o busca por especie.

### Consultar acciones correctivas ambientales

1. Entra a **Acciones Correctivas**.
2. Revisa las tarjetas y la tabla (descripción, hallazgo de origen, responsable, plazo, avance y estado). Las vencidas se ven en rojo.

> ⚠ Por confirmar: la pantalla indica que las acciones *"se generan desde los hallazgos en Inspecciones Ambientales"*, pero el formulario de inspección solo guarda la **cantidad** de hallazgos y no permite crear acciones. Por ahora esta pantalla es solo de consulta y normalmente estará vacía.

### Eliminar un registro

1. En la fila, pulsa el ícono de papelera.
2. Confirma con **Sí** (o **No** para cancelar). La eliminación es definitiva.

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| El formulario no guarda y aparece un mensaje en rojo bajo un campo (puede salir en inglés) | Falta un campo obligatorio (*): Código, Tipo, Inspector, Descripción, Aspecto, Impacto, Número, Sustancia, Reportado Por o Especie | Completa los campos marcados con * y pulsa **Guardar**. |
| Aparece un mensaje de que el valor debe ser un número en Cantidad (kg), Volumen (L) o las calificaciones | Se escribió texto, unidades ("20 kg") o una coma mal ubicada | Escribe solo el número (ej. `20.5`) sin unidades. |
| Me equivoqué en un registro (cantidad, hallazgos, calificación) | Estas pantallas no tienen edición | Elimina el registro (papelera → **Sí**) y créalo de nuevo con los datos correctos. |
| Un aspecto quedó con un nivel absurdo (ej. 200) | La app no impide calificaciones fuera de 1 a 5 | Elimina el aspecto y regístralo con valores entre 1 y 5. |
| **Hallazgos Abiertos** sale negativo o no baja | Se registraron más hallazgos cerrados que encontrados, o la inspección no se actualizó | Revisa los números de cada inspección; para actualizarlos elimina y vuelve a crear la inspección. |
| Una especie amenazada no cuenta en **Con Estado Especial** o sale con color equivocado | La sigla se escribió distinta (ej. "lc", "Vulnerable") | Escribe la sigla en mayúsculas: **LC**, **VU**, **EN**, **CR**. Solo "LC" se considera sin estado especial. |
| La evidencia de la auditoría no sube: *"Error al subir evidencia: …"* | Archivo mayor a 10 MB o conexión interrumpida | Reduce el archivo y vuelve a intentar; luego pulsa **Guardar cambios**. |
| Las cifras del tablero Ambiental no coinciden con mis registros | El tablero tiene datos de demostración | Consulta los contadores dentro de cada sub-módulo. |

---

## Relación con otros módulos

- **HSEQ del proyecto** (`/hseq`): entrada común a [Calidad](08-hseq-calidad.md), [Seguridad](07-hseq-seguridad.md), Ambiental y [Social](10-hseq-social.md).
- **Auditorías corporativas** ([capítulo 11](11-hseq-auditorias.md)): existe también una auditoría ISO 14001 corporativa. Las auditorías hechas dentro del proyecto no aparecen en el tablero ni en el historial corporativo.
- **No Conformidades del proyecto** (menú del proyecto): se pueden registrar allí no conformidades con categoría **Ambiental**; no se conectan automáticamente con derrames ni inspecciones.
- **Informe Diario** ([capítulo 3](03-informe-diario.md)): los eventos ambientales del día no se copian automáticamente a este módulo; deben registrarse aquí.

---

## Buenas prácticas

- Registra el derrame el mismo día del evento, con causa raíz y acciones inmediatas, aunque el volumen sea pequeño.
- Registra los residuos con cada entrega al gestor autorizado y anota siempre el **Gestor Autorizado** (clave para los peligrosos).
- Actualiza la matriz de aspectos e impactos al iniciar cada frente de obra nuevo y revisa primero los **Significativos (≥ 12)**.
- Usa siempre las siglas oficiales de conservación (LC, VU, EN, CR) para que los indicadores funcionen.
- Revisa los datos antes de guardar: como no hay edición, corregir implica borrar y volver a crear.
- Conserva fotos y certificados (de disposición de residuos, de rescate de fauna) en SharePoint; este módulo no adjunta archivos, salvo las evidencias de la auditoría ISO 14001.
