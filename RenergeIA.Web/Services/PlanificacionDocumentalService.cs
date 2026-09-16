using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Enums;
using RenergeIA.Core.Helpers;
using RenergeIA.Infrastructure.Data;
using static RenergeIA.Web.Services.PlanificacionDocumentalParser;

namespace RenergeIA.Web.Services;

// Carga de la Planificación Documental por proyecto: actualiza (no duplica) los documentos por código.
// Si un documento fue editado en la app después de la última carga, se conservan sus valores y se
// informan las diferencias; la usuaria puede elegir usar los del Excel documento por documento.
public class PlanificacionDocumentalService(RenergeIADbContext db)
{
    public enum TipoCambio { Nuevo, Actualizado, SinCambios, Conflicto }

    public sealed class CambioDocumento
    {
        public FilaPlan Fila { get; init; } = null!;
        public Documento? Existente { get; init; }
        public TipoCambio Tipo { get; set; }
        public List<(string Campo, string Antes, string Despues)> Diferencias { get; } = [];
        /// <summary>Solo aplica a conflictos: true = usar los valores del Excel sobre lo editado en la app.</summary>
        public bool UsarExcel { get; set; }
    }

    public sealed class Analisis
    {
        public string Archivo { get; init; } = "";
        public Resultado Lectura { get; init; } = null!;
        public DateTime? FechaCargada { get; init; }
        public string? ArchivoCargado { get; init; }
        public List<CambioDocumento> Cambios { get; } = [];
        public List<Documento> NoEnArchivo { get; } = [];

        public bool EsAnterior => Lectura.FechaActualizacion.HasValue && FechaCargada.HasValue
                                  && Lectura.FechaActualizacion.Value.Date < FechaCargada.Value.Date;
        public int Cuenta(TipoCambio t) => Cambios.Count(c => c.Tipo == t);
        public int Cuenta(TipoCambio t, CategoriaDocumento cat) => Cambios.Count(c => c.Tipo == t && c.Fila.Categoria == cat);
        public IEnumerable<FilaPlan> ConAdvertencias => Cambios.Select(c => c.Fila).Where(f => f.Advertencias.Count > 0);
    }

    public async Task<Analisis> AnalizarAsync(int proyectoId, Stream archivo, string nombreArchivo)
    {
        using var ms = new MemoryStream();
        await archivo.CopyToAsync(ms);
        ms.Position = 0;
        var lectura = Leer(ms);

        var proyecto = await db.Proyectos.AsNoTracking().FirstAsync(p => p.Id == proyectoId);
        var existentes = await db.Documentos.Where(d => d.ProyectoId == proyectoId).ToListAsync();

        return Comparar(lectura, existentes, nombreArchivo, proyecto.FechaActualizacionPlanDocumental, proyecto.ArchivoPlanDocumental);
    }

    public static Analisis Comparar(Resultado lectura, IReadOnlyList<Documento> existentes, string nombreArchivo,
        DateTime? fechaCargada, string? archivoCargado)
    {
        var analisis = new Analisis
        {
            Archivo = nombreArchivo,
            Lectura = lectura,
            FechaCargada = fechaCargada,
            ArchivoCargado = archivoCargado
        };

        // Códigos repetidos dentro del mismo archivo (ej. ITM01 dos veces): no sirven solos para emparejar
        var repetidos = lectura.Filas
            .SelectMany(f => new[] { (f.Categoria, "C:" + Clave(f.Codigo)), (f.Categoria, "L:" + Clave(f.CodigoCliente)) })
            .Where(x => x.Item2.Length > 2)
            .GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToHashSet();

        var usados = new HashSet<int>();
        foreach (var fila in lectura.Filas)
        {
            var candidatos = existentes.Where(d => d.Categoria == fila.Categoria && !usados.Contains(d.Id)).ToList();
            var doc = Emparejar(fila, candidatos,
                codigoRepetido: repetidos.Contains((fila.Categoria, "C:" + Clave(fila.Codigo))),
                clienteRepetido: repetidos.Contains((fila.Categoria, "L:" + Clave(fila.CodigoCliente))));
            var cambio = new CambioDocumento { Fila = fila, Existente = doc };

            if (doc is null)
            {
                cambio.Tipo = TipoCambio.Nuevo;
            }
            else
            {
                usados.Add(doc.Id);
                CalcularDiferencias(doc, fila, cambio.Diferencias);
                var editadoEnApp = doc.FechaEdicionApp.HasValue
                                   && (doc.FechaUltimaImportacion is null || doc.FechaEdicionApp > doc.FechaUltimaImportacion);
                cambio.Tipo = cambio.Diferencias.Count == 0 ? TipoCambio.SinCambios
                            : editadoEnApp ? TipoCambio.Conflicto
                            : TipoCambio.Actualizado;
            }
            analisis.Cambios.Add(cambio);
        }

        var categoriasLeidas = lectura.Hojas.Keys.ToHashSet();
        analisis.NoEnArchivo.AddRange(existentes.Where(d => categoriasLeidas.Contains(d.Categoria) && !usados.Contains(d.Id))
                                                .OrderBy(d => d.Categoria).ThenBy(d => d.Codigo));
        return analisis;
    }

    public async Task<(int Nuevos, int Actualizados, int Conservados)> AplicarAsync(int proyectoId, Analisis analisis, string? usuario)
    {
        var ahora = DateTime.UtcNow;
        int nuevos = 0, actualizados = 0, conservados = 0;

        foreach (var c in analisis.Cambios)
        {
            switch (c.Tipo)
            {
                case TipoCambio.Nuevo:
                    var doc = new Documento
                    {
                        ProyectoId = proyectoId,
                        Categoria = c.Fila.Categoria,
                        Codigo = c.Fila.Codigo ?? "",
                        Titulo = c.Fila.Titulo ?? "Sin título",
                        FechaUltimaImportacion = ahora
                    };
                    Copiar(c.Fila, doc);
                    db.Documentos.Add(doc);
                    nuevos++;
                    break;

                case TipoCambio.Actualizado:
                case TipoCambio.Conflicto when c.UsarExcel:
                    var d = await db.Documentos.FirstAsync(x => x.Id == c.Existente!.Id);
                    Copiar(c.Fila, d);
                    d.FechaUltimaImportacion = ahora;
                    d.FechaModificacion = ahora;
                    actualizados++;
                    break;

                case TipoCambio.Conflicto:
                    conservados++;
                    break;

                case TipoCambio.SinCambios:
                    var s = await db.Documentos.FirstAsync(x => x.Id == c.Existente!.Id);
                    s.FechaUltimaImportacion = ahora;
                    break;
            }
        }

        var proyecto = await db.Proyectos.FirstAsync(p => p.Id == proyectoId);
        if (analisis.Lectura.FechaActualizacion.HasValue
            && (proyecto.FechaActualizacionPlanDocumental is null || analisis.Lectura.FechaActualizacion >= proyecto.FechaActualizacionPlanDocumental))
            proyecto.FechaActualizacionPlanDocumental = DateTime.SpecifyKind(analisis.Lectura.FechaActualizacion.Value, DateTimeKind.Utc);
        proyecto.ArchivoPlanDocumental = analisis.Archivo;

        await db.SaveChangesAsync();
        return (nuevos, actualizados, conservados);
    }

    // ── Emparejamiento: Código Renergeia + Cliente → Código Renergeia → Código Cliente → Nombre ──

    private static Documento? Emparejar(FilaPlan f, List<Documento> candidatos, bool codigoRepetido, bool clienteRepetido)
    {
        var cod = Clave(f.Codigo);
        var cli = Clave(f.CodigoCliente);
        var tit = Clave(f.Titulo);

        Documento? Unico(Func<Documento, bool> pred)
        {
            var l = candidatos.Where(pred).Take(2).ToList();
            return l.Count == 1 ? l[0] : null;
        }
        // El documento de la app no tiene un Código Renergeia propio (vacío o generado: IMP-001, DOC-001) o coincide
        bool CodigoCompatible(Documento d)
        {
            var c = Clave(d.Codigo);
            return cod.Length == 0 || c.Length == 0 || c == cod || c.StartsWith("IMP-") || c.StartsWith("DOC-");
        }

        if (cod.Length > 0 && cli.Length > 0
            && candidatos.FirstOrDefault(x => Clave(x.Codigo) == cod && Clave(x.CodigoCliente) == cli) is { } exacto)
            return exacto;
        if (cod.Length > 0 && !codigoRepetido && Unico(x => Clave(x.Codigo) == cod) is { } porCodigo)
            return porCodigo;
        if (cli.Length > 0 && !clienteRepetido && Unico(x => Clave(x.CodigoCliente) == cli) is { } porCliente && CodigoCompatible(porCliente))
            return porCliente;
        if (tit.Length > 0 && Unico(x => Clave(x.Titulo) == tit) is { } porTitulo && CodigoCompatible(porTitulo)
            && (cli.Length == 0 || Clave(porTitulo.CodigoCliente).Length == 0 || Clave(porTitulo.CodigoCliente) == cli))
            return porTitulo;
        return null;
    }

    private static string Clave(string? s) => Normalizar(s).Replace(" ", "").ToUpperInvariant();

    // ── Diferencias y copia: una celda vacía en el Excel nunca borra lo que ya está en la app ──

    private static void CalcularDiferencias(Documento d, FilaPlan f, List<(string, string, string)> dif)
    {
        void T(string campo, string? actual, string? nuevo)
        {
            if (nuevo is not null && (actual ?? "").Trim() != nuevo.Trim()) dif.Add((campo, actual ?? "—", nuevo));
        }
        void F(string campo, DateTime? actual, DateTime? nuevo)
        {
            if (nuevo.HasValue && actual?.Date != nuevo.Value.Date)
                dif.Add((campo, actual?.ToString("dd/MM/yyyy") ?? "—", nuevo.Value.ToString("dd/MM/yyyy")));
        }
        void V<TV>(string campo, TV actual, TV? nuevo, Func<TV, string> fmt) where TV : struct
        {
            if (nuevo.HasValue && !EqualityComparer<TV>.Default.Equals(actual, nuevo.Value))
                dif.Add((campo, fmt(actual), fmt(nuevo.Value)));
        }
        void N<TV>(string campo, TV? actual, TV? nuevo, Func<TV, string> fmt) where TV : struct
        {
            if (nuevo.HasValue && !Nullable.Equals(actual, nuevo))
                dif.Add((campo, actual.HasValue ? fmt(actual.Value) : "—", fmt(nuevo.Value)));
        }
        static string Si(bool b) => b ? "SI" : "NO";

        T("Código cliente", d.CodigoCliente, f.CodigoCliente);
        T("Código Renergeia", d.Codigo, f.Codigo);
        T("Nombre", d.Titulo, f.Titulo);
        V("Estado", d.Estado, f.Estado, e => EnumDisplay.Mostrar(e));
        T("Versión", d.Version, f.Version);
        F("Emisión", d.FechaEmision, f.FechaEmision);
        F("Entrega 1", d.FechaEntrega1, f.FechaEntrega1);
        F("Devolución 1", d.FechaDevolucion1, f.FechaDevolucion1);
        F("Entrega 2", d.FechaEntrega2, f.FechaEntrega2);
        F("Devolución 2", d.FechaDevolucion2, f.FechaDevolucion2);
        F("Entrega 3", d.FechaEntrega3, f.FechaEntrega3);
        F("Devolución 3", d.FechaDevolucion3, f.FechaDevolucion3);
        F("Entrega 4", d.FechaEntrega4, f.FechaEntrega4);
        F("Devolución 4", d.FechaDevolucion4, f.FechaDevolucion4);
        F("Validación", d.FechaValidacion, f.FechaValidacion);
        V("Fase", d.Fase, f.Fase, x => $"Fase {x}");
        V("Área", d.Area, f.Area, a => EnumDisplay.Mostrar(a));
        T("Transmittal", d.Transmittal, f.Transmittal);
        T("Observaciones", d.Observaciones, f.Observaciones);
        T("Responsable", d.Responsable, f.Responsable);
        T("Observación del retraso", d.ObservacionRetraso, f.ObservacionRetraso);
        T("Observaciones Redline", d.ObservacionRedline, f.ObservacionRedline);
        N("¿Registra cambios?", d.RegistraCambios, f.RegistraCambios, Si);
        T("Responsable Redline", d.ResponsableRedline, f.ResponsableRedline);
        N("¿Requiere Redline?", d.RequiereRedline, f.RequiereRedline, Si);
        N("% Redline", d.AvanceRedline, f.AvanceRedline, x => $"{x:0.#}%");
        N("Redline aprobado Interventoría", d.RedlineAprobadoInterventoria, f.RedlineAprobadoInterventoria, Si);
        N("% As-Built", d.AvanceAsBuilt, f.AvanceAsBuilt, x => $"{x:0.#}%");
        T("Responsable As-Built", d.ResponsableAsBuilt, f.ResponsableAsBuilt);
        N("As-Built aprobado Interventoría", d.AsBuiltAprobadoInterventoria, f.AsBuiltAprobadoInterventoria, Si);
        T("Observaciones As-Built", d.ObservacionAsBuilt, f.ObservacionAsBuilt);
    }

    private static DateTime? Utc(DateTime? f) => f.HasValue ? DateTime.SpecifyKind(f.Value.Date, DateTimeKind.Utc) : null;

    public static void Copiar(FilaPlan f, Documento d)
    {
        if (f.CodigoCliente is not null) d.CodigoCliente = f.CodigoCliente;
        if (f.Codigo is not null) d.Codigo = f.Codigo;
        if (f.Titulo is not null) d.Titulo = f.Titulo;
        if (f.Estado.HasValue) d.Estado = f.Estado.Value;
        if (f.Version is not null) d.Version = f.Version;
        d.FechaEmision = Utc(f.FechaEmision) ?? d.FechaEmision;
        d.FechaEntrega1 = Utc(f.FechaEntrega1) ?? d.FechaEntrega1;
        d.FechaDevolucion1 = Utc(f.FechaDevolucion1) ?? d.FechaDevolucion1;
        d.FechaEntrega2 = Utc(f.FechaEntrega2) ?? d.FechaEntrega2;
        d.FechaDevolucion2 = Utc(f.FechaDevolucion2) ?? d.FechaDevolucion2;
        d.FechaEntrega3 = Utc(f.FechaEntrega3) ?? d.FechaEntrega3;
        d.FechaDevolucion3 = Utc(f.FechaDevolucion3) ?? d.FechaDevolucion3;
        d.FechaEntrega4 = Utc(f.FechaEntrega4) ?? d.FechaEntrega4;
        d.FechaDevolucion4 = Utc(f.FechaDevolucion4) ?? d.FechaDevolucion4;
        d.FechaValidacion = Utc(f.FechaValidacion) ?? d.FechaValidacion;
        if (f.Fase.HasValue) d.Fase = f.Fase.Value;
        if (f.Area.HasValue) d.Area = f.Area.Value;
        if (f.Transmittal is not null) d.Transmittal = f.Transmittal;
        if (f.Observaciones is not null) d.Observaciones = f.Observaciones;
        if (f.Responsable is not null) d.Responsable = f.Responsable;
        if (f.ObservacionRetraso is not null) d.ObservacionRetraso = f.ObservacionRetraso;
        if (f.ObservacionRedline is not null) d.ObservacionRedline = f.ObservacionRedline;
        d.RegistraCambios = f.RegistraCambios ?? d.RegistraCambios;
        if (f.ResponsableRedline is not null) d.ResponsableRedline = f.ResponsableRedline;
        d.RequiereRedline = f.RequiereRedline ?? d.RequiereRedline;
        d.AvanceRedline = f.AvanceRedline ?? d.AvanceRedline;
        d.RedlineAprobadoInterventoria = f.RedlineAprobadoInterventoria ?? d.RedlineAprobadoInterventoria;
        d.AvanceAsBuilt = f.AvanceAsBuilt ?? d.AvanceAsBuilt;
        if (f.ResponsableAsBuilt is not null) d.ResponsableAsBuilt = f.ResponsableAsBuilt;
        d.AsBuiltAprobadoInterventoria = f.AsBuiltAprobadoInterventoria ?? d.AsBuiltAprobadoInterventoria;
        if (f.ObservacionAsBuilt is not null) d.ObservacionAsBuilt = f.ObservacionAsBuilt;
    }
}
