namespace RenergeIA.Web.Services;

public class ControlIngresoNotifier
{
    public event Func<int, Task>? CambioRealizado;

    public async Task NotificarAsync(int proyectoId)
    {
        if (CambioRealizado is null) return;
        foreach (var handler in CambioRealizado.GetInvocationList().Cast<Func<int, Task>>())
        {
            try { await handler(proyectoId); }
            catch { /* un suscriptor caído no debe tumbar la notificación a los demás */ }
        }
    }
}
