namespace RenergeIA.Web.Services;

public static class Iso14001ChecklistData
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
            "Se han determinado las cuestiones internas y externas pertinentes a su propósito que afectan los resultados previstos del Sistema de Gestión Ambiental.",
            "Identificar factores internos (procesos, recursos, cultura, condiciones ambientales de las instalaciones) y externos (legales, tecnológicos, climáticos, sociales, de mercado) que pueden afectar al SGA, incluidas las condiciones ambientales que la organización afecta o por las que es afectada.",
            ["Análisis de contexto ambiental","Matriz DOFA / PESTEL ambiental","Planeación estratégica","Acta de revisión por la dirección"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.1 Comprensión de la organización y su contexto","4.1.2",
            "Se realiza seguimiento y revisión de la información sobre estas cuestiones internas y externas.",
            "El contexto debe revisarse periódicamente (al menos en la revisión por la dirección) y actualizarse ante cambios en la operación, la normativa ambiental o las condiciones del entorno.",
            ["Acta de actualización del contexto","Registro de revisión por la dirección","Control de cambios del análisis de contexto"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.2 Necesidades y expectativas de las partes interesadas","4.2.1",
            "Se han identificado las partes interesadas pertinentes al SGA y sus necesidades y expectativas ambientales.",
            "Incluye autoridades ambientales, comunidad, clientes, contratistas, vecinos y empleados. Se documenta en una matriz con sus expectativas ambientales pertinentes.",
            ["Matriz de partes interesadas","Registro de expectativas ambientales","Mapa de grupos de interés del proyecto"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.2 Necesidades y expectativas de las partes interesadas","4.2.2",
            "Se determina cuáles de esas necesidades y expectativas se convierten en requisitos legales y otros requisitos.",
            "De las expectativas se define cuáles asume la organización como obligación de cumplimiento (legal o voluntaria, p. ej. compromisos con clientes o licencias ambientales).",
            ["Matriz de partes interesadas con requisitos","Matriz de requisitos legales y otros","Obligaciones de licencias / permisos ambientales"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.3 Determinación del alcance del SGA","4.3.1",
            "El alcance del SGA está definido considerando cuestiones, partes interesadas, unidades, funciones y límites físicos.",
            "El alcance debe abarcar las actividades, productos y servicios bajo control e influencia de la organización con perspectiva de ciclo de vida, incluyendo sedes y proyectos.",
            ["Documento de alcance del SGA","Manual del SGA","Política ambiental"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.3 Determinación del alcance del SGA","4.3.2",
            "El alcance se mantiene como información documentada y está disponible para las partes interesadas.",
            "El alcance debe estar escrito, controlado y accesible; no puede excluir actividades con aspectos ambientales significativos para aparentar mejor desempeño.",
            ["Información documentada de alcance (controlada)","Listado maestro de documentos","Evidencia de disponibilidad del alcance"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.4 Sistema de Gestión Ambiental","4.4.1",
            "Se ha establecido, implementado, mantenido y se mejora continuamente el SGA conforme a la norma.",
            "Existe un sistema ambiental real y operando, con ciclo PHVA evidenciable y recursos asignados, para mejorar el desempeño ambiental.",
            ["Manual / estructura del SGA","Plan de trabajo anual ambiental (PHVA)","Indicadores ambientales"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.4 Sistema de Gestión Ambiental","4.4.2",
            "Se han determinado los procesos necesarios y sus interacciones dentro del SGA.",
            "Se identifican los procesos del sistema y cómo se relacionan (mapa de procesos), incluyendo entradas, salidas y responsables ambientales.",
            ["Mapa de procesos","Caracterización de procesos","Diagrama de interacción de procesos"]),

        // ══ 5. LIDERAZGO ════════════════════════════════════════════════════════
        new("5","5. LIDERAZGO",
            "5.1 Liderazgo y compromiso","5.1.1",
            "La alta dirección demuestra liderazgo y compromiso, asumiendo la responsabilidad por la eficacia del SGA.",
            "La dirección rinde cuentas del SGA, asegura que la política y objetivos sean compatibles con la dirección estratégica y garantiza los recursos necesarios.",
            ["Acta de revisión por la dirección","Evidencia de participación de gerencia en temas ambientales","Asignación de recursos aprobada"]),

        new("5","5. LIDERAZGO",
            "5.1 Liderazgo y compromiso","5.1.2",
            "La dirección asegura la integración del SGA en los procesos de negocio y promueve la mejora continua.",
            "La gestión ambiental no funciona aislada: se integra a la operación, presupuesto y toma de decisiones, y la dirección dirige y apoya a las personas.",
            ["Presupuesto ambiental aprobado","Procedimientos que integran lo ambiental a la operación","Acta de asignación de recursos"]),

        new("5","5. LIDERAZGO",
            "5.2 Política ambiental","5.2.1",
            "La política ambiental incluye compromisos de protección del ambiente, prevención de la contaminación y cumplimiento de requisitos legales.",
            "Debe incluir: protección del medio ambiente (incl. prevención de la contaminación), cumplimiento de los requisitos legales y otros, y mejora continua del SGA. Debe ser apropiada al propósito y contexto.",
            ["Política ambiental firmada y vigente","Registro de revisión de la política","Acta de aprobación de la dirección"]),

        new("5","5. LIDERAZGO",
            "5.2 Política ambiental","5.2.2",
            "La política está documentada, comunicada dentro de la organización y disponible para las partes interesadas.",
            "Debe estar publicada, divulgada a quienes trabajan bajo el control de la organización (incl. contratistas) y disponible al público pertinente.",
            ["Evidencia de divulgación (firmas/registros)","Política publicada en sitios visibles","Evidencia de disponibilidad externa"]),

        new("5","5. LIDERAZGO",
            "5.3 Roles, responsabilidades y autoridades","5.3.1",
            "Las responsabilidades y autoridades para los roles pertinentes del SGA están asignadas y comunicadas.",
            "Cada nivel tiene responsabilidades ambientales definidas en manuales de funciones o matrices, incluyendo quien gestiona el SGA.",
            ["Manual de funciones con responsabilidades ambientales","Matriz de roles y responsabilidades","Perfiles de cargo"]),

        new("5","5. LIDERAZGO",
            "5.3 Roles, responsabilidades y autoridades","5.3.2",
            "Se ha asignado la responsabilidad de informar el desempeño del SGA a la alta dirección.",
            "Existe un responsable designado (ej. coordinador HSEQ) que rinde cuentas del desempeño ambiental a la dirección.",
            ["Designación del responsable del SGA","Informes de desempeño ambiental a la dirección","Acta de nombramiento"]),

        // ══ 6. PLANIFICACIÓN ════════════════════════════════════════════════════
        new("6","6. PLANIFICACIÓN",
            "6.1.1 Generalidades — riesgos y oportunidades","6.1.1.1",
            "Se determinan los riesgos y oportunidades relacionados con aspectos ambientales, requisitos legales y otras cuestiones.",
            "Al planificar, se consideran cuestiones (4.1), partes interesadas (4.2) y el alcance para determinar riesgos y oportunidades que aseguren los resultados previstos y prevengan efectos no deseados.",
            ["Matriz de riesgos y oportunidades ambientales","Análisis de contexto","Procedimiento de planificación del SGA"]),

        new("6","6. PLANIFICACIÓN",
            "6.1.1 Generalidades — riesgos y oportunidades","6.1.1.2",
            "Se determinan las situaciones de emergencia potenciales, incluidas las que pueden tener impacto ambiental.",
            "Identificar escenarios de emergencia (derrames, incendios, fugas) que puedan generar impactos ambientales, como insumo para 8.2.",
            ["Matriz de escenarios de emergencia ambiental","Análisis de vulnerabilidad","Registro de situaciones potenciales"]),

        new("6","6. PLANIFICACIÓN",
            "6.1.2 Aspectos ambientales","6.1.2.1",
            "Se han determinado los aspectos ambientales y sus impactos asociados considerando una perspectiva de ciclo de vida.",
            "Identificar aspectos (consumo de agua/energía, residuos, vertimientos, emisiones, ruido) de actividades, productos y servicios que se pueden controlar e influir, en condiciones normales, anormales y de emergencia.",
            ["Matriz de aspectos e impactos ambientales","Procedimiento de identificación de aspectos","Análisis de ciclo de vida"]),

        new("6","6. PLANIFICACIÓN",
            "6.1.2 Aspectos ambientales","6.1.2.2",
            "Se determinan los aspectos ambientales significativos mediante criterios establecidos y se mantienen documentados.",
            "Aplicar criterios definidos (severidad, frecuencia, requisitos legales, partes interesadas) para priorizar los aspectos significativos, que orientan objetivos y controles.",
            ["Criterios de significancia documentados","Matriz con aspectos significativos","Registro de aspectos comunicados a la organización"]),

        new("6","6. PLANIFICACIÓN",
            "6.1.3 Requisitos legales y otros requisitos","6.1.3.1",
            "Se identifican, tienen acceso y se mantienen actualizados los requisitos legales ambientales y otros requisitos aplicables.",
            "Matriz legal ambiental actualizada (normativa de agua, aire, residuos, ruido, licencias y permisos) con seguimiento a cambios normativos.",
            ["Matriz de requisitos legales ambientales (normograma)","Licencias y permisos ambientales","Procedimiento de actualización legal"]),

        new("6","6. PLANIFICACIÓN",
            "6.1.3 Requisitos legales y otros requisitos","6.1.3.2",
            "Se determina cómo aplican estos requisitos a la organización y cómo se tienen en cuenta en el SGA.",
            "No basta listar normas: debe definirse cómo aplica cada una a la operación y reflejarse en controles y objetivos del SGA.",
            ["Matriz legal con análisis de aplicabilidad","Evidencia de comunicación a responsables","Evaluación de cumplimiento legal"]),

        new("6","6. PLANIFICACIÓN",
            "6.1.4 Planificación de acciones","6.1.4.1",
            "Se planifican acciones para abordar aspectos significativos, requisitos legales y riesgos y oportunidades.",
            "Para cada aspecto significativo, requisito o riesgo se definen acciones concretas integradas al SGA y a los procesos.",
            ["Plan de acción ambiental","Plan de trabajo anual","Cronograma de actividades"]),

        new("6","6. PLANIFICACIÓN",
            "6.1.4 Planificación de acciones","6.1.4.2",
            "Las acciones se integran a los procesos del SGA y se evalúa la eficacia de dichas acciones.",
            "Las acciones no son aisladas: se implementan en los procesos y se mide si lograron el resultado ambiental esperado.",
            ["Seguimiento de acciones","Indicadores de eficacia","Registros de cierre de acciones"]),

        new("6","6. PLANIFICACIÓN",
            "6.2.1 Objetivos ambientales","6.2.1.1",
            "Se han establecido objetivos ambientales coherentes con la política, medibles y para funciones y niveles pertinentes.",
            "Objetivos SMART, considerando aspectos significativos, requisitos legales, riesgos y oportunidades; comunicados y actualizados.",
            ["Documento de objetivos y metas ambientales","Tablero de indicadores ambientales","Política ambiental (coherencia)"]),

        new("6","6. PLANIFICACIÓN",
            "6.2.1 Objetivos ambientales","6.2.1.2",
            "Los objetivos consideran los aspectos significativos y se realiza seguimiento, comunicación y actualización.",
            "Los objetivos deben derivarse de los aspectos significativos y requisitos, con seguimiento medible y actualización cuando proceda.",
            ["Ficha técnica de indicadores ambientales","Registros de seguimiento de objetivos","Evidencia de comunicación de objetivos"]),

        new("6","6. PLANIFICACIÓN",
            "6.2.2 Planificación de acciones para lograr los objetivos","6.2.2.1",
            "Se planifica qué se hará, los recursos, el responsable y los plazos para lograr los objetivos ambientales.",
            "Cada objetivo tiene un plan: qué, recursos necesarios, responsable y fecha límite.",
            ["Plan de logro de objetivos","Cronograma con responsables","Asignación de recursos"]),

        new("6","6. PLANIFICACIÓN",
            "6.2.2 Planificación de acciones para lograr los objetivos","6.2.2.2",
            "Se define cómo se evaluarán los resultados, incluidos los indicadores para el seguimiento del avance.",
            "Se determinan indicadores de seguimiento para verificar el progreso hacia el cumplimiento de cada objetivo.",
            ["Indicadores de avance de objetivos","Registros de evaluación de resultados","Tablero de seguimiento"]),

        // ══ 7. APOYO ════════════════════════════════════════════════════════════
        new("7","7. APOYO",
            "7.1 Recursos","7.1.1",
            "Se han determinado y proporcionado los recursos necesarios para el establecimiento y mejora del SGA.",
            "Incluye recursos humanos, financieros, tecnológicos e infraestructura para implementar y mantener el sistema ambiental.",
            ["Presupuesto ambiental","Inventario de recursos asignados","Acta de asignación de recursos"]),

        new("7","7. APOYO",
            "7.1 Recursos","7.1.2",
            "Los recursos se mantienen disponibles para la operación y mejora continua del SGA.",
            "No es un gasto puntual: debe garantizarse continuidad de recursos para la gestión ambiental en el tiempo.",
            ["Ejecución presupuestal ambiental","Plan plurianual de recursos","Evidencia de continuidad de contratos clave"]),

        new("7","7. APOYO",
            "7.2 Competencia","7.2.1",
            "Se determina la competencia necesaria de las personas cuyo trabajo afecta el desempeño ambiental y el cumplimiento legal.",
            "Definir educación, formación y experiencia requeridas por cargo, especialmente para tareas con aspectos ambientales significativos.",
            ["Perfiles de cargo con competencias","Matriz de competencias","Requisitos de competencia ambiental"]),

        new("7","7. APOYO",
            "7.2 Competencia","7.2.2",
            "Se toman acciones para adquirir la competencia necesaria y se conservan registros como evidencia.",
            "Formación, capacitación o tutoría, con evaluación de eficacia y registros conservados.",
            ["Plan de capacitación ambiental","Certificados y registros de asistencia","Evaluaciones de eficacia de la formación"]),

        new("7","7. APOYO",
            "7.3 Toma de conciencia","7.3.1",
            "Las personas que trabajan bajo el control de la organización son conscientes de la política y de los aspectos e impactos significativos.",
            "Conocen la política ambiental, los aspectos significativos e impactos asociados a su trabajo y su contribución a la eficacia del SGA.",
            ["Registros de inducción y reinducción ambiental","Charlas de sensibilización ambiental","Encuestas de toma de conciencia"]),

        new("7","7. APOYO",
            "7.3 Toma de conciencia","7.3.2",
            "Son conscientes de las implicaciones de no cumplir los requisitos del SGA, incluidos los requisitos legales.",
            "Saben qué consecuencias (ambientales y legales) tiene el incumplimiento y cómo su conducta contribuye al beneficio ambiental.",
            ["Material de sensibilización","Registros de divulgación","Evidencia de campañas ambientales"]),

        new("7","7. APOYO",
            "7.4 Comunicación","7.4.1",
            "Se han establecido las comunicaciones internas pertinentes al SGA (qué, cuándo, a quién y cómo comunicar).",
            "Matriz de comunicaciones que define flujos internos en todos los niveles y permite que las personas contribuyan a la mejora continua.",
            ["Matriz / procedimiento de comunicaciones","Carteleras y canales internos","Registros de divulgación interna"]),

        new("7","7. APOYO",
            "7.4 Comunicación","7.4.2",
            "Se gestionan las comunicaciones externas según los requisitos legales y el procedimiento establecido.",
            "Comunicación con autoridades ambientales, comunidad, clientes y contratistas conforme a obligaciones legales y compromisos asumidos.",
            ["Registros de comunicación externa","Reportes a autoridades ambientales","Procedimiento de comunicación externa"]),

        new("7","7. APOYO",
            "7.5 Información documentada","7.5.1",
            "La información documentada requerida por la norma y por la organización para el SGA está creada y actualizada.",
            "Existe la documentación exigida por la norma y la necesaria para la eficacia del SGA, con identificación, formato y revisión adecuados.",
            ["Listado maestro de documentos","Procedimiento de control de información documentada","Documentos ambientales vigentes codificados"]),

        new("7","7. APOYO",
            "7.5 Información documentada","7.5.2",
            "Se controla la creación, actualización, distribución, acceso, conservación y disposición de la información documentada.",
            "Control de versiones, accesos, protección, conservación y disposición de documentos y registros ambientales.",
            ["Control de cambios / versiones","Matriz de retención documental","Control de accesos a la información"]),

        // ══ 8. OPERACIÓN ════════════════════════════════════════════════════════
        new("8","8. OPERACIÓN",
            "8.1 Planificación y control operacional","8.1.1",
            "Se establecen, implementan y controlan los procesos necesarios para cumplir los requisitos del SGA y las acciones de la cláusula 6.",
            "Controles operacionales (criterios de operación, procedimientos, controles de ingeniería) sobre los aspectos significativos, incluida la gestión de cambios y procesos contratados externamente.",
            ["Procedimientos de control operacional","Instructivos de gestión de residuos / vertimientos / emisiones","Control de procesos contratados externamente"]),

        new("8","8. OPERACIÓN",
            "8.1 Planificación y control operacional","8.1.2",
            "Se aplican controles con perspectiva de ciclo de vida sobre el diseño, las compras y los contratistas.",
            "Considerar requisitos ambientales en diseño, adquisición de bienes/servicios, control sobre contratistas y comunicación de requisitos a proveedores, así como tratamiento al final de la vida útil.",
            ["Criterios ambientales de compras","Requisitos ambientales a contratistas","Procedimiento con perspectiva de ciclo de vida"]),

        new("8","8. OPERACIÓN",
            "8.2 Preparación y respuesta ante emergencias","8.2.1",
            "Se han establecido procesos para prepararse y responder ante situaciones potenciales de emergencia ambiental.",
            "Plan que prevenga o mitigue impactos ambientales adversos de emergencias (derrames, incendios, fugas), con recursos y respuesta planificada.",
            ["Plan de emergencias ambientales","Matriz de escenarios","Plan de contingencia para derrames"]),

        new("8","8. OPERACIÓN",
            "8.2 Preparación y respuesta ante emergencias","8.2.2",
            "Se prueban periódicamente las acciones de respuesta planificadas y se revisan tras los eventos o pruebas.",
            "Realizar simulacros, evaluar resultados, revisar y mejorar el plan, y proporcionar información y formación pertinente a las partes interesadas.",
            ["Registros de simulacros ambientales","Informe de evaluación post-evento","Evidencia de divulgación y formación"]),

        // ══ 9. EVALUACIÓN DEL DESEMPEÑO ═════════════════════════════════════════
        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.1.1 Seguimiento, medición, análisis y evaluación","9.1.1.1",
            "Se determina qué necesita seguimiento y medición, los métodos, los criterios y cuándo realizarlo.",
            "Indicadores de desempeño ambiental con métodos válidos y frecuencia definida; equipos de seguimiento calibrados o verificados cuando aplique.",
            ["Ficha técnica de indicadores ambientales","Programa de seguimiento y medición","Certificados de calibración de equipos"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.1.1 Seguimiento, medición, análisis y evaluación","9.1.1.2",
            "Se evalúa el desempeño ambiental y la eficacia del SGA, y se comunica la información pertinente.",
            "Analizar y evaluar los datos para determinar el desempeño ambiental y la eficacia del sistema, y comunicar resultados interna y externamente según corresponda.",
            ["Análisis de datos ambientales","Informe de desempeño ambiental","Evidencia de comunicación de resultados"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.1.2 Evaluación del cumplimiento","9.1.2.1",
            "Se evalúa periódicamente el cumplimiento de los requisitos legales y otros requisitos ambientales.",
            "Evaluación documentada del cumplimiento legal ambiental con frecuencia definida y métodos establecidos.",
            ["Evaluación de cumplimiento legal ambiental","Matriz legal con estado de cumplimiento","Registros de la evaluación"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.1.2 Evaluación del cumplimiento","9.1.2.2",
            "Se toman acciones ante incumplimientos y se mantiene el conocimiento del estado de cumplimiento.",
            "Ante brechas se actúa; la organización mantiene conocimiento y comprensión de su estado de cumplimiento legal.",
            ["Planes de cierre de brechas legales","Registros de acciones tomadas","Conclusiones de la evaluación de cumplimiento"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.2 Auditoría interna","9.2.1",
            "Existe un programa de auditorías internas planificado a intervalos definidos para el SGA.",
            "Programa que considera la importancia ambiental de los procesos y resultados de auditorías previas, con alcance, criterios, frecuencia y auditores competentes e imparciales.",
            ["Programa anual de auditorías","Plan de auditoría","Competencia de auditores"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.2 Auditoría interna","9.2.2",
            "Los resultados de auditoría se informan a la dirección y se conservan como información documentada.",
            "Hallazgos documentados, reportados a los responsables y a la dirección, con acciones correctivas asociadas.",
            ["Informe de auditoría interna","Registro de no conformidades","Plan de acción derivado de auditoría"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.3 Revisión por la dirección","9.3.1",
            "La alta dirección revisa el SGA a intervalos planificados con las entradas requeridas por la norma.",
            "Revisión que considera: acciones previas, cambios en cuestiones y partes interesadas, desempeño ambiental (objetivos, aspectos, cumplimiento legal, no conformidades), recursos y oportunidades de mejora.",
            ["Acta de revisión por la dirección","Informe de entradas de la revisión","Tablero de indicadores ambientales"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.3 Revisión por la dirección","9.3.2",
            "Las salidas de la revisión incluyen decisiones sobre mejora, cambios al SGA y necesidad de recursos.",
            "De la revisión surgen decisiones documentadas: conclusiones sobre conveniencia/adecuación/eficacia, mejoras, cambios y recursos.",
            ["Acta con decisiones y compromisos","Plan de acción post-revisión","Asignación de recursos aprobada"]),

        // ══ 10. MEJORA ══════════════════════════════════════════════════════════
        new("10","10. MEJORA",
            "10.1 Generalidades","10.1.1",
            "Se determinan las oportunidades de mejora y se implementan acciones para lograr los resultados previstos del SGA.",
            "La organización identifica y aprovecha oportunidades de mejora del desempeño ambiental de forma sistemática.",
            ["Registro de oportunidades de mejora","Plan de mejoramiento ambiental","Indicadores de desempeño"]),

        new("10","10. MEJORA",
            "10.1 Generalidades","10.1.2",
            "Las mejoras consideran los resultados del análisis, la evaluación del desempeño y la evaluación del cumplimiento.",
            "Las mejoras se basan en datos (indicadores, auditorías, cumplimiento legal, no conformidades) para mejorar el desempeño ambiental.",
            ["Análisis de datos ambientales","Seguimiento de acciones de mejora","Informe de desempeño"]),

        new("10","10. MEJORA",
            "10.2 No conformidad y acción correctiva","10.2.1",
            "Ante una no conformidad se reacciona, se controla, se corrige y se hace frente a las consecuencias ambientales.",
            "Procedimiento para identificar no conformidades (incluidos incidentes ambientales), tomar acción inmediata y mitigar impactos ambientales.",
            ["Procedimiento de no conformidades","Registro de no conformidades e incidentes ambientales","Acciones inmediatas documentadas"]),

        new("10","10. MEJORA",
            "10.2 No conformidad y acción correctiva","10.2.2",
            "Se evalúa la necesidad de acción correctiva para eliminar las causas, se implementa y se verifica su eficacia.",
            "Análisis de causa raíz, implementación de acciones correctivas, verificación de eficacia y registro de resultados; actualización del SGA si procede.",
            ["Análisis de causa raíz","Plan de acciones correctivas","Verificación de eficacia y registros de cierre"]),

        new("10","10. MEJORA",
            "10.3 Mejora continua","10.3.1",
            "Se mejora continuamente la conveniencia, adecuación y eficacia del SGA para mejorar el desempeño ambiental.",
            "El sistema evoluciona: mejora su capacidad de proteger el ambiente y de cumplir resultados a lo largo del tiempo.",
            ["Tendencias de indicadores ambientales","Histórico de mejoras implementadas","Conclusiones de la revisión por la dirección"]),

        new("10","10. MEJORA",
            "10.3 Mejora continua","10.3.2",
            "Las acciones de mejora continua están alineadas con los resultados de la revisión por la dirección.",
            "La mejora continua se nutre de las decisiones de la dirección y de los análisis de desempeño, cerrando el ciclo PHVA.",
            ["Plan de mejora continua","Seguimiento de decisiones de la dirección","Registros de avances de mejora"]),
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
