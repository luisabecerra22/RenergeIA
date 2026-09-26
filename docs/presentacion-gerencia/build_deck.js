// Genera la presentación RenergeIA para gerencia
const pptxgen = require("pptxgenjs");
const sharp = require("sharp");
const React = require("react");
const ReactDOMServer = require("react-dom/server");
const Fi = require("react-icons/fi");
const path = require("path");
const fs = require("fs");

const DIR = __dirname;
const SHOTS = path.join(DIR, "shots");
const OUT = path.join(DIR, "RenergeIA-Presentacion-Gerencia.pptx");

// ---- Marca ----
const BLUE = "183963", GREEN = "6ABF4B", GRAY = "D9D9D6", DARK = "111921";
const TXT = "1F2933", MUTED = "6B7280", LIGHT = "F4F6F8", WHITE = "FFFFFF";
const FONT = "Verdana";
const TOTAL = 20;

// ---- Íconos (react-icons -> PNG base64) ----
async function icon(name, color, size = 256) {
  const Comp = Fi[name];
  if (!Comp) throw new Error("icono no existe: " + name);
  const svg = ReactDOMServer.renderToStaticMarkup(React.createElement(Comp, { color: "#" + color, size }));
  const buf = await sharp(Buffer.from(svg)).png().toBuffer();
  return "image/png;base64," + buf.toString("base64");
}

async function cropShot(file, w, h, out) {
  const src = path.join(SHOTS, file);
  const dst = path.join(SHOTS, out);
  await sharp(src).extract({ left: 0, top: 0, width: w, height: h }).png().toFile(dst);
  return dst;
}

(async () => {
  const pres = new pptxgen();
  pres.layout = "LAYOUT_WIDE"; // 13.33 x 7.5
  pres.author = "Renergeia";
  pres.title = "RenergeIA — Plataforma para la gestión de proyectos";

  const ICONS = {};
  const iconList = ["FiFolder", "FiCalendar", "FiFileText", "FiDollarSign", "FiBarChart2", "FiFile", "FiShield",
    "FiCheckCircle", "FiFeather", "FiUsers", "FiBell", "FiCloud", "FiMessageCircle", "FiClipboard", "FiAlertTriangle",
    "FiMail", "FiEyeOff", "FiRepeat", "FiCheck", "FiTrendingUp", "FiZap", "FiTarget", "FiGlobe", "FiMonitor",
    "FiDatabase", "FiLayers", "FiClock", "FiAward", "FiFlag", "FiArrowRight", "FiSmartphone", "FiPlayCircle"];
  for (const n of iconList) { ICONS[n + "_w"] = await icon(n, WHITE); ICONS[n + "_b"] = await icon(n, BLUE); ICONS[n + "_g"] = await icon(n, GREEN); }

  const logoLight = path.join(DIR, "logo_light.png");
  const logoWhite = path.join(DIR, "logo_white.png");
  const shot = (f) => path.join(SHOTS, f);

  // recortes
  await cropShot("18b_clima_alerta.png", 1995, 1250, "18b_crop.png");
  await cropShot("18_clima.png", 1995, 1250, "18_crop.png");
  await cropShot("02_dashboard.png", 2400, 1500, "02_cover.png");

  // ---- Helpers de layout ----
  function chrome(slide, n, title, subtitle) {
    slide.background = { color: WHITE };
    slide.addText(title, { x: 0.5, y: 0.32, w: 10.2, h: 0.6, fontFace: FONT, fontSize: 24, bold: true, color: BLUE, margin: 0, isTextBox: true, valign: "middle" });
    if (subtitle) slide.addText(subtitle, { x: 0.5, y: 0.92, w: 10.6, h: 0.35, fontFace: FONT, fontSize: 11, color: MUTED, margin: 0, isTextBox: true, valign: "top" });
    // número de diapositiva
    slide.addShape(pres.shapes.ROUNDED_RECTANGLE, { x: 11.9, y: 0.4, w: 0.95, h: 0.34, rectRadius: 0.08, line: { color: GREEN, width: 1 }, fill: { color: WHITE } });
    slide.addText(String(n).padStart(2, "0") + " / " + TOTAL, { x: 11.9, y: 0.4, w: 0.95, h: 0.34, fontFace: FONT, fontSize: 8, bold: true, color: GREEN, align: "center", valign: "middle", margin: 0, isTextBox: true });
    // pie: logo + lema
    slide.addImage({ path: logoLight, x: 0.5, y: 6.83, w: 1.13, h: 0.5 });
    slide.addText("Plataforma para la gestión de proyectos", { x: 8.3, y: 6.9, w: 4.55, h: 0.35, fontFace: FONT, fontSize: 8, color: MUTED, align: "right", valign: "middle", margin: 0, isTextBox: true });
  }

  function framedImage(slide, file, x, y, w, h) {
    slide.addShape(pres.shapes.RECTANGLE, { x: x - 0.04, y: y - 0.04, w: w + 0.08, h: h + 0.08, fill: { color: WHITE }, line: { color: GRAY, width: 1 }, shadow: { type: "outer", blur: 6, offset: 2, angle: 90, color: "000000", opacity: 0.18 } });
    slide.addImage({ path: file, x, y, w, h });
  }

  // 3 puntos clave en columna derecha
  function keyPoints(slide, points, x = 9.35, y = 1.4, w = 3.5) {
    const cardH = 1.25, gap = 0.28; y = 1.42 + (5.31 - (3 * cardH + 2 * gap)) / 2;
    points.forEach((p, i) => {
      const cy = y + i * (cardH + gap);
      slide.addShape(pres.shapes.ROUNDED_RECTANGLE, { x, y: cy, w, h: cardH, rectRadius: 0.1, fill: { color: LIGHT }, line: { color: LIGHT, width: 0 } });
      slide.addShape(pres.shapes.OVAL, { x: x + 0.18, y: cy + 0.2, w: 0.36, h: 0.36, fill: { color: GREEN }, line: { color: GREEN, width: 0 } });
      slide.addImage({ data: ICONS.FiCheck_w, x: x + 0.25, y: cy + 0.27, w: 0.22, h: 0.22 });
      slide.addText([
        { text: p[0], options: { bold: true, color: BLUE, fontSize: 11, breakLine: true } },
        { text: p[1], options: { color: TXT, fontSize: 9.5 } },
      ], { x: x + 0.66, y: cy + 0.12, w: w - 0.8, h: cardH - 0.2, fontFace: FONT, margin: 0, isTextBox: true, valign: "top", paraSpaceAfter: 3 });
    });
  }

  // Diapositiva de módulo estándar: captura grande a la izquierda + 3 puntos
  function moduleSlide(n, title, subtitle, file, points, notes) {
    const s = pres.addSlide();
    chrome(s, n, title, subtitle);
    framedImage(s, shot(file), 0.5, 1.42, 8.5, 5.31);
    keyPoints(s, points);
    if (notes) s.addNotes(notes);
    return s;
  }

  // ================= 1. PORTADA =================
  {
    const s = pres.addSlide();
    s.background = { color: BLUE };
    s.addImage({ path: logoWhite, x: 0.7, y: 0.6, w: 2.6, h: 1.15 });
    s.addText("RenergeIA", { x: 0.7, y: 2.35, w: 6.0, h: 1.0, fontFace: FONT, fontSize: 44, bold: true, color: WHITE, margin: 0, isTextBox: true });
    s.addText("Plataforma para la gestión de proyectos", { x: 0.7, y: 3.35, w: 6.0, h: 0.6, fontFace: FONT, fontSize: 18, color: GREEN, bold: true, margin: 0, isTextBox: true });
    s.addText("Presentación a gerencia  ·  Septiembre 2026", { x: 0.7, y: 4.1, w: 6.0, h: 0.4, fontFace: FONT, fontSize: 12, color: GRAY, margin: 0, isTextBox: true });
    s.addText("Proyecto piloto: La Soberana (COS5SO)", { x: 0.7, y: 4.5, w: 6.0, h: 0.4, fontFace: FONT, fontSize: 12, color: GRAY, margin: 0, isTextBox: true });
    // mockup de pantalla a la derecha
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x: 7.1, y: 1.05, w: 6.0, h: 3.95, rectRadius: 0.12, fill: { color: DARK }, line: { color: "2B4A75", width: 1 }, shadow: { type: "outer", blur: 14, offset: 4, angle: 90, color: "000000", opacity: 0.4 } });
    s.addImage({ path: shot("02_cover.png"), x: 7.25, y: 1.2, w: 5.7, h: 3.56 });
    s.addShape(pres.shapes.RECTANGLE, { x: 9.3, y: 5.0, w: 1.6, h: 0.14, fill: { color: "0E2340" }, line: { color: "0E2340", width: 0 } });
    s.addNotes("Portada. RenergeIA es la plataforma web desarrollada internamente para gestionar los proyectos EPC de la empresa. Hoy ya tiene cargados los datos reales de La Soberana.");
  }

  // ================= 2. EL RETO DE HOY =================
  {
    const s = pres.addSlide();
    chrome(s, 2, "El reto de hoy", "La información de cada proyecto vive en muchos lugares distintos");
    const cards = [
      ["FiMail", "Archivos y correos dispersos", "Cronograma, tesorería, documentos y HSE en Excel y correos separados."],
      ["FiEyeOff", "Sin visión en tiempo real", "El estado del proyecto se conoce solo cuando alguien lo consolida a mano."],
      ["FiRepeat", "Reprocesos y digitación", "Los mismos datos se pasan varias veces entre archivos y personas."],
      ["FiAlertTriangle", "Vencimientos sin alerta", "Pólizas, licencias y documentos se vencen sin que nadie lo vea a tiempo."],
    ];
    cards.forEach((c, i) => {
      const x = 0.5 + i * 3.1, y = 1.6, w = 2.9, h = 2.75;
      s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x, y, w, h, rectRadius: 0.12, fill: { color: LIGHT }, line: { color: LIGHT, width: 0 } });
      s.addShape(pres.shapes.OVAL, { x: x + 0.3, y: y + 0.35, w: 0.8, h: 0.8, fill: { color: BLUE }, line: { color: BLUE, width: 0 } });
      s.addImage({ data: ICONS[c[0] + "_w"], x: x + 0.5, y: y + 0.55, w: 0.4, h: 0.4 });
      s.addText(c[1], { x: x + 0.3, y: y + 1.35, w: w - 0.6, h: 0.75, fontFace: FONT, fontSize: 12, bold: true, color: BLUE, margin: 0, isTextBox: true, valign: "top" });
      s.addText(c[2], { x: x + 0.3, y: y + 1.95, w: w - 0.6, h: 0.75, fontFace: FONT, fontSize: 9.5, color: TXT, margin: 0, isTextBox: true, valign: "top" });
    });
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x: 0.5, y: 4.85, w: 12.33, h: 1.15, rectRadius: 0.12, fill: { color: BLUE }, line: { color: BLUE, width: 0 } });
    s.addImage({ data: ICONS.FiTarget_w, x: 0.85, y: 5.17, w: 0.5, h: 0.5 });
    s.addText([
      { text: "La pregunta que gerencia hace cada semana: ", options: { color: GRAY } },
      { text: "¿cómo va el proyecto hoy?", options: { color: WHITE, bold: true } },
    ], { x: 1.55, y: 4.85, w: 11.0, h: 1.15, fontFace: FONT, fontSize: 15, margin: 0, isTextBox: true, valign: "middle" });
    s.addNotes("Contexto: hoy la información está repartida en Excel, tesorería, correos y formatos en papel. Responder cómo va el proyecto exige consolidar a mano.");
  }

  // ================= 3. QUÉ ES RENERGEIA =================
  {
    const s = pres.addSlide();
    chrome(s, 3, "Qué es RenergeIA", "Una sola plataforma web, en la nube, que cubre todo el ciclo del proyecto EPC");
    const mods = [
      ["FiFolder", "Proyectos"], ["FiCalendar", "Cronograma"], ["FiFileText", "Informe Diario"], ["FiDollarSign", "Costos"],
      ["FiBarChart2", "Histogramas"], ["FiFile", "Documentos"], ["FiShield", "Seguridad"],
      ["FiCheckCircle", "Calidad"], ["FiFeather", "Ambiental"], ["FiUsers", "Social"], ["FiAward", "Auditorías HSEQ"],
      ["FiBell", "Alertas"], ["FiCloud", "Clima"], ["FiMessageCircle", "WhatsApp"],
    ];
    const cols = 7, tw = 1.62, th = 1.5, gx = 0.165, gy = 0.2, x0 = 0.5, y0 = 1.5;
    mods.forEach((m, i) => {
      const c = i % cols, r = Math.floor(i / cols);
      const x = x0 + c * (tw + gx), y = y0 + r * (th + gy);
      s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x, y, w: tw, h: th, rectRadius: 0.1, fill: { color: WHITE }, line: { color: GRAY, width: 1 } });
      s.addShape(pres.shapes.OVAL, { x: x + (tw - 0.62) / 2, y: y + 0.22, w: 0.62, h: 0.62, fill: { color: r === 0 ? BLUE : GREEN }, line: { color: r === 0 ? BLUE : GREEN, width: 0 } });
      s.addImage({ data: ICONS[m[0] + "_w"], x: x + (tw - 0.32) / 2, y: y + 0.37, w: 0.32, h: 0.32 });
      s.addText(m[1], { x: x + 0.05, y: y + 0.95, w: tw - 0.1, h: 0.45, fontFace: FONT, fontSize: 9, bold: true, color: TXT, align: "center", valign: "top", margin: 0, isTextBox: true });
    });
    const facts = [["FiGlobe", "En la nube (Google Cloud)", "Sin servidores propios ni instalación"], ["FiMonitor", "Desde cualquier navegador", "Oficina, obra o celular"], ["FiDatabase", "Datos reales ya cargados", "La Soberana: cronograma, costos, nómina y documentos"]];
    facts.forEach((f, i) => {
      const x = 0.5 + i * 4.16, y = 5.15, w = 4.0, h = 1.15;
      s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x, y, w, h, rectRadius: 0.1, fill: { color: LIGHT }, line: { color: LIGHT, width: 0 } });
      s.addImage({ data: ICONS[f[0] + "_b"], x: x + 0.25, y: y + 0.36, w: 0.42, h: 0.42 });
      s.addText([{ text: f[1], options: { bold: true, color: BLUE, fontSize: 10.5, breakLine: true } }, { text: f[2], options: { color: TXT, fontSize: 9 } }],
        { x: x + 0.85, y: y + 0.12, w: w - 1.0, h: h - 0.2, fontFace: FONT, margin: 0, isTextBox: true, valign: "middle", paraSpaceAfter: 2 });
    });
    s.addNotes("Mapa de módulos. Todo vive en una sola aplicación web en Google Cloud. A continuación se muestra cada módulo con pantallas reales de La Soberana.");
  }

  // ================= 4..13 MÓDULOS =================
  moduleSlide(4, "Dashboard del proyecto", "Avance real contra planificado en un solo vistazo", "02_dashboard.png", [
    ["Curva S automática", "Se construye sola desde el cronograma y los informes diarios."],
    ["Indicadores clave", "Avance programado, avance real, desviación y SPI del proyecto."],
    ["Semáforo de actividades", "Atrasadas, críticas y finalizadas, siempre al día."],
  ], "Dashboard de La Soberana: 63,8 % programado frente a 56,2 % real, con desviación y SPI. La Curva S se calcula sola.");

  moduleSlide(5, "Cronograma de actividades (WBS)", "La estructura de trabajo del proyecto con avance por actividad", "03_wbs.png", [
    ["Importa MS Project", "Carga el cronograma .mpp del proyecto o la plantilla EPC estándar."],
    ["Reprogramaciones con historial", "Cada versión queda guardada; la vigente alimenta el dashboard."],
    ["Planificado vs real", "170 actividades con desviación, fechas y estado por disciplina."],
  ], "Cronograma con 170 actividades. Se importa directo desde MS Project y se pueden crear reprogramaciones sin perder el histórico.");

  moduleSlide(6, "Informe Diario de obra", "El registro del día a día que alimenta el avance del proyecto", "04_informe_diario.png", [
    ["Avance por actividad", "Toma las actividades del cronograma y registra el avance del día."],
    ["Personal, horas y clima", "Todo el contexto del día en un solo formato."],
    ["Flujo de revisión", "Borrador, revisión y aprobación; se exporta a PDF."],
  ], "Informe diario: el residente registra avances por actividad; el sistema calcula el avance real y lo lleva a la Curva S.");

  moduleSlide(7, "Costos: Presupuesto", "Control por rubro en pesos y dólares", "05_costos_presupuesto.png", [
    ["Ejecutado automático", "Se calcula desde el archivo de tesorería; nadie lo digita."],
    ["Doble moneda", "Vista en COP, en USD o ambas convertidas con la TRM del proyecto."],
    ["Alertas por rubro", "Cercano al límite y sobrecosto, visibles de inmediato."],
  ], "Presupuesto de La Soberana: 2.951 millones COP + 737 mil USD, 35 % ejecutado. El ejecutado se sincroniza con el flujo de caja de tesorería.");

  moduleSlide(8, "Costos: Órdenes de compra", "Compromisos aprobados, facturados y pagados desde tesorería", "06_costos_compromisos.png", [
    ["Una sola importación", "El archivo semanal de tesorería actualiza OC y flujo de caja."],
    ["Aprobado vs facturado vs pagado", "Saldo por pagar y OC sobrepasadas con alerta."],
    ["Hitos por factura", "Cada OC muestra sus facturas y su estado de pago."],
  ], "100 órdenes de compra cargadas desde tesorería. Se detectan 13 OC sobrepasadas y facturas sin código para asignar.");

  moduleSlide(9, "Costos: Dashboard financiero", "Indicadores y gráficos para gerencia, más el consolidado semanal", "09_costos_dashboard.png", [
    ["Exposición total", "Presupuesto, ejecutado, comprometido y saldo disponible."],
    ["Gráficos por categoría", "Presupuesto vs ejecutado vs compromisos por rubro."],
    ["Consolidado semanal", "Informe de cierre con comparativo semana a semana y análisis con IA."],
  ], "Dashboard financiero con exposición total y gráficos por categoría. El consolidado semanal genera el informe para gerencia.");

  moduleSlide(10, "Histogramas de personal", "Personal real desde la nómina contra lo planificado en la oferta", "10_histogramas.png", [
    ["Real desde la nómina", "Personas por cargo y mes, tomadas de tesorería; sin digitar."],
    ["Planificado desde la BOM", "Se importa la hoja de personal de la oferta comercial."],
    ["Comparativo por cargo", "Muestra dónde hay más o menos gente de la prevista."],
  ], "Histograma real construido desde la nómina de tesorería. Se compara con el planificado de la BOM de la oferta.");

  moduleSlide(11, "Documentos del proyecto", "Planificación documental de ingeniería, procedimientos y HSE", "11_documentos.png", [
    ["Carga desde el Excel del cliente", "Se importa la planificación documental sin duplicar."],
    ["Alertas de días sin atender", "Amarillo desde 4 días, rojo desde 8; en cancha de quién está."],
    ["Responsables por área", "Cada disciplina ve sus pendientes y valida en línea."],
  ], "122 documentos de La Soberana: 60 pendientes por emitir, 48 validados. El semáforo muestra cuáles llevan más días sin respuesta.");

  moduleSlide(12, "HSEQ: Seguridad y salud en el trabajo", "Plan de trabajo, inspecciones, EPP, capacitaciones e incidentes", "12_hseq_seguridad.png", [
    ["Plan de trabajo anual", "Actividades mensuales con cumplimiento y vencidas."],
    ["Inspecciones y permisos", "ATS, OTS, STC, pausas activas y permisos de trabajo."],
    ["Indicadores en línea", "Semáforo del área de seguridad y acciones pendientes."],
  ], "Módulo de seguridad: plan de trabajo HSE, inspecciones, EPP, capacitaciones, incidentes e indicadores.");

  moduleSlide(13, "HSEQ: Matriz de riesgos (IPERV)", "Catálogo GTC 45 para proyectos solares e inspecciones con IA", "13b_biblioteca_peligros.png", [
    ["Biblioteca precargada", "17 peligros típicos de un EPC solar listos para agregar a la matriz."],
    ["Inspección con IA", "A partir de fotos y descripción, propone peligros y controles."],
    ["Mapa y dashboard SST", "Riesgos por área, nivel y estado de control."],
  ], "Biblioteca de peligros basada en GTC 45. La inspección con IA sugiere peligros y controles para la matriz IPERV.");

  // ================= 14. CALIDAD · AMBIENTAL · SOCIAL =================
  {
    const s = pres.addSlide();
    chrome(s, 14, "HSEQ: Calidad, Ambiental y Social", "Tres tableros con los mismos indicadores que piden las auditorías ISO");
    const items = [["14_hseq_calidad.png", "Calidad · ISO 9001", "PPIs, calibración, no conformidades y acciones correctivas"],
      ["15_hseq_ambiental.png", "Ambiental · ISO 14001", "Residuos, derrames, aspectos e impactos, fauna y flora"],
      ["16_hseq_social.png", "Social", "Comunidades, PQR, compromisos, contratación y compras locales"]];
    items.forEach((it, i) => {
      const x = 0.5 + i * 4.16, y = 1.45, w = 4.0, h = 2.75;
      framedImage(s, shot(it[0]), x, y, w, h);
      s.addText([{ text: it[1], options: { bold: true, color: BLUE, fontSize: 11, breakLine: true } }, { text: it[2], options: { color: TXT, fontSize: 9 } }],
        { x, y: y + h + 0.2, w, h: 0.9, fontFace: FONT, margin: 0, isTextBox: true, valign: "top", paraSpaceAfter: 3 });
    });
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x: 0.5, y: 5.45, w: 12.33, h: 1.1, rectRadius: 0.12, fill: { color: LIGHT }, line: { color: LIGHT, width: 0 } });
    s.addImage({ data: ICONS.FiAward_b, x: 0.85, y: 5.75, w: 0.5, h: 0.5 });
    s.addText([{ text: "Auditorías HSEQ corporativas: ", options: { bold: true, color: BLUE } }, { text: "ISO 9001, ISO 14001, ISO 45001, Decreto 1072 y Resolución 0312 con un motor de auditoría único y consolidado anual.", options: { color: TXT } }],
      { x: 1.55, y: 5.45, w: 11.0, h: 1.1, fontFace: FONT, fontSize: 11, margin: 0, isTextBox: true, valign: "middle" });
    s.addNotes("Calidad, Ambiental y Social comparten la misma lógica: indicadores, registros y acciones. Las auditorías corporativas se hacen desde el módulo HSEQ global.");
  }

  moduleSlide(15, "Alertas y control de documentos", "Vencimientos de personal, vehículos y proveedores con semáforo", "17_alertas.png", [
    ["Matriz de cumplimiento", "Pólizas, SOAT, revisión técnica, licencias y exámenes por recurso."],
    ["Semáforo de vencimientos", "Vigentes, por vencer en 30, 20 y 10 días, y vencidos."],
    ["Importa el Excel actual", "El control de ingreso se carga tal como está hoy."],
  ], "Alertas: 70 documentos vigentes, 56 vencidos. Cada recurso muestra en color qué le falta para poder ingresar a obra.");

  // ================= 16. CLIMA =================
  {
    const s = pres.addSlide();
    chrome(s, 16, "Clima operacional", "Pronóstico del sitio y alerta por actividad del cronograma");
    framedImage(s, shot("18_crop.png"), 0.5, 1.42, 6.0, 3.76);
    framedImage(s, shot("18b_crop.png"), 6.83, 1.42, 6.0, 3.76);
    s.addText([{ text: "Pronóstico a 16 días", options: { bold: true, color: BLUE, fontSize: 11, breakLine: true } }, { text: "Condiciones actuales, lluvia y viento con la columna “Apto obra” para cada día.", options: { color: TXT, fontSize: 9.5 } }],
      { x: 0.5, y: 5.35, w: 6.0, h: 1.2, fontFace: FONT, margin: 0, isTextBox: true, valign: "top", paraSpaceAfter: 3 });
    s.addText([{ text: "Alerta meteorológica operacional", options: { bold: true, color: BLUE, fontSize: 11, breakLine: true } }, { text: "Cruza el pronóstico con las actividades en curso y recomienda qué hacer con cada una.", options: { color: TXT, fontSize: 9.5 } }],
      { x: 6.83, y: 5.35, w: 6.0, h: 1.2, fontFace: FONT, margin: 0, isTextBox: true, valign: "top", paraSpaceAfter: 3 });
    s.addNotes("Clima: se consulta el pronóstico del sitio (Cereté) y el sistema marca qué actividades del cronograma se ven afectadas, con una recomendación por actividad.");
  }

  // ================= 17. WHATSAPP =================
  {
    const s = pres.addSlide();
    chrome(s, 17, "Canal de WhatsApp", "Consultas desde el celular con un bot de menú, sin abrir la plataforma");
    // teléfono
    const px = 1.3, py = 1.3, pw = 3.2, ph = 5.35;
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x: px, y: py, w: pw, h: ph, rectRadius: 0.35, fill: { color: DARK }, line: { color: DARK, width: 0 }, shadow: { type: "outer", blur: 10, offset: 3, angle: 90, color: "000000", opacity: 0.3 } });
    s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x: px + 0.12, y: py + 0.12, w: pw - 0.24, h: ph - 0.24, rectRadius: 0.28, fill: { color: "ECE5DD" }, line: { color: "ECE5DD", width: 0 } });
    s.addShape(pres.shapes.RECTANGLE, { x: px + 0.12, y: py + 0.12, w: pw - 0.24, h: 0.6, fill: { color: "075E54" }, line: { color: "075E54", width: 0 } });
    s.addText("RenergeIA", { x: px + 0.35, y: py + 0.12, w: 2.4, h: 0.6, fontFace: FONT, fontSize: 10, bold: true, color: WHITE, margin: 0, isTextBox: true, valign: "middle" });
    const bubbles = [
      ["in", "Hola, soy el asistente de RenergeIA. ¿Qué quieres consultar?\n1. Avance del proyecto\n2. Costos\n3. Documentos pendientes\n4. Clima de hoy"],
      ["out", "1"],
      ["in", "La Soberana · 26/09\nAvance real: 56,2 %\nProgramado: 63,8 %\nDesviación: -7,6 %"],
      ["out", "4"],
      ["in", "Hoy: tormenta eléctrica, 29°/24°, lluvia 82 %. Recomendación: limitar trabajo en campo."],
    ];
    let by = py + 0.9;
    bubbles.forEach((b) => {
      const isIn = b[0] === "in";
      const lines = b[1].split("\n").length;
      const bh = 0.22 + lines * 0.19;
      const bw = isIn ? 2.3 : 0.7;
      const bx = isIn ? px + 0.28 : px + pw - 0.28 - bw;
      s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x: bx, y: by, w: bw, h: bh, rectRadius: 0.1, fill: { color: isIn ? WHITE : "DCF8C6" }, line: { color: isIn ? WHITE : "DCF8C6", width: 0 } });
      s.addText(b[1], { x: bx + 0.1, y: by + 0.05, w: bw - 0.2, h: bh - 0.1, fontFace: FONT, fontSize: 7, color: TXT, margin: 0, isTextBox: true, valign: "middle" });
      by += bh + 0.12;
    });
    // puntos a la derecha
    const pts = [
      ["FiSmartphone", "Bot de menú determinista", "Responde con datos del sistema; no inventa. Ideal para gerencia y residentes en campo."],
      ["FiZap", "Conexión directa con Meta", "Sin intermediarios: solo se paga la tarifa por conversación de WhatsApp."],
      ["FiClock", "Listo para crecer", "Hoy consulta avance, costos, documentos y clima. Después: alertas automáticas."],
    ];
    pts.forEach((p, i) => {
      const x = 5.3, y = 1.5 + i * 1.7, w = 7.5, h = 1.45;
      s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x, y, w, h, rectRadius: 0.1, fill: { color: LIGHT }, line: { color: LIGHT, width: 0 } });
      s.addShape(pres.shapes.OVAL, { x: x + 0.25, y: y + 0.4, w: 0.65, h: 0.65, fill: { color: GREEN }, line: { color: GREEN, width: 0 } });
      s.addImage({ data: ICONS[p[0] + "_w"], x: x + 0.41, y: y + 0.56, w: 0.33, h: 0.33 });
      s.addText([{ text: p[1], options: { bold: true, color: BLUE, fontSize: 12, breakLine: true } }, { text: p[2], options: { color: TXT, fontSize: 10 } }],
        { x: x + 1.15, y: y + 0.15, w: w - 1.35, h: h - 0.3, fontFace: FONT, margin: 0, isTextBox: true, valign: "middle", paraSpaceAfter: 3 });
    });
    s.addNotes("Canal de WhatsApp conectado directo con la API de Meta. Es un bot de menú: responde con datos del sistema. Se muestra como ejemplo de conversación.");
  }

  // ================= 18. BENEFICIOS =================
  {
    const s = pres.addSlide();
    chrome(s, 18, "Qué gana la empresa", "Una sola fuente de información para decidir con datos al día");
    const b = [
      ["FiTrendingUp", "Decisiones con datos al día", "Avance, costos y HSEQ del proyecto en tiempo real, sin esperar el consolidado."],
      ["FiRepeat", "Menos reprocesos", "El ejecutado, las OC y la nómina se cargan una sola vez desde tesorería."],
      ["FiShield", "Cumplimiento con alertas", "Documentos, pólizas y planes HSE con semáforo antes de que se venzan."],
      ["FiLayers", "El mismo estándar en todos los proyectos", "Plantilla EPC, codificación de costos y formatos únicos para la empresa."],
    ];
    b.forEach((it, i) => {
      const c = i % 2, r = Math.floor(i / 2);
      const x = 0.5 + c * 6.28, y = 1.5 + r * 2.5, w = 6.05, h = 2.25;
      s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x, y, w, h, rectRadius: 0.12, fill: { color: WHITE }, line: { color: GRAY, width: 1 }, shadow: { type: "outer", blur: 6, offset: 2, angle: 90, color: "000000", opacity: 0.12 } });
      s.addShape(pres.shapes.OVAL, { x: x + 0.35, y: y + 0.7, w: 0.85, h: 0.85, fill: { color: i % 2 === 0 ? BLUE : GREEN }, line: { color: i % 2 === 0 ? BLUE : GREEN, width: 0 } });
      s.addImage({ data: ICONS[it[0] + "_w"], x: x + 0.56, y: y + 0.91, w: 0.43, h: 0.43 });
      s.addText([{ text: it[1], options: { bold: true, color: BLUE, fontSize: 13, breakLine: true } }, { text: it[2], options: { color: TXT, fontSize: 10.5 } }],
        { x: x + 1.5, y: y + 0.25, w: w - 1.8, h: h - 0.5, fontFace: FONT, margin: 0, isTextBox: true, valign: "middle", paraSpaceAfter: 4 });
    });
    s.addNotes("Beneficios: información al día, menos digitación, cumplimiento con alertas y un estándar único para todos los proyectos.");
  }

  // ================= 19. PROPUESTA EN DOS ETAPAS =================
  {
    const s = pres.addSlide();
    chrome(s, 19, "Propuesta: adopción en dos etapas", "Empezar con La Soberana y extender a todos los proyectos");
    // etapa 1
    const stages = [
      [BLUE, "1", "Piloto en La Soberana", "Octubre a diciembre 2026", [
        "Datos reales ya cargados: cronograma, costos, nómina y documentos",
        "Uso semanal por el equipo del proyecto: informe diario, tesorería y HSEQ",
        "Revisión mensual de resultados con gerencia",
      ]],
      [GREEN, "2", "Todos los proyectos", "Desde enero 2027", [
        "Villanueva y los proyectos nuevos arrancan en la plataforma",
        "Plantilla EPC y codificación de costos como estándar de la empresa",
        "Usuarios y roles por proyecto; WhatsApp y notificaciones por correo",
      ]],
    ];
    stages.forEach((st, i) => {
      const x = 0.5 + i * 6.45, y = 1.5, w = 5.9, h = 4.85;
      s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x, y, w, h, rectRadius: 0.14, fill: { color: WHITE }, line: { color: GRAY, width: 1 }, shadow: { type: "outer", blur: 6, offset: 2, angle: 90, color: "000000", opacity: 0.12 } });
      s.addShape(pres.shapes.OVAL, { x: x + 0.35, y: y + 0.35, w: 0.9, h: 0.9, fill: { color: st[0] }, line: { color: st[0], width: 0 } });
      s.addText(st[1], { x: x + 0.35, y: y + 0.35, w: 0.9, h: 0.9, fontFace: FONT, fontSize: 24, bold: true, color: WHITE, align: "center", valign: "middle", margin: 0, isTextBox: true });
      s.addText(st[2], { x: x + 1.45, y: y + 0.35, w: w - 1.7, h: 0.5, fontFace: FONT, fontSize: 15, bold: true, color: BLUE, margin: 0, isTextBox: true, valign: "middle" });
      s.addText(st[3], { x: x + 1.45, y: y + 0.85, w: w - 1.7, h: 0.4, fontFace: FONT, fontSize: 10, color: MUTED, margin: 0, isTextBox: true, valign: "middle" });
      st[4].forEach((t, j) => {
        const ly = y + 1.6 + j * 1.0;
        s.addShape(pres.shapes.OVAL, { x: x + 0.45, y: ly + 0.12, w: 0.34, h: 0.34, fill: { color: st[0] }, line: { color: st[0], width: 0 } });
        s.addImage({ data: ICONS.FiCheck_w, x: x + 0.52, y: ly + 0.19, w: 0.2, h: 0.2 });
        s.addText(t, { x: x + 1.0, y: ly, w: w - 1.35, h: 0.6, fontFace: FONT, fontSize: 10, color: TXT, margin: 0, isTextBox: true, valign: "middle" });
      });
    });
    s.addImage({ data: ICONS.FiArrowRight_g, x: 6.42, y: 3.6, w: 0.5, h: 0.5 });
    s.addNotes("Propuesta: piloto en La Soberana durante el último trimestre de 2026, con revisión mensual, y adopción en todos los proyectos desde enero de 2027.");
  }

  // ================= 20. CIERRE =================
  {
    const s = pres.addSlide();
    s.background = { color: BLUE };
    s.addImage({ path: logoWhite, x: 0.7, y: 0.6, w: 2.1, h: 0.93 });
    s.addText("Lo que pedimos a gerencia", { x: 0.7, y: 1.85, w: 11.9, h: 0.8, fontFace: FONT, fontSize: 30, bold: true, color: WHITE, margin: 0, isTextBox: true });
    const asks = [
      ["FiPlayCircle", "Aprobar el piloto en La Soberana", "Uso oficial de la plataforma por el equipo del proyecto desde octubre."],
      ["FiUsers", "Designar responsables por módulo", "Costos, cronograma, documentos y HSEQ con un dueño de la información."],
      ["FiFlag", "Fijar la fecha de decisión", "Revisar resultados en diciembre para extenderla a todos los proyectos."],
    ];
    asks.forEach((a, i) => {
      const x = 0.7 + i * 4.05, y = 3.0, w = 3.85, h = 2.6;
      s.addShape(pres.shapes.ROUNDED_RECTANGLE, { x, y, w, h, rectRadius: 0.14, fill: { color: "0E2340" }, line: { color: "2B4A75", width: 1 } });
      s.addShape(pres.shapes.OVAL, { x: x + 0.3, y: y + 0.3, w: 0.7, h: 0.7, fill: { color: GREEN }, line: { color: GREEN, width: 0 } });
      s.addImage({ data: ICONS[a[0] + "_w"], x: x + 0.47, y: y + 0.47, w: 0.36, h: 0.36 });
      s.addText([{ text: a[1], options: { bold: true, color: WHITE, fontSize: 12.5, breakLine: true } }, { text: a[2], options: { color: GRAY, fontSize: 10 } }],
        { x: x + 0.3, y: y + 1.15, w: w - 0.6, h: h - 1.3, fontFace: FONT, margin: 0, isTextBox: true, valign: "top", paraSpaceAfter: 4 });
    });
    s.addText("Gracias", { x: 0.7, y: 6.1, w: 5, h: 0.6, fontFace: FONT, fontSize: 20, bold: true, color: GREEN, margin: 0, isTextBox: true });
    s.addText("Plataforma para la gestión de proyectos", { x: 7.6, y: 6.25, w: 5.0, h: 0.35, fontFace: FONT, fontSize: 9, color: GRAY, align: "right", margin: 0, isTextBox: true });
    s.addNotes("Cierre: se pide aprobar el piloto, nombrar responsables por módulo y fijar la fecha de revisión para decidir la adopción general.");
  }

  await pres.writeFile({ fileName: OUT });
  console.log("OK", OUT);
})().catch((e) => { console.error(e); process.exit(1); });
