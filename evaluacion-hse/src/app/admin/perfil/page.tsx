import { redirect } from "next/navigation";
import Topbar from "@/components/Topbar";
import AdminNav from "@/components/AdminNav";
import CambiarPassword from "@/components/CambiarPassword";
import { sesionActual } from "@/lib/auth";

export const dynamic = "force-dynamic";

export default async function PerfilPage() {
  const sesion = await sesionActual();
  if (!sesion) redirect("/admin/login");

  return (
    <>
      <Topbar subtitulo="Panel de administración" sesion={sesion} />
      <main className="container">
        <AdminNav activo="perfil" sesion={sesion} />
        <h1>Mi cuenta</h1>
        <p className="muted">
          Usuario: <strong>{sesion.username}</strong> ·{" "}
          {sesion.rol === "admin" ? "Administrador" : `Área: ${sesion.area}`}
        </p>
        <CambiarPassword />
      </main>
    </>
  );
}
