using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace RenergeIA.Infrastructure.Services;

public class TrmService
{
    private readonly HttpClient _http;

    public TrmService(HttpClient http) => _http = http;

    public async Task<decimal> ObtenerTrmActualAsync()
    {
        var datos = await _http.GetFromJsonAsync<TrmDato[]>(
            "resource/32sa-8pi3.json?$order=vigenciadesde%20DESC&$limit=1");

        if (datos is { Length: > 0 } && decimal.TryParse(datos[0].Valor, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var trm))
            return trm;

        return 0m;
    }

    public async Task<decimal> ObtenerTrmPorFechaAsync(DateTime fecha)
    {
        var fechaStr = fecha.ToString("yyyy-MM-dd");
        var datos = await _http.GetFromJsonAsync<TrmDato[]>(
            $"resource/32sa-8pi3.json?$where=vigenciadesde<='{fechaStr}T23:59:59'&$order=vigenciadesde%20DESC&$limit=1");

        if (datos is { Length: > 0 } && decimal.TryParse(datos[0].Valor, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var trm))
            return trm;

        return 0m;
    }

    private class TrmDato
    {
        [JsonPropertyName("valor")]
        public string Valor { get; set; } = "";

        [JsonPropertyName("vigenciadesde")]
        public string VigenciaDesde { get; set; } = "";
    }
}
