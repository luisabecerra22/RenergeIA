import { redirect } from "next/navigation";
import Topbar from "@/components/Topbar";
import AdminNav from "@/components/AdminNav";
import TablaAsistencia from "@/components/TablaAsistencia";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";

export const dynamic = "force-dynamic";

export default async function AsistenciaAdminPage() {
  const sesion = await sesionActual();
  if (!sesion) redirect("/admin/login");

  const store = await getStore();
  const [todasAsistencias, todasEvaluaciones, areas] = await Promise.all([
    store.listAsistencias(),
    store.listEvaluaciones(),
    store.listAreas(),
  ]);

  const esAdmin = sesion.rol === "admin";
  const asistencias = esAdmin
    ? todasAsistencias
    : todasAsistencias.filter((a) => a.area === sesion.area);
  const evaluaciones = esAdmin
    ? todasEvaluaciones
    : todasEvaluaciones.filter((e) => e.area === sesion.area);
  const areasMap = Object.fromEntries(areas.map((a) => [a.id, a.nombre]));

  return (
    <>
      <Topbar subtitulo="Panel de administración" sesion={sesion} />
      <main className="container-wide">
        <AdminNav activo="asistencia" sesion={sesion} />
        <h1>Registro de asistencia</h1>
        <TablaAsistencia
          asistencias={asistencias}
          evaluaciones={evaluaciones}
          areasMap={areasMap}
          esAdmin={esAdmin}
        />
      </main>
    </>
  );
}
