import { SignJWT, jwtVerify } from "jose";
import { cookies } from "next/headers";
import type { Admin } from "./types";

/**
 * Sesión del administrador basada en un JWT firmado, guardado en cookie httpOnly.
 * jose funciona tanto en el runtime de Node como en el Edge (middleware).
 */

export const SESSION_COOKIE = "renergeia_admin";
const DURACION_SEGUNDOS = 60 * 60 * 8; // 8 horas

/** Datos de la sesión activa: quién es y qué área puede ver. */
export interface Sesion {
  username: string;
  rol: "admin" | "area";
  area?: string; // id del área, solo cuando rol === "area"
}

function getSecret(): Uint8Array {
  const secret = process.env.SESSION_SECRET;
  if (!secret || secret.length < 16) {
    throw new Error(
      "SESSION_SECRET no configurado o demasiado corto (mínimo 16 caracteres).",
    );
  }
  return new TextEncoder().encode(secret);
}

/** Crea el token de sesión para una cuenta de administrador. */
export async function crearSesion(
  admin: Pick<Admin, "username" | "rol" | "area">,
): Promise<string> {
  return new SignJWT({
    sub: admin.username,
    rol: admin.rol,
    area: admin.area ?? null,
  })
    .setProtectedHeader({ alg: "HS256" })
    .setIssuedAt()
    .setExpirationTime(`${DURACION_SEGUNDOS}s`)
    .sign(getSecret());
}

/** Verifica el token; devuelve la sesión o null si es inválido/expiró. */
export async function verificarSesion(
  token: string | undefined,
): Promise<Sesion | null> {
  if (!token) return null;
  try {
    const { payload } = await jwtVerify(token, getSecret());
    if (typeof payload.sub !== "string") return null;
    const rol = payload.rol === "admin" ? "admin" : "area";
    return {
      username: payload.sub,
      rol,
      area: typeof payload.area === "string" ? payload.area : undefined,
    };
  } catch {
    return null;
  }
}

/** Lee y verifica la sesión actual desde la cookie (Server Components y Route Handlers). */
export async function sesionActual(): Promise<Sesion | null> {
  const store = await cookies();
  return verificarSesion(store.get(SESSION_COOKIE)?.value);
}

/** True si la sesión puede ver/gestionar información del área dada. */
export function puedeVerArea(sesion: Sesion, area: string): boolean {
  return sesion.rol === "admin" || sesion.area === area;
}

export const COOKIE_MAX_AGE = DURACION_SEGUNDOS;
