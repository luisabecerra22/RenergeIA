// Pasa los manuales en Markdown a la plantilla oficial de manuales (MA-SG-GC-001-A, plantilla FO-PP-CD-002-0)
// sin cambiar su formato: portada, encabezados, pie con control de cambios, estilos de títulos, viñetas y tablas.
//   - Manual general            (docs/manual-general)
//   - Manual de usuario         (docs/manual, capítulos 00-19)
//   - Instructivo de Helvis     (docs/instructivo-helvis)
//   - Puntos por confirmar      (lista aparte con las notas "⚠ Por confirmar" que se retiran de los manuales)
// Lo que debe completar control documental (código, proceso, custodio, firmas) queda resaltado en amarillo.
// Uso: node docs/build-word/build.js [ruta de la plantilla .docx]   (requiere npm i en esta carpeta)
const fs = require('fs');
const path = require('path');
const JSZip = require('jszip');
const { marked } = require('marked');

const DOCS = path.resolve(__dirname, '..');
const SALIDA = path.join(DOCS, 'word');
const PLANTILLA = (process.argv[2] !== '--indice' && process.argv[2]) || 'C:/Users/Luisa Becerra/Downloads/MA-SG-GC-001-A.docx';
const FECHA = (() => { const d = new Date(Date.now() - 5 * 3600e3); return d.toISOString().slice(0, 10).replace(/-/g, '/'); })();

// ---------------- utilidades XML ----------------
const esc = t => t.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
const desc = t => t.replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&quot;/g, '"').replace(/&#39;/g, "'").replace(/&amp;/g, '&');
const AMARILLO = '<w:highlight w:val="yellow"/>';
const FUENTE = '<w:rFonts w:ascii="Verdana" w:eastAsia="Verdana" w:hAnsi="Verdana" w:cs="Verdana"/>';

function run(texto, { b, i, color, sz, highlight } = {}) {
  const rpr = FUENTE + (b ? '<w:b/><w:bCs/>' : '') + (i ? '<w:i/><w:iCs/>' : '') +
    `<w:color w:val="${color || '111921'}"/>` + (sz ? `<w:sz w:val="${sz}"/><w:szCs w:val="${sz}"/>` : '') +
    (highlight ? AMARILLO : '');
  return `<w:r><w:rPr>${rpr}</w:rPr><w:t xml:space="preserve">${esc(texto)}</w:t></w:r>`;
}

// tokens en línea de marked → runs
function inline(tokens, est = {}) {
  let x = '';
  for (const t of tokens || []) {
    switch (t.type) {
      case 'strong': x += inline(t.tokens, { ...est, b: true, color: est.color || '183963' }); break;
      case 'em': x += inline(t.tokens, { ...est, i: true }); break;
      case 'codespan': x += run(desc(t.text), { ...est, color: '183963' }); break;
      case 'link': x += inline(t.tokens, est); break;
      case 'br': x += '<w:r><w:br/></w:r>'; break;
      case 'text': case 'escape': x += t.tokens ? inline(t.tokens, est) : run(desc(t.text), est); break;
      case 'del': x += inline(t.tokens, est); break;
      case 'html': break;
      default: if (t.text) x += run(desc(t.text), est);
    }
  }
  return x;
}

const P_TEXTO = '<w:pPr><w:spacing w:after="120" w:line="276" w:lineRule="auto"/><w:jc w:val="both"/></w:pPr>';
const parrafo = cont => `<w:p>${P_TEXTO}${cont}</w:p>`;
const titulo = (nivel, texto) =>
  `<w:p><w:pPr><w:pStyle w:val="Ttulo${nivel}"/></w:pPr><w:r><w:t xml:space="preserve">${esc(texto.replace(/^\d+(\.\d+)*\.?\s+/, '').trim().toLocaleUpperCase('es'))}</w:t></w:r></w:p>`;

// numeraciones nuevas (listas numeradas que reinician) que se agregan a numbering.xml
let numerosNuevos = [];
let siguienteNumId = 900;
function nuevaNumeracion() {
  const id = siguienteNumId++;
  numerosNuevos.push(`<w:num w:numId="${id}"><w:abstractNumId w:val="9"/><w:lvlOverride w:ilvl="0"><w:startOverride w:val="1"/></w:lvlOverride></w:num>`);
  return id;
}

function lista(tok, nivel = 0) {
  const numId = tok.ordered ? nuevaNumeracion() : 10; // 10 = viñeta ● / ○ de la plantilla
  let x = '';
  for (const item of tok.items) {
    const partes = [];
    const sub = [];
    for (const t of item.tokens) {
      if (t.type === 'list') sub.push(t);
      else if (t.type === 'text' || t.type === 'paragraph') partes.push(inline(t.tokens || [{ type: 'text', text: t.text }], { sz: 22 }));
    }
    x += `<w:p><w:pPr><w:pStyle w:val="Prrafodelista"/><w:numPr><w:ilvl w:val="${nivel}"/><w:numId w:val="${numId}"/></w:numPr>` +
      `<w:spacing w:after="80" w:line="276" w:lineRule="auto"/><w:jc w:val="both"/></w:pPr>${partes.join(run(' ', { sz: 22 }))}</w:p>`;
    for (const s of sub) x += lista(s, nivel + 1);
  }
  return x;
}

// Tabla con el formato de la plantilla: encabezado azul #183963 en blanco y negrita, cuerpo gris #D9D9D6, bordes blancos
function tabla(tok) {
  const ANCHO = 9356;
  const cols = tok.header.length;
  const largo = tok.header.map((h, c) => Math.max(h.text.length, ...tok.rows.map(r => (r[c] ? r[c].text.length : 0))));
  const peso = largo.map((l, c) => Math.min(Math.max(l, 10, tok.header[c].text.length + 4), 70));
  const suma = peso.reduce((a, b) => a + b, 0);
  const anchos = peso.map(p => Math.round(ANCHO * p / suma));
  const borde = '<w:tblBorders>' + ['top', 'left', 'bottom', 'right', 'insideH', 'insideV']
    .map(b => `<w:${b} w:val="single" w:sz="4" w:space="0" w:color="FFFFFF"/>`).join('') + '</w:tblBorders>';
  const celda = (cont, ancho, fondo, enc) =>
    `<w:tc><w:tcPr><w:tcW w:w="${ancho}" w:type="dxa"/><w:shd w:val="clear" w:color="auto" w:fill="${fondo}"/><w:vAlign w:val="center"/></w:tcPr>` +
    `<w:p><w:pPr><w:spacing w:before="40" w:after="40" w:line="240" w:lineRule="auto"/></w:pPr>${enc
      ? inline(cont.tokens, { b: true, color: 'FFFFFF', sz: 18 })
      : inline(cont.tokens, { sz: 18 })}</w:p></w:tc>`;
  let x = `<w:tbl><w:tblPr><w:tblW w:w="${ANCHO}" w:type="dxa"/><w:tblInd w:w="-5" w:type="dxa"/>${borde}` +
    `<w:tblLayout w:type="fixed"/><w:tblLook w:val="04A0" w:firstRow="1" w:lastRow="0" w:firstColumn="1" w:lastColumn="0" w:noHBand="0" w:noVBand="1"/></w:tblPr>` +
    `<w:tblGrid>${anchos.map(a => `<w:gridCol w:w="${a}"/>`).join('')}</w:tblGrid>`;
  x += `<w:tr><w:trPr><w:tblHeader/><w:cantSplit/></w:trPr>${tok.header.map((h, c) => celda(h, anchos[c], '183963', true)).join('')}</w:tr>`;
  for (const r of tok.rows)
    x += `<w:tr><w:trPr><w:cantSplit/></w:trPr>${Array.from({ length: cols }, (_, c) => celda(r[c] || { tokens: [] }, anchos[c], 'D9D9D6', false)).join('')}</w:tr>`;
  return x + '</w:tbl>' + parrafo('');
}

// Cita "> **Etiqueta:** texto" → tabla de dos columnas como la de ALCANCE de la plantilla; otra cita → párrafo en cursiva
function cita(tok) {
  const parrafos = tok.tokens.filter(t => t.type === 'paragraph');
  const filas = [];
  for (const p of parrafos) {
    // cada línea "**Etiqueta:** texto" es una fila
    const lineas = p.raw.split('\n').map(l => l.trim()).filter(Boolean);
    for (const l of lineas) {
      const m = /^\*\*([^*]+?):?\*\*:?\s*(.*)$/.exec(l);
      if (m) filas.push([m[1].replace(/:$/, ''), m[2]]);
      else filas.push(null);
    }
  }
  if (filas.length && filas.every(Boolean)) {
    const izq = 2496, der = 6860;
    let x = `<w:tbl><w:tblPr><w:tblW w:w="9356" w:type="dxa"/><w:tblInd w:w="-5" w:type="dxa"/><w:tblBorders>` +
      ['top', 'left', 'bottom', 'right', 'insideH', 'insideV'].map(b => `<w:${b} w:val="single" w:sz="4" w:space="0" w:color="FFFFFF"/>`).join('') +
      `</w:tblBorders><w:tblLayout w:type="fixed"/></w:tblPr><w:tblGrid><w:gridCol w:w="${izq}"/><w:gridCol w:w="${der}"/></w:tblGrid>`;
    for (const [etq, txt] of filas) {
      const toks = marked.lexer(txt)[0];
      x += `<w:tr><w:trPr><w:cantSplit/></w:trPr><w:tc><w:tcPr><w:tcW w:w="${izq}" w:type="dxa"/><w:shd w:val="clear" w:color="auto" w:fill="183963"/><w:vAlign w:val="center"/></w:tcPr>` +
        `<w:p><w:pPr><w:spacing w:before="60" w:after="60"/></w:pPr>${run(etq.toUpperCase(), { b: true, color: 'FFFFFF', sz: 20 })}</w:p></w:tc>` +
        `<w:tc><w:tcPr><w:tcW w:w="${der}" w:type="dxa"/><w:shd w:val="clear" w:color="auto" w:fill="D9D9D6"/><w:vAlign w:val="center"/></w:tcPr>` +
        `<w:p><w:pPr><w:spacing w:before="60" w:after="60" w:line="276" w:lineRule="auto"/></w:pPr>${inline(toks ? toks.tokens : [], { sz: 20 })}</w:p></w:tc></w:tr>`;
    }
    return x + '</w:tbl>' + parrafo('');
  }
  return parrafos.map(p => `<w:p><w:pPr><w:spacing w:after="120" w:line="276" w:lineRule="auto"/><w:ind w:left="426"/><w:jc w:val="both"/></w:pPr>${inline(p.tokens, { i: true })}</w:p>`).join('');
}

// Markdown → cuerpo WordprocessingML. desplazamiento: niveles de título (# → Ttulo1 + desplazamiento)
function cuerpo(md, desplazamiento = 0) {
  let x = '';
  for (const t of marked.lexer(md)) {
    switch (t.type) {
      case 'heading': {
        const nivel = Math.min(t.depth + desplazamiento, 4);
        x += nivel <= 3 ? titulo(nivel, desc(t.text)) : parrafo(inline(t.tokens, { b: true, color: '183963' }));
        break;
      }
      case 'paragraph': x += parrafo(inline(t.tokens)); break;
      case 'list': x += lista(t); break;
      case 'table': x += tabla(t); break;
      case 'blockquote': x += cita(t); break;
      case 'code': x += t.text.split('\n').map(l => parrafo(run(l, { color: '183963', sz: 18 }))).join(''); break;
      default: break; // hr, espacios
    }
  }
  return x;
}

// ---------------- limpieza del Markdown ----------------
const porConfirmar = [];
function quitarPorConfirmar(md, origen) {
  const lineas = md.split(/\r?\n/);
  const fuera = [];
  for (let i = 0; i < lineas.length; i++) {
    const l = lineas[i];
    // en una fila de tabla solo se retira la nota, para no perder el resto de la fila
    if (/Por confirmar/i.test(l) && /^\s*\|/.test(l)) {
      const nota = /(⚠\s*)?\**Por confirmar[^|]*/i.exec(l)[0];
      porConfirmar.push({ origen, texto: l.split('|').map(c => c.trim()).filter(Boolean)[0] + ' — ' + nota.replace(/⚠\s*/g, '').replace(/\*\*/g, '').trim() });
      fuera.push(l.replace(nota, ' '));
      continue;
    }
    if (/Por confirmar/i.test(l) && !/^#/.test(l)) {
      porConfirmar.push({ origen, texto: l.replace(/^\s*(>|-|\*|\d+\.)\s*/, '').replace(/^\|\s*|\s*\|$/g, '').replace(/\s*\|\s*/g, ' — ').replace(/⚠\s*/g, '').trim() });
      // si es una cita, también se quitan las líneas que la continúan
      if (/^\s*>/.test(l)) while (i + 1 < lineas.length && /^\s*>\s*\S/.test(lineas[i + 1]) && !/^\s*>\s*\*\*/.test(lineas[i + 1])) i++;
      continue;
    }
    fuera.push(l);
  }
  return fuera.join('\n');
}

// Enlaces entre capítulos y referencias "cap. N" / "capítulo N" → número final del título en el Word
function renumerar(md, mapa) {
  const n = v => (mapa(Number(v)) ?? v);
  md = md.replace(/\[(\d+)\]\(\d{2}-[^)]+\.md\)/g, (_, d) => `capítulo ${n(d)}`);
  md = md.replace(/\[(\d+)\.\s*([^\]]+)\]\(\d{2}-[^)]+\.md\)/g, (_, d, t) => `capítulo ${n(d)} (${t})`);
  md = md.replace(/\[([^\]]+)\]\(\d{2}-[^)]+\.md\)/g, '$1');
  md = md.replace(/\b(cap(?:ítulo)?s?\.?\s+)(\d+)((?:\s*(?:,|y|a)\s*\d+)*)/gi,
    (_, pre, a, resto) => pre + n(a) + resto.replace(/\d+/g, n));
  return md;
}
const sinEnlaces = md => md.replace(/\[([^\]]+)\]\([^)]+\)/g, '$1');

// ---------------- plantilla ----------------
async function documento({ salida, tituloPortada, tituloEncabezado, cuerpoXml }) {
  const zip = await JSZip.loadAsync(fs.readFileSync(PLANTILLA));
  let doc = await zip.file('word/document.xml').async('string');

  const ini = doc.indexOf('<w:body>') + 8;
  const intro = doc.lastIndexOf('<w:p ', doc.indexOf('>INTRODUCCIÓN'));
  const fin = doc.lastIndexOf('<w:sectPr');
  let previo = doc.slice(ini, intro);

  // portada: título del documento
  previo = reemplazarParrafo(previo, 'MANUAL DE GESTIÓN DE CALIDAD', tituloPortada.toUpperCase(), false);

  // índice: se conserva el control de la plantilla, sin las entradas del manual de calidad (Word lo actualiza)
  previo = previo.replace(/(<w:sdtContent>)([\s\S]*?)(<\/w:sdtContent>)/, (_, a, contenido, c) => {
    const tituloTdc = (/<w:p [^>]*>(?:(?!<\/w:p>)[\s\S])*?w:val="TtuloTDC"[\s\S]*?<\/w:p>/.exec(contenido) || [''])[0];
    const campo = `<w:p><w:pPr><w:pStyle w:val="TDC1"/></w:pPr><w:r><w:fldChar w:fldCharType="begin" w:dirty="true"/></w:r>` +
      `<w:r><w:instrText xml:space="preserve"> TOC \\o "1-3" \\h \\z \\u </w:instrText></w:r><w:r><w:fldChar w:fldCharType="separate"/></w:r>` +
      `${run('Actualice el índice: clic derecho → Actualizar campo → Actualizar toda la tabla.')}<w:r><w:fldChar w:fldCharType="end"/></w:r></w:p>`;
    return a + tituloTdc + campo + c;
  });
  // los comentarios de la plantilla no aplican a estos documentos
  previo = previo.replace(/<w:commentRangeStart [^>]*\/>|<w:commentRangeEnd [^>]*\/>|<w:r>(?:(?!<\/w:r>)[\s\S])*?<w:commentReference [^>]*\/>(?:(?!<\/w:r>)[\s\S])*?<\/w:r>/g, '');

  doc = doc.slice(0, ini) + previo + cuerpoXml + doc.slice(fin);
  zip.file('word/document.xml', doc);

  // numeraciones para listas numeradas
  let num = await zip.file('word/numbering.xml').async('string');
  num = num.replace('</w:numbering>', numerosNuevos.join('') + '</w:numbering>');
  zip.file('word/numbering.xml', num);
  numerosNuevos = [];

  // comentarios vacíos
  for (const f of ['word/comments.xml', 'word/commentsExtended.xml', 'word/commentsIds.xml', 'word/commentsExtensible.xml']) {
    const x = zip.file(f) && await zip.file(f).async('string');
    if (x) zip.file(f, x.replace(/(<w\d*\w*:comments[^>]*>|<w15:commentsEx[^>]*>|<w16cid:commentsIds[^>]*>|<w16cex:commentsExtensible[^>]*>)[\s\S]*(<\/[^>]+>)\s*$/, '$1$2'));
  }

  // encabezado de páginas internas: nombre del documento y código por asignar
  let h1 = await zip.file('word/header1.xml').async('string');
  h1 = reemplazarParrafo(h1, 'Manual de Gestión de Calidad', tituloEncabezado, false);
  h1 = reemplazarParrafo(h1, 'MA-SG-GC-001-A', CODIGO, true);
  zip.file('word/header1.xml', h1);

  // encabezado de portada: clasificación Confidencial y campos por completar en amarillo
  let h2 = await zip.file('word/header2.xml').async('string');
  h2 = marcarClasificacion(h2);
  for (const etq of ['Tipo de Proceso:', 'Macroproceso:', 'Proceso:', 'Custodio:']) h2 = valorAmarillo(h2, etq, 'Por definir');
  h2 = valorAmarillo(h2, 'Código Documento:', CODIGO);
  zip.file('word/header2.xml', h2);

  // control de cambios: fecha de hoy y firmas por completar en amarillo
  let f2 = await zip.file('word/footer2.xml').async('string');
  f2 = f2.replace('>2026/09/11<', `>${FECHA}<`);
  for (const v of ['LB', 'JM', 'xx']) f2 = f2.replace(new RegExp(`(<w:r>|<w:r [^>]*>)(<w:rPr>(?:(?!</w:rPr>)[\\s\\S])*?)(</w:rPr><w:t[^>]*>)${v}</w:t>`),
    (_, r, rpr, t) => `${r}${rpr.includes('w:highlight') ? rpr : rpr + AMARILLO}${t}xx</w:t>`);
  zip.file('word/footer2.xml', f2);

  // propiedades del documento
  const core = zip.file('docProps/core.xml') && await zip.file('docProps/core.xml').async('string');
  if (core) zip.file('docProps/core.xml', core.replace(/<dc:title>[^<]*<\/dc:title>/, `<dc:title>${esc(tituloPortada)}</dc:title>`));

  fs.mkdirSync(SALIDA, { recursive: true });
  fs.writeFileSync(salida, await zip.generateAsync({ type: 'nodebuffer', compression: 'DEFLATE' }));
  console.log('Word generado:', path.relative(DOCS, salida));
}
const CODIGO = 'XX-XX-XX-00X-A';

// Reemplaza el texto de los runs de un párrafo (cabecera) que contiene `buscado`
function reemplazarParrafo(xml, buscado, nuevo, amarillo) {
  return xml.replace(/<w:p [^>]*>(?:(?!<\/w:p>)[\s\S])*?<\/w:p>/g, p => {
    const texto = (p.match(/<w:t[^>]*>[^<]*<\/w:t>/g) || []).map(t => t.replace(/<[^>]+>/g, '')).join('');
    if (!texto.includes(buscado)) return p;
    let primero = true;
    return p.replace(/(<w:r(?: [^>]*)?>)(<w:rPr>[\s\S]*?<\/w:rPr>)?(<w:t[^>]*>)[^<]*(<\/w:t>)/g, (_, r, rpr = '', t, tc) => {
      if (!primero) return `${r}${rpr}${t}${tc}`; // los demás runs del párrafo quedan vacíos
      primero = false;
      const rp = amarillo ? (rpr ? rpr.replace('</w:rPr>', AMARILLO + '</w:rPr>') : `<w:rPr>${AMARILLO}</w:rPr>`) : rpr;
      return `${r}${rp}<w:t xml:space="preserve">${esc(nuevo)}${tc}`;
    });
  });
}

// En el párrafo cuya etiqueta en negrita es `etiqueta`, cambia el valor por `valor` resaltado en amarillo
function valorAmarillo(xml, etiqueta, valor) {
  return xml.replace(/<w:p [^>]*>(?:(?!<\/w:p>)[\s\S])*?<\/w:p>/g, p => {
    const runs = p.match(/<w:r(?: [^>]*)?>(?:(?!<\/w:r>)[\s\S])*?<\/w:r>/g) || [];
    const textos = runs.map(r => (r.match(/<w:t[^>]*>([^<]*)<\/w:t>/) || [, ''])[1]);
    const k = textos.findIndex(t => t.trim() === etiqueta || t.trim() === etiqueta.replace(':', '').trim() + ':');
    if (k < 0 || !textos.join('').trim().startsWith(etiqueta)) return p;
    const resto = runs.slice(k + 1);
    if (!resto.length) return p;
    const base = resto.find(r => /<w:t[^>]*>[^<\s]/.test(r)) || resto[0];
    const rpr = ((/<w:rPr>[\s\S]*?<\/w:rPr>/.exec(base) || ['<w:rPr></w:rPr>'])[0]).replace(/<w:b\/>|<w:bCs\/>/g, '').replace('</w:rPr>', AMARILLO + '</w:rPr>');
    let nuevo = p;
    resto.forEach((r, i) => { nuevo = nuevo.replace(r, i === 0 ? `<w:r>${rpr}<w:t xml:space="preserve"> ${esc(valor)}</w:t></w:r>` : ''); });
    return nuevo;
  });
}

// Pasa el relleno negro de la casilla "Interna" a la casilla "Confidencial"
function marcarClasificacion(xml) {
  const SHD = /<w:shd w:val="clear" w:color="auto" w:fill="000000"[^>]*\/>/;
  const runs = [...xml.matchAll(/<w:r(?: [^>]*)?>(?:(?!<\/w:r>)[\s\S])*?<\/w:r>/g)];
  const idxTexto = palabra => runs.findIndex(m => new RegExp(`<w:t[^>]*>${palabra}</w:t>`).test(m[0]));
  const casillaAntes = k => { for (let i = k; i >= 0; i--) if (runs[i][0].includes('<w:drawing>')) return runs[i][0]; return null; };
  const interna = casillaAntes(idxTexto('Interna'));
  const conf = casillaAntes(idxTexto('Confidencial'));
  const shd = interna && (SHD.exec(interna) || [])[0];
  if (!shd || !conf) { console.warn('No se encontró la casilla de clasificación; revísela a mano.'); return xml; }
  const confMarcada = conf.includes('<w:rPr>') ? conf.replace('<w:rPr>', '<w:rPr>' + shd) : conf.replace(/(<w:r(?: [^>]*)?>)/, `$1<w:rPr>${shd}</w:rPr>`);
  return xml.replace(interna, interna.replace(SHD, '')).replace(conf, confMarcada);
}

// ---------------- documentos ----------------
const leer = p => fs.readFileSync(path.join(DOCS, p), 'utf8').replace(/\r\n/g, '\n');
const pre = n => fs.readFileSync(path.join(__dirname, 'preliminares', n + '.md'), 'utf8').replace(/\r\n/g, '\n');
const sinTitulo = md => md.replace(/^# .*\n+/, '');
const sinCitaInicial = md => md.replace(/^(> .*\n)+\n*/, '');
const contarSecciones = md => (md.match(/^# /gm) || []).length;

async function manualGeneral() {
  const p = pre('general');
  const F = contarSecciones(p);
  let md = sinCitaInicial(sinTitulo(leer('manual-general/Manual-General-RenergeIA.md')));
  md = quitarPorConfirmar(md, 'Manual general');
  md = sinEnlaces(md.replace(/^## /gm, '# ').replace(/^### /gm, '## ').replace(/^#### /gm, '### '));
  await documento({
    salida: path.join(SALIDA, 'Manual-General-RenergeIA.docx'),
    tituloPortada: 'Manual general de la plataforma RenergeIA', tituloEncabezado: 'Manual general RenergeIA',
    cuerpoXml: cuerpo(p) + cuerpo(md),
  });
  return F;
}

async function instructivo() {
  const p = pre('helvis');
  const F = contarSecciones(p);
  let md = sinCitaInicial(sinTitulo(leer('instructivo-helvis/Instructivo-Helvis-WhatsApp.md')));
  md = quitarPorConfirmar(md, 'Instructivo de Helvis');
  md = md.replace(/^## /gm, '# ').replace(/^### /gm, '## ').replace(/^#### /gm, '### ');
  md = md.replace(/\b(secci[oó]n(?:es)?\s+)(\d+)/gi, (_, a, d) => a + (Number(d) + F));
  await documento({
    salida: path.join(SALIDA, 'Instructivo-Helvis-WhatsApp.docx'),
    tituloPortada: 'Instructivo de Helvis — asistente de WhatsApp', tituloEncabezado: 'Instructivo de Helvis',
    cuerpoXml: cuerpo(p) + cuerpo(sinEnlaces(md)),
  });
  return F;
}

async function manualUsuario(Fhelvis) {
  const p = pre('usuario');
  const F = contarSecciones(p);
  const dir = path.join(DOCS, 'manual');
  const caps = fs.readdirSync(dir).filter(f => /^\d{2}-.*\.md$/.test(f)).sort();
  const mapa = v => { const n = Number(v); return n >= 0 && n < caps.length ? n + F + 1 : v; };
  let md = '';
  for (const f of caps) {
    let c = leer('manual/' + f);
    c = quitarPorConfirmar(c, 'Manual de usuario — ' + c.match(/^# (.*)$/m)[1]);
    c = c.replace(/Centro de Control \(cap\. 1\)/g, 'Centro de control (cap. 18)');
    c = c.replace(/Instructivo de Helvis, secci[oó]n (\d+)/g, (_, d) => `Instructivo de Helvis, sección ${Number(d) + Fhelvis}`);
    c = renumerar(c, mapa);
    // en la tabla de "Primeros pasos", la columna Capítulo también se renumera
    if (f.startsWith('00')) c = c.replace(/^(\|.*\|)\s*([\d ,ya]+?)\s*\|$/gm, (fila, ini, cap) => /^\|\s*\d+\s*\|/.test(fila) ? `${ini} ${cap.replace(/\d+/g, mapa)} |` : fila);
    md += c + '\n\n';
  }
  await documento({
    salida: path.join(SALIDA, 'Manual-Usuario-RenergeIA.docx'),
    tituloPortada: 'Manual de usuario de RenergeIA', tituloEncabezado: 'Manual de usuario RenergeIA',
    cuerpoXml: cuerpo(p) + cuerpo(sinEnlaces(md)),
  });
}

async function listaPorConfirmar() {
  const grupos = {};
  for (const x of porConfirmar) (grupos[x.origen] ||= []).push(x.texto);
  let md = '# INTRODUCCIÓN\n\nEsta lista reúne las notas "Por confirmar" que se retiraron de los manuales oficiales. Son reglas o comportamientos de la plataforma que aún deben validarse con las áreas responsables o con la gerencia antes de incluirlos como definitivos.\n\n';
  md += '# PUNTOS POR CONFIRMAR\n\n';
  for (const [g, items] of Object.entries(grupos)) md += `## ${g}\n\n` + items.map(t => `- ${t.replace(/^\*\*Por confirmar:?\*\*:?\s*|^Por confirmar:?\s*/i, '')}`).join('\n') + '\n\n';
  await documento({
    salida: path.join(SALIDA, 'Puntos-por-confirmar-manuales.docx'),
    tituloPortada: 'Puntos por confirmar de los manuales de RenergeIA', tituloEncabezado: 'Puntos por confirmar',
    cuerpoXml: cuerpo(sinEnlaces(md)),
  });
  console.log('Puntos por confirmar:', porConfirmar.length);
}

// Después de que Word actualiza el índice: da a las entradas el formato del índice de la plantilla
// (nivel 1 en Verdana negrita, niveles 2 y 3 en Verdana normal). Uso: node build.js --indice <archivo.docx>
async function formatoIndice(archivo) {
  const zip = await JSZip.loadAsync(fs.readFileSync(archivo));
  let doc = await zip.file('word/document.xml').async('string');
  const FORMATO = {
    TDC1: FUENTE + '<w:b/><w:bCs/><w:sz w:val="22"/><w:szCs w:val="22"/>',
    TDC2: FUENTE + '<w:sz w:val="20"/><w:szCs w:val="20"/>',
    TDC3: FUENTE + '<w:sz w:val="20"/><w:szCs w:val="20"/>',
  };
  doc = doc.replace(/<w:p(?: [^>]*)?>(?:(?!<\/w:p>)[\s\S])*?<\/w:p>/g, p => {
    const est = (/<w:pStyle w:val="(TDC[123])"\/>/.exec(p) || [])[1];
    if (!est) return p;
    const f = FORMATO[est];
    return p.replace(/<w:r>(?!<w:rPr>)/g, '<w:r><w:rPr></w:rPr>')
      .replace(/(<w:r(?: [^>]*)?>)<w:rPr>((?:(?!<\/w:rPr>)[\s\S])*?)<\/w:rPr>/g, (_, r, rpr) => {
        const limpio = rpr.replace(/<w:rFonts [^>]*\/>|<w:sz [^>]*\/>|<w:szCs [^>]*\/>|<w:b\/>|<w:bCs\/>|<w:b w:val="0"\/>/g, '');
        const conEstilo = /<w:rStyle [^>]*\/>/.test(limpio) ? limpio.replace(/(<w:rStyle [^>]*\/>)/, '$1' + f) : f + limpio;
        return `${r}<w:rPr>${conEstilo}</w:rPr>`;
      });
  });
  zip.file('word/document.xml', doc);
  fs.writeFileSync(archivo, await zip.generateAsync({ type: 'nodebuffer', compression: 'DEFLATE' }));
  console.log('Índice con formato de la plantilla:', path.basename(archivo));
}

(async () => {
  if (process.argv[2] === '--indice') return formatoIndice(process.argv[3]);
  await manualGeneral();
  const Fh = await instructivo();
  await manualUsuario(Fh);
  await listaPorConfirmar();
})().catch(e => { console.error(e); process.exit(1); });
