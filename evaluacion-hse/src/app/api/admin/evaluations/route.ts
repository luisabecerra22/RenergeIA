import { randomUUID } from "node:crypto";
import { NextResponse } from "next/server";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";
import { FEEDBACK_ESTANDAR } from "@/lib/seed-data";
import type { Evaluacion } from "@/lib/types";

export const runtime = "nodejs";
export const dynamic = "force-dynamic";

export async function GET() {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  const store = await getStore();
  const todas = await store.listEvaluaciones();
  const evaluaciones =
    sesion.rol === "admin" ? todas : todas.filter((e) => e.area === sesion.area);
  return NextResponse.json({ evaluaciones });
}

/** Crea una nueva evaluación en blanco (del área del usuario) y devuelve su id. */
export async function POST() {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  const store = await getStore();
  const ahora = new Date().toISOString();

  // Un usuario de área siempre crea para su propia área; el admin elige
  // luego el área desde el editor (arranca en la primera área existente).
  let area = sesion.area ?? "";
  if (sesion.rol === "admin") {
    const areas = await store.listAreas();
    area = areas[0]?.id ?? "";
  }

  const nueva: Evaluacion = {
    id: randomUUID(),
    titulo: "Nueva evaluación",
    tema: "Nueva capacitación",
    descripcion: "",
    activa: false,
    area,
    creadaEn: ahora,
    actualizadaEn: ahora,
    feedback: FEEDBACK_ESTANDAR,
    preguntas: [
      {
        id: "p1",
        enunciado: "",
        opciones: [
          { id: "a", texto: "" },
          { id: "b", texto: "" },
        ],
        correcta: "a",
      },
    ],
  };
  await store.saveEvaluacion(nueva);
  return NextResponse.json({ id: nueva.id });
}
