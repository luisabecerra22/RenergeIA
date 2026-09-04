/**
 * Migración única: introduce áreas (HSE/RRHH) y roles de usuario en producción.
 *
 * Qué hace:
 * 1) Crea las áreas "hse" y "rrhh" si no existen.
 * 2) Etiqueta con area:"hse" todas las evaluaciones/intentos/asistencias que
 *    no tengan ya un campo `area` (todo lo existente hasta hoy es de HSE).
 * 3) Convierte la cuenta admin actual (la que ya usa el equipo HSE) en una
 *    cuenta de área "hse" — mismas credenciales, nuevo rol.
 * 4) Crea una cuenta admin nueva ("renergeia-administrador") y una cuenta
 *    de RRHH ("rrhh"), cada una con una contraseña generada que se imprime
 *    UNA sola vez en esta consola.
 *
 * Requiere: gcloud auth application-default login (credenciales con acceso
 * de escritura a Firestore en el proyecto renergeia-evaluaciones).
 *
 * Uso:
 *   GOOGLE_CLOUD_PROJECT=renergeia-evaluaciones npx tsx scripts/migrate-roles.ts
 */
import { randomBytes } from "node:crypto";
import { Firestore } from "@google-cloud/firestore";
import bcrypt from "bcryptjs";

function generarPassword(): string {
  const alfabeto = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
  const grupo = () =>
    Array.from(randomBytes(4))
      .map((b) => alfabeto[b % alfabeto.length])
      .join("");
  return `${grupo()}-${grupo()}-${grupo()}`;
}

async function backfillColeccion(
  db: Firestore,
  coleccion: string,
  area = "hse",
): Promise<number> {
  const snap = await db.collection(coleccion).get();
  let actualizados = 0;
  const BATCH = 400;
  let batch = db.batch();
  let enBatch = 0;

  for (const doc of snap.docs) {
    if (doc.data().area) continue; // ya migrado
    batch.update(doc.ref, { area });
    actualizados++;
    enBatch++;
    if (enBatch >= BATCH) {
      await batch.commit();
      batch = db.batch();
      enBatch = 0;
    }
  }
  if (enBatch > 0) await batch.commit();
  return actualizados;
}

async function main(): Promise<void> {
  const projectId = process.env.GOOGLE_CLOUD_PROJECT;
  if (!projectId) {
    throw new Error("Define GOOGLE_CLOUD_PROJECT (ej. renergeia-evaluaciones).");
  }
  const db = new Firestore({ projectId, ignoreUndefinedProperties: true });

  console.log(`\nMigrando roles/áreas en el proyecto: ${projectId}\n`);

  // 1) Áreas base.
  for (const a of [
    { id: "hse", nombre: "HSE" },
    { id: "rrhh", nombre: "Recursos Humanos" },
  ]) {
    const ref = db.collection("areas").doc(a.id);
    const doc = await ref.get();
    if (doc.exists) {
      console.log(`• Área ya existe: ${a.id}`);
    } else {
      await ref.set({ ...a, creadaEn: new Date().toISOString() });
      console.log(`✓ Área creada: ${a.id}`);
    }
  }

  // 2) Backfill de datos existentes → area: "hse".
  for (const coleccion of ["evaluaciones", "intentos", "asistencias"]) {
    const n = await backfillColeccion(db, coleccion);
    console.log(`✓ ${coleccion}: ${n} documento(s) etiquetados como "hse"`);
  }

  // 3) Cuenta admin actual → pasa a ser la cuenta de área HSE.
  const adminsSnap = await db.collection("admins").get();
  const cuentasExistentes = adminsSnap.docs.map((d) => d.id);
  if (cuentasExistentes.length === 0) {
    console.log("⚠ No se encontró ninguna cuenta admin existente para convertir a HSE.");
  } else if (cuentasExistentes.length > 1) {
    console.log(
      `⚠ Hay ${cuentasExistentes.length} cuentas ya creadas (${cuentasExistentes.join(", ")}); no se modifica ninguna automáticamente.`,
    );
  } else {
    const username = cuentasExistentes[0];
    const ref = db.collection("admins").doc(username);
    const doc = await ref.get();
    const data = doc.data()!;
    if (data.rol) {
      console.log(`• La cuenta "${username}" ya tiene rol asignado (${data.rol}), no se toca.`);
    } else {
      await ref.update({ rol: "area", area: "hse", activo: true });
      console.log(`✓ Cuenta "${username}" convertida a área HSE (mismas credenciales).`);
    }
  }

  // 4) Nuevas cuentas: admin global y RRHH.
  async function crearCuenta(
    username: string,
    rol: "admin" | "area",
    area?: string,
  ): Promise<void> {
    const ref = db.collection("admins").doc(username);
    const doc = await ref.get();
    if (doc.exists) {
      console.log(`• Cuenta "${username}" ya existe, no se sobrescribe.`);
      return;
    }
    const password = generarPassword();
    await ref.set({
      username,
      passwordHash: await bcrypt.hash(password, 10),
      rol,
      area,
      activo: true,
      creadoEn: new Date().toISOString(),
    });
    console.log(`✓ Cuenta creada: ${username}  →  contraseña: ${password}`);
  }

  await crearCuenta("renergeia-administrador", "admin");
  await crearCuenta("rrhh", "area", "rrhh");

  console.log("\nMigración completada. Guarda las contraseñas mostradas arriba — no se repiten.\n");
}

main().catch((e) => {
  console.error("Error en la migración:", e);
  process.exit(1);
});
