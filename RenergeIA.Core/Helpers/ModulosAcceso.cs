using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Helpers;

// Catálogo de módulos que se asignan en el panel de permisos y regla que traduce
// cada ruta de la app al permiso que exige. Es la ÚNICA fuente: el menú, la guardia
// del layout y el panel usan estas claves (se guardan en BD, no renombrarlas).
public static class ModulosAcceso
{
    public const string Dashboard = "dashboard";
    public const string Informes = "informes";
    public const string Cronograma = "wbs";
    public const string Documentos = "documentos";
    public const string Costos = "costos";
    public const string HseqSeguridad = "hseq-seguridad";
    public const string HseqCalidad = "hseq-calidad";
    public const string HseqAmbiental = "hseq-ambiental";
    public const string HseqSocial = "hseq-social";
    public const string Restricciones = "restricciones";
    public const string Histogramas = "histogramas";
    public const string Clima = "clima";
    public const string Alertas = "alertas";

    public const string Compromisos = "actas";
    public const string Helvis = "helvis";
    public const string HseqGlobal = "hseq-global";

    public record ModuloInfo(string Clave, string Nombre, string Icono);

    // Módulos dentro de cada proyecto (columnas de la matriz del panel).
    public static readonly ModuloInfo[] DeProyecto =
    [
        new(Dashboard, "Dashboard", "bi-speedometer2"),
        new(Informes, "Informe Diario", "bi-journal-text"),
        new(Cronograma, "Cronograma", "bi-diagram-3"),
        new(Documentos, "Documentos", "bi-folder2-open"),
        new(Costos, "Costos", "bi-currency-dollar"),
        new(HseqSeguridad, "HSEQ Seguridad", "bi-cone-striped"),
        new(HseqCalidad, "HSEQ Calidad", "bi-patch-check"),
        new(HseqAmbiental, "HSEQ Ambiental", "bi-tree"),
        new(HseqSocial, "HSEQ Social", "bi-people"),
        new(Restricciones, "Restricciones", "bi-shield-x"),
        new(Histogramas, "Histogramas", "bi-bar-chart-steps"),
        new(Clima, "Clima", "bi-cloud-sun"),
        new(Alertas, "Alertas", "bi-bell"),
    ];

    // Módulos corporativos (no dependen de un proyecto).
    public static readonly ModuloInfo[] Corporativos =
    [
        new(Compromisos, "Compromisos (actas)", "bi-journal-check"),
        new(Helvis, "Helvis (WhatsApp)", "bi-whatsapp"),
        new(HseqGlobal, "HSEQ corporativo", "bi-shield-check"),
    ];

    public static readonly string[] AreasHseq = [HseqSeguridad, HseqCalidad, HseqAmbiental, HseqSocial];

    public static string NombreNivel(NivelAcceso n) => n switch
    {
        NivelAcceso.Ver => "Ver",
        NivelAcceso.Editar => "Editar",
        NivelAcceso.Aprobar => "Aprobar",
        _ => "Sin acceso"
    };

    public enum TipoRequisito { Libre, Administrador, Proyecto, Corporativo }

    // Modulos: si hay varios basta con tener UNO (portada de HSEQ del proyecto).
    // Modulos vacío en un requisito de proyecto = basta con participar en el proyecto.
    public record Requisito(TipoRequisito Tipo, int? ProyectoId = null, string[]? Modulos = null,
                            NivelAcceso Nivel = NivelAcceso.Ver);

    private static readonly HashSet<string> RutasLibres = new(StringComparer.OrdinalIgnoreCase)
        { "", "login", "logout", "not-found", "error", "proyectos" };

    // Segmentos que indican creación/edición: exigen nivel Editar.
    private static readonly HashSet<string> SegmentosEdicion = new(StringComparer.OrdinalIgnoreCase)
        { "crear", "nuevo", "nueva", "editar", "nueva-inspeccion" };

    private static readonly Dictionary<string, string> ModuloPorSegmento = new(StringComparer.OrdinalIgnoreCase)
    {
        ["dashboard"] = Dashboard,
        ["informes"] = Informes,
        ["wbs"] = Cronograma,
        ["documentos"] = Documentos,
        ["costos"] = Costos,
        ["restricciones"] = Restricciones,
        ["histogramas"] = Histogramas,
        ["clima"] = Clima,
        ["alertas"] = Alertas,
        ["no-conformidades"] = HseqCalidad, // ruta vieja que redirige a HSEQ → Calidad
    };

    // Traduce la ruta (sin dominio ni query) al permiso que exige.
    // Una ruta que no está mapeada exige Administrador: así una página nueva nunca
    // queda abierta por olvido (hay que agregarla aquí).
    public static Requisito RequisitoDeRuta(string ruta)
    {
        var segs = (ruta ?? "").Split('?', '#')[0]
            .Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segs.Length <= 1 && RutasLibres.Contains(segs.Length == 0 ? "" : segs[0]))
            return new(TipoRequisito.Libre);

        var raiz = segs[0].ToLowerInvariant();
        var nivel = segs.Skip(1).Any(SegmentosEdicion.Contains) ? NivelAcceso.Editar : NivelAcceso.Ver;

        switch (raiz)
        {
            case "actas":
                return new(TipoRequisito.Corporativo, Modulos: [Compromisos], Nivel: nivel);
            case "helvis":
                return new(TipoRequisito.Corporativo, Modulos: [Helvis], Nivel: nivel);
            case "hseq":
                return new(TipoRequisito.Corporativo, Modulos: [HseqGlobal], Nivel: nivel);
            case "proyectos":
                break;
            default:
                return new(TipoRequisito.Administrador);
        }

        // /proyectos/nuevo, /proyectos/papelera → solo administradores
        if (!int.TryParse(segs[1], out var proyectoId))
            return new(TipoRequisito.Administrador);

        if (segs.Length == 2)
            return new(TipoRequisito.Proyecto, proyectoId);

        var seg = segs[2].ToLowerInvariant();
        if (seg == "editar") // datos generales del proyecto
            return new(TipoRequisito.Administrador);

        if (seg == "hseq")
        {
            if (segs.Length >= 4 && AreasHseq.Contains("hseq-" + segs[3].ToLowerInvariant()))
                return new(TipoRequisito.Proyecto, proyectoId, ["hseq-" + segs[3].ToLowerInvariant()], nivel);
            return new(TipoRequisito.Proyecto, proyectoId, AreasHseq, NivelAcceso.Ver);
        }

        return ModuloPorSegmento.TryGetValue(seg, out var modulo)
            ? new(TipoRequisito.Proyecto, proyectoId, [modulo], nivel)
            : new(TipoRequisito.Administrador);
    }
}
