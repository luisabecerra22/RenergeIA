# 1. Proyectos

> **Para qué sirve:** es la puerta de entrada a RenergeIA. Aquí se inicia sesión, se ve el estado general del portafolio (Centro de Control), se crean los proyectos EPC (idealmente cargando la BOM de la oferta), se editan sus datos y se envían a la papelera o se restauran. Cada proyecto es el contenedor de todos los demás módulos.
> **Quién lo usa:** gerencia y dirección (Centro de Control), control de proyectos y administradores (crear/editar/eliminar), y todo el equipo para entrar a su proyecto.
> **Dónde está:**
> - Inicio de sesión: `/login`
> - Centro de Control (portafolio): menú lateral **Inicio** → `/`
> - Lista de proyectos: menú lateral **Proyectos** → `/proyectos`
> - Nuevo proyecto: **+ Nuevo proyecto** → `/proyectos/nuevo`
> - Detalle: **Ver detalle** → `/proyectos/{id}`
> - Editar: **Editar** → `/proyectos/{id}/editar`
> - Papelera: **Papelera** → `/proyectos/papelera`

---

## Conceptos clave

- **Proyecto:** una planta o contrato EPC fotovoltaico. Tiene código, nombre, cliente, ubicación, país, capacidad, presupuesto contractual en USD y fechas planeadas y reales.
- **Estado del proyecto:** Planificación, En Ejecución, Suspendido, Completado, Cancelado u Operación y Mantenimiento. Se elige a mano en el formulario. Solo los proyectos **En Ejecución** cuentan como "activos" en el Centro de Control.
- **BOM (oferta):** el Excel con el que se cerró el negocio. Su hoja **Ejecutivo** tiene los datos generales (código, nombre, cliente, país, potencia, TRM, costo, precio y margen) y sus hojas de detalle alimentan el comparativo **BOM vs Real** de Costos.
- **Ficha de la Oferta (BOM):** tarjeta del detalle del proyecto con el costo, el precio de venta y el margen en COP y USD, más el desglose (obras civiles, costo pleno, total con IVA) tomado de la BOM.
- **SPI (índice de desempeño del cronograma):** avance real dividido entre avance programado. 1,00 significa "al día"; menos de 1 significa atraso.
- **Clasificación de un proyecto:** Saludable (SPI de 0,95 o más), En Riesgo (de 0,85 a menos de 0,95), Crítico (menos de 0,85) o Sin datos (aún no tiene avances registrados en Informes Diarios).
- **Papelera:** los proyectos eliminados no se borran; quedan ocultos en la papelera y se pueden restaurar. Solo **Eliminar definitivamente** los borra para siempre.

---

## Cómo funciona

### Qué calcula la app sola

**Centro de Control (Inicio)**
- **Proyectos Activos:** cuenta los proyectos en estado *En Ejecución*, junto al total de proyectos del portafolio.
- **Avance Global Real / Programado, SPI Global Promedio y Desviación:** promedios de los proyectos que no están Cancelados ni Completados y que tienen avances registrados. Salen del **último registro de avance de cada actividad** en los Informes Diarios (no se digitan aquí).
- **Actividades Atrasadas:** actividades cuyo último avance reportado va por debajo de lo programado.
- **Tarjetas Saludables / En Riesgo / Críticos / Sin datos:** cuántos proyectos hay en cada clasificación (según el SPI) y qué porcentaje representan frente a los proyectos activos.
- **Insignia de estado general:** *Crítico* si hay al menos un proyecto crítico; si no, *En Riesgo* si hay alguno en riesgo; si no, *Saludable*; y *Sin datos* si ningún proyecto tiene avances.
- **Resumen ejecutivo:** un párrafo que se arma solo con esos números y una recomendación (por ejemplo, priorizar los proyectos con SPI menor a 0,85).
- **Proyectos en seguimiento:** tabla con hasta 10 proyectos (no se incluyen los Cancelados ni los Completados), ordenados de peor a mejor: primero los Críticos, luego En Riesgo, Saludables y Sin datos. Columnas: Proyecto, Estado, Avance Prog., Avance Real, Desviación, SPI, Atrasadas, Último Registro y Acción (botón **Dashboard**).

**Al crear un proyecto**
- Si cargas la BOM, la app **prellena** código, nombre, cliente, país, capacidad (en MWp), presupuesto contractual (precio en USD) y la descripción ("Alcance: …").
- Al guardar, la app además: guarda la Ficha de la Oferta; toma la **TRM de la BOM** como tasa de cambio COP/USD del proyecto; carga las líneas de la BOM en **Costos → BOM vs Real**; crea un **presupuesto inicial por rubro en COP** con el costo de la BOM, y crea las **plantillas de histogramas** de personal y equipos según la capacidad.
- Busca **proyectos repetidos** (mismo código o mismo nombre, sin importar mayúsculas), incluidos los que están en la papelera.
- Convierte la capacidad: si la digitas en MWp la guarda en kWp (y muestra la equivalencia debajo del campo).

**Al editar un proyecto**
- Si el cambio de capacidad hace que el proyecto pase a otra categoría de tamaño (hasta 5 MW, hasta 12,5 MW, hasta 17,5 MW o más), la app **vuelve a generar las plantillas de histogramas** de personal y equipos para esa nueva categoría.

### Qué se digita y qué no
| Se digita | No se digita (lo calcula o trae la app) |
|---|---|
| Código, nombre, cliente, estado, ubicación, país, capacidad, presupuesto contractual, fechas planeadas, descripción | Ficha de la Oferta (viene de la BOM) |
| Fechas reales de inicio y fin (solo en **Editar**) | Avances, SPI, desviación y clasificación del Centro de Control (vienen de los Informes Diarios) |
| | Tasa de cambio inicial (TRM de la BOM), líneas BOM vs Real, presupuesto inicial por rubro, plantillas de histogramas |

### Navegación general
- **Menú lateral:** **Inicio**, **Proyectos** (con la flecha se despliega la lista de proyectos en orden alfabético) y **HSEQ** (corporativo).
- Cada proyecto del menú se despliega con su flecha y muestra sus módulos: Dashboard, Informe Diario, Cronograma de Actividades, Documentos, Costos, HSEQ, No Conformidades, Restricciones, Histogramas, Clima y Alertas. El módulo en el que estás aparece resaltado y el proyecto en el que estás se despliega solo.
- La sección **HSEQ** corporativa contiene **Dashboard HSEQ**, **Auditorías** (ISO 9001, ISO 14001, ISO 45001, Decreto 1072, Resolución 0312, Aud. Cliente, Aud. Interventoría, Historial) e **Inspecciones** (STC, OTS, Pausas Activas, Consolidado Anual).
- **Barra superior:** muestra el usuario conectado y el botón **Salir**.

> ⚠ Por confirmar: todas las pantallas de este capítulo solo exigen haber iniciado sesión; el código no restringe por rol quién puede crear, editar o eliminar proyectos. Definir con el equipo qué perfiles deben hacerlo.

---

## Paso a paso

### Iniciar sesión
1. Abre la dirección de la aplicación. Si no has iniciado sesión, la app te lleva a la pantalla **Iniciar sesión**.
2. Escribe tu **Correo electrónico** y tu **Contraseña**.
3. Marca **Recordarme** si quieres que el navegador mantenga la sesión abierta.
4. Pulsa **Ingresar**. Llegarás al **Centro de Control RenergeIA**.
5. Para cerrar sesión, pulsa **Salir** en la esquina superior derecha.

> La pantalla de inicio de sesión no tiene opción para registrarse ni para recuperar la contraseña: solicita tu usuario o un cambio de contraseña al administrador de la plataforma.

### Consultar el Centro de Control (portafolio)
1. En el menú lateral pulsa **Inicio**.
2. Revisa la fila superior: **Proyectos Activos**, **Avance Global Real**, **SPI Global Promedio** y **Actividades Atrasadas**. La hora de cálculo aparece en "Actualizado: dd/mm/aaaa hh:mm".
3. Mira las tarjetas **Saludables**, **En Riesgo**, **Críticos** y **Sin datos**.
4. Lee el **RESUMEN EJECUTIVO**.
5. En **PROYECTOS EN SEGUIMIENTO** ubica los proyectos en rojo o amarillo y pulsa **Dashboard** para ver el detalle.
6. Para ver datos recientes, recarga la página (los valores se calculan al abrirla).

### Crear un proyecto cargando la BOM (recomendado)
1. Ve a **Proyectos** y pulsa **+ Nuevo proyecto** (o **Crear Proyecto** en el Centro de Control si aún no hay proyectos).
2. En **Paso 1 — Cargar la BOM de la oferta (recomendado)** pulsa **Cargar BOM** y elige el archivo (.xlsx, .xlsm o .xls, máximo 60 MB).
3. Espera el mensaje verde "BOM leída correctamente…". Indica cuántas líneas se cargarán al comparativo BOM vs Real. Junto al botón verás el nombre del archivo, el código, el nombre, la potencia y el costo leídos.
4. Si aparece el aviso amarillo **⚠ Proyecto posiblemente repetido**, confirma que cargaste la BOM correcta antes de seguir.
5. En **Paso 2 — Revisa y completa los datos** revisa lo que se prellenó y completa lo que falta: **Estado**, **Ubicación**, **Fecha inicio planeada** y **Fecha fin planeada**.
6. Verifica la **Capacidad** y su unidad (**kWp** o **MWp**). Al cambiar la unidad, el número se convierte solo.
7. Pulsa **Guardar proyecto**. La app te lleva al detalle del nuevo proyecto.
8. Si la app confirma que el proyecto está repetido, elige una opción (ver *Resolver un aviso de proyecto repetido*).

> La **Fecha inicio planeada** es importante: la plantilla EPC del Cronograma de Actividades arranca en esa fecha.

### Crear un proyecto sin BOM
1. Pulsa **+ Nuevo proyecto**.
2. Ignora el Paso 1 y llena **Datos del proyecto**: Código*, Nombre del proyecto*, Cliente*, Estado, Ubicación*, País*, Capacidad* (kWp o MWp), Presupuesto contractual (USD)*, Fecha inicio planeada*, Fecha fin planeada* y Descripción (opcional).
3. Pulsa **Guardar proyecto**.
4. Sin BOM no se crean la Ficha de la Oferta, las líneas de BOM vs Real ni el presupuesto inicial por rubro. Las plantillas de histogramas sí se crean.

### Resolver un aviso de proyecto repetido
Al pulsar **Guardar proyecto**, si ya existe un proyecto con el mismo código o nombre (activo o en la papelera), aparece el recuadro rojo **⚠ Proyecto repetido** con tres opciones:
1. **Sí, es la BOM correcta — crear de todas formas:** crea el proyecto aunque se repita.
2. **No era la correcta — cargar la BOM correcta:** borra la BOM cargada; vuelve al Paso 1 y pulsa **Cargar BOM** con el archivo correcto.
3. **Voy a corregir los datos:** cierra el aviso para que cambies el código o el nombre y guardes de nuevo.

> Si el proyecto repetido está **en la papelera**, normalmente es mejor restaurarlo que crear uno nuevo.

### Ver el detalle de un proyecto
1. En **Proyectos**, pulsa **Ver detalle** en la tarjeta (o haz clic en el nombre del proyecto en el menú lateral).
2. Revisa **Información general** (código, cliente, ubicación, país, capacidad en kWp y MWp, presupuesto en USD) y **Cronograma** (inicio y fin planeados y reales).
3. Si el proyecto se creó con BOM, revisa la **Ficha de la Oferta (BOM)**: TRM de la BOM, alcance, tabla de COSTO / PRECIO (VENTA) / MARGEN / MARGEN % en COP y USD, y los desgloses de precio y costo.
4. En **Módulos del proyecto**, pulsa la tarjeta del módulo al que quieras entrar (Cronograma de Actividades, Informe Diario, Documentos, Dashboard, Costos, No Conformidades, HSEQ, Restricciones, Histogramas, Clima, Alertas).

### Editar un proyecto
1. Abre el proyecto y pulsa **Editar** (o el ícono de lápiz en la tarjeta de la lista).
2. Cambia los campos que necesites. En esta pantalla la **Capacidad (kWp)** se digita solo en kWp, y además puedes registrar **Fecha inicio real** y **Fecha fin real**.
3. Actualiza el **Estado** cuando el proyecto cambie de etapa (por ejemplo, de Planificación a En Ejecución), porque de eso depende que cuente como activo en el Centro de Control.
4. Pulsa **Guardar cambios** (o **Cancelar** para salir sin guardar).

> ⚠ Por confirmar: si al cambiar la capacidad el proyecto cambia de categoría de tamaño, las plantillas de histogramas de personal y equipos se vuelven a generar y se reemplazan sus ítems anteriores. Validar con el equipo si eso afecta ajustes hechos a mano en Histogramas.

### Enviar un proyecto a la papelera
**Desde la lista:**
1. En la tarjeta del proyecto pulsa el ícono de papelera (**Enviar a papelera**).
2. Confirma con **Sí** (o pulsa **X** para cancelar).

**Desde el detalle:**
1. Pulsa **Eliminar**.
2. Confirma con **Sí, eliminar** en el mensaje "¿Seguro que deseas eliminar…?" (o **Cancelar**).

El proyecto desaparece de la lista y del Centro de Control, y el botón **Papelera** muestra un contador rojo.

### Restaurar o eliminar definitivamente desde la papelera
1. En **Proyectos**, pulsa **Papelera**.
2. Cada tarjeta muestra el código, el nombre, el cliente, la ubicación y la fecha y hora de eliminación.
3. Para recuperar un proyecto, pulsa **Restaurar**. Vuelve a la lista con todos sus datos.
4. Para borrarlo para siempre, pulsa el ícono de papelera rellena y luego **Eliminar definitivamente** (o **Cancelar**). **Esta acción no se puede deshacer.**

> ⚠ Por confirmar: la eliminación definitiva de un proyecto que ya tiene información en otros módulos (cronograma, informes, costos) puede ser rechazada por la base de datos. En ese caso la pantalla muestra un error general. Validar con el equipo el procedimiento (normalmente basta con dejarlo en la papelera).

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| Al ingresar aparece "Correo o contraseña incorrectos. Intenta de nuevo." | Correo o contraseña mal escritos, o el usuario no existe. | Revisa mayúsculas y espacios. Si persiste, pide al administrador que verifique tu usuario o te asigne una contraseña nueva. |
| Al pulsar **Ingresar** aparece un error HTTP 400 (sobre todo después de una actualización de la app) | El navegador guardó una cookie de seguridad de una sesión anterior. | Borra las cookies del sitio o abre una ventana de incógnito e ingresa de nuevo. |
| En Nuevo proyecto aparece "No se encontró la hoja 'Ejecutivo' con datos válidos en el archivo…" | El Excel no es una BOM de oferta o la hoja *Ejecutivo* tiene otro nombre o está vacía. | Verifica que sea la BOM correcta. Si no la tienes, digita los datos a mano. Las líneas BOM se pueden importar después en Costos. |
| Aparece "Error al leer la BOM: …" | Archivo dañado, protegido con contraseña, abierto en otro programa o de más de 60 MB. | Cierra el archivo, guárdalo de nuevo como .xlsx sin contraseña y vuelve a cargarlo. |
| Aviso "⚠ Proyecto posiblemente repetido" o "⚠ Proyecto repetido" | Ya existe un proyecto (activo o en la papelera) con el mismo código o nombre. | Revisa la lista y la papelera. Si es el mismo proyecto, restáuralo o úsalo. Si es otro, corrige el código o el nombre. Crea el duplicado solo si estás seguro. |
| El formulario no guarda y muestra mensajes rojos ("El código es obligatorio", "La capacidad debe ser mayor a 0", "El presupuesto debe ser mayor a 0", "Máximo 20 caracteres") | Falta un campo obligatorio (*) o hay un valor inválido. | Completa Código (máx. 20 caracteres), Nombre (máx. 200), Cliente, Ubicación y País, y pon Capacidad y Presupuesto mayores que 0. |
| Un proyecto no aparece en "Proyectos Activos" del Centro de Control | Su estado no es *En Ejecución*. | Edita el proyecto y cambia el **Estado** a En Ejecución. |
| Un proyecto sale como "Sin datos" o con SPI "—" | No tiene avances registrados en Informes Diarios. | Registra el primer Informe Diario con avances (capítulo 3). |
| El Centro de Control muestra "No se pudo cargar el resumen ejecutivo. Intenta recargar la página." | Falla temporal al calcular los indicadores. | Recarga la página. Si persiste, informa al administrador. |
| El menú lateral dice "No se pudieron cargar proyectos" | Falla temporal de conexión con la base de datos. | Recarga la página. Si persiste, informa al administrador. |
| No encuentro un proyecto en la lista | Está en la papelera. | Entra a **Papelera** y pulsa **Restaurar**. |

---

## Relación con otros módulos

- **Cronograma de Actividades (cap. 2):** la plantilla EPC y las importaciones de Excel y PDF usan la **Fecha inicio planeada** del proyecto como punto de arranque.
- **Informe Diario (cap. 3):** sus avances alimentan el avance real, el SPI, las actividades atrasadas y la clasificación del Centro de Control.
- **Costos (cap. 5):** la BOM cargada al crear el proyecto llena **BOM vs Real** y el presupuesto inicial por rubro, y la TRM de la BOM queda como tasa de cambio del proyecto. La Ficha de la Oferta (venta, costo, margen) sirve de referencia para el Consolidado.
- **Histogramas (cap. 6):** al crear el proyecto (o al cambiar su categoría de capacidad) se generan las plantillas de personal y equipos.
- **Dashboard del proyecto (cap. 14):** se abre desde la tabla del Centro de Control o desde el detalle del proyecto.
- **Todos los módulos** cuelgan del proyecto: se accede a ellos desde el menú lateral o desde **Módulos del proyecto**.

---

## Buenas prácticas

- **Crea siempre el proyecto con la BOM de la oferta.** Ahorra digitación y deja listos BOM vs Real, el presupuesto por rubro y la TRM.
- **Revisa bien la Fecha inicio planeada antes de guardar**, porque el cronograma plantilla se ubica a partir de ella.
- **Mantén el Estado al día.** Pasa el proyecto a *En Ejecución* cuando arranque y a *Completado* o *Cancelado* al cerrar, para que el Centro de Control refleje la realidad.
- **Registra las fechas reales** de inicio y fin en **Editar** cuando ocurran.
- **No crees proyectos duplicados:** un proyecto repetido confunde al personal de campo y distorsiona los reportes. Si aparece el aviso, investiga primero.
- **Usa la papelera en lugar de eliminar definitivamente.** Restaurar es inmediato; borrar no tiene vuelta atrás.
- **Usa códigos cortos y consistentes** (máx. 20 caracteres), iguales a los de la BOM y la tesorería.
- **Cierra sesión con Salir** en equipos compartidos y no marques *Recordarme* en ellos.
