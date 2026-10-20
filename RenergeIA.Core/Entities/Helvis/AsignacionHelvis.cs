namespace RenergeIA.Core.Entities.Helvis;

// Actividad del WBS asignada a un contacto como responsable de reportar su avance.
// Se liga a la actividad concreta (que pertenece a una versión del cronograma);
// al cambiar de versión, las asignaciones se revisan desde la app — nunca se
// trasladan automáticamente por semejanza de nombre (documento §6).
public class AsignacionHelvis : EntidadBase
{
    public int ContactoHelvisId { get; set; }
    public ContactoHelvis Contacto { get; set; } = null!;

    public int ActividadWBSId { get; set; }
    public ActividadWBS Actividad { get; set; } = null!;

    public bool Activa { get; set; } = true;
    public DateTime? VigenteDesde { get; set; }
    public DateTime? VigenteHasta { get; set; }

    public string? Frente { get; set; }
    public string? Notas { get; set; }
}
