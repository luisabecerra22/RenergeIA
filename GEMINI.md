# Instrucciones para el Agente

> Crea un archivo CLAUDE.md, AGENTS.md y GEMINI.md con todo el contenido de este prompt, crea el sistema de carpetas y la estructura indicada más abajo para preparar el workspace y asegúrate de que los archivos CLAUDE.md, AGENTS.md y GEMINI.md estén siempre iguales, asía eu si haces un cambio o registras un aprendizaje en uno de estos tres archivos, deberás replicarlo en los demás para que el sistema cargue en cualquier entorno de IA agéntica.

> 📌 **RETOMAR EL PROYECTO / TRASPASO A OTRA CUENTA:** lee primero **`TRASPASO-Y-ESTADO.md`** en la raíz del repo. Ahí está el estado actual, los pendientes, el rumbo y cómo desplegar los DOS proyectos (RenergeIA .NET y `evaluacion-hse/`). La memoria automática de Claude NO se comparte entre cuentas: toda la trazabilidad viva vive en archivos del repo.

## Aprendizajes del Agente (Mejora Continua)

> **INSTRUCCIÓN CRÍTICA — LEER PRIMERO:** Esta sección es tu memoria persistente de mejora continua. **Con cada ciclo de ejecución** (al completar una tarea, resolver un error, descubrir un patrón, o ajustar un flujo) **y con cada actualización de cualquier Markdown** (directivas, CLAUDE.md, AGENTS.md, GEMINI.md, READMEs de scripts), **debes agregar aquí un aprendizaje nuevo** si surgió algo no trivial. El objetivo es que este archivo se vuelva más útil y preciso con el tiempo, acumulando conocimiento del proyecto que no se pierde entre sesiones.
>
> **Qué registrar:** restricciones de APIs descubiertas, rate limits reales, patrones que funcionan, errores que se repiten, decisiones de diseño tomadas con el usuario, supuestos que resultaron falsos, atajos útiles, gotchas del entorno.
>
> **Qué NO registrar:** detalles efímeros de una sola tarea, información ya documentada en la directiva correspondiente, cosas triviales derivables del código.
>
> **Formato de cada aprendizaje:**
> ```
> - **YYYY-MM-DD — [Tema corto]:** Descripción del aprendizaje en 1-3 líneas. **Por qué importa:** consecuencia práctica o cómo aplicarlo en el futuro.
> ```
>
> **Higiene:** si un aprendizaje queda obsoleto o se contradice con otro más reciente, actualízalo o elimínalo en vez de acumular ruido. Mantén la lista ordenada por fecha (más recientes arriba). Si superas ~25 entradas, consolida las más antiguas o promuévelas a la directiva que corresponda.

### Registro de aprendizajes

- **2026-09-16 — [Manual de usuario en `docs/manual/`]:** Se creó el manual de usuario de toda la app en Markdown (un capítulo por módulo, índice en `docs/manual/README.md`) con estructura fija: Para qué sirve · Conceptos clave · Cómo funciona · Paso a paso · Qué hacer en caso de… · Relación con otros módulos · Buenas prácticas. Se escribió leyendo las páginas Razor reales; lo que el código no deja claro quedó como "⚠ Por confirmar" (~57, incluye tableros HSEQ con cifras de ejemplo, evidencias guardadas como archivos en el servidor que podrían perderse en cada deploy, falta de permisos por rol). No lleva contraseñas (repo público). **Por qué importa:** cada vez que cambie un módulo hay que actualizar su capítulo y el historial del README del manual; los "Por confirmar" son una lista de decisiones y bugs pendientes para revisar con la usuaria.

- **2026-09-16 — [Planificación Documental: carga por proyecto, alertas y responsables por área]:** El módulo **Documentos** de cada proyecto se alimenta del Excel `FO-SI-GC-002-1 Planificación Documental` con el botón **Cargar planificación** (`PlanificacionDocumentalParser` + `PlanificacionDocumentalService` + `ImportarPlanificacion.razor`; el importador viejo que duplicaba se eliminó). Hojas: Construcción→Procedimientos, HSE, Ingeniería ("Check list" se ignora); encabezados en la fila con "Código Cliente" (14), columnas por NOMBRE de encabezado (Ingeniería trae Fase, retrasos por revisión, "Responsable" = en cancha de quién —fórmula, se ignora—, "Observación tiempo de retraso" y bloque Redline/As-Built Z–AI con sus propios "Responsable"/"Observaciones"); la columna Observaciones trae el **transmittal** (HSEEXT-129, COS5SO-GY-019) y se guarda SIEMPRE completa en `Transmittal` (confirmado por la usuaria; en la app la columna Observaciones muestra la "Observación tiempo de retraso"); la lectura termina en el pie de firmas ("Revisado/Aprobado por"). Reglas de carga: **upsert** por Código Renergeia+Cliente → Código Renergeia (si no está repetido en el archivo, ej. ITM01 x2) → Código Cliente → Nombre; celda vacía nunca borra; documentos que ya no están en el archivo NO se borran (se listan); si el documento se editó en la app después de la última carga (`FechaEdicionApp` > `FechaUltimaImportacion`) **gana la app** y se muestran las diferencias con opción "Usar Excel" por documento (decisión por defecto, no confirmada aún por la usuaria); se advierte si la fecha de actualización (E3) es anterior a la ya cargada (`Proyecto.FechaActualizacionPlanDocumental`). Datos sucios reales tolerados: dos fechas en una celda (se toma la más reciente, conviven dd/MM y MM/dd → se prefiere dd/MM salvo que quede en el futuro), "03/042026" sin separador, fechas en la columna Área. **Alertas** (`Core/Helpers/SeguimientoDocumento`): días sin atender = hoy Colombia − fecha más reciente registrada, solo para Pendiente Emitir / Pendiente Validación / No Validado; **amarillo desde 4 días, rojo desde 8** (definido por la usuaria); en cancha: Pendiente Validación→Cliente, Pendiente Emitir/No Validado→Renergeia. **Responsable interno por área** (`ResponsableAreaDocumento`: Área, Cargo, Nombre, Email; ej. Mecánico → Coordinador mecánico), el `Documento.Responsable` propio manda; "Mis pendientes" cruza el usuario logueado con email/nombre. Botón Validar deja `ValidadoPor` y fecha. Áreas ampliadas: Calidad, Ambiental, Seguridad, Comunicaciones. Pendiente: leer desde SharePoint (requiere app registrada en Microsoft 365) y notificaciones por correo. **Por qué importa:** cualquier cambio al módulo Documentos debe respetar este mapeo, el upsert sin duplicar y la regla de alertas.


- **2026-09-16 — [Histograma de Personal: Real desde la nómina, Planificado desde el BOM]:** Personal ya no usa las 12 columnas fijas (`ItemHistograma`/`ItemHistogramaReal`, que siguen solo para Equipos): usa `PersonalHistogramaMes` (Tipo Planificado/Real, Cargo, Anio, Mes, Cantidad, CantidadNomina, EsManual) sin tope de meses; la pantalla muestra 12 meses desde el mes/año inicial elegido. `PersonalHistogramaService`: **Real** = personas distintas por cargo y mes desde los hitos de Salarios (mes tomado del Periodo `Mmm-AAAA`, I y II quincena = mismo mes; Primas/Seguridad social/Liquidaciones excluidas; persona con 2 cargos cuenta en ambos pero una vez en el TOTAL; importe neto ≤ 0 no cuenta; se descartan cargos que no son nombres (vacío, Sin cargo, #N/A, Fecha de Retiro, Cargo N, textos con dígitos como fechas "02-Jul-2026") y filas que no son personas (Planilla…, Liquidaciones, XX Personas); la pantalla avisa si la ventana de 12 meses no incluye meses con nómina y ofrece "Ver ese período"; nombres normalizados sin tildes y sin lo que sigue a " - "; nombres parecidos (Levenshtein ≤ 3 o prefijo) se unifican SOLO con confirmación, guardada en `AsignacionTesoreria` Tipo PersonaMisma/PersonaDistinta); se recalcula al importar tesorería y al abrir Histogramas, y las celdas editadas distintas a la nómina quedan EsManual (naranja) y se conservan. **Planificado** = importar hoja **H PER** del BOM (fila de fechas semanales desde col G, C=tipo, D=cargo; mes = **máximo semanal**, cargos asignados al mismo destino se suman por semana) con equivalencia cargo BOM → cargo nómina recordada (Tipo CargoBOM), más edición manual; los cargos nuevos de la nómina aparecen en 0. Comparativo incluye tabla por cargo Real/Planificado. Migración única de los datos viejos (marca `AsignacionTesoreria` Tipo Migracion). **Por qué importa:** el personal real NO se digita; cualquier cambio al parser de Salarios (Periodo, Detalle=cargo, Descripcion=nombre) impacta el histograma.

- **2026-09-16 — [WBS: Reiniciar plantilla nunca borra avances del Informe Diario]:** `RegistrosAvanceDiario` tiene FK Restrict a `ActividadesWBS`, así que borrar una versión con avances lanzaba `23503` sin manejar. Ahora el modal cuenta los avances de la versión: si hay, NO borra y ofrece "Crear nueva versión con plantilla EPC" (nueva vigente con el .mpp embebido; la anterior queda Histórica con sus avances); si no hay, borra desvinculando antes `Partida.ActividadWBSId` y los padres, todo con try/catch. **Por qué importa:** cualquier borrado de actividades WBS debe revisar primero avances diarios y partidas enlazadas.

- **2026-09-16 — [Plantilla EPC del WBS = cronograma Timing Template .mpp embebido]:** "Cargar plantilla EPC" en WBS/Actividades ya no usa una lista escrita a mano: lee `RenergeIA.Web/Plantillas/Cronograma_EPC_Template.mpp` (origen `COS6MG_Timing_Template.mpp`, 128 tareas) embebido como `EmbeddedResource` con LogicalName `Plantilla.CronogramaEPC.mpp`, vía `MppParser.PlantillaEPC(inicioProyecto)`: omite la tarea raíz (sube un nivel: quedan 12/35/80 actividades en N1/N2/N3), desplaza todas las fechas para que arranque en `Proyecto.FechaInicioPlaneada` y crea las actividades con `CrearActividadesDesdeMppAsync`. El Informe Diario toma sus actividades del WBS, así que hereda la plantilla. **Por qué importa:** para cambiar la plantilla estándar basta reemplazar ese `.mpp` (mismo nombre) y volver a publicar; no hay que tocar código.

- **2026-09-15 — [Compromisos = órdenes de compra del Forecast Control, con hitos]:** La hoja Compromisos se alimenta del mismo archivo de tesorería que el Flujo de Caja: **una sola importación actualiza ambas** (`TesoreriaImportService` + componente `ImportarTesoreria.razor`), valida la fecha del corte tomada del nombre (`MM.DD.AAAA` vs `Proyecto.FechaCorteTesoreria`) y advierte si el archivo es anterior al ya cargado. Estructura del Excel: fila OC (B proveedor, C descripción, D `CO_AAAA_NNN`, **E aprobado**, **H facturado**) + filas hijas de hitos (A código, D documento F#/CC#/POL#, E fecha, F subtotal, G IVA, H importe, I/J retenciones, K total a pagar; **relleno verde en K = pagado**, detectado con ClosedXML por canal G dominante). Modelo: `CompromisoCosto` (Origen Manual/Tesoreria, Grupo OC/Salarios/Impuestos/Proyectado, ClaveTesoreria, Valor=aprobado, ValorFactura=facturado, ValorPagado) + `HitoCompromiso` + `AsignacionTesoreria` (memoria de números de OC asignados a las `CO_..._XXX` y de códigos asignados a facturas sin código). Reglas: OCs del archivo se actualizan por número y las manuales se conservan; alerta de **OC sobrepasada** si facturado − aprobado > $1.000 COP o > US$ 1 (diferencias menores son redondeo y no alertan); las **OC sin consecutivo (`CO_..._XXX`) son valor PRESUPUESTADO, no comprometido**: `CostoService.PresupuestadoSinOCAsync` suma sus facturas por pagar por rubro/moneda y se descuentan del flujo futuro, así quedan en el Pendiente por ejecutar del Presupuesto (tampoco suman en las tarjetas Total aprobado/Facturado/Pagado/Pendiente de Compromisos; al asignarles número pasan a comprometido); las facturas sin código NUNCA se heredan ni se asumen (se avisa la cantidad y se asignan a mano, quedando recordadas); Salarios va como sección visual al final de la vista de OC y **Proyectado (sin OC) tiene su propia pestaña "Proyectado"** junto a "Resumen de Órdenes de Compra" (tarjetas: total proyectado COP|USD, líneas, códigos), ambos con alerta de "sin rubro"; los **pagos de impuestos DIAN NO se importan** (definido 2026-09-16). **Salarios** se muestra SIN nombres: quincena → cargo (columna D, guardado en `HitoCompromiso.Detalle`) → código de rubro, con totales/pagado/pendiente; bloques especiales "Seguridad social" (planillas) y "Liquidaciones" (D = "Fecha de Retiro"); las filas sin código suman dentro de su cargo y muestran el código más usado por ese cargo como **sugerido**, que solo cuenta al confirmarlo con ✓ (se aplica a todo el grupo cargo+quincena y queda recordado; si el archivo trae código, gana el del archivo). **Por qué importa:** cualquier cambio en el parser o en la UI debe respetar estas reglas; el archivo de tesorería es la fuente única de OCs, flujo, ejecutado y comprometido.

- **2026-09-15 — [Ejecutado del Presupuesto = total del Flujo de Caja, coherente por moneda]:** El Ejecutado de cada código en Actividades Presupuestadas NO se digita: `CostoService.SincronizarEjecutadoDesdeFlujoAsync` lo calcula como la suma del Flujo de Caja **desde el inicio hasta la semana anterior a la actual** (cortes con `FechaCorte` < lunes de la semana en curso, hora Colombia UTC-5; `CostoService.InicioSemanaActual()`); se recalcula al guardar/importar tesorería y al abrir el presupuesto, así que avanza solo con el calendario. Los cortes de la semana en curso y futuros NO cuentan como ejecutado. Regla de monedas: los pagos se agrupan por código y moneda; si el código tiene partida en COP y en USD, los pagos COP van a la COP y los USD a la USD (cada uno nativo); si solo existe en una moneda, los pagos de la otra se convierten con la TRM del proyecto. Se escribe con `ExecuteUpdateAsync` + ajuste del tracker para no guardar por accidente cambios pendientes del DbContext compartido del circuito. El Presupuesto tiene vista **Solo COP / Solo USD / Ambas (en COP)**: categorías, tarjetas y Costos por Categoría filtran y suman en la moneda de la vista (nunca mezclar montos nativos COP+USD sin convertir). En el Flujo de Caja un código aparece una sola vez aunque exista en ambas monedas (el toggle Pesos/Dólares separa). **Por qué importa:** antes el toggle no filtraba y los pagos de un código bimoneda caían en una sola fila, dando ejecutados incoherentes.

- **2026-09-15 — [Importar .mpp (MS Project) con MPXJ.Net + IKVM en Cloud Run]:** El WBS importa cronogramas `.mpp` nativos con el paquete `MPXJ.Net` (IKVM, agrega ~300MB al publish). En Windows funciona directo, pero en Cloud Run lanzaba "The type initializer for '<Module>' threw an exception" → causa raíz: `IKVM.Runtime.InternalException: Could not load libjvm` porque TANTO el `.gcloudignore` (upload) COMO el `.dockerignore` (COPY del build) tenían `**/bin/`, que excluía `publish/ikvm/linux-x64/bin/libjvm.so` — ambos se cambiaron a excluir solo los bin/obj de los proyectos fuente (NUNCA volver a poner `**/bin/` global en ninguno de los dos). Además el Dockerfile instala `libfontconfig1 libfreetype6` vía apt-get para las dependencias nativas de IKVM. El parser vive en `MppParser.cs` (UniversalProjectReader → OutlineNumber/OutlineLevel/Summary/Milestone/Start/Finish/PercentageComplete). Flujo definido por la usuaria: .mpp como carga inicial del WBS (junto a plantilla/Excel/PDF) o como fuente al Crear reprogramación (opcional; sin archivo se copian las actividades actuales); nunca duplicar la versión "Actividades Inicial"; en versiones históricas existe "★ Activar como vigente". **Por qué importa:** si se cambia la imagen base del Dockerfile hay que conservar esas dependencias apt, o el import de Project revienta en producción.

- **2026-09-14 — [Codificación de costos: jerarquía Disciplina → Rubro → Código de tesorería]:** El archivo `Codificación.xlsx` (COD COP: 143 códigos → 47 rubros → 8 disciplinas; COD USD: 13 códigos → 11 rubros; PPTO COP/USD: presupuesto por rubro) traduce los códigos detallados de la tesorería/Forecast (SPET, CGAC, HSDO, CGSTM…) a su **Rubro** de 4 letras (SPSP, CGPR, CGHS…), que es el código que habla con presupuesto y BOM. Decisiones: para **La Soberana** el presupuesto toma la columna Rubro separando COP y USD; duplicados en COD USD se toman una sola vez; CMIE y PTIE se excluyen (sin disciplina). ⚠ POR RECONFIRMAR: La Soberana usa codificación ANTIGUA — solo ese proyecto; los demás mantienen la codificación estándar DP+Apoyo de la BOM. Del Forecast se toma SIEMPRE la columna EXECUTED (definido por la usuaria). Pendiente: % semáforo de desfase. **Por qué importa:** el cruce tesorería → rubro → presupuesto es el corazón del control de costos; nunca cruzar códigos de tesorería directo contra la BOM sin pasar por la tabla de Codificación.

- **2026-09-12 — [BOM: estructura del Excel de oferta y comparativo BOM vs Real]:** La BOM (ej. `COS6ML-PRG03-R6OFRL - BOM.xlsx`, 36 hojas) es la oferta con la que se cerró el negocio. Fuentes de datos para la app: hojas **General** (Check, Concepto PER/MAQ/MAT, Tipo Concepto, Tiempo, Cantidad Proyecto, Costo unitario, Moneda Costo, Costo total), **Sum PPAL** (Check, Descripción, Unidad, Cantidad Total con spares/pérdidas, Costo unitario + Moneda Inicial, Costo total moneda proyecto) y **Rend MAT** (Check, Tipo Concepto, Cantidad Proyecto, Costo Unitario, Costo Total); **Rend PER MAQ** trae solo rendimientos (sin costos directos). La columna **Check** = código de rubro (PASP, ESIM, CGST…), el mismo del Presupuesto, y se repite entre conceptos: el costo por código = suma de sus conceptos. La referencia de comparación contra las OC es el **COSTO** (no el precio de venta); venta/costo/margen de la hoja "Resumen" alimentan el Consolidado. Implementado: entidad `LineaBOM`, pestaña **BOM vs Real** (reemplazó Comparativo) con importador de esas 3 hojas, cantidades/valores reales digitados manualmente y diferencia por línea/código. **Por qué importa:** cuando la usuaria cargue una BOM nueva, se importa con ese mapeo; los valores reales digitados se conservan entre reimportaciones (match fuente+código+descripción). Alertas de desfase: pendientes de definir.

- **2026-09-11 — [HTTP 400 en /login tras cada deploy → DataProtection en BD]:** Cloud Run crea un contenedor nuevo por deploy y las claves de DataProtection se perdían, por lo que las cookies antiforgery viejas de los navegadores daban `HTTP 400` en el POST de /login ("The key {guid} was not found in the key ring"). Solución permanente: paquete `Microsoft.AspNetCore.DataProtection.EntityFrameworkCore`, el DbContext implementa `IDataProtectionKeyContext` y en Program.cs `AddDataProtection().PersistKeysToDbContext<RenergeIADbContext>().SetApplicationName("RenergeIA")` (migración `AddDataProtectionKeys`). **Por qué importa:** si reaparece un 400 en login tras un deploy, es cookie vieja (borrar cookies/incógnito); nunca quitar esta persistencia de claves.

- **2026-09-11 — [Módulo de costos: ventanas interrelacionadas por código de rubro]:** El módulo de costos funciona como sistema integrado: Compromisos = Órdenes de Compra (entidad `CompromisoCosto` con moneda COP/USD, # factura único, saldo por pagar); el **Comprometido** del Presupuesto NO se digita — hoy se calcula desde el Flujo de Caja de la semana en curso en adelante, sin las OC sin consecutivo `XXX` (actualizado 2026-09-16; antes eran los SaldoPorPagar de las OC); Flujo de Caja (`PagoCorteSemanal`) registra pagos semanales por partida; la pestaña Real/Ejecutado fue eliminada. Todo se presenta en doble moneda COP|USD. El Consolidado es el informe final para gerencia y debe recopilar todas las fuentes. **Por qué importa:** al modificar cualquier ventana de costos hay que mantener la coherencia con las demás; Dashboard, Comparativo y Consolidado aún usan fuentes viejas (`c.Valor` sin conversión de moneda, `Partida.MontoComprometido` huérfano) y están pendientes de alinear.

- **2026-09-09 — [Deploy de evaluacion-hse (subproyecto Next.js)]:** El subproyecto `evaluacion-hse/` se despliega en un proyecto GCP DISTINTO (`renergeia-evaluaciones`, #64204106653, servicio Cloud Run `evaluacion-hse`, URL `https://evaluacion-hse-64204106653.us-central1.run.app`). NO usar `gcloud run deploy --source .` porque toma el `Dockerfile` .NET de la raíz y falla al empujar a `renergeia-app` (artifactregistry denied). Flujo correcto: `gcloud builds submit --tag us-central1-docker.pkg.dev/renergeia-evaluaciones/cloud-run-source-deploy/evaluacion-hse --project renergeia-evaluaciones` y luego `gcloud run deploy evaluacion-hse --image <esa-tag>:latest --region us-central1 --project renergeia-evaluaciones --allow-unauthenticated --port 8080`. Hay un `.gcloudignore` en `evaluacion-hse/` que reduce el upload de ~491MB a ~4MB. **Por qué importa:** evita el error de artifact registry y builds contra el Dockerfile equivocado.

- **2026-09-09 — [Matriz de asistencia = asistencias O intentos]:** En la pestaña Personal, una persona se marca ✓ si tiene registro en `asistencias` O si presentó la evaluación (`intentos`), cruzando por cédula. La página `admin/personal` es `force-dynamic`; el botón "Actualizar" hace `router.refresh()` + refetch de `/api/admin/personal` (la UI no se refresca sola). **Por qué importa:** completar una evaluación crea un `Intento`, no una `Asistencia`; sin unificar ambas fuentes el ✓ nunca aparecería.

- **2026-09-09 — [Fuente en controles de formulario]:** Los `<select>/<button>/<input>` nativos no heredan `font-family`; el `input[type=file]` (botón "Elegir archivo") vive en shadow DOM. Solución en `globals.css`: `button,input,select,textarea{font-family:inherit}` + `input[type=file]::file-selector-button` y `::-webkit-file-upload-button{font:inherit}`. **Por qué importa:** mantiene Montserrat en toda la UI.

- **2026-09-09 — [Entorno Windows sin Python; PDFs con Chrome headless]:** En esta máquina no hay Python real (solo el stub de Microsoft Store), así que `reportlab` no sirve. Para generar PDFs con buen diseño: escribir HTML+CSS y convertir con `"/c/Program Files/Google/Chrome/Application/chrome.exe" --headless --disable-gpu --no-pdf-header-footer --print-to-pdf=out.pdf file:///ruta.html`. Embeber imágenes como data URI base64. **Por qué importa:** vía confiable para instructivos/reportes en PDF.

- **2026-08-26 — [Deploy requiere dotnet publish]:** El Dockerfile de RenergeIA usa `COPY publish/ .`, por lo que SIEMPRE se debe ejecutar `dotnet publish RenergeIA.Web -c Release -o publish` antes de `gcloud run deploy`. Sin este paso, Cloud Run despliega una versión antigua de los DLLs. **Por qué importa:** múltiples deploys fallaron silenciosamente (exit code 0 pero código viejo) hasta descubrir esto.

- **2026-08-26 — [Verificar en producción post-deploy]:** Después de cada deploy, verificar los cambios en la URL de producción: `https://renergeia-web-577313322290.us-central1.run.app/proyectos/1/costos`. **Por qué importa:** la usuaria espera confirmación visual de que los cambios están en producción.

- **2026-08-26 — [Gemini API gratis para IA]:** Se usa Google Gemini 2.0 Flash (gratis) en lugar de Claude API para el análisis inteligente de consolidados. Variable de entorno: `GEMINI_API_KEY`. **Por qué importa:** evita costos de API; el free tier es suficiente para análisis semanales.

- **2026-08-26 — [Blazor: no usar @{} en else if]:** Blazor Razor no permite bloques `@{...}` dentro de `else if`. Solución: extraer variables computadas a helper methods. **Por qué importa:** causa error RZ1010 que no es obvio desde el mensaje de error.

- **2026-08-26 — [Patrón soft-delete]:** El proyecto usa `bool Eliminado` + `HasQueryFilter(e => !e.Eliminado)` + `IgnoreQueryFilters()` para papelera. Seguir este patrón para cualquier nueva entidad que necesite papelera. **Por qué importa:** consistencia en todo el proyecto.

<!-- Agrega nuevas entradas arriba de esta línea. -->

---

Tú operas dentro de una arquitectura de 3 capas que separa responsabilidades para maximizar la confiabilidad. Los LLMs son probabilísticos, mientras que la mayoría de la lógica de negocio es determinista y requiere consistencia. Este sistema resuelve esa incompatibilidad.

## La Arquitectura de 3 Capas

**Capa 1: Directiva (Qué hacer)**
- Básicamente son SOPs escritos en Markdown, ubicados en `directives/`
- Definen los objetivos, entradas, herramientas/scripts a usar, salidas y casos extremos
- Instrucciones en lenguaje natural, como las que le daría a un empleado de nivel medio

**Capa 2: Orquestación (Toma de decisiones)**
- Esta es tu función. Tu trabajo: enrutamiento inteligente.
- Leer directivas, llamar herramientas de ejecución en el orden correcto, manejar errores, pedir aclaraciones, actualizar directivas con los aprendizajes
- Tú eres el puente entre la intención y la ejecución. Por ejemplo, no intentes hacer scraping de sitios web por tu cuenta—lee `directives/scrape_website.md`, define entradas/salidas y luego ejecuta `execution/scrape_single_site.py`

**Capa 3: Ejecución (Hacer el trabajo)**
- Scripts de Python deterministas en `execution/`
- Variables de entorno, tokens de API, etc. se almacenan en `.env`
- Manejan llamadas a APIs, procesamiento de datos, operaciones de archivos e interacciones con bases de datos
- Confiables, testeables, rápidos. Use scripts en vez de trabajo manual.

**Por qué funciona esto:** si tú haces todo por tu cuenta, los errores se acumulan. Un 90% de precisión por paso = 59% de éxito en 5 pasos. La solución es empujar la complejidad hacia código determinista. Así tú te concentras solo en la toma de decisiones.

## Principios de Operación

**1. Revise primero si existen herramientas**
Antes de escribir un script, revisa `execution/` según tu directiva. Solo crea scripts nuevos si no existe ninguno.

**2. Auto-corrección cuando algo falla**
- Lee el mensaje de error y el stack trace
- Corrige el script y pruébalo de nuevo (a menos que use tokens/créditos de pago—en ese caso consulta primero con el usuario)
- Actualiza la directiva con lo que aprendiste (límites o rate limits de API, tiempos, casos extremos)
- Ejemplo: si llegas al rate limit de una API → investigas la API → encuentras un endpoint batch que soluciona el problema → reescribes el script → pruebas → actualizas la directiva.

**3. Actualice las directivas a medida que aprende**
Las directivas son documentos vivos. Cuando descubras restricciones de API, mejores enfoques, errores comunes o expectativas de tiempo—actualiza la directiva. Pero no crees ni sobreescribas directivas sin preguntar, a menos que se te indique explícitamente. Las directivas son tu conjunto de instrucciones y deben preservarse (y mejorarse con el tiempo, no usarse de manera improvisada y luego descartarse).

## Ciclo de Auto-corrección

Los errores son oportunidades de aprendizaje. Cuando algo falla:
1. Corrija el problema
2. Actualice la herramienta
3. Pruebe la herramienta, asegúrese de que funcione
4. Actualice la directiva con el nuevo flujo
5. El sistema ahora es más robusto

## Organización de Archivos

**Estructura de directorios:**
- `.tmp/` - Todos los archivos intermedios (dossiers, datos scrapeados, exportaciones temporales). Nunca se suben al repositorio, siempre se regeneran.
- `execution/` - Scripts de Python (las herramientas deterministas).
- `directives/` - SOPs en Markdown (el conjunto de instrucciones).
- `.env` - Variables de entorno y claves de API.
- `credentials.json`, `token.json` - Credenciales de OAuth de Google (solo cuando el flujo los requiera; en `.gitignore`).

**Principio clave:** Los archivos intermedios viven en `.tmp/` y pueden borrarse siempre. Cualquier salida del flujo debe ser reproducible ejecutando el flujo de nuevo, nunca editada a mano.

## Resumen

Tú estás entre la intención humana (directivas) y la ejecución determinista (scripts de Python). Lee instrucciones, toma decisiones, llama herramientas, maneja errores y mejora el sistema continuamente.

Se pragmático. Se confiable. Auto-corríjete.

## Contexto del Proyecto RenergeIA

### Stack Técnico
- .NET 10, Blazor Server (InteractiveServer)
- Entity Framework Core 10 + PostgreSQL (Npgsql)
- Google Cloud Run (service: `renergeia-web`, project: `renergeia-app`, region: `us-central1`)
- Google Gemini API (free tier) para análisis inteligente

### Colores de Marca
- Azul: `#183963`
- Verde: `#6ABF4B`
- Gris: `#D9D9D6`
- Oscuro: `#111921`

### Deploy
```bash
# 1. Siempre publicar primero
dotnet publish RenergeIA.Web -c Release -o publish

# 2. Luego desplegar
gcloud run deploy renergeia-web --source . --project renergeia-app --region us-central1 --allow-unauthenticated --port 8080 --quiet
```

### URL de Producción
- **App principal (.NET Blazor):** https://renergeia-web-577313322290.us-central1.run.app
- **Evaluaciones HSE (Next.js):** https://evaluacion-hse-64204106653.us-central1.run.app

---

## Estado del Proyecto (actualizado 2026-09-11)

### Descripción General
RenergeIA es una plataforma de gestión integral para proyectos de energía solar fotovoltaica tipo EPC. Cubre todo el ciclo de vida del proyecto: planificación (WBS), ejecución diaria (informes), costos y presupuesto, HSEQ (Seguridad, Calidad, Ambiental, Social), control de documentos, alertas, clima, y evaluaciones HSE.

### Dos Subproyectos en el Monorepo

1. **RenergeIA.Web** (raíz) — App principal en .NET 10 / Blazor Server
   - Proyecto GCP: `renergeia-app`
   - Servicio Cloud Run: `renergeia-web`
   - Deploy: `dotnet publish` → `gcloud run deploy --source .`

2. **evaluacion-hse/** — Módulo de evaluaciones HSE en Next.js / React
   - Proyecto GCP: `renergeia-evaluaciones` (#64204106653)
   - Servicio Cloud Run: `evaluacion-hse`
   - Deploy: `gcloud builds submit --tag ...` → `gcloud run deploy --image ...`
   - **IMPORTANTE:** NO usar `gcloud run deploy --source .` desde la raíz — toma el Dockerfile .NET

### Módulos Implementados

| Módulo | Descripción | Estado |
|--------|------------|--------|
| **Proyectos** | CRUD, detalle, papelera con soft-delete y restauración | Completo |
| **WBS** | Estructura desglose de trabajo, importar Excel/PDF (Gemini), plantilla EPC | Completo |
| **Informe Diario** | Registro diario de avances, clima, fotografías | Completo |
| **Costos** | Presupuesto COP/USD, ejecutado, compromisos, comparativo, consolidado semanal con IA | Completo |
| **HSEQ Seguridad** | Plan trabajo HSE, IPERV, inspecciones, capacitaciones, EPP, permisos, ATS/AST, OTS, STC, pausas | Completo |
| **HSEQ Calidad** | Checklist ISO 9001, no conformidades, acciones correctivas, calibración, control documental, PPIs | Completo |
| **HSEQ Ambiental** | ISO 14001, aspectos/impactos, residuos, derrames, fauna/flora, inspecciones ambientales | Completo |
| **HSEQ Social** | Comunidades, reuniones, compromisos, PQR, contratación/compras locales, actas | Completo |
| **HSEQ Global** | Auditorías por norma, consolidado anual, motor de auditoría genérico | Completo |
| **Matriz Riesgos** | IPERV con IA (Gemini), mapa de riesgos, biblioteca de peligros, dashboard SST | Completo |
| **Control Documentos** | 3 categorías (proveedores, recursos, personas), importación/exportación Excel, vencimientos | Completo |
| **Alertas** | Vencimientos, flujo de aprobación multi-etapa, dashboard con tiempos promedio, tiempo real | Completo |
| **Clima** | Alertas meteorológicas operacionales inteligentes | Completo |
| **Histogramas** | Planificación y seguimiento de recursos | Completo |
| **Evaluaciones HSE** | App Next.js: evaluaciones, asistencia, personal, dashboard admin, roles | Completo |

### Variables de Entorno en Cloud Run (renergeia-web)

El servicio requiere 3 variables de entorno. Al hacer deploy con `--set-env-vars`, se REEMPLAZAN TODAS — incluir siempre las 3:
- `ASPNETCORE_ENVIRONMENT=Production`
- `GEMINI_API_KEY=<key>` (Gemini 3.6 Flash)
- `ConnectionStrings__DefaultConnection=<connection-string-postgresql>`

**NOTA:** El clasificador de seguridad de Claude Code bloquea comandos que contienen credenciales de base de datos. La usuaria debe ejecutar el deploy con env vars desde su propia terminal.

### Pendientes y Tareas Futuras

#### Pendiente Inmediato
- [ ] **Gemini API billing:** Habilitar facturación en Google Cloud para que Gemini API funcione (actualmente da error 429 RESOURCE_EXHAUSTED). La usuaria dijo "luego miramos lo de GEMINI" — retomar cuando indique.
- [ ] **CI/CD con GitHub Actions:** Service account `github-actions-deploy@renergeia-app.iam.gserviceaccount.com` ya creada. Falta:
  - Otorgar roles IAM (`roles/run.admin`, `roles/artifactregistry.writer`, `roles/iam.serviceAccountUser`, `roles/cloudbuild.builds.editor`)
  - Generar key JSON y guardarla como secreto `GCP_SA_KEY` en GitHub
  - Crear `.github/workflows/deploy.yml`
  - **Bloqueado:** el clasificador bloquea el comando de IAM — la usuaria debe ejecutarlo o usar la consola web de GCP
- [ ] **Verificar columnas Informe Diario:** Se ajustaron los anchos de columna en `CrearInformeDiario.razor` pero no se pudo verificar visualmente en el browser pane por el tamaño del DOM (10,739px de altura)

#### Mejoras Futuras (sugerencias del desarrollo)
- [ ] Autenticación y autorización de usuarios (login real, roles por proyecto)
- [ ] Dashboard ejecutivo consolidado multi-proyecto
- [ ] Exportación a PDF de informes y reportes desde la app
- [ ] Notificaciones por email (vencimientos de documentos, alertas)
- [ ] App móvil o PWA para registro en campo
- [ ] Integración con APIs de proveedores de clima más robustas

### Patrones de Diseño del Proyecto

- **Soft-delete:** `bool Eliminado` + `HasQueryFilter` + `IgnoreQueryFilters()` para papelera
- **Colores de marca:** Azul `#183963`, Verde `#6ABF4B`, Gris `#D9D9D6`, Oscuro `#111921`
- **UI framework:** Bootstrap 5 con colores customizados, Chart.js para gráficos
- **Gemini API:** modelo `gemini-3.6-flash` (antes era `gemini-2.0-flash`, deprecado)
- **PDF text extraction:** UglyToad.PdfPig v1.7.0-custom-5 (prerelease)
- **Excel parsing:** ClosedXML
- **Fuente en evaluacion-hse:** Montserrat (Google Fonts)

### Estructura de Carpetas

```
RenergeIA/
├── CLAUDE.md, AGENTS.md, GEMINI.md  ← Instrucciones para agentes IA (mantener sincronizados)
├── RenergeIA.Core/                   ← Entidades, enums, helpers
│   ├── Entities/                     ← ~60 entidades EF Core
│   ├── Enums/
│   └── Helpers/
├── RenergeIA.Infrastructure/         ← DbContext, migraciones, servicios
│   ├── Data/RenergeIADbContext.cs
│   ├── Migrations/
│   └── Services/                     ← AnalisisIAService, TrmService
├── RenergeIA.Web/                    ← App Blazor Server
│   ├── Components/Pages/             ← ~85 páginas Razor
│   ├── wwwroot/                      ← JS, CSS, imágenes
│   └── Program.cs
├── evaluacion-hse/                   ← App Next.js (TypeScript/React)
│   ├── src/app/                      ← Pages (admin, evaluacion, asistencia)
│   ├── src/components/
│   └── prisma/                       ← Schema Prisma (PostgreSQL)
├── directives/                       ← SOPs en Markdown
├── execution/                        ← Scripts de Python
├── docs/                             ← Documentación adicional
└── publish/                          ← Output de dotnet publish (no subir a git)
```

### Historial de Commits (resumen de evolución)

El proyecto lleva 28 commits en `main`. Evolución cronológica:
1. Carga inicial del proyecto
2. Módulos HSEQ (Calidad, Ambiental, Social)
3. Motor de auditoría HSEQ, matriz IPERV con IA, costos
4. WBS con disciplinas, Curva S, dashboard, costos rediseñados
5. Alertas meteorológicas inteligentes
6. Control de documentos (3 categorías, importación Excel)
7. Alertas y vencimientos, flujo de aprobación multi-etapa, dashboard
8. Informes diarios con disciplinas, eliminación módulos obsoletos
9. Evaluaciones HSE (Next.js): evaluaciones, asistencia, roles
10. Presupuesto COP/USD, consolidado semanal, compromisos
11. WBS: importación PDF/Excel, Plan HSE, papelera proyectos
12. Evaluaciones HSE: pestaña Personal, matriz de asistencia
