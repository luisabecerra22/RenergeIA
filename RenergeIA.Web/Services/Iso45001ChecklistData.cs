namespace RenergeIA.Web.Services;

public static class Iso45001ChecklistData
{
    public record RequisitoISO(
        string NumClausula,
        string TituloMain,
        string SubClausula,
        string Id,
        string Req,
        string Interp,
        string[] Docs
    );

    public static readonly IReadOnlyList<RequisitoISO> Requisitos =
    [
        // ══ 4. CONTEXTO DE LA ORGANIZACIÓN ══════════════════════════════════════
        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.1 Comprensión de la organización y su contexto","4.1.1",
            "Se han determinado las cuestiones internas y externas pertinentes que afectan los resultados previstos del SG-SST.",
            "Deben identificarse factores internos (cultura, estructura, recursos, condiciones y organización del trabajo) y externos (legales, tecnológicos, sociales, de mercado, ambientales) que influyen en la SST. Habitualmente se evidencia con matriz DOFA / PESTEL.",
            ["Matriz DOFA / PESTEL","Análisis de contexto de la organización","Planeación estratégica vigente","Acta de revisión por la dirección"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.1 Comprensión de la organización y su contexto","4.1.2",
            "La organización realiza seguimiento y revisión de la información sobre dichas cuestiones internas y externas.",
            "El contexto no es estático: debe revisarse periódicamente (mínimo en la revisión por la dirección) y actualizarse cuando cambian las condiciones del negocio o del entorno.",
            ["Acta de actualización del análisis de contexto","Registro de revisión por la dirección","Control de cambios del documento de contexto"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.2 Necesidades y expectativas de trabajadores y partes interesadas","4.2.1",
            "Se han identificado las partes interesadas pertinentes al SG-SST (trabajadores, contratistas, clientes, autoridades, etc.).",
            "Incluye trabajadores y sus representantes y otras partes (ARL, clientes, autoridades, comunidad, proveedores). Se documenta en una matriz de partes interesadas.",
            ["Matriz de partes interesadas","Registro de identificación de grupos de interés","Mapa de stakeholders del proyecto"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.2 Necesidades y expectativas de trabajadores y partes interesadas","4.2.2",
            "Se han determinado las necesidades, expectativas y cuáles de ellas se convierten en requisitos legales y otros requisitos.",
            "De cada parte interesada se establecen sus necesidades/expectativas y se define cuáles asume la organización como requisito obligatorio o voluntario.",
            ["Matriz de partes interesadas con requisitos","Matriz de requisitos legales y otros","Contratos y obligaciones contractuales SST"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.3 Determinación del alcance del SG-SST","4.3.1",
            "El alcance del SG-SST está definido considerando cuestiones, partes interesadas y actividades de la organización.",
            "El alcance debe abarcar las actividades, productos y servicios bajo control de la organización que impactan el desempeño de SST, incluyendo sedes y proyectos.",
            ["Documento de alcance del SG-SST","Manual del SG-SST","Política de SST"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.3 Determinación del alcance del SG-SST","4.3.2",
            "El alcance está disponible y se mantiene como información documentada.",
            "El alcance debe estar escrito, controlado, accesible y comunicado a las partes pertinentes.",
            ["Información documentada de alcance (controlada)","Listado maestro de documentos","Evidencia de divulgación del alcance"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.4 Sistema de Gestión de SST","4.4.1",
            "Se ha establecido, implementado, mantenido y se mejora continuamente el SG-SST conforme a la norma.",
            "Existe un sistema real y operando (no solo en papel), con ciclo PHVA evidenciable y recursos asignados.",
            ["Manual / estructura del SG-SST","Plan de trabajo anual (PHVA)","Indicadores del sistema"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.4 Sistema de Gestión de SST","4.4.2",
            "Se han determinado los procesos necesarios y sus interacciones dentro del SG-SST.",
            "Se identifican los procesos del sistema y cómo se relacionan entre sí (mapa de procesos), incluyendo entradas, salidas y responsables.",
            ["Mapa de procesos","Caracterización de procesos","Diagrama de interacción de procesos"]),

        // ══ 5. LIDERAZGO Y PARTICIPACIÓN DE LOS TRABAJADORES ════════════════════
        new("5","5. LIDERAZGO Y PARTICIPACIÓN DE LOS TRABAJADORES",
            "5.1 Liderazgo y compromiso","5.1.1",
            "La alta dirección asume la responsabilidad y rendición de cuentas por la eficacia del SG-SST.",
            "La dirección demuestra liderazgo: protege a trabajadores, garantiza recursos, comunica importancia de la SST y participa activamente. No es delegable totalmente.",
            ["Acta de revisión por la dirección","Evidencia de participación de gerencia en SST","Asignación de recursos aprobada"]),

        new("5","5. LIDERAZGO Y PARTICIPACIÓN DE LOS TRABAJADORES",
            "5.1 Liderazgo y compromiso","5.1.2",
            "El SG-SST está integrado a los procesos de negocio y se garantizan los recursos necesarios.",
            "La SST no funciona aislada: se integra a la operación, presupuesto y toma de decisiones. La dirección asegura recursos humanos, técnicos y financieros.",
            ["Presupuesto de SST aprobado","Procedimientos que integran SST a la operación","Acta de asignación de recursos"]),

        new("5","5. LIDERAZGO Y PARTICIPACIÓN DE LOS TRABAJADORES",
            "5.2 Política de SST","5.2.1",
            "La política de SST incluye compromisos de condiciones seguras, eliminación de peligros y reducción de riesgos.",
            "La política debe incluir: condiciones de trabajo seguras, cumplimiento de requisitos legales, eliminación de peligros, mejora continua y consulta/participación de los trabajadores.",
            ["Política de SST firmada y vigente","Registro de revisión de la política","Acta de aprobación de la dirección"]),

        new("5","5. LIDERAZGO Y PARTICIPACIÓN DE LOS TRABAJADORES",
            "5.2 Política de SST","5.2.2",
            "La política está documentada, comunicada, disponible y se revisa periódicamente.",
            "Debe estar publicada, divulgada a trabajadores y disponible a partes interesadas; se revisa al menos una vez al año.",
            ["Evidencia de divulgación (firmas/registros)","Política publicada en sitios visibles","Registro de actualización anual"]),

        new("5","5. LIDERAZGO Y PARTICIPACIÓN DE LOS TRABAJADORES",
            "5.3 Roles, responsabilidades y autoridades","5.3.1",
            "Las responsabilidades y autoridades en SST están asignadas, documentadas y comunicadas en todos los niveles.",
            "Cada nivel jerárquico tiene responsabilidades de SST definidas en manuales de funciones o matrices de responsabilidades.",
            ["Manual de funciones con responsabilidades SST","Matriz de roles y responsabilidades","Perfiles de cargo / profesiogramas"]),

        new("5","5. LIDERAZGO Y PARTICIPACIÓN DE LOS TRABAJADORES",
            "5.3 Roles, responsabilidades y autoridades","5.3.2",
            "Se ha asignado la responsabilidad de informar el desempeño del SG-SST a la alta dirección.",
            "Existe un responsable designado (ej. coordinador HSEQ) que rinde cuentas del desempeño del sistema a la dirección.",
            ["Designación del responsable del SG-SST","Informes de desempeño a la dirección","Acta de nombramiento"]),

        new("5","5. LIDERAZGO Y PARTICIPACIÓN DE LOS TRABAJADORES",
            "5.4 Consulta y participación de los trabajadores","5.4.1",
            "Existen mecanismos para la consulta y participación de los trabajadores en el SG-SST.",
            "Se consulta a los trabajadores (especialmente no directivos) en identificación de peligros, investigación de incidentes, definición de controles, etc. Aplica el COPASST/Vigía.",
            ["Actas de COPASST / Vigía SST","Encuestas o buzones de participación","Registros de consulta en IPEVR"]),

        new("5","5. LIDERAZGO Y PARTICIPACIÓN DE LOS TRABAJADORES",
            "5.4 Consulta y participación de los trabajadores","5.4.2",
            "Se eliminan los obstáculos a la participación (tiempo, idioma, represalias) y se brindan recursos.",
            "La organización facilita la participación: tiempo dentro de la jornada, formación, acceso a información y protección contra represalias.",
            ["Procedimiento de participación","Evidencia de tiempo asignado para SST","Política de no represalias"]),

        // ══ 6. PLANIFICACIÓN ════════════════════════════════════════════════════
        new("6","6. PLANIFICACIÓN",
            "6.1.2 Identificación de peligros y evaluación de riesgos","6.1.2.1",
            "Existe un proceso continuo y proactivo de identificación de peligros (rutinarios, no rutinarios, emergencias, factor humano).",
            "La IPEVR debe considerar actividades rutinarias y no rutinarias, factor humano, cambios, situaciones de emergencia, incidentes pasados y diseño de áreas. En Colombia se aplica la GTC 45.",
            ["Matriz IPEVR","Procedimiento de identificación de peligros","GTC 45","Registros de participación de trabajadores"]),

        new("6","6. PLANIFICACIÓN",
            "6.1.2 Identificación de peligros y evaluación de riesgos","6.1.2.2",
            "La evaluación de riesgos aplica la jerarquía de controles y valora la eficacia de los controles existentes.",
            "Los controles se priorizan: eliminar > sustituir > controles de ingeniería > administrativos > EPP. Debe valorarse riesgo residual.",
            ["Matriz IPEVR con jerarquía de controles","Evaluación de riesgo residual","Plan de intervención de riesgos prioritarios"]),

        new("6","6. PLANIFICACIÓN",
            "6.1.3 Determinación de requisitos legales y otros requisitos","6.1.3.1",
            "Se identifican y mantienen actualizados los requisitos legales y otros requisitos aplicables.",
            "Matriz legal actualizada (Decreto 1072, Resolución 0312, RETIE, etc.) con seguimiento a cambios normativos.",
            ["Matriz de requisitos legales (normograma)","Procedimiento de actualización legal","Evidencia de revisión periódica"]),

        new("6","6. PLANIFICACIÓN",
            "6.1.3 Determinación de requisitos legales y otros requisitos","6.1.3.2",
            "Se determina cómo aplican esos requisitos a la organización y se comunican.",
            "No basta listar normas: debe definirse cómo aplica cada una a la operación y comunicarse a los responsables.",
            ["Matriz legal con análisis de aplicabilidad","Evidencia de comunicación a responsables","Evaluación de cumplimiento legal"]),

        new("6","6. PLANIFICACIÓN",
            "6.1.4 Planificación de acciones","6.1.4.1",
            "Se planifican acciones para abordar riesgos, oportunidades y requisitos legales identificados.",
            "Para cada riesgo/oportunidad/requisito se definen acciones concretas, integradas al plan de trabajo anual.",
            ["Plan de acción de SST","Plan de trabajo anual","Cronograma de actividades"]),

        new("6","6. PLANIFICACIÓN",
            "6.1.4 Planificación de acciones","6.1.4.2",
            "Las acciones se integran a los procesos del SG-SST y se evalúa su eficacia.",
            "Las acciones no son aisladas: se implementan en los procesos y se mide si lograron el resultado esperado.",
            ["Seguimiento de acciones","Indicadores de eficacia","Registros de cierre de acciones"]),

        new("6","6. PLANIFICACIÓN",
            "6.2 Objetivos de SST y planificación para lograrlos","6.2.1",
            "Los objetivos de SST son medibles, coherentes con la política y se han establecido en funciones y niveles pertinentes.",
            "Objetivos SMART, alineados a la política, comunicados y actualizados. Deben tener metas medibles.",
            ["Documento de objetivos y metas de SST","Tablero de indicadores","Política de SST (coherencia)"]),

        new("6","6. PLANIFICACIÓN",
            "6.2 Objetivos de SST y planificación para lograrlos","6.2.2",
            "Se planifica qué se hará, recursos, responsable, plazo y cómo se evaluarán los resultados.",
            "Cada objetivo tiene un plan: qué, recursos necesarios, responsable, fecha límite e indicador de medición.",
            ["Plan de logro de objetivos","Cronograma con responsables","Ficha técnica de indicadores"]),

        // ══ 7. APOYO ════════════════════════════════════════════════════════════
        new("7","7. APOYO",
            "7.1 Recursos","7.1.1",
            "Se han determinado y proporcionado los recursos necesarios para el SG-SST.",
            "Incluye recursos humanos, financieros, tecnológicos y de infraestructura para implementar y mantener el sistema.",
            ["Presupuesto de SST","Inventario de recursos asignados","Acta de asignación de recursos"]),

        new("7","7. APOYO",
            "7.1 Recursos","7.1.2",
            "Los recursos se mantienen disponibles para la operación y mejora continua del sistema.",
            "No es un gasto puntual: debe garantizarse continuidad de recursos en el tiempo.",
            ["Ejecución presupuestal SST","Plan plurianual de recursos","Evidencia de continuidad de contratos clave"]),

        new("7","7. APOYO",
            "7.2 Competencia","7.2.1",
            "Se determina la competencia necesaria de los trabajadores que afectan el desempeño de SST.",
            "Definir educación, formación y experiencia requeridas por cargo, especialmente en tareas de alto riesgo.",
            ["Perfiles de cargo con competencias","Matriz de competencias","Profesiogramas"]),

        new("7","7. APOYO",
            "7.2 Competencia","7.2.2",
            "Se toman acciones para adquirir competencia y se conservan registros como evidencia.",
            "Formación, capacitación, tutoría o reasignación, con evaluación de eficacia y registros conservados.",
            ["Plan de capacitación / formación","Certificados y registros de asistencia","Evaluaciones de eficacia de la formación"]),

        new("7","7. APOYO",
            "7.3 Toma de conciencia","7.3.1",
            "Los trabajadores son conscientes de la política, los peligros y riesgos pertinentes a su labor.",
            "Los trabajadores conocen la política, los peligros de su puesto y su contribución a la eficacia del sistema.",
            ["Registros de inducción y reinducción","Charlas de seguridad (ATS / pre-tarea)","Encuestas de toma de conciencia"]),

        new("7","7. APOYO",
            "7.3 Toma de conciencia","7.3.2",
            "Son conscientes de las consecuencias del incumplimiento y de su capacidad de retirarse ante peligro grave.",
            "Saben qué pasa si no cumplen y conocen su derecho a alejarse de situaciones de peligro inminente sin represalias.",
            ["Procedimiento de derecho a rehusar trabajo inseguro","Registros de divulgación","Política de parada segura (Stop Work)"]),

        new("7","7. APOYO",
            "7.4 Comunicación","7.4.1",
            "Se han definido las comunicaciones internas de SST (qué, cuándo, a quién y cómo comunicar).",
            "Matriz de comunicaciones que define flujos internos en todos los niveles, considerando diversidad de los trabajadores.",
            ["Matriz / procedimiento de comunicaciones","Carteleras y canales internos","Registros de divulgación interna"]),

        new("7","7. APOYO",
            "7.4 Comunicación","7.4.2",
            "Se gestionan las comunicaciones externas pertinentes según requisitos legales y partes interesadas.",
            "Comunicación con autoridades, ARL, contratistas, clientes y comunidad según obligaciones legales y contractuales.",
            ["Registros de comunicación externa","Reportes a ARL / autoridades","Procedimiento de comunicación externa"]),

        new("7","7. APOYO",
            "7.5 Información documentada","7.5.1",
            "La información documentada requerida por la norma y la organización está creada y actualizada.",
            "Existe la documentación exigida por la norma y la necesaria para la eficacia del sistema, con identificación, formato y revisión adecuados.",
            ["Listado maestro de documentos","Procedimiento de control de información documentada","Documentos vigentes codificados"]),

        new("7","7. APOYO",
            "7.5 Información documentada","7.5.2",
            "Se controla la creación, actualización, distribución, acceso y conservación de la información documentada.",
            "Control de versiones, accesos, protección, conservación y disposición de documentos y registros.",
            ["Control de cambios / versiones","Matriz de retención documental","Control de accesos a la información"]),

        // ══ 8. OPERACIÓN ════════════════════════════════════════════════════════
        new("8","8. OPERACIÓN",
            "8.1.2 Eliminar peligros y reducir riesgos","8.1.2.1",
            "Se aplica la jerarquía de controles para eliminar peligros y reducir riesgos de SST.",
            "Implementación efectiva en campo de la jerarquía: eliminación, sustitución, ingeniería, administrativos y EPP.",
            ["Registros de implementación de controles","Permisos de trabajo de alto riesgo","Inspecciones de controles operacionales"]),

        new("8","8. OPERACIÓN",
            "8.1.2 Eliminar peligros y reducir riesgos","8.1.2.2",
            "Se conservan registros de la implementación y verificación de los controles operacionales.",
            "Debe evidenciarse que los controles están implementados y se verifica su cumplimiento en obra.",
            ["Listas de chequeo de controles","Reportes de inspección","Permisos de trabajo diligenciados"]),

        new("8","8. OPERACIÓN",
            "8.1.3 Gestión del cambio","8.1.3.1",
            "Existe un proceso para implementar y controlar los cambios planificados que impactan la SST.",
            "Cambios en procesos, equipos, instalaciones, productos o personal se gestionan evaluando sus riesgos antes de ejecutarse (MOC).",
            ["Procedimiento de gestión del cambio (MOC)","Formato de análisis de cambio","Evaluación de riesgos del cambio"]),

        new("8","8. OPERACIÓN",
            "8.1.3 Gestión del cambio","8.1.3.2",
            "Se revisan las consecuencias de los cambios no previstos y se toman acciones para mitigar efectos adversos.",
            "Ante cambios no planificados, se evalúan efectos y se actúa para controlar nuevos riesgos generados.",
            ["Registros de revisión de cambios no previstos","Acciones derivadas","Actualización de matriz IPEVR"]),

        new("8","8. OPERACIÓN",
            "8.1.4 Compras, contratistas y contratación externa","8.1.4.1",
            "Se controla la compra de bienes y servicios para asegurar conformidad con los requisitos de SST.",
            "Criterios de SST en la selección de proveedores y verificación de bienes/servicios que afectan la seguridad.",
            ["Criterios SST de compras","Evaluación de proveedores","Registros de control de bienes/servicios adquiridos"]),

        new("8","8. OPERACIÓN",
            "8.1.4 Compras, contratistas y contratación externa","8.1.4.2",
            "Se coordinan y controlan los requisitos de SST con contratistas y procesos externalizados.",
            "Contratistas cumplen requisitos de SST; se coordina la actividad y se verifica su desempeño en obra.",
            ["Procedimiento de gestión de contratistas","Evaluación HSE de contratistas","Documentación de afiliación y EPP de contratistas"]),

        new("8","8. OPERACIÓN",
            "8.2 Preparación y respuesta ante emergencias","8.2.1",
            "Se ha establecido un plan de preparación y respuesta ante emergencias acorde a los peligros identificados.",
            "Plan de emergencias con análisis de vulnerabilidad, brigadas, rutas, recursos y procedimientos de respuesta.",
            ["Plan de emergencias","Análisis de vulnerabilidad","Conformación de brigadas"]),

        new("8","8. OPERACIÓN",
            "8.2 Preparación y respuesta ante emergencias","8.2.2",
            "Se realizan simulacros, se evalúa la respuesta y se comunica a las partes interesadas pertinentes.",
            "Pruebas periódicas (simulacros), evaluación de resultados y comunicación a trabajadores, contratistas y visitantes.",
            ["Registros de simulacros","Informe de evaluación post-simulacro","Evidencia de divulgación del plan"]),

        // ══ 9. EVALUACIÓN DEL DESEMPEÑO ═════════════════════════════════════════
        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.1 Seguimiento, medición, análisis y evaluación","9.1.1",
            "Se determina qué medir, los métodos, los criterios y cuándo realizar seguimiento del desempeño de SST.",
            "Indicadores de estructura, proceso y resultado, con métodos y frecuencia definidos; equipos de medición calibrados cuando aplique.",
            ["Ficha técnica de indicadores","Programa de seguimiento y medición","Certificados de calibración de equipos"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.1 Seguimiento, medición, análisis y evaluación","9.1.2",
            "Se evalúa periódicamente el cumplimiento de los requisitos legales y otros requisitos.",
            "Evaluación documentada del cumplimiento legal, con frecuencia definida y acciones ante incumplimientos.",
            ["Evaluación de cumplimiento legal","Matriz legal con estado de cumplimiento","Planes de cierre de brechas legales"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.2 Auditoría interna","9.2.1",
            "Existe un programa de auditorías internas planificado a intervalos definidos.",
            "Programa anual de auditorías con alcance, criterios, frecuencia y auditores competentes e imparciales.",
            ["Programa anual de auditorías","Plan de auditoría","Hojas de vida / competencia de auditores"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.2 Auditoría interna","9.2.2",
            "Los resultados de auditoría se informan a la dirección y se toman las correcciones necesarias.",
            "Hallazgos documentados, reportados a los responsables y a la dirección, con acciones correctivas asociadas.",
            ["Informe de auditoría interna","Registro de no conformidades","Plan de acción derivado de auditoría"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.3 Revisión por la dirección","9.3.1",
            "La alta dirección revisa el SG-SST a intervalos planificados con las entradas requeridas por la norma.",
            "Revisión que considera: estado de acciones previas, cambios, desempeño (incidentes, indicadores, cumplimiento legal), participación, recursos y oportunidades de mejora.",
            ["Acta de revisión por la dirección","Informe de entradas de la revisión","Tablero de indicadores presentado"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.3 Revisión por la dirección","9.3.2",
            "Las salidas de la revisión incluyen decisiones sobre mejora, cambios y recursos necesarios.",
            "De la revisión deben surgir decisiones documentadas: mejoras, necesidades de cambio y asignación de recursos.",
            ["Acta con decisiones y compromisos","Plan de acción post-revisión","Asignación de recursos aprobada"]),

        // ══ 10. MEJORA ══════════════════════════════════════════════════════════
        new("10","10. MEJORA",
            "10.1 Generalidades","10.1.1",
            "Se determinan oportunidades de mejora y se implementan acciones para lograr los resultados previstos del SG-SST.",
            "La organización identifica y aprovecha oportunidades de mejora del desempeño en SST de forma sistemática.",
            ["Registro de oportunidades de mejora","Plan de mejoramiento","Indicadores de desempeño"]),

        new("10","10. MEJORA",
            "10.1 Generalidades","10.1.2",
            "Las acciones de mejora consideran resultados del análisis, evaluación y participación de los trabajadores.",
            "Las mejoras se basan en datos (indicadores, auditorías, incidentes) y en la participación de los trabajadores.",
            ["Análisis de datos de SST","Actas de participación en mejora","Seguimiento de acciones de mejora"]),

        new("10","10. MEJORA",
            "10.2 Incidentes, no conformidades y acciones correctivas","10.2.1",
            "Existe un proceso para reportar, investigar y tomar acciones ante incidentes y no conformidades.",
            "Procedimiento de reporte e investigación de incidentes (con metodología de causa raíz) y tratamiento de no conformidades.",
            ["Procedimiento de investigación de incidentes","Reportes e investigaciones (causa raíz)","Registro de no conformidades"]),

        new("10","10. MEJORA",
            "10.2 Incidentes, no conformidades y acciones correctivas","10.2.2",
            "Se implementan acciones correctivas, se evalúa su eficacia y se conservan los registros.",
            "Las acciones correctivas eliminan la causa raíz, se verifica su eficacia y se documentan los resultados.",
            ["Plan de acciones correctivas","Verificación de eficacia","Registros de cierre"]),

        new("10","10. MEJORA",
            "10.3 Mejora continua","10.3.1",
            "Se mejora continuamente la conveniencia, adecuación y eficacia del SG-SST.",
            "El sistema evoluciona: mejora su capacidad de proteger a los trabajadores y de cumplir resultados a lo largo del tiempo.",
            ["Tendencias de indicadores","Histórico de mejoras implementadas","Revisión por la dirección (conclusiones)"]),

        new("10","10. MEJORA",
            "10.3 Mejora continua","10.3.2",
            "Se promueve la participación de los trabajadores en la mejora continua y se comunican los resultados.",
            "Los trabajadores aportan a la mejora continua y se les comunican los resultados y avances pertinentes.",
            ["Registros de participación en mejora","Comunicación de resultados a trabajadores","Buzón / canales de sugerencias"]),
    ];

    private static readonly Dictionary<string, RequisitoISO> _porId =
        Requisitos.ToDictionary(r => r.Id);

    public static RequisitoISO? GetById(string id) =>
        _porId.TryGetValue(id, out var r) ? r : null;

    public static RequisitoISO? GetByIdFuzzy(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;

        if (_porId.TryGetValue(id, out var exact)) return exact;

        var lastDot = id.LastIndexOf('.');
        if (lastDot > 0)
        {
            var parent = id[..lastDot];
            if (_porId.TryGetValue(parent, out var parentR)) return parentR;

            var grandLastDot = parent.LastIndexOf('.');
            if (grandLastDot > 0)
            {
                var grandParent = parent[..grandLastDot];
                if (_porId.TryGetValue(grandParent, out var gpR)) return gpR;
            }
        }

        var byPrefix = _porId.Values.FirstOrDefault(r => r.Id.StartsWith(id + ".", StringComparison.Ordinal));
        if (byPrefix is not null) return byPrefix;

        var clausula = id.Contains('.') ? id[..id.IndexOf('.')] : id;
        return _porId.Values.FirstOrDefault(r => r.NumClausula == clausula);
    }
}
