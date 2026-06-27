using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Enums;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services;

public class HomeDashboardService(RenergeIADbContext db, InformeDiarioService informeSvc)
{
    public async Task<HomeDashboardVM> ObtenerAsync()
    {
        try
        {
            var todos = await db.Proyectos
                .OrderByDescending(p => p.FechaCreacion)
                .ToListAsync();

            var activos = todos.Where(p => p.Estado == EstadoProyecto.EnEjecucion).ToList();

            var paraTabla = todos
                .Where(p => p.Estado != EstadoProyecto.Cancelado && p.Estado != EstadoProyecto.Completado)
                .ToList();

            var filas = new List<ProyectoAtencionVM>();
            foreach (var p in paraTabla)
            {
                var kpis = await informeSvc.KPIsProyectoAsync(p.Id);
                var ultimaFecha = await db.RegistrosAvanceDiario
                    .Where(r => r.ProyectoId == p.Id)
                    .OrderByDescending(r => r.Fecha)
                    .Select(r => (DateTime?)r.Fecha)
                    .FirstOrDefaultAsync();

                filas.Add(new ProyectoAtencionVM
                {
                    ProyectoId           = p.Id,
                    Codigo               = p.Codigo,
                    Nombre               = p.Nombre,
                    AvanceProgramado     = kpis.EsperadoGeneral,
                    AvanceReal           = kpis.AvanceGeneral,
                    Desviacion           = kpis.Desviacion,
                    SPI                  = kpis.SPIGlobal,
                    ActividadesAtrasadas = kpis.ActividadesAtrasadas,
                    UltimoInforme        = ultimaFecha,
                    Clasificacion        = Clasificar(kpis),
                    UrlDashboard         = $"/proyectos/{p.Id}/dashboard"
                });
            }

            var conDatos   = filas.Where(f => f.Clasificacion != "SinDatos").ToList();
            var saludables = filas.Count(f => f.Clasificacion == "Saludable");
            var enRiesgo   = filas.Count(f => f.Clasificacion == "EnRiesgo");
            var criticos   = filas.Count(f => f.Clasificacion == "Critico");
            var sinDatos   = filas.Count(f => f.Clasificacion == "SinDatos");

            decimal avgProg = conDatos.Any() ? Math.Round(conDatos.Average(f => f.AvanceProgramado), 1) : 0;
            decimal avgReal = conDatos.Any() ? Math.Round(conDatos.Average(f => f.AvanceReal), 1) : 0;
            decimal avgSPI  = conDatos.Any() ? Math.Round(conDatos.Average(f => f.SPI), 2) : 0;
            int totalAtraso = filas.Sum(f => f.ActividadesAtrasadas);

            var estadoGeneral = criticos   > 0 ? "Critico"
                              : enRiesgo   > 0 ? "EnRiesgo"
                              : saludables > 0 ? "Saludable"
                              : "SinDatos";

            var atencion = filas
                .OrderBy(f => f.Clasificacion switch
                {
                    "Critico"   => 0,
                    "EnRiesgo"  => 1,
                    "Saludable" => 2,
                    _           => 3
                })
                .ThenBy(f => f.SPI)
                .ThenByDescending(f => f.ActividadesAtrasadas)
                .ThenBy(f => f.Desviacion)
                .Take(10)
                .ToList();

            return new HomeDashboardVM
            {
                TotalProyectos            = todos.Count,
                TotalProyectosActivos     = activos.Count,
                TotalProyectosSaludables  = saludables,
                TotalProyectosEnRiesgo    = enRiesgo,
                TotalProyectosCriticos    = criticos,
                TotalProyectosSinDatos    = sinDatos,
                AvanceGlobalProgramado    = avgProg,
                AvanceGlobalReal          = avgReal,
                DesviacionGlobal          = Math.Round(avgReal - avgProg, 1),
                SpiGlobalPromedio         = avgSPI,
                TotalActividadesAtrasadas = totalAtraso,
                FechaActualizacion        = DateTime.Now,
                EstadoGeneral             = estadoGeneral,
                ResumenEjecutivoTexto     = GenerarResumen(activos.Count, saludables, enRiesgo, criticos, avgReal, avgProg),
                ProyectosAtencion         = atencion
            };
        }
        catch
        {
            return new HomeDashboardVM { FechaActualizacion = DateTime.Now };
        }
    }

    private static string Clasificar(KPIsProyecto kpis) =>
        kpis.TotalActividades == 0 ? "SinDatos" :
        kpis.SPIGlobal >= 0.95m   ? "Saludable" :
        kpis.SPIGlobal >= 0.85m   ? "EnRiesgo"  : "Critico";

    private static string GenerarResumen(int activos, int sanos, int riesgo, int criticos, decimal real, decimal prog)
    {
        if (activos == 0)
            return "No hay proyectos en ejecución actualmente. Crea un proyecto para comenzar el seguimiento del portafolio.";

        var sb = new System.Text.StringBuilder();
        sb.Append($"Actualmente hay {activos} proyecto{(activos != 1 ? "s" : "")} activo{(activos != 1 ? "s" : "")}. ");

        var estados = new List<string>();
        if (criticos > 0) estados.Add($"{criticos} en estado crítico");
        if (riesgo   > 0) estados.Add($"{riesgo} en riesgo");
        if (sanos    > 0) estados.Add($"{sanos} saludable{(sanos != 1 ? "s" : "")}");
        if (estados.Any()) sb.Append(string.Join(", ", estados) + ". ");

        decimal desv = Math.Round(real - prog, 1);
        sb.Append($"El avance global real es {real:F1}% frente a un avance programado de {prog:F1}%, con una desviación global de {desv:+0.0;-0.0;0.0}%. ");

        sb.Append(criticos > 0
            ? "Se recomienda priorizar los proyectos con SPI inferior a 0.85."
            : riesgo > 0
                ? "Se recomienda seguimiento cercano a los proyectos con SPI inferior a 0.95."
                : "El portafolio de proyectos se encuentra en estado saludable.");

        return sb.ToString();
    }
}

public class HomeDashboardVM
{
    public int TotalProyectos { get; set; }
    public int TotalProyectosActivos { get; set; }
    public int TotalProyectosSaludables { get; set; }
    public int TotalProyectosEnRiesgo { get; set; }
    public int TotalProyectosCriticos { get; set; }
    public int TotalProyectosSinDatos { get; set; }
    public decimal AvanceGlobalProgramado { get; set; }
    public decimal AvanceGlobalReal { get; set; }
    public decimal DesviacionGlobal { get; set; }
    public decimal SpiGlobalPromedio { get; set; }
    public int TotalActividadesAtrasadas { get; set; }
    public DateTime FechaActualizacion { get; set; } = DateTime.Now;
    public string EstadoGeneral { get; set; } = "SinDatos";
    public string ResumenEjecutivoTexto { get; set; } = "";
    public List<ProyectoAtencionVM> ProyectosAtencion { get; set; } = [];
}

public class ProyectoAtencionVM
{
    public int ProyectoId { get; set; }
    public string Codigo { get; set; } = "";
    public string Nombre { get; set; } = "";
    public decimal AvanceProgramado { get; set; }
    public decimal AvanceReal { get; set; }
    public decimal Desviacion { get; set; }
    public decimal SPI { get; set; }
    public int ActividadesAtrasadas { get; set; }
    public DateTime? UltimoInforme { get; set; }
    public string Clasificacion { get; set; } = "SinDatos";
    public string UrlDashboard { get; set; } = "";
}
