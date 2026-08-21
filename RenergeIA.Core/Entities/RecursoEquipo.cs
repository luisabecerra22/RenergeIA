using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class RecursoEquipo : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public int? ProveedorId { get; set; }
    public Proveedor? Proveedor { get; set; }

    public TipoRecurso Tipo { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string? PlacaSerial { get; set; }

    public int? ConductorOperadorId { get; set; }
    public PersonaExterna? ConductorOperador { get; set; }

    public DateTime? FechaInicioContrato { get; set; }
    public DateTime? FechaFinContrato { get; set; }
    public DateTime? FechaIngresoSitio { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<DocumentoControl> Documentos { get; set; } = [];
    public ICollection<EtapaRevision> Etapas { get; set; } = [];
}
