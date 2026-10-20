using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

// Documento oficial emitido (Word/PDF, versiones interna y externa). Se guarda el binario
// en la base (el filesystem de Cloud Run no es persistente). Los históricos no se reescriben.
public class ArchivoActaReunion : EntidadBase
{
    public int ActaReunionId { get; set; }
    public ActaReunion ActaReunion { get; set; } = null!;
    public TipoArchivoActa Tipo { get; set; }
    public string Filename { get; set; } = string.Empty;
    public int Tamano { get; set; }
    public byte[] Contenido { get; set; } = [];
    public int? LoteImportacionId { get; set; }
}
