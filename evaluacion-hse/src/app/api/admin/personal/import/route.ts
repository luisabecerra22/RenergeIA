import { NextResponse } from "next/server";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";
import type { Persona } from "@/lib/types";
import * as XLSX from "xlsx";

export const runtime = "nodejs";
export const dynamic = "force-dynamic";

function val(r: Record<string, unknown>, ...keys: string[]): string {
  for (const k of keys) {
    for (const rk of Object.keys(r)) {
      if (rk.trim().toLowerCase() === k.toLowerCase()) return String(r[rk] ?? "");
    }
  }
  return "";
}

export async function POST(req: Request) {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  const formData = await req.formData();
  const file = formData.get("archivo") as File | null;
  if (!file) return NextResponse.json({ error: "No se envió archivo." }, { status: 400 });

  const buffer = Buffer.from(await file.arrayBuffer());
  const wb = XLSX.read(buffer, { type: "buffer" });
  const ws = wb.Sheets[wb.SheetNames[0]];
  const rows = XLSX.utils.sheet_to_json<Record<string, unknown>>(ws);

  if (rows.length === 0) {
    return NextResponse.json({ error: "El archivo está vacío." }, { status: 400 });
  }

  const ahora = new Date().toISOString();
  const personas: Persona[] = rows.map((r) => ({
    cedula: val(r, "Número identificación", "Cedula", "cedula", "CEDULA"),
    nombre: val(r, "Nombre", "nombre", "NOMBRE"),
    apellido: val(r, "Apellido", "apellido", "APELLIDO"),
    tipoDocumento: val(r, "Tipo de documento", "tipoDocumento") || "Cédula de ciudadanía",
    correo: val(r, "Correo electrónico", "Correo", "correo"),
    area: val(r, "Área", "Area", "area"),
    cargo: val(r, "Cargo", "cargo"),
    proyecto: val(r, "Proyecto", "proyecto"),
    fechaContratacion: val(r, "Fecha de contratación", "fechaContratacion"),
    trabajo: val(r, "Trabajo", "trabajo"),
    creadoEn: ahora,
  })).filter((p) => p.cedula && p.cedula !== "undefined" && p.cedula !== "null");

  if (personas.length === 0) {
    return NextResponse.json({ error: "No se encontraron registros válidos con cédula." }, { status: 400 });
  }

  const store = await getStore();
  await store.savePersonalBatch(personas);

  return NextResponse.json({ importados: personas.length });
}
