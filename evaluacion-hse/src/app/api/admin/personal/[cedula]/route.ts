import { NextResponse } from "next/server";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";

export const runtime = "nodejs";
export const dynamic = "force-dynamic";

export async function PATCH(
  req: Request,
  { params }: { params: Promise<{ cedula: string }> },
) {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  const { cedula } = await params;
  const store = await getStore();
  const persona = await store.getPersona(cedula);
  if (!persona) return NextResponse.json({ error: "Persona no encontrada." }, { status: 404 });

  const body = await req.json();
  const campos = ["nombre", "apellido", "tipoDocumento", "correo", "area", "cargo", "proyecto", "fechaContratacion", "trabajo"] as const;
  for (const c of campos) {
    if (body[c] !== undefined) (persona as unknown as Record<string, string>)[c] = body[c];
  }

  await store.savePersona(persona);
  return NextResponse.json(persona);
}

export async function DELETE(
  _req: Request,
  { params }: { params: Promise<{ cedula: string }> },
) {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  const { cedula } = await params;
  const store = await getStore();
  await store.deletePersona(cedula);
  return NextResponse.json({ ok: true });
}
