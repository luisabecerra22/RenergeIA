using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class PersonalProyecto : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string DocumentoIdentidad { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public TipoPersonal TipoPersonal { get; set; }
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaSalida { get; set; }
    public bool Activo { get; set; } = true;
    public string? Email { get; set; }
    public string? Telefono { get; set; }

    public ICollection<DocumentoPersona> Documentos { get; set; } = [];
}
