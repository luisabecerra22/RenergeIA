// Genera los PDF con colores de marca a partir de los Markdown de docs/:
//   - Manual general (docs/manual-general)
//   - Instructivo de Helvis (docs/instructivo-helvis)
//   - Manual de usuario completo (docs/manual, README + capítulos 00-19)
// Uso: node docs/build-pdf/build.js   (requiere npm i en esta carpeta y Google Chrome instalado)
const fs = require('fs');
const path = require('path');
const { marked } = require('marked');
const puppeteer = require('puppeteer-core');

const DOCS = path.resolve(__dirname, '..');
const RAIZ = path.resolve(DOCS, '..');
const CHROME = 'C:/Program Files/Google/Chrome/Application/chrome.exe';
const LOGO = 'data:image/png;base64,' +
  fs.readFileSync(path.join(RAIZ, 'RenergeIA.Web/wwwroot/images/logo-renergeia.png.png')).toString('base64');

const AZUL = '#183963', VERDE = '#6ABF4B', GRIS = '#D9D9D6', OSCURO = '#111921';

const CSS = `
@import url('https://fonts.googleapis.com/css2?family=Montserrat:wght@400;500;600;700;800&display=swap');
* { box-sizing: border-box; }
body { font-family: Montserrat, Verdana, sans-serif; color: ${OSCURO}; font-size: 10pt; line-height: 1.5; margin: 0; }
.portada { height: 257mm; display: flex; flex-direction: column; page-break-after: always; }
.portada .arriba { flex: 1; display: flex; align-items: center; justify-content: center; }
.portada .arriba img { width: 105mm; }
.portada .banda { background: ${AZUL}; color: #fff; padding: 18mm 16mm; border-top: 3mm solid ${VERDE}; border-radius: 0 0 4mm 4mm; }
.portada .tipo { color: ${VERDE}; font-weight: 700; letter-spacing: .18em; text-transform: uppercase; font-size: 10pt; }
.portada h1 { color: #fff; font-size: 27pt; line-height: 1.15; margin: 4mm 0 5mm; border: 0; padding: 0; }
.portada .sub { font-size: 11.5pt; opacity: .92; max-width: 150mm; }
.portada .meta { margin-top: 10mm; font-size: 9pt; opacity: .85; }
h1 { color: ${AZUL}; font-size: 19pt; font-weight: 800; border-bottom: 1.2mm solid ${VERDE}; padding-bottom: 2mm; margin: 0 0 5mm; }
h1.cap { page-break-before: always; }
h2 { color: ${AZUL}; font-size: 13.5pt; font-weight: 700; margin: 7mm 0 2.5mm; page-break-after: avoid; }
h2::before { content: ''; display: inline-block; width: 2.2mm; height: 4.2mm; background: ${VERDE}; margin-right: 2.5mm; vertical-align: -0.6mm; border-radius: .6mm; }
h3 { color: ${AZUL}; font-size: 11pt; font-weight: 700; margin: 5mm 0 2mm; page-break-after: avoid; }
h4 { color: ${OSCURO}; font-size: 10pt; margin: 4mm 0 1.5mm; }
p, li { orphans: 3; widows: 3; }
a { color: ${AZUL}; text-decoration: none; border-bottom: .2mm dotted ${AZUL}; }
strong { color: ${AZUL}; }
code { font-family: Consolas, monospace; font-size: 8.6pt; background: #f1f3f6; padding: .2mm 1mm; border-radius: .8mm; }
pre { background: #f1f3f6; padding: 3mm; border-radius: 1.5mm; white-space: pre-wrap; word-break: break-all; font-size: 8pt; }
pre code { background: none; padding: 0; }
blockquote { margin: 3mm 0 4mm; padding: 3mm 4mm; background: #eef5ea; border-left: 1.4mm solid ${VERDE}; border-radius: 0 1.5mm 1.5mm 0; }
blockquote p { margin: 1mm 0; }
table { width: 100%; border-collapse: collapse; margin: 3mm 0 4mm; font-size: 8.6pt; page-break-inside: auto; }
tr { page-break-inside: avoid; }
thead th { background: ${AZUL}; color: #fff; font-weight: 600; text-align: left; padding: 1.8mm 2.2mm; }
thead th strong { color: #fff; }
td { padding: 1.6mm 2.2mm; border-bottom: .3mm solid ${GRIS}; vertical-align: top; }
tbody tr:nth-child(even) td { background: #f6f7f8; }
hr { border: 0; border-top: .3mm solid ${GRIS}; margin: 6mm 0; }
ul, ol { padding-left: 6mm; }
.indice { page-break-after: always; }
.indice ol { list-style: none; padding: 0; columns: 1; }
.indice ol li { display: flex; gap: 3mm; padding: 2mm 0; border-bottom: .3mm solid ${GRIS}; font-size: 10.5pt; }
.indice ol li span.n { color: #fff; background: ${VERDE}; font-weight: 700; min-width: 8mm; text-align: center; border-radius: 1mm; }
.indice ol li a { border: 0; font-weight: 600; }
`;

const FOOTER = (titulo) => `
<div style="width:100%;font-family:Verdana,sans-serif;font-size:7.5pt;color:#555;padding:0 15mm;display:flex;justify-content:space-between;align-items:center">
  <span style="color:${AZUL};font-weight:bold">RenergeIA</span>
  <span>${titulo}</span>
  <span>Página <span class="pageNumber"></span> de <span class="totalPages"></span></span>
</div>`;
const HEADER = `<div style="width:100%;margin:0 15mm;height:1.2mm;background:${VERDE}"></div>`;

function portada({ tipo, titulo, sub, version }) {
  return `<section class="portada">
    <div class="arriba"><img src="${LOGO}" alt="Renergeia"></div>
    <div class="banda">
      <div class="tipo">${tipo}</div>
      <h1>${titulo}</h1>
      <div class="sub">${sub}</div>
      <div class="meta">${version}<br>Plataforma para la gestión de proyectos · Renergeia S.A.S.</div>
    </div>
  </section>`;
}

// Convierte enlaces entre capítulos (05-costos.md) en anclas internas (#cap-05)
function enlacesInternos(md) {
  return md.replace(/\]\((\d{2})-[^)]+\.md\)/g, '](#cap-$1)');
}

async function imprimir(html, salida, titulo) {
  const navegador = await puppeteer.launch({ executablePath: CHROME, headless: true });
  const pagina = await navegador.newPage();
  await pagina.setContent(html, { waitUntil: 'networkidle0', timeout: 60000 });
  await pagina.pdf({
    path: salida, format: 'A4', printBackground: true,
    displayHeaderFooter: true, headerTemplate: HEADER, footerTemplate: FOOTER(titulo),
    margin: { top: '16mm', bottom: '16mm', left: '15mm', right: '15mm' },
  });
  await navegador.close();
  console.log('PDF generado:', path.relative(RAIZ, salida));
}

function documento(cuerpo, titulo) {
  return `<!doctype html><html lang="es"><head><meta charset="utf-8"><title>${titulo}</title><style>${CSS}</style></head><body>${cuerpo}</body></html>`;
}

// Quita el H1 y el bloque de cita inicial (ya van en la portada)
function sinEncabezado(md) {
  return md.replace(/^# .*\n+/, '').replace(/^(> .*\n)+\n*/, '');
}

async function documentoSimple({ md, salida, tipo, titulo, sub, version }) {
  const texto = fs.readFileSync(md, 'utf8');
  const intro = (texto.match(/^(> .*\n)+/m) || [''])[0];
  // cada línea de la cita inicial como párrafo propio
  const cuerpo = portada({ tipo, titulo, sub, version }) +
    marked.parse(intro.replace(/\n> /g, '\n>\n> ')) + marked.parse(sinEncabezado(texto));
  const html = documento(cuerpo, titulo);
  await imprimir(html, salida, titulo);
}

async function manualUsuario() {
  const dir = path.join(DOCS, 'manual');
  const capitulos = fs.readdirSync(dir).filter(f => /^\d{2}-.*\.md$/.test(f)).sort();
  const indice = capitulos.map(f => {
    const n = f.slice(0, 2);
    const t = fs.readFileSync(path.join(dir, f), 'utf8').match(/^# (.*)$/m)[1].replace(/^\d+\.\s*/, '');
    return `<li><span class="n">${Number(n)}</span><a href="#cap-${n}">${t}</a></li>`;
  }).join('');
  const readme = fs.readFileSync(path.join(dir, 'README.md'), 'utf8');
  const comoUsar = (readme.match(/## Cómo usar este manual[\s\S]*?(?=\n---)/) || [''])[0];

  let cuerpo = portada({
    tipo: 'Manual de usuario',
    titulo: 'RenergeIA — Manual de usuario',
    sub: 'Paso a paso de cada módulo de la plataforma: para qué sirve, cómo funciona, qué hacer en caso de problemas y buenas prácticas.',
    version: 'Actualizado al 28 de octubre de 2026',
  });
  cuerpo += `<section class="indice"><h1>Contenido</h1><ol>${indice}</ol>${marked.parse(comoUsar)}</section>`;
  for (const f of capitulos) {
    const n = f.slice(0, 2);
    const html = marked.parse(enlacesInternos(fs.readFileSync(path.join(dir, f), 'utf8')));
    cuerpo += html.replace(/^<h1>/, `<h1 class="cap" id="cap-${n}">`);
  }
  const salida = path.join(dir, 'Manual-Usuario-RenergeIA.pdf');
  await imprimir(documento(cuerpo, 'Manual de usuario'), salida, 'Manual de usuario');
}

(async () => {
  const solo = process.argv[2];
  if (!solo || solo === 'general') await documentoSimple({
    md: path.join(DOCS, 'manual-general/Manual-General-RenergeIA.md'),
    salida: path.join(DOCS, 'manual-general/Manual-General-RenergeIA.pdf'),
    tipo: 'Manual general', titulo: 'RenergeIA — Manual general de la plataforma',
    sub: 'Qué es, qué módulos tiene, de dónde sale la información, qué indicadores entrega y hacia dónde va.',
    version: 'Versión 2.0 · 28 de octubre de 2026',
  });
  if (!solo || solo === 'helvis') await documentoSimple({
    md: path.join(DOCS, 'instructivo-helvis/Instructivo-Helvis-WhatsApp.md'),
    salida: path.join(DOCS, 'instructivo-helvis/Instructivo-Helvis-WhatsApp.pdf'),
    tipo: 'Instructivo', titulo: 'Helvis — asistente de WhatsApp',
    sub: 'Qué es, quién puede usarlo, cómo se administra y qué hacer si algo falla, incluidos los problemas con Meta.',
    version: 'Versión 1.1 · 29 de octubre de 2026',
  });
  if (!solo || solo === 'usuario') await manualUsuario();
})().catch(e => { console.error(e); process.exit(1); });
