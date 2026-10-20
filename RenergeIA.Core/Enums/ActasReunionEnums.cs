namespace RenergeIA.Core.Enums;

// Enums del módulo de Compromisos (actas de reunión). Se persisten como int:
// los valores nuevos SIEMPRE se agregan al final (regla del proyecto).
public enum EstadoActaReunion
{
    Borrador = 0,
    ConComentarios = 1,
    Aprobada = 2
}

public enum EstadoCompromisoReunion
{
    Pendiente = 0,
    EnSeguimiento = 1,
    Cerrado = 2
}

public enum TipoCompromisoReunion
{
    Propio = 0,       // de Renergeia o sus proveedores
    Recordatorio = 1  // recordatorio al cliente
}

public enum TipoArchivoActa
{
    InternaPdf = 0,
    InternaDocx = 1,
    ExternaPdf = 2,
    ExternaDocx = 3
}
