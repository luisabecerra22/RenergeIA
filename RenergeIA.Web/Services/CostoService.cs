using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services;

public class CostoService(RenergeIADbContext db)
{
    public async Task<List<Partida>> ObtenerPartidasConCostosAsync(int proyectoId)
    {
        return await db.Partidas
            .Include(p => p.CostosReales)
            .Where(p => p.ProyectoId == proyectoId)
            .OrderBy(p => p.Nivel)
            .ThenBy(p => p.Numero)
            .ThenBy(p => p.Codigo)
            .ToListAsync();
    }

    public static decimal CostoEjecutado(Partida p)
        => p.CostosReales.Sum(c => c.Cantidad * c.PrecioUnitario);

    public static decimal PorcentajeEjecucion(Partida p)
    {
        var presup = p.CantidadPresupuestada * p.PrecioUnitario;
        if (presup == 0) return 0;
        return Math.Round(CostoEjecutado(p) / presup * 100, 1);
    }

    public async Task CargarPlantillaEstandarAsync(int proyectoId)
    {
        var existentes = await db.Partidas.AnyAsync(p => p.ProyectoId == proyectoId);
        if (existentes)
        {
            var todas = await db.Partidas.Where(p => p.ProyectoId == proyectoId).ToListAsync();
            db.Partidas.RemoveRange(todas);
            await db.SaveChangesAsync();
        }

        var principales = PlantillaEstandar();
        foreach (var (principal, subs) in principales)
        {
            principal.ProyectoId = proyectoId;
            db.Partidas.Add(principal);
            await db.SaveChangesAsync();

            foreach (var sub in subs)
            {
                sub.ProyectoId = proyectoId;
                sub.PadreId = principal.Id;
                db.Partidas.Add(sub);
            }
            await db.SaveChangesAsync();
        }
    }

    private static List<(Partida Principal, List<Partida> Subs)> PlantillaEstandar()
    {
        static Partida P(string num, string codigo, string desc, string unidad = "Global", decimal cant = 1, decimal precio = 0) =>
            new() { Numero = num, Codigo = codigo, Descripcion = desc, Unidad = unidad, CantidadPresupuestada = cant, PrecioUnitario = precio, Nivel = 1, EsPrincipal = true };

        static Partida S(string num, string codigo, string desc, string unidad = "Global", decimal cant = 1, decimal precio = 0) =>
            new() { Numero = num, Codigo = codigo, Descripcion = desc, Unidad = unidad, CantidadPresupuestada = cant, PrecioUnitario = precio, Nivel = 2, EsPrincipal = false };

        return
        [
            (P("1","PASP","SUMINISTROS PRINCIPALES"), [
                S("1.1","PASP-001","Módulos fotovoltaicos","Un",1,0),
                S("1.2","PASP-002","Inversores string","Un",1,0),
                S("1.3","PASP-003","Estructura soporte fija","kg",1,0),
                S("1.4","PASP-004","Cable DC solar","m",1,0),
                S("1.5","PASP-005","Cable AC BT","m",1,0),
                S("1.6","PASP-006","Cable MT","m",1,0),
                S("1.7","PASP-007","Transformador MT/BT","Un",1,0),
                S("1.8","PASP-008","Tablero AC principal","Un",1,0),
                S("1.9","PASP-009","Sistema SCADA","Global",1,0),
                S("1.10","PASP-010","Sistema CCTV","Global",1,0),
                S("1.11","PASP-011","Estación meteorológica","Un",1,0),
                S("1.12","PASP-012","Sistema de puesta a tierra","Global",1,0),
                S("1.13","PASP-013","Protecciones MT","Global",1,0),
                S("1.14","PASP-014","Celdas MT","Un",1,0),
                S("1.15","PASP-015","Banco de baterías (si aplica)","Un",1,0),
                S("1.16","PASP-016","Sistema de monitoreo remoto","Global",1,0),
                S("1.17","PASP-017","Medidor bidireccional","Un",1,0),
                S("1.18","PASP-018","Materiales auxiliares y consumibles","Global",1,0),
                S("1.19","PASP-019","Flete y aduanas","Global",1,0),
                S("1.20","PASP-020","Seguros de transporte","Global",1,0),
            ]),
            (P("2","ESSP","TRABAJOS CIVILES"), [
                S("2.1","ESSP-001","Replanteo y descapote","m²",1,0),
                S("2.2","ESSP-002","Limpieza y nivelación del terreno","m²",1,0),
                S("2.3","ESSP-003","Hincado de perfiles","Un",1,0),
                S("2.4","ESSP-004","Excavación zanjas","m³",1,0),
                S("2.5","ESSP-005","Relleno y compactación zanjas","m³",1,0),
                S("2.6","ESSP-006","Construcción vías internas","m²",1,0),
                S("2.7","ESSP-007","Cimentación caseta eléctrica","m³",1,0),
                S("2.8","ESSP-008","Edificio de control","m²",1,0),
                S("2.9","ESSP-009","Caseta de inversores","m²",1,0),
                S("2.10","ESSP-010","Cerramiento perimetral","m",1,0),
                S("2.11","ESSP-011","Pórtico de entrada","Un",1,0),
                S("2.12","ESSP-012","Señalización","Global",1,0),
                S("2.13","ESSP-013","Drenajes pluviales","ml",1,0),
                S("2.14","ESSP-014","Instalaciones hidrosanitarias","Global",1,0),
                S("2.15","ESSP-015","Obras de paisajismo","Global",1,0),
                S("2.16","ESSP-016","Pruebas de suelos","Global",1,0),
                S("2.17","ESSP-017","Pull out test","Un",1,0),
                S("2.18","ESSP-018","Concreto plintos y pedestales","m³",1,0),
                S("2.19","ESSP-019","Limpieza final de obra civil","Global",1,0),
            ]),
            (P("3","MTTC","INSTALACIÓN MECÁNICA"), [
                S("3.1","MTTC-001","Montaje estructura soporte","kg",1,0),
                S("3.2","MTTC-002","Instalación módulos fotovoltaicos","Un",1,0),
                S("3.3","MTTC-003","Montaje inversores string","Un",1,0),
                S("3.4","MTTC-004","Montaje transformador","Un",1,0),
                S("3.5","MTTC-005","Montaje tableros AC","Un",1,0),
                S("3.6","MTTC-006","Montaje celdas MT","Un",1,0),
                S("3.7","MTTC-007","Instalación sistema CCTV","Global",1,0),
                S("3.8","MTTC-008","Instalación estación meteorológica","Un",1,0),
                S("3.9","MTTC-009","Montaje bandejas portacables","m",1,0),
                S("3.10","MTTC-010","Instalación tubería conduit","m",1,0),
                S("3.11","MTTC-011","Montaje sistema puesta a tierra","Global",1,0),
                S("3.12","MTTC-012","Instalación equipos sala control","Global",1,0),
                S("3.13","MTTC-013","Pruebas mecánicas precomisionado","Global",1,0),
                S("3.14","MTTC-014","Torque final conexiones","Global",1,0),
                S("3.15","MTTC-015","Limpieza módulos","Global",1,0),
                S("3.16","MTTC-016","Retiro sobrantes obra mecánica","Global",1,0),
            ]),
            (P("4","HCSP","HOT COMMISSIONING"), [
                S("4.1","HCSP-001","Pruebas de energización y comisionamiento","Global",1,0),
            ]),
            (P("5","CTSP","INSTALACIÓN ELÉCTRICA"), [
                S("5.1","CTSP-001","Cableado DC string - combiner box","m",1,0),
                S("5.2","CTSP-002","Cableado DC combiner box - inversor","m",1,0),
                S("5.3","CTSP-003","Cableado AC inversor - tablero BT","m",1,0),
                S("5.4","CTSP-004","Cableado BT tablero - transformador","m",1,0),
                S("5.5","CTSP-005","Cableado MT transformador - celda","m",1,0),
                S("5.6","CTSP-006","Cableado MT celda - punto de entrega","m",1,0),
                S("5.7","CTSP-007","Conexionado tableros de control","Global",1,0),
                S("5.8","CTSP-008","Conexionado medidor bidireccional","Un",1,0),
                S("5.9","CTSP-009","Instalación pararrayos","Un",1,0),
                S("5.10","CTSP-010","Red de tierra fisica","m",1,0),
                S("5.11","CTSP-011","Pruebas eléctricas - megado y continuidad","Global",1,0),
                S("5.12","CTSP-012","Pruebas de polaridad DC","Global",1,0),
                S("5.13","CTSP-013","Retiro sobrantes obra eléctrica","Global",1,0),
            ]),
            (P("6","CCSP","CCTV"), [
                S("6.1","CCSP-001","Cámaras PTZ perimetral","Un",1,0),
                S("6.2","CCSP-002","DVR / NVR central","Un",1,0),
                S("6.3","CCSP-003","Cableado y ducterías CCTV","Global",1,0),
                S("6.4","CCSP-004","Configuración y pruebas CCTV","Global",1,0),
            ]),
            (P("7","SCSP","SCADA"), [
                S("7.1","SCSP-001","Software SCADA licencia","Global",1,0),
                S("7.2","SCSP-002","Configuración y puesta en marcha SCADA","Global",1,0),
                S("7.3","SCSP-003","Capacitación sistema SCADA","Global",1,0),
            ]),
            (P("8","EMSP","ESTACIONES METEOROLÓGICAS"), [
                S("8.1","EMSP-001","Estación meteorológica completa","Un",1,0),
                S("8.2","EMSP-002","Integración con SCADA","Global",1,0),
            ]),
            (P("9","CGSP","COSTOS GENERALES"), [
                S("9.1","CGSP-001","Ingeniero residente","mes",1,0),
                S("9.2","CGSP-002","Supervisor HSE","mes",1,0),
                S("9.3","CGSP-003","Operario de planta","mes",1,0),
                S("9.4","CGSP-004","Vehículo de campo","mes",1,0),
                S("9.5","CGSP-005","Herramientas y EPP","Global",1,0),
                S("9.6","CGSP-006","Equipos de medición y prueba","Global",1,0),
                S("9.7","CGSP-007","Gestión ambiental y permisos","Global",1,0),
                S("9.8","CGSP-008","Seguros de construcción","Global",1,0),
                S("9.9","CGSP-009","Gastos administrativos de obra","Global",1,0),
            ]),
        ];
    }
}
