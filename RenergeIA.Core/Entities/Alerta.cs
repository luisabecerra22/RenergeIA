using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class Alerta : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public CategoriaAlerta Categoria { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string Severidad { get; set; } = string.Empty;
    public bool EsLeida { get; set; }
    public string? DestinatarioId { get; set; }
    public string? Referencia { get; set; }
}
