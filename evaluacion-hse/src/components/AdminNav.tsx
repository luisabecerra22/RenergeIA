"use client";

import Link from "next/link";
import type { Sesion } from "@/lib/auth";

export default function AdminNav({
  activo,
  sesion,
}: {
  activo: "resultados" | "evaluaciones" | "dashboard" | "asistencia" | "usuarios" | "perfil";
  sesion: Sesion;
}) {
  return (
    <nav
      style={{
        display: "flex",
        gap: 8,
        alignItems: "center",
        marginBottom: 22,
        flexWrap: "wrap",
      }}
    >
      <Link
        className={`btn ${activo === "resultados" ? "btn-primary" : "btn-secondary"}`}
        href="/admin"
      >
        Resultados
      </Link>
      <Link
        className={`btn ${activo === "evaluaciones" ? "btn-primary" : "btn-secondary"}`}
        href="/admin/evaluaciones"
      >
        Evaluaciones
      </Link>
      <Link
        className={`btn ${activo === "dashboard" ? "btn-primary" : "btn-secondary"}`}
        href="/admin/dashboard"
      >
        Dashboard
      </Link>
      <Link
        className={`btn ${activo === "asistencia" ? "btn-primary" : "btn-secondary"}`}
        href="/admin/asistencia"
      >
        Asistencia
      </Link>
      {sesion.rol === "admin" && (
        <Link
          className={`btn ${activo === "usuarios" ? "btn-primary" : "btn-secondary"}`}
          href="/admin/usuarios"
        >
          Usuarios
        </Link>
      )}
    </nav>
  );
}
