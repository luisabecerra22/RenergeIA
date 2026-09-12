namespace RenergeIA.Web.Services;

public class CambiosPendientesService
{
    public bool HayCambios { get; private set; }
    public string? Origen { get; private set; }

    public void Marcar(string origen)
    {
        HayCambios = true;
        Origen = origen;
    }

    public void Limpiar()
    {
        HayCambios = false;
        Origen = null;
    }
}
