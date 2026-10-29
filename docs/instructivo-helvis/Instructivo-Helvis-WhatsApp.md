# Instructivo — Helvis, el asistente de WhatsApp de RenergeIA

> **Para qué sirve:** explicar qué es Helvis, quién puede usarlo, qué responde hoy, cómo se administra desde RenergeIA y qué hacer si algo falla, incluidos los problemas con Meta (WhatsApp).
> **Quién lo usa:** el equipo de proyectos que consulta por WhatsApp y las personas que administran Helvis en RenergeIA.
> **Responsable de la gestión:** Renergeia. La administración de Helvis, del número y de la cuenta de Meta es **gestión interna**; no depende de proveedores externos.
> **Versión:** 1.1 — 29 de octubre de 2026.

---

## 1. Qué es Helvis

**Helvis** es el asistente de RenergeIA en WhatsApp. Permite que el equipo de obra y de gestión consulte la información de sus proyectos desde el celular, sin abrir un computador.

- Responde **siempre con la información de RenergeIA**: los mismos datos que se ven en la plataforma (Curva S, cronograma, planificación documental).
- Funciona con **menús y botones**. Las respuestas las arma la plataforma con reglas fijas, no una inteligencia artificial que improvisa; por eso son predecibles y se pueden auditar.
- **Reconoce a cada persona por su número de celular.** Si el número no está registrado en RenergeIA, Helvis no entrega ninguna información.
- **Todo queda guardado:** cada mensaje que entra y cada respuesta que sale se registra en RenergeIA, con fecha, hora y si se entregó o falló.

### Cómo funciona por dentro (resumen)

| Paso | Qué pasa |
|---|---|
| 1 | La persona escribe al número de Helvis desde su WhatsApp. |
| 2 | Meta (la empresa dueña de WhatsApp) entrega el mensaje a RenergeIA. |
| 3 | RenergeIA busca el celular entre los contactos registrados y revisa sus permisos y proyectos. |
| 4 | RenergeIA arma la respuesta con los datos del proyecto y la envía de vuelta por Meta. |
| 5 | Mensaje y respuesta quedan guardados en el panel **Helvis → Conversaciones**. |

RenergeIA se conecta **directamente** con la plataforma oficial de WhatsApp de Meta (WhatsApp Cloud API), sin intermediarios: solo se pagan las tarifas de Meta.

---

## 2. Estado actual (octubre de 2026)

| Tema | Estado |
|---|---|
| Conexión con WhatsApp | ✅ Funcionando de punta a punta desde el 20/10/2026. |
| Reconocimiento por celular, permisos y proyectos | ✅ En producción (Etapa 1, verificada el 22/10/2026). |
| Ajustes de conversación (saludo una vez al día, avisos de fotos y notas de voz, aviso a administradores por números desconocidos) | ✅ En producción desde el 29/10/2026. |
| Selector de proyecto y menú (avance, alertas, actividades asignadas) | ✅ En producción desde el 27/10/2026. |
| Registro de avance por WhatsApp (informe diario conversacional) | ⏳ Etapa 2 — en preparación. Hoy Helvis **muestra** las actividades asignadas, pero todavía **no recibe** reportes de avance. |
| Mensajes automáticos (recordatorios, avisos por horario) | ⛔ Apagados por diseño. Helvis **solo responde**; la única excepción es el aviso a los administradores cuando escribe un número no registrado. |
| Número de WhatsApp | ⚠ **Número de prueba** de Meta (+1 555-305-7165). Solo pueden escribirle hasta 5 celulares autorizados. Falta conectar el **número real** de Renergeia (ver sección 8). |

Mientras se use el número de prueba, los mensajes de Helvis dicen *"el asistente de Renergeia (prueba)"*.

---

## 3. Quién puede usar Helvis

1. La persona debe estar **registrada como contacto** en RenergeIA (panel **Helvis → Contactos**) con su celular, y estar **Activa**.
2. Debe tener **proyectos asignados**: solo ve la información de esos proyectos.
3. Sus **permisos** definen qué puede hacer:

| Permiso | Para qué sirve hoy |
|---|---|
| **Consultar información** | Ver avance, alertas y actividades de sus proyectos. Viene marcado por defecto. **Sin este permiso Helvis no entrega información**, aunque la persona esté registrada. |
| **Reportar avances** | Preparado para la Etapa 2 (registrar avance por WhatsApp). Se puede asignar desde ya. |
| **Corregir avances** | Preparado para la Etapa 2. |
| **Aprobar informes** | Preparado para la etapa del informe diario consolidado. |
| **Administrador de Helvis** | Identifica a quienes administran el asistente. Reciben por WhatsApp el aviso cuando escribe un número no registrado. |

> Un permiso no incluye a los otros: tener **Consultar** no da derecho a reportar ni a aprobar. Los permisos **solo** se asignan en RenergeIA; Helvis nunca los concede porque alguien diga un nombre o un cargo en el chat.

---

## 4. Cómo se usa (persona registrada)

### 4.1 Iniciar

1. Guarda el número de Helvis en tus contactos.
2. Escríbele cualquier mensaje, por ejemplo **hola**.
3. Helvis te saluda por tu nombre (el registrado en RenergeIA). El saludo aparece solo en el **primer mensaje del día**; después responde directo.

### 4.2 Elegir el proyecto

- **Si tienes un solo proyecto**, Helvis lo elige solo y te muestra el menú.
- **Si tienes 2 o 3 proyectos**, aparecen como **botones** con el código de cada proyecto: pulsa uno.
- **Si tienes 4 o más**, Helvis envía una lista numerada: responde con el **número**.
- Si pediste algo (por ejemplo *avance*) antes de elegir el proyecto, Helvis lo recuerda y te lo muestra apenas elijas el proyecto.
- Helvis **recuerda el proyecto elegido durante 12 horas**. Para cambiarlo antes, escribe **proyecto**.

### 4.3 Menú del proyecto

Aparecen tres botones:

| Botón | Qué responde |
|---|---|
| **📊 Ver avance** | Avance **programado** y **real** a hoy, la **brecha** en puntos porcentuales con un indicador (✅ al día · ⚠️ atraso leve · 🔴 atraso mayor a 5 puntos) y las 3 actividades más atrasadas: primero las que tienes asignadas; si no tienes, las del proyecto. Son los mismos números de la Curva S del Dashboard. |
| **📋 Alertas** | Documentos de la planificación documental que necesitan atención: cuántos están en **rojo** (8 días o más sin atender), en **amarillo** (4 a 7 días) y en total, con los 5 más urgentes, sus días y **en cancha de** quién están (Renergeia o Cliente). |
| **📝 Informe diario** | Tus **actividades asignadas** en ese proyecto con su avance actual. El registro de avance por WhatsApp llega en la Etapa 2. |

También puedes escribir en lugar de pulsar botones (sin importar mayúsculas ni tildes):

| Escribe | Equivale a |
|---|---|
| **1**, *avance*, *consulta* | Ver avance |
| **2**, *alertas*, *vencimientos*, *pendientes* | Alertas |
| **3**, *informe*, *reporte*, *diario* | Informe diario |
| *proyecto*, *cambiar* | Cambiar de proyecto |
| *costo*, *presupuesto*, *pago* | Aviso de que la consulta de costos por WhatsApp aún no está disponible |
| **menú** o cualquier otro texto | Vuelve a mostrar el menú |

### 4.4 Lo que Helvis todavía no guarda

| Si envías… | Helvis responde |
|---|---|
| Una **foto, video o documento** | Que lo recibió pero **no queda guardado**; las fotos van por ahora en el Informe diario de la app. |
| Una **nota de voz** | Que aún no puede escucharlas y te pide escribirlo. |
| Un texto que **parece un reporte de obra** (ej. *hincamos 120 pilotes en la mesa 4*) | Que el reporte por WhatsApp aún no está activo y que **no quedó guardado** como avance; el avance se registra en el Informe diario de la app. |

### 4.5 Números no registrados

Si alguien escribe desde un celular que no está en RenergeIA (o que está inactivo):

- Helvis responde con cortesía, se presenta y pregunta **qué necesita y con quién quiere comunicarse**.
- **No entrega ninguna información** de proyectos ni datos del equipo.
- Si sigue escribiendo, responde *"quedó registrado tu mensaje"* (la bienvenida se repite como máximo una vez cada 24 horas).
- Todo lo que escriba queda en **Helvis → Conversaciones**.
- Helvis **avisa por WhatsApp a los contactos marcados como Administrador de Helvis** con el número y el mensaje. Si ese administrador no le ha escrito a Helvis en las últimas 24 horas, Meta puede rechazar el aviso (queda como *fallo* en Conversaciones); por eso conviene revisar el panel igualmente.

---

## 5. Cómo se administra (panel Helvis en RenergeIA)

Menú lateral → **Helvis**. Tiene tres pestañas.

### 5.1 Contactos — registrar a una persona

1. Pulsa **Nuevo contacto**.
2. Llena **Nombre** y **Celular** (obligatorios), **Cargo** y **Correo**.
   - El celular se guarda solo con dígitos y con indicativo de país. Si escribes un celular colombiano de 10 dígitos que empieza por 3, RenergeIA le agrega el **57** automáticamente (ej. 3187222977 → 573187222977).
3. Marca los **Proyectos que puede ver**.
4. Marca **Qué puede hacer** (permisos de la sección 3).
5. Deja **Activo** marcado.
6. Opcional: **En pausa hasta** y **Motivo de la pausa** (ej. vacaciones) —se tendrá en cuenta cuando existan mensajes automáticos— y **Notas**.
7. Pulsa **Guardar**.

Para dar de baja a alguien **sin perder su historial**, edítalo y desmarca **Activo** (en lugar de eliminarlo).

### 5.2 Conversaciones — revisar lo que se habló

- A la izquierda, la lista de números con su último mensaje y el total.
- Al seleccionar uno, el hilo completo tipo chat, en **hora Colombia**, con el estado de cada respuesta (**enviado** o **fallo**).
- Úsala para: atender números desconocidos, comprobar qué se le respondió a alguien y detectar envíos fallidos.

### 5.3 Asignaciones — actividades por responsable

1. Elige el **Responsable** y el **Proyecto**.
2. Aparecen las actividades hoja del **cronograma vigente** con fechas, avance, frente y a quién más están asignadas (**También asignada a**).
3. Marca las actividades (o usa **✓ Asignar todas** / **✕ Quitar todas**) y, si aplica, escribe el **Frente**.
4. Pulsa **Guardar**.

Avisos que puede mostrar:
- *No tiene el permiso Reportar avances*: se le pueden asignar actividades, pero cuando llegue la Etapa 2 no podrá reportarlas hasta activarle ese permiso.
- *Asignaciones que apuntan a una versión anterior del cronograma*: pasa tras una reprogramación. Pulsa **Desactivarlas** y vuelve a asignar en la versión vigente.

---

## 6. Reglas de WhatsApp (Meta) que hay que conocer

| Regla | Qué significa para nosotros |
|---|---|
| **Ventana de 24 horas** | Helvis solo puede responder libremente dentro de las 24 horas siguientes al último mensaje de la persona. Para escribirle a alguien que no ha escrito, Meta exige **plantillas aprobadas** previamente (aún no creadas). Por eso hoy Helvis solo responde. |
| **Número de prueba** | Solo acepta mensajes de **máximo 5 celulares** previamente autorizados. Los mensajes de cualquier otro celular **se descartan en silencio**: no llegan a RenergeIA y no quedan en Conversaciones. |
| **Costos** | Desde el 01/10/2026 Meta cobra los mensajes, con **1.000 mensajes de servicio gratis al mes por número**. Tarifas de referencia para Colombia: utilidad/servicio US$ 0,0008 por mensaje; marketing US$ 0,0125. |
| **Seguridad de la cuenta** | Meta exige que **cada persona** que administra el Portfolio comercial de Renergeia tenga activada una **llave de acceso (passkey)** en su cuenta. |
| **Nombre visible** | El nombre con el que aparece el número debe estar aprobado por Meta; si no, Meta puede rechazar envíos de forma intermitente. |

---

## 7. Qué hacer en caso de…

### 7.1 Problemas de uso

| Situación | Causa probable | Qué hacer |
|---|---|---|
| Escribo a Helvis y **no responde nada** | Con el número de prueba, tu celular no está entre los 5 autorizados (Meta descarta el mensaje). | Confirma con la persona administradora si tu celular está autorizado. Con el número real esto desaparece. |
| Helvis responde *"Este número aún no está registrado conmigo"* | Tu celular no está en **Contactos**, está mal escrito o el contacto está **inactivo**. | La persona administradora revisa el celular (con indicativo 57) y que esté **Activo**. |
| *"Aún no tienes proyectos asignados"* | El contacto no tiene proyectos marcados. | Asignar proyectos en **Contactos → Editar**. |
| No aparece un proyecto que sí manejo | No está marcado en **Proyectos que puede ver**, o el proyecto está en la papelera de RenergeIA. | Marcarlo en el contacto; si está en la papelera, restaurarlo en Proyectos. |
| *"Aún no tienes actividades asignadas en este proyecto"* | No hay asignaciones en ese proyecto. | Asignarlas en **Helvis → Asignaciones**. |
| El avance de Helvis no coincide con el Dashboard | Se consultaron en momentos distintos o el cronograma cambió entre consultas. | Ambos usan el mismo cálculo (Curva S). Vuelve a consultar; si persiste, revisar la versión vigente del cronograma. |
| *"Este número está registrado, pero aún no tiene habilitada la consulta"* | El contacto no tiene el permiso **Consultar información**. | Activarlo en **Contactos → Editar**. |
| Envié una foto o un reporte y *no quedó guardado* | El registro por WhatsApp llega en la Etapa 2. | Registrar el avance y las fotos en el **Informe diario** de la app. |
| El administrador no recibió el aviso de un número desconocido | El administrador no le había escrito a Helvis en 24 horas (regla de Meta). | Revisar **Helvis → Conversaciones**; se resolverá con una plantilla aprobada. |
| Helvis siempre me devuelve el menú | El texto no corresponde a ninguna opción. | Usa los botones o escribe 1, 2, 3 o *proyecto*. |
| Quiero cambiar de proyecto | Helvis recuerda el último durante 12 horas. | Escribe **proyecto**. |
| Cambió mi número de celular | Helvis reconoce por número. | La persona administradora actualiza el **Celular** del contacto; el historial anterior se conserva. |

### 7.2 Problemas con Meta o con el servicio

| Situación | Cómo se reconoce | Qué hacer |
|---|---|---|
| **Helvis no responde a nadie, pero los mensajes sí aparecen en Conversaciones con estado *fallo*** | Los mensajes entran, las respuestas fallan. Casi siempre es el **token de acceso** de Meta vencido o revocado (error 401 / código 190). | 1) En **business.facebook.com → Configuración del negocio → Usuarios → Usuarios del sistema**, elegir el usuario del sistema del bot → **Generar token** → app del bot → caducidad **Nunca** → permisos *whatsapp_business_messaging* y *whatsapp_business_management*. 2) Cargar el nuevo token en el servidor de RenergeIA (variable `WhatsApp__AccessToken`, con `--update-env-vars`; ver `GUIA_DE_DESARROLLO.md` §33). El token se muestra una sola vez: guardarlo en un lugar seguro y **nunca** enviarlo por chat ni correo. |
| **Algunas respuestas fallan y otras no** | Rechazos intermitentes de Meta (visto el 27/10/2026 con el error del nombre visible). | RenergeIA ya **reintenta automáticamente 3 veces**. Si se repite, revisar en **WhatsApp Manager** el estado del número y del **nombre visible** (que esté aprobado). |
| **Ningún mensaje llega a Conversaciones** (ni de personas registradas) | Meta no está entregando al servidor: suscripción del webhook caída, número desconectado o servidor detenido. | 1) Revisar que la plataforma RenergeIA abra normalmente. 2) En **WhatsApp Manager**, verificar que el número aparezca **Conectado** y sin restricciones. 3) Si todo está bien, el área técnica revisa la suscripción del webhook (pasos en `GUIA_DE_DESARROLLO.md` §33; **no** volver a suscribir si ya está activa). |
| **Meta pide "Passkey required" / "La llave de acceso no está activada"** | Una persona del Portfolio comercial no tiene passkey. Meta bloquea administrar el número. | Cada persona activa su propia passkey: **Centro de cuentas → Contraseña y seguridad → Autenticación en dos pasos → elegir la cuenta → Llave de acceso**, y confirma con huella, rostro o PIN. Debe hacerse en **la misma cuenta** que Meta marca en rojo; si el PC no lo ofrece, hacerlo desde la app de Facebook/Instagram en el celular. |
| **El número quedó restringido, en revisión o con calidad baja** | Aviso en WhatsApp Manager (calidad roja, límite de mensajes, número bloqueado). | No forzar envíos. Revisar el motivo en **WhatsApp Manager → Calidad / Estado**; apelar desde ahí si corresponde. Helvis solo responde a quien escribe, lo que reduce el riesgo. |
| **Meta pide verificar el negocio o actualizar datos** | Aviso en Business Settings. | Lo atiende la persona administradora del Portfolio comercial con los documentos de Renergeia. |
| **Llega un cobro o aviso de facturación de Meta** | Consumo por encima de los 1.000 mensajes de servicio gratis del mes. | Revisar en **WhatsApp Manager → Información (Insights)** el volumen; cualquier cambio de método de pago lo decide gerencia. |
| **Se sospecha que un token se filtró** (se pegó en un chat, correo o documento) | — | Inmediatamente: **Usuarios del sistema → Revocar tokens**, generar uno nuevo y cargarlo en el servidor (fila 1). |

> **Ruta de escalamiento interna:** 1) persona administradora de Helvis (revisa Contactos y Conversaciones) → 2) persona administradora del Portfolio comercial de Meta (número, tokens, passkeys) → 3) área técnica de RenergeIA (servidor y webhook). La gestión es **interna de Renergeia**.

---

## 8. Pendientes para pasar de prueba a operación

| # | Pendiente | Quién | Por qué importa |
|---|---|---|---|
| 1 | **Conectar el número real** de Renergeia en *Business Settings → Cuentas de WhatsApp → Agregar número* (requiere una SIM que pueda recibir el código por SMS o llamada). | Administración del Portfolio | Desaparece el límite de 5 celulares y cualquier persona del equipo puede escribirle a Helvis. |
| 2 | **Dejar la app de Meta administrada solo por cuentas de Renergeia** (el App Secret y el acceso al portal de desarrolladores). | Administración del Portfolio | Hoy la app fue creada con una cuenta externa; para que la gestión sea 100 % interna, Renergeia debe tener el control total. ⚠ Por confirmar el procedimiento con Meta. |
| 3 | **Registrar a todo el equipo** en Contactos con sus celulares y proyectos. | Administración de Helvis | Sin registro no hay información. |
| 4 | **Cantidades y unidades del cronograma** (cantidad total y unidad por actividad). | Planeación | Necesarias para que en la Etapa 2 se pueda reportar "50 de 200 hincas" y no solo porcentajes. |
| 5 | **Plantillas aprobadas** por Meta (recordatorios, avisos). | Administración del Portfolio | Para que Helvis pueda escribir primero (fuera de la ventana de 24 h). |
| 6 | **Etapa 2: reporte de avance por WhatsApp** con resumen y confirmación antes de guardar. | Desarrollo | El objetivo final: que el informe diario se alimente desde el celular. |

---

## 9. Buenas prácticas

- Registra a cada persona con **su celular personal** y solo los proyectos que necesita ver.
- Da de baja con **Activo** desmarcado, no eliminando: así se conserva el historial.
- Revisa **Conversaciones** al menos una vez por semana para atender números desconocidos y respuestas fallidas.
- Después de una **reprogramación del cronograma**, revisa las **Asignaciones** (aviso de versión anterior).
- **Nunca** compartas tokens, el App Secret ni códigos de verificación por WhatsApp, correo o documentos.
- Ante un problema con Meta, revisa primero **Conversaciones**: si los mensajes entran y las respuestas fallan, es el token; si no entra nada, es la conexión con Meta.
