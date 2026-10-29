# 5. Costos

> **Para qué sirve:** controlar el dinero del proyecto de principio a fin: cuánto se presupuestó por rubro, cuánto se ha pagado (ejecutado), cuánto está por pagar (comprometido), qué órdenes de compra hay con sus facturas, cómo se compara el presupuesto base de la oferta con lo real y el informe semanal para gerencia. Casi todo se alimenta de **un solo archivo**: el **Seguimiento de pagos** semanal.
> **Quién lo usa:** control de costos, directores de proyecto, gerencia.
> **Dónde está:** menú lateral → **Proyectos** → *(nombre del proyecto)* → **Costos**, o la tarjeta **Costos** en el detalle del proyecto (`/proyectos/{id}/costos`).

El módulo tiene 5 pestañas en la parte superior:

| Pestaña | Qué contiene |
|---|---|
| **Presupuesto** | Resumen del presupuesto, Costos por Categoría y, dentro de la tabla, dos vistas: **Actividades Presupuestadas** y **Flujo de Caja** (los pagos semanales). |
| **Compromisos** | Dos vistas: **Resumen de Órdenes de Compra** (con la sección de Salarios al final) y **Proyectado** (costos futuros sin orden de compra). |
| **Dashboard** | Indicadores financieros y 4 gráficas. |
| **Presupuesto base vs Real** | Comparación línea a línea entre el presupuesto base de la oferta y lo realmente comprado/ejecutado. |
| **Consolidado** | Informes semanales de costos para gerencia, con análisis de IA e impresión en PDF. |

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Disciplina / Categoría** | Agrupación grande del presupuesto (ej. Suministros principales, Trabajos civiles, Instalación mecánica, Instalación eléctrica, Costos generales, Estudios). En pantalla aparece como fila gris con el total de sus rubros. |
| **Rubro (código de presupuesto)** | Código de 4 letras (ej. `SPSP`, `CGPR`, `CGHS`, `PASP`) con el que se presupuesta. Es el "idioma común" entre Presupuesto, Flujo de Caja, Compromisos y Presupuesto base. Un mismo rubro puede existir en COP y en USD. |
| **Subcódigo de pagos** | Código más detallado que trae el archivo de Seguimiento de pagos (ej. `CGAC`, `HSDO`, `CGSTM`). La app lo traduce a su rubro con la tabla de **Codificación** (ej. `CGAC` → `CGPR`). |
| **Equivalencia guardada** | Traducción subcódigo de pagos → rubro que usted define a mano cuando la app no la encuentra. Queda guardada y se aplica sola en las siguientes importaciones. |
| **Seguimiento de pagos (archivo semanal)** | Excel semanal de pagos del proyecto. Trae los egresos por código y semana (para el Flujo de Caja) y las órdenes de compra con sus facturas (para Compromisos). Su nombre debe incluir la fecha de corte en formato `MM.DD.AAAA`. |
| **OC (Orden de Compra)** | Compromiso con un proveedor. Número con formato `CO_AAAA_NNN` (ej. `CO_2026_150`). Tiene **valor aprobado**, **facturado**, **pagado** y **saldo por pagar**. |
| **OC sin consecutivo (XXX)** | OC que aún no tiene número real (`CO_2026_XXX`). Se muestra **En proceso**. Su valor se considera **presupuestado, no comprometido**, hasta que usted le asigne el número. |
| **Hito** | Cada factura o documento dentro de una OC (factura F#, cuenta de cobro CC#, póliza POL#…), con fecha, subtotal, IVA, importe, retenciones y total a pagar. |
| **Pagado (verde)** | Una factura está pagada cuando en el archivo de Seguimiento de pagos la celda **Total a pagar** está rellena de **verde**. La app lo detecta sola. |
| **OC sobrepasada** | OC cuyo facturado supera el aprobado en más de **$1.000 COP** o **US$ 1**. Diferencias menores se consideran redondeo y no alertan. |
| **Ejecutado** | Lo ya pagado: suma del Flujo de Caja del código **desde el inicio hasta la semana anterior** a la actual (hora Colombia). No se digita. |
| **Comprometido** | Lo que falta pagar según el Flujo de Caja: pagos del código **desde la semana en curso en adelante**, descontando las OC sin consecutivo (XXX). No se digita. |
| **Pendiente por ejecutar** | Presupuesto − Ejecutado − Comprometido. Incluye el valor de las OC sin consecutivo (XXX). |
| **Proyectado** | Costos futuros que el área de pagos proyecta y que todavía no tienen orden de compra (sección PROYECTADO del archivo). |
| **Salarios** | Nómina del proyecto por quincena, agrupada por **cargo** y **código de rubro**. Se muestra **sin nombres de personas**, solo el número de personas. |
| **Código sugerido** | En salarios, cuando una línea no trae código, la app propone el código más usado por ese cargo. **No cuenta** hasta que usted lo confirme con **✓**. |
| **TRM** | Tasa de cambio COP/USD del proyecto. Se usa para convertir cuando se ven montos "en COP" o "en USD". Se edita en la pestaña Presupuesto (campo **TRM $**). En el Consolidado se usa la TRM de cierre de cada informe. |
| **Corte de pagos** | Copia de los pagos por semana del Flujo de Caja que la app guarda cada vez que se actualiza el Seguimiento de pagos, con la fecha de corte del archivo. El Consolidado compara dos cortes para calcular la Variación temporal. |
| **Presupuesto base** | El Excel de la oferta con la que se cerró el negocio (cantidades y costos por concepto). Su columna **Check** es el código de rubro. Se compara por **COSTO** (no por precio de venta). |

---

## Cómo funciona

### De dónde salen los datos

| Dato | Fuente | ¿Se digita? |
|---|---|---|
| Estructura del presupuesto (disciplinas y rubros) y valor presupuestado | Archivo **Codificación** (hojas COD COP / COD USD / PPTO COP / PPTO USD), la **plantilla EPC** o un Excel con columnas *Código* y *Presupuesto* | Se importa; el valor por rubro también se puede corregir a mano |
| Pagos semanales (Flujo de Caja) | Archivo de **Seguimiento de pagos** | Se importa; se puede ajustar a mano (ver advertencia más abajo) |
| **Ejecutado** y **Comprometido** de cada rubro | Calculados desde el Flujo de Caja | **No** |
| Órdenes de compra, facturas, pagado, salarios, proyectado | Archivo de **Seguimiento de pagos** | No (solo se asignan números de OC y códigos faltantes). Se pueden crear OC manuales |
| Líneas del presupuesto base | Archivo del **presupuesto base** de la oferta (hojas General, Sum PPAL, Rend MAT) | Se importa |
| Cantidad real y valor real del presupuesto base | Usuario | **Sí** |
| Consolidado semanal | Totales desde el Presupuesto; pagos de la Variación temporal desde los cortes de pagos guardados; TRM automática. Se digitan las justificaciones, los ingresos semanales y el responsable | Parcialmente |

### Qué calcula la app sola

- **Ejecutado y Comprometido por rubro:** cada vez que se actualiza el Seguimiento de pagos, se guarda el Flujo de Caja o se abre el Presupuesto. Avanzan solos con el calendario: al empezar una semana nueva, los pagos de la semana pasada dejan de ser "comprometido" y pasan a "ejecutado".
- **Regla de monedas:** los pagos se agrupan por código y moneda. Si el rubro existe en COP **y** en USD, los pagos en pesos van a la línea COP y los de dólares a la línea USD. Si el rubro existe en una sola moneda, los pagos de la otra moneda se convierten con la TRM del proyecto.
- **Traducción de subcódigos de pagos a rubro**, en este orden: (1) equivalencia guardada por usted, (2) tabla de Codificación (solo si ese rubro existe en el presupuesto), (3) el mismo código. Si no encuentra destino, el código queda como **subcódigo sin equivalencia** y **no entra** al Flujo de Caja hasta que usted lo clasifique.
- **Estado de cada OC:** En proceso (sin consecutivo XXX), Pendiente o Pagada; alerta de **Sobrepasada**.
- **Alerta de costo por rubro y categoría** (Presupuesto), según (Ejecutado + Comprometido) ÷ Presupuesto:

  | Alerta | Regla |
  |---|---|
  | Sin presupuesto | Presupuesto = 0 |
  | Sin ejecución | No hay ejecutado ni comprometido |
  | 🟢 Dentro del presupuesto | Menos del 70 % |
  | 🟠 En riesgo | Del 70 % al 85 % |
  | 🟡 Cercano al límite | Del 85 % al 100 % |
  | 🔴 Sobrecosto | Más del 100 % |

- **Histograma de personal:** al actualizar el Seguimiento de pagos también se recalcula el histograma de personal **Real** a partir de la nómina (ver módulo Histogramas).
- **Corte de pagos guardado:** cada actualización guarda una copia de los pagos por semana con la fecha de corte del archivo (si se vuelve a cargar el mismo corte, se reemplaza). El Consolidado usa estas copias para la Variación temporal.

### Cómo se interrelacionan las pestañas

```
  Codificación.xlsx ──► PRESUPUESTO (Actividades Presupuestadas: disciplinas → rubros → valor presupuestado)
                                   ▲             ▲
                                   │ Ejecutado   │ Comprometido (semana en curso en adelante,
                                   │ (hasta la   │  menos OC sin consecutivo XXX)
                                   │  semana     │
                                   │  anterior)  │
                                   │             │
  Seguimiento ──► ACTUALIZAR SEGUIMIENTO ──► FLUJO DE CAJA (pagos semanales por rubro y moneda)
  de pagos       DE PAGOS    │            └─► CORTE DE PAGOS guardado ──► Variación temporal del CONSOLIDADO
                             └──────────────► COMPROMISOS
                                               ├─ Resumen de Órdenes de Compra (OC + facturas/hitos)
                                               │     └─ OC sin consecutivo (XXX) ─► restan del Comprometido
                                               │                                    y quedan en "Pendiente Ejec."
                                               ├─ Salarios (quincena → cargo → código) ─► Histograma de personal Real
                                               └─ Proyectado (costos futuros sin OC)

  Presupuesto base de la oferta ──► PRESUPUESTO BASE vs REAL (costo base vs cantidad/valor real digitado)

  PRESUPUESTO (presupuesto, ejecutado, comprometido por categoría) ──► DASHBOARD
                                                                   └─► CONSOLIDADO semanal (+ TRM + IA Gemini) ──► PDF para gerencia
```

> ⚠ **Importante:** la actualización del Seguimiento de pagos **reemplaza por completo** el Flujo de Caja del proyecto. Cualquier valor digitado a mano en el Flujo de Caja se pierde en la siguiente importación. Las equivalencias guardadas, los números de OC asignados y los códigos asignados a facturas **sí se conservan**.

---

## Paso a paso

### Cargar la codificación (estructura del presupuesto)

Se hace una sola vez al iniciar el proyecto (o cuando cambie la codificación).

1. Entre a **Costos** → pestaña **Presupuesto**.
2. Haga clic en **Importar Codificación** y elija el archivo `.xlsx` de Codificación. Debe tener las hojas **COD COP**, **COD USD**, **PPTO COP** y **PPTO USD**.
3. Aparece un aviso amarillo: *"Codificación leída: N rubros COP y N rubros USD en N disciplinas"*.
4. Lea la advertencia: **esto reemplaza todas las partidas del presupuesto y los pagos del Flujo de Caja** asociados.
5. Haga clic en **Confirmar e importar** (o **Cancelar**).
6. La app crea una categoría por disciplina y, dentro de ella, un rubro por cada fila de PPTO COP y PPTO USD con su valor presupuestado.

Detalles que conviene saber:
- Si un rubro aparece repetido en la misma hoja, se toma una sola vez.
- Los rubros `CMIE` y `PTIE` se excluyen (no tienen disciplina).
- Si un rubro no tiene disciplina en COD COP/COD USD, queda en la categoría **Otros**.
- Si el proyecto no tiene Codificación, puede usar **📋 Cargar plantilla EPC** (catálogo estándar de rubros con presupuesto en cero). Si ya hay partidas, el botón se llama **↻ Recargar plantilla** y pide **Confirmar** porque reemplaza todo.

> ⚠ Por confirmar: el proyecto **La Soberana** usa una codificación ANTIGUA (presupuesto por columna Rubro, separando COP y USD). Los demás proyectos mantienen la codificación estándar del presupuesto base. Esta regla está pendiente de reconfirmar con control de costos.

### Cargar o ajustar el presupuesto

**Opción A — desde Excel:**
1. En **Presupuesto**, haga clic en **Importar Excel**.
2. Elija un `.xlsx` cuya primera hoja tenga, en las primeras 10 filas, un encabezado con una columna **Código** y otra **Presupuesto**.
3. La app actualiza el valor presupuestado de cada rubro cuyo código coincida y muestra: *"N partidas actualizadas desde Excel. N códigos del archivo no coinciden con ninguna partida."*

**Opción B — a mano:**
1. En la vista **Actividades Presupuestadas**, escriba el valor en la columna **Presupuesto** del rubro.
2. Si el rubro está en la moneda equivocada, use los botones **COP / USD** de la columna **Moneda** (convierte los valores con la TRM).
3. Haga clic en **💾 Guardar cambios**.

**Vistas por moneda:** use **Solo COP**, **Solo USD** o **Ambas (en COP)** arriba a la derecha. Las tarjetas, la tabla Costos por Categoría y los totales se calculan en la moneda de la vista (en "Ambas", los USD se convierten a COP con la TRM). Cuando la vista no es "Solo COP" aparece el campo **TRM $**; al cambiarlo se guarda de inmediato.

Lo que NO se digita en esta tabla: **Ejecutado**, **% Ejec.**, **Comprometido**, **Pendiente Ejec.** y **Alerta**. Si el Pendiente muestra un ícono naranja ⓘ, pase el mouse: indica cuánto de ese pendiente corresponde a OC sin consecutivo (XXX).

### Actualizar el Seguimiento de pagos semanal

Se hace cada semana, cuando llega el archivo de Seguimiento de pagos. Una sola importación actualiza **Flujo de Caja y Compromisos** al mismo tiempo.

**Antes de importar, revise el archivo:**
- El nombre debe tener la fecha de corte en formato `MM.DD.AAAA` (ej. `Seguimiento de pagos 09.12.2026.xlsx`).
- La **primera hoja** es la que se lee. Debe tener las secciones **EGRESOS - COP** y **EGRESOS - USD** en la columna B, las fechas de corte semanales en la **fila 2 desde la columna L**, y termina en la fila **Total Acumulado**.
- Los egresos vienen en negativo (la app los registra en positivo); un valor positivo se toma como reintegro y resta.
- Las facturas pagadas deben tener la celda **Total a pagar** (columna K) rellena de **verde**.

**Pasos:**
1. Entre a **Costos** → **Compromisos** (o **Presupuesto** → vista **Flujo de Caja**). Ambas tienen el botón.
2. Haga clic en **Actualizar seguimiento de pagos** y elija el archivo (`.xlsx`, `.xlsm` o `.xls`). Verá "Leyendo…".
3. Se abre la ventana **Importar seguimiento de pagos** con la vista previa:
   - **Archivo** y **Fecha de corte del documento** (y el último corte ya cargado).
   - **Flujo de Caja:** N movimientos de N códigos, con el rango de cortes.
   - **Compromisos:** N órdenes de compra, y cuántas están sin consecutivo (XXX).
   - En rojo, si las hay: **"Se encontraron N facturas sin código asignado"**.
4. Revise las alertas de la ventana:
   - 🔴 **Documento desactualizado:** el archivo tiene un corte anterior al ya cargado. Verifique que es el archivo correcto.
   - 🟡 **No se pudo validar la fecha:** el nombre del archivo no trae la fecha `MM.DD.AAAA`.
5. Haga clic en **Confirmar y actualizar** (si el documento está desactualizado, el botón es rojo y dice **Reemplazar de todas formas**), o en **Cancelar**.
6. Al terminar aparece un mensaje con el resultado: OC nuevas y actualizadas, valores cargados al flujo y avisos (OC sin consecutivo, facturas sin código, líneas de salarios/proyectado sin rubro, subcódigos sin equivalencia).
7. En **Compromisos** aparece la etiqueta **"Seguimiento de pagos actualizado · corte dd/mm/aaaa"**.

**Qué hace la importación:**
- **Flujo de Caja:** borra los pagos anteriores y carga los del archivo (el corte más reciente trae todo el histórico). Amplía, si hace falta, las fechas de inicio y fin del flujo.
- **Órdenes de compra:** se actualizan por número de OC (no se duplican). Las OC importadas que ya no vienen en el archivo se eliminan. Las **OC manuales se conservan**; si una OC manual tiene el mismo número que una del archivo, pasa a ser la del archivo.
- **Salarios y Proyectado:** se recargan.
- **Pagos de impuestos DIAN:** **no se importan** (decisión definida).
- Recalcula el Ejecutado y el Comprometido del Presupuesto y el histograma Real de personal.
- Guarda el **corte de pagos** de esa fecha para la Variación temporal del Consolidado.

**Después de importar, atienda los pendientes:**
- Subcódigos sin equivalencia → ver *Clasificar un subcódigo sin equivalencia*.
- OC XXX → ver *Asignar número a una OC sin consecutivo (XXX)*.
- Facturas sin código → ver *Asignar código a facturas sin código*.
- Salarios sin código → ver *Confirmar el código sugerido de salarios*.

### Clasificar un subcódigo sin equivalencia

1. Vaya a **Presupuesto** → vista **Flujo de Caja**.
2. Verá un recuadro naranja de **subcódigos sin equivalencia** (N) que pide clasificarlos para incluirlos en el flujo, con el código, número de movimientos y total en COP/USD.
3. En la lista **— Elegir código destino —**, seleccione el rubro correcto del presupuesto.
4. Haga clic en **Clasificar e incluir**. Los valores entran al flujo y la equivalencia queda guardada.
5. Para revisar o borrar equivalencias, use **Equivalencias guardadas (N)** y el botón **✕** de cada una. Si borra una, en la próxima importación se le pedirá clasificarla de nuevo.

> ⚠ Por confirmar: en la versión actual las equivalencias guardadas no están separadas por proyecto (una equivalencia creada en un proyecto se aplica también en los demás). Verifique antes de cambiar una equivalencia que usan otros proyectos.

> ⚠ Por confirmar: el título de ese recuadro naranja todavía muestra el nombre interno del archivo; falta cambiarlo en pantalla por "Seguimiento de pagos", como en el resto del módulo.

> Nota: el recuadro naranja solo se muestra justo después de importar. Si sale de la pantalla sin clasificar, vuelva a importar el archivo para que reaparezca.

### Asignar número a una OC sin consecutivo (XXX)

1. Vaya a **Compromisos** → **Resumen de Órdenes de Compra**.
2. Las OC en proceso aparecen en naranja con un campo de texto en la columna **# ORDEN COMPRA** y el estado **En proceso**.
3. Escriba el número real (ej. `CO_2026_150`).
4. Haga clic en **Asignar**.
5. La OC pasa a **Pendiente** o **Pagada**, su valor pasa a contar como **comprometido** y el número queda recordado para las próximas importaciones.

La app no deja asignar un número que contenga "XXX" ni uno que ya exista en otra OC.

### Asignar código a facturas sin código

Las facturas sin código **nunca se heredan ni se asumen**: hay que asignarlas a mano una vez.

1. En **Compromisos** → **Resumen de Órdenes de Compra**, busque las OC con la etiqueta roja **⚠ N sin código** (o use **Filtros** → Estado **Con facturas sin código**).
2. Haga clic en la fila de la OC para desplegar sus facturas.
3. En la factura sin código, escriba o elija el código en el campo **Código** (la lista sugiere los rubros del presupuesto).
4. Haga clic en **✓**.
5. La factura muestra el código con la marca **manual** y queda recordada para próximas importaciones.

Para corregir un código ya asignado, use el lápiz ✏ junto al código, escriba el nuevo y confirme con **✓** (o **×** para cancelar). Si el código no existe en el presupuesto ni en la codificación, la app muestra un error y no lo guarda.

### Confirmar el código sugerido de salarios

1. En **Compromisos** → **Resumen de Órdenes de Compra**, baje hasta la sección **Salarios y mano de obra**.
2. Use **Expandir todo** o haga clic en una quincena. La tabla muestra **Quincena → Cargo → Código** con **Personas**, **Total**, **Pagado** y **Pendiente**. Hay además bloques de **Seguridad social** (planillas) y **Liquidaciones**.
3. Las líneas sin código aparecen en rojo (**⚠ Sin código**) con un código propuesto y la etiqueta **sugerido** (el código más usado por ese cargo en la nómina).
4. Si el sugerido es correcto, haga clic en **✓**. Si no, escriba el código correcto y haga clic en **✓**.
5. El código se aplica a **todas las personas de ese cargo sin código en esa quincena** y queda recordado. Si en una importación futura el archivo trae el código, gana el del archivo.

> Mientras no confirme con ✓, el código sugerido **no cuenta**.

### Revisar la pestaña Proyectado

1. En **Compromisos**, haga clic en **Proyectado**.
2. Tarjetas: **Total proyectado** (COP | USD), **Líneas proyectadas** (con aviso de cuántas están sin rubro) y **Códigos de rubro** con valor proyectado.
3. La tabla agrupa por período. Las líneas sin rubro muestran un campo **Código** con **✓** para asignarlo (igual que las facturas).

> ⚠ Por confirmar: si los valores de la sección Proyectado también se reflejan en las semanas futuras del Flujo de Caja (y por tanto en el Comprometido) depende de cómo el área de pagos los registre en el archivo; confirmar con control de costos.

### Registrar o editar una OC manual

Solo para OC que no vienen en el archivo de Seguimiento de pagos.

1. En **Compromisos** → **Resumen de Órdenes de Compra**, haga clic en **Nueva Orden de Compra**.
2. Complete: **Moneda**, **Código rubro (presupuesto)**, **# Orden de compra** \*, **Proveedor** \*, **Descripción del servicio**, **Valor aprobado OC** \*, **# Factura**, **Fecha factura**, **Facturado**, **Pagado** y **Fecha OC**.
3. Haga clic en **💾 Guardar**.
4. La OC aparece con la etiqueta **Manual**. Solo las OC manuales tienen botones de editar (✏) y eliminar (✕, pide confirmar con ✓).

El número de factura no se puede repetir entre OC. Las OC manuales suman en las tarjetas de Compromisos, pero **no** alimentan el Ejecutado ni el Comprometido del Presupuesto (esos salen del Flujo de Caja).

### Ajustar el Flujo de Caja a mano

1. Vaya a **Presupuesto** → vista **Flujo de Caja**.
2. Elija **Pesos (COP)** o **Dólares (USD)**; cada código aparece una sola vez y la vista separa las monedas.
3. Ajuste **Fecha inicio** y **Fecha fin** para cambiar las columnas semanales mostradas.
4. Para cambiar un valor, escríbalo en la celda de la semana. Para ver o agregar pagos detallados, haga clic en 🔍 dentro de la celda y use **+ Agregar** (Descripción, Monto, Factura, Proveedor).
5. Para ocultar un código en esa vista, use el ícono 👁‍🗨 junto a la descripción; recupérelo con **Desactivadas (N)** → **Activar**.
6. Haga clic en **💾 Guardar cambios**.

También puede cargar pagos con **Importar Excel** (primera hoja con columna **Código** y fechas en el encabezado); se cargan en la moneda de la vista activa.

> ⚠ Recuerde: la siguiente **Actualizar seguimiento de pagos** reemplaza todo el Flujo de Caja. Use los ajustes manuales solo de forma temporal.

### Filtrar y exportar las órdenes de compra

1. En **Compromisos**, haga clic en **Filtros**.
2. Filtre por **Código / rubro**, **# Orden compra**, **Proveedor**, **Descripción**, **# Factura / documento**, **Estado** (Pendiente, Pagada, En proceso, Sobrepasada, Con facturas sin código) y **Fecha factura desde/hasta**.
3. Haga clic en **Aplicar filtros** (o **Limpiar**).
4. Haga clic en **Exportar Excel**: descarga las OC filtradas con sus facturas, más las secciones de Salarios y Proyectado.

### Importar el presupuesto base

1. Vaya a **Costos** → **Presupuesto base vs Real**.
2. Haga clic en **Importar presupuesto base** y elija el Excel de la oferta.
3. La app lee las hojas **General**, **Sum PPAL** y **Rend MAT**. En cada hoja busca (en las primeras 15 filas) el encabezado con la columna **Check** y toma: Check (código), descripción (Tipo Concepto o Descripción), concepto, unidad, cantidad, costo unitario, moneda y **costo total**.
4. Mensaje de resultado: *"Presupuesto base importado: N líneas desde General (N), Sum PPAL (N), Rend MAT (N)"*, con las hojas no leídas si faltó alguna.
5. Si vuelve a importar el presupuesto base, **se conservan** las cantidades y valores reales ya digitados en las líneas que coincidan (misma hoja + código + descripción).

### Digitar reales en Presupuesto base vs Real

1. En **Presupuesto base vs Real**, las líneas están agrupadas por código (fila gris con subtotal).
2. Por cada línea escriba:
   - **CANT. REAL**: cantidad realmente comprada/ejecutada.
   - **VALOR REAL (COP)**: valor real en pesos.
   - **UND** (unidad), si el presupuesto base no la trae.
3. La app calcula **DIF. CANT.** (CANT. BASE − Real) y **DIFERENCIA** (TOTAL BASE − Valor real). En rojo = desfase (lo real supera el presupuesto base).
4. Aparece el aviso **"⚠ Tienes cambios sin guardar"**. Haga clic en **💾 Guardar ahora** o **💾 Guardar cambios**. Si intenta cambiar de pestaña sin guardar, la app le pregunta si desea salir sin guardar.
5. Use **Filtros** (Código, Fuente, Concepto PER/MAQ/MAT, Descripción, Diferencia: con desfase / a favor / con real / sin real, Diferencia mínima COP) y **Exportar Excel** según necesite.

Tarjetas: **Costo Total Presupuesto base**, **Costo Real** y **Diferencia** (A favor o Desfase), en COP y USD.

> ⚠ Por confirmar: los valores reales del presupuesto base se digitan a mano; todavía no se cruzan automáticamente con las OC ni con el Flujo de Caja. Los porcentajes de alerta (semáforo) de desfase Presupuesto base vs Real están pendientes de definir.

### Generar el consolidado semanal

1. Vaya a **Costos** → **Consolidado** y haga clic en **Nuevo consolidado** (o **Crear primer informe**).
2. La app arma el informe automáticamente:
   - **N° de informe** consecutivo, versión 1, estado Borrador.
   - **Período:** desde el día siguiente al fin del consolidado anterior hasta hoy (editable).
   - **TRM de cierre:** la TRM oficial del día; si no la consigue, usa la TRM del proyecto.
   - **Presupuesto, Ejecutado y Comprometido** (COP y USD) y el **Desglose por Categoría**, tomados del Presupuesto.
   - **Comparar con:** el consolidado anterior.
3. Revise y complete:
   - **Responsable del informe**.
   - TRM: escríbala o use **TRM automática** (recalcula los montos en USD).
   - **Justificación de las variaciones** de **Venta** y de **Costo**.
   - **Variación temporal:** elija los cortes a comparar y digite los **Ingresos** y las **Justificaciones** por semana (ver *Revisar la Variación temporal del consolidado*). Los **Pagos** ya no se digitan.
4. Si cambió datos en Presupuesto o en el Seguimiento de pagos mientras tanto, haga clic en **Recalcular**.
5. Análisis con IA: haga clic en **Generar Análisis**. La IA (Google Gemini) redacta un análisis a partir de los totales y categorías del informe. Active **Incluir en impreso** si debe salir en el PDF.
6. Haga clic en **Guardar**. (El análisis de IA solo queda guardado si guarda el informe después de generarlo.)
7. Para imprimir o guardar como PDF: **Imprimir** dentro del informe, o el botón PDF 📄 en el listado. Sale en horizontal con encabezado y logo.

Otras acciones desde el listado: **Ver** 👁, **Editar** ✏ (solo informes en Borrador), **Nueva versión** (copia el informe como v2, v3…), **Eliminar** (lo envía a la **Papelera**, desde donde se puede **Restaurar**).

Qué muestra el informe: tarjetas **Presupuesto / Ejecutado / Comprometido / Disponible** (Disponible = Presupuesto − Ejecutado − Comprometido), barras de **Distribución de Costos (USD)**, **Ejecución Presupuestal** (% ejecutado, con la TRM base de la oferta y la TRM actual), **Comparativo** entre el informe de comparación y el actual, **Resumen USD** y **Resumen COP** (Venta, Costo total = Ejecutado + Comprometido, Margen y %; columnas Base Inicial, Sem. Anterior, Resultado Actual y Variación), desglose por categoría, variaciones temporales y análisis IA.

### Comparativo Semana N vs Semana M

Aparece cuando el informe tiene otro informe en **Comparar con**. Su título es **Comparativo: Semana N vs Semana M** (los números de informe) y compara **Presupuesto**, **Ejecutado**, **Comprometido** y **Disponible** en tres grupos de columnas:

| Grupo | Qué muestra |
|---|---|
| **USD** | Montos en dólares de cada informe y su variación. |
| **COP** | Montos en pesos de cada informe y su variación. |
| **USD+COP** | Todo expresado en dólares: USD + (COP ÷ TRM). **Ambas semanas se convierten con la misma TRM de cierre del informe actual**, para que la variación muestre solo el cambio en costos y no el efecto de la tasa de cambio (definido el 2026-09-30). |

### Revisar la Variación temporal del consolidado

Las tablas **Variación temporal USD+COP**, **Variación temporal USD** y **Variación temporal COP** comparan los pagos por semana de **dos cortes del Seguimiento de pagos**. Ya no se generan semanas ni se digitan pagos: salen solos de los cortes guardados en cada actualización del Seguimiento de pagos.

1. Encima de las tablas está la barra **Comparar cortes de pagos:** con dos listas:
   - La primera es el **corte actual**: cualquier corte guardado (por defecto, el más reciente).
   - La segunda (después de **contra**) es el **corte anterior**: cualquier semana anterior al corte actual, de 7 en 7 días hasta el inicio del Flujo de Caja. Por defecto es la **semana anterior** (corte actual − 7 días).
2. Si la fecha elegida como anterior tiene un corte guardado, se usan sus datos. Si no, la app la **calcula del archivo actual** (el archivo es acumulado): mismas cifras, pero con vacías las semanas que se pagaron después de ese cierre. Se ve la nota *"Cierre dd/mm/aaaa calculado del documento cargado: pagos hasta esa fecha + programación vigente."* Así nunca hace falta volver a cargar archivos viejos.
3. Cada tabla tiene las mismas columnas: un bloque **Cierre semana** del corte actual y otro del corte anterior, cada uno con **Semana**, **Ingresos**, **Pagos** y **Caja**; luego **Variación** y **Justificación**. Las filas son las semanas del Flujo de Caja (de su fecha de inicio a su fecha de fin, cada 7 días).
   - **Pagos:** salen del corte (solo lectura).
   - **Ingresos:** se digitan en el bloque del corte actual, en modo edición. En el bloque del corte anterior aparecen como "-".
   - **Caja:** ingresos − pagos, acumulado semana a semana.
   - **Variación:** Caja acumulada del corte actual − Caja acumulada del corte anterior. Muestra lo pagado (o reprogramado) entre los dos cierres.
   - **Justificación:** texto libre por semana y por tabla, en modo edición.
4. La tabla **USD+COP está en dólares**: los pesos se convierten con la **TRM de cierre del informe** (definido el 2026-09-29).
5. Debajo de la tabla USD+COP hay una gráfica que compara los pagos por semana de los dos cortes.
6. La elección de cortes, los ingresos y las justificaciones quedan en el informe al **Guardar** (cambiar los cortes fuera de modo edición solo cambia la vista en pantalla).

Si aparece *"Sin cortes de pagos guardados. Importa el archivo en Costos → Flujo de Caja o Compromisos."*, actualice el Seguimiento de pagos una vez para que se guarde el primer corte.

> ⚠ Por confirmar: los **Ingresos** siguen siendo digitados hasta que exista una hoja de ingresos en el archivo. Mientras tanto, el bloque del corte anterior no tiene ingresos, de modo que la Variación incluye los ingresos digitados del corte actual. Confirmar con control de costos cómo deben leerse.

> ⚠ Por confirmar: la **Venta** del consolidado se toma del *Presupuesto contractual* registrado en la ficha del proyecto, no de la hoja "Resumen" del presupuesto base. Confirmar cuál debe ser la fuente oficial de venta/costo/margen.
> ⚠ Por confirmar: en la pantalla no hay un control para cambiar el estado del informe (En revisión, Aprobado, Rechazado); los informes quedan en Borrador.

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| Al actualizar el Seguimiento de pagos sale **"⚠ Documento desactualizado"** y el botón rojo **Reemplazar de todas formas** | El archivo tiene una fecha de corte anterior a la del último archivo cargado | Cancele y busque el archivo de Seguimiento de pagos más reciente. Solo reemplace si está seguro (por ejemplo, corrección de un archivo). |
| **"⚠ No se pudo validar la fecha"** | El nombre del archivo no tiene la fecha en formato `MM.DD.AAAA` | Renombre el archivo con la fecha de corte (ej. `... 09.12.2026.xlsx`) y vuelva a importar. |
| **"No se encontraron egresos ni órdenes de compra. Verifica que sea el archivo de seguimiento de pagos correcto."** | Archivo equivocado, o la primera hoja no es la de pagos (faltan las secciones EGRESOS - COP/USD) | Verifique el archivo y que la hoja de pagos sea la primera del libro. |
| Una OC muestra **⚠ Sobrepasada** | Lo facturado supera el valor aprobado en más de $1.000 COP o US$ 1 | Revise con compras si falta un otrosí o ampliación de la OC, o si hay una factura mal asociada. Se corrige en el archivo de Seguimiento de pagos. |
| **"⚠ N facturas sin código asignado"** | El archivo no trae código en esas facturas | Asígnelas en Compromisos (ver *Asignar código a facturas sin código*). Quedan recordadas. |
| Líneas de Salarios o Proyectado con **⚠ sin rubro / ⚠ Sin código** | El archivo no trae código de rubro en esas líneas | Salarios: confirme o corrija el código sugerido con ✓. Proyectado: escriba el código y confirme con ✓. |
| **"⚠ N subcódigos sin equivalencia NO se cargaron al flujo"** | Subcódigo nuevo que no está en la Codificación ni en las equivalencias guardadas, o su rubro no existe en el presupuesto | Clasifíquelo en **Flujo de Caja** con **Clasificar e incluir**. Si el rubro no existe, primero agréguelo al presupuesto (Codificación). |
| **"El código 'XXXX' no existe en el listado de códigos del presupuesto ni en la codificación"** | Código mal escrito o rubro que no está en el presupuesto | Revise la ortografía; elija el código de la lista desplegable. |
| **"La orden de compra CO_… ya existe en la lista"** al asignar número | Ese número ya está usado por otra OC | Verifique el número real con compras. |
| Una OC sigue **En proceso** y su valor no aparece en Comprometido | Es una OC sin consecutivo (XXX): cuenta como presupuestado, no comprometido | Asígnele el número real con **Asignar** cuando compras lo emita. |
| El **Ejecutado** no cambió después de digitar un pago de esta semana | El Ejecutado solo cuenta hasta la semana anterior; lo de esta semana en adelante es Comprometido | Es el comportamiento esperado. El lunes siguiente pasará a Ejecutado. |
| Desaparecieron valores que digité a mano en el Flujo de Caja | Se actualizó el Seguimiento de pagos, que reemplaza todo el flujo | Pida que el dato quede en el archivo de Seguimiento de pagos; evite digitar a mano en el flujo. |
| Un código aparece con montos raros al ver **Ambas (en COP)** | La TRM del proyecto está desactualizada | Actualice el campo **TRM $** en Presupuesto. |
| Al importar la Codificación desaparecieron los pagos del Flujo de Caja | La importación de Codificación (o Recargar plantilla) reemplaza partidas y pagos | Vuelva a cargar el último archivo de Seguimiento de pagos. |
| **"No se encontraron rubros. Verifica que el archivo tenga las hojas COD COP/USD y PPTO COP/USD."** | Nombres de hojas distintos o archivo equivocado | Revise que las hojas se llamen exactamente así. |
| **"No se encontraron líneas válidas. Verifica que el archivo tenga las hojas General, Sum PPAL y Rend MAT con la columna Check."** | El presupuesto base no tiene esas hojas o el encabezado **Check** | Revise el archivo del presupuesto base; el encabezado debe estar en las primeras 15 filas. |
| En el análisis IA aparece **"⚠ Error al consultar IA: TooManyRequests"** | Límite de uso de la API de Gemini agotado (error 429); la facturación de Gemini está pendiente de habilitar | Escriba el análisis a mano o intente más tarde. Reporte a la administración de la plataforma. |
| **"⚠ API key de Gemini no configurada"** o **"⚠ Error de conexión con IA"** | Falta la configuración de la IA en el servidor o no hay conexión | Reporte a la administración de la plataforma. El resto del consolidado funciona sin IA. |
| **"Tienes cambios sin guardar en … ¿Salir sin guardar?"** | Intentó cambiar de pestaña con reales del presupuesto base sin guardar | Cancele y haga clic en **Guardar cambios**, o acepte para descartarlos. |
| La Variación temporal sale vacía con *"Sin cortes de pagos guardados"* | El proyecto no tiene ningún corte guardado (por ejemplo, el archivo se cargó antes de que existiera esta función, 29/09/2026) | Actualice el Seguimiento de pagos una vez; desde ahí cada carga guarda su corte. |
| La Variación de todas las semanas es 0 | Se eligió como corte anterior una fecha con los mismos pagos que el actual, o el mismo corte | Elija en **contra** una semana anterior al corte actual. |
| Una nota dice *"calculado del documento cargado"* | La fecha elegida como anterior no tiene corte guardado y se deriva del archivo actual | Es normal; no hace falta cargar archivos viejos. |
| En **Comparativo** el grupo USD+COP no cuadra con la suma de USD y COP de cada informe | El grupo USD+COP convierte ambas semanas con la TRM de cierre del informe actual | Es intencional: así la variación no incluye efecto cambiario. |

---

## Relación con otros módulos

- **Histogramas:** cada actualización del Seguimiento de pagos recalcula el histograma de **Personal Real** desde la nómina (sección Salarios). Cualquier error de cargo o quincena en el archivo de Seguimiento de pagos se refleja allí.
- **Proyectos:** la **TRM del proyecto**, las fechas planeadas (usadas como fechas iniciales del Flujo de Caja) y el **Presupuesto contractual** (Venta del Consolidado) vienen de la ficha del proyecto.
- **Dashboard de Costos:** muestra **Presupuesto Total**, **Total Ejecutado**, **Compromisos Activos**, **Exposición Total** (Ejecutado + Comprometido), **% Ejecución**, **Saldo Disponible**, **Registros de Costo** y **Compromisos Vencidos**, más las gráficas *Presupuesto vs Ejecutado vs Compromisos por categoría*, *Distribución por tipo de costo*, *Evolución mensual de costos reales* y *Estado de compromisos*. Todos los montos están en COP y USD.

> ⚠ Por confirmar: en el Dashboard, la tarjeta **Compromisos Activos** dice "Saldos por pagar de OC", pero su valor es el Comprometido del Presupuesto (Flujo de Caja pendiente). Las gráficas *Distribución por tipo de costo*, *Evolución mensual* y la tarjeta *Registros de Costo* usan los costos registrados con el botón 💰 de Actividades Presupuestadas (no el archivo de Seguimiento de pagos), y *Compromisos Vencidos* depende de una fecha de vencimiento que hoy no se carga. Estas vistas están pendientes de alinearse con las fuentes actuales.

- **Centro de control** ([18](18-centro-de-control.md)) y **Dashboard del proyecto** ([14](14-dashboard-proyecto.md)): muestran presupuesto, ejecutado, comprometido, sobrecosto y costo estimado al cierre en pesos, calculados con el mismo Presupuesto de este módulo.

---

## Buenas prácticas

1. **Actualice el Seguimiento de pagos una vez por semana**, siempre con el archivo más reciente y con la fecha `MM.DD.AAAA` en el nombre.
2. **No digite pagos en el Flujo de Caja**: corrija la fuente (el archivo de Seguimiento de pagos), porque cada importación reemplaza el flujo.
3. **Atienda los pendientes justo después de importar:** subcódigos sin equivalencia (antes de salir de la pantalla), OC XXX, facturas sin código y salarios sin código.
4. **Nunca confirme un código sugerido sin revisarlo.** Las asignaciones quedan recordadas para siempre en ese proyecto.
5. **Revise las OC sobrepasadas** cada semana con compras.
6. **Mantenga la TRM del proyecto al día** antes de revisar vistas en "Ambas (en COP)" o el Dashboard.
7. **No vuelva a importar la Codificación ni recargar la plantilla** con el proyecto en marcha, salvo que sea necesario: borra el Flujo de Caja (hay que volver a cargar el Seguimiento de pagos).
8. **Guarde los reales del presupuesto base** antes de cambiar de pestaña.
9. **En el consolidado:** use **Recalcular** antes de guardar, revise que los cortes comparados en la Variación temporal sean los correctos, escriba siempre las justificaciones de venta y costo, y genere el análisis IA antes de **Guardar**. Para corregir un informe ya emitido, use **Nueva versión** en vez de editar el original.
10. **Lea los montos en su moneda:** no sume pesos y dólares sin convertir; use las vistas por moneda que ofrece la app.
