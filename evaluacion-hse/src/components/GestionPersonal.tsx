"use client";

import { useState, useMemo, useRef } from "react";
import { useRouter } from "next/navigation";
import type { Persona } from "@/lib/types";
import TablaConScrollDoble from "./TablaConScrollDoble";

interface EvalRef { id: string; titulo: string; }
interface AsistRef { cedula: string; evaluacionId: string; }

export default function GestionPersonal({
  personalInicial,
  evaluaciones,
  asistencias,
}: {
  personalInicial: Persona[];
  evaluaciones: EvalRef[];
  asistencias: AsistRef[];
}) {
  const [personal, setPersonal] = useState(personalInicial);
  const [error, setError] = useState<string | null>(null);
  const [exito, setExito] = useState<string | null>(null);
  const [importando, setImportando] = useState(false);
  const [busqueda, setBusqueda] = useState("");
  const [filtroArea, setFiltroArea] = useState("");
  const [filtroCargo, setFiltroCargo] = useState("");
  const [filtroProyecto, setFiltroProyecto] = useState("");
  const [filtroTrabajo, setFiltroTrabajo] = useState("");
  const [evalsActivas, setEvalsActivas] = useState<Set<string>>(() => new Set(evaluaciones.map((e) => e.id)));
  const [filtroAsist, setFiltroAsist] = useState<{ evalId: string; modo: "asistio" | "falto" } | null>(null);
  const [editando, setEditando] = useState<string | null>(null);
  const [formEdit, setFormEdit] = useState<Partial<Persona>>({});
  const fileRef = useRef<HTMLInputElement>(null);

  // Form nueva persona
  const [showForm, setShowForm] = useState(false);
  const [formNuevo, setFormNuevo] = useState({ cedula: "", nombre: "", apellido: "", tipoDocumento: "Cédula de ciudadanía", correo: "", area: "", cargo: "", proyecto: "", fechaContratacion: "", trabajo: "Presencial" });
  const [creando, setCreando] = useState(false);
  const [actualizando, setActualizando] = useState(false);
  const router = useRouter();

  async function actualizarDatos() {
    setActualizando(true);
    setError(null);
    try {
      // Refresca asistencias/evaluaciones (props del servidor) y el listado de personal.
      router.refresh();
      const resPersonal = await fetch("/api/admin/personal", { cache: "no-store" });
      if (resPersonal.ok) setPersonal(await resPersonal.json());
      setExito("Información actualizada.");
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setActualizando(false);
    }
  }

  const asistenciaPorCedula = useMemo(() => {
    const m = new Map<string, Set<string>>();
    for (const a of asistencias) {
      if (!m.has(a.cedula)) m.set(a.cedula, new Set());
      m.get(a.cedula)!.add(a.evaluacionId);
    }
    return m;
  }, [asistencias]);

  const evsFiltradas = evaluaciones.filter((e) => evalsActivas.has(e.id));

  function toggleEval(id: string) {
    setEvalsActivas((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
    // Si se apaga la evaluación que estaba filtrando por asistencia, limpiar ese filtro.
    setFiltroAsist((f) => (f && f.evalId === id ? null : f));
  }

  // Opciones únicas para los filtros desplegables.
  const opciones = useMemo(() => {
    const uniq = (vals: string[]) => Array.from(new Set(vals.filter((v) => v && v.trim()))).sort((a, b) => a.localeCompare(b));
    return {
      areas: uniq(personal.map((p) => p.area)),
      cargos: uniq(personal.map((p) => p.cargo)),
      proyectos: uniq(personal.map((p) => p.proyecto)),
      trabajos: uniq(personal.map((p) => p.trabajo)),
    };
  }, [personal]);

  const personalFiltrado = useMemo(() => {
    const q = busqueda.trim().toLowerCase();
    return personal.filter((p) => {
      if (q && !`${p.nombre} ${p.apellido} ${p.cedula} ${p.cargo} ${p.area}`.toLowerCase().includes(q)) return false;
      if (filtroArea && p.area !== filtroArea) return false;
      if (filtroCargo && p.cargo !== filtroCargo) return false;
      if (filtroProyecto && p.proyecto !== filtroProyecto) return false;
      if (filtroTrabajo && p.trabajo !== filtroTrabajo) return false;
      return true;
    }).sort((a, b) => `${a.nombre} ${a.apellido}`.localeCompare(`${b.nombre} ${b.apellido}`, "es", { sensitivity: "base" }));
  }, [personal, busqueda, filtroArea, filtroCargo, filtroProyecto, filtroTrabajo]);

  // Lista mostrada en la tabla, aplicando además el filtro por asistencia (✓/✗) de una evaluación.
  const personasMostradas = useMemo(() => {
    if (!filtroAsist) return personalFiltrado;
    return personalFiltrado.filter((p) => {
      const asistio = asistenciaPorCedula.get(p.cedula)?.has(filtroAsist.evalId) ?? false;
      return filtroAsist.modo === "asistio" ? asistio : !asistio;
    });
  }, [personalFiltrado, filtroAsist, asistenciaPorCedula]);

  async function importarExcel() {
    const file = fileRef.current?.files?.[0];
    if (!file) return;
    setImportando(true);
    setError(null);
    setExito(null);
    try {
      const fd = new FormData();
      fd.append("archivo", file);
      const res = await fetch("/api/admin/personal/import", { method: "POST", body: fd });
      const data = await res.json();
      if (!res.ok) throw new Error(data.error);
      setExito(`Se importaron ${data.importados} personas correctamente.`);
      const resPersonal = await fetch("/api/admin/personal");
      setPersonal(await resPersonal.json());
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setImportando(false);
      if (fileRef.current) fileRef.current.value = "";
    }
  }

  async function crearPersona(e: React.FormEvent) {
    e.preventDefault();
    setCreando(true);
    setError(null);
    try {
      const res = await fetch("/api/admin/personal", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(formNuevo),
      });
      const data = await res.json();
      if (!res.ok) throw new Error(data.error);
      setPersonal((prev) => [...prev, data]);
      setFormNuevo({ cedula: "", nombre: "", apellido: "", tipoDocumento: "Cédula de ciudadanía", correo: "", area: "", cargo: "", proyecto: "", fechaContratacion: "", trabajo: "Presencial" });
      setShowForm(false);
      setExito("Persona agregada correctamente.");
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setCreando(false);
    }
  }

  async function guardarEdicion(cedula: string) {
    setError(null);
    try {
      const res = await fetch(`/api/admin/personal/${cedula}`, {
        method: "PATCH",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(formEdit),
      });
      const data = await res.json();
      if (!res.ok) throw new Error(data.error);
      setPersonal((prev) => prev.map((p) => (p.cedula === cedula ? data : p)));
      setEditando(null);
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function eliminarPersona(cedula: string) {
    if (!confirm(`¿Eliminar a la persona con cédula ${cedula}?`)) return;
    setError(null);
    try {
      const res = await fetch(`/api/admin/personal/${cedula}`, { method: "DELETE" });
      if (!res.ok) throw new Error("No se pudo eliminar.");
      setPersonal((prev) => prev.filter((p) => p.cedula !== cedula));
    } catch (e) {
      setError((e as Error).message);
    }
  }

  return (
    <>
      {error && <div className="alert alert-error">{error}</div>}
      {exito && <div className="alert alert-info">{exito} <button className="btn btn-secondary" style={{ padding: "2px 8px", fontSize: 12, marginLeft: 8 }} onClick={() => setExito(null)}>OK</button></div>}

      {/* Importar y agregar */}
      <div className="card" style={{ display: "flex", gap: 14, flexWrap: "wrap", alignItems: "center" }}>
        <div>
          <label style={{ fontSize: 13, fontWeight: 600 }}>Importar desde Excel</label>
          <div style={{ display: "flex", gap: 8, marginTop: 4 }}>
            <input ref={fileRef} type="file" accept=".xlsx,.xls,.csv" style={{ fontSize: 13 }} />
            <button className="btn btn-primary" onClick={importarExcel} disabled={importando} style={{ whiteSpace: "nowrap" }}>
              {importando ? "Importando…" : "Importar"}
            </button>
          </div>
        </div>
        <div style={{ marginLeft: "auto" }}>
          <button className="btn btn-secondary" onClick={() => setShowForm(!showForm)}>
            {showForm ? "Cancelar" : "+ Agregar persona"}
          </button>
        </div>
      </div>

      {showForm && (
        <div className="card">
          <h3 style={{ marginTop: 0 }}>Nueva persona</h3>
          <form onSubmit={crearPersona} style={{ display: "flex", gap: 10, flexWrap: "wrap", alignItems: "flex-end" }}>
            <div className="field" style={{ minWidth: 140 }}>
              <label>Cédula</label>
              <input value={formNuevo.cedula} onChange={(e) => setFormNuevo({ ...formNuevo, cedula: e.target.value })} required />
            </div>
            <div className="field" style={{ minWidth: 140 }}>
              <label>Nombre</label>
              <input value={formNuevo.nombre} onChange={(e) => setFormNuevo({ ...formNuevo, nombre: e.target.value })} required />
            </div>
            <div className="field" style={{ minWidth: 140 }}>
              <label>Apellido</label>
              <input value={formNuevo.apellido} onChange={(e) => setFormNuevo({ ...formNuevo, apellido: e.target.value })} required />
            </div>
            <div className="field" style={{ minWidth: 140 }}>
              <label>Correo</label>
              <input value={formNuevo.correo} onChange={(e) => setFormNuevo({ ...formNuevo, correo: e.target.value })} />
            </div>
            <div className="field" style={{ minWidth: 140 }}>
              <label>Área</label>
              <input value={formNuevo.area} onChange={(e) => setFormNuevo({ ...formNuevo, area: e.target.value })} />
            </div>
            <div className="field" style={{ minWidth: 140 }}>
              <label>Cargo</label>
              <input value={formNuevo.cargo} onChange={(e) => setFormNuevo({ ...formNuevo, cargo: e.target.value })} />
            </div>
            <div className="field" style={{ minWidth: 140 }}>
              <label>Proyecto</label>
              <input value={formNuevo.proyecto} onChange={(e) => setFormNuevo({ ...formNuevo, proyecto: e.target.value })} />
            </div>
            <button className="btn btn-primary" disabled={creando}>{creando ? "Guardando…" : "Guardar"}</button>
          </form>
        </div>
      )}

      {/* Búsqueda + controles */}
      <div style={{ display: "flex", gap: 14, alignItems: "center", marginBottom: 12, flexWrap: "wrap" }}>
        <input
          placeholder="Buscar por nombre, cédula, cargo..."
          value={busqueda}
          onChange={(e) => setBusqueda(e.target.value)}
          style={{ flex: 1, minWidth: 200, padding: "8px 12px", border: "1px solid var(--gris-borde)", borderRadius: 6 }}
        />
        <button className="btn btn-secondary" onClick={actualizarDatos} disabled={actualizando} style={{ whiteSpace: "nowrap" }}>
          {actualizando ? "Actualizando…" : "↻ Actualizar"}
        </button>
        <a href="/api/admin/personal/export" className="btn btn-primary" style={{ textDecoration: "none", whiteSpace: "nowrap" }}>
          Exportar a Excel
        </a>
        <span className="muted" style={{ fontSize: 13 }}>{personasMostradas.length} de {personal.length} personas</span>
      </div>

      {/* Filtros por columna */}
      <div style={{ display: "flex", gap: 10, alignItems: "center", marginBottom: 12, flexWrap: "wrap" }}>
        <span style={{ fontSize: 13, fontWeight: 600 }}>Filtrar por:</span>
        <select value={filtroArea} onChange={(e) => setFiltroArea(e.target.value)} style={{ padding: "6px 10px", maxWidth: 200 }}>
          <option value="">Área (todas)</option>
          {opciones.areas.map((v) => <option key={v} value={v}>{v}</option>)}
        </select>
        <select value={filtroCargo} onChange={(e) => setFiltroCargo(e.target.value)} style={{ padding: "6px 10px", maxWidth: 200 }}>
          <option value="">Cargo (todos)</option>
          {opciones.cargos.map((v) => <option key={v} value={v}>{v}</option>)}
        </select>
        <select value={filtroProyecto} onChange={(e) => setFiltroProyecto(e.target.value)} style={{ padding: "6px 10px", maxWidth: 200 }}>
          <option value="">Proyecto (todos)</option>
          {opciones.proyectos.map((v) => <option key={v} value={v}>{v}</option>)}
        </select>
        <select value={filtroTrabajo} onChange={(e) => setFiltroTrabajo(e.target.value)} style={{ padding: "6px 10px", maxWidth: 200 }}>
          <option value="">Trabajo (todos)</option>
          {opciones.trabajos.map((v) => <option key={v} value={v}>{v}</option>)}
        </select>
        {(filtroArea || filtroCargo || filtroProyecto || filtroTrabajo) && (
          <button type="button" className="btn btn-secondary" style={{ padding: "4px 10px", fontSize: 12 }} onClick={() => { setFiltroArea(""); setFiltroCargo(""); setFiltroProyecto(""); setFiltroTrabajo(""); }}>
            Limpiar filtros
          </button>
        )}
      </div>

      {/* Prender / apagar evaluaciones (afecta columnas visibles y % de asistencia) */}
      {evaluaciones.length > 0 && (
        <div style={{ display: "flex", gap: 8, alignItems: "center", marginBottom: 16, flexWrap: "wrap" }}>
          <span style={{ fontSize: 13, fontWeight: 600 }}>Mostrar evaluaciones:</span>
          {evaluaciones.map((ev) => {
            const activa = evalsActivas.has(ev.id);
            return (
              <button
                key={ev.id}
                type="button"
                onClick={() => toggleEval(ev.id)}
                title={activa ? "Clic para ocultar esta evaluación" : "Clic para mostrar esta evaluación"}
                style={{
                  cursor: "pointer",
                  border: activa ? "1px solid var(--verde)" : "1px solid var(--gris-borde)",
                  background: activa ? "var(--verde)" : "#fff",
                  color: activa ? "#fff" : "var(--texto)",
                  borderRadius: 999,
                  padding: "5px 12px",
                  fontSize: 12,
                  fontWeight: 600,
                  maxWidth: 240,
                  overflow: "hidden",
                  textOverflow: "ellipsis",
                  whiteSpace: "nowrap",
                  opacity: activa ? 1 : 0.6,
                }}
              >
                {activa ? "✓ " : "○ "}{ev.titulo}
              </button>
            );
          })}
          <button
            type="button"
            onClick={() => setEvalsActivas(new Set(evaluaciones.map((e) => e.id)))}
            className="btn btn-secondary"
            style={{ padding: "4px 10px", fontSize: 12 }}
          >Todas</button>
          <button
            type="button"
            onClick={() => { setEvalsActivas(new Set()); setFiltroAsist(null); }}
            className="btn btn-secondary"
            style={{ padding: "4px 10px", fontSize: 12 }}
          >Ninguna</button>
        </div>
      )}

      {filtroAsist && (
        <div className="alert alert-info" style={{ display: "flex", alignItems: "center", gap: 10, flexWrap: "wrap" }}>
          <span>
            Mostrando solo quienes {filtroAsist.modo === "asistio" ? "asistieron a" : "NO asistieron a"}{" "}
            <strong>{evaluaciones.find((e) => e.id === filtroAsist.evalId)?.titulo}</strong> — {personasMostradas.length} persona{personasMostradas.length !== 1 ? "s" : ""}
          </span>
          <button className="btn btn-secondary" style={{ padding: "2px 10px", fontSize: 12 }} onClick={() => setFiltroAsist(null)}>Ver todas</button>
        </div>
      )}

      {/* Tabla única: personal + asistencia a evaluaciones */}
      {personal.length > 0 && (
        <TablaConScrollDoble>
          <table className="tabla">
            <thead>
              <tr>
                <th>Nombre</th>
                <th>Apellido</th>
                <th>Cédula</th>
                <th>Correo</th>
                <th>Área</th>
                <th>Cargo</th>
                <th>Proyecto</th>
                <th>Trabajo</th>
                {evsFiltradas.map((ev) => {
                  const activoAsistio = filtroAsist?.evalId === ev.id && filtroAsist.modo === "asistio";
                  const activoFalto = filtroAsist?.evalId === ev.id && filtroAsist.modo === "falto";
                  return (
                    <th key={ev.id} style={{ minWidth: 120, maxWidth: 150, fontSize: 12, fontWeight: 600, whiteSpace: "normal", wordBreak: "break-word", lineHeight: 1.3, verticalAlign: "middle", textAlign: "center", padding: "10px 8px" }}>
                      <div>{ev.titulo}</div>
                      <div style={{ display: "flex", gap: 4, justifyContent: "center", marginTop: 6 }}>
                        <button
                          type="button"
                          title="Ver solo quienes asistieron"
                          onClick={() => setFiltroAsist(activoAsistio ? null : { evalId: ev.id, modo: "asistio" })}
                          style={{ cursor: "pointer", border: "none", borderRadius: 4, padding: "2px 8px", fontWeight: 700, fontSize: 12, lineHeight: 1, background: activoAsistio ? "var(--verde)" : "rgba(255,255,255,0.2)", color: "#fff" }}
                        >✓</button>
                        <button
                          type="button"
                          title="Ver solo quienes NO asistieron"
                          onClick={() => setFiltroAsist(activoFalto ? null : { evalId: ev.id, modo: "falto" })}
                          style={{ cursor: "pointer", border: "none", borderRadius: 4, padding: "2px 8px", fontWeight: 700, fontSize: 12, lineHeight: 1, background: activoFalto ? "#d32f2f" : "rgba(255,255,255,0.2)", color: "#fff" }}
                        >✗</button>
                      </div>
                    </th>
                  );
                })}
                {evsFiltradas.length > 0 && <th style={{ verticalAlign: "middle", textAlign: "center" }}>% Asist.</th>}
                <th>Acciones</th>
              </tr>
            </thead>
            <tbody>
              {personasMostradas.map((p) => {
                const cedulaAsist = asistenciaPorCedula.get(p.cedula) ?? new Set<string>();
                let total = 0;
                const esEdit = editando === p.cedula;
                return (
                  <tr key={p.cedula}>
                    {esEdit ? (
                      <>
                        <td><input value={formEdit.nombre ?? ""} onChange={(e) => setFormEdit({ ...formEdit, nombre: e.target.value })} style={{ width: 100 }} /></td>
                        <td><input value={formEdit.apellido ?? ""} onChange={(e) => setFormEdit({ ...formEdit, apellido: e.target.value })} style={{ width: 100 }} /></td>
                        <td>{p.cedula}</td>
                        <td><input value={formEdit.correo ?? ""} onChange={(e) => setFormEdit({ ...formEdit, correo: e.target.value })} style={{ width: 120 }} /></td>
                        <td><input value={formEdit.area ?? ""} onChange={(e) => setFormEdit({ ...formEdit, area: e.target.value })} style={{ width: 80 }} /></td>
                        <td><input value={formEdit.cargo ?? ""} onChange={(e) => setFormEdit({ ...formEdit, cargo: e.target.value })} style={{ width: 100 }} /></td>
                        <td><input value={formEdit.proyecto ?? ""} onChange={(e) => setFormEdit({ ...formEdit, proyecto: e.target.value })} style={{ width: 100 }} /></td>
                        <td><input value={formEdit.trabajo ?? ""} onChange={(e) => setFormEdit({ ...formEdit, trabajo: e.target.value })} style={{ width: 80 }} /></td>
                      </>
                    ) : (
                      <>
                        <td>{p.nombre}</td>
                        <td>{p.apellido}</td>
                        <td>{p.cedula}</td>
                        <td>{p.correo}</td>
                        <td>{p.area}</td>
                        <td>{p.cargo}</td>
                        <td>{p.proyecto}</td>
                        <td>{p.trabajo}</td>
                      </>
                    )}
                    {evsFiltradas.map((ev) => {
                      const asistio = cedulaAsist.has(ev.id);
                      if (asistio) total++;
                      return (
                        <td key={ev.id} style={{
                          textAlign: "center",
                          fontWeight: 700,
                          color: asistio ? "var(--verde-osc)" : "#d32f2f",
                          background: asistio ? undefined : "rgba(211,47,47,0.06)",
                        }}>
                          {asistio ? "✓" : "✗"}
                        </td>
                      );
                    })}
                    {evsFiltradas.length > 0 && (
                      <td style={{
                        fontWeight: 700,
                        textAlign: "center",
                        color: total === evsFiltradas.length ? "var(--verde-osc)" : total === 0 ? "#d32f2f" : "var(--texto)",
                      }}>
                        {`${Math.round((total / evsFiltradas.length) * 100)}%`}
                      </td>
                    )}
                    <td>
                      {esEdit ? (
                        <div style={{ display: "flex", gap: 4 }}>
                          <button className="btn btn-primary" style={{ padding: "4px 8px", fontSize: 12 }} onClick={() => guardarEdicion(p.cedula)}>Guardar</button>
                          <button className="btn btn-secondary" style={{ padding: "4px 8px", fontSize: 12 }} onClick={() => setEditando(null)}>Cancelar</button>
                        </div>
                      ) : (
                        <div style={{ display: "flex", gap: 4 }}>
                          <button className="btn btn-secondary" style={{ padding: "4px 8px", fontSize: 12 }} onClick={() => { setEditando(p.cedula); setFormEdit({ nombre: p.nombre, apellido: p.apellido, correo: p.correo, area: p.area, cargo: p.cargo, proyecto: p.proyecto, trabajo: p.trabajo }); }}>Editar</button>
                          <button className="btn btn-secondary" style={{ padding: "4px 8px", fontSize: 12, color: "var(--rojo)" }} onClick={() => eliminarPersona(p.cedula)}>Eliminar</button>
                        </div>
                      )}
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>
        </TablaConScrollDoble>
      )}
    </>
  );
}
