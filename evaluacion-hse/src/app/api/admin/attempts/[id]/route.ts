import { NextResponse } from "next/server";
import { getStore } from "@/lib/db";
import { puedeVerArea, sesionActual } from "@/lib/auth";

export const runtime = "nodejs";
export const dynamic = "force-dynamic";

export async function DELETE(
  _req: Request,
  { params }: { params: Promise<{ id: string }> },
) {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  const { id } = await params;
  const store = await getStore();
  const intento = await store.getIntento(id);
  if (intento && !puedeVerArea(sesion, intento.area)) {
    return NextResponse.json({ error: "No autorizado." }, { status: 403 });
  }
  await store.softDeleteIntento(id);
  return NextResponse.json({ ok: true });
}

export async function PATCH(
  req: Request,
  { params }: { params: Promise<{ id: string }> },
) {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  const { id } = await params;
  const store = await getStore();
  const intento = await store.getIntento(id);
  if (intento && !puedeVerArea(sesion, intento.area)) {
    return NextResponse.json({ error: "No autorizado." }, { status: 403 });
  }

  const { accion } = (await req.json()) as { accion: string };

  if (accion === "restaurar") {
    await store.restaurarIntento(id);
  } else if (accion === "eliminar-permanente") {
    await store.deleteIntento(id);
  }

  return NextResponse.json({ ok: true });
}
