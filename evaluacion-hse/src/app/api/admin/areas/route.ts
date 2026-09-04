import { NextResponse } from "next/server";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";
import type { Area } from "@/lib/types";

export const runtime = "nodejs";
export const dynamic = "force-dynamic";

function slugificar(nombre: string): string {
  return nombre
    .normalize("NFD")
    .replace(/[̀-ͯ]/g, "")
    .toLowerCase()
    .replace(/[^a-z0-9]+/g, "-")
    .replace(/(^-|-$)/g, "");
}

/** Lista las áreas — cualquier usuario autenticado (necesario para filtros/etiquetas). */
export async function GET() {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  const store = await getStore();
  const areas = await store.listAreas();
  return NextResponse.json({ areas });
}

/** Crea un área nueva — solo admin. */
export async function POST(req: Request) {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });
  if (sesion.rol !== "admin") {
    return NextResponse.json({ error: "No autorizado." }, { status: 403 });
  }

  let body: { nombre?: string };
  try {
    body = await req.json();
  } catch {
    return NextResponse.json({ error: "Solicitud inválida." }, { status: 400 });
  }

  const nombre = body.nombre?.trim() ?? "";
  if (!nombre) {
    return NextResponse.json({ error: "El nombre es obligatorio." }, { status: 400 });
  }

  const id = slugificar(nombre);
  if (!id) {
    return NextResponse.json({ error: "Nombre de área inválido." }, { status: 400 });
  }

  const store = await getStore();
  const existente = await store.getArea(id);
  if (existente) {
    return NextResponse.json({ error: "Ya existe un área con ese nombre." }, { status: 409 });
  }

  const area: Area = { id, nombre, creadaEn: new Date().toISOString() };
  await store.saveArea(area);
  return NextResponse.json({ area });
}
