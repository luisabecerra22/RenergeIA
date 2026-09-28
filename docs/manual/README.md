# Manual de Usuario — RenergeIA

> Plataforma de gestión integral de proyectos EPC fotovoltaicos de Renergeia.
> **Estado del manual:** en construcción (inició el 16 de septiembre de 2026). Cada capítulo se actualiza cuando cambia el módulo.
> **Aplicación:** https://renergeia-web-577313322290.us-central1.run.app

---

## Cómo usar este manual

- Cada capítulo explica un módulo con la misma estructura: **Para qué sirve → Conceptos clave → Cómo funciona → Paso a paso → Qué hacer en caso de… → Relación con otros módulos → Buenas prácticas**.
- Los nombres de botones y pestañas aparecen en **negrita**, tal como se ven en la pantalla.
- Las notas **⚠ Por confirmar** marcan reglas que aún deben validarse con el equipo; no las tomes como definitivas.
- Para detalles técnicos (código, base de datos, despliegue) consulta `GUIA_DE_DESARROLLO.md`; este manual es para quien **usa** la aplicación.

---

## Índice

| # | Capítulo | Módulo |
|---|---|---|
| 0 | [Primeros pasos](00-primeros-pasos.md) | Acceso, navegación y flujo general de un proyecto |
| 1 | [Proyectos](01-proyectos.md) | Crear, editar, detalle, papelera |
| 2 | [Cronograma de Actividades](02-cronograma-actividades.md) | WBS, plantilla EPC, MS Project, reprogramaciones, restricciones |
| 3 | [Informe Diario](03-informe-diario.md) | Avance diario, clima, fotos, aprobación |
| 4 | [Documentos](04-documentos.md) | Planificación documental, alertas sin atender, responsables, validación |
| 5 | [Costos](05-costos.md) | Presupuesto, tesorería, órdenes de compra, flujo de caja, BOM vs Real, consolidado |
| 6 | [Histogramas](06-histogramas.md) | Personal y equipos planificado vs real |
| 7 | [HSEQ — Seguridad](07-hseq-seguridad.md) | SST, IPERV, inspecciones, capacitaciones, permisos |
| 8 | [HSEQ — Calidad](08-hseq-calidad.md) | ISO 9001, no conformidades, calibración, PPIs |
| 9 | [HSEQ — Ambiental](09-hseq-ambiental.md) | ISO 14001, residuos, aspectos e impactos |
| 10 | [HSEQ — Social](10-hseq-social.md) | Comunidades, PQR, contratación local |
| 11 | [HSEQ — Auditorías](11-hseq-auditorias.md) | Motor de auditoría y consolidado anual |
| 12 | [Control de ingreso y alertas](12-control-ingreso-y-alertas.md) | Proveedores, equipos, personas, vencimientos |
| 13 | [Clima](13-clima.md) | Alertas meteorológicas operacionales |
| 14 | [Dashboard del proyecto](14-dashboard-proyecto.md) | Indicadores del proyecto |
| 15 | [Evaluaciones HSE](15-evaluaciones-hse.md) | Plataforma de evaluaciones y asistencia (aplicación aparte) |
| 16 | [Problemas frecuentes](16-problemas-frecuentes.md) | Qué hacer en caso de fallas generales |

---

## Historial del manual

| Fecha | Cambio |
|---|---|
| 2026-09-26 | Documentos: columna **Acciones** fija al inicio de la tabla, ventana **Editar documento** con todos los campos (incluidos Redline/As-Built y responsables), estado **No aplica** con motivo obligatorio, Informe PDF corregido (abría en blanco) y la página ya no se ensancha con la tabla. Se retiró el módulo **No Conformidades** del menú del proyecto (duplicado con HSEQ → Calidad). |
| 2026-09-26 | Cronograma de Actividades: disciplinas **Puesta en marcha** (antes Hot Commissioning) y **Cierre de proyecto** (antes Dossier), nuevas **Ingeniería** y **Construcción**; disciplina automática al cargar cronogramas y botón **Completar disciplinas**; las actividades con subactividades ya no llevan disciplina (promedian a sus hijas). |
| 2026-09-26 | Dashboard: la **Recomendación** del Plan de acción se arma con datos reales (fechas, restricciones abiertas ligadas por el Informe Diario, última observación, ruta crítica) y se puede corregir a mano por actividad (✎, etiqueta Manual). |
| 2026-09-26 | Dashboard: **Avance por disciplina** y **Detalle por disciplina** muestran solo Suministros, Ingeniería, Civil, Mecánica, Eléctrica, Puesta en marcha y Cierre de proyecto, en ese orden, con colores de marca y tipografía Montserrat en todas las gráficas. |
| 2026-09-26 | Dashboard: Curva S ponderada por duración, categorías con peso (Suministro / Ingeniería / Construcción / Línea y pruebas) por códigos WBS, línea real en tres tramos (histórico cargado · informes diarios · estimación punteada), botón **Histórico real** para pegar la serie del informe interno, selector de alcance (Todo el proyecto / Solo construcción) y fecha de corte en hora Colombia. |
| 2026-09-16 | Creación del manual: índice, primeros pasos, Documentos (planificación documental) y capítulos iniciales de los demás módulos. |
