import { Firestore } from "@google-cloud/firestore";

const OLD_USERNAME = "Admin-Renergeia";
const NEW_USERNAME = "hseq";

async function main() {
  const db = new Firestore({ ignoreUndefinedProperties: true });

  const oldDoc = await db.collection("admins").doc(OLD_USERNAME).get();
  if (!oldDoc.exists) {
    console.log(`No se encontró el usuario "${OLD_USERNAME}".`);
    return;
  }

  const existing = await db.collection("admins").doc(NEW_USERNAME).get();
  if (existing.exists) {
    console.log(`Ya existe un usuario con username "${NEW_USERNAME}". Abortando.`);
    return;
  }

  const data = oldDoc.data()!;
  data.username = NEW_USERNAME;

  await db.collection("admins").doc(NEW_USERNAME).set(data);
  await db.collection("admins").doc(OLD_USERNAME).delete();

  console.log(`Usuario renombrado: "${OLD_USERNAME}" → "${NEW_USERNAME}"`);
  console.log("Datos conservados:", JSON.stringify({ rol: data.rol, area: data.area, activo: data.activo }));
}

main().catch(console.error);
