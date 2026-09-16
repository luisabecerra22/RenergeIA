using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Enums;
using RenergeIA.Infrastructure.Data;

namespace RenergeIA.Web.Services;

// Histograma de personal por cargo y mes calendario.
// Real = personas distintas por cargo y mes según la nómina de tesorería (Compromisos → Salarios):
//   - la I y II quincena del mismo mes son un solo mes; una persona que cobra en ambas cuenta 1
//   - Primas, Seguridad social y Liquidaciones no son personal en obra
//   - una persona con dos cargos en el mes cuenta en ambos cargos, pero una sola vez en el total
//   - nombres: se quita lo que sigue a " - " (PAGO REBOTADO…) y los parecidos se unifican solo si la usuaria confirma
// Planificado = importado del BOM (hoja H PER, máximo semanal de cada mes) y/o digitado.
public partial class PersonalHistogramaService(RenergeIADbContext db)
{
    public const string Planificado = "Planificado";
    public const string Real = "Real";

    private const string TipoMisma = "PersonaMisma";
    private const string TipoDistinta = "PersonaDistinta";
    private const string TipoCargoBom = "CargoBOM";
    private const string TipoMigracion = "Migracion";

    public sealed record ParNombres(string A, string B, string CargosA, string CargosB);

    public sealed record Nomina(
        Dictionary<(string Cargo, int Anio, int Mes), int> PorCargo,
        Dictionary<(int Anio, int Mes), int> TotalDistintos,
        List<ParNombres> Pendientes,
        (int Anio, int Mes)? UltimoMes,
        (int Anio, int Mes)? PrimerMes);

    // ── Nómina → personas por cargo y mes ──────────────────────────────────

    private static readonly Dictionary<string, int> _meses = new(StringComparer.OrdinalIgnoreCase)
    {
        ["jan"] = 1, ["ene"] = 1, ["feb"] = 2, ["mar"] = 3, ["apr"] = 4, ["abr"] = 4, ["may"] = 5,
        ["jun"] = 6, ["jul"] = 7, ["aug"] = 8, ["ago"] = 8, ["sep"] = 9, ["oct"] = 10,
        ["nov"] = 11, ["dec"] = 12, ["dic"] = 12
    };

    [GeneratedRegex(@"^([A-Za-z]{3})-(\d{4})")]
    private static partial Regex RegexMesPeriodo();

    // "Mar-2026 · II Quincena - 9 Personas" → (2026, 3). Primas, Seguridad social y Liquidaciones no aplican.
    public static (int Anio, int Mes)? MesDePeriodo(string? periodo)
    {
        if (string.IsNullOrWhiteSpace(periodo)) return null;
        var m = RegexMesPeriodo().Match(periodo.Trim());
        if (!m.Success || !_meses.TryGetValue(m.Groups[1].Value, out var mes)) return null;
        return (int.Parse(m.Groups[2].Value), mes);
    }

    public static string CargoDe(HitoCompromiso h)
    {
        var cargo = (h.Detalle ?? "").Split(" · ")[0].Trim();
        return cargo.Length == 0 ? "Sin cargo" : cargo;
    }

    // Mayúsculas, sin tildes, espacios simples y sin agregados después de " - " (PAGO REBOTADO, PAGO EXITOSO…)
    public static string NormalizarNombre(string nombre)
    {
        var baseNombre = nombre.Split(" - ")[0];
        var sinTildes = new StringBuilder();
        foreach (var ch in baseNombre.Normalize(NormalizationForm.FormD))
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark) sinTildes.Append(ch);
        return Regex.Replace(sinTildes.ToString().ToUpperInvariant(), @"\s+", " ").Trim();
    }

    public async Task<Nomina> CalcularNominaAsync(int proyectoId)
    {
        var hitos = await db.HitosCompromiso.AsNoTracking()
            .Where(h => h.CompromisoCosto.ProyectoId == proyectoId && h.CompromisoCosto.Grupo == "Salarios")
            .Select(h => new { h.Periodo, h.Detalle, h.Descripcion, h.Importe })
            .ToListAsync();

        var registros = hitos
            .Select(h => new
            {
                MesAnio = MesDePeriodo(h.Periodo),
                Cargo = CargoDe(new HitoCompromiso { Detalle = h.Detalle }),
                Nombre = NormalizarNombre(h.Descripcion),
                h.Importe
            })
            .Where(x => x.MesAnio is not null && x.Nombre.Length > 0 && EsCargoValido(x.Cargo) && EsPersona(x.Nombre))
            .ToList();

        var decisiones = await db.AsignacionesTesoreria.AsNoTracking()
            .Where(a => a.ProyectoId == proyectoId && (a.Tipo == TipoMisma || a.Tipo == TipoDistinta))
            .ToListAsync();

        // Unificación confirmada (unión de conjuntos; el nombre canónico es el más largo)
        var padre = new Dictionary<string, string>();
        string Raiz(string n)
        {
            while (padre.TryGetValue(n, out var p) && p != n) n = p;
            return n;
        }
        foreach (var d in decisiones.Where(d => d.Tipo == TipoMisma))
        {
            var partes = d.Clave.Split('|');
            if (partes.Length != 2) continue;
            var (ra, rb) = (Raiz(partes[0]), Raiz(partes[1]));
            if (ra == rb) continue;
            if (ra.Length >= rb.Length) padre[rb] = ra; else padre[ra] = rb;
        }

        var porCargo = registros
            .GroupBy(r => (Cargo: CargoClave(r.Cargo), r.MesAnio!.Value.Anio, r.MesAnio.Value.Mes, Persona: Raiz(r.Nombre)))
            .Where(g => g.Sum(x => x.Importe) > 0) // pagos rebotados y reversados no cuentan
            .Select(g => g.Key)
            .ToList();

        var nombreCargo = registros.GroupBy(r => CargoClave(r.Cargo)).ToDictionary(g => g.Key, g => g.First().Cargo);

        var conteo = porCargo
            .GroupBy(k => (Cargo: nombreCargo[k.Cargo], k.Anio, k.Mes))
            .ToDictionary(g => g.Key, g => g.Select(x => x.Persona).Distinct().Count());
        var totales = porCargo
            .GroupBy(k => (k.Anio, k.Mes))
            .ToDictionary(g => g.Key, g => g.Select(x => x.Persona).Distinct().Count());

        // Nombres parecidos aún sin decidir
        var decididos = decisiones.Select(d => d.Clave).ToHashSet();
        var cargosPorNombre = registros.GroupBy(r => r.Nombre)
            .ToDictionary(g => g.Key, g => string.Join(", ", g.Select(x => x.Cargo).Distinct()));
        var nombres = cargosPorNombre.Keys.OrderBy(n => n).ToList();
        var pendientes = new List<ParNombres>();
        for (int i = 0; i < nombres.Count; i++)
            for (int j = i + 1; j < nombres.Count; j++)
            {
                var (a, b) = (nombres[i], nombres[j]);
                if (Raiz(a) == Raiz(b) || decididos.Contains(ClavePar(a, b))) continue;
                if (SonParecidos(a, b)) pendientes.Add(new ParNombres(a, b, cargosPorNombre[a], cargosPorNombre[b]));
            }

        var ultimo = totales.Keys.Count == 0 ? ((int, int)?)null : totales.Keys.OrderBy(k => k.Anio).ThenBy(k => k.Mes).Last();
        var primero = totales.Keys.Count == 0 ? ((int, int)?)null : totales.Keys.OrderBy(k => k.Anio).ThenBy(k => k.Mes).First();
        return new Nomina(conteo, totales, pendientes, ultimo, primero);
    }

    private static string CargoClave(string cargo) => NormalizarNombre(cargo);

    [GeneratedRegex(@"d")]
    private static partial Regex RegexDigito();

    // Un cargo real es un nombre: se descartan encabezados, errores y fechas que el archivo trae en la columna D/E
    // (Fecha de Retiro, Cargo, #N/A, "02-Jul-2026", vacío…)
    public static bool EsCargoValido(string cargo)
    {
        var c = NormalizarNombre(cargo);
        if (c.Length < 3 || c == "SIN CARGO" || c.StartsWith('#') || c.StartsWith("FECHA") || c.StartsWith("CARGO") || c == "FORMA DE PAGO")
            return false;
        return !RegexDigito().IsMatch(c);
    }

    // Filas que no son personas: planillas de seguridad social, liquidaciones, encabezados "XX Personas"
    private static bool EsPersona(string nombre) =>
        !nombre.StartsWith("PLANILLA") && !nombre.StartsWith("LIQUIDACION") && !nombre.Contains("PERSONAS") && !RegexDigito().IsMatch(nombre);

    public static string ClavePar(string a, string b) => string.CompareOrdinal(a, b) <= 0 ? $"{a}|{b}" : $"{b}|{a}";

    private static bool SonParecidos(string a, string b)
    {
        if (Math.Min(a.Length, b.Length) >= 8 && Levenshtein(a, b) <= 3) return true;
        var (ta, tb) = (a.Split(' '), b.Split(' '));
        var (corto, largo) = ta.Length <= tb.Length ? (ta, tb) : (tb, ta);
        return corto.Length >= 2 && corto.Length < largo.Length && corto.SequenceEqual(largo.Take(corto.Length));
    }

    private static int Levenshtein(string a, string b)
    {
        var d = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) d[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) d[0, j] = j;
        for (int i = 1; i <= a.Length; i++)
            for (int j = 1; j <= b.Length; j++)
                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + (a[i - 1] == b[j - 1] ? 0 : 1));
        return d[a.Length, b.Length];
    }

    public async Task DecidirParAsync(int proyectoId, string a, string b, bool mismaPersona)
    {
        var clave = ClavePar(a, b);
        var previas = await db.AsignacionesTesoreria
            .Where(x => x.ProyectoId == proyectoId && (x.Tipo == TipoMisma || x.Tipo == TipoDistinta) && x.Clave == clave)
            .ToListAsync();
        db.AsignacionesTesoreria.RemoveRange(previas);
        db.AsignacionesTesoreria.Add(new AsignacionTesoreria
        {
            ProyectoId = proyectoId,
            Tipo = mismaPersona ? TipoMisma : TipoDistinta,
            Clave = clave,
            Valor = Corta(a.Length >= b.Length ? a : b, 100)
        });
        await db.SaveChangesAsync();
    }

    // Sincroniza las filas Real con la nómina, conservando los ajustes manuales
    public async Task<Nomina> RecalcularRealAsync(int proyectoId)
    {
        var nomina = await CalcularNominaAsync(proyectoId);
        var filas = await db.PersonalHistogramaMeses.Where(f => f.ProyectoId == proyectoId && f.Tipo == Real).ToListAsync();
        var vistas = new HashSet<(string, int, int)>();

        foreach (var f in filas)
        {
            var clave = nomina.PorCargo.Keys.FirstOrDefault(k => k.Anio == f.Anio && k.Mes == f.Mes && CargoClave(k.Cargo) == CargoClave(f.Cargo));
            var auto = clave == default ? 0 : nomina.PorCargo[clave];
            if (clave != default) vistas.Add((CargoClave(clave.Cargo), clave.Anio, clave.Mes));
            f.CantidadNomina = auto;
            if (f.EsManual) continue;
            if (auto == 0) db.PersonalHistogramaMeses.Remove(f);
            else f.Cantidad = auto;
        }

        foreach (var ((cargo, anio, mes), cantidad) in nomina.PorCargo)
        {
            if (vistas.Contains((CargoClave(cargo), anio, mes))) continue;
            db.PersonalHistogramaMeses.Add(new PersonalHistogramaMes
            {
                ProyectoId = proyectoId, Tipo = Real, Cargo = Corta(cargo), Anio = anio, Mes = mes,
                Cantidad = cantidad, CantidadNomina = cantidad
            });
        }
        await db.SaveChangesAsync();
        return nomina;
    }

    // ── Consultas y guardado por ventana de 12 meses ───────────────────────

    public Task<List<PersonalHistogramaMes>> ObtenerFilasAsync(int proyectoId, string tipo) =>
        db.PersonalHistogramaMeses.Where(f => f.ProyectoId == proyectoId && f.Tipo == tipo).ToListAsync();

    public static (int Anio, int Mes) MesDeVentana(int mesInicial, int anioInicial, int posicion)
    {
        var offset = mesInicial - 1 + posicion - 1;
        return (anioInicial + offset / 12, offset % 12 + 1);
    }

    // celdas: cargo → 12 valores de la ventana. renombres: nombre original → nombre nuevo. eliminados: cargos quitados.
    public async Task GuardarPlanificadoAsync(int proyectoId, int mesInicial, int anioInicial,
        List<(string Cargo, decimal[] Valores)> celdas, Dictionary<string, string> renombres, IEnumerable<string> eliminados)
    {
        var filas = await ObtenerFilasAsync(proyectoId, Planificado);
        var quitar = eliminados.Select(CargoClave).ToHashSet();
        db.PersonalHistogramaMeses.RemoveRange(filas.Where(f => quitar.Contains(CargoClave(f.Cargo))));

        foreach (var (original, nuevo) in renombres)
            foreach (var f in filas.Where(f => CargoClave(f.Cargo) == CargoClave(original)))
                f.Cargo = Corta(nuevo);

        var ventana = Enumerable.Range(1, 12).Select(p => MesDeVentana(mesInicial, anioInicial, p)).ToList();
        foreach (var (cargo, valores) in celdas)
        {
            if (string.IsNullOrWhiteSpace(cargo)) continue;
            for (int i = 0; i < 12; i++)
            {
                var (anio, mes) = ventana[i];
                var fila = filas.FirstOrDefault(f => f.Anio == anio && f.Mes == mes && CargoClave(f.Cargo) == CargoClave(cargo) && !quitar.Contains(CargoClave(f.Cargo)));
                if (fila is null && valores[i] != 0)
                    db.PersonalHistogramaMeses.Add(new PersonalHistogramaMes { ProyectoId = proyectoId, Tipo = Planificado, Cargo = Corta(cargo.Trim()), Anio = anio, Mes = mes, Cantidad = valores[i] });
                else if (fila is not null && valores[i] == 0) db.PersonalHistogramaMeses.Remove(fila);
                else if (fila is not null) fila.Cantidad = valores[i];
            }
        }
        await db.SaveChangesAsync();
    }

    // Una celda distinta a la nómina queda como ajuste manual; igual a la nómina vuelve a automática
    public async Task GuardarRealAsync(int proyectoId, int mesInicial, int anioInicial,
        List<(string Cargo, decimal[] Valores)> celdas, IEnumerable<string> eliminados)
    {
        var filas = await ObtenerFilasAsync(proyectoId, Real);
        var quitar = eliminados.Select(CargoClave).ToHashSet();
        db.PersonalHistogramaMeses.RemoveRange(filas.Where(f => quitar.Contains(CargoClave(f.Cargo)) && f.CantidadNomina == 0));

        var ventana = Enumerable.Range(1, 12).Select(p => MesDeVentana(mesInicial, anioInicial, p)).ToList();
        foreach (var (cargo, valores) in celdas)
        {
            if (string.IsNullOrWhiteSpace(cargo)) continue;
            for (int i = 0; i < 12; i++)
            {
                var (anio, mes) = ventana[i];
                var fila = filas.FirstOrDefault(f => f.Anio == anio && f.Mes == mes && CargoClave(f.Cargo) == CargoClave(cargo));
                var auto = fila?.CantidadNomina ?? 0;
                if (fila is null)
                {
                    if (valores[i] != 0)
                        db.PersonalHistogramaMeses.Add(new PersonalHistogramaMes { ProyectoId = proyectoId, Tipo = Real, Cargo = Corta(cargo.Trim()), Anio = anio, Mes = mes, Cantidad = valores[i], EsManual = true });
                    continue;
                }
                fila.EsManual = valores[i] != auto;
                fila.Cantidad = valores[i];
                if (!fila.EsManual && auto == 0) db.PersonalHistogramaMeses.Remove(fila);
            }
        }
        await db.SaveChangesAsync();
    }

    // Pasa los datos del histograma de personal anterior (12 columnas fijas) al modelo por mes, una sola vez
    public async Task AsegurarMigracionAsync(Proyecto proyecto)
    {
        if (await db.AsignacionesTesoreria.AnyAsync(a => a.ProyectoId == proyecto.Id && a.Tipo == TipoMigracion && a.Clave == "HistogramaPersonal"))
            return;

        if (!await db.PersonalHistogramaMeses.AnyAsync(f => f.ProyectoId == proyecto.Id))
        {
            var plan = await db.PlantillasHistograma.AsNoTracking().Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.ProyectoId == proyecto.Id && p.Tipo == TipoHistograma.Personal);
            var real = await db.HistogramasReales.AsNoTracking().Include(h => h.Items)
                .FirstOrDefaultAsync(h => h.ProyectoId == proyecto.Id && h.Tipo == TipoHistograma.Personal);

            foreach (var item in plan?.Items ?? [])
                foreach (var (valor, pos) in HistogramaService.ValoresMes(item).Select((v, i) => (v, i + 1)).Where(x => x.v != 0))
                {
                    var (anio, mes) = MesDeVentana(proyecto.MesInicialHistograma, proyecto.AnioInicialHistograma, pos);
                    db.PersonalHistogramaMeses.Add(new PersonalHistogramaMes { ProyectoId = proyecto.Id, Tipo = Planificado, Cargo = Corta(item.Nombre), Anio = anio, Mes = mes, Cantidad = valor });
                }
            foreach (var item in real?.Items ?? [])
            {
                decimal[] valores = [item.Mes1, item.Mes2, item.Mes3, item.Mes4, item.Mes5, item.Mes6, item.Mes7, item.Mes8, item.Mes9, item.Mes10, item.Mes11, item.Mes12];
                foreach (var (valor, pos) in valores.Select((v, i) => (v, i + 1)).Where(x => x.v != 0))
                {
                    var (anio, mes) = MesDeVentana(proyecto.MesInicialHistograma, proyecto.AnioInicialHistograma, pos);
                    db.PersonalHistogramaMeses.Add(new PersonalHistogramaMes { ProyectoId = proyecto.Id, Tipo = Real, Cargo = Corta(item.Nombre), Anio = anio, Mes = mes, Cantidad = valor, EsManual = true });
                }
            }
        }

        db.AsignacionesTesoreria.Add(new AsignacionTesoreria { ProyectoId = proyecto.Id, Tipo = TipoMigracion, Clave = "HistogramaPersonal", Valor = "ok" });
        await db.SaveChangesAsync();
    }

    // Reemplaza el planificado con una plantilla de 12 posiciones (plantilla por capacidad) en la ventana actual
    public async Task ReemplazarPlanificadoConPlantillaAsync(int proyectoId, int mesInicial, int anioInicial, List<ItemHistograma> items)
    {
        db.PersonalHistogramaMeses.RemoveRange(await ObtenerFilasAsync(proyectoId, Planificado));
        foreach (var item in items)
            foreach (var (valor, pos) in HistogramaService.ValoresMes(item).Select((v, i) => (v, i + 1)).Where(x => x.v != 0))
            {
                var (anio, mes) = MesDeVentana(mesInicial, anioInicial, pos);
                db.PersonalHistogramaMeses.Add(new PersonalHistogramaMes { ProyectoId = proyectoId, Tipo = Planificado, Cargo = Corta(item.Nombre), Anio = anio, Mes = mes, Cantidad = valor });
            }
        await db.SaveChangesAsync();
    }

    // ── Importar planificado desde el BOM (hoja H PER) ─────────────────────

    public sealed record CargoBom(string Nombre, Dictionary<DateTime, decimal> Semanas)
    {
        public decimal Pico => Semanas.Count == 0 ? 0 : Semanas.Values.Max();
        public int SemanasConPersonal => Semanas.Count(s => s.Value > 0);
    }

    public sealed record ResultadoBom(DateTime? FechaInicio, List<CargoBom> Cargos);

    // H PER: fila con fechas de semana (desde la columna G), columna C = tipo (General/Sitio/Operativo), D = cargo
    public static ResultadoBom LeerHPer(XLWorkbook wb)
    {
        var ws = wb.Worksheets.FirstOrDefault(w => w.Name.Trim().Equals("H PER", StringComparison.OrdinalIgnoreCase))
                 ?? throw new InvalidOperationException("El archivo no tiene la hoja \"H PER\" (histograma de personal del BOM).");
        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 0;
        var lastCol = ws.LastColumnUsed()?.ColumnNumber() ?? 0;

        // Fila de fechas: la primera con 3+ fechas desde la columna G
        int filaFechas = 0;
        var fechas = new Dictionary<int, DateTime>();
        for (int r = 1; r <= Math.Min(lastRow, 30) && filaFechas == 0; r++)
        {
            var encontradas = new Dictionary<int, DateTime>();
            for (int c = 7; c <= lastCol; c++)
                if (ws.Cell(r, c).DataType == XLDataType.DateTime) encontradas[c] = ws.Cell(r, c).GetDateTime().Date;
            if (encontradas.Count >= 3) { filaFechas = r; fechas = encontradas; }
        }
        if (filaFechas == 0) throw new InvalidOperationException("No se encontró la fila de fechas semanales en la hoja H PER.");

        DateTime? fechaInicio = null;
        for (int r = 1; r < filaFechas; r++)
            for (int c = 1; c <= 6; c++)
                if (ws.Cell(r, c).GetString().Trim().Equals("Fecha inicio", StringComparison.OrdinalIgnoreCase) &&
                    ws.Cell(r, c + 1).DataType == XLDataType.DateTime)
                    fechaInicio = ws.Cell(r, c + 1).GetDateTime().Date;

        var cargos = new Dictionary<string, CargoBom>(StringComparer.OrdinalIgnoreCase);
        for (int r = filaFechas + 1; r <= lastRow; r++)
        {
            var tipo = ws.Cell(r, 3).GetString().Trim();
            var nombre = ws.Cell(r, 4).GetString().Trim();
            if (tipo.Length == 0 || nombre.Length == 0 || nombre.Equals("Descripción", StringComparison.OrdinalIgnoreCase)) continue;
            if (!cargos.TryGetValue(nombre, out var cargo)) cargos[nombre] = cargo = new CargoBom(nombre, []);
            foreach (var (col, fecha) in fechas)
            {
                var v = BomParser.Num(ws.Cell(r, col)) ?? 0m;
                if (v != 0) cargo.Semanas[fecha] = cargo.Semanas.GetValueOrDefault(fecha) + v;
            }
        }
        return new ResultadoBom(fechaInicio ?? fechas.Values.Min(), cargos.Values.Where(c => c.Pico > 0).ToList());
    }

    public async Task<Dictionary<string, string>> EquivalenciasCargoBomAsync(int proyectoId) =>
        (await db.AsignacionesTesoreria.AsNoTracking()
            .Where(a => a.ProyectoId == proyectoId && a.Tipo == TipoCargoBom).ToListAsync())
        .GroupBy(a => a.Clave).ToDictionary(g => g.Key, g => g.First().Valor);

    public static string ClaveCargoBom(string nombre) => NormalizarNombre(nombre);

    // asignacion: cargo BOM → cargo de nómina (vacío = conserva el nombre del BOM).
    // Se suman por semana los cargos que van al mismo destino y el mes toma el máximo semanal.
    public async Task<int> ImportarBomAsync(int proyectoId, ResultadoBom bom, DateTime fechaInicio, Dictionary<string, string?> asignacion)
    {
        var desplazamiento = fechaInicio.Date - (bom.FechaInicio ?? fechaInicio).Date;
        var semanal = new Dictionary<(string Destino, DateTime Semana), decimal>();
        var nombreDestino = new Dictionary<string, string>();
        foreach (var cargo in bom.Cargos)
        {
            var destino = asignacion.GetValueOrDefault(cargo.Nombre);
            destino = string.IsNullOrWhiteSpace(destino) ? cargo.Nombre : destino.Trim();
            nombreDestino.TryAdd(CargoClave(destino), destino);
            foreach (var (semana, valor) in cargo.Semanas)
            {
                var k = (CargoClave(destino), semana + desplazamiento);
                semanal[k] = semanal.GetValueOrDefault(k) + valor;
            }
        }

        var mensual = semanal
            .GroupBy(x => (x.Key.Destino, x.Key.Semana.Year, x.Key.Semana.Month))
            .ToDictionary(g => g.Key, g => g.Max(x => x.Value));

        db.PersonalHistogramaMeses.RemoveRange(await ObtenerFilasAsync(proyectoId, Planificado));
        foreach (var ((destino, anio, mes), cantidad) in mensual.Where(m => m.Value > 0))
            db.PersonalHistogramaMeses.Add(new PersonalHistogramaMes
            {
                ProyectoId = proyectoId, Tipo = Planificado, Cargo = Corta(nombreDestino[destino]),
                Anio = anio, Mes = mes, Cantidad = Math.Ceiling(cantidad)
            });

        // Recordar las equivalencias usadas
        var previas = await db.AsignacionesTesoreria.Where(a => a.ProyectoId == proyectoId && a.Tipo == TipoCargoBom).ToListAsync();
        foreach (var (cargoBom, destino) in asignacion.Where(a => !string.IsNullOrWhiteSpace(a.Value)))
        {
            var clave = ClaveCargoBom(cargoBom);
            var previa = previas.FirstOrDefault(p => p.Clave == clave);
            if (previa is null)
                db.AsignacionesTesoreria.Add(new AsignacionTesoreria { ProyectoId = proyectoId, Tipo = TipoCargoBom, Clave = clave, Valor = Corta(destino!.Trim(), 100) });
            else previa.Valor = Corta(destino!.Trim(), 100);
        }

        await db.SaveChangesAsync();
        return mensual.Count(m => m.Value > 0);
    }

    private static string Corta(string s, int max = 150) => s.Length > max ? s[..max] : s;
}
