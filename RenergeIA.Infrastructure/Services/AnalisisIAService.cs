using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RenergeIA.Infrastructure.Services;

public class AnalisisIAService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public AnalisisIAService(HttpClient http, string apiKey)
    {
        _http = http;
        _apiKey = apiKey;
    }

    public async Task<string> GenerarAnalisisConsolidadoAsync(DatosAnalisis datos)
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            return "⚠ API key de Gemini no configurada. Configure la variable GEMINI_API_KEY.";

        var prompt = ConstruirPrompt(datos);
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.6-flash:generateContent?key={_apiKey}";

        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                maxOutputTokens = 1024,
                temperature = 0.4
            }
        };

        try
        {
            var response = await _http.PostAsync(url,
                new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return $"⚠ Error al consultar IA: {response.StatusCode}";

            var result = JsonSerializer.Deserialize<GeminiResponse>(json);
            return result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text
                   ?? "Sin respuesta de IA.";
        }
        catch (Exception ex)
        {
            return $"⚠ Error de conexión con IA: {ex.Message}";
        }
    }

    private static string ConstruirPrompt(DatosAnalisis d)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Eres un analista financiero experto en proyectos EPC de energía solar fotovoltaica. Analiza el siguiente informe semanal de costos y genera un análisis ejecutivo en español, conciso y profesional (máximo 5 párrafos cortos). Incluye:");
        sb.AppendLine("1. Estado general del proyecto en términos de costos");
        sb.AppendLine("2. Análisis de la ejecución presupuestal y margen");
        sb.AppendLine("3. Alertas o riesgos identificados");
        sb.AppendLine("4. Recomendaciones concretas");
        sb.AppendLine();
        sb.AppendLine($"Proyecto: {d.NombreProyecto}");
        sb.AppendLine($"Período: {d.FechaInicio:dd/MM/yyyy} — {d.FechaFin:dd/MM/yyyy}");
        sb.AppendLine($"Informe N°: {d.NumeroInforme}");
        sb.AppendLine($"TRM BOM: ${d.TrmBom:N0} | TRM Actual: ${d.TrmActual:N0}");
        sb.AppendLine();
        sb.AppendLine("=== RESUMEN USD ===");
        sb.AppendLine($"Presupuesto: US$ {d.PresupuestoUSD:N2}");
        sb.AppendLine($"Ejecutado: US$ {d.EjecutadoUSD:N2}");
        sb.AppendLine($"Comprometido: US$ {d.ComprometidoUSD:N2}");
        sb.AppendLine($"Disponible: US$ {d.DisponibleUSD:N2}");
        sb.AppendLine($"% Ejecución: {d.PctEjecucion:N1}%");
        sb.AppendLine($"Venta contractual: US$ {d.VentaUSD:N2}");
        sb.AppendLine($"Margen: US$ {d.MargenUSD:N2} ({d.PctMargen:N1}%)");
        sb.AppendLine();
        sb.AppendLine("=== RESUMEN COP ===");
        sb.AppendLine($"Presupuesto: ${d.PresupuestoCOP:N0}");
        sb.AppendLine($"Ejecutado: ${d.EjecutadoCOP:N0}");
        sb.AppendLine($"Disponible: ${d.DisponibleCOP:N0}");
        sb.AppendLine();

        if (d.Categorias.Any())
        {
            sb.AppendLine("=== DESGLOSE POR CATEGORÍA (USD) ===");
            foreach (var c in d.Categorias)
                sb.AppendLine($"- {c.Nombre}: Presup US${c.PresupuestoUSD:N2} | Ejec US${c.EjecutadoUSD:N2} | Disp US${c.DisponibleUSD:N2}");
        }

        sb.AppendLine();
        sb.AppendLine("No uses markdown, solo texto plano con saltos de línea. Sé directo y orientado a la acción.");
        return sb.ToString();
    }

    private class GeminiResponse
    {
        [JsonPropertyName("candidates")]
        public List<GeminiCandidate>? Candidates { get; set; }
    }

    private class GeminiCandidate
    {
        [JsonPropertyName("content")]
        public GeminiContent? Content { get; set; }
    }

    private class GeminiContent
    {
        [JsonPropertyName("parts")]
        public List<GeminiPart>? Parts { get; set; }
    }

    private class GeminiPart
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}

public class DatosAnalisis
{
    public string NombreProyecto { get; set; } = "";
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int NumeroInforme { get; set; }
    public decimal TrmBom { get; set; }
    public decimal TrmActual { get; set; }
    public decimal PresupuestoUSD { get; set; }
    public decimal EjecutadoUSD { get; set; }
    public decimal ComprometidoUSD { get; set; }
    public decimal DisponibleUSD { get; set; }
    public decimal PctEjecucion { get; set; }
    public decimal VentaUSD { get; set; }
    public decimal MargenUSD { get; set; }
    public decimal PctMargen { get; set; }
    public decimal PresupuestoCOP { get; set; }
    public decimal EjecutadoCOP { get; set; }
    public decimal DisponibleCOP { get; set; }
    public List<CategoriaAnalisis> Categorias { get; set; } = [];
}

public class CategoriaAnalisis
{
    public string Nombre { get; set; } = "";
    public decimal PresupuestoUSD { get; set; }
    public decimal EjecutadoUSD { get; set; }
    public decimal DisponibleUSD { get; set; }
}
