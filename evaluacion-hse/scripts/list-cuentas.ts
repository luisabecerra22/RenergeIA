import { Firestore } from "@google-cloud/firestore";

async function main() {
  const db = new Firestore({ projectId: "renergeia-evaluaciones", ignoreUndefinedProperties: true });
  const admins = await db.collection("admins").get();
  const areas = await db.collection("areas").get();
  console.log("=== ADMINS ===");
  admins.forEach((d) => {
    const a = d.data();
    console.log(`- ${a.username} | rol: ${a.rol} | area: ${a.area ?? "-"} | activo: ${a.activo}`);
  });
  console.log("=== AREAS ===");
  areas.forEach((d) => {
    const a = d.data();
    console.log(`- ${d.id} | ${a.nombre ?? JSON.stringify(a)}`);
  });
}

main().catch(console.error);
