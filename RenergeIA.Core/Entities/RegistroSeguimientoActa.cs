namespace RenergeIA.Core.Entities;

// Registro de trazabilidad confirmada entre reuniones (colección "seguimientos" del sistema anterior).
public class RegistroSeguimientoActa : EntidadBase
{
    public string IdOrigen { get; set; } = string.Empty;   // ej. COS6ML-TR-P03
    public string? ProyectoTexto { get; set; }
    public string? Tema { get; set; }
    public string? Accion { get; set; }
    public string? EstadoRegistro { get; set; }            // Cumplido | Cumplimiento parcial | En seguimiento | Acuerdo recurrente | Por aclarar
    public string? Evidencia { get; set; }
    public string? ReferenciasJson { get; set; }           // códigos de acta o acta-NN
    public string? DatosOrigenJson { get; set; }
    public string? HistorialJson { get; set; }
    public bool Confirmada { get; set; }
    public string? ConfirmadoPor { get; set; }
    public DateTime? ActualizadoEl { get; set; }
    public int? LoteImportacionId { get; set; }
}
