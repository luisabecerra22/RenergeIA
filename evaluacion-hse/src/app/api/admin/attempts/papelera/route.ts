import { NextResponse } from "next/server";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";

export const runtime = "nodejs";
export const dynamic = "force-dynamic";

export async function GET() {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  const store = await getStore();
  const todos = await store.listIntentosEliminados();
  const intentos =
    sesion.rol === "admin" ? todos : todos.filter((i) => i.area === sesion.area);
  return NextResponse.json({ intentos });
}
