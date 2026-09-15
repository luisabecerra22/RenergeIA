using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services;

public static class WbsMantenimiento
{
    // Elimina versiones de cronograma duplicadas (mismo nombre en el mismo proyecto) que no
    // tienen actividades; si la vigente quedaba entre las borradas, promueve la que sí tiene datos
    public static async Task LimpiarVersionesDuplicadasAsync(RenergeIADbContext db)
    {
        var versiones = await db.CronogramasVersion.ToListAsync();
        var conteos = await db.ActividadesWBS
            .Where(a => a.CronogramaVersionId != null)
            .GroupBy(a => a.CronogramaVersionId!.Value)
            .Select(g => new { VersionId = g.Key, N = g.Count() })
            .ToListAsync();
        var actividadesPorVersion = conteos.ToDictionary(c => c.VersionId, c => c.N);

        var huboCambios = false;
        foreach (var grupo in versiones
            .GroupBy(v => new { v.ProyectoId, Nombre = v.Nombre.Trim().ToUpperInvariant() })
            .Where(g => g.Count() > 1))
        {
            // Conservar la más reciente (Id mayor, priorizando la vigente); depurar las demás
            var ordenadas = grupo.OrderByDescending(v => v.EsVigente).ThenByDescending(v => v.Id).ToList();
            var sobrantes = ordenadas.Skip(1).ToList();

            foreach (var vieja in sobrantes)
            {
                var tieneActividades = actividadesPorVersion.ContainsKey(vieja.Id);
                if (tieneActividades)
                {
                    var idsActividades = await db.ActividadesWBS
                        .Where(a => a.CronogramaVersionId == vieja.Id)
                        .Select(a => a.Id)
                        .ToListAsync();

                    var tieneHistorial = await db.RegistrosAvanceDiario
                        .AnyAsync(r => idsActividades.Contains(r.ActividadWBSId));

                    if (tieneHistorial)
                    {
                        // Tiene avances diarios registrados: no se borra, se renombra para distinguirla
                        if (!vieja.Nombre.Contains("(anterior", StringComparison.OrdinalIgnoreCase))
                        {
                            vieja.Nombre = $"{vieja.Nombre.Trim()} (anterior)";
                            db.CronogramasVersion.Update(vieja);
                            huboCambios = true;
                        }
                        continue;
                    }

                    await db.Partidas
                        .Where(p => p.ActividadWBSId != null && idsActividades.Contains(p.ActividadWBSId.Value))
                        .ExecuteUpdateAsync(s => s.SetProperty(p => p.ActividadWBSId, (int?)null));
                    await db.ActividadesWBS
                        .Where(a => a.CronogramaVersionId == vieja.Id)
                        .ExecuteDeleteAsync();
                }

                var eraVigente = vieja.EsVigente;
                db.CronogramasVersion.Remove(vieja);
                huboCambios = true;

                if (eraVigente)
                {
                    var candidata = ordenadas[0];
                    candidata.EsVigente = true;
                    db.CronogramasVersion.Update(candidata);
                }
            }
        }

        if (huboCambios) await db.SaveChangesAsync();
    }
}
