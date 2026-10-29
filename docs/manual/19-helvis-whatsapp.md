# 19. Helvis — asistente de WhatsApp

> **Para qué sirve:** consultar desde WhatsApp el avance, las alertas documentales y las actividades asignadas de cada proyecto, y administrar quién puede hacerlo.
> **Quién lo usa:** el equipo de obra y de gestión (consulta por WhatsApp) y las personas administradoras de Helvis (panel en RenergeIA).
> **Dónde está:** menú lateral → **Helvis** (`/helvis`). El detalle completo está en el **Instructivo de Helvis** (`docs/instructivo-helvis/Instructivo-Helvis-WhatsApp.md`).

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Helvis** | Asistente de RenergeIA en WhatsApp. Responde con menús y botones a partir de los datos de la plataforma. |
| **Contacto** | Persona registrada con su celular. **El celular es su identidad**: sin registro, Helvis no entrega información. |
| **Permisos** | Consultar información, Reportar avances, Corregir avances, Aprobar informes y Administrador de Helvis. Son independientes. |
| **Proyecto activo** | Proyecto sobre el que se está conversando; Helvis lo recuerda 12 horas. |
| **Asignación** | Actividad hoja del cronograma vigente asignada a un responsable (y opcionalmente a un frente). |
| **Número de prueba** | Número temporal de Meta que solo admite 5 celulares autorizados. Se reemplazará por el número real de Renergeia. |

---

## Cómo funciona

- La persona escribe al número de Helvis; RenergeIA la reconoce por su celular, le pregunta sobre qué proyecto trabaja (si tiene varios) y le muestra el menú: **📊 Ver avance**, **📋 Alertas** y **📝 Informe diario**.
- **Ver avance** usa el mismo cálculo de la Curva S del Dashboard ([14](14-dashboard-proyecto.md)); **Alertas** usa los semáforos de Documentos ([4](04-documentos.md)); **Informe diario** muestra las actividades asignadas (el registro de avance por WhatsApp llega en la Etapa 2).
- Saluda por el nombre solo en el primer mensaje del día. Sin el permiso **Consultar información** no entrega datos.
- Fotos, notas de voz y textos que parecen reportes de obra reciben un aviso de que **todavía no se guardan** (llega en la Etapa 2).
- Números no registrados reciben una respuesta cordial sin información; lo que escriban queda en **Conversaciones** y Helvis avisa por WhatsApp a los contactos **Administrador de Helvis**.
- Fuera de ese aviso, Helvis **no envía mensajes automáticos**: solo responde.
- Todo mensaje entrante y saliente queda guardado.

---

## Paso a paso

### Registrar a una persona
1. **Helvis → Contactos → Nuevo contacto**.
2. Nombre, Celular (un celular colombiano de 10 dígitos recibe el 57 automáticamente), Cargo y Correo.
3. Marca **Proyectos que puede ver** y **Qué puede hacer**.
4. **Guardar**.

### Asignar actividades
1. **Helvis → Asignaciones**, elige **Responsable** y **Proyecto**.
2. Marca las actividades (o **✓ Asignar todas**) y escribe el **Frente** si aplica.
3. **Guardar**.

### Revisar conversaciones
1. **Helvis → Conversaciones**, elige el número.
2. Lee el hilo (hora Colombia) y el estado de cada respuesta (**enviado** / **fallo**).

---

## Qué hacer en caso de…

| Situación | Causa probable | Qué hacer |
|---|---|---|
| Helvis no responde a un celular | Con el número de prueba, ese celular no está autorizado. | Ver el Instructivo de Helvis, sección 7. |
| *"Este número aún no está registrado conmigo"* | Celular sin registrar, mal escrito o inactivo. | Revisar el contacto en **Contactos**. |
| Respuestas con estado **fallo** en Conversaciones | Token de Meta vencido o rechazo de Meta. | Ver el Instructivo de Helvis, sección 7.2. |
| Aviso de asignaciones en una versión anterior | Hubo reprogramación del cronograma. | **Desactivarlas** y asignar de nuevo en la versión vigente. |

---

## Relación con otros módulos

- **Cronograma de Actividades** ([2](02-cronograma-actividades.md)): de aquí salen las actividades que se asignan.
- **Dashboard** ([14](14-dashboard-proyecto.md)) y **Centro de control** ([18](18-centro-de-control.md)): mismo cálculo de avance.
- **Documentos** ([4](04-documentos.md)): fuente de las alertas.
- **Informe Diario** ([3](03-informe-diario.md)): en la Etapa 2 se alimentará desde WhatsApp.

---

## Buenas prácticas

- Desactiva contactos en lugar de eliminarlos para conservar su historial.
- Revisa **Conversaciones** cada semana.
- Nunca compartas tokens ni códigos de Meta por chat o correo.
