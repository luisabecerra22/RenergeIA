using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities.HSEQ;

public class HallazgoHSEQ : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Codigo { get; set; } = string.Empty;
    public DivisionHSEQ Division { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string? Causa { get; set; }
    public string? Clasificacion { get; set; }
    public string Responsable { get; set; } = string.Empty;
    public DateTime FechaDeteccion { get; set; }
    public DateTime? FechaCompromiso { get; set; }
    public DateTime? FechaCierre { get; set; }
    public EstadoNoConformidad Estado { get; set; } = EstadoNoConformidad.Abierta;
    public SeveridadNoConformidad Severidad { get; set; }
    public string? Evidencias { get; set; }
    public string? Ubicacion { get; set; }

    public ICollection<AccionHSEQ> Acciones { get; set; } = [];
}
