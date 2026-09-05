/* eslint-disable @next/next/no-img-element */

import type { Sesion } from "@/lib/auth";
import TopbarActions from "./TopbarActions";

export default function Topbar({
  subtitulo,
  sesion,
}: {
  subtitulo?: string;
  sesion?: Sesion;
}) {
  return (
    <header className="topbar">
      <img src="/logo-renergeia.png" alt="Renergeia" />
      <div style={{ textAlign: "right" }}>
        <span className="sub">{subtitulo ?? "Sistema de Evaluaciones SST"}</span>
        {sesion && (
          <>
            <div className="user-info">
              {sesion.username} · {sesion.rol === "admin" ? "Administrador" : sesion.area}
            </div>
            <TopbarActions />
          </>
        )}
      </div>
    </header>
  );
}
