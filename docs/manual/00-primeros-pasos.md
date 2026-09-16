# 0. Primeros pasos

> **Para qué sirve:** entender qué es RenergeIA, cómo entrar, cómo moverse y en qué orden se alimenta un proyecto desde que se gana hasta que se entrega.
> **Quién lo usa:** todas las personas que usan la plataforma por primera vez.
> **Dónde está:** https://renergeia-web-577313322290.us-central1.run.app

---

## Qué es RenergeIA

RenergeIA es la plataforma de Renergeia para gestionar proyectos solares fotovoltaicos tipo **EPC** (ingeniería, compras y construcción). Reúne en un solo lugar:

- **Planeación:** cronograma de actividades y histogramas de recursos.
- **Ejecución:** informe diario de avance, clima y restricciones.
- **Documentos:** planificación documental con alertas por días sin atender.
- **Costos:** presupuesto, órdenes de compra, flujo de caja y comparación contra la oferta (BOM).
- **HSEQ:** seguridad y salud en el trabajo, calidad, ambiental, social y auditorías.
- **Control de ingreso:** documentos de personas, equipos y proveedores con vencimiento.

La plataforma de **Evaluaciones HSE** es una aplicación aparte (capítulo 15).

---

## Entrar y moverse

1. Abre la dirección de la aplicación en Chrome o Edge.
2. Escribe tu **correo** y **contraseña** y haz clic en **Ingresar** (detalle en el capítulo 1).
3. El **menú lateral** tiene el Inicio (Centro de Control del portafolio), la lista de **Proyectos** y, dentro de cada proyecto, sus módulos: Dashboard, Informe Diario, Cronograma de Actividades, Documentos, Costos, HSEQ, No Conformidades, Restricciones, Histogramas, Clima y Alertas.
4. Desde el **detalle del proyecto** también encuentras accesos directos a cada módulo.

> Si al iniciar sesión aparece **HTTP 400**, borra las cookies del sitio o usa una ventana de incógnito (capítulo 16).

---

## Ciclo de vida de un proyecto en RenergeIA (orden recomendado)

| Paso | Qué hacer | Archivo fuente | Capítulo |
|---|---|---|---|
| 1 | **Crear el proyecto** cargando la BOM de la oferta | BOM (Excel de oferta) | 1 |
| 2 | **Cargar el cronograma**: plantilla EPC, MS Project (.mpp), Excel o PDF | Cronograma | 2 |
| 3 | **Cargar la codificación y el presupuesto** de costos | Codificación / presupuesto | 5 |
| 4 | **Cargar la planificación documental** y asignar responsables por área | FO-SI-GC-002-1 | 4 |
| 5 | **Configurar HSEQ**: plan de trabajo HSE, matriz IPERV, capacitaciones | — | 7 a 11 |
| 6 | **Diario:** registrar el informe diario de avance | — | 3 |
| 7 | **Semanal:** importar el archivo de tesorería (Forecast Control) y actualizar la planificación documental | Tesorería / planificación | 5 y 4 |
| 8 | **Semanal:** revisar alertas (documentos en rojo, vencimientos de control de ingreso, clima) | — | 4, 12 y 13 |
| 9 | **Semanal / mensual:** revisar el Dashboard, histogramas y generar el consolidado de costos | — | 14, 6 y 5 |
| 10 | **Cierre:** Redline / As-Built validados, auditorías y consolidado final | — | 4 y 11 |

---

## Reglas generales que conviene saber

- **Muchos datos no se digitan: se calculan** a partir de archivos que se importan (tesorería, BOM, planificación documental, cronograma). Si un número no cuadra, revisa primero el archivo fuente y vuelve a importarlo.
- **Las importaciones no duplican:** actualizan lo existente y avisan si el archivo es más antiguo que el último cargado.
- **Lo que se borra de la papelera de proyectos no se recupera.**
- **Los cambios de otros usuarios** se ven al recargar la página (F5).
- **Hora de referencia:** Colombia (UTC−5) para cálculos de días y semanas.

> ⚠ Por confirmar: hoy la plataforma no restringe acciones por rol; cualquier usuario con sesión puede crear, editar o eliminar información. Definir los permisos por rol antes de ampliar el número de usuarios.
