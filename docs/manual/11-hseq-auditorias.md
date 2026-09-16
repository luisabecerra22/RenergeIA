# 11. HSEQ — Auditorías corporativas y consolidado anual

> **Para qué sirve:** hacer auditorías del sistema de gestión HSEQ de la compañía contra varias normas (ISO 9001, ISO 14001, ISO 45001, Decreto 1072, Resolución 0312) o auditorías de Cliente e Interventoría, con o sin proyecto asociado; consultar su historial; y generar el **Informe Consolidado HSEQ** anual de Pausas Activas, STC y OTS.
> **Quién lo usa:** auditores internos y externos, coordinación HSEQ corporativa, gerencia.
> **Dónde está:** menú lateral principal → **HSEQ** (fuera de los proyectos):
> - **Dashboard HSEQ** (`/hseq/dashboard`)
> - **Auditorías** → **ISO 9001**, **ISO 14001**, **ISO 45001**, **Decreto 1072**, **Resolución 0312**, **Aud. Cliente**, **Aud. Interventoría** (`/hseq/auditorias/norma/{norma}`) e **Historial** (`/hseq/auditorias/historial`)
> - **Inspecciones** → **Consolidado Anual** (`/hseq/inspecciones/consolidado`)

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Módulo corporativo** | Las auditorías de este capítulo no dependen de un proyecto: el proyecto es **opcional**. Son distintas de las auditorías ISO 9001 / ISO 14001 que se hacen dentro de un proyecto (capítulos [8](08-hseq-calidad.md) y [9](09-hseq-ambiental.md)). |
| **Norma** | ISO 9001:2015 · ISO 14001:2015 · ISO 45001:2018 · Decreto 1072/2015 · Res. 0312/2019 · Auditoría Cliente · Auditoría Interventoría. Cada una tiene su color e ícono. |
| **Tipo de auditoría** | **Interna**, **Cliente** o **Interventoría**. |
| **Proceso / Área** | Proceso auditado (ej. Gestión de calidad, Obras civiles, Compras). Obligatorio. |
| **Motor de auditoría** | La pantalla donde se diligencian los requisitos de la norma (igual para todas las normas). |
| **Cumplimiento / Puntaje / Seguimiento** | Igual que en Calidad: Sin evaluar · Cumple (2) · En proceso (1) · No cumple (0) · No aplica; seguimiento Pendiente · En proceso · Ejecutado. |
| **Estado de la auditoría** | Borrador · En Proceso · Finalizada. |
| **STC / OTS / Pausas Activas** | Inspecciones corporativas cargadas cada mes desde los reportes de Flokzu (ver capítulo [7](07-hseq-seguridad.md)). El consolidado anual mide su cumplimiento contra metas por persona. |

---

## Cómo funciona

### Auditorías

1. **Al iniciar una auditoría** la app carga automáticamente la lista de requisitos de la norma elegida, todos en *Sin evaluar*, y la deja en estado **En Proceso**.
2. **% de cumplimiento** = suma de puntajes ÷ (2 × requisitos evaluados) × 100; no cuentan *Sin evaluar* ni *No aplica*. Se guarda al pulsar cualquier botón de guardar.
3. **Semáforo dentro de la auditoría:** 🟢 *Cumplimiento Alto* (85 % o más) · 🟡 *Requiere Atención* (70 % a 84 %) · 🔴 *Crítico* (menos de 70 %).
4. **Colores del % en "Últimas auditorías" del Dashboard:** verde 80 % o más · ámbar 60 % a 79 % · naranja 40 % a 59 % · rojo menos de 40 %.
5. **Lo que calcula el Dashboard HSEQ:** **Auditorías totales**, **Finalizadas**, **En proceso**, **% cumplimiento promedio**, y por cada norma: número de auditorías, finalizadas y % promedio. Muestra las 6 auditorías más recientes.
6. **Lo que se digita:** los datos de inicio (auditor, proceso, fecha, proyecto, tipo, observaciones) y, en el motor, cada requisito.

> ⚠ Por confirmar: las normas **Auditoría Cliente** y **Auditoría Interventoría** no tienen lista de requisitos cargada. Al iniciarlas, la auditoría se crea **sin ítems** (0 requisitos) y no hay forma de agregar requisitos desde la app.

> ⚠ Por confirmar: no hay botón para **eliminar** auditorías corporativas.

### Consolidado anual

1. Se alimenta de los reportes mensuales cargados en **Inspecciones → STC**, **OTS** y **Pausas Activas** (solo las pausas corporativas, sin proyecto).
2. **Metas por persona:** Pausas Activas **40 por mes**, STC **5 por mes**, OTS **10 por mes** (en los encabezados: 480, 60 y 120 por año).
3. **Meses transcurridos:** si el año consultado es el actual, se toma el mes actual completo (ej. en septiembre = 9); si es un año anterior, 12.
4. **Cumplimiento por departamento** = promedio por persona (total del año de cada persona, promediado) ÷ (meta mensual × meses transcurridos) × 100, con tope de 100 %.
5. **Cumplimiento global** (círculos del resumen) = mismo cálculo con todas las personas.
6. **Semáforo:** ✓ verde **80 % o más** (Cumple) · ! amarillo **50 % a 79 %** (En proceso) · ✗ rojo **menos de 50 %** (Crítico).
7. **Meses cargados: X de 12** = el mayor número de meses con datos entre las tres inspecciones.

---

## Paso a paso

### Revisar el tablero corporativo

1. Menú lateral → **HSEQ** (o **Dashboard HSEQ**).
2. Revisa las tarjetas de indicadores y los **Módulos de Auditoría** por norma.
3. En cada norma, **Ver auditorías** abre su listado.
4. En **Últimas auditorías**, la flecha abre la auditoría. **Ver historial completo** lleva al historial.

> ⚠ Por confirmar: el botón **+** (Nueva auditoría) de cada tarjeta de norma en el Dashboard lleva a una dirección que no existe. Para crear una auditoría usa **Ver auditorías → Nueva Auditoría**.

### Iniciar una auditoría

1. Menú **HSEQ → Auditorías →** la norma deseada (o **Ver auditorías** en el Dashboard).
2. Pulsa **Nueva Auditoría**.
3. Diligencia:
   - **Auditor / Responsable** (obligatorio)
   - **Proceso / Área** (obligatorio)
   - **Fecha de Auditoría**
   - **Proyecto (opcional)** — deja *— Sin proyecto específico —* si es una auditoría de la compañía
   - **Tipo de Auditoría** (Interna, Cliente, Interventoría)
   - **Observaciones generales** (alcance, objetivos)
4. Pulsa **Iniciar Auditoría**. Se abre el motor de auditoría.

### Diligenciar la auditoría (motor)

1. Revisa el encabezado: estado, auditor, fecha, norma y proceso.
2. En **Dashboard de Cumplimiento** verás el % general, las tarjetas Cumple / En Proceso / No Cumple / Sin Evaluar y las gráficas **Cumplimiento por Cláusula**, **Distribución de Estados** y **Seguimiento de Acciones** (clic en el encabezado para ocultarlo).
3. En la tabla **Requisitos**, para cada requisito:
   1. Usa **💡** junto al requisito para ver la **interpretación** y **💡** en Evidencia para ver los **documentos auditables**.
   2. Elige el **Cumplimiento** (el puntaje se llena solo; puedes ajustarlo entre 0 y 2).
   3. Adjunta la **Evidencia** (PDF, JPG, PNG, Word, Excel; máximo 10 MB). Con **Ver** la abres y con **x** la quitas.
   4. Escribe **Hallazgo**, **Oportunidad de Mejora**, **Responsable**, **Plazo** y **Seguimiento**.
4. Guarda:
   - **Guardar cambios** → queda **En Proceso**.
   - **Guardar borrador** → queda **Borrador**.
   - **Finalizar auditoría** → queda **Finalizada**.
5. Exporta:
   - **Exportar PDF** → ventana de impresión del navegador ("Guardar como PDF").
   - **Exportar Excel** → archivo `.csv` con todos los requisitos (nombre: norma y fecha).
6. **Listado Auditorías** te regresa al listado de la norma.

### Consultar el historial

1. Menú **HSEQ → Auditorías → Historial**.
2. Combina los filtros: **— Todas las normas —**, **— Todos los estados —**, fecha **Desde**, fecha **Hasta** y la caja **Buscar auditor, proceso...** (también busca por nombre de proyecto).
3. Arriba de la tabla verás el número de resultados y el **Promedio** de cumplimiento de lo filtrado.
4. La flecha de cada fila abre la auditoría.

### Generar el Informe Consolidado HSEQ anual

1. Verifica que estén cargados los reportes mensuales en **Inspecciones → STC**, **OTS** y **Pausas Activas** (botón **Cargar Excel**, elegir mes y año, seleccionar archivo, **Procesar**).
2. Menú **HSEQ → Inspecciones → Consolidado Anual**.
3. Escribe el **Año** y pulsa **Aplicar**.
4. Revisa el **Resumen Ejecutivo** (Pausas Activas, STC, OTS), la convención del semáforo y las tablas **Cumplimiento por Departamento** (personas, total, promedio por persona, meta acumulada y %).
5. Pulsa **Exportar PDF** y elige "Guardar como PDF" en la ventana de impresión. Los controles de la parte superior no salen en el PDF.

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| Al iniciar la auditoría sale *"Ingrese el nombre del auditor."* | Auditor / Responsable vacío | Escribe el nombre y pulsa **Iniciar Auditoría**. |
| Sale *"Ingrese el proceso o área a auditar."* | Proceso / Área vacío | Escribe el proceso (ej. Compras, Obras civiles) y vuelve a intentar. |
| Sale *"Error al crear la auditoría: …"* | Problema de conexión o de la base de datos | Recarga la página e intenta de nuevo; si se repite, reporta el mensaje completo. |
| La página muestra *"Norma no reconocida: …"* | La dirección tiene una norma mal escrita | Entra desde el menú **HSEQ → Auditorías**. Valores válidos: iso9001, iso14001, iso45001, decreto1072, resolucion0312, cliente, interventoria. |
| La auditoría de Cliente o Interventoría se abre con la tabla vacía y 0 % | Esas normas no tienen requisitos cargados | Registra los hallazgos en **Observaciones generales** o en un documento aparte mientras se define su lista (ver nota Por confirmar). |
| Sale *"Error al subir evidencia: …"* | Archivo de más de 10 MB o conexión interrumpida | Reduce el archivo, vuelve a adjuntarlo y pulsa **Guardar cambios**. |
| Mi auditoría ISO 9001 del proyecto no aparece en el Dashboard ni en el Historial | Las auditorías hechas dentro del proyecto no se incluyen en el módulo corporativo | Consúltala en el proyecto (capítulo 8). Para que sume en el corporativo, créala desde **HSEQ → Auditorías** eligiendo el proyecto. |
| El historial dice *"No hay auditorías con los filtros seleccionados."* | Los filtros son muy restrictivos (fechas, norma, estado) | Limpia los filtros (vuelve a "Todas") y borra la búsqueda. |
| El consolidado dice *"No hay datos para el año …"* | No se han cargado reportes de STC, OTS ni Pausas para ese año | Carga los reportes mensuales en cada pantalla de Inspecciones y pulsa **Aplicar** de nuevo. |
| Al cargar el Excel de STC/OTS/Pausas sale *"No se encontraron columnas…"* o *"El archivo no contiene datos."* | El archivo no es el reporte de Flokzu esperado: los encabezados deben estar en la **fila 2** y los datos desde la **fila 3** | Descarga de nuevo el reporte original sin modificar encabezados: STC → *Inspector (STC)* y *Departamento (STC)*; OTS → *Inspector interno (OTS)* y *Departamento (OTS)*; Pausas → *Nombre (RPA)* y *Departamento (RPA)*. |
| El % del consolidado bajó de un mes a otro sin razón aparente | Al empezar un mes, la meta ya suma ese mes completo; o falta cargar un mes | Verifica **Meses cargados: X de 12** y carga los meses faltantes. Ten en cuenta que a inicio de mes el % baja hasta que se cargue el reporte. |
| Cargué dos veces el mismo mes | La carga reemplaza los datos de ese mes y año | No hay duplicado: la última carga es la que queda. Si cargaste el archivo equivocado, vuelve a cargar el correcto para ese mes. |

---

## Relación con otros módulos

- **Calidad y Ambiental del proyecto** ([8](08-hseq-calidad.md), [9](09-hseq-ambiental.md)): usan la misma forma de diligenciar requisitos y el mismo cálculo de %, pero sus auditorías están separadas del módulo corporativo.
- **Seguridad** ([7](07-hseq-seguridad.md)): allí están las listas de chequeo ISO 45001 / Resolución 0312 del proyecto y el detalle de las cargas de STC, OTS y Pausas Activas que alimentan el consolidado.
- **Proyectos** ([1](01-proyectos.md)): una auditoría corporativa puede asociarse a un proyecto; su nombre aparece en el listado y en el historial.
- **Social** ([10](10-hseq-social.md)) y demás registros HSEQ: son la fuente de evidencias para las auditorías de Cliente e Interventoría.

---

## Buenas prácticas

- Define bien el **Proceso / Área**: es lo que permite comparar auditorías del mismo proceso en el historial.
- Asocia el **Proyecto** siempre que la auditoría sea sobre una obra específica; déjalo vacío solo para procesos transversales de la compañía.
- Evalúa todos los requisitos antes de **Finalizar auditoría**; el % solo considera lo evaluado y puede verse artificialmente alto si faltan requisitos.
- Para cada *No cumple* registra Hallazgo, Responsable, Plazo y Seguimiento, y actualiza el Seguimiento en las siguientes revisiones.
- Guarda una copia de las evidencias en SharePoint. ⚠ Por confirmar si los archivos adjuntos se conservan después de cada actualización de la plataforma.
- Carga los reportes de STC, OTS y Pausas Activas en los primeros días de cada mes para que el consolidado anual sea confiable.
- Exporta el Informe Consolidado en PDF al cierre de cada trimestre y del año, y archívalo con fecha de corte.
