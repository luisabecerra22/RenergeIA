using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services;

public class DocumentoService
{
    private readonly IWebHostEnvironment _env;

    private static readonly HashSet<string> ExtensionesPermitidas = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".dwg", ".dxf",
        ".xlsx", ".xls", ".docx", ".doc",
        ".png", ".jpg", ".jpeg", ".gif",
        ".zip", ".rar"
    };

    private const long MaxTamanioBytes = 50L * 1024 * 1024; // 50 MB

    public DocumentoService(IWebHostEnvironment env)
    {
        _env = env;
    }

    // Guarda el archivo en wwwroot/uploads/documentos/{proyectoId}/{documentoId}/
    // Retorna (ruta relativa, null) al éxito o (null, mensaje de error) al fallar.
    public async Task<(string? Ruta, string? Error)> GuardarArchivoAsync(
        Stream contenido,
        string nombreOriginal,
        long tamanio,
        int proyectoId,
        int documentoId,
        string numeroVersion)
    {
        var ext = Path.GetExtension(nombreOriginal);
        if (!ExtensionesPermitidas.Contains(ext))
            return (null, $"Extensión '{ext}' no permitida.");

        if (tamanio > MaxTamanioBytes)
            return (null, "El archivo supera el límite de 50 MB.");

        var carpeta = Path.Combine(
            _env.WebRootPath, "uploads", "documentos",
            proyectoId.ToString(), documentoId.ToString());

        Directory.CreateDirectory(carpeta);

        var versionLimpia = numeroVersion.Replace(" ", "_");
        var nombreArchivo = $"{documentoId}_v{versionLimpia}_{Path.GetFileName(nombreOriginal)}";
        var rutaCompleta = Path.Combine(carpeta, nombreArchivo);

        await using var fs = File.Create(rutaCompleta);
        await contenido.CopyToAsync(fs);

        var ruta = Path.Combine("uploads", "documentos",
            proyectoId.ToString(), documentoId.ToString(), nombreArchivo)
            .Replace('\\', '/');

        return (ruta, null);
    }

    // Calcula la siguiente versión: Rev 0 → Rev A → Rev B → ... → Rev Z → Rev AA
    public async Task<string> SiguienteVersionAsync(RenergeIADbContext db, int documentoId)
    {
        var versiones = await db.VersionesDocumento
            .Where(v => v.DocumentoId == documentoId)
            .Select(v => v.NumeroVersion)
            .ToListAsync();

        if (versiones.Count == 0)
            return "Rev 0";

        // Filtrar versiones con formato "Rev X"
        var letras = versiones
            .Where(v => v.StartsWith("Rev ") && v.Length > 4 && v[4] != '0')
            .Select(v => v[4..])
            .ToList();

        if (letras.Count == 0)
            return "Rev A";

        var ultima = letras.OrderBy(l => l.Length).ThenBy(l => l).Last();
        return "Rev " + IncrementarLetras(ultima);
    }

    private static string IncrementarLetras(string letras)
    {
        var chars = letras.ToCharArray();
        for (int i = chars.Length - 1; i >= 0; i--)
        {
            if (chars[i] < 'Z')
            {
                chars[i]++;
                return new string(chars);
            }
            chars[i] = 'A';
        }
        return new string('A', chars.Length + 1);
    }

    // Marca todas las versiones del documento como no-actual.
    public async Task DesmarcarVersionesActualesAsync(RenergeIADbContext db, int documentoId)
    {
        var activas = await db.VersionesDocumento
            .Where(v => v.DocumentoId == documentoId && v.EsVersionActual)
            .ToListAsync();

        foreach (var v in activas)
            v.EsVersionActual = false;
    }

    // Elimina el archivo físico; no lanza excepción si no existe.
    public void EliminarArchivo(string rutaRelativa)
    {
        var ruta = Path.Combine(_env.WebRootPath, rutaRelativa.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(ruta))
            File.Delete(ruta);
    }

    public async Task<bool> CodigoEsUnicoAsync(
        RenergeIADbContext db, int proyectoId, string codigo, int? excluirId = null)
    {
        var query = db.Documentos
            .Where(d => d.ProyectoId == proyectoId &&
                        d.Codigo == codigo);

        if (excluirId.HasValue)
            query = query.Where(d => d.Id != excluirId.Value);

        return !await query.AnyAsync();
    }

    public static string FormatearTamanio(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        if (bytes < 1024L * 1024 * 1024) return $"{bytes / (1024.0 * 1024):F1} MB";
        return $"{bytes / (1024.0 * 1024 * 1024):F2} GB";
    }

    public string ObtenerRutaCompleta(string rutaRelativa) =>
        Path.Combine(_env.WebRootPath, rutaRelativa.Replace('/', Path.DirectorySeparatorChar));
}
