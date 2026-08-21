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
}
