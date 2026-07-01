namespace RenergeIA.Web.Services;

public static class Decreto1072ChecklistData
{
    public static readonly IReadOnlyList<RequisitoNorma> Requisitos =
    [
        // ══ CAPÍTULO 6 — SISTEMA DE GESTIÓN DE LA SEGURIDAD Y SALUD EN EL TRABAJO ══

        new("2.2.4.6.1","2.2.4.6.1 Objeto y campo de aplicación",
            "Art. 2.2.4.6.1 — Objeto","2.2.4.6.1",
            "El empleador aplica el SG-SST en todos sus centros de trabajo, cubriendo a todos sus trabajadores (dependientes, contratistas, subcontratistas, estudiantes en práctica y trabajadores de servicios temporales).",
            "El decreto establece que el SG-SST debe aplicarse a todos los empleadores y contratantes, para todos los trabajadores y en todos los centros de trabajo. Debe haber un documento o política que declare el alcance.",
            ["Política SST","Acta de implementación SG-SST","Resolución interna de adopción"]),

        new("2.2.4.6.2","2.2.4.6.2 Definiciones",
            "Art. 2.2.4.6.2 — Definiciones aplicadas","2.2.4.6.2",
            "Los términos utilizados en los documentos del SG-SST (acción correctiva, acción preventiva, ciclo PHVA, incidente, etc.) están alineados con las definiciones del Decreto 1072/2015.",
            "Verificar que los procedimientos y registros utilicen los conceptos del decreto correctamente (ciclo PHVA, mejora continua, peligro vs riesgo).",
            ["Glosario SST","Manual SG-SST","Procedimiento de gestión de riesgos"]),

        new("2.2.4.6.3","2.2.4.6.3 Seguridad y Salud en el Trabajo",
            "Art. 2.2.4.6.3 — Implementación SG-SST","2.2.4.6.3",
            "El empleador ha diseñado e implementado el Sistema de Gestión de la Seguridad y Salud en el Trabajo (SG-SST) con un enfoque de proceso basado en el ciclo PHVA.",
            "El SG-SST debe estar documentado, implementado y en mejora continua. El ciclo PHVA debe evidenciarse en la planificación, implementación, seguimiento y mejora.",
            ["Plan de trabajo anual SST","Programa de implementación SG-SST","Acta de revisión por la dirección"]),

        // ══ POLÍTICA SST ══

        new("2.2.4.6.5","Política de SST",
            "Art. 2.2.4.6.5 — Política de SST","2.2.4.6.5",
            "Existe una política de SST firmada por el representante legal, que incluye el compromiso con la protección de la seguridad y salud de todos los trabajadores, el cumplimiento normativo, la mejora continua y que es comunicada a todas las partes interesadas.",
            "La política debe ser específica a la empresa (no una copia genérica), estar fechada y firmada por la alta dirección, incluir todos los compromisos requeridos y ser difundida y visible en la organización.",
            ["Política SST firmada","Evidencias de comunicación (correo, cartelera)","Listado de asistencia a socialización"]),

        new("2.2.4.6.6","Obligaciones del Empleador",
            "Art. 2.2.4.6.6 — Obligaciones generales","2.2.4.6.6",
            "El empleador cumple las obligaciones establecidas: definir y firmar la política SST, asignar recursos, garantizar la participación de los trabajadores, implementar la IPVR, realizar exámenes médicos, afiliar a ARL y pagar oportunamente, y garantizar los equipos y herramientas en condiciones seguras.",
            "Se verifican los 21 numerales del artículo. Son obligaciones mínimas del empleador. Requiere evidencia documentada para cada una.",
            ["Certificados de afiliación ARL","Constancias de pago ARL","Exámenes de ingreso/periódicos","Registros de dotación EPP","Presupuesto SST"]),

        new("2.2.4.6.8","Obligaciones de las ARL",
            "Art. 2.2.4.6.8 — Asesoría ARL","2.2.4.6.8",
            "La empresa recibe asesoría y asistencia técnica de la ARL para el diseño e implementación del SG-SST, incluyendo visitas, capacitaciones y conceptos técnicos.",
            "Verificar que la ARL ha prestado servicios de asesoría. Esto incluye visitas de seguimiento, conceptos técnicos, capacitaciones o actividades de prevención.",
            ["Registro de visitas ARL","Actas de asesoría ARL","Certificados de capacitación ARL"]),

        // ══ PLANIFICACIÓN ══

        new("2.2.4.6.12","Planificación — Evaluación Inicial",
            "Art. 2.2.4.6.12 — Evaluación inicial","2.2.4.6.12",
            "Se realizó la evaluación inicial del SG-SST mediante los estándares mínimos o una auditoría interna, identificando las prioridades en SST para el año vigente.",
            "La evaluación inicial es el diagnóstico base del SG-SST. Debe estar documentada, fechada y usada para elaborar el plan de trabajo anual.",
            ["Evaluación inicial documentada","Autoevaluación estándares mínimos","Plan de mejora post-evaluación"]),

        new("2.2.4.6.13","Planificación — Identificación de Peligros",
            "Art. 2.2.4.6.13 — Identificación de peligros y valoración","2.2.4.6.13",
            "Se tiene metodología documentada para la identificación continua de peligros, evaluación y valoración de riesgos con participación de todos los niveles de la empresa. Se implementan medidas de intervención según la jerarquía de controles.",
            "La metodología (GTC 45 u otra) debe estar definida y aplicada. La IPVR debe cubrir todos los procesos, actividades rutinarias y no rutinarias, cargos y zonas de la empresa. Las medidas de control deben ser consecuentes con el nivel de riesgo.",
            ["Matriz de peligros y valoración de riesgos (IPVR)","Procedimiento identificación de peligros","Registro de participación de trabajadores"]),

        new("2.2.4.6.14","Planificación — Objetivos y Plan de Trabajo",
            "Art. 2.2.4.6.14 — Plan de trabajo anual SST","2.2.4.6.14",
            "Existe un plan de trabajo anual firmado por el empleador que incluye los objetivos del SG-SST, metas, responsables, recursos y cronograma de actividades para el año vigente.",
            "El plan de trabajo anual debe estar basado en la evaluación inicial y la matriz de peligros. Debe incluir objetivos medibles, responsables, plazos y presupuesto asignado.",
            ["Plan de trabajo anual SST firmado","Cronograma de actividades SST","Presupuesto SST aprobado"]),

        // ══ IMPLEMENTACIÓN ══

        new("2.2.4.6.15","Implementación — Gestión de Peligros y Riesgos",
            "Art. 2.2.4.6.15 — Aplicación de controles","2.2.4.6.15",
            "Se implementan las medidas de prevención y control derivadas de la IPVR siguiendo la jerarquía: eliminación, sustitución, controles de ingeniería, controles administrativos y EPP.",
            "Los controles deben ser proporcionales al nivel de riesgo. Los controles de ingeniería y administrativos deben estar documentados. El EPP debe tener registros de entrega.",
            ["Registros de entrega EPP","Fichas técnicas EPP","Procedimientos de control de riesgos críticos","Registros de mantenimiento de controles"]),

        new("2.2.4.6.16","Implementación — Capacitación",
            "Art. 2.2.4.6.16 — Programa de capacitación","2.2.4.6.16",
            "Existe un programa de capacitación en SST que cubre a todos los trabajadores, incluyendo inducción, reinducción, capacitación en riesgos específicos, uso de EPP, procedimientos de emergencia y derechos y deberes en SST.",
            "El programa debe estar documentado, con evidencias de ejecución (actas de asistencia, material entregado). Debe incluir evaluación de efectividad de la capacitación.",
            ["Programa de capacitación anual SST","Actas de asistencia a capacitaciones","Evaluaciones de efectividad","Registro de inducción y reinducción"]),

        new("2.2.4.6.17","Implementación — Prevención Enfermedades Laborales",
            "Art. 2.2.4.6.17 — Vigilancia epidemiológica","2.2.4.6.17",
            "Se han implementado programas de vigilancia epidemiológica para los riesgos identificados como prioritarios (ruido, posturas, químicos, etc.) con seguimiento médico periódico.",
            "Los programas de vigilancia deben estar basados en la IPVR y los resultados de los exámenes médicos. Deben incluir indicadores de seguimiento.",
            ["Programas de vigilancia epidemiológica","Resultados exámenes médicos ocupacionales","Estadísticas de morbilidad y ausentismo"]),

        new("2.2.4.6.25","Implementación — Emergencias",
            "Art. 2.2.4.6.25 — Plan de prevención y respuesta ante emergencias","2.2.4.6.25",
            "Existe un plan de prevención, preparación y respuesta ante emergencias documentado, con brigadas de emergencia conformadas, capacitadas y dotadas, y simulacros realizados al menos una vez al año.",
            "El plan debe identificar las amenazas, vulnerabilidades, zonas de riesgo y recursos disponibles. Las brigadas deben tener acta de conformación y certificaciones. Los simulacros deben tener evidencia fotográfica y acta de evaluación.",
            ["Plan de emergencias","Acta conformación brigada de emergencias","Certificados de capacitación brigadistas","Registro y evaluación simulacros"]),

        // ══ VERIFICACIÓN ══

        new("2.2.4.6.29","Verificación — Indicadores",
            "Art. 2.2.4.6.29 — Indicadores del SG-SST","2.2.4.6.29",
            "Se definen, recopilan y analizan indicadores de estructura (disponibilidad de recursos), proceso (cumplimiento de actividades) y resultado (accidentalidad, ausentismo, enfermedad laboral) del SG-SST.",
            "Los indicadores deben estar definidos con fórmula, fuente de datos, frecuencia de medición y responsable. Los resultados deben analizarse y usarse para la mejora.",
            ["Ficha técnica de indicadores SST","Informes periódicos de indicadores","Estadísticas de accidentalidad","Tasas de ausentismo"]),

        new("2.2.4.6.30","Verificación — Auditoría",
            "Art. 2.2.4.6.30 — Auditoría interna del SG-SST","2.2.4.6.30",
            "Se realiza al menos una auditoría interna anual al SG-SST para verificar el cumplimiento del Decreto 1072/2015. El auditor es competente y el proceso está documentado.",
            "La auditoría debe cubrir todos los componentes del SG-SST. Los hallazgos deben registrarse y gestionarse mediante acciones correctivas. El informe de auditoría debe quedar archivado.",
            ["Plan de auditoría interna SG-SST","Informe de auditoría","Registro de hallazgos y acciones","Competencia del auditor"]),

        new("2.2.4.6.31","Verificación — Revisión por la Alta Dirección",
            "Art. 2.2.4.6.31 — Revisión por la dirección","2.2.4.6.31",
            "La alta dirección revisa el SG-SST al menos una vez al año, analizando los resultados de indicadores, auditorías, investigación de incidentes, cambios en la empresa y recomendaciones de las partes interesadas.",
            "La revisión debe estar documentada en un acta firmada por la alta dirección. Los resultados deben usarse para actualizar el plan de trabajo y los objetivos del SG-SST.",
            ["Acta de revisión por la dirección","Informe presentado a la alta dirección","Plan de mejora resultante"]),

        // ══ MEJORA CONTINUA ══

        new("2.2.4.6.32","Mejora — Investigación de Incidentes",
            "Art. 2.2.4.6.32 — Investigación de accidentes e incidentes","2.2.4.6.32",
            "Se investigan todos los accidentes e incidentes de trabajo, identificando causas básicas e inmediatas, implementando acciones correctivas y reportando a la ARL y al Ministerio cuando aplica.",
            "La investigación debe realizarse en máximo 15 días hábiles para accidentes graves o mortales. Debe contar con el equipo investigador, análisis de causas raíz y seguimiento a las acciones correctivas.",
            ["Informes de investigación de AT/EL","Reportes a ARL (FURAT)","Seguimiento acciones correctivas","Estadísticas de accidentalidad"]),

        new("2.2.4.6.33","Mejora — Acciones Correctivas y Preventivas",
            "Art. 2.2.4.6.33 — Acciones correctivas y preventivas","2.2.4.6.33",
            "Se definen e implementan acciones correctivas y preventivas derivadas de las auditorías, investigaciones de accidentes, revisiones por la dirección y monitoreo del SG-SST.",
            "Las acciones deben tener responsable, plazo, recursos y seguimiento documentado. Se verifica la eficacia de las acciones implementadas.",
            ["Registro de acciones correctivas y preventivas","Seguimiento y cierre de ACAP","Evidencias de implementación"]),
    ];
}
