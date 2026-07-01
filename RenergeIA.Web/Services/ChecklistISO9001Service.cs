using Microsoft.AspNetCore.Components.Forms;
using RenergeIA.Core.Entities.HSEQ;
using System.Text;

namespace RenergeIA.Web.Services;

public class ChecklistISO9001Service
{
    private readonly IWebHostEnvironment _env;

    public ChecklistISO9001Service(IWebHostEnvironment env) => _env = env;

    public async Task<string> GuardarEvidenciaAsync(IBrowserFile file, int proyectoId, long maxBytes = 10_000_000)
    {
        var carpeta = Path.Combine(_env.WebRootPath, "uploads", "evidencias", proyectoId.ToString());
        Directory.CreateDirectory(carpeta);

        var ext = Path.GetExtension(file.Name).ToLowerInvariant();
        var nombreArchivo = $"{Guid.NewGuid()}{ext}";
        var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

        await using var fs = new FileStream(rutaCompleta, FileMode.Create);
        await file.OpenReadStream(maxAllowedSize: maxBytes).CopyToAsync(fs);

        return $"/uploads/evidencias/{proyectoId}/{nombreArchivo}";
    }

    public void EliminarEvidencia(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return;
        var ruta = Path.Combine(_env.WebRootPath, url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(ruta)) File.Delete(ruta);
    }

    public byte[] GenerarCsvExcel(ChecklistAuditoria auditoria)
    {
        var sb = new StringBuilder();
        sb.AppendLine("sep=,");
        sb.AppendLine("Cláusula,Título Cláusula,Requisito,Descripción,Cumplimiento,Puntaje,Evidencia URL,Hallazgo,Oportunidad de Mejora,Responsable,Plazo,Seguimiento,Observaciones");

        foreach (var item in auditoria.Items.OrderBy(i => i.NumeroRequisito))
        {
            static string Esc(string? s) => $"\"{(s ?? "").Replace("\"", "\"\"")}\"";

            sb.AppendLine(string.Join(",",
                Esc(item.Clausula),
                Esc(item.TituloClausula),
                Esc(item.NumeroRequisito),
                Esc(item.DescripcionRequisito),
                Esc(LabelCumplimiento(item.Estado)),
                item.Puntaje.ToString("F1"),
                Esc(item.EvidenciaUrl),
                Esc(item.Hallazgo),
                Esc(item.OportunidadMejora),
                Esc(item.Responsable),
                item.Plazo.HasValue ? item.Plazo.Value.ToString("dd/MM/yyyy") : "",
                Esc(LabelSeguimiento(item.Seguimiento)),
                Esc(item.Observaciones)
            ));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string LabelCumplimiento(RenergeIA.Core.Enums.EstadoCumplimiento e) => e switch
    {
        RenergeIA.Core.Enums.EstadoCumplimiento.Cumple     => "Cumple",
        RenergeIA.Core.Enums.EstadoCumplimiento.NoCumple   => "No Cumple",
        RenergeIA.Core.Enums.EstadoCumplimiento.EnProceso  => "En Proceso",
        RenergeIA.Core.Enums.EstadoCumplimiento.Parcial    => "Parcial",
        RenergeIA.Core.Enums.EstadoCumplimiento.NoAplica   => "No Aplica",
        _                                                   => "Sin Evaluar"
    };

    private static string LabelSeguimiento(RenergeIA.Core.Enums.EstadoSeguimiento s) => s switch
    {
        RenergeIA.Core.Enums.EstadoSeguimiento.Ejecutado  => "Ejecutado",
        RenergeIA.Core.Enums.EstadoSeguimiento.EnProceso  => "En Proceso",
        _                                                   => "Pendiente"
    };
}
