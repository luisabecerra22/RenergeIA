"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";

export default function TopbarActions() {
  const router = useRouter();

  async function salir() {
    await fetch("/api/admin/logout", { method: "POST" });
    router.push("/admin/login");
    router.refresh();
  }

  return (
    <div style={{ display: "flex", gap: 8, marginTop: 4 }}>
      <Link className="btn btn-secondary" href="/admin/perfil" style={{ fontSize: 12, padding: "4px 10px" }}>
        Mi cuenta
      </Link>
      <button className="btn btn-secondary" onClick={salir} style={{ fontSize: 12, padding: "4px 10px" }}>
        Cerrar sesión
      </button>
    </div>
  );
}
