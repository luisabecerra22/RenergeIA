import { NextResponse } from "next/server";
import { getStore } from "@/lib/db";
import { sesionActual, puedeVerArea } from "@/lib/auth";
import * as XLSX from "xlsx";

export const runtime = "nodejs";
export const dynamic = "force-dynamic";

export async function GET() {
  const sesion = await sesionActual();
  if (!sesion) return NextResponse.json({ error: "No autorizado." }, { status: 401 });

  const store = await getStore();
  const [personal, todasEvals, todasAsist, todosIntentos] = await Promise.all([
    store.listPersonal(),
    store.listEvaluaciones(),
    store.listAsistencias(),
    store.listIntentos(),
  ]);

  const esAdmin = sesion.rol === "admin";
  const evaluaciones = esAdmin ? todasEvals : todasEvals.filter((e) => puedeVerArea(sesion, e.area));
  const asistencias = esAdmin ? todasAsist : todasAsist.filter((a) => puedeVerArea(sesion, a.area));
  const intentos = esAdmin ? todosIntentos : todosIntentos.filter((i) => puedeVerArea(sesion, i.area));

  // Cuenta como asistente si tiene asistencia registrada O presentó la evaluación (intento).
  const asistenciaPorCedula = new Map<string, Set<string>>();
  const marcar = (cedula: string, evaluacionId: string) => {
    if (!asistenciaPorCedula.has(cedula)) asistenciaPorCedula.set(cedula, new Set());
    asistenciaPorCedula.get(cedula)!.add(evaluacionId);
  };
  for (const a of asistencias) marcar(a.cedula, a.evaluacionId);
  for (const i of intentos) marcar(i.participante.cedula, i.evaluacionId);

  const headers = ["Nombre", "Apellido", "Cédula", "Cargo", "Área", "Proyecto"];
  for (const ev of evaluaciones) headers.push(ev.titulo);
  headers.push("% Asistencia");

  const rows: string[][] = [headers];
  for (const p of personal) {
    const fila: string[] = [p.nombre, p.apellido, p.cedula, p.cargo, p.area, p.proyecto];
    const cedulaAsist = asistenciaPorCedula.get(p.cedula) ?? new Set();
    let asistio = 0;
    for (const ev of evaluaciones) {
      if (cedulaAsist.has(ev.id)) {
        fila.push("✓");
        asistio++;
      } else {
        fila.push("✗");
      }
    }
    const pct = evaluaciones.length > 0 ? Math.round((asistio / evaluaciones.length) * 100) : 0;
    fila.push(`${pct}%`);
    rows.push(fila);
  }

  const ws = XLSX.utils.aoa_to_sheet(rows);
  const wb = XLSX.utils.book_new();
  XLSX.utils.book_append_sheet(wb, ws, "Matriz Asistencia");

  // Hojas adicionales: ausentes por evaluación
  for (const ev of evaluaciones) {
    const ausentes = personal.filter((p) => {
      const c = asistenciaPorCedula.get(p.cedula);
      return !c || !c.has(ev.id);
    });
    const hRows: string[][] = [["Nombre", "Apellido", "Cédula", "Cargo", "Área"]];
    for (const a of ausentes) hRows.push([a.nombre, a.apellido, a.cedula, a.cargo, a.area]);
    const hWs = XLSX.utils.aoa_to_sheet(hRows);
    const sheetName = ev.titulo.slice(0, 28).replace(/[:\\/?*[\]]/g, "");
    XLSX.utils.book_append_sheet(wb, hWs, `No - ${sheetName}`);
  }

  const buf = XLSX.write(wb, { type: "buffer", bookType: "xlsx" });

  return new NextResponse(buf, {
    headers: {
      "Content-Type": "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
      "Content-Disposition": "attachment; filename=matriz-asistencia.xlsx",
    },
  });
}
