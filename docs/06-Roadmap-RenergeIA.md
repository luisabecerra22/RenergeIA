# Roadmap de Desarrollo — RenergeIA
**Versión:** 1.0 | **Fecha:** 2026-06-28 | **Estado:** Vigente

> Este documento define el plan de desarrollo de RenergeIA por fases. Cada fase tiene objetivos claros, entregables verificables y criterios de éxito. No se avanza a la siguiente fase hasta completar la actual.

---

## Visión del producto

RenergeIA debe convertirse en el sistema operativo digital de Renergeia S.A.S. para la gestión de proyectos EPC fotovoltaicos: desde la planificación hasta el cierre, pasando por costos, HSEQ, documentos, personas y equipos, todo en un solo lugar, con inteligencia artificial integrada.

---

## Resumen ejecutivo de fases

| Fase | Nombre | Estado | Período estimado |
|---|---|---|---|
| 1 | Fundamentos operativos | ✅ En progreso avanzado | Meses 1–4 |
| 2 | Design System y componentes | 🔜 Próxima | Mes 5 |
| 3 | Estandarización de dashboards | 🔜 Pendiente | Mes 5–6 |
| 4 | Funciones críticas pendientes | 🔜 Pendiente | Mes 6–7 |
| 5 | IA y análisis inteligente | 🔜 Pendiente | Mes 8–10 |
| 6 | Integraciones externas y Azure | 🔜 Pendiente | Mes 10–14 |

---

## Fase 1 — Fundamentos operativos

**Objetivo:** Tener una plataforma funcional con todos los módulos base que permitan al equipo de obra operar el día a día.

**Estado:** ✅ Completado en su mayoría

### Módulos completados en Fase 1

| Módulo | Estado |
|---|---|
| Autenticación (Login / Logout) | ✅ |
| Proyectos (CRUD completo) | ✅ |
| WBS / Cronograma (con versiones) | ✅ |
| Informe Diario | ✅ |
| Documentos | ✅ |
| Dashboard del proyecto | ✅ |
| Costos y Partidas | ✅ |
| Histogramas | ✅ |
| Personal | ✅ |
| Equipos | ✅ |
| No Conformidades | ✅ |
| Restricciones | ✅ |
| Clima (Leaflet + Open-Meteo) | ✅ |
| HSEQ Seguridad (14 pantallas) | ✅ |
| HSEQ Calidad (7 pantallas) | ✅ |
| HSEQ Ambiental (8 pantallas) | ✅ |
| HSEQ Social (8 pantallas) | ✅ |
| GitHub (control de versiones) | ✅ |

### Pendientes de Fase 1 (críticos)

| Tarea | Prioridad | Por qué es importante |
|---|---|---|
| CRUD de usuarios y roles | 🔴 Alta | Sin esto, no se puede gestionar acceso multi-usuario |
| Carga física de archivos | 🔴 Alta | El módulo de Documentos necesita subir archivos reales |
| Generación de PDF (QuestPDF) | 🟡 Media | Informes diarios y reportes formales |
| Alertas automáticas | 🟡 Media | 20 alertas definidas en la documentación |
| Comparativo versiones WBS | 🟡 Media | Análisis de desviaciones del cronograma |

---

## Fase 2 — Design System y componentes reutilizables

**Objetivo:** Crear la biblioteca de componentes estándar de RenergeIA para que todos los módulos futuros sean coherentes.

**Estado:** 🔜 Próxima fase

### Entregables

| Entregable | Descripción |
|---|---|
| `PageHeader.razor` | Cabecera estándar de todas las páginas |
| `FilterBar.razor` | Barra de filtros de nivel 1 |
| `StatusChip.razor` | Badge de estado reutilizable |
| `AlertCard.razor` | Tarjeta de alerta estándar |
| `EmptyState.razor` | Estado vacío estándar |
| `LoadingState.razor` | Indicador de carga estándar |
| `ErrorState.razor` | Pantalla de error estándar |
| `SectionTitle.razor` | Separador de sección liviano |
| `SmartTable.razor` | Tabla con búsqueda y paginación |
| `RankingCard.razor` | Tarjeta de Top-N |
| Documentación CSS `app.css` | Clases para los nuevos componentes |

### Criterio de éxito

- Todos los componentes tienen parámetros documentados en `03-Componentes-Reutilizables.md`.
- Todos los componentes funcionan sin modificar páginas existentes.
- Los nuevos componentes siguen el prefijo `rn-` en sus clases CSS.

---

## Fase 3 — Estandarización de dashboards existentes

**Objetivo:** Aplicar la estructura de 5 niveles (04-Reglas-Dashboards.md) a los dashboards que ya existen en la plataforma.

**Estado:** 🔜 Pendiente (requiere Fase 2 completada)

### Dashboards a estandarizar

| Dashboard | Módulo | Trabajo estimado |
|---|---|---|
| Dashboard del Proyecto | `DashboardProyecto.razor` | Revisión y ajuste de estructura |
| Dashboard HSEQ General | `HSEQDashboard.razor` | Revisión y ajuste |
| Dashboard Seguridad | `SeguridadDashboard.razor` | Revisión y ajuste |
| Dashboard Calidad | `CalidadDashboard.razor` | Revisión y ajuste |
| Dashboard Ambiental | `AmbientalDashboard.razor` | Revisión y ajuste |
| Dashboard Social | `SocialDashboard.razor` | Revisión y ajuste |
| Home (Dashboard General) | `Home.razor` | Rediseño con tarjetas de proyecto |

### Criterio de éxito

- Todos los dashboards pasan el checklist de `04-Reglas-Dashboards.md`.
- Todos los dashboards usan `<GaugeCircular />`, `<TarjetaKPI />`, `<ChartCard />`, `<SeccionDash />`, `<AnalisisIA />`.
- Ningún módulo queda con HTML repetido que debería ser un componente.

---

## Fase 4 — Funciones críticas pendientes

**Objetivo:** Completar las funciones que hacen que la plataforma sea realmente utilizable en producción.

**Estado:** 🔜 Pendiente

### Módulo de Usuarios y Roles

- CRUD completo de usuarios (crear, editar, desactivar)
- Asignación de roles por proyecto
- Pantalla de gestión de permisos
- Uso de ASP.NET Core Identity (ya instalado, pendiente de exponer en UI)

### Carga de archivos físicos

- Upload de documentos al módulo de Documentos
- Almacenamiento local en `/uploads/` para Fase 1
- Preparación para migrar a Azure Blob Storage en Fase 6
- Soporte para: PDF, DOCX, XLSX, imágenes, XML

### Generación de PDF (QuestPDF)

Documentos a generar en PDF:
- Informe Diario (formato ejecutivo)
- Reporte de Costos del período
- Acta de inspección SST
- Certificado de capacitación
- Reporte HSEQ mensual

### Alertas automáticas

20 alertas definidas en la documentación v1.0. Las 5 más críticas para Fase 4:
1. Actividad del cronograma con más de 3 días de atraso
2. Costo ejecutado supera el 95% del presupuesto de la partida
3. Informe diario no registrado en las últimas 24 horas
4. Incidente de seguridad sin acción correctiva asignada
5. Documento con fecha de vigencia vencida

### Comparativo de versiones WBS

- Gráfico de comparación entre línea base y versión actual
- Tabla de desviaciones por actividad
- Indicador de SPI (Schedule Performance Index)

---

## Fase 5 — IA y análisis inteligente

**Objetivo:** Conectar el panel `<AnalisisIA />` con Azure OpenAI para generar análisis reales a partir de los datos de la plataforma.

**Estado:** 🔜 Pendiente (requiere Fases 1–4 completadas)

### Stack de IA

- **Modelo:** GPT-4o via Azure OpenAI (región East US 2)
- **Temperatura:** 0.3 (respuestas precisas)
- **Tokens máx:** 600 por solicitud
- **Autenticación:** Azure Managed Identity (sin keys hardcodeadas)

### Módulos con análisis IA

En orden de implementación:

1. Costos — detección de desviaciones y proyección de cierre
2. WBS — predicción de retraso basada en tendencia
3. HSEQ Seguridad — análisis de incidentes y tendencias
4. HSEQ Calidad — análisis de no conformidades
5. Dashboard General — resumen ejecutivo multi-proyecto

### Funciones de IA planificadas

| Función | Descripción |
|---|---|
| Análisis de período | Resumen automático del mes/semana |
| Detección de anomalías | Identifica datos atípicos vs. histórico |
| Predicción de cierre | Proyecta fecha de terminación del proyecto |
| Recomendaciones | Acciones sugeridas basadas en el estado |
| Alertas inteligentes | Alertas generadas por patrones, no solo umbrales |

---

## Fase 6 — Integraciones externas y Azure

**Objetivo:** Conectar RenergeIA con sistemas externos y migrar a infraestructura en la nube.

**Estado:** 🔜 Pendiente (requiere Fases 1–5 completadas)

### Integraciones planificadas

| Integración | Para qué | Estado |
|---|---|---|
| Azure SQL Database | Migración de SQL Server local a nube | Pendiente Fase 6 |
| Azure Blob Storage | Archivos de documentos en la nube | Pendiente Fase 6 |
| Open-Meteo API | Clima en tiempo real | ✅ Ya implementado |
| SharePoint | Sincronización de documentos | Pendiente Fase 6 |
| Correo electrónico | Notificaciones automáticas de alertas | Pendiente Fase 6 |
| WhatsApp Business API | Alertas críticas por WhatsApp | Pendiente Fase 6 |
| Excel (exportación) | Exportar cualquier tabla a Excel | ✅ ClosedXML instalado |
| PDF (exportación) | Generar reportes formales | Pendiente Fase 4 |

### Migración a Azure

1. Crear Azure SQL Database (misma estructura, migración EF Core)
2. Mover archivos locales a Azure Blob Storage
3. Configurar Azure App Service (despliegue de la app Blazor)
4. Configurar Azure OpenAI (para módulo IA)
5. Configurar dominio personalizado y SSL

---

## Criterios de cierre del proyecto (v1.0 completa)

Para considerar que RenergeIA v1.0 está completo, deben cumplirse:

- [ ] Todas las fases 1–4 completadas
- [ ] 18 módulos funcionales con datos reales
- [ ] 13 roles RBAC configurados y asignados
- [ ] Al menos 10 de las 20 alertas automáticas activas
- [ ] Generación de PDF para informe diario y reporte de costos
- [ ] Al menos 1 proyecto real gestionado completamente en la plataforma
- [ ] Manual de usuario básico escrito

---

## Historial de versiones del roadmap

| Versión | Fecha | Cambios |
|---|---|---|
| 1.0 | 2026-06-28 | Versión inicial — creada al completar Fase 1 en progreso avanzado |

---

*Documento generado: 2026-06-28. Revisar y actualizar al inicio de cada fase.*
