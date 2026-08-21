using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class TipoDocumentoControl : EntidadBase
{
    public string Nombre { get; set; } = string.Empty;
    public CategoriaDocumentoControl Categoria { get; set; }
    public bool RequiereVencimiento { get; set; } = true;
    public bool Activo { get; set; } = true;
}
