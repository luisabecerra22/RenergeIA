import { randomBytes } from "node:crypto";
import { NextResponse } from "next/server";
import bcrypt from "bcryptjs";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";
import type { Admin } from "@/lib/types";

export const runtime = "nodejs";
export const dynamic = "force-dynamic";

/** Genera una contraseña aleatoria legible-segura (ej. "xK7m-Qp2r-Tz9w"). */
function generarPassword(): string {
  const alfabeto = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
  const grupo = () =>
    Array.from(randomBytes(4))
      .map((b) => alfabeto[b % alfabeto.length])
      .join("");
  return `${grupo()}-${grupo()}-${grupo()}`;
}

function sinPassword(a: Admin) {
  const { passwordHash: _passwordHash, ...resto } = a;
  return resto;
}

/** Lista las cuentas de administrador — solo admin (protegido también en middleware). */
export async function GET() {
  const sesion = await sesionActual();
  if (!sesion || sesion.rol !== "admin") {
    return NextResponse.json({ error: "No autorizado." }, { status: 403 });
  }
  const store = await getStore();
  const admins = await store.listAdmins();
  return NextResponse.json({ admins: admins.map(sinPassword) });
}

/** Crea una nueva cuenta con contraseña generada — solo admin. */
export async function POST(req: Request) {
  const sesion = await sesionActual();
  if (!sesion || sesion.rol !== "admin") {
    return NextResponse.json({ error: "No autorizado." }, { status: 403 });
  }

  let body: { username?: string; rol?: string; area?: string };
  try {
    body = await req.json();
  } catch {
    return NextResponse.json({ error: "Solicitud inválida." }, { status: 400 });
  }

  const username = body.username?.trim().toLowerCase() ?? "";
  const rol = body.rol === "admin" ? "admin" : "area";
  const area = body.area?.trim() ?? "";

  if (!/^[a-z0-9_.-]{3,40}$/.test(username)) {
    return NextResponse.json(
      { error: "Usuario inválido (mínimo 3 caracteres, sin espacios)." },
      { status: 400 },
    );
  }
  if (rol === "area" && !area) {
    return NextResponse.json(
      { error: "Selecciona el área de esta cuenta." },
      { status: 400 },
    );
  }

  const store = await getStore();
  if (rol === "area") {
    const areaExiste = await store.getArea(area);
    if (!areaExiste) {
      return NextResponse.json({ error: "El área seleccionada no existe." }, { status: 400 });
    }
  }

  const existente = await store.getAdmin(username);
  if (existente) {
    return NextResponse.json({ error: "Ese usuario ya existe." }, { status: 409 });
  }

  const password = generarPassword();
  const admin: Admin = {
    username,
    passwordHash: await bcrypt.hash(password, 10),
    rol,
    area: rol === "area" ? area : undefined,
    activo: true,
    creadoEn: new Date().toISOString(),
  };
  await store.saveAdmin(admin);

  // La contraseña en texto plano se devuelve UNA sola vez; no se guarda en ningún lado.
  return NextResponse.json({ admin: sinPassword(admin), password });
}
