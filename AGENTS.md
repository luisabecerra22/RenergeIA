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

- **2026-09-29 — [Presentación de Evaluaciones HSE con capturas reales del panel]:** Se creó `docs/presentacion-evaluaciones/` (PPTX de 17 diapositivas para los equipos HSE/RRHH; generador `build_deck.js` con pptxgenjs+sharp+react-icons; capturas en `shots/`, logo al pie izquierdo, Montserrat, colores de marca). **Capturas del panel admin (requiere login que yo no puedo escribir):** la usuaria inicia sesión en SU navegador y se maneja con la extensión Claude in Chrome (`mcp__claude-in-chrome__*`); `computer` screenshot con `save_to_disk:true` deja el JPEG en la carpeta tool-results de la sesión y de ahí se copia a `shots/`. Para **difuminar cédulas/correos** se inyecta CSS con `javascript_tool` ANTES de capturar: blur a las celdas de las columnas cuyo `<th>` contiene "cedula"/"correo" y a los `<p>` del modal con "C.C."/"@"; para columnas anchas se pone `scrollLeft=scrollWidth` en el contenedor con overflow. **Montserrat** no viene instalada: se bajan las TTF estáticas (repo `JulietaUla/Montserrat`), se instalan a nivel usuario (copiar a `%LOCALAPPDATA%\Microsoft\Windows\Fonts` + registrar en `HKCU\...\Fonts` + `AddFontResource` + broadcast `WM_FONTCHANGE`) y se **incrustan** abriendo el PPTX con PowerPoint COM y `SaveAs(path, 24, msoTrue)`; el PDF con `SaveAs(path, 32)` y los PNG de revisión con `Presentation.Export(dir,"PNG",1600,900)`. **Por qué importa:** las pantallas con login se capturan en el navegador ya autenticado de la usuaria (no puedo teclear contraseñas), y para que Montserrat se vea igual en cualquier PC hay que instalarla y luego incrustarla con PowerPoint.

- **2026-09-29 — [Consolidado: variación temporal = pagos del Flujo de Caja entre dos cortes de tesorería]:** Las tablas "Variación temporal USD+COP/USD/COP" ya NO se digitan: cada importación de tesorería guarda una copia del flujo semanal en `CortesFlujoTesoreria` (ProyectoId, FechaCorte del nombre del archivo, Semana, PagosCOP, PagosUSD — los mismos totales por semana de la pantalla Flujo de Caja; se reemplaza si se reimporta el mismo corte) y el Consolidado compara dos cortes elegibles (selects "Comparar cortes", por defecto los dos más recientes; la elección se guarda en `InformeConsolidado.CorteFlujoActual/Anterior`, migración `AddCortesFlujoTesoreria`). Filas = rango del Flujo de Caja (`Proyecto.FechaInicioPagos→FechaFinPagos`, paso 7 días). La **estructura de la tabla NO se toca** (exigido por la usuaria el 2026-09-29): dos bloques lado a lado "Cierre semana <corte actual>" y "Cierre semana <corte anterior>" cada uno con Semana/Ingresos/Pagos/Caja, luego Variación (caja acumulada actual − anterior) y Justificación. Los **Pagos** salen de los cortes guardados (solo lectura); los **Ingresos** siguen digitables (se guardan en `FlujoCajaSemanal`) hasta que exista la hoja de ingresos; Caja = ingresos − pagos acumulados. Justificaciones se conservan en `FlujoCajaSemanal` por (TipoFlujo, Semana). Gráfica `renderFlujoComparativo` en app.js (la vieja llamaba `renderFlujoChart`, que nunca existió). El selector "contra" lista TODAS las semanas anteriores al corte (de 7 en 7 hasta `FechaInicioPagos`), con la semana anterior (corte − 7 días) por defecto; si esa fecha tiene snapshot importado se usa, y si no se **deriva del corte actual** (el archivo es acumulado): mismas cifras pero con las semanas pagadas después de ese cierre vacías (la columna de cada semana cubre lunes→domingo, así que a un cierre X están pagadas las semanas ANTERIORES a X: se excluyen las semanas w con cierre ≤ w < corte actual) → la variación (Caja actual − Caja anterior) muestra lo pagado desde ese cierre. La tabla **USD+COP se expresa en DÓLARES**: los pesos se convierten con la TRM de cierre del informe (`InformeConsolidado.TRM`), definido por la usuaria el 2026-09-29. El **Comparativo Semana N vs Semana M** (Presupuesto/Ejecutado/Comprometido/Disponible) tiene además del grupo USD y el COP un tercer grupo **USD+COP en dólares** (USD + COP÷TRM), donde AMBAS semanas se convierten con la MISMA TRM de cierre actual (`_informe.TRM`), para que la variación refleje solo costos sin efecto cambiario (definido por la usuaria el 2026-09-30) (definido por la usuaria: NUNCA pedirle cargar archivos viejos). La UI usa campos locales `_corteActual/_corteAnterior` y solo escribe en el informe en modo edición (DbContext compartido). **Por qué importa:** las variaciones salen solas del archivo de tesorería; si cambia el parser del flujo hay que mantener la copia por corte.

- **2026-09-28 — [Razor imprime literal lo que parece un correo: `F@doc.Fase`]:** La columna FASE de Documentos mostraba el texto crudo `F@doc.Fase` porque Razor aplica la heurística de direcciones de correo: si el `@` va pegado a un carácter alfanumérico anterior (`F@…`) lo trata como email y NO evalúa la expresión. Solución: interpolar (`@($"F{doc.Fase}")`) o separar (`F @doc.Fase`). Ojo: `@expr1@expr2` sí funciona (el `@` posterior a una expresión no dispara la heurística), por eso `@DesvPref@_vm.Desviacion…` en Home.razor sí renderiza. **Por qué importa:** nunca pegar un prefijo alfanumérico a una expresión Razor; se ve en producción como texto literal y no da error de compilación.

- **2026-09-28 — [Informe Diario: gráficas y comentarios por disciplina en el PDF]:** El "Exportar PDF" del informe es `window.print` sobre `DetalleInformeDiario.razor` con CSS `@media print` (horizontal). Ahora la página dibuja con Chart.js la **Curva S al corte de la fecha del informe** (`DatosCurvaSAsync(proyectoId, soloConstruccion, fechaCorte)`; `ConstruirCurvaS` usa `hoy = fechaCorte ?? HoyColombia()`) y **Programado vs Real por disciplina** (promedios de las filas hoja del propio informe, no del dashboard), ambas antes de la tabla y con `page-break-inside: avoid`; la impresión automática (`?print=1`) espera 1,2 s para que Chart.js dibuje. **Ajuste 2026-09-28 (pedido de la usuaria):** las gráficas NO se muestran en pantalla en el informe (viven en el Dashboard); el contenedor `.graficas-informe` se dibuja fuera de la vista (`position:absolute; left:-12000px; width:1120px`) con canvas de tamaño FIJO 1120px (renderCurvaS/renderBarChart aceptan `fixedW, fixedH` → `responsive:false, maintainAspectRatio:false, animation:false`; los canvas ocultos no se pueden medir, por eso el tamaño fijo) y en `@media print` el contenedor vuelve al flujo con `canvas { width:100% }`, ocupando todo lo ancho de la hoja horizontal. **Ajuste 2:** el Real % del informe (tabla y grafica de disciplinas) cruza los registros del informe por CodigoWBS con la version vigente y, para actividades sin registro en ese informe, usa `AvanceRealActividadesAsync(proyectoId, fechaCorte)` (misma logica de la curva); las actividades al 100 % sin FechaFinReal se asumen terminadas en su fecha fin PLANEADA (no repartidas hasta hoy). `renderBarChart` quedo con `maintainAspectRatio:false` siempre y el canvas del dashboard va en un wrapper `position:relative;height:280px` para llenar el ancho. Los % del informe se cortan a la FECHA DEL INFORME (el dashboard corta a hoy): es esperado que difieran. Al final, **Comentarios por disciplina**: entidad `ComentarioDisciplinaInforme` (tabla `ComentariosDisciplinaInforme`, único por InformeDiarioId+Clave, migración `AddComentariosDisciplinaInforme`) con seis secciones fijas en código (ingenieria, suministros, construccion, puesta-marcha, cierre, hseq); se editan en el detalle mientras el informe no esté Aprobado y se imprimen con `.comentario-print` (los textarea se ocultan al imprimir). **Por qué importa:** cualquier cambio al PDF del informe se hace en esa página y su bloque `@media print`; las secciones de comentarios se agregan en `SeccionesComentario`, no en BD.

- **2026-09-28 — [Blazor Server en Cloud Run: timeout 300 s cortaba el WebSocket cada 5 min]:** La usuaria reportó que al pulsar "Actualizar" en el dashboard la página se bloqueaba. Causa: el servicio `renergeia-web` tenía `timeoutSeconds=300` y sin afinidad de sesión; Cloud Run cerraba el WebSocket de Blazor (`_blazor`) exactamente cada 5 min (consola: `WebSocket closed with status code: 1006`) y el circuito se reconectaba perdiendo el estado. Solución: desplegar SIEMPRE con `--timeout=3600 --session-affinity` (ya incluido en el comando de deploy de este archivo). **También:** las categorías de la Curva S aparecieron guardadas con `Incluye` vacío (curva en blanco con "No hay actividades con fechas programadas"); ahora `GuardarCategoriasCurvaAsync` rechaza categorías sin códigos y, si las categorías no coinciden con ningún código del cronograma, la curva cae a "sin categorías" y avisa (`CurvaSData.CategoriasSinEfecto`). **Decisión de la usuaria:** la fuente de la curva real es el Cronograma de Actividades de la app (más el informe diario cuando exista); NO pedirle informes ni históricos: la línea real se dibuja completa y sólida desde el avance del cronograma; "Histórico real" queda como opción secundaria. **Por qué importa:** no bajar el timeout ni quitar la afinidad en futuros deploys, y no volver a redactar la UI como si faltaran datos.

- **2026-09-26 — [Documentos: acciones fijas, edición en modal, estado No aplica, PDF con Blob; No Conformidades del proyecto retirado]:** (1) La página se ensanchaba con la tabla ancha (KPIs y botones quedaban fuera de la vista) porque `main` es flex item sin `min-width:0`: la tabla dentro de `.table-responsive` fija el min-content del `<main>`; se corrigió en `MainLayout.razor.css` (`main{min-width:0}`) y aplica a TODAS las páginas con tablas anchas. (2) En `ListaDocumentos` la columna **Acciones** es la PRIMERA y queda fija (`.tabla-documentos .col-fija`, sticky left, en `app.css`); la edición inline se reemplazó por la ventana modal **Editar documento** (todos los campos, incluidos Redline/As-Built con responsables y observaciones); clic en la celda Redline/As-Built también abre el modal. (3) Nuevo `EstadoDocumento.NoAplica` agregado al FINAL del enum (se guarda como int); exige motivo en Observaciones al guardar, no cuenta en "sin atender" (`RequiereAtencion`), "En cancha de" = No aplica, y el parser del Excel lo reconoce como "no aplica"/"N/A". (4) **Chrome y Edge bloquean `window.open('data:text/html…')`** (pestaña en blanco): el Informe PDF ahora usa `window.abrirInformeHtml` (Blob URL; si el popup se bloquea descarga el .html) y Exportar usa `downloadFile`; nunca volver a `eval` con URLs `data:` para abrir pestañas. (5) Se eliminó el módulo **No Conformidades** del menú del proyecto y de las tarjetas de DetalleProyecto porque duplicaba HSEQ → Calidad → No Conformidades (definido por la usuaria); la ruta vieja `/proyectos/{id}/no-conformidades` redirige a la de Calidad; la entidad `NoConformidad` y su tabla se conservan (las usan otras páginas HSEQ). **Por qué importa:** cualquier página nueva con tabla ancha ya no necesita hacks de ancho; los estados de documento se agregan siempre al final; y no reintroducir un registro de NC fuera de HSEQ.

- **2026-09-26 — [Recomendaciones del Plan de acción: reglas con datos + corrección manual]:** La usuaria rechazó las 3 frases fijas que había en `ActividadDashboard.Recomendacion` ("así no van a ser aprobadas"). Ahora `Core/Helpers/RecomendacionActividad.Generar` arma un texto determinista con: situación frente al cronograma (vencida / días restantes y ritmo %/semana / no iniciada), restricciones Abierta o EnGestion ligadas a la actividad vía `RegistroAvanceDiario.RestriccionesRelacionadas` (cruce por CódigoWBS para cubrir versiones anteriores), última observación o novedad del Informe Diario, aviso si no hay informe hace ≥ 7 días y ruta crítica. La manual gana: `ActividadWBS.RecomendacionManual/Por/Fecha` (migración `AddRecomendacionManualActividad`), se edita con ✎ en el Dashboard (`ExecuteUpdateAsync`, sin tocar el tracker) y se copia en las reprogramaciones. No se usa Gemini. **Por qué importa:** la única forma de que aparezca una restricción en la recomendación es marcarla en el Informe Diario al reportar la actividad; y cualquier texto nuevo del Dashboard debe salir de datos del proyecto, no de frases genéricas. **Gotcha:** otra sesión estaba editando `DashboardProyecto.razor` en paralelo y lo re-guardó con CRLF, por lo que los parches por texto deben normalizar finales de línea antes de buscar.

- **2026-09-26 — [Dashboard: disciplinas graficadas, colores y tipografía de Chart.js]:** "Avance por disciplina" y "Detalle por disciplina" usan `InformeDiarioService.DisciplinasDashboard` = Suministros → Ingeniería → Civil → Mecánica → Eléctrica → Puesta en marcha → Cierre de proyecto (orden fijo definido por la usuaria; Contractual, General y Construcción NO se grafican aunque existan). Chart.js dibuja por defecto en Helvetica/Arial y con verdes/azules de Bootstrap: en `app.js` se fijó `Chart.defaults.font.family = Montserrat, Verdana` para TODAS las gráficas y `renderBarChart` usa verde `#6ABF4B` (real) y azul `#183963` (programado). **Por qué importa:** cualquier gráfica nueva debe usar los colores de marca y no los de Bootstrap (`#198754`, `#0d6efd`), y si se agrega una disciplina al enum hay que decidir explícitamente si entra en `DisciplinasDashboard`.

- **2026-09-26 — [Disciplinas del WBS: enum como int, se agregan al FINAL, y disciplina automática]:** `Disciplina` se persiste con `HasConversion<int>()`: renombrar un miembro es seguro (`HotCommissioning` → `PuestaEnMarcha`, valor 4; `CierreProyecto` ahora se muestra "Cierre de proyecto", antes "Dossier") pero un miembro nuevo SIEMPRE va al final (`Ingenieria` = 8, `Construccion` = 9) para no correr los valores guardados; no requiere migración. Los selectores (`FormWBS`, `ListaWBS`) se llenan con `Enum.GetValues<Disciplina>()` y el texto sale de `EnumDisplay`. La usuaria no quiere volver a clasificar a mano al cargar otro cronograma: `Core/Helpers/DisciplinaSugeridor` resuelve (1) memoria por nombre normalizado (sin tildes/mayúsculas/signos) contra TODAS las actividades ya clasificadas —mismo proyecto y versión más reciente primero, luego otros proyectos—, (2) palabras clave ordenadas de lo específico a lo general (Cierre → Puesta en marcha → Ingeniería → Contractual → Suministros → Civil fuerte → Eléctrica → Mecánica → Civil → Construcción; claves de ≤4 letras se exigen como palabra completa) y (3) disciplina del padre. Se aplica en `CrearActividadesDesdeMppAsync` (plantilla, .mpp inicial, reprogramación), en la importación Excel/PDF y con el botón **Completar disciplinas** (solo rellena las vacías de la versión vigente). **Solo las HOJAS llevan disciplina** (definido por la usuaria el 2026-09-26): las actividades con hijas promedian el avance y muestran "—" fijo en la lista y en el formulario; el tercer criterio del sugeridor pasó a ser palabras clave del NOMBRE del padre/abuelo (ya no su disciplina); `RecalcularYGuardarPadresAsync` borra la disciplina de cualquier padre al abrir la versión vigente, `FormWBS` la anula al guardar si tiene hijas, y en .mpp se usa `EsResumen` / en Excel-PDF "nadie tiene CodigoPadre == mi código" para saber qué es hoja. Hincado cuenta como Mecánica (así lo codifica la empresa: ESTC). `DisciplinasConstruccion` de la Curva S incluye Construcción. **Por qué importa:** insertar un valor en medio del enum reasignaría en silencio todas las disciplinas; y toda corrección manual queda en la memoria y gana sobre las palabras clave en la siguiente carga.

- **2026-09-26 — [Capturas reales de la app y presentación PowerPoint para gerencia]:** Se creó `docs/presentacion-gerencia/` (PPTX de 20 diapositivas, capturas en `capturas/` y el generador `build_deck.js` con pptxgenjs). Cómo se tomaron las capturas **sin login manual**: la extensión Claude in Chrome controla el Edge de la usuaria (con su sesión), pero sus screenshots no se guardan en disco y la captura de escritorio del monitor LG sale con colores lavados (HDR); lo que funcionó fue inyectar **html2canvas** en la página con `javascript_tool`, forzar `body{width:1600px}` + `main{min-width:0}` + `canvas{max-width:100%}` y disparar `resize` (si no, Chart.js y el `<main>` conservan el ancho del viewport de 2529 px, porque la usuaria tiene Edge al 75 %), y enviar el PNG por `fetch` a un **servidor Node local** (puerto 8765) que lo escribe en disco. Las pestañas de Blazor se cambian con `button.click()` por texto. Para revisar el PPTX se exporta cada diapositiva a PNG con **PowerPoint por COM** desde PowerShell (`Slide.Export`), porque no hay LibreOffice ni Python. **Montserrat NO está instalada** en la máquina (Verdana sí): la presentación usa Verdana. Decisiones de la usuaria: fondo blanco salvo portada y cierre (azul `#183963`), títulos en negrita, logo en el pie, lema "Plataforma para la gestión de proyectos", sin Evaluaciones HSE, datos reales de La Soberana sin ocultar, propuesta en dos etapas (piloto La Soberana → todos los proyectos). **Por qué importa:** para actualizar la presentación basta retomar capturas con el mismo método y correr `node build_deck.js` (requiere `npm i pptxgenjs sharp react-icons react react-dom` en la carpeta); no intentar capturas de escritorio ni LibreOffice en este equipo.

- **2026-09-26 — [Curva S del Dashboard: ponderada por duración y real por actividad]:** La curva anterior no tenía forma de S por tres causas: (1) el planificado era un promedio SIMPLE de las actividades hoja (un hito de 0 días pesaba igual que una actividad de 6 meses) y `CalcularAvanceEsperado` devolvía 100 % para los hitos (fin <= inicio) desde el día 1, por eso la línea base arrancaba en ~5 %; (2) el real promediaba solo las actividades informadas ESE día (salto de 0 a 35 % en el primer informe y plano después) y el punto de hoy usaba otra fórmula (promedio de `ActividadWBS.AvanceReal`), de ahí el segundo salto; (3) el alcance es distinto: el WBS de la app incluye contractual/ingeniería/suministros desde Ago-25 y el informe interno solo construcción desde Feb-26. Ahora (`InformeDiarioService.CalculadoraCurvaS`): peso = duración planeada en días (hitos sin peso; si todo pesa 0, pesos iguales), hito = 0 % antes de su fecha y 100 % desde ella, real(fecha) = último `AvanceAcumulado` informado por actividad hasta esa fecha (0 si no se había informado; sin ningún informe → `AvanceReal` del WBS), la línea real tiene tramo **punteado = estimado** (antes del primer informe: avance de cada actividad repartido linealmente desde su `FechaInicioReal` o planeada; sin informes, hasta hoy) y tramo **sólido = informado** (flag `RealEstimado` por punto → `segment.borderDash` en Chart.js). Ojo: los informes diarios existentes del proyecto 1 apuntan a actividades que NO son hojas del cronograma vigente (versión anterior o padres), por eso la app dice "aún no hay informes" y toda la línea sale estimada. `hoy` en hora Colombia, y las tarjetas AVANCE PROGRAMADO/REAL usan el mismo cálculo que la curva. **Histórico real (misma fecha):** la usuaria rechazó la línea estimada ("se ve falsa"); como el proyecto 1 solo tiene UN informe diario (05/07/2026, sobre una versión anterior del WBS), el histórico real de la Curva S solo puede venir del informe interno: entidad `PuntoCurvaReal` (tabla `PuntosCurvaReal`, único por ProyectoId+SoloConstruccion+Fecha, migración `AddPuntosCurvaReal`), botón **Histórico real** en el dashboard con textarea para pegar fecha + % desde Excel (`CurvaRealHistoricaParser`: tab/;/|/espacios, dd/MM/yyyy, d-MMM-yy es/en, seriales Excel, "41,5%" o fracciones si todo ≤ 1), guardar REEMPLAZA el histórico del alcance. La línea real queda en 3 tramos: histórico (sólido) → informes diarios (sólido, los registros de versiones anteriores se cruzan por CódigoWBS con la hoja vigente) → puente estimado punteado hasta el avance actual del WBS. **Gotcha `dotnet ef`:** el tool global fallaba (apphost RC); se actualizó a 10.0.12 y solo corre con `PATH` incluyendo `~/.dotnet`, `DOTNET_ROOT=C:SERSuisa becerra.dotnet` y `dotnet_roll_forward=major`; nunca usar `--no-build` al agregar migraciones con entidades nuevas. **Categorías con peso (misma fecha):** la usuaria explicó que su Curva S General = Σ peso de categoría × curva de la categoría (Suministro principal 25 %, Ingeniería 10 %, Construcción de la planta 50 %, EPC Línea de transmisión y pruebas 15 %; dentro de cada una, "Ponderado Actividad" y avance lineal por duración). Implementado: entidad `CategoriaCurvaS` (tabla `CategoriasCurvaS`, migración `AddCategoriasCurvaS`: Nombre, Peso, Incluye/Excluye = códigos WBS por prefijo, EsConstruccion) + editor "Categorías y pesos" en el dashboard; `CalculadoraCurvaS` combina grupos (peso de categoría × promedio ponderado por duración del grupo); hojas sin categoría NO entran y se avisa. Mapeo La Soberana definido por la usuaria: Suministro 2.6, 2.7 · Ingeniería 2.4, 2.5 · Construcción 2.8 y 2.9 menos 2.8.7/2.8.9/2.8.10 más hitos 1, 2.1, 2.2, 2.3 · Línea y pruebas 2.8.7, 2.8.9, 2.8.10, 2.10, 2.11, 2.12, 2.13, 2.14, 2.15, 2.16 (hitos finales incluidos por decisión de la usuaria el 2026-09-28; 0 actividades sin categoría). Decidido también: NO reemplazar el cronograma de la app con las fechas del Excel (están desactualizadas). Fuente: `COS5SO - Informe Diario La Soberana 09.07.2026.xlsx`, hoja "Curva S General" (fila 4 fechas diarias desde K, filas 5-12 planning/real por categoría, fila 17 total planning, fila 18 total real; real hasta 16/07/2026 = 41,5 %). Ambas series (total y construcción, fila 10) quedaron cargadas en producción el 2026-09-26 vía el modal Histórico real, y las 4 categorías configuradas; los hitos 2.14/2.15/2.16 se incluyeron en Línea y pruebas el 2026-09-28. Regla añadida: el tramo con informes diarios solo se dibuja sólido si ≥ 50 % del peso de la curva tiene informe a esa fecha (`CalculadoraCurvaS.ParteInformada`); si no, sigue punteado. Selector de alcance en el dashboard: Todo el proyecto / Solo construcción (disciplinas Civil, Mecánica, Eléctrica; `DatosCurvaSAsync(id, soloConstruccion)`). Chart.js pasó a `cubicInterpolationMode: monotone` para que no haga ondas entre puntos. No hay ponderación por costo ni horas-hombre porque el .mpp importado no las trae (`MppParser` solo lee fechas y % completo); sería el siguiente paso si la usuaria quiere igualar exactamente el informe interno. **Por qué importa:** cualquier ajuste al avance esperado, a los pesos o al alcance debe hacerse en `CalculadoraCurvaS` para que curva y tarjetas sigan diciendo lo mismo; y el histórico real anterior al primer informe diario no existe en la app, no se puede "reconstruir".

- **2026-09-24 — [Canal de WhatsApp: webhook propio contra la Cloud API de Meta]:** Se decidió conectar **directo con Meta, sin BSP**: Twilio cobra US$ 0,005 por mensaje enviado **y recibido** (6x la tarifa de Meta para Colombia) y 360dialog €49/mes por número; directo solo se pagan las tarifas de Meta (Colombia utilidad/servicio US$ 0,0008, "Resto de América Latina" —donde cae Panamá— US$ 0,0113, marketing 0,0125 / 0,0740). Desde el **01/10/2026** Meta cobra también los mensajes de servicio, con **1.000 gratis al mes por número**, y deja de ser gratis la plantilla de utilidad dentro de la ventana de 24 h. Reglas que condicionan el diseño: solo se puede escribir libre dentro de las **24 h** desde el último mensaje de la persona; fuera de eso, **plantillas aprobadas** previamente. Implementación en `RenergeIA.Web/Services/WhatsApp/` (`WhatsAppOptions`, `WhatsAppClient`, `WhatsAppWebhook`, `WhatsAppConversacion`): endpoint `GET/POST /api/whatsapp` (`.AllowAnonymous()` + `.DisableAntiforgery()`), verificación por `hub.verify_token`, validación de firma `X-Hub-Signature-256` (HMAC-SHA256 del cuerpo crudo con el App Secret, comparación en tiempo constante), **dedupe por `message.id`** porque Meta reintenta si no se responde rápido, y **siempre HTTP 200** ante errores propios para que no reintente. Config por `WhatsApp__VerifyToken|AppSecret|AccessToken|PhoneNumberId` (en Cloud Run se pasan TODAS las env vars juntas, ver más abajo). El bot es **de menú, determinista**, sin IA decidiendo. **Gotcha del entorno:** `dotnet` no está en el PATH, vive en `C:\Users\Luisa Becerra\.dotnet\dotnet.exe`, y no hay PostgreSQL local, así que la app completa no corre en esta máquina; para probar el webhook sin BD se levantó un csproj temporal en el scratchpad que enlaza esos `.cs` con `<Compile Include>` y se probó con curl + `openssl dgst -sha256 -hmac`. **Por qué importa:** cualquier cambio al webhook debe conservar la validación de firma, el dedupe y el 200 siempre; y esa es la forma de probarlo sin base de datos.

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
gcloud run deploy renergeia-web --source . --project renergeia-app --region us-central1 --allow-unauthenticated --port 8080 --timeout=3600 --session-affinity --quiet
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
