// Recorta las capturas del manual para quitar espacio muerto y el widget de la extensión.
const sharp = require("C:/Users/Luisa Becerra/Downloads/RenergeIA/docs/presentacion-evaluaciones/node_modules/sharp");
const fs = require("fs");
const path = require("path");

const dir = "C:/Users/Luisa Becerra/Downloads/RenergeIA/docs/manual-evaluaciones/img";
const out = path.join(dir, "final");
fs.mkdirSync(out, { recursive: true });

(async () => {
  const files = fs.readdirSync(dir).filter((f) => /\.(jpg|png)$/i.test(f) && f !== "diploma.png");
  for (const f of files) {
    const src = path.join(dir, f);
    const m = await sharp(src).metadata();
    let crop = null;
    if (f === "p06_certificado.png") {
      crop = null; // certificado: sin recorte
    } else if (/^a\d/.test(f)) {
      crop = { left: 290, top: 68, width: 665, height: m.height - 76 }; // admin
    } else {
      crop = { left: 330, top: 0, width: 790, height: m.height }; // participante
    }
    if (crop) {
      crop.left = Math.max(0, crop.left);
      crop.top = Math.max(0, crop.top);
      crop.width = Math.min(crop.width, m.width - crop.left);
      crop.height = Math.min(crop.height, m.height - crop.top);
    }
    let img = sharp(src);
    if (crop) img = img.extract(crop);
    await img.toFile(path.join(out, f));
  }
  console.log("cropped", files.length, "images ->", out);
})().catch((e) => { console.error(e); process.exit(1); });
