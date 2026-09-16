# 16. Problemas frecuentes (generales)

> Soluciones a fallas que no son de un módulo en particular. Para problemas de un módulo, revisa la sección **Qué hacer en caso de…** de su capítulo.

---

| Situación | Causa probable | Qué hacer |
|---|---|---|
| Error **HTTP 400** al iniciar sesión | El navegador guardó datos de una sesión anterior a una actualización de la aplicación | Borra las cookies del sitio o abre una ventana de incógnito e inicia sesión de nuevo. |
| La página se queda "Cargando…" o aparece un aviso de reconexión | Se perdió la conexión con el servidor (internet inestable o la aplicación se acaba de actualizar) | Espera unos segundos; si no se recupera, recarga la página (F5). Lo que no se haya guardado se pierde. |
| Los cambios que hizo otra persona no se ven | La pantalla muestra lo que se cargó al abrirla | Recarga la página (F5). |
| El análisis con IA falla (mensaje con **429** o **RESOURCE_EXHAUSTED**) | Se agotó la cuota gratuita del servicio de IA (Google Gemini) o falta habilitar la facturación | Intenta más tarde. Si persiste, el administrador debe habilitar la facturación del proyecto en Google Cloud. |
| Un Excel no carga o carga 0 registros | El archivo no tiene el formato esperado (hojas o encabezados renombrados) | Verifica el formato descrito en el capítulo del módulo. No cambies nombres de hojas ni títulos de columnas. |
| Aviso de archivo **desactualizado** al importar | El archivo es más antiguo que el último cargado | Confirma que tienes la versión más reciente antes de continuar. |
| No veo un proyecto | Fue enviado a la **Papelera** | Revisa la Papelera de proyectos y restáuralo (capítulo 1). |
| Una exportación a PDF no se descarga | El informe se abre en otra pestaña para imprimir; el navegador puede bloquear ventanas emergentes | Permite ventanas emergentes para el sitio y usa **Imprimir → Guardar como PDF**. |

---

## Antes de reportar un problema

Anota y envía al equipo de soporte:

1. **Módulo y pantalla** (copia la dirección web de la barra del navegador).
2. **Qué hiciste** paso a paso y **qué esperabas** que pasara.
3. **Mensaje exacto** que apareció (captura de pantalla).
4. **Archivo** que estabas cargando, si aplica.
5. Fecha y hora aproximada.
