# 6. Histogramas — Personal y equipos planificado vs real

> **Para qué sirve:** comparar, mes a mes, cuánto personal (por cargo) y cuántos equipos se planearon para el proyecto contra lo que realmente hubo en obra. El personal real **no se digita**: sale de la nómina del archivo de tesorería. El planificado se importa del BOM (hoja H PER) o se digita.
> **Quién lo usa:** dirección de proyecto, control de proyectos / planeación, administración de obra, costos.
> **Dónde está:** menú lateral del proyecto → **Histogramas**, o la tarjeta **Histogramas** en el detalle del proyecto (`/proyectos/{id}/histogramas`).

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Personal / Equipos** | Las dos pestañas principales. Cada una tiene tres sub-pestañas: **Planificado**, **Real** y **Comparativo**. |
| **Mes inicial / Año inicial** | El primer mes de la ventana que se ve en pantalla. La pantalla siempre muestra **12 meses** desde ahí (ej. *Marzo 2026 → Febrero 2027*). |
| **Plantilla de referencia** | Histograma tipo que la app propone según la potencia del proyecto: hasta 5 MW → **1 MW**; hasta 12,5 MW → **10 MW**; hasta 17,5 MW → **15 MW**; más de 17,5 MW → **20 MW**. Aparece junto al título. |
| **Personal Real** | Personas **distintas** por cargo y por mes según la nómina (Salarios) del archivo de tesorería. |
| **Personal Planificado** | Personas por cargo y por mes según la oferta (BOM, hoja **H PER**) y/o lo digitado a mano. |
| **Máximo semanal** | Al importar del BOM, cada mes toma el **pico** de las semanas de ese mes (no el promedio ni la suma). |
| **Ajuste manual (naranja)** | Celda del Personal Real que alguien cambió y quedó distinta a lo que dice la nómina. Se conserva al volver a importar tesorería. |
| **Fila manual** | Fila del Personal Real que no viene de la nómina (por ejemplo, personal de un contratista). |
| **Personas-mes / equipos-mes** | Suma de los 12 meses de la ventana. Se usa en las tarjetas del Comparativo. |
| **Mes actual** | El mes de hoy se resalta en verde en las tarjetas y en el encabezado de las tablas. |

---

## Cómo funciona

### Personal Real — lo calcula la app sola

1. La fuente es la **nómina** que llega con el archivo de tesorería (Costos → Compromisos → Salarios). Se recalcula **cada vez que se importa tesorería** y **cada vez que se abre Histogramas**.
2. **El mes sale del periodo de pago** (ej. *Mar-2026*). La **I y la II quincena del mismo mes son un solo mes**: quien cobró en ambas cuenta **1**.
3. **No cuentan** como personal en obra: primas, seguridad social (planillas) ni liquidaciones.
4. **Una persona con dos cargos en el mes** cuenta en cada cargo, pero **una sola vez** en la fila **TOTAL PERSONAS**.
5. Si la suma de pagos de una persona en el mes es cero o negativa (pagos rebotados o reversados), **no cuenta**.
6. Se descartan "cargos" que no son cargos: vacíos, *Sin cargo*, errores tipo *#N/A*, *Fecha de Retiro*, encabezados *Cargo…* y textos con números (como fechas). También se descartan filas que no son personas (*Planilla…*, *Liquidaciones*, *XX Personas*).
7. Los nombres se comparan sin tildes ni mayúsculas y sin lo que venga después de " - " (ej. "— PAGO REBOTADO").
8. **Nombres parecidos** (por ejemplo, con una letra distinta o uno más corto que el otro) **no se unen solos**: la app pregunta si son la misma persona. Mientras no se responda, cuentan como personas distintas. La respuesta queda recordada.

> ⚠ Por confirmar: la regla que descarta cargos o nombres con números (fechas como "02-Jul-2026") podría no estar filtrando en todos los casos. Si ves un "cargo" que es una fecha o un número, repórtalo.

### Personal Planificado — se importa o se digita

- **Importar desde BOM (H PER):** la app busca la fila de fechas semanales de la hoja **H PER**, toma el tipo (columna C) y el cargo (columna D) y calcula el **máximo semanal de cada mes**. Si varios cargos del BOM se asignan al mismo cargo de la nómina, **se suman semana a semana** antes de sacar el máximo. Los valores con decimales se redondean hacia arriba.
- La importación **reemplaza todo el personal planificado** del proyecto.
- Las **equivalencias** "cargo del BOM → cargo de la nómina" quedan recordadas para la próxima importación.
- Después de importar, la ventana de 12 meses se ubica automáticamente en el **primer mes planificado**.
- Los **cargos nuevos que aparecen en la nómina** y no están en el planificado se muestran en el Planificado con **0**, para que los completes.

### Personal: los datos están amarrados al calendario

El personal se guarda por **mes calendario** (ej. *junio 2026*), sin límite de meses. Cambiar el mes/año inicial solo mueve la ventana que ves: los valores no se corren. Guardar solo afecta los 12 meses visibles; lo que está fuera de la ventana se conserva.

### Equipos: 12 columnas fijas

Los equipos siguen usando **12 posiciones fijas** (Mes 1 a Mes 12) contadas desde el mes inicial. Si cambias el mes inicial, **las cantidades se quedan en su posición** y solo cambian los nombres de los meses. En Equipos, tanto el Planificado como el Real **se digitan**.

### Qué calcula la app en cada sub-pestaña

- **Planificado:** tarjetas con el total por mes, gráfica de barras y el indicador **Personal requerido / Equipos requeridos a corte de hoy** (según el planificado). Si hoy es anterior a la ventana muestra *Histograma no iniciado*; si es posterior, *Histograma finalizado*.
- **Comparativo:** Total Planificado, Total Real, **Diferencia Acumulada** (real − planificado), **% Desviación Global**, **Mes Mayor Desviación**, gráfica Planificado vs Real, gráfica de % de desviación por mes y tabla de detalle mensual. En Personal agrega el **Comparativo por cargo** (Real / Planificado por mes).
- **Colores:** rojo = más recurso que lo planificado (sobreuso); verde = menos (ahorro).
- En Personal, el total real del mes usa **personas distintas** (no la suma de cargos), más los ajustes manuales.
- Si un mes tiene planificado 0 y real mayor a 0, su desviación se muestra como **100 %**.

---

## Paso a paso

### Elegir el período que quiero ver

1. Entra a **Histogramas** del proyecto.
2. En **Mes inicial** y **Año inicial** selecciona el primer mes de la ventana.
3. Verifica el texto **Período: … → …** a la derecha. El cambio se guarda solo.

### Cargar o recargar la plantilla por capacidad

1. Pulsa **↻ Recargar plantilla [X MW]** (arriba a la derecha), o **Cargar plantilla [X MW]** si la tabla está vacía.
2. La app reemplaza **el personal planificado y los equipos planificados** con la plantilla de referencia, ubicada desde el mes inicial actual.
3. Aparece el mensaje *Plantilla X MW cargada exitosamente.*

> Atención: recargar la plantilla **borra** el personal planificado importado del BOM y los equipos planificados que hayas digitado.

### Importar el personal planificado desde el BOM

1. Ve a **Personal → Planificado**.
2. Pulsa **Importar desde BOM (H PER)** y elige el Excel del BOM (`.xlsx` o `.xlsm`).
3. En la ventana **Importar personal planificado desde el BOM** revisa la lista de cargos (con **Semanas** con personal y **Pico**).
4. En **Semana 1 inicia el** confirma la fecha de inicio (la app propone la *Fecha inicio* del BOM). Si la cambias, todo el histograma se corre esas semanas.
5. En la columna **Cargo en la nómina** escribe o elige el cargo equivalente de la nómina. Si lo dejas vacío, se conserva el nombre del BOM.
6. Pulsa **Importar**. (Con **Cancelar** no se cambia nada.)
7. Revisa la tabla y, si hace falta, ajusta a mano y pulsa **Guardar cambios**.

### Ajustar el personal planificado a mano

1. En **Personal → Planificado**, edita las cantidades en la tabla **Distribución mensual — Personal Planificado**.
2. Para cambiar el nombre de un cargo, edítalo en la primera columna (usa el mismo nombre de la nómina para que el comparativo cruce).
3. Para agregar un cargo, pulsa **+ Agregar fila**; para quitarlo, pulsa **✕**.
4. Pulsa **Guardar cambios**. Nada se guarda hasta ese momento.

### Revisar el personal real

1. Ve a **Personal → Real**. Los datos ya están calculados desde la nómina; la franja azul indica la **Última nómina** cargada.
2. Si aparece el aviso **posible(s) nombre(s) repetido(s)**, resuélvelo (ver la tarea siguiente).
3. Si necesitas corregir una celda, cambia el número y pulsa **Guardar cambios**. La celda queda en **naranja** (al pasar el mouse muestra el valor según nómina).
4. Para volver una celda a lo que dice la nómina, escribe el mismo valor de la nómina y guarda: deja de ser ajuste manual.
5. Para personal que no pasa por nómina, pulsa **+ Agregar fila manual**, escribe el cargo y las cantidades, y guarda. Solo las filas manuales se pueden quitar con **✕**.

### Confirmar si dos nombres parecidos son la misma persona

1. En **Personal → Real**, en el recuadro naranja, revisa cada par de nombres (debajo de cada nombre se ven sus cargos).
2. Pulsa **Sí, unificar** si son la misma persona, o **No, son distintas** si no lo son.
3. La app recalcula y la decisión queda guardada; ese par no vuelve a preguntarse.

### Ir al período donde hay nómina

1. Si ves el aviso amarillo *La nómina tiene datos de … fuera del período que estás viendo*, pulsa **Ver ese período**.
2. La ventana se mueve al primer mes con nómina.

### Registrar equipos (planificado y real)

1. Ve a la pestaña **Equipos**.
2. En **Planificado**, edita las cantidades, usa **+ Agregar fila** o **✕** y pulsa **Guardar cambios**.
3. En **Real**, pulsa **Copiar nombres del planificado** para traer la lista de equipos, digita las cantidades reales (y **Observaciones** si aplica) y pulsa **Guardar cambios**.

### Analizar el comparativo

1. Abre la sub-pestaña **Comparativo** (en Personal o en Equipos).
2. Revisa las cinco tarjetas de resumen, las dos gráficas y la tabla **Detalle mensual**.
3. En Personal, baja a **Comparativo por cargo**: cada celda muestra **Real / Planificado**; pasa el mouse para ver ambos valores.

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| Personal Real dice *No hay nómina cargada* | No se ha importado el archivo de tesorería, o no trae la sección Salarios. | Importa el archivo en **Costos → Compromisos** y vuelve a abrir Histogramas. |
| Aviso amarillo *La nómina tiene datos de … fuera del período* | La ventana de 12 meses no incluye ningún mes con nómina. | Pulsa **Ver ese período** o cambia el mes/año inicial. |
| Al importar el BOM: *El archivo no tiene la hoja "H PER"* | El Excel no es el BOM o la hoja tiene otro nombre. | Usa el BOM de la oferta y verifica que la hoja se llame **H PER**. |
| *No se encontró la fila de fechas semanales en la hoja H PER* | Las fechas no están en las primeras 30 filas desde la columna G, o no tienen formato de fecha. | Revisa en el Excel que las semanas sean fechas reales (no texto) y vuelve a importar. |
| *La hoja H PER no tiene cargos con personal planificado* | Las filas no tienen tipo (col. C), cargo (col. D) o cantidades. | Revisa el contenido de la hoja H PER. |
| *No se pudo leer el BOM: …* | Archivo dañado, protegido, en formato no soportado o mayor a 60 MB. | Guarda una copia como `.xlsx` sin contraseña e intenta de nuevo. |
| Un mismo cargo aparece dos veces en el Comparativo por cargo (uno solo con planificado y otro solo con real) | El cargo planificado tiene un nombre distinto al de la nómina. | Renombra el cargo en el Planificado igual a la nómina y guarda, o reimporta el BOM asignando el **Cargo en la nómina**. |
| El total de personas del mes es menor que la suma de los cargos | Hay personas con dos cargos en el mes: cuentan una sola vez en el total. | Es correcto; no requiere acción. |
| Una persona parece contada dos veces | Su nombre viene escrito distinto en la nómina. | Resuelve el par en el recuadro **posible(s) nombre(s) repetido(s)** con **Sí, unificar**. Si no aparece como parecido, repórtalo. |
| Después de **Recargar plantilla** desapareció el planificado del BOM | La plantilla reemplaza todo el personal y los equipos planificados. | Vuelve a **Importar desde BOM (H PER)**. |
| En Equipos, al cambiar el mes inicial las cantidades quedaron en meses equivocados | Equipos usa 12 posiciones fijas; los valores no se mueven con el calendario. | Vuelve a poner el mes inicial anterior o reubica los valores y guarda. |

---

## Relación con otros módulos

- **Costos → Compromisos (tesorería):** la sección **Salarios** del archivo de tesorería es la fuente del Personal Real. Cada importación recalcula el histograma. Ver [5. Costos](05-costos.md).
- **Costos → BOM:** la hoja **H PER** del mismo BOM de la oferta alimenta el Personal Planificado.
- **Proyectos:** la **potencia (kWp)** del proyecto define la plantilla de referencia. Al crear un proyecto se carga su plantilla automáticamente. Ver [1. Proyectos](01-proyectos.md).

> ⚠ Por confirmar: si se cambia la potencia en **Editar proyecto**, la app actualiza la plantilla de equipos, pero el personal planificado de un proyecto que ya se abrió en Histogramas podría no cambiar. En ese caso usa **Recargar plantilla**.

---

## Buenas prácticas

- **Nunca digites el personal real a mano** si viene en la nómina: importa la tesorería y deja que la app lo calcule. Usa ajustes solo para excepciones justificadas.
- Usa **los mismos nombres de cargo de la nómina** en el planificado; así el comparativo por cargo cruza bien.
- Al importar el BOM, **completa la columna Cargo en la nómina** la primera vez; las siguientes importaciones la recordarán.
- Resuelve los **nombres repetidos** apenas aparezcan, para que el total de personas sea confiable.
- Registra en filas manuales al personal de contratistas que no pasa por nómina, con un nombre de cargo claro.
- Antes de **Recargar plantilla**, confirma que no vas a perder un planificado importado del BOM.
- Pulsa **Guardar cambios** antes de cambiar entre **Personal** y **Equipos**, cambiar el mes inicial o salir de la página: lo digitado sin guardar se pierde.
