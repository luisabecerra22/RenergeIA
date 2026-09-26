namespace RenergeIA.Core.Enums;

public enum Disciplina
{
    Mecanica,
    Civil,
    Electrica,
    Contractual,
    PuestaEnMarcha,     // antes HotCommissioning (valor 4 en BD, se mantiene)
    CierreProyecto,     // se muestra "Cierre de proyecto" (antes "Dossier")
    General,
    Suministros,
    Ingenieria,
    Construccion
}
