import { NextResponse } from "next/server";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";
import type { Persona } from "@/lib/types";

export const runtime = "nodejs";
export const dynamic = "force-dynamic";

export async function GET() {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  const store = await getStore();
  const personal = await store.listPersonal();
  return NextResponse.json(personal);
}

export async function POST(req: Request) {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  const body = await req.json();
  const { cedula, nombre, apellido, tipoDocumento, correo, area, cargo, proyecto, fechaContratacion, trabajo } = body;

  if (!cedula || !nombre || !apellido) {
    return NextResponse.json({ error: "Cédula, nombre y apellido son obligatorios." }, { status: 400 });
  }

  const store = await getStore();
  const existente = await store.getPersona(String(cedula));
  if (existente) {
    return NextResponse.json({ error: "Ya existe una persona con esa cédula." }, { status: 409 });
  }

  const persona: Persona = {
    cedula: String(cedula),
    nombre,
    apellido,
    tipoDocumento: tipoDocumento || "Cédula de ciudadanía",
    correo: correo || "",
    area: area || "",
    cargo: cargo || "",
    proyecto: proyecto || "",
    fechaContratacion: fechaContratacion || "",
    trabajo: trabajo || "",
    creadoEn: new Date().toISOString(),
  };

  await store.savePersona(persona);
  return NextResponse.json(persona, { status: 201 });
}
