import { Firestore } from "@google-cloud/firestore";
import * as XLSX from "xlsx";
import path from "node:path";

const FILE = path.resolve(process.argv[2] || "C:\\Users\\Luisa Becerra\\Downloads\\RENERGEIA_Informacion_Personas HSE (3108).xlsx");

function val(r: Record<string, unknown>, ...keys: string[]): string {
  for (const k of keys) {
    for (const rk of Object.keys(r)) {
      if (rk.trim().toLowerCase() === k.toLowerCase()) return String(r[rk] ?? "");
    }
  }
  return "";
}

async function main() {
  const db = new Firestore({ ignoreUndefinedProperties: true });
  const wb = XLSX.readFile(FILE);
  const ws = wb.Sheets[wb.SheetNames[0]];
  const rows = XLSX.utils.sheet_to_json<Record<string, unknown>>(ws);

  const ahora = new Date().toISOString();
  let count = 0;
  const BATCH_SIZE = 500;

  for (let i = 0; i < rows.length; i += BATCH_SIZE) {
    const batch = db.batch();
    const chunk = rows.slice(i, i + BATCH_SIZE);
    for (const r of chunk) {
      const cedula = val(r, "Número identificación", "Cedula", "cedula", "CEDULA");
      if (!cedula) continue;
      batch.set(db.collection("personal").doc(cedula), {
        cedula,
        nombre: val(r, "Nombre"),
        apellido: val(r, "Apellido"),
        tipoDocumento: val(r, "Tipo de documento") || "Cédula de ciudadanía",
        correo: val(r, "Correo electrónico", "Correo"),
        area: val(r, "Área", "Area"),
        cargo: val(r, "Cargo"),
        proyecto: val(r, "Proyecto"),
        fechaContratacion: val(r, "Fecha de contratación", "fechaContratacion"),
        trabajo: val(r, "Trabajo"),
        creadoEn: ahora,
      });
      count++;
    }
    await batch.commit();
  }

  console.log(`Importadas ${count} personas a Firestore.`);
}

main().catch(console.error);
