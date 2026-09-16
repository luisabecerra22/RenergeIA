# 13. Clima — Pronóstico y alertas meteorológicas operacionales

> **Para qué sirve:** fijar la ubicación exacta del proyecto en un mapa, consultar el clima actual y el pronóstico de 16 días, saber qué días son aptos para obra y recibir alertas de riesgo para las actividades del cronograma programadas en los próximos 3 días.
> **Quién lo usa:** residentes y coordinadores de obra, HSE, planeación, dirección de proyecto.
> **Dónde está:** menú lateral del proyecto → **Clima**, o la tarjeta **Clima** en el detalle del proyecto (`/proyectos/{id}/clima`).

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Ubicación climática** | País, departamento, municipio, dirección y, sobre todo, **latitud y longitud** del proyecto. Sin coordenadas no se puede consultar el clima. |
| **Confirmar ubicación** | Guardar la ubicación en el proyecto. Habilita la consulta del clima. |
| **Open-Meteo** | Servicio meteorológico gratuito (sin clave) del que la app toma el clima actual y el pronóstico. |
| **Condiciones actuales** | Temperatura, sensación térmica, humedad, viento y precipitación en el momento de la consulta. |
| **Apto obra** | Calificación de cada día del pronóstico según la probabilidad de lluvia. |
| **Alerta Meteorológica Operacional** | Cruce del pronóstico con las actividades del cronograma de los próximos 3 días. Marca riesgo **MEDIO** o **ALTO** con una recomendación. |
| **Historial climático** | Registro de cada consulta realizada (últimos 100). |

---

## Cómo funciona

### Lo que hace la app sola

- **Al hacer clic en el mapa** (o escribir latitud y longitud), la app pone el marcador y trata de **llenar solos** País, Departamento/Estado y Municipio/Ciudad a partir de las coordenadas (usa OpenStreetMap). Si no lo logra, no muestra error: puedes escribirlos a mano.
- **El clima no se consulta automáticamente**: solo cuando pulsas **Consultar clima y pronóstico**.
- **Cada consulta guarda un registro** en el Historial climático con fecha y hora, condición, temperatura, humedad, viento, precipitación y fuente *Open-Meteo*.
- **Apto obra** se calcula solo con la probabilidad máxima de lluvia del día:

| Resultado | Regla |
|---|---|
| **Sí** | Probabilidad de lluvia menor a 60 % |
| **Con precauciones** | Entre 60 % y 84 % |
| **No · Revisar actividades** | 85 % o más |

- En la tabla de 16 días, la probabilidad se pinta verde (< 30 %), amarilla (30–59 %) o roja (≥ 60 %); la precipitación mayor a 5 mm y el viento mayor a 40 km/h se resaltan.
- Los pronósticos a más de 7 días tienen mayor incertidumbre.

### Cómo se generan las alertas operacionales

1. La app toma las actividades del **cronograma vigente** que están activas, **no completadas** y cuyas fechas planeadas se cruzan con **hoy y los 2 días siguientes**.
2. Para cada día en que la actividad está programada, clasifica la actividad **por palabras de su nombre** y evalúa el pronóstico de ese día (probabilidad de lluvia, milímetros de lluvia, viento máximo y si hay tormenta eléctrica).
3. Solo se muestran los riesgos **MEDIO** y **ALTO**. Primero por fecha y, dentro de cada día, los ALTO arriba.
4. Si hay actividades programadas pero ningún riesgo, aparece **Sin alertas operacionales**. Si no hay actividades en esos 3 días, no aparece ninguno de los dos recuadros.
5. Las alertas **no se guardan**: se recalculan en cada consulta.

**Tipos de actividad que reconoce (por palabras en el nombre)**

| Tipo | Palabras que lo activan (ejemplos) | Riesgo ALTO cuando… |
|---|---|---|
| Vaciado de concreto | vaciado, fundición, concreto, hormigón | tormenta, lluvia ≥ 75 % o > 12 mm |
| Excavación | excavación, zanja, movimiento de tierra | tormenta, lluvia ≥ 70 % o > 15 mm |
| Vías | vía, sub-base, base granular, afirmado, compactación | tormenta o lluvia ≥ 80 % |
| Cimentación | cimentación, pedestal, zapata, dado, placa de | tormenta, lluvia ≥ 75 % o > 10 mm |
| Estructura | estructura (no eléctrica) | tormenta, viento > 45 km/h, o lluvia ≥ 75 % con viento > 30 |
| Malla a tierra | malla + tierra | tormenta |
| Tendido de cable | tendido, tirado, cable | tormenta |
| Conexionado | conexión, empalme, acometida | tormenta |
| Tableros | tablero, CCM/MCC, subestación, celda | tormenta |
| Pruebas eléctricas | prueba + eléctrica / megado / aislamiento / hi-pot | tormenta o lluvia ≥ 30 % |
| Izaje | izaje, grúa, maniobra | tormenta o viento > 40 km/h |
| Soldadura | soldadura, soldar | tormenta, lluvia ≥ 60 % o viento > 35 km/h |
| Montaje | montaje, instalación | tormenta o viento > 45 km/h |
| Pintura | pintura, recubrimiento, sandblast, galvanizado | tormenta, lluvia ≥ 40 % o > 2 mm |
| Commissioning | commissioning, arranque, puesta en marcha | tormenta o viento > 50 km/h |
| Liberación / inspección | liberación, inspección, punch | (solo llega a MEDIO: tormenta o lluvia ≥ 70 %) |

Cada tipo tiene además un umbral más bajo que genera riesgo **MEDIO** con su propia recomendación. Si el nombre no coincide con ningún tipo, la app evalúa por **disciplina** de la actividad (Civil, Eléctrica, Mecánica, Hot Commissioning); las demás disciplinas solo generan MEDIO si hay tormenta. Una actividad sin disciplina se trata como **General**.

> La clasificación es por palabras: el orden importa. Por ejemplo, una actividad llamada "Instalación de tableros" se reconoce como **Tableros**, porque esa regla se revisa antes que Montaje.

### Lo que se digita

- Solo la ubicación (o el clic en el mapa). Todo lo demás lo calcula la app.

---

## Paso a paso

### Fijar la ubicación del proyecto

1. Entra a **Clima** del proyecto.
2. En **Mapa interactivo**, navega hasta el sitio del proyecto y **haz clic** en el punto exacto. También puedes escribir **Latitud** (−90 a 90) y **Longitud** (−180 a 180) con punto decimal.
3. Espera a que termine *Detectando...* y revisa País, Departamento/Estado y Municipio/Ciudad; corrígelos si hace falta.
4. Completa **Dirección o referencia** (ej. kilómetro y vereda).
5. Verifica el recuadro verde **Marcador fijado en: …**.
6. Pulsa **Confirmar ubicación**. Debe aparecer *Ubicación climática confirmada correctamente.*

### Consultar el clima y el pronóstico

1. Con la ubicación confirmada, pulsa **Consultar clima y pronóstico** (si el botón está bloqueado, confirma primero la ubicación).
2. Espera el mensaje *Clima y pronóstico de 16 días obtenidos correctamente (hh:mm).*
3. Revisa, de arriba abajo:
   - **Condiciones actuales** (con la hora de actualización),
   - **Pronóstico — próximos 7 días** (máxima en rojo, mínima en azul, gota = probabilidad de lluvia),
   - **Pronóstico extendido — 16 días** con la columna **Apto obra**,
   - **Alerta Meteorológica Operacional** o **Sin alertas operacionales**.

### Actuar sobre una alerta operacional

1. En **Alerta Meteorológica Operacional**, ubica el día (**HOY**, **MAÑANA** o la fecha).
2. Lee cada actividad: disciplina, nombre, nivel (**Riesgo MEDIO** o **Riesgo ALTO**), responsable y código WBS.
3. Aplica la **Recomendación** o coordina la reprogramación con el responsable.
4. Si decides reprogramar, hazlo en el **Cronograma de Actividades** y vuelve a consultar el clima.

### Consultar el historial climático

1. Baja a **Historial climático del proyecto**.
2. Revisa las consultas guardadas (las más recientes arriba; se muestran hasta 100).

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| *Latitud y longitud son obligatorias. Haz clic en el mapa o escríbelas manualmente.* | Se pulsó **Confirmar ubicación** sin coordenadas. | Haz clic en el mapa o escribe ambas coordenadas. |
| El campo de latitud o longitud se pone en rojo (*Debe estar entre −90 y 90* / *−180 y 180*) | Valor fuera de rango, o latitud y longitud intercambiadas. | Corrige el valor. En Colombia la latitud es positiva (aprox. 0 a 13) y la longitud negativa (aprox. −67 a −79). |
| El botón **Consultar clima y pronóstico** está bloqueado | La ubicación no se ha confirmado. | Pulsa **Confirmar ubicación** primero. |
| *Open-Meteo respondió [código]. Intenta de nuevo en un momento.* | El servicio de clima no respondió bien. | Espera unos minutos y vuelve a consultar. |
| *Error al consultar Open-Meteo: …* | Falla de conexión o respuesta incompleta del servicio. | Verifica tu conexión y vuelve a intentar. |
| *Error al guardar: …* al confirmar la ubicación | Problema al guardar en la base de datos. | Intenta de nuevo; si persiste, repórtalo al administrador. |
| País/Departamento/Municipio no se llenan solos | El servicio de direcciones no respondió o no reconoce el punto (zona rural). | Escríbelos a mano; no afecta la consulta del clima. |
| No aparece ninguna alerta ni el recuadro *Sin alertas operacionales* | No hay actividades del cronograma vigente programadas para hoy y los 2 días siguientes, o todas están completadas. | Revisa las fechas planeadas en el **Cronograma de Actividades**. |
| Una actividad con riesgo evidente no genera alerta | Su nombre no tiene palabras reconocidas y su disciplina es general o no está asignada. | Asigna la **disciplina** correcta a la actividad en el cronograma. |
| El pronóstico no corresponde al sitio | Se movió el marcador pero no se confirmó, o la ubicación guardada está mal. | Vuelve a fijar el punto, pulsa **Confirmar ubicación** y consulta de nuevo. |

---

## Relación con otros módulos

- **Cronograma de Actividades (WBS)** ([2](02-cronograma-actividades.md)): las alertas operacionales usan las actividades del **cronograma vigente**, con sus fechas planeadas, disciplina, responsable y estado. Una buena disciplina y nombres claros mejoran las alertas.
- **Proyectos** ([1](01-proyectos.md)): la ubicación confirmada se guarda en los datos del proyecto (país, departamento, municipio, dirección y coordenadas).
- **Informe Diario** ([3](03-informe-diario.md)): no toma datos de esta pantalla; las novedades de clima del día se escriben como texto en las observaciones del informe.

> ⚠ Por confirmar: si el historial climático de esta pantalla debe alimentar el Informe Diario o algún reporte. Hoy solo se muestra aquí.

---

## Buenas prácticas

- Fija la ubicación **una sola vez al inicio del proyecto**, con el punto exacto del sitio (no el casco urbano del municipio).
- Consulta el clima **cada mañana** antes de la reunión de inicio de jornada, y la tarde anterior a vaciados, izajes o pruebas eléctricas.
- Toma las alertas **ALTO** como señal para suspender o reprogramar; las **MEDIO**, para preparar medidas (carpas, bombeo, plásticos, vigía de viento).
- Usa nombres de actividad descriptivos en el cronograma (ej. "Vaciado de pedestales", "Tendido de cable DC") y asigna la **disciplina**: así la app clasifica mejor el riesgo.
- Recuerda que **Apto obra** solo mira la probabilidad de lluvia: revisa también viento y tormenta en las alertas.
- Si cambias el marcador, **vuelve a confirmar** la ubicación antes de consultar.
