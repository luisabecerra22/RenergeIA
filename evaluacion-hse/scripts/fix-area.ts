import { Firestore } from "@google-cloud/firestore";

async function main() {
  const db = new Firestore({ projectId: "renergeia-evaluaciones", ignoreUndefinedProperties: true });
  const cedula = "1003252755"; // Luisa Fernanda Becerra Patiño
  const ref = db.collection("personal").doc(cedula);
  const snap = await ref.get();
  if (!snap.exists) {
    console.error(`No existe persona con cédula ${cedula}`);
    return;
  }
  const antes = snap.data()?.area;
  await ref.update({ area: "HSEQ" });
  console.log(`Área actualizada para ${cedula}: "${antes}" -> "HSEQ"`);
}

main().catch(console.error);
