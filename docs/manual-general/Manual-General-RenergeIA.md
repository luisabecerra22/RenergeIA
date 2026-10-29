# RenergeIA — Manual general de la plataforma

> **Qué es este documento:** la visión completa de RenergeIA para gerencia: qué es, qué módulos tiene, de dónde sale la información, qué indicadores entrega, cómo se opera, en qué estado está y qué decisiones quedan pendientes.
> **Complementos:** el **Manual de usuario** (paso a paso por módulo) y el **Instructivo de Helvis** (asistente de WhatsApp).
> **Versión:** 2.0 — 28 de octubre de 2026.
> **Aplicación:** https://renergeia-web-577313322290.us-central1.run.app

---

## 1. Qué es RenergeIA

RenergeIA es la **plataforma para la gestión de proyectos** de Renergeia. Reúne en un solo lugar la información que antes vivía en archivos de Excel, correos, actas y chats, para los proyectos solares fotovoltaicos tipo **EPC** (ingeniería, compras y construcción).

Sus tres ideas centrales:

1. **Una sola fuente de verdad.** Cada cifra se calcula una vez y se muestra igual en todas las pantallas (inicio, dashboard, informes y WhatsApp).
2. **Los datos no se digitan dos veces.** La plataforma lee los archivos que el equipo ya produce (cronograma de MS Project, seguimiento de pagos, presupuesto base de la oferta, planificación documental, actas de reunión) y calcula a partir de ellos.
3. **Trazabilidad.** Las importaciones no duplican, avisan si un archivo es más antiguo que el último cargado, conservan quién cambió qué y permiten revertir cargas.

### Para quién

| Rol | Qué obtiene |
|---|---|
| **Gerencia** | Centro de control del portafolio: estado de cada proyecto, avance, costos al cierre, bloqueos y cumplimiento de compromisos. |
| **Dirección de proyecto / PM** | Dashboard por proyecto, plan de acción con recomendaciones, costos y flujo de caja, restricciones. |
| **Planeación / control de proyectos** | Cronograma con versiones, Curva S ponderada, histogramas de personal y equipos. |
| **Coordinadores de disciplina y obra** | Informe diario, actividades asignadas, consultas por WhatsApp. |
| **Calidad, HSE, ambiental y social** | Módulos HSEQ completos, auditorías por norma, control de ingreso y evaluaciones HSE. |
| **Control documental** | Planificación documental con alertas por días sin atender y responsables por área. |

---

## 2. Mapa de la plataforma

| Bloque | Módulos | Estado |
|---|---|---|
| **Portafolio** | Centro de control (Inicio) | ✅ En producción |
| **Por proyecto** | Dashboard · Cronograma de Actividades · Informe Diario · Documentos · Costos · Histogramas · Restricciones · Clima · Alertas | ✅ En producción |
| **Corporativo** | Compromisos (actas de reunión) · Helvis (WhatsApp) · HSEQ (Seguridad, Calidad, Ambiental, Social, Auditorías) · Control de ingreso | ✅ En producción (Helvis en etapa de prueba) |
| **Aplicación aparte** | Evaluaciones HSE (evaluaciones, asistencia, personal) | ✅ En producción |

---

## 3. Qué hay en cada módulo

### 3.1 Centro de control (Inicio) — vista del portafolio
- Una fila por proyecto con **avance programado vs real**, **brecha** en puntos porcentuales, **fecha proyectada vs contractual**, **costo al cierre vs presupuesto** y **bloqueos** abiertos o vencidos.
- **Estado** de cada proyecto: Crítico, En riesgo, Saludable, Por iniciar o Sin información, y por separado la **calidad del dato** (qué tan reciente es el último avance reportado).
- Gráficas: dona de estados (clic para filtrar), presupuesto vs costo al cierre y avance real vs programado por proyecto (clic para abrir su dashboard).
- El avance general del portafolio se pondera por presupuesto.
- ⚠ **Decisión pendiente de gerencia:** los umbrales de estado (Crítico si el retraso supera 30 días o la brecha es de −10 puntos o más; En riesgo con cualquier retraso o brecha de −3 puntos o más) son **propuestos** y deben aprobarse.

### 3.2 Dashboard del proyecto
- Tarjetas de **avance programado y real**, **brecha**, **índice de cumplimiento** (real ÷ programado) y **plazo**; estado del proyecto con su motivo.
- **Curva S** ponderada por duración y por **categorías con peso** (ej. La Soberana: Suministro 25 %, Ingeniería 10 %, Construcción 50 %, Línea y pruebas 15 %), con alcance *Todo el proyecto* o *Solo construcción*.
- **Avance por actividades** por semana (pasada, actual, futura), filtrado por **área** con su responsable y cumplimiento por área.
- **Presupuesto del proyecto** y **plan de acción** con recomendaciones armadas con datos reales (fechas, restricciones, última observación), corregibles a mano.

### 3.3 Cronograma de Actividades
- Carga desde **MS Project (.mpp)**, Excel, PDF o la **plantilla EPC estándar** de Renergeia (128 tareas).
- **Versiones**: inicial y reprogramaciones; la vigente alimenta todo lo demás y las anteriores se conservan con sus avances.
- **Disciplina automática** al cargar (aprende de las clasificaciones anteriores).

### 3.4 Informe Diario
- Avance diario por actividad, clima, fotografías, restricciones y aprobación.
- El **PDF** incluye la Curva S al corte del informe, programado vs real por disciplina y comentarios por frente (ingeniería, suministros, construcción, puesta en marcha, cierre, HSEQ).

### 3.5 Documentos (planificación documental)
- Carga del formato **FO-SI-GC-002-1** por proyecto, sin duplicar.
- **Semáforo** por días sin atender: amarillo desde 4 días, rojo desde 8; indica **en cancha de quién** está (Renergeia o Cliente).
- Responsables por área, validación, Redline / As-Built y estado *No aplica* con motivo.

### 3.6 Costos
- **Presupuesto** en COP y USD por código de rubro.
- **Seguimiento de pagos** semanal: una sola importación actualiza órdenes de compra, flujo de caja, ejecutado y comprometido.
- **Ejecutado** = lo pagado hasta la semana anterior; **comprometido** = lo programado desde la semana en curso. No se digitan.
- **Órdenes de compra** con hitos, alerta de OC sobrepasada, salarios por cargo (sin nombres) y valores proyectados.
- **Presupuesto base vs Real** (comparación contra la oferta, por costo).
- **Consolidado** semanal para gerencia con variación temporal entre cortes y comparativo USD+COP a la TRM de cierre.

### 3.7 Histogramas
- **Personal real** calculado automáticamente de la nómina (personas distintas por cargo y mes).
- **Planificado** desde el presupuesto base de la oferta; comparativo real vs planificado. Equipos se registran aparte.

### 3.8 Compromisos (actas de reunión)
- Consolida las actas y compromisos que antes se gestionaban en el sistema Actas IA.
- **Carga de actas en Word** (formato FO-SG-GC-021) una a una, con vista previa; **complementa y nunca reescribe** lo gestionado; cada carga se puede revertir.
- **Tablero** de compromisos: propios vs recordatorios a clientes, semáforo a 7 días, cierre con nota y autor, historial.
- **Seguimiento por persona**: % de cumplimiento (cumplidos ÷ total asignado), vencidos con días de atraso, pendientes y cumplidos a tiempo o tarde, por proyecto y área.

### 3.9 Helvis — asistente de WhatsApp
- El equipo consulta desde el celular el **avance**, las **alertas documentales** y sus **actividades asignadas** de cada proyecto.
- Reconoce a cada persona por su celular registrado; sin registro no entrega información. Todo queda guardado.
- **Hoy:** consulta. **Siguiente etapa:** reportar el avance diario por WhatsApp con confirmación antes de guardar.
- Detalle en el **Instructivo de Helvis**.

### 3.10 HSEQ
- **Seguridad (SST):** plan de trabajo HSE, matriz **IPERV** con apoyo de IA, inspecciones, capacitaciones, EPP, permisos de trabajo, ATS/AST, pausas activas.
- **Calidad (ISO 9001):** checklist, no conformidades, acciones correctivas, calibración, control documental, PPI.
- **Ambiental (ISO 14001):** aspectos e impactos, residuos, derrames, fauna y flora.
- **Social:** comunidades, reuniones, PQR, contratación y compras locales.
- **Auditorías** por norma (ISO 9001, 14001, 45001, Decreto 1072, Resolución 0312, cliente, interventoría) y consolidado anual.

### 3.11 Control de ingreso, alertas y clima
- Documentos de **proveedores, equipos y personas** con vencimientos y flujo de aprobación.
- **Alertas** de vencimientos con tiempos promedio de atención.
- **Clima:** alertas meteorológicas operacionales según las actividades programadas.

### 3.12 Evaluaciones HSE (aplicación aparte)
- Evaluaciones en línea, asistencia, planta de personal y matriz de asistencia por evaluación; roles por área (HSE, RRHH).
- https://evaluacion-hse-64204106653.us-central1.run.app

---

## 4. De dónde sale la información

| Archivo o fuente | Quién lo produce | Qué alimenta en RenergeIA | Frecuencia |
|---|---|---|---|
| Cronograma MS Project (.mpp) | Planeación | Cronograma, Dashboard, Curva S, Centro de control, Helvis, Clima | Al iniciar y en cada reprogramación |
| Informe diario | Obra | Avance real, Curva S, PDF del informe | Diario |
| Seguimiento de pagos (archivo semanal) | Financiera | Órdenes de compra, flujo de caja, ejecutado, comprometido, histograma de personal, consolidado | Semanal |
| Presupuesto base (oferta) | Comercial / Ingeniería | Presupuesto base vs Real, histograma planificado | Al ganar el proyecto |
| Codificación de costos | Financiera | Cruce código de pago → rubro → presupuesto | Al iniciar |
| Planificación documental FO-SI-GC-002-1 | Control documental | Documentos, alertas, Helvis | Semanal |
| Actas de reunión FO-SG-GC-021 (Word) | Quien levanta el acta | Compromisos, seguimiento por persona | Por reunión |
| WhatsApp | Equipo de proyecto | Consultas (hoy) y reportes de avance (Etapa 2) | Continuo |

---

## 5. Indicadores clave y cómo leerlos

| Indicador | Cálculo | Lectura |
|---|---|---|
| **Avance programado / real** | Curva S ponderada por duración y por categorías | Lo que debería llevar el proyecto hoy vs lo que lleva. |
| **Brecha** | Real − programado, en puntos porcentuales | Negativa = atraso. |
| **Índice de cumplimiento** | Real ÷ programado | 1,00 = al día. No es un índice de valor ganado (el cronograma no trae costos). |
| **Fecha proyectada vs contractual** | Fin del cronograma vigente vs fecha fin del contrato | Días de retraso o adelanto. |
| **Costo al cierre** | Presupuesto + sobrecostos por código (ejecutado + comprometido por encima del presupuesto) | Riesgo de sobrecosto, en COP. |
| **Documentos en rojo** | Documentos con 8 días o más sin atender | Cuello de botella documental y en cancha de quién está. |
| **% de cumplimiento de compromisos** | Compromisos cumplidos ÷ total asignado a la persona | Disciplina de seguimiento por responsable. |

---

## 6. Cómo se opera (rutina recomendada)

| Cuándo | Qué hacer | Quién |
|---|---|---|
| **Al iniciar un proyecto** | Crear el proyecto, cargar presupuesto base, cronograma, codificación, planificación documental y configurar HSEQ | PM, planeación, financiera, control documental |
| **Diario** | Registrar el informe diario de avance | Obra |
| **Semanal** | Importar seguimiento de pagos y planificación documental; revisar alertas, documentos en rojo y compromisos vencidos | Financiera, control documental, PM |
| **Semanal** | Reunión de planeación con el Dashboard y el plan de acción; cargar las actas | PM, planeación |
| **Semanal / mensual** | Revisar el Centro de control y el consolidado de costos | Gerencia |
| **Cierre** | Redline / As-Built, auditorías y consolidado final | PM, calidad |

---

## 7. Seguridad, datos y continuidad

- La plataforma corre en **Google Cloud** (Cloud Run) con base de datos **PostgreSQL**; cada actualización se publica sin perder datos.
- Acceso con **usuario y contraseña**; las sesiones sobreviven a las actualizaciones.
- **Helvis** reconoce por número de celular y no entrega información a números no registrados; la conexión con WhatsApp valida la firma de cada mensaje de Meta.
- Las cargas de archivos son **idempotentes** (no duplican) y las de actas se pueden **revertir**.
- Los proyectos eliminados pasan a una **papelera** y se pueden restaurar.
- ⚠ **Pendiente:** permisos por rol dentro de la plataforma (hoy cualquier usuario con sesión puede editar).

---

## 8. Estado y hoja de ruta

### Logros recientes (septiembre – octubre de 2026)
- Centro de control del portafolio con una sola fuente de indicadores.
- Curva S ponderada con categorías y alcance de construcción.
- Costos integrados al seguimiento de pagos semanal, con variación temporal en el consolidado.
- Módulo de Compromisos con carga de actas en Word y seguimiento por persona.
- Helvis conectado a WhatsApp: reconocimiento por celular, selector de proyecto, avance, alertas y actividades asignadas.
- Nombres de documentos internos ocultos en la interfaz.

### Próximos pasos

| # | Paso | Requiere |
|---|---|---|
| 1 | **Aprobar los umbrales** de estado del Centro de control | Decisión de gerencia |
| 2 | **Número real de WhatsApp** para Helvis y control 100 % interno de la cuenta de Meta | SIM corporativa y gestión del Portfolio comercial |
| 3 | **Helvis Etapa 2:** reporte de avance por WhatsApp | Cantidades y unidades por actividad del cronograma |
| 4 | **Permisos por rol** en la plataforma | Definición de roles |
| 5 | **Despliegue automático** desde GitHub | Permisos en Google Cloud |
| 6 | **Análisis con IA** (consolidado, matriz IPERV) | Habilitar facturación de la API de Gemini |
| 7 | Lectura automática desde **SharePoint** y notificaciones por correo | Registro de la app en Microsoft 365 (IT) |

---

## 9. Decisiones que se esperan de gerencia

1. Aprobación de los **umbrales** de estado del portafolio (Crítico / En riesgo).
2. **Número corporativo** para Helvis y quién administra la cuenta de Meta.
3. **Roles y permisos** de usuarios en la plataforma.
4. Presupuesto para **facturación de la IA** (Gemini) y de los mensajes de WhatsApp por encima de los gratuitos.
5. Priorización entre Helvis Etapa 2, permisos por rol e integración con Microsoft 365.
