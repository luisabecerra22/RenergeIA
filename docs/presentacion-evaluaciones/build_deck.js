// Genera la presentación de la plataforma de Evaluaciones HSE para los equipos HSE y RRHH.
const pptxgen = require("pptxgenjs");
const sharp = require("sharp");
const React = require("react");
const ReactDOMServer = require("react-dom/server");
const Fi = require("react-icons/fi");
const path = require("path");
const fs = require("fs");

const DIR = __dirname;
const SHOTS = path.join(DIR, "shots");
const PREP = path.join(DIR, "prep");
const OUT = path.join(DIR, "RenergeIA-Evaluaciones-HSE.pptx");
if (!fs.existsSync(PREP)) fs.mkdirSync(PREP, { recursive: true });

// ---- Marca ----
const BLUE = "183963", GREEN = "6ABF4B", GRAY = "D9D9D6", DARK = "111921";
const TXT = "1F2933", MUTED = "6B7280", LIGHT = "F4F6F8", WHITE = "FFFFFF";
const FONT = "Montserrat";
const TOTAL = 17;

// Recortes por captura {left,top,width,height} (frame 1568x778/738). Enfocan el contenido.
const CROPS = {
  "01_login.jpg": { left: 466, top: 150, width: 330, height: 345 },
  "03_resultados.jpg": { left: 306, top: 82, width: 628, height: 548 },
  "04_evaluaciones.jpg": { left: 306, top: 82, width: 628, height: 315 },
  "04b_links.jpg": { left: 306, top: 82, width: 628, height: 315 },
  "06_pregunta.jpg": { left: 442, top: 92, width: 366, height: 545 },
  "07_asistencia.jpg": { left: 306, top: 82, width: 628, height: 560 },
  "08_personal.jpg": { left: 306, top: 82, width: 628, height: 548 },
  "08b_matriz.jpg": { left: 306, top: 82, width: 628, height: 548 },
  "09_detalle.jpg": { left: 446, top: 18, width: 346, height: 604 },
  "10_usuarios.jpg": { left: 306, top: 82, width: 628, height: 432 },
  "13_dashboard.jpg": { left: 306, top: 82, width: 628, height: 512 },
};

async function icon(name, color, size = 256) {
  const Comp = Fi[name];
  if (!Comp) throw new Error("icono no existe: " + name);
  const svg = ReactDOMServer.renderToStaticMarkup(React.createElement(Comp, { color: "#" + color, size }));
  const buf = await sharp(Buffer.from(svg)).png().toBuffer();
  return "image/png;base64," + buf.toString("base64");
}

// Devuelve {path,w,h} de una captura, recortada según CROPS (o completa si no hay config).
async function shot(file) {
  const src = path.join(SHOTS, file);
  if (!fs.existsSync(src)) { console.warn("FALTA captura:", file); return null; }
  const crop = CROPS[file];
  let out = src, meta;
  if (crop) {
    out = path.join(PREP, file);
    await sharp(src).extract(crop).toFile(out);
    meta = { width: crop.width, height: crop.height };
  } else {
    meta = await sharp(src).metadata();
  }
  return { path: out, w: meta.width, h: meta.height };
}

(async () => {
  const pres = new pptxgen();
  pres.layout = "LAYOUT_WIDE"; // 13.33 x 7.5
  pres.author = "Renergeia";
  pres.title = "Capacitaciones e Inducciones de Renergeia — Plataforma de Evaluaciones HSE";

  const ICONS = {};
  const iconList = ["FiLogIn", "FiGrid", "FiEdit3", "FiList", "FiLink2", "FiShare2", "FiCheckCircle", "FiXCircle",
    "FiEye", "FiUserCheck", "FiUsers", "FiFilter", "FiBarChart2", "FiDownload", "FiShield", "FiAward", "FiCheck",
    "FiArrowRight", "FiClipboard", "FiFileText", "FiSearch", "FiPlusCircle", "FiKey", "FiMonitor", "FiClock",
    "FiCopy", "FiPieChart", "FiThumbsUp", "FiMail", "FiLock", "FiTarget"];
  for (const n of iconList) { ICONS[n + "_w"] = await icon(n, WHITE); ICONS[n + "_b"] = await icon(n, BLUE); ICONS[n + "_g"] = await icon(n, GREEN); }

  const logoLight = path.join(DIR, "logo_light.png");
  const logoWhite = path.join(PREP, "logo_white.png");
  {
    const meta = await sharp(logoLight).metadata();
    const alpha = await sharp(logoLight).ensureAlpha().extractChannel(3).toColourspace("b-w").toBuffer();
    const white = await sharp({ create: { width: meta.width, height: meta.height, channels: 3, background: { r: 255, g: 255, b: 255 } } })
      .joinChannel(alpha).png().toBuffer();
    fs.writeFileSync(logoWhite, white);
  }

  // ---- Helpers de layout ----
  function chrome(slide, n, title, subtitle) {
    slide.background = { color: WHITE };
    slide.addText(title, { x: 0.5, y: 0.32, w: 10.2, h: 0.6, fontFace: FONT, fontSize: 24, bold: true, color: BLUE, margin: 0, isTextBox: true, valign: "middle" });
    if (subtitle) slide.addText(subtitle, { x: 0.5, y: 0.92, w: 10.6, h: 0.35, fontFace: FONT, fontSize: 11, color: MUTED, margin: 0, isTextBox: true, valign: "top" });
    slide.addShape(pres.shapes.ROUNDED_RECTANGLE, { x: 11.9, y: 0.4, w: 0.95, h: 0.34, rectRadius: 0.08, line: { color: GREEN, width: 1 }, fill: { color: WHITE } });
    slide.addText(String(n).padStart(2, "0") + " / " + TOTAL, { x: 11.9, y: 0.4, w: 0.95, h: 0.34, fontFace: FONT, fontSize: 8, bold: true, color: GREEN, align: "center", valign: "middle", margin: 0, isTextBox: true });
    slide.addImage({ path: logoLight, x: 0.5, y: 6.86, w: 1.13, h: 0.5 });
    slide.addText("Capacitaciones e Inducciones de Renergeia", { x: 7.8, y: 6.92, w: 5.05, h: 0.35, fontFace: FONT, fontSize: 8, color: MUTED, align: "right", valign: "middle", margin: 0, isTextBox: true });
  }

  // Imagen enmarcada, sin distorsión (contain) dentro del recuadro dado.
  function framedImage(slide, sh, x, y, w, h) {
    slide.addShape(pres.shapes.RECTANGLE, { x: x - 0.04, y: y - 0.04, w: w + 0.08, h: h + 0.08, fill: { color: WHITE }, line: { color: GRAY, width: 1 }, shadow: { type: "outer", blur: 6, offset: 2, angle: 90, color: "000000", opacity: 0.18 } });
    if (sh) slide.addImage({ path: sh.path, x, y, w, h, sizing: { type: "contain", w, h } });
    else slide.addText("captura pendiente", { x, y: y + h / 2 - 0.2, w, h: 0.4, fontFace: FONT, fontSize: 12, color: MUTED, align: "center", isTextBox: true });
  }

  function steps(slide, points, x = 7.95, w = 4.9) {
    const cardH = 1.25, gap = 0.28;
    const y0 = 1.42 + (5.31 - (points.length * cardH + (points.length - 1) * gap)) / 2;
    points.forEach((p, i) => {
      const cy = y0 + i * (cardH + gap);
      slide.addShape(pres.shapes.ROUNDED_RECTANGLE, { x, y: cy, w, h: cardH, rectRadius: 0.1, fill: { color: LIGHT }, line: { color: LIGHT, width: 0 } });
      slide.addShape(pres.shapes.OVAL, { x: x + 0.2, y: cy + 0.44, w: 0.38, h: 0.38, fill: { color: i % 2 ? GREEN : BLUE }, line: { color: i % 2 ? GREEN : BLUE, width: 0 } });
      slide.addText(String(i + 1), { x: x + 0.2, y: cy + 0.44, w: 0.38, h: 0.38, fontFace: FONT, fontSize: 14, bold: true, color: WHITE, align: "center", valign: "middle", margin: 0, isTextBox: true });
      slide.addText([
        { text: p[0], options: { bold: true, color: BLUE, fontSize: 12.5, breakLine: true } },
        { text: p[1], options: { color: TXT, fontSize: 10.5 } },
      ], { x: x + 0.72, y: cy + 0.12, w: w - 0.9, h: cardH - 0.2, fontFace: FONT, margin: 0, isTextBox: true, valign: "middle", paraSpaceAfter: 3 });
    });
  }

  async function moduleSlide(n, title, subtitle, file, points, notes) {
    const s = pres.addSlide();
    chrome(s, n, title, subtitle);
    framedImage(s, await shot(file), 0.5, 1.42, 7.15, 5.31);
    steps(s, points);
    if (notes) s.addNotes(notes);
    return s;
  }

  // ================= 1. PORTADA =================
  {
    const s = pres.addSlide();
    s.background = { color: BLUE };
    s.addImage({ path: logoWhite, x: 0.7, y: 0.62, w: 2.6, h: 1.15 });
    s.addText("Capacitaciones e Inducciones", { x: 0.7, y: 2.3, w: 6.3, h: 0.7, fontFace: FONT, fontSize: 30, bold: true, color: WHITE, margin: 0, isTextBox: true });
    s.addText("de Renergeia", { x: 0.7, y: 2.9, w: 6.3, h: 0.7, fontFace: FONT, fontSize: 30, bold: true, color: WHITE, margin: 0, isTextBox: true });
    s.addText("Guía de uso de la plataforma de Evaluaciones HSE", { x: 0.7, y: 3.8, w: 6.2, h: 0.5, fontFace: FONT, fontSize: 15, color: GREEN, bold: true, margin: 0, isTextBox: true });
    s.addText("Dirigido a los equipos de HSE y Recursos Humanos", { x: 0.7, y: 4.35, w: 6.2, h: 0.4, fontFace: FONT, fontSize: 12, color: GRAY, margin: 0, isTextBox: true });
    s.addText("Septiembre 2026", { x: 0.7, y: 4.75, w: 6.2, h: 0.4, fontFace: FONT, fontSize: 12, color: GRAY, margin: 0, isTextBox: true });
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x: 7.1, y: 1.15, w: 6.0, h: 3.95, rectRadius: 0.12, fill: { color: DARK }, line: { color: "2B4A75", width: 1 }, shadow: { type: "outer", blur: 14, offset: 4, angle: 90, color: "000000", opacity: 0.4 } });
    const cover = await shot("13_dashboard.jpg");
    if (cover) framedImageBare(s, cover, 7.28, 1.32, 5.64, 3.6);
    s.addText("Acceso restringido para HSE y Recursos Humanos", { x: 7.1, y: 5.3, w: 6.0, h: 0.4, fontFace: FONT, fontSize: 10, color: GRAY, align: "center", margin: 0, isTextBox: true });
    s.addNotes("Portada. Esta es la guía de la plataforma web de Evaluaciones e Inducciones HSE, para que los equipos de HSE y RRHH la usen de principio a fin.");
  }
  function framedImageBare(slide, sh, x, y, w, h) {
    slide.addImage({ path: sh.path, x, y, w, h, sizing: { type: "contain", w, h } });
  }

  // ================= 2. QUÉ VAS A APRENDER =================
  {
    const s = pres.addSlide();
    chrome(s, 2, "Qué vas a aprender", "Todo lo que necesitas para manejar la plataforma de principio a fin");
    const items = [
      ["FiLogIn", "Ingresar a la plataforma"],
      ["FiEdit3", "Crear una evaluación o inducción"],
      ["FiLink2", "Compartir los enlaces"],
      ["FiCheckCircle", "Ver aprobados y reprobados"],
      ["FiEye", "Revisar y corregir"],
      ["FiUserCheck", "Registrar y ver asistencia"],
      ["FiUsers", "Matriz de asistencia por persona"],
      ["FiFilter", "Buscar y filtrar"],
      ["FiBarChart2", "Ver el dashboard"],
      ["FiDownload", "Exportar la información"],
      ["FiShield", "Usuarios y roles"],
      ["FiThumbsUp", "Buenas prácticas"],
    ];
    const cols = 4, tw = 2.92, th = 1.5, gx = 0.18, gy = 0.22, x0 = 0.5, y0 = 1.55;
    items.forEach((m, i) => {
      const c = i % cols, r = Math.floor(i / cols);
      const x = x0 + c * (tw + gx), y = y0 + r * (th + gy);
      s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x, y, w: tw, h: th, rectRadius: 0.1, fill: { color: WHITE }, line: { color: GRAY, width: 1 } });
      s.addShape(pres.shapes.OVAL, { x: x + 0.28, y: y + 0.45, w: 0.6, h: 0.6, fill: { color: i % 2 ? GREEN : BLUE }, line: { color: i % 2 ? GREEN : BLUE, width: 0 } });
      s.addImage({ data: ICONS[m[0] + "_w"], x: x + 0.43, y: y + 0.6, w: 0.3, h: 0.3 });
      s.addText(m[1], { x: x + 1.05, y: y + 0.2, w: tw - 1.2, h: th - 0.4, fontFace: FONT, fontSize: 11.5, bold: true, color: TXT, margin: 0, isTextBox: true, valign: "middle" });
    });
    s.addNotes("Agenda: estos son los temas que cubre la guía. Cada uno tiene su diapositiva con pantallas reales y el paso a paso.");
  }

  // ================= 3..15 =================
  await moduleSlide(3, "Cómo ingresar a la plataforma", "Acceso restringido con usuario y contraseña", "01_login.jpg", [
    ["Abre el enlace", "Entra a la dirección de la plataforma que comparte el área de Calidad."],
    ["Escribe tus datos", "Usuario y contraseña asignados a tu área (HSE o Recursos Humanos)."],
    ["Presiona Ingresar", "Entras al panel. Si no tienes acceso, solicítalo a Calidad."],
  ], "Cómo ingresar: la plataforma es de acceso restringido. Cada área tiene su usuario. Al ingresar se abre el panel de administración.");

  await moduleSlide(4, "El panel por dentro", "Seis pestañas para todo el proceso de capacitación", "03_resultados.jpg", [
    ["Barra de pestañas", "Resultados, Evaluaciones, Dashboard, Asistencia, Personal y Usuarios."],
    ["Siempre a la vista", "Cambias de sección con un clic; la pestaña activa se resalta en verde."],
    ["Tarjetas de resumen", "Cada sección abre con indicadores rápidos arriba y la tabla debajo."],
  ], "El panel tiene seis pestañas. La barra superior siempre está visible. La pestaña activa se ve resaltada.");

  await moduleSlide(5, "Crear una evaluación o inducción", "Desde la pestaña Evaluaciones", "04_evaluaciones.jpg", [
    ["+ Nueva evaluación", "Crea una capacitación nueva y abre su editor."],
    ["Editar o eliminar", "Desde la lista puedes volver a editar cualquiera cuando quieras."],
    ["Estado Activa", "Solo las evaluaciones activas son visibles para los participantes."],
  ], "En Evaluaciones se ve la lista. Con '+ Nueva evaluación' se crea una y se abre el editor. El estado Activa/Inactiva controla si es visible.");

  await moduleSlide(6, "Escribe las preguntas y marca la correcta", "El editor de la evaluación", "06_pregunta.jpg", [
    ["Escribe el enunciado", "Agrega tantas preguntas y opciones como necesites."],
    ["Marca la correcta", "El círculo verde a la izquierda define la respuesta correcta."],
    ["Guardar cambios", "La calificación se hará sola con base en las respuestas correctas."],
  ], "El editor: se escriben preguntas y opciones. El círculo verde marca la respuesta correcta. Al guardar, la plataforma califica automáticamente.");

  await moduleSlide(7, "Compartir los enlaces", "Un enlace para la evaluación y otro para la asistencia", "04b_links.jpg", [
    ["Copiar link (evaluación)", "En la columna Links, copia el enlace de esa evaluación."],
    ["Copiar link de asistencia", "Botón arriba: enlace para registrar la asistencia."],
    ["Pégalo donde quieras", "WhatsApp, correo o un QR: la persona entra desde su celular."],
  ], "Cada evaluación tiene su enlace (columna Links → Copiar link). El botón 'Copiar link de asistencia' da el enlace general de asistencia. Se comparten por WhatsApp, correo o QR.");

  await moduleSlide(8, "Ver aprobados y reprobados", "La pestaña Resultados", "03_resultados.jpg", [
    ["Tarjetas de resumen", "Presentadas, aprobados, reprobados y tasa de aprobación."],
    ["Columna Estado", "Cada persona aparece como Aprobado (verde) o Reprobado (rojo)."],
    ["Nota y certificado", "Ves la nota; si aprobó, puedes abrir su certificado."],
  ], "Resultados muestra las tarjetas (aprobados/reprobados/tasa) y la tabla con el estado por persona. Los aprobados tienen botón de certificado.");

  await moduleSlide(9, "Revisar el detalle y corregir", "Botón Ver en cada registro", "09_detalle.jpg", [
    ["Ver respuestas", "Muestra qué respondió y cuál era la correcta, pregunta por pregunta."],
    ["Retroalimentación", "Incluye la valoración del capacitador y la firma del participante."],
    ["Corregir la evaluación", "Si una pregunta quedó mal, se ajusta en Editar → Guardar."],
  ], "El botón Ver abre el detalle de cada intento: respuestas correctas e incorrectas, retroalimentación y firma. Para corregir la prueba se edita en la evaluación.");

  await moduleSlide(10, "Registrar y ver asistencia", "La pestaña Asistencia", "07_asistencia.jpg", [
    ["Registros por persona", "Fecha, datos, capacitación y firma de cada asistente."],
    ["Filtra por capacitación", "Elige la capacitación o busca por nombre o cédula."],
    ["Exportar a Excel", "Descarga la asistencia de la capacitación seleccionada."],
  ], "Asistencia lista los registros con su firma. Se puede filtrar por capacitación y exportar a Excel la que se necesite.");

  await moduleSlide(11, "Matriz de asistencia por persona", "La pestaña Personal", "08b_matriz.jpg", [
    ["Planta de personal", "Toda la gente, con área, cargo y proyecto en una sola tabla."],
    ["✓ / ✗ por capacitación", "Una columna por evaluación: verde asistió, rojo faltó."],
    ["% de asistencia", "El porcentaje se recalcula según las evaluaciones mostradas."],
  ], "Personal cruza la planta con las capacitaciones: verde asistió, rojo faltó, y una columna con el % de asistencia por persona.");

  await moduleSlide(12, "Buscar y filtrar", "Encuentra a una persona o un grupo en segundos", "08_personal.jpg", [
    ["Buscador", "Escribe nombre, cédula o cargo para encontrar a alguien."],
    ["Filtros por columna", "Área, cargo, proyecto y tipo de trabajo."],
    ["Mostrar evaluaciones", "Enciende o apaga capacitaciones para ver solo esas."],
  ], "Los filtros permiten buscar por texto o acotar por área, cargo, proyecto y trabajo. Los botones de cada capacitación filtran quién asistió o faltó.");

  await moduleSlide(13, "Dashboard de capacitación", "El resumen visual del área", "13_dashboard.jpg", [
    ["Indicadores clave", "Personas capacitadas, horas, aprobados y registros de asistencia."],
    ["Gráficas de barras", "Por capacitación y por departamento, de un vistazo."],
    ["Siempre actualizado", "Se arma solo con las evaluaciones y asistencias registradas."],
  ], "El Dashboard resume el área: tarjetas y gráficas por capacitación y departamento. Se actualiza solo con lo registrado.");

  // ================= 14. EXPORTAR =================
  {
    const s = pres.addSlide();
    chrome(s, 14, "Exportar la información", "Cada sección tiene su botón para descargar a Excel");
    const cards = [
      ["FiCheckCircle", "Resultados", "Botón Exportar a Excel (CSV): aprobados, reprobados y notas, con los filtros que tengas puestos.", BLUE],
      ["FiUserCheck", "Asistencia", "Botón Exportar Excel: la asistencia de la capacitación seleccionada, lista para tu informe.", GREEN],
      ["FiUsers", "Personal", "Botón Exportar a Excel: la matriz de personas con su asistencia y el % por capacitación.", BLUE],
    ];
    cards.forEach((c, i) => {
      const x = 0.5 + i * 4.16, y = 1.6, w = 4.0, h = 3.5;
      s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x, y, w, h, rectRadius: 0.12, fill: { color: WHITE }, line: { color: GRAY, width: 1 }, shadow: { type: "outer", blur: 6, offset: 2, angle: 90, color: "000000", opacity: 0.12 } });
      s.addShape(pres.shapes.OVAL, { x: x + (w - 1.0) / 2, y: y + 0.4, w: 1.0, h: 1.0, fill: { color: c[3] }, line: { color: c[3], width: 0 } });
      s.addImage({ data: ICONS[c[0] + "_w"], x: x + (w - 0.5) / 2, y: y + 0.65, w: 0.5, h: 0.5 });
      s.addText(c[1], { x, y: y + 1.55, w, h: 0.5, fontFace: FONT, fontSize: 16, bold: true, color: BLUE, align: "center", margin: 0, isTextBox: true });
      s.addText(c[2], { x: x + 0.3, y: y + 2.1, w: w - 0.6, h: 1.3, fontFace: FONT, fontSize: 10.5, color: TXT, align: "center", margin: 0, isTextBox: true, valign: "top" });
    });
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x: 0.5, y: 5.4, w: 12.33, h: 1.05, rectRadius: 0.12, fill: { color: LIGHT }, line: { color: LIGHT, width: 0 } });
    s.addImage({ data: ICONS.FiPieChart_b, x: 0.85, y: 5.62, w: 0.5, h: 0.5 });
    s.addText([{ text: "El Dashboard es visual: ", options: { bold: true, color: BLUE } }, { text: "para llevar sus cifras a un informe, exporta desde Resultados, Asistencia o Personal y arma tus tablas y gráficas en Excel.", options: { color: TXT } }],
      { x: 1.55, y: 5.4, w: 11.0, h: 1.05, fontFace: FONT, fontSize: 11, margin: 0, isTextBox: true, valign: "middle" });
    s.addNotes("Exportar: cada sección descarga a Excel (Resultados en CSV, Asistencia y Personal en Excel). El dashboard es visual; sus datos salen de esas tres exportaciones.");
  }

  await moduleSlide(15, "Usuarios y roles", "Quién ve qué (solo el administrador)", "10_usuarios.jpg", [
    ["Administrador", "Ve y gestiona todas las áreas y crea usuarios."],
    ["Área HSE / RRHH", "Cada área ve solo sus evaluaciones, asistencias y personal."],
    ["Contraseñas", "Se cambian en Usuarios (admin) o en Mi cuenta (cada quien)."],
  ], "Usuarios (solo admin) define roles: administrador ve todo; los usuarios de área (HSE, RRHH) ven solo lo suyo. Las contraseñas se cambian aquí o en Mi cuenta.");

  // ================= 16. BUENAS PRÁCTICAS =================
  {
    const s = pres.addSlide();
    chrome(s, 16, "Buenas prácticas", "Para que la información sea confiable y fácil de auditar");
    const tips = [
      ["FiEdit3", "Un título claro por capacitación", "Usa nombres reconocibles; el tema aparece en el certificado."],
      ["FiUserCheck", "Comparte los dos enlaces", "El de asistencia al iniciar y el de evaluación al terminar."],
      ["FiUsers", "Mantén la planta al día", "Importa o agrega al personal para que la matriz cruce bien."],
      ["FiDownload", "Exporta y respalda", "Descarga los resultados y la asistencia para tus informes."],
    ];
    tips.forEach((it, i) => {
      const c = i % 2, r = Math.floor(i / 2);
      const x = 0.5 + c * 6.28, y = 1.55 + r * 2.5, w = 6.05, h = 2.25;
      s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x, y, w, h, rectRadius: 0.12, fill: { color: WHITE }, line: { color: GRAY, width: 1 }, shadow: { type: "outer", blur: 6, offset: 2, angle: 90, color: "000000", opacity: 0.12 } });
      s.addShape(pres.shapes.OVAL, { x: x + 0.35, y: y + 0.7, w: 0.85, h: 0.85, fill: { color: i % 2 ? GREEN : BLUE }, line: { color: i % 2 ? GREEN : BLUE, width: 0 } });
      s.addImage({ data: ICONS[it[0] + "_w"], x: x + 0.56, y: y + 0.91, w: 0.43, h: 0.43 });
      s.addText([{ text: it[1], options: { bold: true, color: BLUE, fontSize: 13, breakLine: true } }, { text: it[2], options: { color: TXT, fontSize: 10.5 } }],
        { x: x + 1.5, y: y + 0.25, w: w - 1.8, h: h - 0.5, fontFace: FONT, margin: 0, isTextBox: true, valign: "middle", paraSpaceAfter: 4 });
    });
    s.addNotes("Buenas prácticas para que la información quede confiable y auditable.");
  }

  // ================= 17. CIERRE =================
  {
    const s = pres.addSlide();
    s.background = { color: BLUE };
    s.addImage({ path: logoWhite, x: 0.7, y: 0.6, w: 2.1, h: 0.93 });
    s.addText("A capacitar con la plataforma", { x: 0.7, y: 1.95, w: 11.9, h: 0.8, fontFace: FONT, fontSize: 30, bold: true, color: WHITE, margin: 0, isTextBox: true });
    const asks = [
      ["FiLogIn", "Entra con tu usuario", "Cada área (HSE y RRHH) tiene su acceso a la plataforma."],
      ["FiEdit3", "Crea y comparte", "Arma la evaluación o inducción y comparte sus enlaces."],
      ["FiBarChart2", "Mide y reporta", "Consulta resultados y asistencia, y expórtalos a Excel."],
    ];
    asks.forEach((a, i) => {
      const x = 0.7 + i * 4.05, y = 3.05, w = 3.85, h = 2.6;
      s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x, y, w, h, rectRadius: 0.14, fill: { color: "0E2340" }, line: { color: "2B4A75", width: 1 } });
      s.addShape(pres.shapes.OVAL, { x: x + 0.3, y: y + 0.3, w: 0.7, h: 0.7, fill: { color: GREEN }, line: { color: GREEN, width: 0 } });
      s.addImage({ data: ICONS[a[0] + "_w"], x: x + 0.47, y: y + 0.47, w: 0.36, h: 0.36 });
      s.addText([{ text: a[1], options: { bold: true, color: WHITE, fontSize: 13, breakLine: true } }, { text: a[2], options: { color: GRAY, fontSize: 10 } }],
        { x: x + 0.3, y: y + 1.15, w: w - 0.6, h: h - 1.3, fontFace: FONT, margin: 0, isTextBox: true, valign: "top", paraSpaceAfter: 4 });
    });
    s.addText("¿Dudas? Escríbele al área de Calidad de Renergeia.", { x: 0.7, y: 6.05, w: 8, h: 0.5, fontFace: FONT, fontSize: 13, bold: true, color: GREEN, margin: 0, isTextBox: true });
    s.addNotes("Cierre: resumen del flujo — entrar, crear y compartir, medir y reportar. Soporte con el área de Calidad.");
  }

  await pres.writeFile({ fileName: OUT });
  console.log("OK", OUT);
})().catch((e) => { console.error(e); process.exit(1); });
