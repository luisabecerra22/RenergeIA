using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Entities;

public class InformeDiario : EntidadBase
{
    public int ProyectoId { get; set; }
    public Proyecto Proyecto { get; set; } = null!;

    public DateTime Fecha { get; set; }
    public string? NumeroCertificado { get; set; }
    public string? ResumenActividades { get; set; }
    public int PersonalTotal { get; set; }
    public string? Observaciones { get; set; }
    public string CreadoPor { get; set; } = string.Empty;
    public bool Enviado { get; set; }

    public EstadoInforme Estado { get; set; } = EstadoInforme.Borrador;
    public string? RevisadoPor { get; set; }
    public DateTime? FechaRevision { get; set; }
    public string? MotivoRechazo { get; set; }
    public int Version { get; set; } = 1;
    public string? ComentariosGenerales { get; set; }
    public int? InformeDiarioAnteriorId { get; set; }
    public InformeDiario? InformeDiarioAnterior { get; set; }

    public ICollection<RegistroAvanceDiario> RegistrosAvance { get; set; } = [];
    public ICollection<Fotografia> Fotografias { get; set; } = [];
    public ICollection<RegistroClima> RegistrosClima { get; set; } = [];
}
