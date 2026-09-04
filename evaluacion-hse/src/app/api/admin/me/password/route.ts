import { NextResponse } from "next/server";
import bcrypt from "bcryptjs";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";

export const runtime = "nodejs";
export const dynamic = "force-dynamic";

/** Permite a cualquier usuario logueado cambiar su propia contraseña. */
export async function POST(req: Request) {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  let body: { actual?: string; nueva?: string };
  try {
    body = await req.json();
  } catch {
    return NextResponse.json({ error: "Solicitud inválida." }, { status: 400 });
  }

  const actual = body.actual ?? "";
  const nueva = body.nueva ?? "";
  if (nueva.length < 8) {
    return NextResponse.json(
      { error: "La contraseña nueva debe tener al menos 8 caracteres." },
      { status: 400 },
    );
  }

  const store = await getStore();
  const admin = await store.getAdmin(sesion.username);
  if (!admin) {
    return NextResponse.json({ error: "Cuenta no encontrada." }, { status: 404 });
  }

  const ok = await bcrypt.compare(actual, admin.passwordHash);
  if (!ok) {
    return NextResponse.json({ error: "La contraseña actual no es correcta." }, { status: 401 });
  }

  admin.passwordHash = await bcrypt.hash(nueva, 10);
  await store.saveAdmin(admin);
  return NextResponse.json({ ok: true });
}
