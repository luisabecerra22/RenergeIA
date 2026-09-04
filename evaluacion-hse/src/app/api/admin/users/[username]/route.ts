import { randomBytes } from "node:crypto";
import { NextResponse } from "next/server";
import bcrypt from "bcryptjs";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";

export const runtime = "nodejs";
export const dynamic = "force-dynamic";

function generarPassword(): string {
  const alfabeto = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
  const grupo = () =>
    Array.from(randomBytes(4))
      .map((b) => alfabeto[b % alfabeto.length])
      .join("");
  return `${grupo()}-${grupo()}-${grupo()}`;
}

/** Activa/desactiva una cuenta, o le genera una contraseña nueva — solo admin. */
export async function PATCH(
  req: Request,
  { params }: { params: Promise<{ username: string }> },
) {
  const sesion = await sesionActual();
  if (!sesion || sesion.rol !== "admin") {
    return NextResponse.json({ error: "No autorizado." }, { status: 403 });
  }

  const { username } = await params;
  const store = await getStore();
  const admin = await store.getAdmin(username);
  if (!admin) {
    return NextResponse.json({ error: "No encontrada." }, { status: 404 });
  }

  const { accion } = (await req.json()) as { accion: string };

  if (accion === "activar" || accion === "desactivar") {
    if (username === sesion.username) {
      return NextResponse.json(
        { error: "No puedes desactivar tu propia cuenta." },
        { status: 400 },
      );
    }
    admin.activo = accion === "activar";
    await store.saveAdmin(admin);
    return NextResponse.json({ ok: true });
  }

  if (accion === "resetear-password") {
    const password = generarPassword();
    admin.passwordHash = await bcrypt.hash(password, 10);
    await store.saveAdmin(admin);
    return NextResponse.json({ ok: true, password });
  }

  return NextResponse.json({ error: "Acción no reconocida." }, { status: 400 });
}
