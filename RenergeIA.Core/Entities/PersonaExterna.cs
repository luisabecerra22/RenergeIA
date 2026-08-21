namespace RenergeIA.Core.Entities;

public class PersonaExterna : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public int? ProveedorId { get; set; }
    public Proveedor? Proveedor { get; set; }

    public string Nombre { get; set; } = string.Empty;
    public string? Cedula { get; set; }
    public string? Rol { get; set; }
    public bool Activo { get; set; } = true;

    public ICollection<DocumentoControl> Documentos { get; set; } = [];
}
