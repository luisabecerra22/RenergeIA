# 7. HSEQ — Seguridad y Salud en el Trabajo

> **Para qué sirve:** reúne en un solo lugar la gestión de Seguridad y Salud en el Trabajo (SST) de cada proyecto: plan de trabajo HSE, matriz de riesgos IPERV según GTC 45 (con apoyo de inteligencia artificial a partir de una foto), inspecciones y observaciones, EPP y dotación, capacitaciones, incidentes, permisos de trabajo, ATS, pausas activas y las autoevaluaciones ISO 45001 y Resolución 0312. Con lo que se registra, la app calcula indicadores y tableros.
> **Quién lo usa:** coordinadores y profesionales HSE (registran y actualizan), supervisores e inspectores de campo (inspecciones, ATS, permisos, EPP) y gerencia (tableros e indicadores).
> **Dónde está:** **Proyectos** → abrir el proyecto → tarjeta **HSEQ** (`/proyectos/{id}/hseq`) → **Ver Seguridad →** (`/proyectos/{id}/hseq/seguridad`). Dentro de Seguridad hay una barra de pestañas:
>
> | Pestaña | URL relativa |
> |---|---|
> | **Dashboard** | `/proyectos/{id}/hseq/seguridad` |
> | **Plan de Trabajo** | `/proyectos/{id}/hseq/seguridad/plan-trabajo` |
> | **Inspecciones** (sub-pestañas **Inspecciones de Seguridad** y **OTS**) | `/proyectos/{id}/hseq/seguridad/inspecciones` · `/proyectos/{id}/hseq/seguridad/ots` |
> | **Indicadores** | `/proyectos/{id}/hseq/seguridad/indicadores` |
> | **EPP / Dotación** | `/proyectos/{id}/hseq/seguridad/epp` |
> | **Matriz Riesgos** | `/proyectos/{id}/hseq/seguridad/matriz` |
> | **Incidentes** | `/proyectos/{id}/hseq/seguridad/incidentes` |
> | **Capacitaciones** | `/proyectos/{id}/hseq/seguridad/capacitaciones` |
> | **Acciones** | `/proyectos/{id}/hseq/seguridad/acciones` |
>
> Pantallas que existen pero **no tienen pestaña ni botón de acceso** (se abren escribiendo la dirección en el navegador): STC `/hseq/seguridad/stc`, ATS/AST `/hseq/seguridad/ats`, Permisos de Trabajo `/hseq/seguridad/permisos`, Pausas Activas `/hseq/seguridad/pausas-activas`, Checklist ISO 45001 `/hseq/seguridad/checklist` y Resolución 0312 `/hseq/seguridad/resolucion0312` (todas precedidas de `/proyectos/{id}`).
>
> ⚠ Por confirmar: si estas seis pantallas deben quedar enlazadas en la barra de Seguridad. Mientras tanto, guarde las direcciones en favoritos.

---

## Conceptos clave

| Término | Qué significa en RenergeIA |
|---|---|
| **SST** | Seguridad y Salud en el Trabajo. |
| **GTC 45** | Guía técnica colombiana para identificar peligros y valorar riesgos. Es la metodología de la Matriz IPERV. |
| **IPERV** | Identificación de Peligros, Evaluación y Valoración de Riesgos. Cada fila de la matriz es un riesgo. |
| **ND – Nivel de Deficiencia** | 10 Muy Alto (MA) · 6 Alto (A) · 2 Medio (M) · 0 Bajo (B). |
| **NE – Nivel de Exposición** | 4 Continua (EC) · 3 Frecuente (EF) · 2 Ocasional (EO) · 1 Esporádica (EEsp). |
| **NP – Nivel de Probabilidad** | ND × NE (lo calcula la app). |
| **NC – Nivel de Consecuencia** | 100 Mortal (M) · 60 Muy Grave (MG) · 25 Grave (G) · 10 Leve (L). |
| **NR – Nivel de Riesgo** | NP × NC (lo calcula la app). Define la aceptabilidad y el color. |
| **Aceptabilidad** | I — Crítico / No aceptable (NR ≥ 4000, rojo) · II — No Aceptable (2000–3999, naranja) · III — Mejorable (1000–1999, ámbar) · IV — Aceptable con control (200–999, amarillo) · V — Aceptable (< 200, verde). |
| **Estado del riesgo** | **Activo** (sin controlar) → **EnControl** → **Controlado**; o **Eliminado**. |
| **Estado de validación IA** | **PendienteValidacion**, **EnRevision**, **Aprobado**, **Rechazado**. Indica qué decidió SST sobre la propuesta de la IA. |
| **Fuente del riesgo** | De dónde salió la fila: **InspeccionIA** (🤖), **Manual** (✏️, también los agregados desde la Biblioteca), entre otras. |
| **OTS** | Observación de Trabajo Seguro. |
| **STC** | En la pantalla aparece como "Seguridad en Trabajo en Casa". ⚠ Por confirmar el significado que usa la empresa. |
| **ATS / AST** | Análisis de Trabajo Seguro / Análisis Seguro de Trabajo, antes de ejecutar una actividad. |
| **Acto inseguro / Condición insegura** | Conteos que se digitan en cada inspección, OTS o STC. |
| **HHT** | Horas-Hombre Trabajadas; base de los índices de frecuencia e incidentalidad. |
| **PHVA** | Planear, Hacer, Verificar, Actuar. Se usa en el Plan de Trabajo y en la Resolución 0312. |
| **P / E** | En el Plan de Trabajo: **P**rogramado y **E**jecutado de cada mes. |

---

## Cómo funciona

### Lo que calcula la app sola

- **Tablero HSEQ del proyecto** (`/hseq`): hoy **muestra datos de demostración**, no datos reales. La propia pantalla lo avisa con el aviso *"Fase 1 — Estructura base. Los KPIs muestran datos de demostración"*. Los porcentajes por división, los "Días sin accidentes", la tendencia mensual y el "Análisis Inteligente HSEQ" (marcado **Próximamente**) son fijos. Úselo solo para navegar a Calidad, Seguridad, Ambiental o Social.
- **Dashboard de Seguridad**: sí usa datos reales del proyecto y muestra 7 indicadores circulares:
  - **Plan de Trabajo**: actividades en estado *Ejecutada* ÷ total de actividades del plan.
  - **Insp. Seguridad** y **OTS**: cantidad de registros de cada tipo.
  - **Hallazgos Abiertos**: suma de (hallazgos encontrados − cerrados) de todas las inspecciones.
  - **Acciones Vencidas**: acciones correctivas SST no cerradas con fecha de compromiso vencida.
  - **EPP / Dotación**: entregas "al día" (sin fecha de próxima entrega o con más de 30 días por delante) ÷ total de entregas.
  - **Capacitaciones**: capacitaciones ejecutadas ÷ capacitaciones planificadas.
  - La insignia de arriba (**Cumple / En Riesgo / Crítico — %**) es el promedio de Plan de Trabajo, Capacitaciones y EPP (solo de los que tienen valor mayor que cero). Colores: ≥ 80 % verde, ≥ 60 % amarillo, menor rojo.
  - Debajo aparece la tabla **Actividades Vencidas del Plan de Trabajo** (máximo 10): fecha planificada anterior a hoy y estado distinto de *Ejecutada* o *Cancelada*.
- **Plan de Trabajo**: el **Cumplimiento** = actividades con estado *Ejecutada* ÷ total. Los gráficos cuentan cuántas casillas **P** y **E** hay marcadas por mes. Marcar casillas **no** cambia el estado de la actividad: el estado se elige a mano.
- **Matriz IPERV**: al guardar un riesgo la app calcula NP, NR y la aceptabilidad a partir de ND, NE y NC.
- **Biblioteca de peligros → Agregar a Matriz**: la biblioteca trae un "NR sugerido" y la app lo traduce a valores ND/NE/NC aproximados. Por eso el NR que queda en la matriz **puede no ser igual** al NR sugerido de la tarjeta (por ejemplo, un peligro con NR sugerido 4000 queda con ND 10, NE 4, NC 100 = 4000; uno de 3000 queda en 2400). Revise y ajuste la valoración después de agregarlo.
- **Matriz de Riesgos (inicio), Dashboard SST, Mapa de Riesgos y Acciones Correctivas** leen los riesgos de la matriz: totales, críticos (NR ≥ 2000), controlados (Controlado + Eliminado), distribución por nivel, riesgos por área (top 8) y estado de control.
- **Mapa de Riesgos**: agrupa los riesgos **por área** en tarjetas. Cada tarjeta toma el color del NR más alto del área, muestra activos/controlados/total, el % controlado y los 4 peligros más críticos.
- **Inspecciones de Seguridad (tablero)**: 12 indicadores (inspecciones de seguridad, OTS, STC, hallazgos abiertos, actos inseguros, condiciones inseguras, acciones vencidas, plan de trabajo, pausas activas, capacitaciones, EPP e ISO 45001) y gráficos: hallazgos por área (top 8), actos vs. condiciones por área, estado de hallazgos, tendencia de 6 meses, inspecciones por mes, Pareto, ranking (por **Área**, **Frente**, **Inspector** o **Responsable**) y mapa de calor Área × tipo. Una fila se pinta de amarillo si tiene fecha de compromiso vencida y no está *Realizada*.
- **Indicadores**: con los eventos del año elegido y las HHT digitadas calcula:
  - **IF** = (accidentes leves + graves + fatales) × 200.000 ÷ HHT. Semáforo: ≤ 1 Bajo, ≤ 5 Medio, mayor Alto.
  - **II** = incidentes × 200.000 ÷ HHT. Semáforo: ≤ 5 Bajo, ≤ 20 Medio, mayor Alto.
  - **IG** (índice de gravedad): **siempre muestra 0**, porque la app todavía no registra días perdidos.
  - % cumplimiento Plan HSE (del año elegido), % capacitaciones, % ISO 45001 (promedio de las auditorías ISO 45001 del proyecto) y total de inspecciones.
- **EPP / Dotación**: marca como **Vencida** (rojo) una entrega cuya "Próxima Entrega" ya pasó y como **Pronto** (amarillo) la que vence en los próximos 30 días; muestra avisos arriba con un enlace **Ver** que filtra la tabla.
- **Capacitaciones**: cumplimiento, horas hombre (duración × asistentes), vencidas (planificadas con fecha pasada que no están ejecutadas ni canceladas), pendientes, asistentes, "este mes", y gráficos mensuales, por área y por tema.
- **Pausas activas**: la meta es **60 pausas por trabajador al mes**. % de cumplimiento = promedio de pausas ÷ 60 (máximo 100 %). Colores: ≥ 100 % verde, ≥ 70 % amarillo, menor rojo.
- **Permisos de trabajo**: la fecha de vencimiento se pone en rojo si ya pasó y el permiso sigue *Activo*. El estado **no** cambia solo a *Vencido*.
- **Checklist ISO 45001**: al elegir el cumplimiento de cada requisito se asigna el puntaje (Cumple = 2, En proceso = 1, No cumple = 0; puede corregirlo entre 0 y 2). % global = suma de puntajes ÷ (requisitos evaluados × 2). *No aplica* y *Sin evaluar* no cuentan. Semáforo: ≥ 85 % Cumplimiento Alto, ≥ 70 % Requiere Atención, menor Crítico.
- **Resolución 0312:2019**: cada ítem tiene un peso en puntos. Cumple = peso completo, En proceso = mitad del peso, No cumple = 0. La puntuación total es sobre 100 y se reparte por PHVA (Planear 25, Hacer 60, Verificar 5, Actuar 10). Clasificación: ≥ 85 **Aceptable**, 61–84 **Moderadamente aceptable**, ≤ 60 **Crítico**.

### Lo que hace la IA y sus límites

- **Nueva Inspección con IA** (dentro de Matriz Riesgos) envía a un servicio de inteligencia artificial el área, la actividad, la tarea, la observación del inspector y la foto adjunta. Devuelve una **propuesta**: peligros identificados, clasificación GTC 45, efectos posibles, controles faltantes, ND/NE/NC y NR, medidas de intervención, EPP requerido, documentos relacionados, un hallazgo redactado y una acción correctiva con responsable y plazo sugeridos.
- La propuesta **no es oficial** hasta que SST la valide. La pantalla lo dice: *"Propuesta generada por IA · No oficial hasta aprobación HSEQ"*.
- Límites conocidos:
  - La IA solo "ve" fotos **JPG, JPEG, PNG, GIF o WEBP**. El campo de evidencia también acepta PDF, Word o Excel, pero esos archivos **no se analizan** (la IA trabaja solo con el texto).
  - Si la IA responde un nivel que no reconoce, la app usa el valor más bajo de la escala (NE = 1, NC = 10), así que el NR puede quedar subestimado. Revise siempre los valores.
  - El **plazo sugerido** por la IA es texto ("Inmediato", etc.) y **no** se copia como fecha al riesgo; el campo Plazo queda vacío.
  - Como responsable del riesgo queda el **responsable sugerido por la IA**, no el que usted escribió en el formulario.
  - Una inspección guardada como **pendiente de revisión** no tiene hoy una pantalla para aprobarla después (solo se ve en **Evidencias**). Si quiere que alimente la matriz, apruébela en el momento o registre el riesgo a mano.
- ⚠ Por confirmar: el proveedor de IA de esta función. En el código, la inspección con IA usa la API de Anthropic (Claude) con una clave propia del servidor, mientras que otros análisis de RenergeIA usan Google Gemini. Si la clave o la cuota del proveedor fallan, la IA no responde (ver *Qué hacer en caso de…*).
- Los recuadros **"Análisis Inteligente"** de Inspecciones de Seguridad y de Capacitaciones **no** consultan a una IA externa: son frases armadas por la app con las mismas cifras de la pantalla. El de Inspecciones dice *"Beta · Conectando IA"*.

### Lo que se digita

Todo lo demás es captura manual: actividades y casillas del plan de trabajo (o importación desde Excel), inspecciones/OTS/STC con sus conteos de hallazgos, actos y condiciones, entregas de EPP con su próxima fecha de entrega (la app **no** la calcula; el texto "Reposición cada 4 meses" es solo una guía), capacitaciones planificadas y ejecutadas, incidentes, permisos, ATS, pausas activas por trabajador, las HHT en Indicadores y cada requisito de los checklists.

> ⚠ Por confirmar: las fotos y evidencias se guardan como archivos en el servidor de la aplicación. Confirme con el administrador que se conservan después de cada actualización de la plataforma; mientras tanto, guarde también una copia de las evidencias importantes en el repositorio documental del proyecto.

---

## Paso a paso

### Entrar a Seguridad y leer el Dashboard

1. En **Proyectos**, abra el proyecto y pulse la tarjeta **HSEQ**.
2. Pulse **Ver Seguridad →** en la tarjeta roja de Seguridad (recuerde que las cifras de esa página de inicio HSEQ son de demostración).
3. En **Dashboard** revise la insignia de cumplimiento y los 7 indicadores. Un indicador gris con "—" significa que aún no hay datos.
4. Si aparece la tabla roja **Actividades Vencidas del Plan de Trabajo**, pulse **Ver detalle del Plan de Trabajo** para actualizarlas.

### Construir el Plan de Trabajo HSE

1. Vaya a la pestaña **Plan de Trabajo** y elija el **año** en el selector de la esquina superior (de 2 años atrás a 2 años adelante).
2. Diligencie el encabezado azul: **Responsable**, **Cargo**, **Ubicación**, **Fecha Elaboración**, **Fecha Actualización**, **Objetivo General** e indicadores de **% Cumplimiento**, **% Eficacia** y **% Cobertura**.
3. Para cargar desde Excel: en **Importar Excel (.xlsx)** elija el archivo (máx. 20 MB) y pulse **Importar**. La app:
   - lee solo la **primera hoja**;
   - busca las columnas por su título: *Actividad*, *PHVA/Etapa/Ciclo*, *Responsable*, *Verificación/Frecuencia*, *Estado*, y los meses a partir de la columna *ENE/Enero* (dos columnas por mes: P y E);
   - da por marcada una casilla con 1, x, ✓ o un relleno verde;
   - si la actividad ya existe (mismo nombre), **la actualiza**; si no, la agrega;
   - también llena los campos del encabezado que estén vacíos.
4. Para agregar a mano pulse **Agregar Fila** y escriba la actividad, la etapa **PHVA**, el responsable, la **F. Verificación** y el **Estado**.
5. Haga clic en las celdas **P** (verde) y **E** (azul) de cada mes para marcarlas o desmarcarlas.
6. Cambie el **Estado** a *Ejecutada* cuando la actividad se cumpla (esto es lo que mueve el % de cumplimiento).
7. Pulse **Guardar Cambios**. Las filas nuevas, los cambios y las filas borradas con la **✕** **solo quedan guardados al pulsar este botón**.
8. Para imprimir, pulse **Imprimir PDF** (sale en horizontal, con los gráficos).

### Registrar una inspección de seguridad, OTS o STC

1. Vaya a **Inspecciones** → **Inspecciones de Seguridad** y pulse **Nueva Inspección**.
2. Complete **Código**, **Inspector** y **Responsable** (obligatorios), elija el **Tipo** (*Seguridad*, *OTS* o *STC*) y diligencie Área, Frente, Fecha, Estado, **Hallazgos**, **Cerrados**, **Actos**, **Cond.**, Responsable Cierre, **F. Compromiso** y observaciones.
3. Pulse **Guardar**. Los indicadores y gráficos se recalculan.
4. Para editar use el lápiz; para borrar, la papelera y luego **Sí**. El borrado es definitivo.
5. Use los filtros (**Buscar**, **Tipo**, **Estado**, **Mes**, **Con hallazgos**), ordene haciendo clic en los títulos de columna y pulse el botón verde de Excel para **exportar** lo filtrado. La **✕** limpia los filtros.
6. Alternativas: la sub-pestaña **OTS** tiene su propio formulario (**Nueva OTS**, con campo *Acciones Generadas*), y la pantalla STC (`/hseq/seguridad/stc`) tiene **Nueva STC**. Ambas guardan en el mismo registro de inspecciones con el tipo fijo.

### Hacer una inspección con IA y alimentar la Matriz IPERV

1. Vaya a **Matriz Riesgos** → **Nueva Inspección IA**.
2. Escriba **Área**, **Actividad** y **Tarea** (sin estos tres la IA no arranca), la fecha, el inspector, el responsable del área y la ubicación.
3. En **Foto / Evidencia** adjunte una foto JPG o PNG del frente de trabajo (máx. 10 MB). Para quitarla use la **✕** roja sobre la imagen.
4. Describa en **Observación del inspector** lo que ve: condiciones y actos inseguros, EPP, señalización.
5. Pulse **Analizar con IA** y espere el mensaje *"Analizando con IA…"*.
6. Revise la propuesta: NR y aceptabilidad, peligros, clasificación, efectos, controles faltantes, medidas, EPP, documentos, hallazgo y acción correctiva.
7. Decida:
   - **Aprobar y alimentar Matriz IPERV**: guarda la inspección como *Aprobado* y crea un riesgo **Activo** con fuente *InspeccionIA*; lo lleva a la Matriz IPERV.
   - **Guardar pendiente de revisión**: guarda la inspección sin crear riesgo.
   - **Rechazar**: guarda la inspección como *Rechazado* y limpia la propuesta.
8. En la Matriz IPERV, abra el riesgo con el lápiz y complete lo que la IA no llena: controles en el medio y en el individuo, eliminación, sustitución, ingeniería y el responsable real.

### Registrar o editar un riesgo manualmente en la Matriz IPERV

1. Vaya a **Matriz Riesgos** → **Matriz IPERV** y pulse **Ingresar manualmente** (o el lápiz de una fila para editar).
2. Escriba **Área**, **Actividad**, **Tarea**, **Descripción del peligro**, **Clasificación (GTC 45)** y **Efectos posibles**. La app no los exige, pero sin ellos la fila queda sin área en el Mapa de Riesgos y no aparece en las búsquedas.
3. Elija **ND**, **NE** y **NC**. El recuadro **NR calculado** se actualiza al instante con su aceptabilidad.
4. Complete **Controles existentes** (Fuente, Medio, Individuo), **Medidas de intervención** (Eliminación, Sustitución, Ingeniería, Administrativo, EPP), **Responsable** y **Estado**.
5. Pulse **Guardar**. La tabla queda ordenada de mayor a menor NR.
6. Filtre con **Buscar área / peligro…**, **Todos los estados** y **Todos los NR**.

> La Matriz IPERV no tiene botón para borrar riesgos. Para sacarlos de la gestión, márquelos como **Eliminado** (en el formulario o desde Acciones Correctivas).

### Usar la Biblioteca de Peligros

1. Vaya a **Matriz Riesgos** → **Biblioteca Peligros**. Verá el catálogo precargado de peligros típicos de proyectos solares EPC (Obras Civiles, Estructuras, Módulos FV, Eléctricas AC, Salas Eléctricas, Subestación, Izajes, Commissioning, O&M, General).
2. Filtre por texto, **área** o **clasificación**.
3. Pulse **Ver controles y EPP** para desplegar controles en la fuente, el medio y el individuo, medidas, EPP y documentos asociados.
4. Pulse **Agregar a Matriz** para copiarlo a la Matriz IPERV del proyecto. Queda *Activo*, con responsable "Por definir" y valoración aproximada (ver *Cómo funciona*).
5. Cada clic agrega una fila nueva: **no pulse dos veces** el mismo peligro.

### Hacer seguimiento a los riesgos (Acciones Correctivas)

1. Vaya a **Matriz Riesgos** → **Acciones Correctivas**.
2. Revise las tarjetas **Pendientes** (activos), **En Control**, **Controlados** y **Eliminados**.
3. Filtre por texto o estado, o marque **Solo con acción correctiva**.
4. Avance cada riesgo con los botones de la fila: **En control** (desde Activo) → **Controlar** (desde En Control). **Eliminar** lo pasa a *Eliminado* (no lo borra). Cada clic se guarda al instante.
5. Si el plazo ya pasó y el riesgo sigue activo, la fecha aparece en rojo con ⚠.

### Consultar Mapa de Riesgos, Dashboard SST y Evidencias

1. **Mapa de Riesgos**: vea las áreas ordenadas de la más crítica a la menos crítica y use **Ver en Matriz** para ir al detalle.
2. **Dashboard SST**: vea inspecciones IA, las aprobadas por SST, críticos activos y % de riesgos controlados, los gráficos y la tabla **Riesgos Críticos Activos — Requieren atención inmediata** (NR ≥ 2000 y estado Activo).
3. **Evidencias**: vea las inspecciones con IA en galería o en lista (botones de la derecha), filtre por área/actividad o estado de validación y use **Descargar** para abrir el archivo.

### Registrar entregas de EPP y dotación

1. Vaya a **EPP / Dotación** y pulse **Registrar Entrega**.
2. Complete **Trabajador**, **Documento (CC)**, **Área**, **Ítem** y **Entregado Por** (obligatorios), y además cargo, tipo (*EPP* o *Dotación*), descripción, talla, cantidad y **Fecha Entrega**.
3. Escriba la **Próxima Entrega** (la fecha de reposición). Sin esta fecha la entrega cuenta siempre como "al día".
4. Marque **Firma OK** si el trabajador firmó la conformidad y pulse **Guardar**.
5. Use **Exportar Excel** (con columna de estado coloreada) o **Imprimir**. Los avisos amarillo y rojo tienen un enlace **Ver** que filtra las entregas.

### Planificar y registrar capacitaciones

1. Vaya a **Capacitaciones**.
2. Para programar pulse **Planificar** y complete **Nombre** y **Tema** (obligatorios), área, público objetivo, responsable, **Fecha Planificada** y duración. Pulse **Guardar**.
3. Cuando se dicte, pulse **Registrar Ejecutada** y complete **Título** e **Instructor** (obligatorios), tema, área, responsable, fecha, duración, **N° Asistentes** y lugar. Pulse **Guardar**.
4. Filtre por **Área**, **Tema**, **Mes**, **Año** o **Estado** y pulse **Aplicar** (los filtros no se aplican solos). **Limpiar** los quita.
5. Consulte las pestañas **Planificadas** y **Ejecutadas** y el buscador.

> Importante: la planificada y la ejecutada son registros **independientes**. No hay botón para marcar una planificada como ejecutada ni para editarla, así que una planificada con fecha pasada seguirá apareciendo como vencida. El cumplimiento compara **cantidades** (ejecutadas ÷ planificadas). ⚠ Por confirmar: cómo se quiere cerrar una capacitación planificada una vez dictada.

### Registrar incidentes y actualizar los indicadores

1. Vaya a **Incidentes** y pulse **Registrar Evento**.
2. Complete **Número**, **Descripción** y **Persona Involucrada** (obligatorios); elija **Tipo** (Casi Accidente, Incidente, Accidente Leve, Accidente Grave o Accidente Fatal), **Gravedad** (Leve, Moderado, Grave, Fatal), fecha, frente, investigador, **Causa Raíz** y **Estado**. Pulse **Guardar**.
3. Vaya a **Indicadores**, elija el **Período** (año) y escriba las **Horas-Hombre Trabajadas** del período (vienen en 200.000 por defecto).
4. Después de escribir las HHT, **vuelva a elegir el año** en el selector para que los índices se recalculen: la app solo recalcula al cambiar el año o al volver a abrir la página.

> Los eventos no se pueden editar, solo borrar. Si se equivocó, bórrelo y regístrelo de nuevo.

### Registrar permisos de trabajo y ATS

1. Abra `/proyectos/{id}/hseq/seguridad/permisos` y pulse **Nuevo Permiso**.
2. Complete **Número**, **Actividad** y **Responsable Trabajo** (obligatorios); elija **Tipo** (Trabajo en Altura, Espacio Confinado, Trabajo en Caliente, Energías Peligrosas, Eléctrico, Excavación o General), frente, **Emitido Por**, fechas de emisión y vencimiento, **Estado** y medidas de control. Pulse **Guardar**.
3. Para el ATS abra `/proyectos/{id}/hseq/seguridad/ats`, pulse **Nuevo ATS**, complete **Código**, **Actividad** y **Responsable** (obligatorios), frente, fecha, **N° Trabajadores**, riesgos identificados, medidas de control y observaciones. Pulse **Guardar**.
4. Ninguna de las dos pantallas permite editar. Cambie el estado de un permiso vencido borrándolo y registrándolo de nuevo, o déjelo como está (la fecha en rojo ya lo señala).

### Registrar pausas activas

1. Abra `/proyectos/{id}/hseq/seguridad/pausas-activas`.
2. Elija el **Mes** (recarga sola). Si cambia el **Año**, pulse **Aplicar**.
3. Pulse **Registrar** y complete **Trabajador**, **Documento (CC)**, **Área** y **Responsable Registro** (obligatorios), cargo, mes, año y **N° Pausas** del mes. Pulse **Guardar**.
4. Revise las tarjetas: trabajadores con registro, total de pausas, promedio por persona y **% Cumplimiento meta (60)**.

### Diligenciar la autoevaluación ISO 45001 o Resolución 0312

1. Abra `/proyectos/{id}/hseq/seguridad/checklist` (ISO 45001:2018) o `/proyectos/{id}/hseq/seguridad/resolucion0312` (Res. 0312:2019).
2. Pulse **Nueva Auditoría**, escriba el **Auditor / Responsable** (obligatorio), la fecha, el estado inicial (*Borrador* o *En Proceso*) y las observaciones. Pulse **Crear y Diligenciar Checklist**: la app crea todos los requisitos en *Sin evaluar*.
3. En cada fila:
   - use **💡** junto al requisito para ver su **interpretación** y **💡** en Evidencia para ver los **documentos auditables**;
   - elija el **Cumplimiento**. En ISO 45001 existe también *No aplica*; en Res. 0312 las opciones muestran los puntos que otorgan;
   - en ISO 45001 puede ajustar el **Puntaje** (0 a 2);
   - adjunte la **evidencia** (PDF, imagen, Word o Excel; máx. 10 MB) y escriba hallazgo, oportunidad de mejora, responsable, plazo y **Seguimiento** (Pendiente, En proceso, Ejecutado).
4. Guarde con frecuencia: **Guardar cambios** (queda *En Proceso*), **Guardar borrador** / **Borrador** o **Finalizar auditoría** / **Finalizar**. Los cambios en la tabla **no se guardan solos**.
5. Exporte con **Exportar Excel** / **Excel** (descarga un archivo CSV que abre en Excel) o imprima con **Exportar PDF** / **PDF**.
6. Desde el listado, el lápiz reabre la auditoría, el ícono de hoja la exporta y la papelera la elimina (con confirmación **Sí**).

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| El tablero HSEQ del proyecto muestra 127 días sin accidentes y porcentajes que no coinciden con la realidad. | Esa página tiene datos de demostración (lo indica el aviso "Fase 1"). | Use el **Dashboard** de Seguridad y **Indicadores**, que sí leen los datos del proyecto. |
| Al pulsar **Analizar con IA** sale *"Complete Área, Actividad y Tarea antes de analizar."* | Falta alguno de esos tres campos. | Complételos y vuelva a pulsar. |
| Sale *"No se ha configurado la clave de API de Anthropic…"* | El servidor no tiene configurada la clave del servicio de IA. | Registre el riesgo con **Ingresar manualmente** en la Matriz IPERV y avise al administrador. |
| Sale *"Error API (429): …"* al analizar con IA. | El proveedor de IA rechazó la solicitud por **cuota agotada o facturación no habilitada**. En RenergeIA este error es conocido en Google Gemini (falta habilitar la facturación en Google Cloud); en la inspección con IA el mensaje viene del proveedor configurado para esa función. | No reintente muchas veces seguidas. Registre el riesgo manualmente y pida al administrador revisar la facturación/cuota del proveedor de IA. ⚠ Por confirmar qué proveedor queda definitivo para esta función. |
| Sale *"Error de conexión con la API…"* o *"No se pudo interpretar la respuesta de la IA…"* | Sin conexión con el servicio de IA, o la IA respondió en un formato que la app no entiende. | Espere un momento y pulse **Analizar con IA** otra vez; si persiste, registre el riesgo manualmente. |
| La IA no tuvo en cuenta la foto adjunta. | El archivo no es JPG, PNG, GIF o WEBP (por ejemplo, un PDF), o no se pudo leer. | Adjunte una foto en formato de imagen y vuelva a analizar. |
| *"Error al subir archivo: …"* o *"Error al subir evidencia: …"* | El archivo supera **10 MB**. | Reduzca la foto o el PDF (o tome la foto con menor resolución) y súbalo de nuevo. |
| Al importar el Plan de Trabajo aparece *"Error: …"* o "0 actividades importadas". | El archivo no es .xlsx válido, la actividad no está en la primera hoja, o no hay una columna titulada "Actividad" en las primeras 20 filas. | Deje los datos en la primera hoja, con títulos en una fila (Actividad, PHVA, Responsable, ENE…DIC, Estado) y vuelva a pulsar **Importar**. |
| Borré o agregué filas del Plan de Trabajo y al volver no estaban. | No se pulsó **Guardar Cambios**. | Repita los cambios y pulse **Guardar Cambios**; espere el aviso verde "Cambios guardados correctamente". |
| El formulario no guarda y aparece un mensaje en rojo bajo un campo. | Falta un campo obligatorio (marcado con *). | Complete el campo marcado y pulse **Guardar** de nuevo. |
| En **Indicadores** cambié las HHT y los índices no cambian. | La app solo recalcula al cambiar el año o recargar la página. | Vuelva a elegir el año en **Período** o recargue la página. |
| El **IG — Índice de Gravedad** siempre está en 0. | La app aún no registra días perdidos. | Calcule el IG por fuera mientras se agrega ese dato. |
| La pestaña **Acciones** y el indicador "Acciones Vencidas" están siempre vacíos. | Las acciones SST salen de hallazgos de la división Seguridad, y hoy ninguna pantalla del proyecto crea ese tipo de hallazgo. | Haga el seguimiento de controles en **Matriz Riesgos → Acciones Correctivas**. ⚠ Por confirmar dónde se registrarán los hallazgos SST. |
| En el listado de ISO 45001 aparecen también las auditorías de Resolución 0312. | El listado de ISO 45001 muestra todas las auditorías de Seguridad del proyecto. | Fíjese en la columna de cumplimiento y abra cada auditoría desde su propia pantalla (Res. 0312 muestra "pts"). |
| Las capacitaciones planificadas siguen "vencidas" aunque ya se dictaron. | Registrar una ejecutada no cierra la planificada. | Registre la ejecutada igualmente (el cumplimiento se calcula por cantidad). Si la planificada sobra, bórrela. |
| No encuentro STC, ATS, Permisos, Pausas Activas ni los checklists en la barra de Seguridad. | Esas pantallas no tienen pestaña. | Escriba la dirección indicada al inicio de este capítulo y guárdela en favoritos. |

Para fallas generales (sesión, página que no carga, error al entrar después de una actualización), vea el capítulo [16. Problemas frecuentes](16-problemas-frecuentes.md).

---

## Relación con otros módulos

- **Proyectos** ([capítulo 1](01-proyectos.md)): todo lo de Seguridad pertenece a un proyecto; se entra desde la tarjeta **HSEQ** del detalle del proyecto.
- **Dentro de Seguridad**, las pantallas se alimentan entre sí:
  - Plan de Trabajo, Capacitaciones y EPP → **Dashboard** de Seguridad (insignia de cumplimiento) y tablero de **Inspecciones de Seguridad**.
  - Incidentes → **Indicadores** (IF, II y conteos por tipo).
  - Auditorías ISO 45001 → indicador **ISO 45001** en Indicadores y en el tablero de Inspecciones.
  - Pausas activas del mes en curso → indicador **Pausas Activas** del tablero de Inspecciones.
  - Inspección con IA y Biblioteca de Peligros → **Matriz IPERV** → Mapa de Riesgos, Dashboard SST y Acciones Correctivas.
- **HSEQ — Calidad, Ambiental y Social** ([8](08-hseq-calidad.md), [9](09-hseq-ambiental.md), [10](10-hseq-social.md)): se abren desde el mismo tablero HSEQ del proyecto. Las acciones correctivas SST usan el mismo tipo de hallazgo/acción que las no conformidades de Calidad, separadas por división.
- **HSEQ — Auditorías** ([capítulo 11](11-hseq-auditorias.md)): el menú lateral **HSEQ** (corporativo) tiene sus propias auditorías por norma (incluidas ISO 45001 y Res. 0312) y consolidados de STC, OTS y pausas activas. Son independientes de los checklists y registros de este capítulo, que son **por proyecto**.
- **Evaluaciones HSE** ([capítulo 15](15-evaluaciones-hse.md)): la asistencia y las evaluaciones del personal se manejan en esa aplicación aparte; no alimentan automáticamente las capacitaciones de este módulo.

---

## Buenas prácticas

- **No confíe ciegamente en la IA:** revise ND, NE, NC y el hallazgo antes de pulsar **Aprobar y alimentar Matriz IPERV**. La responsabilidad de la valoración es de SST.
- **Tome buenas fotos:** una sola escena, bien iluminada, en JPG o PNG y de menos de 10 MB. Describa también en texto lo que la foto no muestra (duración de la exposición, número de personas).
- **Tenga un plan B sin IA:** si la IA falla, use **Ingresar manualmente** o la **Biblioteca de Peligros**. El trabajo de campo no debe detenerse.
- **Complete el riesgo después de aprobarlo:** la IA no llena controles en el medio ni en el individuo, ni eliminación, sustitución o ingeniería, ni el plazo.
- **Mantenga vivo el estado de los riesgos:** pase a **En control** y **Controlar** en Acciones Correctivas a medida que avanza. Use **Eliminado** en lugar de dejar filas obsoletas.
- **Siempre escriba el Responsable del riesgo:** además de ser clave para el seguimiento, un riesgo sin responsable puede provocar un error al usar el buscador de **Acciones Correctivas**.
- **Guarde a menudo** en Plan de Trabajo y en los checklists: allí los cambios no se guardan solos.
- **Códigos consistentes:** use una numeración única para inspecciones, OTS, STC, ATS, permisos e incidentes (la app no controla duplicados).
- **Siempre fecha de próxima entrega en EPP:** sin ella, la entrega nunca aparecerá como vencida.
- **Actualice las HHT cada período** en Indicadores y recalcule eligiendo de nuevo el año.
- **Estado del Plan de Trabajo:** marcar las casillas **E** no basta; cambie el **Estado** a *Ejecutada* para que cuente en el cumplimiento.
- **Borrados definitivos:** en inspecciones, EPP, capacitaciones, incidentes, permisos, ATS, pausas y auditorías, **Sí** elimina para siempre (no hay papelera). Exporte antes de borrar si tiene dudas.
- **Evidencias con respaldo:** guarde una copia de las evidencias críticas en el control documental del proyecto.
