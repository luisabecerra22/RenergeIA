import { NextResponse } from "next/server";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";

export const runtime = "nodejs";
export const dynamic = "force-dynamic";

export async function GET() {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  const store = await getStore();
  const todas = await store.listAsistencias();
  const asistencias =
    sesion.rol === "admin" ? todas : todas.filter((a) => a.area === sesion.area);
  return NextResponse.json({ asistencias });
}
