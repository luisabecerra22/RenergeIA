namespace RenergeIA.Web.Services;

public record RequisitoRes0312(
    string PhvaKey,
    string TituloPhva,
    string NumGrupo,
    string TituloGrupo,
    string SubClausula,
    string Id,
    decimal Peso,
    string Req,
    string Interp,
    string[] Docs
);

public static class Resolucion0312ChecklistData
{
    public static readonly IReadOnlyList<RequisitoRes0312> Requisitos = new List<RequisitoRes0312>
    {
        // ══════════════════════════════════════════════════════
        // CICLO PLANEAR · 25 puntos
        // ══════════════════════════════════════════════════════

        // I · RECURSOS (10 puntos) · 1.1 Recursos Financieros, Técnicos, Humanos · 4 puntos
        new("planear","CICLO PLANEAR · 25 puntos","I","I. RECURSOS (10 puntos)",
            "1.1 Recursos Financieros, Técnicos, Humanos y de Otra Índole – 4 puntos",
            "1.1.1", 0.5m,
            "Responsable del SG-SST designado mediante acto administrativo con licencia vigente en SST y funciones definidas.",
            "Para +50 trabajadores el responsable debe ser profesional con posgrado en SST y licencia vigente expedida por la Secretaría de Salud. Debe designarse formalmente con funciones específicas y tiempo asignado dentro de su jornada laboral.",
            new[]{"Acta de designación del responsable del SG-SST","Licencia en SST vigente","Resolución o contrato con funciones definidas","Diploma de posgrado o certificado del curso de 50 horas"}),

        new("planear","CICLO PLANEAR · 25 puntos","I","I. RECURSOS (10 puntos)",
            "1.1 Recursos Financieros, Técnicos, Humanos y de Otra Índole – 4 puntos",
            "1.1.2", 0.5m,
            "Responsabilidades en SST definidas y documentadas para todos los niveles: alta dirección, mandos medios, supervisores y trabajadores.",
            "Todos los niveles tienen sus responsabilidades en SST documentadas y comunicadas. No es un responsable único: todos tienen obligaciones conforme al Decreto 1072/15 art. 2.2.4.6.8.",
            new[]{"Manual de funciones con responsabilidades SST","Acta de compromiso de la alta dirección","Rendición de cuentas en SST por nivel jerárquico"}),

        new("planear","CICLO PLANEAR · 25 puntos","I","I. RECURSOS (10 puntos)",
            "1.1 Recursos Financieros, Técnicos, Humanos y de Otra Índole – 4 puntos",
            "1.1.3", 0.5m,
            "Recursos humanos, físicos, económicos y técnicos asignados y suficientes para el desarrollo del SG-SST.",
            "La alta dirección asigna y garantiza los recursos para el cumplimiento del plan de trabajo anual. Se evidencian en el presupuesto aprobado y la ejecución frente al plan.",
            new[]{"Presupuesto del SG-SST aprobado y firmado","Acta de asignación de recursos","Plan de trabajo con recursos estimados por actividad"}),

        new("planear","CICLO PLANEAR · 25 puntos","I","I. RECURSOS (10 puntos)",
            "1.1 Recursos Financieros, Técnicos, Humanos y de Otra Índole – 4 puntos",
            "1.1.4", 0.5m,
            "Todos los trabajadores (propios, contratistas y temporales) afiliados al Sistema General de Riesgos Laborales según su nivel de riesgo.",
            "El empleador responde por la afiliación de sus trabajadores directos. Para contratistas verifica que sus trabajadores estén afiliados antes de iniciar labores.",
            new[]{"Planillas PILA con aportes a SGRL","Certificados de afiliación vigentes a la ARL","Verificación de afiliación de trabajadores de contratistas"}),

        new("planear","CICLO PLANEAR · 25 puntos","I","I. RECURSOS (10 puntos)",
            "1.1 Recursos Financieros, Técnicos, Humanos y de Otra Índole – 4 puntos",
            "1.1.5", 0.5m,
            "Trabajadores en actividades de alto riesgo (categorías IV y V, Decreto 2090/03) con pago del 75% de cotización a pensiones a cargo del empleador.",
            "Para actividades de alto riesgo según el Decreto 2090/03, el empleador asume el 75% de la cotización especial al fondo de pensiones. Aplica a actividades de instalación y mantenimiento eléctrico de alto riesgo en Renergeia.",
            new[]{"Planillas de pago especial a pensiones (riesgo IV-V)","Soporte de cotización del 75% por empleador","Clasificación interna de actividades de alto riesgo"}),

        new("planear","CICLO PLANEAR · 25 puntos","I","I. RECURSOS (10 puntos)",
            "1.1 Recursos Financieros, Técnicos, Humanos y de Otra Índole – 4 puntos",
            "1.1.6", 0.5m,
            "COPASST conformado con igual número de representantes, elegido por votación libre, inscrito ante el Ministerio, vigencia de 2 años y reuniones mensuales.",
            "El COPASST es obligatorio para empresas ≥10 trabajadores. Debe elegirse por votación libre, inscribirse ante el Ministerio de Trabajo y reunirse mensualmente, con actas documentadas.",
            new[]{"Acta de constitución y elección del COPASST","Formulario de inscripción ante el Ministerio del Trabajo","Resolución de nombramiento","Actas de reunión mensual"}),

        new("planear","CICLO PLANEAR · 25 puntos","I","I. RECURSOS (10 puntos)",
            "1.1 Recursos Financieros, Técnicos, Humanos y de Otra Índole – 4 puntos",
            "1.1.7", 0.5m,
            "Integrantes del COPASST o Vigía de SST capacitados para el ejercicio de sus funciones (inspecciones, investigación de AT, auditoría, entre otras).",
            "El empleador debe capacitar a los integrantes del COPASST en sus funciones legales: inspecciones, investigación de AT, revisión del plan de trabajo, etc. Debe realizarse dentro de la jornada.",
            new[]{"Registros de capacitación del COPASST","Plan de capacitación de los integrantes","Certificados de formación","Actas del COPASST con temas de capacitación"}),

        new("planear","CICLO PLANEAR · 25 puntos","I","I. RECURSOS (10 puntos)",
            "1.1 Recursos Financieros, Técnicos, Humanos y de Otra Índole – 4 puntos",
            "1.1.8", 0.5m,
            "Comité de Convivencia Laboral conformado (Res. 652 y 1356/2012), capacitado y con reuniones trimestrales documentadas.",
            "Obligatorio para empresas ≥20 trabajadores. Se conforma con igual número de representantes, vigencia de 2 años. Funciones: recibir y tramitar quejas de acoso laboral y promover la convivencia.",
            new[]{"Acta de constitución del Comité de Convivencia","Actas de reuniones trimestrales","Registros de capacitación de sus integrantes","Evidencia de atención de casos"}),

        // I · 1.2 Capacitación en SST · 6 puntos
        new("planear","CICLO PLANEAR · 25 puntos","I","I. RECURSOS (10 puntos)",
            "1.2 Capacitación en el Sistema de Gestión de la SST – 6 puntos",
            "1.2.1", 2m,
            "Programa de capacitación en Promoción y Prevención (PYP) con los peligros identificados, cronograma, responsables y evidencias de ejecución.",
            "El programa cubre todos los peligros de la IPEVR, diferenciado por cargo y riesgo, ejecutado dentro del horario de trabajo y con evaluación de comprensión. Incluye PYP en salud, riesgos del cargo y derechos/deberes.",
            new[]{"Programa anual de capacitación en SST","Cronograma de capacitaciones","Registros de asistencia firmados","Evaluaciones de comprensión post-capacitación"}),

        new("planear","CICLO PLANEAR · 25 puntos","I","I. RECURSOS (10 puntos)",
            "1.2 Capacitación en el Sistema de Gestión de la SST – 6 puntos",
            "1.2.2", 2m,
            "Inducción y reinducción en SST para todos los trabajadores (incluidos temporales y contratistas) con contenidos sobre riesgos del cargo, EPP y emergencias.",
            "La inducción al ingreso cubre: política, objetivos, peligros del cargo, EPP, plan de emergencias y derechos/deberes. La reinducción se realiza periódicamente o ante cambios de cargo o proceso.",
            new[]{"Procedimiento de inducción y reinducción en SST","Registros de inducción firmados por cada trabajador","Evaluación de comprensión","Inducción a contratistas documentada"}),

        new("planear","CICLO PLANEAR · 25 puntos","I","I. RECURSOS (10 puntos)",
            "1.2 Capacitación en el Sistema de Gestión de la SST – 6 puntos",
            "1.2.3", 2m,
            "Responsable del SG-SST con curso virtual de 50 horas del Ministerio (o entidad habilitada) o posgrado en SST, con licencia vigente.",
            "Sin este requisito el estándar no puede cumplirse. El certificado de 50 horas debe ser del SENA o entidad habilitada por el Ministerio. La licencia en SST la expide la Secretaría de Salud departamental.",
            new[]{"Certificado del curso de 50 horas (SENA / entidad habilitada)","Licencia en SST vigente del responsable","Diploma de posgrado en SST (si aplica)"}),

        // ══════════════════════════════════════════════════════
        // II · GESTIÓN INTEGRAL DEL SG-SST (15 puntos)
        // ══════════════════════════════════════════════════════
        new("planear","CICLO PLANEAR · 25 puntos","II","II. GESTIÓN INTEGRAL DEL SG-SST (15 puntos)",
            "2.1 Política de SST – 1 punto",
            "2.1.1", 1m,
            "Política del SG-SST firmada por el representante legal, fechada, con compromisos del art. 2.2.4.6.7 del Decreto 1072/15, comunicada al COPASST y todos los trabajadores.",
            "La política debe: estar firmada y fechada por el representante legal, contener los 5 compromisos del Decreto 1072 (mejora continua, cumplimiento legal, compromiso de toda la organización, PYP y protección de la salud), y evidenciar comunicación amplia.",
            new[]{"Política del SG-SST firmada y vigente","Evidencia de comunicación (carteleras, firmas, correos)","Acta de socialización al COPASST","Registro de revisión anual de la política"}),

        new("planear","CICLO PLANEAR · 25 puntos","II","II. GESTIÓN INTEGRAL DEL SG-SST (15 puntos)",
            "2.2 Objetivos del SG-SST – 1 punto",
            "2.2.1", 1m,
            "Objetivos del SG-SST claros, medibles, cuantificables con metas, coherentes con la política, con responsables y cronograma de seguimiento, socializados.",
            "Los objetivos deben cubrir las dimensiones de la política (PYP, cumplimiento legal, mejora continua) y tener indicadores con línea base y meta. Deben comunicarse a todos los niveles y actualizarse según resultados.",
            new[]{"Documento de objetivos y metas del SG-SST","Indicadores con línea base y meta definida","Plan de trabajo alineado con los objetivos"}),

        new("planear","CICLO PLANEAR · 25 puntos","II","II. GESTIÓN INTEGRAL DEL SG-SST (15 puntos)",
            "2.3 Evaluación inicial – 1 punto",
            "2.3.1", 1m,
            "Evaluación inicial del SG-SST con los estándares mínimos (Res. 0312/19), identificación de prioridades y resultados incorporados al plan de trabajo.",
            "La evaluación inicial es el diagnóstico de partida del sistema. Se realiza al iniciar, anualmente o ante cambios. Sus resultados determinan las prioridades del plan de trabajo y se comparten con el COPASST.",
            new[]{"Autoevaluación de estándares mínimos (Res. 0312/19)","Análisis de brechas o diagnóstico inicial","Plan de trabajo derivado de la evaluación","Socialización al COPASST"}),

        new("planear","CICLO PLANEAR · 25 puntos","II","II. GESTIÓN INTEGRAL DEL SG-SST (15 puntos)",
            "2.4 Plan de Trabajo Anual – 2 puntos",
            "2.4.1", 2m,
            "Plan Anual de Trabajo firmado por el representante legal con objetivos, metas, actividades, responsables, recursos y cronograma para el año en curso.",
            "El plan debe ser aprobado y firmado por el representante legal, derivado de la evaluación inicial y resultados del año anterior, e incluir actividades para cumplir todos los estándares mínimos.",
            new[]{"Plan Anual de Trabajo firmado por el representante legal","Cronograma de actividades con responsables y recursos","Seguimiento de ejecución mensual","Plan del año anterior con evaluación de resultados"}),

        new("planear","CICLO PLANEAR · 25 puntos","II","II. GESTIÓN INTEGRAL DEL SG-SST (15 puntos)",
            "2.5 Conservación de la documentación – 2 puntos",
            "2.5.1", 2m,
            "Archivo y retención de la información documentada del SG-SST conforme a los tiempos legales (mínimo 20 años para historias clínicas e investigaciones de AT).",
            "Conservar: historia clínica ocupacional (20 años post retiro), investigaciones de AT (20 años), indicadores, documentos del sistema y demás registros exigidos por la normativa vigente.",
            new[]{"Procedimiento de control y retención documental","Listado maestro de documentos con tiempos de retención","Registros históricos archivados y accesibles"}),

        new("planear","CICLO PLANEAR · 25 puntos","II","II. GESTIÓN INTEGRAL DEL SG-SST (15 puntos)",
            "2.6 Rendición de cuentas – 1 punto",
            "2.6.1", 1m,
            "La alta dirección y todos los niveles rinden cuentas periódicamente sobre el cumplimiento de sus responsabilidades en SST.",
            "La rendición de cuentas en SST involucra a todos los niveles: directivos, jefes, supervisores y trabajadores informan sobre el cumplimiento de sus obligaciones en SST.",
            new[]{"Actas de rendición de cuentas en SST","Informe de desempeño del SG-SST","Acta de revisión por la dirección","Seguimiento de compromisos adquiridos"}),

        new("planear","CICLO PLANEAR · 25 puntos","II","II. GESTIÓN INTEGRAL DEL SG-SST (15 puntos)",
            "2.7 Normatividad vigente en SST – 2 puntos",
            "2.7.1", 2m,
            "Matriz legal actualizada con todas las normas de SST aplicables: Ley 1562/12, Decreto 1072/15, Res. 0312/19, RETIE, CNO 1549 y normas del sector energético.",
            "El normograma incluye normas nacionales, sectoriales y locales con estado de cumplimiento y acciones para brechas. Debe actualizarse ante nuevas normas. Para Renergeia aplica especialmente la normativa del sector energético solar.",
            new[]{"Matriz legal / normograma de SST actualizado","Procedimiento de actualización legal","Evidencia de revisión periódica","Normas específicas del sector energético fotovoltaico"}),

        new("planear","CICLO PLANEAR · 25 puntos","II","II. GESTIÓN INTEGRAL DEL SG-SST (15 puntos)",
            "2.8 Comunicación y auto reporte – 1 punto",
            "2.8.1", 1m,
            "Mecanismos documentados de comunicación interna y externa en SST, canales de auto reporte de condiciones inseguras y coordinación activa con la ARL.",
            "Los trabajadores pueden reportar condiciones inseguras sin temor a represalias. La coordinación con la ARL debe ser activa y documentada (programa de servicios, visitas, capacitaciones).",
            new[]{"Procedimiento de comunicaciones en SST","Registros de auto reporte de condiciones inseguras","Comunicaciones con la ARL (planes, visitas, servicios)"}),

        new("planear","CICLO PLANEAR · 25 puntos","II","II. GESTIÓN INTEGRAL DEL SG-SST (15 puntos)",
            "2.9 Adquisición de productos y servicios en SST – 1 punto",
            "2.9.1", 1m,
            "Procedimiento para identificar y evaluar especificaciones en SST de productos y servicios adquiridos antes de su compra o contratación.",
            "Antes de adquirir equipos, sustancias o servicios se evalúan sus implicaciones para la SST: FDS, verificación técnica de equipos y habilitaciones de contratistas de servicios especializados.",
            new[]{"Procedimiento de adquisiciones en SST","Fichas de Datos de Seguridad (FDS) de sustancias","Criterios SST en órdenes de compra"}),

        new("planear","CICLO PLANEAR · 25 puntos","II","II. GESTIÓN INTEGRAL DEL SG-SST (15 puntos)",
            "2.10 Evaluación y selección de contratistas – 2 puntos",
            "2.10.1", 2m,
            "Proceso de evaluación y selección de contratistas en SST que verifica afiliación al SGRL, inducción, EPP, habilitaciones para trabajo de alto riesgo y cumplimiento legal.",
            "Los contratistas deben cumplir los requisitos SST: afiliación verificada al SGRL, inducción al inicio, EPP adecuados, certificados para trabajo de alto riesgo (alturas, eléctrico) y seguimiento a su accidentalidad.",
            new[]{"Procedimiento de gestión de contratistas en SST","Evaluación HSE de contratistas previa al inicio","Documentación SST de contratistas recibida","Seguimiento a indicadores de accidentalidad de contratistas"}),

        new("planear","CICLO PLANEAR · 25 puntos","II","II. GESTIÓN INTEGRAL DEL SG-SST (15 puntos)",
            "2.11 Gestión del cambio – 1 punto",
            "2.11.1", 1m,
            "Evaluación del impacto sobre el SG-SST de cambios internos (procesos, equipos, cargos) y externos (normativos) antes de implementarlos.",
            "Todo cambio significativo requiere evaluación previa de implicaciones para la SST, actualización de la IPEVR cuando corresponda y comunicación a los trabajadores afectados.",
            new[]{"Procedimiento de gestión del cambio en SST","Registros de evaluación de cambios","Actualización de la IPEVR ante cambios","Comunicación de cambios a trabajadores"}),

        // ══════════════════════════════════════════════════════
        // CICLO HACER · 60 puntos
        // ══════════════════════════════════════════════════════

        // III · GESTIÓN DE LA SALUD (20 puntos) · 3.1 (9 puntos)
        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.1 Condiciones de salud en el trabajo – 9 puntos",
            "3.1.1", 1m,
            "Programa de evaluaciones médicas ocupacionales con tipos de examen (ingreso, periódico, retiro), frecuencia y criterios según los peligros del cargo.",
            "El programa lo elabora el médico especialista en SST, basado en perfiles de cargo y peligros de la IPEVR, definiendo exámenes específicos según el tipo de exposición (alturas, trabajo eléctrico, carga física).",
            new[]{"Programa de evaluaciones médicas ocupacionales","Cronograma de exámenes periódicos vigente","Contrato con IPS habilitada en SST"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.1 Condiciones de salud en el trabajo – 9 puntos",
            "3.1.2", 1m,
            "Actividades de Promoción y Prevención en Salud (PYP) desarrolladas conforme al art. 2.2.4.6.24 del Decreto 1072/15.",
            "Incluye: vacunación, programas de estilos de vida saludable, actividad física, nutrición, manejo del estrés y vigilancia epidemiológica de riesgos ocupacionales del sector.",
            new[]{"Plan de actividades de PYP en salud","Registros de ejecución de actividades","Cronograma de campañas de salud","Convenio con la ARL para actividades de PYP"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.1 Condiciones de salud en el trabajo – 9 puntos",
            "3.1.3", 1m,
            "El médico evaluador recibe información completa sobre perfiles de cargo, peligros y condiciones de trabajo antes de realizar los exámenes.",
            "El médico requiere: perfiles de cargo con exigencias físicas y cognitivas, e información sobre los peligros de la IPEVR para cada cargo, antes de definir los exámenes específicos por exposición.",
            new[]{"Perfiles de cargo / profesiogramas entregados al médico","Comunicación formal a la IPS con información de peligros","Descripción de condiciones de trabajo por cargo"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.1 Condiciones de salud en el trabajo – 9 puntos",
            "3.1.4", 1m,
            "Exámenes médicos de ingreso, periódicos y de retiro realizados según lo planificado; conceptos de aptitud emitidos por médico con licencia en SST.",
            "Los exámenes se realizan: al ingreso (antes de iniciar labores), periódicamente según el riesgo del cargo y al momento del retiro. El concepto médico de aptitud es obligatorio.",
            new[]{"Resultados de exámenes y conceptos médicos de aptitud","Listado de trabajadores con examen vigente","Registros de exámenes de retiro practicados"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.1 Condiciones de salud en el trabajo – 9 puntos",
            "3.1.5", 1m,
            "Historias clínicas ocupacionales custodiadas por la IPS o el médico evaluador, con acceso restringido y confidencial durante el tiempo legal.",
            "Las historias son confidenciales y las custodia exclusivamente el médico o la IPS; el empleador solo recibe el concepto de aptitud. Deben conservarse durante la vida laboral más 20 años post retiro.",
            new[]{"Contrato con IPS con cláusula de custodia de historias clínicas","Constancia de custodia por parte de la IPS","Política de confidencialidad médica"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.1 Condiciones de salud en el trabajo – 9 puntos",
            "3.1.6", 1m,
            "Restricciones y recomendaciones médico-laborales de los conceptos de aptitud implementadas por la empresa (reubicación, adaptación, seguimiento).",
            "El empleador está obligado a cumplir las restricciones del médico. Si un trabajador tiene restricciones para trabajo en alturas, carga de peso u otras, deben implementarse con evidencia documentada.",
            new[]{"Conceptos médicos con restricciones recibidos","Acta de seguimiento de recomendaciones médicas","Evidencia de reubicación o adaptación del puesto"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.1 Condiciones de salud en el trabajo – 9 puntos",
            "3.1.7", 1m,
            "Programas de estilos de vida y entornos saludables: actividad física, nutrición, manejo del estrés y bienestar para los trabajadores.",
            "El empleador implementa actividades de promoción de la salud integral. Incluye pausas activas, programas deportivos, talleres de nutrición y manejo del estrés, con registros de ejecución.",
            new[]{"Programa de estilos de vida y entornos saludables","Registros de actividades realizadas (asistencia, fotos)","Informe de ejecución del programa"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.1 Condiciones de salud en el trabajo – 9 puntos",
            "3.1.8", 1m,
            "Centros de trabajo con suministro de agua potable, servicios sanitarios suficientes en buenas condiciones y disposición adecuada de basuras.",
            "Verificar en campo: agua potable disponible, número de baños suficientes por género (Ley 9/79), estado de sanitarios, separación de residuos y recolección periódica.",
            new[]{"Inspección de instalaciones sanitarias (registro y fotos)","Contrato de recolección de residuos sólidos","Análisis de agua potable (cuando aplique)"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.1 Condiciones de salud en el trabajo – 9 puntos",
            "3.1.9", 1m,
            "Disposición adecuada de residuos sólidos, líquidos y gaseosos generados en las actividades, conforme a la normativa ambiental y sanitaria.",
            "El manejo de residuos incluye: clasificación en la fuente, almacenamiento temporal correcto y disposición final con empresa autorizada. Para RESPEL aplica el Decreto 4741/05.",
            new[]{"Programa de gestión de residuos","Registros de disposición (manifiestos)","Contrato con gestor autorizado","Manifiestos de residuos peligrosos (cuando aplique)"}),

        // III · 3.2 Registro, reporte e investigación · 5 puntos
        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.2 Registro, reporte e investigación de AT, incidentes y EL – 5 puntos",
            "3.2.1", 2m,
            "AT reportados a la ARL y EPS dentro de los dos días hábiles siguientes; EL reportada al diagnosticarla; AT graves y mortales reportados a la Dirección Territorial del Ministerio.",
            "El FURAT se envía dentro de los 2 días hábiles siguientes al AT; el FUREL al diagnosticar la EL. La omisión o retraso genera responsabilidad legal. Los AT graves o mortales requieren reporte inmediato a la Dirección Territorial.",
            new[]{"FURAT diligenciado con acuse de recibo de la ARL","FUREL cuando aplique","Evidencia de reporte oportuno a la ARL","Reporte a la Dirección Territorial (AT graves/mortales)"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.2 Registro, reporte e investigación de AT, incidentes y EL – 5 puntos",
            "3.2.2", 2m,
            "AT, incidentes de alto potencial y EL investigados con metodología de análisis de causa raíz, con plan de acciones preventivas y correctivas.",
            "La investigación debe realizarse dentro de los 15 días calendario siguientes al evento, con participación del COPASST y el jefe inmediato, usando metodología estructurada (5 Por Qués, árbol de causas).",
            new[]{"Informes de investigación de AT/incidentes","Análisis de causa raíz documentado","Plan de acciones correctivas derivadas","Informe presentado al COPASST y alta dirección"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.2 Registro, reporte e investigación de AT, incidentes y EL – 5 puntos",
            "3.2.3", 1m,
            "Registro y análisis estadístico de incidentes, AT y EL con datos mensuales y análisis de tendencias para la toma de decisiones.",
            "Las estadísticas permiten identificar tendencias y áreas críticas. Deben presentarse mensualmente al COPASST y a la dirección, con análisis gráfico de la evolución y conclusiones.",
            new[]{"Base de datos de AT, incidentes y EL","Estadísticas mensuales con indicadores y gráficos","Informe mensual al COPASST"}),

        // III · 3.3 Mecanismos de vigilancia · 6 puntos
        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.3 Mecanismos de vigilancia de las condiciones de salud – 6 puntos",
            "3.3.1", 1m,
            "Medición del Índice de Severidad de AT y EL: (Días de incapacidad × 1.000.000) / Total HHT trabajadas.",
            "El IS mide la gravedad promedio de los AT. Se calcula mensualmente, se acumula anualmente y se compara con la meta. Un IS alto indica AT de mayor gravedad.",
            new[]{"Cálculo mensual del Índice de Severidad (IS)","Registro de días de incapacidad por AT/EL","Registro de horas-hombre trabajadas (HHT)"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.3 Mecanismos de vigilancia de las condiciones de salud – 6 puntos",
            "3.3.2", 1m,
            "Medición del Índice de Frecuencia de incidentes y AT: (No. AT × 1.000.000) / Total HHT trabajadas.",
            "El IF mide cuántos AT ocurren por millón de HHT. Se calcula mensualmente y se compara con meta e histórico. Una tendencia al alza indica deterioro del sistema preventivo.",
            new[]{"Cálculo mensual del Índice de Frecuencia (IF)","Registro del número de AT en el período","Comparación con meta e histórico"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.3 Mecanismos de vigilancia de las condiciones de salud – 6 puntos",
            "3.3.3", 1m,
            "Medición y registro del índice de mortalidad por AT y EL: (No. AT mortales × 1.000.000) / No. total de trabajadores.",
            "Aunque se espera cero, el indicador debe calcularse formalmente y presentarse. Evidencia el compromiso con la prevención de fatalidades y el seguimiento sistemático.",
            new[]{"Cálculo del índice de mortalidad","Registro de AT mortales (si aplica)","Tablero de indicadores del SG-SST"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.3 Mecanismos de vigilancia de las condiciones de salud – 6 puntos",
            "3.3.4", 1m,
            "Medición de la prevalencia de la EL: (Casos existentes × 100.000) / No. de trabajadores expuestos al riesgo.",
            "La prevalencia mide la proporción de trabajadores que padecen una EL independientemente de cuándo inició. Debe monitorearse por tipo de riesgo (DME en cargos con carga física).",
            new[]{"Indicador de prevalencia de EL","Diagnósticos de EL confirmados","Seguimiento del programa de vigilancia epidemiológica"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.3 Mecanismos de vigilancia de las condiciones de salud – 6 puntos",
            "3.3.5", 1m,
            "Medición de la incidencia de la EL: (Casos nuevos × 100.000) / No. de trabajadores expuestos en el período.",
            "La incidencia mide casos nuevos de EL en el período. Permite evaluar la eficacia de los controles para los peligros que generan enfermedad laboral.",
            new[]{"Indicador de incidencia de EL","Registro de casos nuevos de EL diagnosticados","Programa de vigilancia epidemiológica activo"}),

        new("hacer","CICLO HACER · 60 puntos","III","III. GESTIÓN DE LA SALUD (20 puntos)",
            "3.3 Mecanismos de vigilancia de las condiciones de salud – 6 puntos",
            "3.3.6", 1m,
            "Medición del ausentismo por AT y EL: tasa de ausentismo con análisis de causas y tendencias mensuales.",
            "El ausentismo por AT y EL impacta la productividad y refleja la efectividad del programa preventivo. Se analiza mensualmente, se compara con la meta y orienta decisiones sobre PYP.",
            new[]{"Registro de ausentismo laboral por AT y EL","Cálculo de la tasa de ausentismo","Análisis de causas","Plan de reducción del ausentismo"}),

        // IV · GESTIÓN DE PELIGROS Y RIESGOS (30 puntos) · 4.1 (15 puntos)
        new("hacer","CICLO HACER · 60 puntos","IV","IV. GESTIÓN DE PELIGROS Y RIESGOS (30 puntos)",
            "4.1 Identificación de peligros, evaluación y valoración de riesgos – 15 puntos",
            "4.1.1", 6m,
            "Metodología documentada, sistemática y actualizada para la IPEVR, coherente con la GTC 45 u otro método reconocido, que cubre condiciones normales, anormales y de emergencia.",
            "La metodología debe estar definida, documentada y aplicada consistentemente. En Colombia se recomienda la GTC 45. Debe contemplar actividades rutinarias, no rutinarias, emergencias y el factor humano.",
            new[]{"Procedimiento de IPEVR con metodología definida","Matriz IPEVR actualizada (GTC 45 u otra)","Criterios de aceptabilidad del riesgo","Registros de actualización periódica de la matriz"}),

        new("hacer","CICLO HACER · 60 puntos","IV","IV. GESTIÓN DE PELIGROS Y RIESGOS (30 puntos)",
            "4.1 Identificación de peligros, evaluación y valoración de riesgos – 15 puntos",
            "4.1.2", 6m,
            "Identificación de peligros realizada con participación de trabajadores de todos los niveles, contratistas, COPASST y grupos especiales (nocturnos, embarazadas).",
            "La participación de los trabajadores es requisito legal (Decreto 1072/15 art. 2.2.4.6.15). Quienes realizan la tarea conocen mejor sus riesgos. Se evidencia con firmas y actas.",
            new[]{"Evidencia de participación en la IPEVR (listas de asistencia)","Actas del COPASST en identificación de peligros","Convocatorias de participación por proceso","Validación de la matriz con trabajadores de campo"}),

        new("hacer","CICLO HACER · 60 puntos","IV","IV. GESTIÓN DE PELIGROS Y RIESGOS (30 puntos)",
            "4.1 Identificación de peligros, evaluación y valoración de riesgos – 15 puntos",
            "4.1.3", 3m,
            "Identificación y priorización de todos los tipos de peligros: físicos, químicos, biológicos, biomecánicos, de seguridad, psicosociales, de tránsito, públicos y fenómenos naturales.",
            "La matriz contempla todos los tipos de peligro según la GTC 45 y el Decreto 1072/15. Para Renergeia aplica especialmente: riesgo eléctrico, trabajo en alturas, exposición a radiación solar y trabajo en exteriores.",
            new[]{"Matriz IPEVR con todos los tipos de peligro clasificados","Priorización de riesgos no aceptables","Plan de intervención para riesgos prioritarios"}),

        // IV · 4.2 Medidas de prevención y control · 15 puntos
        new("hacer","CICLO HACER · 60 puntos","IV","IV. GESTIÓN DE PELIGROS Y RIESGOS (30 puntos)",
            "4.2 Medidas de prevención y control – 15 puntos",
            "4.2.1", 2.5m,
            "Implementación de medidas de prevención y control de peligros siguiendo la jerarquía: eliminación, sustitución, ingeniería, administrativos y EPP.",
            "Los controles se priorizan en la jerarquía. El EPP es el último recurso. Se evidencia en campo con registros de implementación y seguimiento a la eficacia de los controles.",
            new[]{"Plan de intervención de riesgos con jerarquía de controles","Registros de implementación de controles","Seguimiento a eficacia de controles en campo"}),

        new("hacer","CICLO HACER · 60 puntos","IV","IV. GESTIÓN DE PELIGROS Y RIESGOS (30 puntos)",
            "4.2 Medidas de prevención y control – 15 puntos",
            "4.2.2", 2.5m,
            "Verificación en campo de la aplicación efectiva de las medidas de prevención y control mediante inspecciones planeadas y observaciones de comportamiento.",
            "No basta definir controles: deben verificarse en campo. Las inspecciones del COPASST, jefes y responsable del SG-SST generan hallazgos y acciones de mejora documentadas.",
            new[]{"Listas de verificación de controles operacionales","Informes de inspecciones planeadas (con hallazgos)","Observaciones de comportamiento seguro","Seguimiento a hallazgos con cierre"}),

        new("hacer","CICLO HACER · 60 puntos","IV","IV. GESTIÓN DE PELIGROS Y RIESGOS (30 puntos)",
            "4.2 Medidas de prevención y control – 15 puntos",
            "4.2.3", 2.5m,
            "Procedimientos escritos de trabajo seguro (PETS), instructivos, fichas o protocolos para actividades de alto riesgo: alturas, espacios confinados, trabajo eléctrico, manejo de sustancias.",
            "Para cada actividad crítica existe un PETS que define paso a paso la forma segura de realizarla. Incluye el ATS/AST previo a cada jornada. Para Renergeia aplica el protocolo de trabajo eléctrico, alturas y espacios confinados.",
            new[]{"Procedimientos Escritos de Trabajo Seguro (PETS)","Formatos de Análisis de Trabajo Seguro (ATS)","Permisos de trabajo diligenciados","Protocolos específicos: alturas, eléctrico, espacios confinados"}),

        new("hacer","CICLO HACER · 60 puntos","IV","IV. GESTIÓN DE PELIGROS Y RIESGOS (30 puntos)",
            "4.2 Medidas de prevención y control – 15 puntos",
            "4.2.4", 2.5m,
            "El COPASST realiza inspecciones programadas a instalaciones, equipos y condiciones de trabajo con informes y seguimiento a recomendaciones.",
            "Las inspecciones del COPASST son función legal (Decreto 1295/94). Deben ser planeadas, con lista de verificación, producir informes con hallazgos y recomendaciones, y llevar seguimiento.",
            new[]{"Programa de inspecciones del COPASST","Actas de inspección con hallazgos y recomendaciones","Seguimiento a implementación de recomendaciones"}),

        new("hacer","CICLO HACER · 60 puntos","IV","IV. GESTIÓN DE PELIGROS Y RIESGOS (30 puntos)",
            "4.2 Medidas de prevención y control – 15 puntos",
            "4.2.5", 2.5m,
            "Plan de mantenimiento preventivo y correctivo de instalaciones, equipos, máquinas, herramientas y vehículos con ejecución y registros actualizados.",
            "El mantenimiento preventivo es clave para prevenir accidentes por fallas. Debe haber plan con cronograma, responsables y evidencias. Para Renergeia aplica a equipos eléctricos, herramientas de alturas y vehículos.",
            new[]{"Plan de mantenimiento preventivo por equipo","Órdenes de trabajo ejecutadas","Hojas de vida de equipos críticos","Registros de mantenimiento correctivo"}),

        new("hacer","CICLO HACER · 60 puntos","IV","IV. GESTIÓN DE PELIGROS Y RIESGOS (30 puntos)",
            "4.2 Medidas de prevención y control – 15 puntos",
            "4.2.6", 2.5m,
            "EPP entregados formalmente según los riesgos del cargo, con registro firmado, capacitación de uso y verificación con contratistas y subcontratistas.",
            "La entrega de EPP se documenta con firma del trabajador, incluye capacitación sobre uso e inspección previa al uso. Debe gestionarse la reposición oportuna. Contratistas se verifican con los mismos criterios.",
            new[]{"Formato de entrega de EPP firmado por trabajador","Inventario de EPP y programa de reposición","Registros de inspección del estado de EPP","Verificación de EPP a contratistas"}),

        // V · GESTIÓN DE AMENAZAS (10 puntos)
        new("hacer","CICLO HACER · 60 puntos","V","V. GESTIÓN DE AMENAZAS (10 puntos)",
            "5.1 Plan de prevención, preparación y respuesta ante emergencias – 10 puntos",
            "5.1.1", 5m,
            "Plan de Prevención, Preparación y Respuesta ante Emergencias actualizado, con análisis de vulnerabilidad, protocolos, recursos, cadena de llamadas y plan de evacuación.",
            "El plan incluye: análisis de vulnerabilidad (NFPA u otra), amenazas propias del sector (arco eléctrico, caída en alturas, emergencias climáticas), protocolos de respuesta, brigadas, recursos y rutas de evacuación.",
            new[]{"Plan de emergencias actualizado y vigente","Análisis de vulnerabilidad (metodología NFPA u otra)","Mapas de evacuación y puntos de encuentro","Inventario de recursos de emergencia (extintores, botiquines)"}),

        new("hacer","CICLO HACER · 60 puntos","V","V. GESTIÓN DE AMENAZAS (10 puntos)",
            "5.1 Plan de prevención, preparación y respuesta ante emergencias – 10 puntos",
            "5.1.2", 5m,
            "Brigada de emergencias conformada, capacitada y dotada, con roles definidos y simulacros periódicos realizados y evaluados.",
            "La brigada tiene: acta de conformación, roles por tipo de emergencia, capacitación certificada (primeros auxilios, extintores, evacuación), dotación actualizada y simulacros mínimo anuales con informe de evaluación.",
            new[]{"Acta de conformación de la brigada","Plan de capacitación de la brigada","Certificados de formación de brigadistas","Registros de simulacros con evaluación y mejoras"}),

        // ══════════════════════════════════════════════════════
        // CICLO VERIFICAR · 5 puntos
        // ══════════════════════════════════════════════════════
        new("verificar","CICLO VERIFICAR · 5 puntos","VI","VI. VERIFICACIÓN DEL SG-SST (5 puntos)",
            "6.1 Gestión y resultados del SG-SST – 5 puntos",
            "6.1.1", 1.25m,
            "Indicadores de estructura, proceso y resultado del SG-SST definidos, calculados y analizados con meta, línea base y seguimiento periódico.",
            "Estructura: recursos asignados, afiliaciones. Proceso: actividades del plan ejecutadas. Resultado: IF, IS, mortalidad, prevalencia, incidencia y ausentismo. Todos con meta, responsable y frecuencia.",
            new[]{"Ficha técnica de indicadores del SG-SST","Tablero de indicadores actualizado","Informe de resultados al COPASST y dirección"}),

        new("verificar","CICLO VERIFICAR · 5 puntos","VI","VI. VERIFICACIÓN DEL SG-SST (5 puntos)",
            "6.1 Gestión y resultados del SG-SST – 5 puntos",
            "6.1.2", 1.25m,
            "Auditoría interna al SG-SST realizada por lo menos una vez al año, con participación del COPASST, plan documentado e informe de hallazgos.",
            "La auditoría verifica el cumplimiento de los estándares mínimos. Puede realizarla personal interno con conocimiento en SST o externo. El COPASST participa activamente.",
            new[]{"Programa anual de auditorías al SG-SST","Plan de auditoría con alcance y criterios","Informe de auditoría con hallazgos","Evidencia de participación del COPASST"}),

        new("verificar","CICLO VERIFICAR · 5 puntos","VI","VI. VERIFICACIÓN DEL SG-SST (5 puntos)",
            "6.1 Gestión y resultados del SG-SST – 5 puntos",
            "6.1.3", 1.25m,
            "La alta dirección realiza revisión anual del SG-SST evaluando auditoría, objetivos, indicadores, AT/EL y plan de trabajo, con compromisos documentados.",
            "La revisión debe ser formal, en acta, e incluir todos los elementos del SG-SST. Genera compromisos, asignación de recursos y actualización del plan para el siguiente período.",
            new[]{"Acta de revisión por la alta dirección","Informe de resultados presentado","Compromisos y acciones generadas","Seguimiento a compromisos de revisiones anteriores"}),

        new("verificar","CICLO VERIFICAR · 5 puntos","VI","VI. VERIFICACIÓN DEL SG-SST (5 puntos)",
            "6.1 Gestión y resultados del SG-SST – 5 puntos",
            "6.1.4", 1.25m,
            "Planificación de auditorías realizada con participación del COPASST, definiendo criterios, alcance, metodología y periodicidad.",
            "El COPASST tiene función legal de participar en la planificación y ejecución de auditorías al SG-SST (Decreto 1295/94 art. 63). La agenda se acuerda con ellos y se documenta.",
            new[]{"Plan de auditorías elaborado con el COPASST","Acta de planificación de auditoría","Criterios, alcance y metodología definidos"}),

        // ══════════════════════════════════════════════════════
        // CICLO ACTUAR · 10 puntos
        // ══════════════════════════════════════════════════════
        new("actuar","CICLO ACTUAR · 10 puntos","VII","VII. MEJORAMIENTO (10 puntos)",
            "7.1 Acciones preventivas y correctivas basadas en resultados del SG-SST – 10 puntos",
            "7.1.1", 2.5m,
            "Acciones de mejora continua en PYP definidas con base en los resultados del SG-SST, la autoevaluación de estándares mínimos y el cumplimiento del plan de trabajo.",
            "Las acciones de mejora se derivan del análisis de resultados del SG-SST (indicadores, auditorías, ejecución del plan) y se orientan a mejorar el desempeño preventivo y de salud.",
            new[]{"Plan de acción de mejora continua","Análisis de resultados con acciones derivadas","Seguimiento de la autoevaluación de estándares","Acta de definición de acciones con responsables y plazos"}),

        new("actuar","CICLO ACTUAR · 10 puntos","VII","VII. MEJORAMIENTO (10 puntos)",
            "7.1 Acciones preventivas y correctivas basadas en resultados del SG-SST – 10 puntos",
            "7.1.2", 2.5m,
            "Medidas correctivas, preventivas y de mejora formuladas y ejecutadas con base en resultados del SG-SST, con responsables, plazos y verificación de eficacia.",
            "Las acciones se formulan, se ejecutan y se verifica su eficacia cerrando el ciclo PHVA: Planear (causa), Hacer (implementar), Verificar (eficacia) y Actuar (estandarizar).",
            new[]{"Registro de acciones correctivas y preventivas","Seguimiento de eficacia","Cierre documentado de acciones verificadas"}),

        new("actuar","CICLO ACTUAR · 10 puntos","VII","VII. MEJORAMIENTO (10 puntos)",
            "7.1 Acciones preventivas y correctivas basadas en resultados del SG-SST – 10 puntos",
            "7.1.3", 2.5m,
            "Acciones preventivas y correctivas derivadas de investigaciones de AT, incidentes y EL ejecutadas, con seguimiento e implementación verificada.",
            "Cada investigación genera acciones que atacan causas básicas. El seguimiento verifica que las acciones eliminaron o redujeron la probabilidad de recurrencia y se evidencia en registros.",
            new[]{"Plan de acciones de investigaciones de AT e incidentes","Seguimiento de implementación","Evidencia de cierre y verificación de eficacia"}),

        new("actuar","CICLO ACTUAR · 10 puntos","VII","VII. MEJORAMIENTO (10 puntos)",
            "7.1 Acciones preventivas y correctivas basadas en resultados del SG-SST – 10 puntos",
            "7.1.4", 2.5m,
            "Medidas y acciones correctivas ordenadas por autoridades (Ministerio de Trabajo, ARL, entidades de inspección) implementadas dentro de los plazos establecidos.",
            "Los requerimientos de autoridades (actas de visita del Ministerio, recomendaciones de la ARL) deben atenderse con plan de respuesta formal y evidencia de implementación dentro de los plazos.",
            new[]{"Comunicaciones y recomendaciones de autoridades y ARL","Plan de respuesta a requerimientos oficiales","Evidencia de implementación de medidas ordenadas"}),
    };

    private static readonly Dictionary<string, RequisitoRes0312> _porId =
        Requisitos.ToDictionary(r => r.Id);

    public static RequisitoRes0312? GetById(string id) =>
        _porId.TryGetValue(id, out var r) ? r : null;

    public static decimal GetPeso(string id) => GetById(id)?.Peso ?? 0m;

    public static string GetPhvaKey(string numGrupo) => numGrupo switch
    {
        "I" or "II"         => "planear",
        "III" or "IV" or "V" => "hacer",
        "VI"                => "verificar",
        "VII"               => "actuar",
        _                   => "planear"
    };
}
