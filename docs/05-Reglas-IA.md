# Reglas de IA — RenergeIA
**Versión:** 1.0 | **Fecha:** 2026-06-28 | **Estado:** Vigente

> Este documento define cómo RenergeIA integra inteligencia artificial en la plataforma: la estructura del panel de análisis, cómo funciona hoy (Fase 1), cómo evolucionará (Fase 3), y las reglas de diseño para que el panel sea confiable y útil.

---

## Principio fundamental

> La IA en RenergeIA no reemplaza al profesional. Lo amplifica.
> El panel de Análisis Inteligente provee síntesis y recomendaciones, pero la decisión siempre es del usuario.

Esto tiene dos consecuencias prácticas:
1. El panel IA siempre muestra **la fuente del análisis** (qué datos analizó, de qué período).
2. Las recomendaciones son sugerencias, no órdenes. El lenguaje debe reflejar eso ("se recomienda", "considerar", "podría indicar").

---

## El componente: AnalisisIA

Todos los módulos tienen un panel `<AnalisisIA />` al final del dashboard (Nivel 5 de la estructura estándar).

### Ubicación en el código

```
RenergeIA.Web/Components/Shared/Dashboard/AnalisisIA.razor
```

### Parámetros actuales

```razor
[Parameter] public string          Titulo       { get; set; } = "Análisis Inteligente";
[Parameter] public string          Subtitulo    { get; set; } = "Generado por RenergeIA";
[Parameter] public RenderFragment? ChildContent { get; set; }
```

### Aspecto visual

El panel tiene un estilo diferenciado del resto del dashboard para comunicar que su contenido fue generado automáticamente:
- Fondo ligeramente distinto
- Ícono `bi-cpu` en el header
- Título "Análisis Inteligente" siempre visible

---

## Estructura del contenido del panel

Independientemente de si el contenido es simulado (Fase 1) o real (Fase 3), el panel siempre sigue este orden:

### 1. Estado general
Una sola oración que resume el estado del módulo en ese momento.

```
"El módulo de Seguridad presenta indicadores SALUDABLES para el período analizado."
```

### 2. Hallazgos principales
Lista de 3 a 5 observaciones concretas, ordenadas por relevancia.

```
• La tasa de capacitaciones alcanzó el 94%, superando la meta mensual del 90%.
• Se registraron 2 casi-accidentes en la semana 24, un 50% más que la semana anterior.
• 3 inspecciones de alto voltaje están vencidas desde hace más de 7 días.
```

### 3. Alertas activas
Solo ítems que requieren acción inmediata. Si no hay alertas, omitir esta sección.

```
⚠ ALERTA: Permiso de trabajo #PT-2026-041 vence mañana sin cierre formal.
⚠ ALERTA: Trabajador Juan Pérez no completó la inducción SST (día 3 en obra).
```

### 4. Tendencias
Qué está mejorando y qué está empeorando respecto al período anterior.

```
↑ Mejorando: Cumplimiento de EPP pasó de 78% a 91% en el último mes.
↓ Atención: Los incidentes de tipo ergonómico aumentaron 3 casos vs. mes anterior.
```

### 5. Recomendaciones
Máximo 3. Concretas, accionables, con fecha o responsable si aplica.

```
1. Programar cierre de inspecciones vencidas antes del viernes 03/07.
2. Reforzar capacitación en manejo manual de cargas con el grupo de la semana 25.
3. Revisar protocolo de permisos de trabajo con el equipo antes del próximo turno.
```

### 6. Riesgos identificados
Factores que podrían deteriorar los indicadores si no se actúa. No alarmar, informar.

```
• La salida de 2 trabajadores certificados en alturas podría afectar el cumplimiento 
  de la semana 26 si no se reemplaza con personal calificado.
```

---

## Fase 1 — Contenido simulado (estado actual)

En la Fase 1, el contenido del panel IA es **escrito manualmente por el desarrollador** basándose en los datos reales del módulo. No hay conexión a ninguna API de IA.

### Cómo escribir buen contenido simulado

El objetivo del contenido simulado no es engañar. Es:
- **Demostrar** cómo se verá el panel cuando esté conectado a IA real.
- **Dar valor inmediato** a los usuarios que leen los dashboards.
- **Validar** la estructura antes de invertir en la integración real.

**Reglas para el contenido simulado:**

1. Basar el texto en datos reales del módulo, no en datos inventados.
2. Usar el subtítulo `"Basado en datos del período"` para ser transparente.
3. Escribir en tercera persona ("se observa", "se recomienda").
4. No prometer más de lo que el sistema puede detectar actualmente.
5. Actualizar el contenido cuando cambien los datos significativamente.

### Ejemplo completo (módulo Capacitaciones)

```razor
<AnalisisIA Titulo="Análisis Inteligente" Subtitulo="Capacitaciones · Período: junio 2026">
    <p><strong>Estado general:</strong> El módulo de Capacitaciones presenta
    indicadores <span class="text-success fw-bold">SALUDABLES</span> para el mes analizado.</p>

    <p class="fw-bold mt-2 mb-1">Hallazgos principales:</p>
    <ul>
        <li>La cobertura de capacitaciones alcanzó el 94%, superando la meta del 90%.</li>
        <li>El área de trabajo en alturas concentra el 38% de las horas de capacitación.</li>
        <li>3 trabajadores nuevos completaron la inducción SST esta semana.</li>
    </ul>

    <p class="fw-bold mt-2 mb-1">Recomendaciones:</p>
    <ul class="mb-0">
        <li>Programar capacitación de primeros auxilios para el grupo del turno tarde.</li>
        <li>Reforzar el tema de señalización vial antes del inicio de la fase de canalización.</li>
    </ul>
</AnalisisIA>
```

---

## Fase 3 — IA real (planificado)

En la Fase 3, el panel se conectará a **Azure OpenAI** (GPT-4o) o **OpenAI API** para generar el análisis automáticamente a partir de los datos reales de la base de datos.

### Arquitectura planificada

```
Dashboard del módulo
        ↓
IAnalisisIAService.GenerarAnalisisAsync(contexto)
        ↓
Servicio IA (RenergeIA.Web/Services/AnalisisIAService.cs)
        ↓
Azure OpenAI API (GPT-4o)
   └─ Prompt con datos reales del módulo
   └─ Temperature: 0.3 (respuestas precisas, no creativas)
   └─ Max tokens: 600
        ↓
Respuesta estructurada (JSON)
        ↓
<AnalisisIA /> renderiza el contenido
```

### Datos que se enviarán a la IA

El servicio construirá un contexto estructurado con:
- KPIs numéricos del período
- Comparativo vs. período anterior
- Alertas activas del módulo
- Tendencias de los últimos 3 meses
- Metas y umbrales del módulo

### Lo que la IA NO hará

- No tomará decisiones por el usuario.
- No enviará notificaciones automáticas sin confirmación.
- No modificará datos de la base de datos.
- No generará reportes formales (eso es función de QuestPDF, no IA).

---

## Preparación para IA — reglas para nuevos módulos

Todo módulo nuevo debe estar preparado para IA desde su creación:

### 1. Incluir el panel `<AnalisisIA />` desde el inicio

```razor
<AnalisisIA Titulo="Análisis Inteligente" Subtitulo="@($"{_moduloNombre} · {_periodo}")">
    <!-- contenido simulado aquí -->
</AnalisisIA>
```

### 2. Los KPIs deben tener valores calculados, no hardcodeados

La IA solo puede analizar datos reales. Si los valores del dashboard son fijos en el código, la IA no tendrá nada que analizar.

### 3. Exponer los datos del módulo a través del servicio

```csharp
// Ejemplo en CostoService.cs
public async Task<ResumenCostosDto> GetResumenAsync(int proyectoId, int mes, int anio)
{
    // retorna los datos consolidados que el panel IA necesita
}
```

### 4. Nombrar las variables con semántica clara

La IA recibe nombres de variables como parte del prompt. `costoEjecutado`, `presupuestoOriginal`, `desviacionPorcentual` son buenos nombres. `c1`, `val`, `x` no lo son.

---

## Principios éticos del uso de IA en RenergeIA

1. **Transparencia:** El usuario siempre sabe cuándo está leyendo contenido generado por IA.
2. **Trazabilidad:** El análisis cita los datos en los que se basó.
3. **Revisión humana:** Las recomendaciones de IA son sugerencias. Un profesional HSEQ o un gerente siempre revisa antes de actuar.
4. **Sin datos personales:** El contexto enviado a la IA no incluye nombres de trabajadores, datos de salud, ni información personal identificable.
5. **Confiabilidad sobre creatividad:** El modelo se configura con temperatura baja (≤ 0.3) para maximizar precisión, no originalidad.

---

*Documento generado: 2026-06-28. Actualizar cuando se inicie la integración con Azure OpenAI en Fase 3.*
