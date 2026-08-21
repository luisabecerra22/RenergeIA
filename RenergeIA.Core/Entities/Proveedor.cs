namespace RenergeIA.Core.Entities;

public class Proveedor : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public string Nombre { get; set; } = string.Empty;
    public string? NIT { get; set; }
    public bool EsRenergeia { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<PersonaExterna> Personas { get; set; } = [];
    public ICollection<RecursoEquipo> Recursos { get; set; } = [];
    public ICollection<DocumentoControl> Documentos { get; set; } = [];
}
