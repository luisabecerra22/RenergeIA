namespace RenergeIA.Core.Enums;

// Nivel de acceso a un módulo. Cada nivel incluye a los anteriores
// (Aprobar ⊃ Editar ⊃ Ver). Se guarda como int: agregar valores SOLO al final.
public enum NivelAcceso
{
    Ninguno = 0,
    Ver = 1,
    Editar = 2,
    Aprobar = 3
}
