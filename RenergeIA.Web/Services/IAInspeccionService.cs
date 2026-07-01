using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components.Forms;

namespace RenergeIA.Web.Services;

public class AnalisisIAResult
{
    [JsonPropertyName("peligros_identificados")]   public List<string> PeligrosIdentificados { get; set; } = [];
    [JsonPropertyName("clasificacion_peligro")]    public List<string> ClasificacionPeligro { get; set; } = [];
    [JsonPropertyName("efectos_posibles")]         public List<string> EfectosPosibles { get; set; } = [];
    [JsonPropertyName("controles_existentes_detectados")] public List<string> ControlesExistentes { get; set; } = [];
    [JsonPropertyName("controles_faltantes")]      public List<string> ControlesFaltantes { get; set; } = [];
    [JsonPropertyName("nivel_deficiencia")]        public string NivelDeficiencia { get; set; } = "";
    [JsonPropertyName("nivel_exposicion")]         public string NivelExposicion { get; set; } = "";
    [JsonPropertyName("nivel_probabilidad")]       public string NivelProbabilidad { get; set; } = "";
    [JsonPropertyName("nivel_consecuencia")]       public string NivelConsecuencia { get; set; } = "";
    [JsonPropertyName("nivel_riesgo")]             public string NivelRiesgo { get; set; } = "";
    [JsonPropertyName("aceptabilidad")]            public string Aceptabilidad { get; set; } = "";
    [JsonPropertyName("medidas_intervencion")]     public List<string> MedidasIntervencion { get; set; } = [];
    [JsonPropertyName("responsable_sugerido")]     public string ResponsableSugerido { get; set; } = "";
    [JsonPropertyName("plazo_sugerido")]           public string PlazoSugerido { get; set; } = "";
    [JsonPropertyName("hallazgo_redactado")]       public string HallazgoRedactado { get; set; } = "";
    [JsonPropertyName("accion_correctiva")]        public string AccionCorrectiva { get; set; } = "";
    [JsonPropertyName("documentos_relacionados")]  public List<string> DocumentosRelacionados { get; set; } = [];
    [JsonPropertyName("epp_requerido")]            public List<string> EPPRequerido { get; set; } = [];

    // Valores numéricos GTC 45 calculados a partir de los niveles
    public int ND => NivelDeficiencia?.ToUpper() switch
    {
        "MA" or "MUY ALTO" or "MUY ALTA" => 10,
        "A"  or "ALTO" or "ALTA"         => 6,
        "M"  or "MEDIO" or "MEDIA"       => 2,
        _                                => 0
    };
    public int NE => NivelExposicion?.ToUpper() switch
    {
        "EC" or "CONTINUA"    => 4,
        "EF" or "FRECUENTE"  => 3,
        "EO" or "OCASIONAL"  => 2,
        _                    => 1
    };
    public int NP => ND * NE;
    public int NC => NivelConsecuencia?.ToUpper() switch
    {
        "M"  or "MORTAL" or "CATASTRÓFICO" or "CATASTROFICO" => 100,
        "MG" or "MUY GRAVE"                                   => 60,
        "G"  or "GRAVE"                                       => 25,
        _                                                     => 10
    };
    public int NR => NP * NC;
    public string AceptabilidadCalculada => NR switch
    {
        >= 4000             => "I — No Aceptable",
        >= 2000             => "II — No Aceptable",
        >= 1000             => "III — Mejorable",
        >= 200              => "IV — Aceptable con control",
        _                  => "V — Aceptable"
    };
    public string ColorNR => NR switch
    {
        >= 4000 => "#C0392B",
        >= 2000 => "#E65100",
        >= 1000 => "#F39C12",
        >= 200  => "#F1C40F",
        _       => "#27AE60"
    };
}

public class IAInspeccionService
{
    private readonly IHttpClientFactory _httpFactory;
    private readonly IConfiguration _config;
    private readonly IWebHostEnvironment _env;

    private static readonly JsonSerializerOptions _jsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false
    };

    public IAInspeccionService(IHttpClientFactory httpFactory, IConfiguration config, IWebHostEnvironment env)
    {
        _httpFactory = httpFactory;
        _config = config;
        _env = env;
    }

    public async Task<(AnalisisIAResult? Resultado, string? Error)> AnalizarAsync(
        string? evidenciaUrl,
        string area,
        string actividad,
        string tarea,
        string? observacion)
    {
        var apiKey = _config["Anthropic:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
            return (null, "No se ha configurado la clave de API de Anthropic en appsettings.json (Anthropic:ApiKey).");

        var model = _config["Anthropic:Model"] ?? "claude-sonnet-4-6";

        // Construir el contenido del mensaje
        var contentParts = new List<object>();

        // Adjuntar imagen si existe
        if (!string.IsNullOrEmpty(evidenciaUrl))
        {
            var (base64, mediaType, ok) = await LeerImagenBase64Async(evidenciaUrl);
            if (ok && base64 is not null)
            {
                contentParts.Add(new
                {
                    type = "image",
                    source = new { type = "base64", media_type = mediaType, data = base64 }
                });
            }
        }

        // Texto del prompt
        contentParts.Add(new
        {
            type = "text",
            text = ConstruirPrompt(area, actividad, tarea, observacion)
        });

        var requestBody = new
        {
            model,
            max_tokens = 2000,
            messages = new[] { new { role = "user", content = contentParts } }
        };

        var json = JsonSerializer.Serialize(requestBody, _jsonOpts);

        using var client = _httpFactory.CreateClient();
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        try
        {
            response = await client.PostAsync("https://api.anthropic.com/v1/messages", content);
        }
        catch (Exception ex)
        {
            return (null, $"Error de conexión con la API: {ex.Message}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
            return (null, $"Error API ({(int)response.StatusCode}): {responseBody[..Math.Min(300, responseBody.Length)]}");

        // Extraer el texto de la respuesta de Anthropic
        using var doc = JsonDocument.Parse(responseBody);
        var textoRespuesta = doc.RootElement
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString() ?? "";

        // Extraer el JSON del texto (puede tener texto antes o después)
        var resultado = ExtraerYParsearJSON(textoRespuesta);
        if (resultado is null)
            return (null, $"No se pudo interpretar la respuesta de la IA. Respuesta: {textoRespuesta[..Math.Min(400, textoRespuesta.Length)]}");

        return (resultado, null);
    }

    private string ConstruirPrompt(string area, string actividad, string tarea, string? observacion)
    {
        var obs = string.IsNullOrWhiteSpace(observacion) ? "Sin observaciones adicionales." : observacion;
        return $$"""
Eres un experto en Seguridad y Salud en el Trabajo (SST) especializado en proyectos EPC de energía solar fotovoltaica en Colombia.

Analiza la imagen adjunta y la información de la inspección de campo para identificar peligros y valorar riesgos según la metodología GTC 45 de ICONTEC.

DATOS DE LA INSPECCIÓN:
- Área: {{area}}
- Actividad: {{actividad}}
- Tarea: {{tarea}}
- Observación del inspector: {{obs}}

INSTRUCCIONES:
1. Identifica todos los peligros visibles en la imagen y mencionados en la observación.
2. Clasifícalos según GTC 45 (Locativo, Mecánico, Eléctrico, Químico, Biológico, Biomecánico, Psicosocial, Físico, Fenómenos naturales, Público).
3. Valora el riesgo según GTC 45:
   - Nivel de Deficiencia (ND): MA (Muy Alto=10), A (Alto=6), M (Medio=2), B (Bajo=0)
   - Nivel de Exposición (NE): EC (Continua=4), EF (Frecuente=3), EO (Ocasional=2), EEsp (Esporádica=1)
   - Nivel de Consecuencia (NC): M (Mortal=100), MG (Muy Grave=60), G (Grave=25), L (Leve=10)
4. Propón controles en la jerarquía: Eliminación → Sustitución → Ingeniería → Administrativos → EPP.
5. Redacta el hallazgo y la acción correctiva en lenguaje técnico formal.

RESPONDE ÚNICAMENTE CON UN JSON VÁLIDO SIN TEXTO ADICIONAL:
{
  "peligros_identificados": ["peligro1", "peligro2"],
  "clasificacion_peligro": ["Locativo", "Mecánico"],
  "efectos_posibles": ["efecto1", "efecto2"],
  "controles_existentes_detectados": ["control1"],
  "controles_faltantes": ["faltante1"],
  "nivel_deficiencia": "A",
  "nivel_exposicion": "EF",
  "nivel_probabilidad": "Alto",
  "nivel_consecuencia": "G",
  "nivel_riesgo": "Alto",
  "aceptabilidad": "II",
  "medidas_intervencion": ["medida1", "medida2"],
  "responsable_sugerido": "Jefe de SST",
  "plazo_sugerido": "Inmediato",
  "hallazgo_redactado": "texto del hallazgo...",
  "accion_correctiva": "texto de la acción...",
  "documentos_relacionados": ["ATS", "Permiso de trabajo en alturas"],
  "epp_requerido": ["Casco", "Arnés", "Botas con puntera"]
}
""";
    }

    private AnalisisIAResult? ExtraerYParsearJSON(string texto)
    {
        try
        {
            // Intenta parsear directo
            if (texto.TrimStart().StartsWith('{'))
                return JsonSerializer.Deserialize<AnalisisIAResult>(texto, _jsonOpts);

            // Busca bloque JSON entre ```json ... ``` o { ... }
            var start = texto.IndexOf('{');
            var end   = texto.LastIndexOf('}');
            if (start >= 0 && end > start)
            {
                var jsonStr = texto[start..(end + 1)];
                return JsonSerializer.Deserialize<AnalisisIAResult>(jsonStr, _jsonOpts);
            }
            return null;
        }
        catch { return null; }
    }

    private async Task<(string? Base64, string MediaType, bool Ok)> LeerImagenBase64Async(string evidenciaUrl)
    {
        try
        {
            var ruta = Path.Combine(_env.WebRootPath,
                evidenciaUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (!File.Exists(ruta)) return (null, "", false);

            var ext = Path.GetExtension(ruta).ToLowerInvariant();
            var mediaType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png"            => "image/png",
                ".gif"            => "image/gif",
                ".webp"           => "image/webp",
                _                 => ""
            };
            if (string.IsNullOrEmpty(mediaType)) return (null, "", false);

            var bytes  = await File.ReadAllBytesAsync(ruta);
            var base64 = Convert.ToBase64String(bytes);
            return (base64, mediaType, true);
        }
        catch { return (null, "", false); }
    }

    public string SerializarResultado(AnalisisIAResult r) =>
        JsonSerializer.Serialize(r, _jsonOpts);

    public AnalisisIAResult? DeserializarResultado(string? json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        try { return JsonSerializer.Deserialize<AnalisisIAResult>(json, _jsonOpts); }
        catch { return null; }
    }
}
