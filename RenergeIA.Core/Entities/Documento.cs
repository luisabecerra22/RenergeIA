using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class Documento : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public string Codigo { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public TipoDocumento TipoDocumento { get; set; }
    public EstadoDocumento Estado { get; set; } = EstadoDocumento.Borrador;
    public DisciplinaDocumento Disciplina { get; set; } = DisciplinaDocumento.Otro;
    public string? Descripcion { get; set; }
    public DateTime? FechaEmision { get; set; }

    public ICollection<VersionDocumento> Versiones { get; set; } = [];
}
