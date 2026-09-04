"use client";

import { useState } from "react";

export default function CambiarPassword() {
  const [actual, setActual] = useState("");
  const [nueva, setNueva] = useState("");
  const [confirmar, setConfirmar] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [ok, setOk] = useState(false);
  const [guardando, setGuardando] = useState(false);

  async function guardar(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    setOk(false);
    if (nueva !== confirmar) {
      setError("La confirmación no coincide con la contraseña nueva.");
      return;
    }
    setGuardando(true);
    try {
      const res = await fetch("/api/admin/me/password", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ actual, nueva }),
      });
      const data = await res.json();
      if (!res.ok) throw new Error(data.error || "No se pudo cambiar la contraseña.");
      setOk(true);
      setActual("");
      setNueva("");
      setConfirmar("");
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setGuardando(false);
    }
  }

  return (
    <div className="card" style={{ maxWidth: 420 }}>
      <h2>Cambiar contraseña</h2>
      {error && <div className="alert alert-error">{error}</div>}
      {ok && <div className="alert alert-info">Contraseña actualizada correctamente.</div>}
      <form onSubmit={guardar}>
        <div className="field">
          <label>Contraseña actual</label>
          <input
            type="password"
            value={actual}
            onChange={(e) => setActual(e.target.value)}
            autoComplete="current-password"
            required
          />
        </div>
        <div className="field">
          <label>Contraseña nueva (mínimo 8 caracteres)</label>
          <input
            type="password"
            value={nueva}
            onChange={(e) => setNueva(e.target.value)}
            autoComplete="new-password"
            minLength={8}
            required
          />
        </div>
        <div className="field">
          <label>Confirmar contraseña nueva</label>
          <input
            type="password"
            value={confirmar}
            onChange={(e) => setConfirmar(e.target.value)}
            autoComplete="new-password"
            minLength={8}
            required
          />
        </div>
        <button className="btn btn-primary" disabled={guardando}>
          {guardando ? "Guardando…" : "Cambiar contraseña"}
        </button>
      </form>
    </div>
  );
}
