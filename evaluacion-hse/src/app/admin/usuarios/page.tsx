import { redirect } from "next/navigation";
import Topbar from "@/components/Topbar";
import AdminNav from "@/components/AdminNav";
import GestionUsuarios from "@/components/GestionUsuarios";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";

export const dynamic = "force-dynamic";

export default async function UsuariosPage() {
  const sesion = await sesionActual();
  if (!sesion) redirect("/admin/login");
  if (sesion.rol !== "admin") redirect("/admin");

  const store = await getStore();
  const [admins, areas] = await Promise.all([store.listAdmins(), store.listAreas()]);

  return (
    <>
      <Topbar subtitulo="Panel de administración" sesion={sesion} />
      <main className="container-wide">
        <AdminNav activo="usuarios" sesion={sesion} />
        <h1>Usuarios</h1>
        <p className="muted">
          Crea cuentas por área (solo ven y gestionan sus propias capacitaciones) o cuentas
          admin (ven y gestionan todo).
        </p>
        <GestionUsuarios
          usuarioActual={sesion.username}
          adminsIniciales={admins.map(({ username, rol, area, activo, creadoEn }) => ({
            username,
            rol,
            area,
            activo,
            creadoEn,
          }))}
          areasIniciales={areas}
        />
      </main>
    </>
  );
}
