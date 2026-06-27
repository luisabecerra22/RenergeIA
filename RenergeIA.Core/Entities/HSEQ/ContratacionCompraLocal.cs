namespace RenergeIA.Core.Entities.HSEQ;

public class ContratacionLocal : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string NombrePersona { get; set; } = string.Empty;
    public string Documento { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Empresa { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public DateTime? FechaRetiro { get; set; }
    public bool EsLocal { get; set; } = true;
    public string? Observaciones { get; set; }
}

public class CompraLocal : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;
    public string Proveedor { get; set; } = string.Empty;
    public string Municipio { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal ValorCOP { get; set; }
    public DateTime Fecha { get; set; }
    public string? NumeroFactura { get; set; }
    public bool EsLocal { get; set; } = true;
    public string? Observaciones { get; set; }
}
