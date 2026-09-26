using System.Globalization;
using System.Text;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Helpers;

/// <summary>
/// Sugiere la disciplina de una actividad del cronograma sin que la usuaria tenga que digitarla
/// cada vez que carga un cronograma nuevo (Excel, PDF, Project o plantilla).
///
/// Orden de resolución:
///   1. Memoria: una actividad con el MISMO nombre (normalizado) ya clasificada en cualquier
///      versión del cronograma del proyecto (o de otro proyecto) conserva esa disciplina.
///   2. Palabras clave del nombre (suministro, ingeniería, cableado, hincado, commissioning…).
///   3. Palabras clave del nombre de la actividad padre (y luego del abuelo, etc.).
/// Si nada aplica, devuelve null y la actividad queda "—" para clasificarla a mano.
/// Solo se clasifican las actividades HOJA: las que tienen hijas promedian el avance de estas
/// y, por definición de la usuaria (2026-09-26), no llevan disciplina.
/// </summary>
public static class DisciplinaSugeridor
{
    public enum Origen { Ninguno, Memoria, PalabraClave, Padre }

    /// <summary>Minúsculas, sin tildes, sin signos y con espacios simples; para comparar nombres.</summary>
    public static string Normalizar(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre)) return string.Empty;
        var descompuesto = nombre.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new StringBuilder(descompuesto.Length);
        var espacioPendiente = false;
        foreach (var ch in descompuesto)
        {
            var cat = CharUnicodeInfo.GetUnicodeCategory(ch);
            if (cat == UnicodeCategory.NonSpacingMark) continue;
            if (char.IsLetterOrDigit(ch))
            {
                if (espacioPendiente && sb.Length > 0) sb.Append(' ');
                espacioPendiente = false;
                sb.Append(ch);
            }
            else
            {
                espacioPendiente = true;
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Construye la memoria nombre → disciplina. Las actividades deben venir ordenadas por
    /// prioridad (primero las del mismo proyecto y de la versión más reciente): la primera
    /// aparición de cada nombre es la que manda.
    /// </summary>
    public static Dictionary<string, Disciplina> ConstruirMemoria(IEnumerable<ActividadWBS> clasificadas)
    {
        var memoria = new Dictionary<string, Disciplina>();
        foreach (var a in clasificadas)
        {
            if (a.Disciplina is null) continue;
            var clave = Normalizar(a.Nombre);
            if (clave.Length == 0 || memoria.ContainsKey(clave)) continue;
            memoria[clave] = a.Disciplina.Value;
        }
        return memoria;
    }

    /// <param name="nombresAncestros">Nombres del padre, abuelo, … (del más cercano al más lejano).</param>
    public static Disciplina? Sugerir(string nombre,
                                      IReadOnlyDictionary<string, Disciplina>? memoria,
                                      IEnumerable<string>? nombresAncestros,
                                      out Origen origen)
    {
        var clave = Normalizar(nombre);

        if (memoria is not null && clave.Length > 0 && memoria.TryGetValue(clave, out var recordada))
        {
            origen = Origen.Memoria;
            return recordada;
        }

        var porClave = PorPalabrasClave(clave);
        if (porClave is not null)
        {
            origen = Origen.PalabraClave;
            return porClave;
        }

        if (nombresAncestros is not null)
        {
            foreach (var ancestro in nombresAncestros)
            {
                var porPadre = PorPalabrasClave(Normalizar(ancestro));
                if (porPadre is null) continue;
                origen = Origen.Padre;
                return porPadre;
            }
        }

        origen = Origen.Ninguno;
        return null;
    }

    // El orden importa: se evalúa de lo más específico a lo más general para que, por ejemplo,
    // "Permisos de construcción" sea Contractual y no Construcción, "Suministro de módulos" sea
    // Suministros y no Mecánica, y "Pruebas eléctricas" sea Puesta en marcha y no Eléctrica.
    private static readonly (Disciplina Disciplina, string[] Claves)[] Reglas =
    [
        (Disciplina.CierreProyecto, ["cierre", "dossier", "as built", "asbuilt", "entrega final", "acta de entrega",
                                     "recepcion provisional", "recepcion definitiva", "liquidacion", "planos record",
                                     "manuales de operacion", "capacitacion al cliente", "garantia"]),
        (Disciplina.PuestaEnMarcha, ["puesta en marcha", "puesta en servicio", "commissioning", "comisionado",
                                     "precomisionado", "pre comisionado", "energizacion", "energizar", "arranque",
                                     "pruebas", "prueba de", "ensayos", "sincronizacion", "inyeccion a la red",
                                     "cod ", "fecha de operacion comercial"]),
        (Disciplina.Ingenieria,     ["ingenier", "estudio", "diseno", "topograf", "hidraul", "hidrolog", "geotec",
                                     "geolog", "plano", "memoria de calculo", "calculo", "levantamiento",
                                     "modelacion", "simulacion", "revision de conclusiones"]),
        (Disciplina.Contractual,    ["contrat", "contractual", "anticipo", "orden de compra", "acuerdo", "permiso",
                                     "licencia", "poliza", "firma", "hito", "hitos", "notificacion", "acta de inicio",
                                     "legalizacion", "aprobacion del cliente", "pago"]),
        (Disciplina.Suministros,    ["suministro", "fabricacion", "transporte", "llegada", "arribo", "importacion",
                                     "nacionalizacion", "entrega de equipos", "entrega de materiales", "recepcion de",
                                     "procura", "compra", "adquisicion", "logistica", "despacho"]),
        // Términos inequívocamente civiles van antes que Eléctrica/Mecánica: "Cimentación de inversores" es Civil
        (Disciplina.Civil,          ["cimentac", "excavac", "movimiento de tierra", "obra civil", "obras civiles", "concreto",
                                     "hormigon", "zanja", "terracer", "explanacion", "descapote", "compactacion", "relleno"]),
        (Disciplina.Electrica,      ["electric", "cablead", "cable", "inversor", "transformador", "media tension",
                                     "baja tension", "alta tension", "subestacion", "puesta a tierra", "malla de tierra",
                                     "string", "canalizacion", "conexionado", "tendido", "iluminacion", "scada",
                                     "comunicaciones", "fibra optica", "celda", "tablero", "bandeja", "ups",
                                     "linea de media", "linea de alta", "linea de baja", "interconexion", "medidor"]),
        (Disciplina.Mecanica,       ["mecanic", "estructura", "modulo", "panel", "tracker", "seguidor", "izaje",
                                     "montaje", "instalacion de mesas", "mesas", "perfiler", "torque"]),
        (Disciplina.Civil,          ["civil", "movimiento de tierra", "excavac", "cimentac", "concreto", "hormigon",
                                     "via", "vias", "camino", "acceso ", "drenaje", "cerramiento", "cerca", "hincado", "pilot",
                                     "zanja", "adecuacion", "descapote", "nivelacion", "terracer", "explanacion",
                                     "relleno", "compactacion", "obra civil", "caseta", "campamento", "canal",
                                     "alcantarill", "topografia de replanteo", "replanteo", "cuneta", "box culvert"]),
        (Disciplina.Construccion,   ["construccion", "obra", "obras", "ejecucion de obra", "frente de trabajo", "movilizacion",
                                     "desmovilizacion", "instalacion", "hse", "seguridad industrial"]),
    ];

    public static Disciplina? PorPalabrasClave(string nombreNormalizado)
    {
        if (string.IsNullOrEmpty(nombreNormalizado)) return null;
        var texto = " " + nombreNormalizado + " ";
        foreach (var (disciplina, claves) in Reglas)
        {
            foreach (var clave in claves)
            {
                // Claves cortas o ambiguas se exigen como palabra completa; las demás como prefijo/substring
                var esPalabraCompleta = clave.Length <= 4 || clave.EndsWith(' ');
                var patron = esPalabraCompleta ? " " + clave.Trim() + " " : clave;
                if (texto.Contains(patron, StringComparison.Ordinal))
                    return disciplina;
            }
        }
        return null;
    }
}
