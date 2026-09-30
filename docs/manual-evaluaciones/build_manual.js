// Genera el Manual de Usuario de Evaluaciones HSE dentro del formato de la plantilla IN-SG-SS-007.
const fs = require("fs");
const path = require("path");
const sharp = require("C:/Users/Luisa Becerra/Downloads/RenergeIA/docs/presentacion-evaluaciones/node_modules/sharp");
const D = require("docx");
const {
  Document, Packer, Paragraph, TextRun, ImageRun, Table, TableRow, TableCell,
  WidthType, BorderStyle, AlignmentType, HeadingLevel, TableOfContents, PageBreak,
  Header, Footer, PageNumber, VerticalAlign, ShadingType, LevelFormat, TabStopType, TabStopPosition,
} = D;

const BASE = "C:/Users/Luisa Becerra/Downloads/RenergeIA/docs/manual-evaluaciones";
const IMG = path.join(BASE, "img", "final");
const LOGO = "C:/Users/Luisa Becerra/Downloads/RenergeIA/evaluacion-hse/public/logo-renergeia.png";
const OUT = path.join(BASE, "IN-SG-SS-007-1.docx");

const FONT = "Calibri";
const BLUE = "1F3A5F";
const BLACK = "000000";
const GREEN = "6ABF4B";
const GRAY = "595959";
const HEADW = "2E4C74"; // azul encabezado

const CONTENT_W_PX = 540, MAXH_PX = 320;

async function dims(file) {
  const m = await sharp(path.join(IMG, file)).metadata();
  return { w: m.width, h: m.height };
}

(async () => {
  // Precalcular dimensiones de imagenes
  const files = fs.readdirSync(IMG).filter(f => /\.(jpg|png)$/i.test(f));
  const dim = {};
  for (const f of files) dim[f] = await dims(f);
  const logoMeta = await sharp(LOGO).metadata();

  // ---------- Helpers ----------
  const run = (text, opts = {}) => new TextRun({ text, font: FONT, size: opts.size || 22, bold: !!opts.bold, italics: !!opts.italics, color: opts.color || BLACK });
  const body = (text, opts = {}) => new Paragraph({
    alignment: AlignmentType.JUSTIFIED,
    spacing: { line: 259, lineRule: "auto", after: opts.after == null ? 120 : opts.after },
    keepNext: opts.keepNext || (typeof text === "string" && text.trim().endsWith(":")),
    children: Array.isArray(text) ? text : [run(text, opts)],
  });
  const h1 = (text) => new Paragraph({ heading: HeadingLevel.HEADING_1, spacing: { before: 240, after: 120 }, children: [run(text, { bold: true, color: BLUE, size: 30 })] });
  const h2 = (text) => new Paragraph({ heading: HeadingLevel.HEADING_2, spacing: { before: 180, after: 100 }, children: [run(text, { bold: true, color: BLACK, size: 24 })] });
  const bullet = (text) => new Paragraph({ numbering: { reference: "bul", level: 0 }, alignment: AlignmentType.JUSTIFIED, spacing: { line: 259, lineRule: "auto", after: 60 }, children: [run(text)] });
  const step = (ref, text) => new Paragraph({ numbering: { reference: ref, level: 0 }, alignment: AlignmentType.JUSTIFIED, spacing: { line: 259, lineRule: "auto", after: 60 }, children: [run(text)] });
  const note = (label, text) => body([run(label, { bold: true }), run(" " + text)]);

  function image(file, opts = {}) {
    const d = dim[file];
    const scale = Math.min(CONTENT_W_PX / d.w, MAXH_PX / d.h, 1) * (opts.scale || 1);
    const wPx = Math.round(d.w * scale), hPx = Math.round(d.h * scale);
    const type = file.toLowerCase().endsWith(".png") ? "png" : "jpg";
    return new Paragraph({
      alignment: AlignmentType.CENTER,
      spacing: { before: 80, after: 140 },
      children: [new ImageRun({ type, data: fs.readFileSync(path.join(IMG, file)), transformation: { width: wPx, height: hPx } })],
    });
  }

  // Bordes finos para tablas
  const bd = { style: BorderStyle.SINGLE, size: 4, color: "BFBFBF" };
  const cellBorders = { top: bd, bottom: bd, left: bd, right: bd };
  function tcell(children, opts = {}) {
    return new TableCell({
      width: { size: opts.w || 50, type: WidthType.PERCENTAGE },
      borders: cellBorders,
      shading: opts.shad ? { type: ShadingType.CLEAR, fill: opts.shad } : undefined,
      verticalAlign: VerticalAlign.CENTER,
      margins: { top: 40, bottom: 40, left: 90, right: 90 },
      children: Array.isArray(children) ? children : [children],
    });
  }
  const cellP = (text, opts = {}) => new Paragraph({ spacing: { after: 0, line: 259, lineRule: "auto" }, alignment: opts.align || AlignmentType.LEFT, children: [run(text, opts)] });

  // Tabla 2 columnas (accesos, datos)
  function table2(rows, headerRow, colW = [38, 62]) {
    const trs = [];
    if (headerRow) {
      trs.push(new TableRow({ tableHeader: true, children: [
        tcell(cellP(headerRow[0], { bold: true, color: "FFFFFF", align: AlignmentType.CENTER }), { w: colW[0], shad: BLUE }),
        tcell(cellP(headerRow[1], { bold: true, color: "FFFFFF", align: AlignmentType.CENTER }), { w: colW[1], shad: BLUE }),
      ] }));
    }
    for (const r of rows) {
      trs.push(new TableRow({ children: [
        tcell(cellP(r[0], { bold: r[2] || false }), { w: colW[0] }),
        tcell(cellP(r[1]), { w: colW[1] }),
      ] }));
    }
    return new Table({ width: { size: 100, type: WidthType.PERCENTAGE }, columnWidths: [colW[0] * 100, colW[1] * 100], rows: trs });
  }

  // ---------- Encabezado (tabla logo | titulo | codigo) ----------
  const logoScale = 0.42;
  const header = new Header({
    children: [
      new Table({
        width: { size: 100, type: WidthType.PERCENTAGE },
        columnWidths: [3100, 4300, 2900],
        rows: [
          new TableRow({ children: [
            new TableCell({ width: { size: 30, type: WidthType.PERCENTAGE }, borders: cellBorders, verticalAlign: VerticalAlign.CENTER, margins: { top: 40, bottom: 40, left: 90, right: 90 }, children: [
              new Paragraph({ alignment: AlignmentType.CENTER, children: [new ImageRun({ type: "png", data: fs.readFileSync(LOGO), transformation: { width: Math.round(logoMeta.width * logoScale), height: Math.round(logoMeta.height * logoScale) } })] }),
            ] }),
            new TableCell({ width: { size: 42, type: WidthType.PERCENTAGE }, borders: cellBorders, verticalAlign: VerticalAlign.CENTER, children: [
              new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 0 }, children: [run("Instructivo de Plataforma", { bold: true, color: HEADW })] }),
              new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 0 }, children: [run("de Capacitación,", { bold: true, color: HEADW })] }),
              new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 0 }, children: [run("Evaluación y Asistencia", { bold: true, color: HEADW })] }),
            ] }),
            new TableCell({ width: { size: 28, type: WidthType.PERCENTAGE }, borders: cellBorders, children: [new Paragraph({ text: "" })] }),
          ] }),
          new TableRow({ children: [
            tcell(cellP("HSE", { bold: true, color: HEADW, align: AlignmentType.CENTER }), { w: 30 }),
            tcell(cellP("GENERAL", { bold: true, color: HEADW, align: AlignmentType.CENTER }), { w: 42 }),
            tcell(cellP("IN-SG-SS-007-A", { bold: true, color: HEADW, align: AlignmentType.CENTER }), { w: 28 }),
          ] }),
        ],
      }),
      new Paragraph({ text: "", spacing: { after: 60 } }),
    ],
  });

  // ---------- Pie (linea verde + codigo + pagina) ----------
  const footer = new Footer({
    children: [
      new Paragraph({ spacing: { after: 0 }, border: { top: { style: BorderStyle.SINGLE, size: 12, color: GREEN, space: 4 } }, children: [] }),
      new Paragraph({
        tabStops: [{ type: TabStopType.RIGHT, position: TabStopPosition.MAX }],
        spacing: { before: 40, after: 0 },
        children: [
          run("Código de plantilla: FO-PP-CD-003-0", { size: 16, color: GRAY }),
          new TextRun({ text: "\t", font: FONT }),
          new TextRun({ text: "Página ", font: FONT, size: 16, color: GRAY }),
          new TextRun({ children: [PageNumber.CURRENT], font: FONT, size: 16, color: GRAY }),
          new TextRun({ text: " de ", font: FONT, size: 16, color: GRAY }),
          new TextRun({ children: [PageNumber.TOTAL_PAGES], font: FONT, size: 16, color: GRAY }),
        ],
      }),
      new Paragraph({ spacing: { after: 0 }, children: [run("Fecha de plantilla: 2025/03/24", { size: 16, color: GRAY })] }),
    ],
  });

  // ---------- Portada ----------
  const cover = [
    new Paragraph({ text: "", spacing: { after: 1400 } }),
    new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 0 }, children: [run("INSTRUCTIVO DE CAPACITACIÓN,", { bold: true, size: 40, color: BLACK })] }),
    new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 0 }, children: [run("EVALUACIÓN Y ASISTENCIA", { bold: true, size: 40, color: BLACK })] }),
    new Paragraph({ text: "", spacing: { after: 2600 } }),
    new Table({
      width: { size: 100, type: WidthType.PERCENTAGE },
      columnWidths: [1500, 1700, 2600, 1400, 1400, 1400],
      rows: [
        new TableRow({ tableHeader: true, children: ["Revisión", "Fecha", "Descripción", "Elaboró", "Revisó", "Aprobó"].map((t, i) =>
          tcell(cellP(t, { bold: true, align: AlignmentType.CENTER }), { w: [15, 17, 26, 14, 14, 14][i] })) }),
        new TableRow({ children: [
          tcell(cellP("A", { align: AlignmentType.CENTER }), { w: 15 }),
          tcell(cellP("2026/07/08", { align: AlignmentType.CENTER }), { w: 17 }),
          tcell(cellP("Emisión Inicial", { align: AlignmentType.CENTER }), { w: 26 }),
          tcell(cellP("LB", { align: AlignmentType.CENTER }), { w: 14 }),
          tcell(cellP("TP/MM", { align: AlignmentType.CENTER }), { w: 14 }),
          tcell(cellP("MB", { align: AlignmentType.CENTER }), { w: 14 }),
        ] }),
      ],
    }),
    new Paragraph({ children: [new PageBreak()] }),
  ];

  // ---------- Índice ----------
  const toc = [
    new Paragraph({ alignment: AlignmentType.CENTER, spacing: { after: 160 }, children: [run("INDICE", { bold: true, size: 24 })] }),
    new TableOfContents("Tabla de contenido", { hyperlink: true, headingStyleRange: "1-2" }),
    new Paragraph({ text: "", spacing: { after: 200 } }),
    body("La información descrita en el presente documento es de uso reservado y exclusivo de El Grupo Renergeia. Está prohibida su reproducción sin previa autorización o su utilización en otros fines distintos para el cual fue entregada."),
    new Paragraph({ children: [new PageBreak()] }),
  ];

  // ---------- Contenido ----------
  const c = [];
  // 1 OBJETIVO
  c.push(h1("1\tOBJETIVO"));
  c.push(body("El Sistema de Evaluaciones de Renergeia es una plataforma web desarrollada para digitalizar y optimizar los procesos de capacitación en Seguridad, Salud en el Trabajo y Ambiental y Calidad (HSEQ)."));
  c.push(body("Permite evaluar los conocimientos adquiridos por los colaboradores, registrar la asistencia a capacitaciones y generar certificados de aprobación de forma automática."));
  c.push(h2("1.1\tOBJETIVOS PRINCIPALES"));
  ["Evaluar los conocimientos de los colaboradores posterior a cada capacitación.",
   "Registrar la asistencia de participantes a las capacitaciones HSE.",
   "Generar certificados de aprobación automáticos en formato PDF.",
   "Centralizar la información de resultados, estadísticas y trazabilidad.",
   "Facilitar la gestión administrativa y el seguimiento del plan de capacitación."].forEach(t => c.push(bullet(t)));

  // 2 ALCANCE
  c.push(h1("2\tALCANCE"));
  c.push(body("El presente documento aplica a todos los colaboradores, contratistas y personal vinculado a Renergeia S.A.S. que participen en las capacitaciones de formación en Seguridad, Salud en el Trabajo, Ambiental y Calidad (HSEQ)."));
  c.push(body("Comprende el uso de la plataforma digital para:"));
  ["Presentación de evaluaciones de conocimiento posteriores a cada capacitación.",
   "Registro digital de asistencia a jornadas de capacitación.",
   "Generación y descarga de certificados de aprobación.",
   "Consulta de resultados y seguimiento de indicadores por parte del área administrativa."].forEach(t => c.push(bullet(t)));

  // 3 ACCESO
  c.push(h1("3\tACCESO A LA PLATAFORMA"));
  c.push(body("La plataforma cuenta con tres puntos de acceso principales. A continuación, se presentan los enlaces para cada uno:"));
  c.push(table2([
    ["Portal de Evaluaciones (para los participantes)", "https://evaluacion-hse-64204106653.us-central1.run.app/", true],
    ["Registro de Asistencia (para los participantes)", "https://evaluacion-hse-64204106653.us-central1.run.app/asistencia", true],
    ["Panel de Administración (acceso restringido)", "https://evaluacion-hse-64204106653.us-central1.run.app/admin/login", true],
  ], null, [40, 60]));
  c.push(new Paragraph({ text: "", spacing: { after: 80 } }));
  c.push(note("Nota:", "El panel de administración requiere credenciales de acceso. Solicite sus credenciales al área de HSEQ para poder gestionar evaluaciones, ver resultados y administrar el sistema."));
  c.push(body("Vista del portal principal:"));
  c.push(image("p01_portal.jpg"));

  // 4 MODULO EVALUACIONES
  c.push(h1("4\tMÓDULO DE EVALUACIONES – ¿CÓMO PRESENTAR UNA EVALUACIÓN?"));
  c.push(body("El módulo de evaluaciones permite a los participantes presentar las pruebas de conocimiento asociadas a cada capacitación. El proceso es sencillo e intuitivo:"));
  c.push(h2("4.1\tPASO A PASO PARA EL PARTICIPANTE"));
  ["Ingrese al enlace del portal de evaluaciones proporcionado por el área HSE.",
   "En la pantalla principal verá las evaluaciones activas disponibles. Seleccione la capacitación que va a presentar haciendo clic en “Presentar evaluación”.",
   "Complete sus datos personales: nombre, apellido, número de cédula, cargo, departamento, proyecto y correo electrónico.",
   "Responda todas las preguntas de conocimiento seleccionando la opción que considere correcta.",
   "Diligencie la sección de retroalimentación del capacitador (si aplica).",
   "Registre su firma digital en el espacio habilitado.",
   "Haga clic en “Enviar evaluación”. El sistema le mostrará su calificación de inmediato.",
   "Si aprueba (nota igual o superior a 3.0/5.0), podrá descargar su certificado de aprobación en PDF directamente desde la pantalla de resultado."].forEach(t => c.push(step("s41", t)));
  c.push(body("Formulario de datos y preguntas:", { after: 40 }));
  c.push(image("p02_datos.jpg"));
  c.push(image("p03_preguntas.jpg"));
  c.push(body("Firma del participante y envío:", { after: 40 }));
  c.push(image("p04_firma.jpg"));
  c.push(body("Resultado con la calificación (aprobado) y descarga del certificado:", { after: 40 }));
  c.push(image("p05_aprobado.jpg"));
  c.push(note("Importante:", "La nota mínima de aprobación es 3.0 sobre 5.0. Si el participante no aprueba, podrá presentar nuevamente la evaluación dentro de 24 horas. El certificado de aprobación se genera automáticamente con los datos del participante, nombre de la capacitación, fecha y firma institucional."));
  c.push(body("Ejemplo de reprobado:", { after: 40 }));
  c.push(image("p07_reprobado.jpg"));
  c.push(body("Ejemplo de certificado de aprobación:", { after: 40 }));
  c.push(image("p06_certificado.png"));

  // 4.2 DATOS
  c.push(h2("4.2\tDATOS QUE SE CAPTURAN EN LA EVALUACIÓN"));
  c.push(table2([
    ["Nombre y Apellido", "Nombre completo del participante"],
    ["Cédula", "Número de identificación"],
    ["Cargo", "Cargo actual del colaborador"],
    ["Departamento", "Área o departamento asignado"],
    ["Proyecto", "Proyecto en el que se encuentra"],
    ["Correo electrónico", "Para trazabilidad y comunicación"],
    ["Firma digital", "Firma del participante en canvas"],
    ["Respuestas", "Respuestas a preguntas de conocimiento"],
    ["Retroalimentación", "Evaluación del capacitador (escala)"],
  ], ["Campo", "Descripción"], [35, 65]));

  // 5 ASISTENCIA
  c.push(h1("5\tMÓDULO DE ASISTENCIA – REGISTRO DE ASISTENCIA"));
  c.push(body("El módulo de asistencia permite registrar la participación de los colaboradores en las capacitaciones, independientemente de si presentan evaluación o no. Este registro es fundamental para la trazabilidad del plan de capacitación anual."));
  c.push(h2("5.1\tPASO A PASO PARA REGISTRAR LA ASISTENCIA"));
  ["Ingrese al enlace de registro de asistencia proporcionado por el área HSE, o desde el portal principal haga clic en “Registrar asistencia”.",
   "Complete sus datos personales: nombre, apellido, número de identificación, cargo, departamento, proyecto y correo electrónico.",
   "Seleccione la capacitación a la que asiste del menú desplegable. Si la capacitación no aparece en la lista, seleccione “Otro” y escriba el nombre.",
   "Registre su firma digital en el espacio habilitado.",
   "Haga clic en “Registrar asistencia” para enviar el formulario."].forEach(t => c.push(step("s51", t)));
  c.push(image("p08_asistencia.jpg"));
  c.push(note("Nota:", "El registro de asistencia incluye firma digital del participante y queda almacenado en la base de datos para consulta posterior por parte del área administrativa."));

  // 6 PANEL ADMINISTRATIVO
  c.push(h1("6\tPANEL ADMINISTRATIVO – GESTIÓN Y RESULTADOS"));
  c.push(body("El panel de administración es el centro de control del sistema. Desde allí, el área de HSE puede gestionar las evaluaciones, revisar resultados, consultar asistencias y acceder a estadísticas. El acceso requiere credenciales autorizadas."));
  c.push(body("Ingreso al panel (usuario y contraseña):", { after: 40 }));
  c.push(image("a01_login.jpg"));
  c.push(body("Módulos del panel administrativo (barra de pestañas):", { after: 40 }));
  c.push(image("a02_resultados.jpg"));

  c.push(h2("6.1\tRESULTADOS"));
  c.push(body("Consulte todos los resultados de evaluaciones presentadas. Incluye filtros por evaluación, estado (aprobado/reprobado), departamento y búsqueda. Puede ver el detalle de cada intento, descargar certificados, exportar datos a Excel (CSV) y gestionar registros eliminados en la papelera."));
  c.push(image("a03_resultados_estado.jpg"));

  c.push(h2("6.2\tEVALUACIONES"));
  c.push(body("Gestione las evaluaciones activas: cree nuevas evaluaciones con preguntas de selección múltiple, configure la nota mínima de aprobación, active o desactive evaluaciones, y copie enlaces para compartir con los participantes."));
  c.push(image("a05_evaluaciones.jpg"));

  c.push(h2("6.3\tASISTENCIA"));
  c.push(body("Consulte el registro de asistencia con filtros por capacitación, departamento y búsqueda. Incluye tarjetones con totales por capacitación, opción de eliminar registros y acceso a la papelera para restaurar o eliminar permanentemente."));
  c.push(image("a09_asistencia.jpg"));

  // 6.4 CREAR EVALUACION
  c.push(h2("6.4\tCREACIÓN DE UNA EVALUACIÓN (PASO A PASO)"));
  c.push(body("Para crear una nueva evaluación o inducción, ingrese a la pestaña Evaluaciones y siga estos pasos:"));
  ["Haga clic en el botón “+ Nueva evaluación”. El sistema creará la evaluación y abrirá el editor.",
   "En “Datos generales”, escriba el Título de la evaluación y el Tema (este último aparece en el certificado). Seleccione el Área responsable (HSE o Recursos Humanos).",
   "En “Preguntas de conocimiento”, escriba el enunciado de cada pregunta y sus opciones de respuesta. Utilice “+ Agregar opción” y “+ Agregar pregunta” según lo necesite.",
   "Marque la respuesta correcta de cada pregunta con el círculo verde ubicado a la izquierda de la opción.",
   "Active la casilla “Evaluación activa” para que la evaluación sea visible a los participantes.",
   "Haga clic en “Guardar cambios”. La plataforma calificará automáticamente cada intento con base en las respuestas marcadas como correctas."].forEach(t => c.push(step("s64", t)));
  c.push(image("a07_editor.jpg"));
  c.push(image("a08_pregunta.jpg"));
  c.push(note("Nota:", "La nota mínima de aprobación es 3.0 sobre 5.0. La sección de retroalimentación del capacitador es fija del formato y no afecta la calificación."));

  // 6.5 ENLACES
  c.push(h2("6.5\tCOPIAR Y COMPARTIR LOS ENLACES"));
  c.push(body("Desde la pestaña Evaluaciones puede obtener los enlaces para compartir con los participantes:"));
  ["En la columna “Links” de cada evaluación, haga clic en “Copiar link” para copiar el enlace de esa evaluación.",
   "Para el enlace general de registro de asistencia, haga clic en el botón “Copiar link de asistencia”.",
   "Comparta el enlace por WhatsApp, correo electrónico o mediante un código QR. El participante podrá ingresar desde su computador o celular."].forEach(t => c.push(step("s65", t)));
  c.push(image("a06_links.jpg"));

  // 6.6 REVISAR Y CORREGIR
  c.push(h2("6.6\tREVISAR EL DETALLE DE UN INTENTO Y CORREGIR"));
  c.push(body("En la pestaña Resultados puede revisar en detalle cada evaluación presentada y verificar la calificación:"));
  ["Ubique a la persona en la tabla y haga clic en el botón “Ver” de esa fila.",
   "Se abrirá el detalle con la nota obtenida y, pregunta por pregunta, la respuesta que seleccionó y cuál era la correcta (las respuestas incorrectas se muestran en rojo).",
   "Revise también la retroalimentación del capacitador y la firma del participante.",
   "Si detecta que una pregunta o su respuesta correcta quedó mal configurada, corríjala en la pestaña Evaluaciones (botón “Editar”) y guarde los cambios."].forEach(t => c.push(step("s66", t)));
  c.push(image("a04_detalle.jpg"));

  // 6.7 PERSONAL Y MATRIZ
  c.push(h2("6.7\tPLANTA DE PERSONAL Y MATRIZ DE ASISTENCIA"));
  c.push(body("La pestaña Personal permite gestionar la planta de personal y consultar, en una sola tabla, quién ha asistido a cada capacitación:"));
  ["Cargue la planta de personal con el botón “Importar desde Excel”, o agregue personas manualmente con “+ Agregar persona”.",
   "La matriz muestra una columna por cada capacitación: un ✓ verde indica que la persona asistió y un ✗ rojo que no asistió. El cruce se realiza por número de cédula, tomando tanto la asistencia registrada como la evaluación presentada.",
   "La columna “% Asist.” indica el porcentaje de asistencia de cada persona según las capacitaciones mostradas.",
   "Con el botón “Actualizar” refresca la información y con “Exportar a Excel” descarga la matriz completa."].forEach(t => c.push(step("s67", t)));
  c.push(image("a10_personal.jpg"));
  c.push(image("a11_matriz.jpg"));

  // 6.8 FILTROS
  c.push(h2("6.8\tBÚSQUEDA Y FILTROS"));
  c.push(body("Todas las tablas del panel cuentan con búsqueda y filtros para encontrar la información rápidamente:"));
  ["Utilice el campo “Buscar” para encontrar a una persona por nombre, cédula o correo.",
   "Filtre por evaluación, estado (aprobado/reprobado), departamento, área, cargo o proyecto, según la pestaña.",
   "En la pestaña Personal, use los botones ✓ y ✗ del encabezado de cada capacitación para ver únicamente quién asistió o quién faltó a esa capacitación."].forEach(t => c.push(step("s68", t)));

  // 6.9 USUARIOS
  c.push(h2("6.9\tUSUARIOS Y ROLES"));
  c.push(body("La gestión de usuarios está disponible únicamente para el rol administrador, en la pestaña Usuarios:"));
  ["Cree cuentas nuevas con el botón “Crear cuenta”, indicando el usuario, el rol y el área.",
   "El rol Administrador ve y gestiona todas las áreas; un usuario de área (HSE o Recursos Humanos) solo ve y gestiona lo correspondiente a su área.",
   "Para cambiar una contraseña, utilice “Resetear contraseña” (administrador) o la opción “Mi cuenta” (cada usuario)."].forEach(t => c.push(step("s69", t)));
  c.push(image("a13_usuarios.jpg"));

  // 7 DASHBOARD
  c.push(h1("7\tDASHBOARD Y GRÁFICOS – INDICADORES Y ESTADÍSTICAS"));
  c.push(body("El módulo de Dashboard ofrece una vista consolidada de toda la información del sistema mediante tarjetones de resumen y gráficos de barras. Esta información permite tomar decisiones basadas en datos reales sobre el plan de capacitación."));
  c.push(h2("7.1\tTARJETONES DE RESUMEN (KPIS)"));
  ["Total de evaluaciones presentadas",
   "Cantidad de aprobados y reprobados",
   "Tasa de aprobación (porcentaje)",
   "Total de registros de asistencia"].forEach(t => c.push(bullet(t)));
  c.push(image("a12_dashboard.jpg"));
  c.push(h2("7.2\tGRÁFICOS DISPONIBLES"));
  c.push(body("Los datos del dashboard se actualizan en tiempo real. Cuando se eliminan registros (enviados a papelera), las estadísticas y gráficos se recalculan automáticamente, garantizando que la información siempre refleje el estado actual."));
  ["Resultados por capacitación: muestra aprobados y reprobados por cada evaluación.",
   "Evaluaciones por departamento: distribución de participaciones por área.",
   "Horas de capacitación por departamento: cálculo estimado de horas invertidas.",
   "Asistencia por capacitación: cantidad de asistentes registrados por evento."].forEach(t => c.push(bullet(t)));

  // 8 EVALUACIONES DISPONIBLES
  c.push(h1("8\tEVALUACIONES DISPONIBLES ACTUALMENTE"));
  c.push(body("Actualmente se encuentran creadas y activas las siguientes evaluaciones en la plataforma:"));
  c.push(image("a05_evaluaciones.jpg"));
  c.push(body("Se pueden crear nuevas evaluaciones en cualquier momento desde el panel administrativo, según las necesidades del plan de capacitación anual."));

  // 9 FUNCIONALIDADES ADICIONALES
  c.push(h1("9\tFUNCIONALIDADES ADICIONALES"));
  c.push(h2("9.1\tSISTEMA DE PAPELERA (ELIMINACIÓN SEGURA)"));
  c.push(body("El sistema cuenta con un mecanismo de eliminación segura. Cuando se elimina un registro (resultado o asistencia), este no se borra permanentemente, sino que se mueve a una papelera. Desde allí se puede restaurar o eliminar de forma definitiva. Las estadísticas y gráficos se actualizan automáticamente al realizar estas acciones."));
  c.push(h2("9.2\tCERTIFICADO DE APROBACIÓN"));
  c.push(body("Los participantes que aprueban la evaluación pueden descargar un certificado en formato PDF con su nombre, número de cédula, nombre de la capacitación, fecha y firmas institucionales. Este certificado también puede ser descargado por el administrador desde el panel de resultados."));
  c.push(image("p06_certificado.png"));
  c.push(h2("9.3\tEXPORTACIÓN DE DATOS"));
  c.push(body("Desde el módulo de resultados se pueden exportar los datos a formato CSV (compatible con Excel), permitiendo análisis adicionales, presentaciones de informes y auditorías. Las pestañas Asistencia y Personal también cuentan con su propio botón “Exportar a Excel”."));
  c.push(h2("9.4\tCOMPARTIR ENLACES"));
  c.push(body("Desde el módulo de evaluaciones del panel administrativo, se pueden copiar los enlaces tanto de las evaluaciones como del registro de asistencia para compartirlos fácilmente con los participantes vía correo electrónico, WhatsApp u otros medios."));

  // 10 BUEN USO
  c.push(h1("10\tBUEN USO DE LA PLATAFORMA"));
  c.push(body("Para garantizar la confiabilidad y la trazabilidad de la información, tenga en cuenta las siguientes recomendaciones sobre el uso correcto de la plataforma."));
  c.push(h2("10.1\tRECOMENDACIONES (QUÉ SÍ HACER)"));
  ["Ingrese siempre sus datos reales y completos (nombre, cédula, cargo, departamento y proyecto) al presentar una evaluación o registrar asistencia.",
   "Presente usted mismo su evaluación; la firma digital es personal e intransferible.",
   "Utilice títulos claros y reconocibles para cada capacitación; recuerde que el tema aparece en el certificado.",
   "Comparta el enlace de asistencia al iniciar la capacitación y el de la evaluación al finalizar.",
   "Mantenga actualizada la planta de personal para que la matriz de asistencia cruce correctamente.",
   "Descargue y respalde periódicamente los resultados y la asistencia para sus informes.",
   "Verifique que las evaluaciones queden en estado “Activa” antes de compartirlas."].forEach(t => c.push(bullet(t)));
  c.push(h2("10.2\tQUÉ NO SE DEBE HACER"));
  ["No comparta las credenciales del panel de administración con personas no autorizadas.",
   "No presente evaluaciones a nombre de otra persona ni firme por terceros.",
   "No registre datos falsos o incompletos, pues afecta la trazabilidad y los indicadores.",
   "No elimine registros de resultados o asistencia sin autorización; utilice la papelera y verifique antes de eliminar de forma permanente.",
   "No cree evaluaciones de prueba en el entorno real; si lo hace, elimínelas al terminar.",
   "No modifique las preguntas ni las respuestas correctas de una evaluación mientras haya personas presentándola.",
   "No comparta información personal de los participantes (cédulas, correos) fuera de los canales autorizados."].forEach(t => c.push(bullet(t)));

  // 11 AUTORIZACION
  c.push(h1("11\tAUTORIZACIÓN Y VIGENCIA"));
  c.push(body("Por medio del presente documento se informa y autoriza el uso del Sistema de Evaluaciones HSEQ de Renergeia como herramienta oficial para la gestión de capacitaciones, evaluaciones de conocimiento y registro de asistencia."));
  c.push(body("Esta plataforma será utilizada en todas las futuras capacitaciones programadas dentro del plan anual de capacitación, garantizando la trazabilidad, el registro digital y la medición de la eficacia de las actividades formativas."));

  // ---------- Numbering ----------
  const numConfigs = [
    { reference: "bul", levels: [{ level: 0, format: LevelFormat.BULLET, text: "\u2022", alignment: AlignmentType.LEFT, style: { paragraph: { indent: { left: 600, hanging: 300 } }, run: { font: FONT } } }] },
  ];
  for (const ref of ["s41", "s51", "s64", "s65", "s66", "s67", "s68", "s69"]) {
    numConfigs.push({ reference: ref, levels: [{ level: 0, format: LevelFormat.DECIMAL, text: "%1.", alignment: AlignmentType.START, style: { paragraph: { indent: { left: 600, hanging: 320 } } } }] });
  }

  const doc = new Document({
    creator: "Renergeia",
    title: "Instructivo de Capacitación, Evaluación y Asistencia",
    features: { updateFields: true },
    styles: {
      default: { document: { run: { font: FONT, size: 22, color: BLACK } } },
      paragraphStyles: [
        { id: "Heading1", name: "Heading 1", basedOn: "Normal", next: "Normal", quickFormat: true, run: { font: FONT, size: 30, bold: true, color: BLUE }, paragraph: { spacing: { before: 240, after: 120 }, keepNext: true } },
        { id: "Heading2", name: "Heading 2", basedOn: "Normal", next: "Normal", quickFormat: true, run: { font: FONT, size: 24, bold: true, color: BLACK }, paragraph: { spacing: { before: 180, after: 100 }, keepNext: true } },
      ],
    },
    numbering: { config: numConfigs },
    sections: [{
      properties: { page: { size: { width: 12240, height: 15840 }, margin: { top: 1420, right: 1420, bottom: 1420, left: 1420, header: 480, footer: 480 } } },
      headers: { default: header },
      footers: { default: footer },
      children: [...cover, ...toc, ...c],
    }],
  });

  const buffer = await Packer.toBuffer(doc);
  fs.writeFileSync(OUT, buffer);
  console.log("OK", OUT, buffer.length, "bytes");
})().catch(e => { console.error(e); process.exit(1); });
