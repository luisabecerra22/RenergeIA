namespace RenergeIA.Core.Entities;

public class DocumentoControl : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public int TipoDocumentoControlId { get; set; }
    public TipoDocumentoControl TipoDocumentoControl { get; set; } = null!;

    public int? PersonaExternaId { get; set; }
    public PersonaExterna? PersonaExterna { get; set; }

    public int? RecursoEquipoId { get; set; }
    public RecursoEquipo? RecursoEquipo { get; set; }

    public int? ProveedorId { get; set; }
    public Proveedor? Proveedor { get; set; }

    public DateTime? FechaVencimiento { get; set; }
    public bool Entregado { get; set; }
    public bool Vigente { get; set; } = true;

    public string? RutaArchivo { get; set; }
    public string? NombreArchivo { get; set; }

    public string? ResponsableNombre { get; set; }
    public string? ResponsableEmail { get; set; }
    public string? Observaciones { get; set; }
}
