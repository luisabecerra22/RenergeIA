using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Enums;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services;

public enum SemaforoVencimiento
{
    Vigente,
    Amarillo,
    Naranja,
    Rojo,
    Vencido,
    SinFecha
}

public class ControlIngresoService(RenergeIADbContext db)
{
    public static readonly (string Nombre, CategoriaDocumentoControl Categoria, bool RequiereVencimiento)[] CatalogoBase =
    [
        ("SOAT",                              CategoriaDocumentoControl.Equipo,    true),
        ("RTM (Revisión Técnico-Mecánica)",   CategoriaDocumentoControl.Equipo,    true),
        ("Póliza Responsabilidad Civil",      CategoriaDocumentoControl.Equipo,    true),
        ("Póliza Todo Riesgo",                CategoriaDocumentoControl.Equipo,    true),
        ("Licencia de Tránsito",              CategoriaDocumentoControl.Equipo,    false),
        ("Tarjeta de Registro de Maquinaria",  CategoriaDocumentoControl.Equipo,    false),
        ("Hoja de Vida de Equipo",            CategoriaDocumentoControl.Equipo,    false),
        ("Ficha Técnica",                     CategoriaDocumentoControl.Equipo,    false),
        ("Registro de Mantenimiento P/C",     CategoriaDocumentoControl.Equipo,    false),
        ("Preoperacional",                    CategoriaDocumentoControl.Equipo,    false),
        ("Declaración de Importación",        CategoriaDocumentoControl.Equipo,    false),
        ("Inspección de Equipo",              CategoriaDocumentoControl.Equipo,    false),
        ("Licencia de Conducción",            CategoriaDocumentoControl.Persona,   true),
        ("ARL",                               CategoriaDocumentoControl.Persona,   true),
        ("Examen Médico Ocupacional",         CategoriaDocumentoControl.Persona,   true),
        ("Seguridad Social",                  CategoriaDocumentoControl.Persona,   true),
        ("RUT",                               CategoriaDocumentoControl.Proveedor, false),
        ("Cámara de Comercio",                CategoriaDocumentoControl.Proveedor, false),
        ("Póliza RC Contratista",             CategoriaDocumentoControl.Proveedor, true),
    ];

    public async Task SembrarCatalogoAsync()
    {
        if (await db.TiposDocumentoControl.AnyAsync()) return;

        foreach (var (nombre, categoria, requiereVencimiento) in CatalogoBase)
        {
            db.TiposDocumentoControl.Add(new TipoDocumentoControl
            {
                Nombre = nombre,
                Categoria = categoria,
                RequiereVencimiento = requiereVencimiento
            });
        }
        await db.SaveChangesAsync();
    }

    public static SemaforoVencimiento CalcularSemaforo(DateTime? fechaVencimiento)
    {
        if (fechaVencimiento is null) return SemaforoVencimiento.SinFecha;
        var dias = (fechaVencimiento.Value.Date - DateTime.Today).Days;
        if (dias < 0)   return SemaforoVencimiento.Vencido;
        if (dias < 10)  return SemaforoVencimiento.Rojo;
        if (dias <= 20) return SemaforoVencimiento.Naranja;
        if (dias <= 30) return SemaforoVencimiento.Amarillo;
        return SemaforoVencimiento.Vigente;
    }

    public static (string Texto, string Bg, string Fg) EstiloSemaforo(SemaforoVencimiento semaforo) => semaforo switch
    {
        SemaforoVencimiento.Vigente  => ("Vigente",      "#6ABF4B", "#fff"),
        SemaforoVencimiento.Amarillo => ("Vence pronto",  "#ffc107", "#212529"),
        SemaforoVencimiento.Naranja  => ("Vence pronto",  "#fd7e14", "#fff"),
        SemaforoVencimiento.Rojo     => ("Vence pronto",  "#dc3545", "#fff"),
        SemaforoVencimiento.Vencido  => ("Vencido",       "#3b0764", "#fff"),
        _                             => ("Sin fecha",     "#D9D9D6", "#111921")
    };

    public static (string Bg, string Fg) EstiloEstadoEtapa(EstadoEtapa estado) => estado switch
    {
        EstadoEtapa.Aprobado       => ("#6ABF4B", "#fff"),
        EstadoEtapa.EnRevision     => ("#0d6efd", "#fff"),
        EstadoEtapa.ConComentarios => ("#fd7e14", "#fff"),
        EstadoEtapa.NoAplica       => ("#D9D9D6", "#111921"),
        _                          => ("#e9ecef", "#495057")
    };

    public static readonly EtapaProceso[] EtapasOrdenadas =
        [EtapaProceso.Compras, EtapaProceso.RRHH, EtapaProceso.HSE, EtapaProceso.Cliente, EtapaProceso.Proyecto];

    public static List<EtapaRevision> CrearEtapasIniciales(RecursoEquipo recurso)
    {
        var tieneConductor = recurso.ConductorOperadorId.HasValue;
        return EtapasOrdenadas.Select(etapa => new EtapaRevision
        {
            RecursoEquipo = recurso,
            Etapa = etapa,
            Estado = etapa == EtapaProceso.RRHH && !tieneConductor ? EstadoEtapa.NoAplica : EstadoEtapa.Pendiente
        }).ToList();
    }

    public static string EstadoGeneral(RecursoEquipo recurso)
    {
        var etapas = recurso.Etapas;
        if (etapas is null || etapas.Count == 0) return "Sin iniciar";
        if (etapas.Any(e => e.Estado == EstadoEtapa.ConComentarios)) return "Con comentarios";
        if (etapas.All(e => e.Estado is EstadoEtapa.Aprobado or EstadoEtapa.NoAplica)) return "Aprobado";
        if (etapas.Any(e => e.Estado is EstadoEtapa.Aprobado or EstadoEtapa.EnRevision)) return "En proceso";
        return "Pendiente";
    }

    public async Task SembrarEtapasFaltantesAsync()
    {
        var idsConEtapas = await db.EtapasRevision.Select(e => e.RecursoEquipoId).Distinct().ToListAsync();
        var recursosSinEtapas = await db.RecursosEquipo
            .Where(r => !idsConEtapas.Contains(r.Id))
            .ToListAsync();

        foreach (var recurso in recursosSinEtapas)
            foreach (var etapa in CrearEtapasIniciales(recurso))
                db.EtapasRevision.Add(etapa);

        if (recursosSinEtapas.Count > 0) await db.SaveChangesAsync();
    }

    public static (string Bg, string Fg) EstiloEstadoGeneral(string estadoGeneral) => estadoGeneral switch
    {
        "Aprobado"        => ("#6ABF4B", "#fff"),
        "Con comentarios" => ("#fd7e14", "#fff"),
        "En proceso"      => ("#0d6efd", "#fff"),
        _                 => ("#D9D9D6", "#111921")
    };
}
