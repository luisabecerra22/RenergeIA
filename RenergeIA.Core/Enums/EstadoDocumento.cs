namespace RenergeIA.Core.Enums;

public enum EstadoDocumento
{
    PendienteEmitir,
    PendienteValidacion,
    ValidadoConComentarios,
    Validado,
    Informativos,
    NoValidado,
    /// <summary>El documento no aplica para el proyecto; el motivo se registra en Observaciones. Va al FINAL para no correr los valores guardados.</summary>
    NoAplica
}
