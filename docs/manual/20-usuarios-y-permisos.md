# 20. Usuarios y permisos

> **Para qué sirve:** crear los usuarios de la plataforma y decidir qué ve y qué puede hacer cada persona: en qué proyectos participa y a qué módulos de cada proyecto entra.
> **Quién lo usa:** solo los administradores de la plataforma.
> **Dónde está:** menú lateral → **Configuración** (`/configuracion/usuarios`). El ítem solo aparece para los administradores.

---

## Conceptos clave

| Término | Qué significa |
|---|---|
| **Usuario** | Persona que ingresa a RenergeIA con su **correo y contraseña**. La contraseña la define el administrador y queda fija hasta que se cambie. |
| **Administrador** | Ve todos los proyectos y módulos y es el único que entra a Configuración, crea, edita o elimina proyectos y abre la papelera. |
| **Proyectos asignados** | Los proyectos en los que participa la persona. Los demás no le aparecen en el menú, el inicio ni la lista de proyectos. |
| **Nivel de acceso** | Por cada módulo: **Sin acceso**, **Ver** (consulta), **Editar** (crear y modificar) o **Aprobar** (además aprueba informes diarios y valida documentos). Cada nivel incluye al anterior. |
| **Módulos de proyecto** | Dashboard, Informe Diario, Cronograma, Documentos, Costos, HSEQ Seguridad, HSEQ Calidad, HSEQ Ambiental, HSEQ Social, Restricciones, Histogramas, Clima y Alertas. Se asignan **por proyecto**. |
| **Módulos corporativos** | Compromisos (actas), Helvis y HSEQ corporativo (auditorías e inspecciones generales). No dependen de un proyecto. |
| **Perfil** | Plantilla de niveles (por ejemplo Residente de obra o HSE de obra) que precarga la matriz. Después cada persona se puede ajustar sin cambiar el perfil. |
| **Sin configurar** | Usuario creado antes de que existiera el panel. **Conserva el acceso total** hasta que un administrador guarde sus permisos. |
| **Bitácora** | Registro de quién dio o quitó accesos y cuándo. |

---

## Cómo funciona

- Cada página revisa el permiso **en el servidor antes de mostrarse**. Si alguien escribe la dirección a mano, ve "Sin acceso a esta sección".
- Las páginas de **crear o editar** (nuevo informe, editar actividad, crear documento…) exigen nivel **Editar**.
- **Aprobar o rechazar** un informe diario y **validar** un documento exigen nivel **Aprobar**. Con Ver o Editar esos botones no aparecen.
- La portada **HSEQ** del proyecto se ve si la persona tiene al menos un área HSEQ, y solo muestra las áreas asignadas.
- En el **Inicio** (Centro de control) aparecen solo los proyectos asignados. Si en un proyecto la persona no tiene **Costos**, sus cifras de dinero no se muestran: el proyecto aparece como "sin costos cargados".
- Un usuario **desactivado** no puede ingresar, y si tenía una sesión abierta queda bloqueado en menos de un minuto.
- Los cambios de permisos se aplican en menos de un minuto. Si la persona no los ve, que recargue la página.
- Tras **5 intentos fallidos** de contraseña el usuario se bloquea unos minutos.

---

## Paso a paso

### Crear un usuario

1. **Configuración → Usuarios → Nuevo usuario**.
2. Escribe el nombre, el cargo, el correo y la contraseña (mínimo 8 caracteres, con al menos una mayúscula y un número).
3. Si es administrador, marca **Administrador**. Ya no necesita matriz.
4. Si no lo es, elige un **Perfil** (opcional) y marca sus **proyectos**. Cada proyecto marcado arranca con los niveles del perfil.
5. Ajusta la **matriz**. El selector de la fila **Todos** cambia la columna completa.
6. Elige los **módulos corporativos** y pulsa **Guardar**.
7. Entrega a la persona su correo y su contraseña por un canal privado.

### Cambiar los permisos de alguien

Abre el lápiz junto a su nombre, cambia lo necesario y guarda. Para pasarle otro perfil completo, elige el perfil y pulsa **Aplicar perfil a todos sus proyectos**. Esto reemplaza la matriz.

### Cambiar una contraseña

Abre el usuario, escribe la nueva en **Nueva contraseña** y guarda. Si dejas el campo vacío, la contraseña no cambia.

### Desactivar a alguien

Abre el usuario, desmarca **Activo** y guarda. No se borra: su historial (informes, aprobaciones) se conserva. Puedes activarlo de nuevo cuando quieras.

### Perfiles

En la pestaña **Perfiles** puedes crear los sugeridos (Gerente de proyecto, Residente de obra, HSE de obra, Control de costos y Consulta gerencia) o uno propio. Editar o borrar un perfil **no cambia** los permisos de quienes ya lo usaron.

---

## Qué hacer en caso de…

| Situación | Qué hacer |
|---|---|
| Alguien dice que no ve un proyecto o módulo | Revisa que el proyecto esté marcado y que el módulo tenga al menos **Ver**. Pídele que recargue la página. |
| Ve el módulo pero no le aparece Aprobar o Validar | Necesita nivel **Aprobar** en ese módulo y proyecto. |
| Aparece "Sin acceso a esta sección" al crear algo | Tiene **Ver** y necesita **Editar**. |
| Un usuario sale como "Sin configurar · acceso total" | Ábrelo, asigna sus proyectos y módulos, y guarda. |
| No puedo quitarme el rol de administrador o desactivarme | Es a propósito, para que la plataforma no quede sin administrador. Debe hacerlo otro administrador. |
| "Demasiados intentos fallidos" al ingresar | Espera unos minutos. Si olvidó la contraseña, un administrador le pone una nueva. |

---

## Relación con otros módulos

- **Menú lateral, Inicio ([18](18-centro-de-control.md)) y Proyectos:** se filtran con estos permisos.
- **Informe Diario ([3](03-informe-diario.md)) y Documentos ([4](04-documentos.md)):** usan el nivel Aprobar para aprobar o rechazar informes y validar documentos.
- **Helvis ([19](19-helvis-whatsapp.md)):** sus contactos de WhatsApp tienen permisos propios por celular, separados de los de la página web.

---

## Buenas prácticas

- Da el **mínimo** acceso necesario y súbelo cuando haga falta.
- Usa perfiles para que personas con el mismo cargo tengan los mismos accesos.
- Desactiva a quien sale de la empresa el mismo día; no compartas usuarios entre personas.
- Revisa la **Bitácora** de vez en cuando.
