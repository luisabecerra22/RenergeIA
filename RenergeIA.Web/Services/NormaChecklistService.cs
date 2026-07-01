using Microsoft.AspNetCore.Components.Forms;
using RenergeIA.Core.Entities.HSEQ;
using RenergeIA.Core.Enums;
using System.Text;

namespace RenergeIA.Web.Services;

public record RequisitoNorma(
    string NumClausula,
    string TituloMain,
    string SubClausula,
    string Id,
    string Req,
    string Interp,
    string[] Docs
);

public class NormaChecklistService
{
    private readonly IWebHostEnvironment _env;
    public NormaChecklistService(IWebHostEnvironment env) => _env = env;

    // ── Metadatos de cada norma ──────────────────────────────────────────────

    public static readonly Dictionary<TipoNormaHSEQ, (string Titulo, string Icono, string Color, string Version)> Meta = new()
    {
        [TipoNormaHSEQ.ISO9001]        = ("ISO 9001",          "bi-patch-check-fill",    "#1565C0", "ISO 9001:2015"),
        [TipoNormaHSEQ.ISO14001]       = ("ISO 14001",         "bi-tree-fill",           "#2E7D32", "ISO 14001:2015"),
        [TipoNormaHSEQ.ISO45001]       = ("ISO 45001",         "bi-shield-fill-check",   "#E65100", "ISO 45001:2018"),
        [TipoNormaHSEQ.Decreto1072]    = ("Decreto 1072",      "bi-file-earmark-text-fill","#6A1B9A","Decreto 1072/2015"),
        [TipoNormaHSEQ.Resolucion0312] = ("Resolución 0312",   "bi-file-earmark-ruled-fill","#880E4F","Res. 0312/2019"),
        [TipoNormaHSEQ.Cliente]        = ("Auditoría Cliente", "bi-person-check-fill",   "#0277BD", "Específica cliente"),
        [TipoNormaHSEQ.Interventoria]  = ("Auditoría Interventoría","bi-building-check", "#4E342E", "Específica interventoría"),
    };

    // ── Conversión URL slug → enum ───────────────────────────────────────────

    public static TipoNormaHSEQ? SlugANorma(string slug) => slug.ToLowerInvariant() switch
    {
        "iso9001"        => TipoNormaHSEQ.ISO9001,
        "iso14001"       => TipoNormaHSEQ.ISO14001,
        "iso45001"       => TipoNormaHSEQ.ISO45001,
        "decreto1072"    => TipoNormaHSEQ.Decreto1072,
        "resolucion0312" => TipoNormaHSEQ.Resolucion0312,
        "cliente"        => TipoNormaHSEQ.Cliente,
        "interventoria"  => TipoNormaHSEQ.Interventoria,
        _                => null
    };

    public static string NormaASlug(TipoNormaHSEQ norma) => norma switch
    {
        TipoNormaHSEQ.ISO9001        => "iso9001",
        TipoNormaHSEQ.ISO14001       => "iso14001",
        TipoNormaHSEQ.ISO45001       => "iso45001",
        TipoNormaHSEQ.Decreto1072    => "decreto1072",
        TipoNormaHSEQ.Resolucion0312 => "resolucion0312",
        TipoNormaHSEQ.Cliente        => "cliente",
        TipoNormaHSEQ.Interventoria  => "interventoria",
        _                            => ""
    };

    // ── Carga de requisitos según norma ──────────────────────────────────────

    public IReadOnlyList<RequisitoNorma> ObtenerRequisitos(TipoNormaHSEQ norma)
    {
        return norma switch
        {
            TipoNormaHSEQ.ISO9001        => MapearDesdeISO9001(),
            TipoNormaHSEQ.ISO14001       => MapearDesdeISO14001(),
            TipoNormaHSEQ.ISO45001       => MapearDesdeISO45001(),
            TipoNormaHSEQ.Decreto1072    => Decreto1072ChecklistData.Requisitos,
            TipoNormaHSEQ.Resolucion0312 => MapearDesdeRes0312(),
            TipoNormaHSEQ.Cliente        => [],
            TipoNormaHSEQ.Interventoria  => [],
            _                            => []
        };
    }

    private static IReadOnlyList<RequisitoNorma> MapearDesdeISO9001() =>
        Iso9001ChecklistData.Requisitos.Select(r =>
            new RequisitoNorma(r.NumClausula, r.TituloMain, r.SubClausula, r.Id, r.Req, r.Interp, r.Docs)).ToList();

    private static IReadOnlyList<RequisitoNorma> MapearDesdeISO14001() =>
        Iso14001ChecklistData.Requisitos.Select(r =>
            new RequisitoNorma(r.NumClausula, r.TituloMain, r.SubClausula, r.Id, r.Req, r.Interp, r.Docs)).ToList();

    private static IReadOnlyList<RequisitoNorma> MapearDesdeISO45001() =>
        Iso45001ChecklistData.Requisitos.Select(r =>
            new RequisitoNorma(r.NumClausula, r.TituloMain, r.SubClausula, r.Id, r.Req, r.Interp, r.Docs)).ToList();

    private static IReadOnlyList<RequisitoNorma> MapearDesdeRes0312() =>
        Resolucion0312ChecklistData.Requisitos.Select(r =>
            new RequisitoNorma(r.NumGrupo, r.TituloGrupo, r.SubClausula, r.Id, r.Req, r.Interp, r.Docs)).ToList();

    // ── Crea los ItemChecklist en la auditoría desde el catálogo ────────────

    public List<ItemChecklist> GenerarItems(TipoNormaHSEQ norma)
    {
        var requisitos = ObtenerRequisitos(norma);
        return requisitos.Select(r => new ItemChecklist
        {
            Clausula            = r.NumClausula,
            TituloClausula      = r.TituloMain,
            NumeroRequisito     = r.Id,
            DescripcionRequisito = r.Req,
            Estado              = EstadoCumplimiento.SinEvaluar,
            Puntaje             = 0
        }).ToList();
    }

    // ── Cálculo de porcentaje ────────────────────────────────────────────────

    public static decimal CalcularPorcentaje(IEnumerable<ItemChecklist> items)
    {
        var evaluables = items.Where(i => i.Estado != EstadoCumplimiento.NoAplica).ToList();
        if (!evaluables.Any()) return 0;

        var puntajeTotal = evaluables.Sum(i => i.Estado switch
        {
            EstadoCumplimiento.Cumple    => 100m,
            EstadoCumplimiento.Parcial   => 50m,
            EstadoCumplimiento.EnProceso => 25m,
            _                            => 0m
        });

        return Math.Round(puntajeTotal / evaluables.Count, 1);
    }

    // ── Subida de evidencias ─────────────────────────────────────────────────

    public async Task<string> GuardarEvidenciaAsync(IBrowserFile file, int auditoriaId, long maxBytes = 10_000_000)
    {
        var carpeta = Path.Combine(_env.WebRootPath, "uploads", "auditorias", auditoriaId.ToString());
        Directory.CreateDirectory(carpeta);

        var ext = Path.GetExtension(file.Name).ToLowerInvariant();
        var nombre = $"{Guid.NewGuid()}{ext}";
        var ruta = Path.Combine(carpeta, nombre);

        await using var fs = new FileStream(ruta, FileMode.Create);
        await file.OpenReadStream(maxAllowedSize: maxBytes).CopyToAsync(fs);

        return $"/uploads/auditorias/{auditoriaId}/{nombre}";
    }

    public void EliminarEvidencia(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;
        var ruta = Path.Combine(_env.WebRootPath, url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(ruta)) File.Delete(ruta);
    }

    // ── Exportar CSV / Excel ─────────────────────────────────────────────────

    public byte[] GenerarCsv(ChecklistAuditoria auditoria)
    {
        var sb = new StringBuilder();
        sb.AppendLine("sep=,");
        sb.AppendLine("Cláusula,Título,Requisito,Descripción,Estado,Evidencia,Hallazgo,Oportunidad,Responsable,Plazo,Seguimiento,Observaciones");

        foreach (var item in auditoria.Items.OrderBy(i => i.NumeroRequisito))
        {
            static string E(string? s) => $"\"{(s ?? "").Replace("\"", "\"\"")}\"";
            sb.AppendLine(string.Join(",",
                E(item.Clausula), E(item.TituloClausula), E(item.NumeroRequisito),
                E(item.DescripcionRequisito), E(LabelCumplimiento(item.Estado)),
                E(item.EvidenciaUrl), E(item.Hallazgo), E(item.OportunidadMejora),
                E(item.Responsable),
                item.Plazo.HasValue ? item.Plazo.Value.ToString("dd/MM/yyyy") : "",
                E(LabelSeguimiento(item.Seguimiento)), E(item.Observaciones)
            ));
        }
        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    // ── Helpers de labels ────────────────────────────────────────────────────

    public static string LabelCumplimiento(EstadoCumplimiento e) => e switch
    {
        EstadoCumplimiento.Cumple    => "Cumple",
        EstadoCumplimiento.NoCumple  => "No Cumple",
        EstadoCumplimiento.EnProceso => "En Proceso",
        EstadoCumplimiento.Parcial   => "Parcial",
        EstadoCumplimiento.NoAplica  => "No Aplica",
        _                            => "Sin Evaluar"
    };

    public static string ColorCumplimiento(EstadoCumplimiento e) => e switch
    {
        EstadoCumplimiento.Cumple    => "#2E7D32",
        EstadoCumplimiento.NoCumple  => "#C62828",
        EstadoCumplimiento.Parcial   => "#E65100",
        EstadoCumplimiento.EnProceso => "#0277BD",
        EstadoCumplimiento.NoAplica  => "#546E7A",
        _                            => "#9E9E9E"
    };

    public static string LabelSeguimiento(EstadoSeguimiento s) => s switch
    {
        EstadoSeguimiento.Ejecutado  => "Ejecutado",
        EstadoSeguimiento.EnProceso  => "En Proceso",
        _                            => "Pendiente"
    };

    public static string ColorEstadoAuditoria(EstadoAuditoria e) => e switch
    {
        EstadoAuditoria.Finalizada => "#2E7D32",
        EstadoAuditoria.EnProceso  => "#0277BD",
        _                          => "#9E9E9E"
    };

    public static string LabelEstadoAuditoria(EstadoAuditoria e) => e switch
    {
        EstadoAuditoria.Finalizada => "Finalizada",
        EstadoAuditoria.EnProceso  => "En Proceso",
        _                          => "Borrador"
    };
}
