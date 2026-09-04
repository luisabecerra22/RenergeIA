"use client";

import { useState } from "react";
import type { Area, RolAdmin } from "@/lib/types";
import TablaConScrollDoble from "./TablaConScrollDoble";

interface AdminFila {
  username: string;
  rol: RolAdmin;
  area?: string;
  activo: boolean;
  creadoEn: string;
}

export default function GestionUsuarios({
  usuarioActual,
  adminsIniciales,
  areasIniciales,
}: {
  usuarioActual: string;
  adminsIniciales: AdminFila[];
  areasIniciales: Area[];
}) {
  const [admins, setAdmins] = useState(adminsIniciales);
  const [areas, setAreas] = useState(areasIniciales);
  const [error, setError] = useState<string | null>(null);
  const [passwordGenerada, setPasswordGenerada] = useState<{ para: string; valor: string } | null>(null);

  // Formulario: nueva cuenta.
  const [nuevoUsuario, setNuevoUsuario] = useState("");
  const [nuevoRol, setNuevoRol] = useState<RolAdmin>("area");
  const [nuevaArea, setNuevaArea] = useState(areas[0]?.id ?? "");
  const [creandoUsuario, setCreandoUsuario] = useState(false);

  // Formulario: nueva área.
  const [nombreArea, setNombreArea] = useState("");
  const [creandoArea, setCreandoArea] = useState(false);

  const areasMap = Object.fromEntries(areas.map((a) => [a.id, a.nombre]));

  async function crearUsuario(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    setCreandoUsuario(true);
    try {
      const res = await fetch("/api/admin/users", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          username: nuevoUsuario,
          rol: nuevoRol,
          area: nuevoRol === "area" ? nuevaArea : undefined,
        }),
      });
      const data = await res.json();
      if (!res.ok) throw new Error(data.error || "No se pudo crear la cuenta.");
      setAdmins((prev) => [...prev, data.admin]);
      setPasswordGenerada({ para: data.admin.username, valor: data.password });
      setNuevoUsuario("");
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setCreandoUsuario(false);
    }
  }

  async function crearArea(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    setCreandoArea(true);
    try {
      const res = await fetch("/api/admin/areas", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ nombre: nombreArea }),
      });
      const data = await res.json();
      if (!res.ok) throw new Error(data.error || "No se pudo crear el área.");
      setAreas((prev) => [...prev, data.area]);
      setNuevaArea(data.area.id);
      setNombreArea("");
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setCreandoArea(false);
    }
  }

  async function alternarActivo(username: string, activar: boolean) {
    setError(null);
    try {
      const res = await fetch(`/api/admin/users/${username}`, {
        method: "PATCH",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ accion: activar ? "activar" : "desactivar" }),
      });
      const data = await res.json();
      if (!res.ok) throw new Error(data.error || "No se pudo actualizar la cuenta.");
      setAdmins((prev) =>
        prev.map((a) => (a.username === username ? { ...a, activo: activar } : a)),
      );
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function resetearPassword(username: string) {
    if (!confirm(`¿Generar una contraseña nueva para "${username}"? La anterior dejará de funcionar.`))
      return;
    setError(null);
    try {
      const res = await fetch(`/api/admin/users/${username}`, {
        method: "PATCH",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ accion: "resetear-password" }),
      });
      const data = await res.json();
      if (!res.ok) throw new Error(data.error || "No se pudo resetear la contraseña.");
      setPasswordGenerada({ para: username, valor: data.password });
    } catch (e) {
      setError((e as Error).message);
    }
  }

  return (
    <>
      {error && <div className="alert alert-error">{error}</div>}

      {passwordGenerada && (
        <div className="alert alert-info">
          <strong>Contraseña para {passwordGenerada.para}:</strong>{" "}
          <code style={{ fontSize: 15 }}>{passwordGenerada.valor}</code>
          <br />
          Guárdala ahora — no se volverá a mostrar. Compártela con la persona por un canal seguro.
          <div style={{ marginTop: 8 }}>
            <button
              className="btn btn-secondary"
              style={{ padding: "6px 12px", fontSize: 13 }}
              onClick={() => {
                navigator.clipboard.writeText(passwordGenerada.valor);
              }}
            >
              Copiar
            </button>{" "}
            <button
              className="btn btn-secondary"
              style={{ padding: "6px 12px", fontSize: 13 }}
              onClick={() => setPasswordGenerada(null)}
            >
              Ya la guardé
            </button>
          </div>
        </div>
      )}

      <div className="card">
        <h2>Nueva cuenta</h2>
        <form onSubmit={crearUsuario} style={{ display: "flex", gap: 14, flexWrap: "wrap", alignItems: "flex-end" }}>
          <div className="field" style={{ minWidth: 200 }}>
            <label>Usuario</label>
            <input
              value={nuevoUsuario}
              onChange={(e) => setNuevoUsuario(e.target.value)}
              placeholder="ej. rrhh, jperez"
              required
            />
          </div>
          <div className="field" style={{ minWidth: 160 }}>
            <label>Rol</label>
            <select value={nuevoRol} onChange={(e) => setNuevoRol(e.target.value as RolAdmin)}>
              <option value="area">De área (acceso limitado)</option>
              <option value="admin">Admin (acceso total)</option>
            </select>
          </div>
          {nuevoRol === "area" && (
            <div className="field" style={{ minWidth: 200 }}>
              <label>Área</label>
              <select value={nuevaArea} onChange={(e) => setNuevaArea(e.target.value)}>
                {areas.length === 0 && <option value="">Crea un área primero</option>}
                {areas.map((a) => (
                  <option key={a.id} value={a.id}>
                    {a.nombre}
                  </option>
                ))}
              </select>
            </div>
          )}
          <button
            className="btn btn-primary"
            disabled={creandoUsuario || (nuevoRol === "area" && !nuevaArea)}
          >
            {creandoUsuario ? "Creando…" : "Crear cuenta"}
          </button>
        </form>
      </div>

      <div className="card">
        <h2>Áreas</h2>
        <p className="muted" style={{ marginTop: 0 }}>
          {areas.length === 0
            ? "No hay áreas creadas todavía."
            : areas.map((a) => a.nombre).join(", ")}
        </p>
        <form onSubmit={crearArea} style={{ display: "flex", gap: 14, alignItems: "flex-end" }}>
          <div className="field" style={{ minWidth: 200 }}>
            <label>Nueva área</label>
            <input
              value={nombreArea}
              onChange={(e) => setNombreArea(e.target.value)}
              placeholder="ej. Recursos Humanos"
              required
            />
          </div>
          <button className="btn btn-secondary" disabled={creandoArea}>
            {creandoArea ? "Creando…" : "+ Agregar área"}
          </button>
        </form>
      </div>

      <TablaConScrollDoble>
        <table className="tabla">
          <thead>
            <tr>
              <th>Usuario</th>
              <th>Rol</th>
              <th>Área</th>
              <th>Estado</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {admins.map((a) => (
              <tr key={a.username}>
                <td>{a.username}</td>
                <td>{a.rol === "admin" ? "Admin" : "De área"}</td>
                <td>{a.area ? areasMap[a.area] ?? a.area : "—"}</td>
                <td>
                  <span className={`badge ${a.activo ? "badge-ok" : "badge-fail"}`}>
                    {a.activo ? "Activa" : "Desactivada"}
                  </span>
                </td>
                <td style={{ display: "flex", gap: 8 }}>
                  <button
                    className="btn btn-secondary"
                    style={{ padding: "6px 12px", fontSize: 13 }}
                    onClick={() => resetearPassword(a.username)}
                  >
                    Resetear contraseña
                  </button>
                  {a.username !== usuarioActual && (
                    <button
                      className="btn btn-secondary"
                      style={{ padding: "6px 12px", fontSize: 13, color: a.activo ? "var(--rojo)" : "var(--verde-osc)" }}
                      onClick={() => alternarActivo(a.username, !a.activo)}
                    >
                      {a.activo ? "Desactivar" : "Activar"}
                    </button>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </TablaConScrollDoble>
    </>
  );
}
