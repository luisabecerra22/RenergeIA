namespace RenergeIA.Web.Services;

public static class Iso9001ChecklistData
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
        // ══ 4. CONTEXTO DE LA ORGANIZACIÓN ══════════════════════════════════
        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.1 Comprensión de la organización y su contexto","4.1",
            "Se han determinado las cuestiones internas y externas pertinentes al propósito y dirección estratégica que afectan los resultados previstos del SGC, y se les hace seguimiento.",
            "Identificar factores internos (cultura, conocimiento, desempeño) y externos (legal, tecnológico, competitivo, de mercado) que influyen en la capacidad de lograr los resultados del SGC. Suele evidenciarse con DOFA/PESTEL y revisarse periódicamente.",
            ["Análisis de contexto","Matriz DOFA / PESTEL","Planeación estratégica","Acta de revisión por la dirección"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.2 Comprensión de las necesidades y expectativas de las partes interesadas","4.2",
            "Se han determinado las partes interesadas pertinentes al SGC y sus requisitos, con seguimiento y revisión de la información.",
            "Identificar partes interesadas (clientes, proveedores, entes reguladores, empleados) cuyos requisitos afectan la conformidad del producto/servicio y la satisfacción del cliente.",
            ["Matriz de partes interesadas","Registro de requisitos de partes interesadas","Mapa de grupos de interés"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.3 Determinación del alcance del SGC","4.3",
            "El alcance del SGC está determinado y documentado, considerando cuestiones, requisitos y productos/servicios, justificando cualquier requisito no aplicable.",
            "El alcance define límites y aplicabilidad del SGC. Toda exclusión (ej. 8.3 Diseño) debe justificarse y no afectar la capacidad de cumplir requisitos.",
            ["Documento de alcance del SGC","Manual de calidad / información documentada de alcance","Justificación de exclusiones"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.4 Sistema de Gestión de la Calidad y sus procesos","4.4.1",
            "Se han determinado los procesos del SGC, sus entradas, salidas, secuencia, interacción, criterios, recursos, responsabilidades, riesgos y oportunidades.",
            "Enfoque a procesos: definir cada proceso con sus elementos (PEPSU/caracterización), indicadores, recursos y responsables, y cómo interactúan entre sí.",
            ["Mapa de procesos","Caracterización de procesos (entradas/salidas)","Indicadores de proceso"]),

        new("4","4. CONTEXTO DE LA ORGANIZACIÓN",
            "4.4 Sistema de Gestión de la Calidad y sus procesos","4.4.2",
            "Se mantiene y conserva la información documentada necesaria para apoyar la operación de los procesos y confiar en que se realizan según lo planificado.",
            "Debe existir información documentada para operar (procedimientos) y registros que evidencien que los procesos se ejecutan como se planificó.",
            ["Procedimientos documentados","Registros de operación de procesos","Listado maestro de documentos"]),

        // ══ 5. LIDERAZGO ════════════════════════════════════════════════════
        new("5","5. LIDERAZGO",
            "5.1.1 Liderazgo y compromiso (generalidades)","5.1.1",
            "La alta dirección demuestra liderazgo y compromiso asumiendo la responsabilidad de la eficacia del SGC, integrándolo a los procesos de negocio y promoviendo el enfoque a procesos y el pensamiento basado en riesgos.",
            "La dirección rinde cuentas del SGC, asegura política y objetivos, disponibilidad de recursos, comunica la importancia de la calidad y apoya a las personas.",
            ["Acta de revisión por la dirección","Evidencia de participación de gerencia","Asignación de recursos aprobada"]),

        new("5","5. LIDERAZGO",
            "5.1.2 Enfoque al cliente","5.1.2",
            "La alta dirección asegura que se determinan y cumplen los requisitos del cliente y legales, se abordan los riesgos y oportunidades, y se mantiene el enfoque en aumentar la satisfacción del cliente.",
            "El cliente es el centro: deben entenderse sus requisitos, gestionarse los riesgos que afectan la conformidad y mantenerse el foco en su satisfacción.",
            ["Requisitos del cliente documentados","Análisis de riesgos y oportunidades","Indicadores de satisfacción del cliente"]),

        new("5","5. LIDERAZGO",
            "5.2.1 Establecimiento de la política de la calidad","5.2.1",
            "La alta dirección ha establecido una política de calidad apropiada al propósito y contexto, que incluye compromiso de cumplir requisitos y de mejora continua, y proporciona marco para los objetivos.",
            "La política debe ser coherente con la dirección estratégica, comprometerse con el cumplimiento de requisitos y la mejora continua, y servir de marco para fijar objetivos de calidad.",
            ["Política de calidad firmada y vigente","Acta de aprobación de la dirección","Registro de revisión de la política"]),

        new("5","5. LIDERAZGO",
            "5.2.2 Comunicación de la política de la calidad","5.2.2",
            "La política de calidad está disponible como información documentada, se comunica, se entiende y se aplica dentro de la organización, y está disponible para las partes interesadas pertinentes.",
            "No basta tenerla escrita: debe divulgarse, ser comprendida por el personal y estar disponible a las partes interesadas que lo requieran.",
            ["Evidencia de divulgación (firmas/registros)","Política publicada en sitios visibles","Evidencia de disponibilidad externa"]),

        new("5","5. LIDERAZGO",
            "5.3 Roles, responsabilidades y autoridades en la organización","5.3",
            "La alta dirección ha asignado y comunicado las responsabilidades y autoridades para los roles pertinentes, incluyendo asegurar conformidad del SGC, integridad ante cambios e informar el desempeño.",
            "Roles definidos para: asegurar que los procesos generan salidas previstas, informar desempeño y oportunidades de mejora, promover el enfoque al cliente y mantener la integridad del SGC.",
            ["Manual de funciones / responsabilidades","Matriz de roles y autoridades","Organigrama y designaciones"]),

        // ══ 6. PLANIFICACIÓN ════════════════════════════════════════════════
        new("6","6. PLANIFICACIÓN",
            "6.1 Acciones para abordar riesgos y oportunidades","6.1.1",
            "Al planificar el SGC se consideran las cuestiones (4.1) y requisitos (4.2) y se determinan los riesgos y oportunidades a abordar para asegurar resultados, prevenir efectos no deseados y lograr la mejora.",
            "Pensamiento basado en riesgos: identificar riesgos y oportunidades que pueden afectar la conformidad y la satisfacción del cliente.",
            ["Matriz de riesgos y oportunidades","Metodología de gestión del riesgo","Análisis de contexto"]),

        new("6","6. PLANIFICACIÓN",
            "6.1 Acciones para abordar riesgos y oportunidades","6.1.2",
            "Se planifican acciones para abordar los riesgos y oportunidades, su integración a los procesos del SGC y la evaluación de su eficacia, de forma proporcional al impacto en la conformidad.",
            "Las acciones se integran en los procesos y se evalúa si fueron eficaces. Las opciones incluyen evitar, asumir, eliminar la fuente, compartir o retener el riesgo.",
            ["Plan de acciones sobre riesgos","Seguimiento de eficacia","Registros de integración a procesos"]),

        new("6","6. PLANIFICACIÓN",
            "6.2 Objetivos de la calidad y planificación para lograrlos","6.2.1",
            "Se han establecido objetivos de calidad medibles, coherentes con la política, pertinentes a la conformidad y a la satisfacción del cliente, objeto de seguimiento, comunicados y actualizados.",
            "Objetivos SMART en funciones y niveles pertinentes, alineados a la política y mantenidos como información documentada.",
            ["Documento de objetivos de calidad","Tablero de indicadores","Política de calidad (coherencia)"]),

        new("6","6. PLANIFICACIÓN",
            "6.2 Objetivos de la calidad y planificación para lograrlos","6.2.2",
            "Se planifica cómo lograr los objetivos: qué se hará, recursos, responsable, plazo y cómo se evaluarán los resultados.",
            "Cada objetivo tiene un plan de logro con qué, recursos, responsable, fecha e indicador de evaluación.",
            ["Plan de logro de objetivos","Cronograma con responsables","Ficha técnica de indicadores"]),

        new("6","6. PLANIFICACIÓN",
            "6.3 Planificación de los cambios","6.3",
            "Cuando se determina la necesidad de cambios en el SGC, estos se llevan a cabo de manera planificada, considerando propósito, integridad del SGC, recursos y asignación de responsabilidades.",
            "Los cambios al sistema no se improvisan: se evalúan consecuencias potenciales, se preserva la integridad del SGC y se asignan recursos y responsables.",
            ["Procedimiento de gestión del cambio","Registros de planificación de cambios","Evaluación de impacto del cambio"]),

        // ══ 7. APOYO ════════════════════════════════════════════════════════
        new("7","7. APOYO",
            "7.1.1 Recursos (generalidades)","7.1.1",
            "Se han determinado y proporcionado los recursos necesarios para el establecimiento, implementación, mantenimiento y mejora continua del SGC.",
            "Considerar capacidades y limitaciones de recursos internos y qué se debe obtener de proveedores externos.",
            ["Presupuesto de calidad","Inventario de recursos","Acta de asignación de recursos"]),

        new("7","7. APOYO",
            "7.1.2 Personas","7.1.2",
            "Se han determinado y proporcionado las personas necesarias para la operación y control eficaz de los procesos del SGC.",
            "Asegurar el talento humano suficiente y adecuado para operar el sistema y sus procesos.",
            ["Planta de personal / cargos","Plan de contratación","Matriz de cargos por proceso"]),

        new("7","7. APOYO",
            "7.1.3 Infraestructura","7.1.3",
            "Se ha determinado, proporcionado y mantiene la infraestructura necesaria (edificios, equipos, hardware, software, transporte, TIC) para la conformidad.",
            "La infraestructura física y tecnológica debe garantizar la conformidad de productos y servicios y mantenerse en condiciones adecuadas.",
            ["Plan de mantenimiento de infraestructura","Inventario de equipos","Registros de mantenimiento"]),

        new("7","7. APOYO",
            "7.1.4 Ambiente para la operación de los procesos","7.1.4",
            "Se determina, proporciona y mantiene el ambiente necesario (factores sociales, psicológicos y físicos) para la operación de los procesos y la conformidad.",
            "Considerar factores humanos (no discriminación, calma) y físicos (temperatura, ruido, iluminación, higiene) apropiados a la actividad.",
            ["Evaluación de condiciones de trabajo","Registros de control ambiental de procesos","Encuestas de clima"]),

        new("7","7. APOYO",
            "7.1.5 Recursos de seguimiento y medición","7.1.5",
            "Se determinan y proporcionan recursos de seguimiento y medición adecuados, y cuando se requiere trazabilidad, los equipos se calibran/verifican, identifican y protegen.",
            "Los equipos de medición deben ser apropiados, estar calibrados o verificados contra patrones trazables, identificados y protegidos contra daños o desajustes.",
            ["Programa de calibración","Certificados de calibración trazables","Inventario de equipos de medición"]),

        new("7","7. APOYO",
            "7.1.6 Conocimientos de la organización","7.1.6",
            "Se han determinado los conocimientos necesarios para la operación de los procesos, se mantienen, se ponen a disposición y se gestionan ante necesidades y tendencias cambiantes.",
            "El conocimiento organizacional (lecciones aprendidas, experiencia, propiedad intelectual) debe conservarse y estar disponible; ante cambios, determinar cómo adquirir el conocimiento adicional.",
            ["Base de conocimiento / lecciones aprendidas","Registros de gestión del conocimiento","Repositorio documental"]),

        new("7","7. APOYO",
            "7.2 Competencia","7.2",
            "Se determina la competencia necesaria, se asegura mediante educación/formación/experiencia, se toman acciones para adquirirla y se evalúa su eficacia, conservando información documentada.",
            "Definir competencias por cargo, cerrar brechas con formación u otras acciones, evaluar la eficacia y conservar evidencia.",
            ["Matriz de competencias","Perfiles de cargo","Certificados / registros de formación y evaluación de eficacia"]),

        new("7","7. APOYO",
            "7.3 Toma de conciencia","7.3",
            "Las personas son conscientes de la política y objetivos de calidad, su contribución a la eficacia del SGC, los beneficios de la mejora del desempeño y las implicaciones de no cumplir los requisitos.",
            "El personal debe conocer la política, su aporte al sistema y las consecuencias del incumplimiento.",
            ["Registros de inducción/sensibilización","Campañas de calidad","Encuestas de toma de conciencia"]),

        new("7","7. APOYO",
            "7.4 Comunicación","7.4",
            "Se han determinado las comunicaciones internas y externas pertinentes al SGC: qué, cuándo, a quién, cómo comunicar y quién comunica.",
            "Matriz de comunicaciones que define flujos internos y externos relevantes para el SGC.",
            ["Matriz / procedimiento de comunicaciones","Canales internos","Registros de comunicación externa"]),

        new("7","7. APOYO",
            "7.5.1 Información documentada (generalidades)","7.5.1",
            "El SGC incluye la información documentada requerida por la norma y la determinada como necesaria para su eficacia.",
            "Existe la documentación exigida por la norma y la que la organización determina necesaria según su tamaño, procesos y competencia del personal.",
            ["Listado maestro de documentos","Manual / estructura documental","Documentos vigentes codificados"]),

        new("7","7. APOYO",
            "7.5.2 Creación y actualización","7.5.2",
            "Al crear y actualizar información documentada se asegura identificación, formato, soporte, revisión y aprobación apropiados.",
            "Cada documento debe tener identificación (título, fecha, autor, código), formato adecuado y haber sido revisado y aprobado.",
            ["Procedimiento de control de documentos","Formatos con codificación e identificación","Registros de revisión/aprobación"]),

        new("7","7. APOYO",
            "7.5.3 Control de la información documentada","7.5.3",
            "La información documentada se controla para asegurar su disponibilidad, idoneidad y protección, gestionando distribución, acceso, almacenamiento, control de cambios, conservación y disposición.",
            "Control de versiones, acceso, protección contra pérdida de integridad, conservación y disposición; control de documentos de origen externo.",
            ["Control de cambios / versiones","Matriz de retención documental","Control de documentos externos"]),

        // ══ 8. OPERACIÓN ════════════════════════════════════════════════════
        new("8","8. OPERACIÓN",
            "8.1 Planificación y control operacional","8.1",
            "Se planifican, implementan y controlan los procesos para cumplir requisitos y las acciones de la cláusula 6, mediante criterios, recursos, control conforme a criterios e información documentada.",
            "Determinar requisitos del producto/servicio, criterios de proceso y aceptación, recursos necesarios y conservar evidencia de la conformidad; controlar cambios planificados y no previstos y los procesos contratados externamente.",
            ["Planificación de la operación / plan de calidad","Criterios de aceptación","Control de procesos contratados externamente"]),

        new("8","8. OPERACIÓN",
            "8.2.1 Comunicación con el cliente","8.2.1",
            "Se gestiona la comunicación con el cliente sobre información del producto/servicio, consultas, contratos, pedidos y cambios, retroalimentación (incluidas quejas), propiedad del cliente y contingencias.",
            "Canales definidos para informar al cliente, manejar pedidos y cambios, recibir retroalimentación y quejas, y acordar acciones de contingencia cuando sea pertinente.",
            ["Procedimiento de comunicación con el cliente","Registros de PQR / quejas","Contratos y órdenes"]),

        new("8","8. OPERACIÓN",
            "8.2.2 Determinación de los requisitos para los productos y servicios","8.2.2",
            "Se determinan los requisitos para los productos y servicios ofrecidos, incluyendo los legales y reglamentarios y los que la organización considere necesarios, y se puede cumplir lo declarado.",
            "Antes de ofrecer un producto/servicio se definen todos sus requisitos (cliente, legales, propios) y se verifica que la organización puede cumplir lo que declara.",
            ["Especificaciones de producto/servicio","Matriz de requisitos legales y reglamentarios","Fichas técnicas"]),

        new("8","8. OPERACIÓN",
            "8.2.3 Revisión de los requisitos para los productos y servicios","8.2.3",
            "Antes de comprometerse a suministrar, se revisan los requisitos del cliente (incluidos los de entrega y posteriores), los no establecidos pero necesarios, los propios y los legales, conservando información documentada.",
            "Revisar la capacidad de cumplir antes de aceptar un pedido; resolver discrepancias entre lo solicitado y lo ofertado; conservar resultados de la revisión y nuevos requisitos.",
            ["Registros de revisión de pedidos/cotizaciones","Actas de revisión de requisitos","Confirmación de requisitos no documentados"]),

        new("8","8. OPERACIÓN",
            "8.2.4 Cambios en los requisitos para los productos y servicios","8.2.4",
            "Cuando cambian los requisitos del producto/servicio, se modifica la información documentada pertinente y se asegura que las personas pertinentes sean conscientes de los cambios.",
            "Todo cambio de requisitos debe reflejarse en los documentos y comunicarse a quienes ejecutan la operación.",
            ["Registros de cambios de requisitos","Comunicación de cambios a involucrados","Documentos actualizados"]),

        new("8","8. OPERACIÓN",
            "8.3.1 Diseño y desarrollo (generalidades)","8.3.1",
            "Se ha establecido, implementado y mantenido un proceso de diseño y desarrollo adecuado para asegurar la posterior provisión de productos y servicios.",
            "Aplica a Renergeia por su rol EPC (ingeniería). Debe existir un proceso formal de diseño que asegure la conformidad de lo que luego se construye/provee.",
            ["Procedimiento de diseño y desarrollo","Plan de diseño de proyectos","Memorias de cálculo / ingeniería"]),

        new("8","8. OPERACIÓN",
            "8.3.2 Planificación del diseño y desarrollo","8.3.2",
            "Se determinan las etapas y controles del diseño considerando naturaleza, duración, etapas, revisiones, verificación, validación, responsabilidades, recursos e información documentada.",
            "Planificar el diseño: etapas, puntos de control (revisión/verificación/validación), responsables, interfaces, recursos y nivel de control del cliente.",
            ["Plan de diseño y desarrollo","Cronograma de etapas de diseño","Asignación de responsabilidades de diseño"]),

        new("8","8. OPERACIÓN",
            "8.3.3 Entradas para el diseño y desarrollo","8.3.3",
            "Se determinan los elementos de entrada (requisitos funcionales y de desempeño, legales, normas, consecuencias de fallas) adecuados, completos y sin ambigüedades, conservando información documentada.",
            "Las entradas de diseño deben ser claras y completas: requisitos del cliente, normas técnicas (ej. RETIE, IEC), información de diseños similares y consecuencias potenciales de fallos.",
            ["Registro de entradas de diseño","Requisitos técnicos y normativos aplicables","Bases de diseño del proyecto"]),

        new("8","8. OPERACIÓN",
            "8.3.4 Controles del diseño y desarrollo","8.3.4",
            "Se aplican controles al proceso de diseño: definición de resultados, revisiones, verificación, validación y acciones sobre los problemas detectados, conservando información documentada.",
            "Durante el diseño se realizan revisiones (¿cumple requisitos?), verificación (¿las salidas cumplen las entradas?) y validación (¿el resultado sirve para el uso previsto?).",
            ["Actas de revisión de diseño","Registros de verificación y validación","Acciones sobre problemas de diseño"]),

        new("8","8. OPERACIÓN",
            "8.3.5 Salidas del diseño y desarrollo","8.3.5",
            "Las salidas de diseño cumplen los requisitos de entrada, son adecuadas para procesos posteriores, incluyen o referencian criterios de aceptación y especifican características esenciales para uso seguro y correcto.",
            "Los entregables de diseño (planos, especificaciones) deben ser suficientes para producir/construir y contener criterios de aceptación y características críticas.",
            ["Planos y especificaciones aprobados","Criterios de aceptación de diseño","Memorias técnicas finales"]),

        new("8","8. OPERACIÓN",
            "8.3.6 Cambios del diseño y desarrollo","8.3.6",
            "Se identifican, revisan y controlan los cambios del diseño durante o después, evitando impactos adversos en la conformidad, conservando información documentada de cambios, revisiones, autorizaciones y acciones.",
            "Todo cambio de diseño se gestiona formalmente para no afectar la conformidad, con revisión y autorización registradas.",
            ["Registros de control de cambios de diseño","Autorizaciones de cambio","Revisión de impacto del cambio"]),

        new("8","8. OPERACIÓN",
            "8.4.1 Control de procesos, productos y servicios externos (generalidades)","8.4.1",
            "Se asegura que los procesos, productos y servicios suministrados externamente son conformes, y se determinan controles y criterios de evaluación, selección, seguimiento y reevaluación de proveedores.",
            "Definir criterios para evaluar, seleccionar y reevaluar proveedores externos en función de su capacidad de cumplir requisitos, conservando registros.",
            ["Procedimiento de compras / proveedores","Criterios de evaluación y selección","Registros de reevaluación de proveedores"]),

        new("8","8. OPERACIÓN",
            "8.4.2 Tipo y alcance del control","8.4.2",
            "Se asegura que los procesos externos permanecen dentro del control del SGC, definiendo controles al proveedor y a las salidas, considerando el impacto y la eficacia de los controles del proveedor.",
            "El control sobre lo externalizado debe ser proporcional al impacto en la conformidad; definir verificaciones a aplicar a los productos/servicios recibidos.",
            ["Controles a proveedores definidos","Inspección de recepción","Verificación de servicios contratados"]),

        new("8","8. OPERACIÓN",
            "8.4.3 Información para los proveedores externos","8.4.3",
            "Se comunican a los proveedores los requisitos sobre procesos/productos, aprobación, competencia del personal, interacción, control del desempeño y actividades de verificación en sus instalaciones.",
            "Antes de comprar, comunicar claramente los requisitos al proveedor (especificaciones, métodos, aprobaciones, controles).",
            ["Órdenes de compra con requisitos","Especificaciones comunicadas al proveedor","Acuerdos de calidad con proveedores"]),

        new("8","8. OPERACIÓN",
            "8.5.1 Control de la producción y de la provisión del servicio","8.5.1",
            "La producción y provisión del servicio se realizan bajo condiciones controladas: información documentada, recursos de seguimiento, validación de procesos, infraestructura, competencia, prevención de error humano y liberación.",
            "Controlar la ejecución con especificaciones disponibles, recursos de medición, validación de procesos especiales, personal competente y actividades de liberación, entrega y posteriores.",
            ["Instructivos de trabajo / procedimientos operativos","Registros de control de producción/servicio","Validación de procesos especiales"]),

        new("8","8. OPERACIÓN",
            "8.5.2 Identificación y trazabilidad","8.5.2",
            "Se identifican las salidas cuando es necesario asegurar la conformidad, se identifica su estado respecto a los requisitos de seguimiento y medición y se controla la trazabilidad cuando es un requisito.",
            "Identificar productos/servicios y su estado (aprobado, en proceso, rechazado) y mantener trazabilidad cuando se requiere, con información documentada.",
            ["Sistema de identificación / etiquetado","Registros de trazabilidad","Estado de inspección y ensayo"]),

        new("8","8. OPERACIÓN",
            "8.5.3 Propiedad perteneciente a los clientes o proveedores externos","8.5.3",
            "Se cuida la propiedad del cliente o proveedor externo (materiales, componentes, datos, propiedad intelectual): se identifica, verifica, protege y se informa cualquier pérdida o deterioro.",
            "Toda propiedad de terceros bajo control de la organización debe cuidarse; ante pérdida, daño o inadecuación, informar y conservar registro.",
            ["Registro de propiedad del cliente","Procedimiento de control de bienes de terceros","Reportes de pérdida/deterioro"]),

        new("8","8. OPERACIÓN",
            "8.5.4 Preservación","8.5.4",
            "Se preservan las salidas durante la producción y provisión del servicio en la medida necesaria para asegurar la conformidad (identificación, manipulación, embalaje, almacenamiento, protección).",
            "Preservar producto y sus componentes (incl. condiciones de almacenamiento y transporte) para que no se deterioren antes de la entrega.",
            ["Procedimiento de almacenamiento y preservación","Condiciones de manipulación y embalaje","Registros de inventario / bodega"]),

        new("8","8. OPERACIÓN",
            "8.5.5 Actividades posteriores a la entrega","8.5.5",
            "Se cumplen los requisitos para las actividades posteriores a la entrega (garantías, mantenimiento, soporte), considerando legales, consecuencias no deseadas, ciclo de vida, requisitos del cliente y retroalimentación.",
            "Determinar el alcance de las actividades post-entrega según riesgos, naturaleza y uso del producto/servicio y compromisos asumidos.",
            ["Procedimiento de servicio posventa","Registros de garantías / mantenimiento","Acuerdos de soporte"]),

        new("8","8. OPERACIÓN",
            "8.5.6 Control de los cambios","8.5.6",
            "Se revisan y controlan los cambios en la producción o provisión del servicio para asegurar la conformidad continua, conservando información documentada de la revisión, autorización y acciones.",
            "Los cambios en la operación se gestionan formalmente para no afectar la conformidad, con autorización registrada.",
            ["Registros de control de cambios operacionales","Autorizaciones de cambio","Revisión de impacto"]),

        new("8","8. OPERACIÓN",
            "8.6 Liberación de los productos y servicios","8.6",
            "Se verifica el cumplimiento de los requisitos antes de liberar el producto/servicio; la liberación no procede hasta completar lo planificado, salvo aprobación de autoridad pertinente y del cliente, conservando evidencia.",
            "Antes de entregar se verifica conformidad contra criterios de aceptación; se conserva evidencia con trazabilidad a quien autoriza la liberación.",
            ["Registros de liberación / actas de entrega","Criterios de aceptación verificados","Firma de autoridad que libera"]),

        new("8","8. OPERACIÓN",
            "8.7 Control de las salidas no conformes","8.7",
            "Las salidas no conformes se identifican y controlan para prevenir su uso o entrega no intencionada, tomando acciones (corrección, separación, devolución, información al cliente o concesión) y conservando información documentada.",
            "Tratar producto/servicio no conforme mediante corrección, segregación, devolución o concesión autorizada; conservar evidencia de la no conformidad y de las acciones tomadas.",
            ["Registro de producto/servicio no conforme","Procedimiento de control de salidas no conformes","Autorizaciones de concesión"]),

        // ══ 9. EVALUACIÓN DEL DESEMPEÑO ═════════════════════════════════════
        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.1.1 Seguimiento, medición, análisis y evaluación (generalidades)","9.1.1",
            "Se determina qué necesita seguimiento y medición, los métodos, cuándo realizarlo y cuándo analizar/evaluar, para asegurar resultados válidos y evaluar el desempeño y la eficacia del SGC.",
            "Definir indicadores, métodos, frecuencia y momento de análisis para evaluar el desempeño del SGC, conservando información documentada como evidencia.",
            ["Ficha técnica de indicadores","Programa de seguimiento y medición","Tablero de control (KPI)"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.1.2 Satisfacción del cliente","9.1.2",
            "Se realiza seguimiento de las percepciones del cliente sobre el cumplimiento de sus necesidades y expectativas, y se determinan los métodos para obtener, seguir y revisar esta información.",
            "Medir la satisfacción del cliente mediante encuestas, retroalimentación, reuniones, quejas, felicitaciones o análisis de cuota de mercado.",
            ["Encuestas de satisfacción del cliente","Análisis de PQR","Informe de percepción del cliente"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.1.3 Análisis y evaluación","9.1.3",
            "Se analizan y evalúan los datos del seguimiento para demostrar conformidad, satisfacción del cliente, desempeño de procesos, eficacia de acciones sobre riesgos, desempeño de proveedores y necesidad de mejora.",
            "Los datos recopilados deben analizarse (no solo recolectarse) para tomar decisiones sobre el desempeño y la mejora del SGC.",
            ["Informe de análisis de datos","Análisis de indicadores / tendencias","Entradas para la revisión por la dirección"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.2 Auditoría interna","9.2",
            "Se realizan auditorías internas a intervalos planificados mediante un programa que considera importancia de procesos y resultados previos, con criterios, alcance, auditores objetivos, reporte e información documentada.",
            "Programa de auditorías con auditores imparciales; los resultados se informan a la dirección pertinente y se toman correcciones sin demora injustificada.",
            ["Programa anual de auditorías","Plan e informe de auditoría","Registro de no conformidades y competencia de auditores"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.3.1 Revisión por la dirección (generalidades)","9.3.1",
            "La alta dirección revisa el SGC a intervalos planificados para asegurar su conveniencia, adecuación, eficacia y alineación con la dirección estratégica.",
            "La revisión es responsabilidad indelegable de la alta dirección y se planifica con una frecuencia definida.",
            ["Programación de la revisión por la dirección","Acta de revisión por la dirección","Convocatoria y asistentes"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.3.2 Entradas de la revisión por la dirección","9.3.2",
            "La revisión considera: estado de acciones previas, cambios en cuestiones externas/internas, información del desempeño (satisfacción, objetivos, procesos, NC, seguimiento, auditorías, proveedores), recursos, eficacia de acciones sobre riesgos y oportunidades de mejora.",
            "Deben presentarse todas las entradas exigidas por la norma para una revisión completa y útil.",
            ["Informe de entradas de la revisión","Tablero de indicadores","Informes de auditoría y satisfacción"]),

        new("9","9. EVALUACIÓN DEL DESEMPEÑO",
            "9.3.3 Salidas de la revisión por la dirección","9.3.3",
            "Las salidas incluyen decisiones y acciones sobre oportunidades de mejora, necesidades de cambio en el SGC y necesidades de recursos, conservando información documentada.",
            "La revisión debe producir decisiones documentadas: mejoras, cambios y recursos, con responsables y seguimiento.",
            ["Acta con decisiones y compromisos","Plan de acción post-revisión","Asignación de recursos aprobada"]),

        // ══ 10. MEJORA ══════════════════════════════════════════════════════
        new("10","10. MEJORA",
            "10.1 Generalidades","10.1",
            "Se determinan y seleccionan oportunidades de mejora y se implementan acciones para cumplir requisitos del cliente y aumentar su satisfacción (mejora de productos/servicios, corrección/prevención y mejora del SGC).",
            "La mejora abarca corregir, prevenir o reducir efectos no deseados y mejorar el desempeño y la eficacia del SGC, incluyendo mejora reactiva, incremental, innovación y reorganización.",
            ["Registro de oportunidades de mejora","Plan de mejoramiento","Indicadores de desempeño"]),

        new("10","10. MEJORA",
            "10.2 No conformidad y acción correctiva","10.2.1",
            "Ante una no conformidad (incluidas las de quejas) se reacciona para controlarla y corregirla, se hace frente a las consecuencias y se evalúa la necesidad de eliminar sus causas para que no vuelva a ocurrir.",
            "Análisis de causa raíz, determinación de causas, evaluación de NC similares, implementación de acciones y revisión de su eficacia; actualización de riesgos y del SGC si procede.",
            ["Procedimiento de NC y acciones correctivas","Análisis de causa raíz","Registro de no conformidades"]),

        new("10","10. MEJORA",
            "10.2 No conformidad y acción correctiva","10.2.2",
            "Se conserva información documentada como evidencia de la naturaleza de las no conformidades, las acciones tomadas y los resultados de las acciones correctivas.",
            "Debe quedar trazabilidad completa: qué pasó, qué se hizo y qué resultado dio la acción correctiva.",
            ["Registros de acciones correctivas","Verificación de eficacia","Cierre documentado de NC"]),

        new("10","10. MEJORA",
            "10.3 Mejora continua","10.3",
            "Se mejora continuamente la conveniencia, adecuación y eficacia del SGC, considerando los resultados del análisis, evaluación y las salidas de la revisión por la dirección para determinar necesidades u oportunidades de mejora.",
            "La mejora continua se nutre de los análisis de datos y de las decisiones de la dirección, cerrando el ciclo PHVA.",
            ["Plan de mejora continua","Tendencias de indicadores","Seguimiento de decisiones de la dirección"]),
    ];

    // Diccionario de acceso rápido por Id de requisito
    private static readonly Dictionary<string, RequisitoISO> _porId =
        Requisitos.ToDictionary(r => r.Id);

    // Búsqueda exacta
    public static RequisitoISO? GetById(string id) =>
        _porId.TryGetValue(id, out var r) ? r : null;

    // Búsqueda tolerante: maneja IDs de versiones antiguas (ej. "4.1.1" → "4.1", "7.1" → "7.1.1")
    public static RequisitoISO? GetByIdFuzzy(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;

        // 1. Coincidencia exacta
        if (_porId.TryGetValue(id, out var exact)) return exact;

        // 2. Truncar último segmento: "4.1.1" → "4.1", "8.2.1" → "8.2"
        var lastDot = id.LastIndexOf('.');
        if (lastDot > 0)
        {
            var parent = id[..lastDot];
            if (_porId.TryGetValue(parent, out var parentR)) return parentR;
        }

        // 3. Buscar por prefijo: "7.1" encuentra "7.1.1" (el primero que empiece igual)
        var byPrefix = _porId.Values.FirstOrDefault(r => r.Id.StartsWith(id + ".", StringComparison.Ordinal));
        if (byPrefix is not null) return byPrefix;

        // 4. Coincidencia parcial por cláusula principal como último recurso
        var clausula = id.Contains('.') ? id[..id.IndexOf('.')] : id;
        return _porId.Values.FirstOrDefault(r => r.NumClausula == clausula);
    }
}
