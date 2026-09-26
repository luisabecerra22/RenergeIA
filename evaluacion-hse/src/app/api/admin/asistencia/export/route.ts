import { NextRequest, NextResponse } from "next/server";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";
import * as XLSX from "xlsx";

export const runtime = "nodejs";
export const dynamic = "force-dynamic";

export async function GET(req: NextRequest) {
  const sesion = await sesionActual();
  if (!sesion)
    return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  const store = await getStore();
  const todas = await store.listAsistencias();
  const asistencias =
    sesion.rol === "admin" ? todas : todas.filter((a) => a.area === sesion.area);

  const params = req.nextUrl.searchParams;
  const fEval = params.get("eval") ?? "";
  const fDepto = params.get("depto") ?? "";
  const busqueda = (params.get("q") ?? "").trim().toLowerCase();

  const filtrados = asistencias.filter((a) => {
    if (fEval && a.evaluacionId !== fEval) return false;
    if (fDepto && a.departamento !== fDepto) return false;
    if (busqueda) {
      const txt = `${a.nombre} ${a.apellido} ${a.cedula} ${a.correo}`.toLowerCase();
      if (!txt.includes(busqueda)) return false;
    }
    return true;
  });

  const headers = [
    "Fecha",
    "Nombre",
    "Apellido",
    "Cedula",
    "Cargo",
    "Departamento",
    "Proyecto",
    "Correo",
    "Capacitacion",
  ];
  if (sesion.rol === "admin") headers.push("Area");

  const rows: string[][] = [headers];
  for (const a of filtrados) {
    const fecha = new Date(a.registradoEn).toLocaleString("es-CO", {
      day: "2-digit",
      month: "2-digit",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
    const fila = [
      fecha,
      a.nombre,
      a.apellido,
      a.cedula,
      a.cargo,
      a.departamento,
      a.proyecto,
      a.correo,
      a.evaluacionTitulo,
    ];
    if (sesion.rol === "admin") fila.push(a.area);
    rows.push(fila);
  }

  const ws = XLSX.utils.aoa_to_sheet(rows);
  const wb = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(wb, ws, "Asistencia");

  const buf = XLSX.write(wb, { type: "buffer", bookType: "xlsx" });

  const nombreEval = fEval
    ? (filtrados[0]?.evaluacionTitulo ?? "evaluacion").replace(/[^a-zA-Z0-9áéíóúñÁÉÍÓÚÑ ]/g, "").slice(0, 40)
    : "todas";
  const filename = `asistencia-${nombreEval}.xlsx`.replace(/ /g, "-");

  return new NextResponse(buf, {
    headers: {
      "Content-Type":
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      "Content-Disposition": `attachment; filename=${filename}`,
    },
  });
}
