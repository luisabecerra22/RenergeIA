import { redirect } from "next/navigation";
import Topbar from "@/components/Topbar";
import AdminNav from "@/components/AdminNav";
import DashboardCharts from "@/components/DashboardCharts";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";

export const dynamic = "force-dynamic";

export default async function DashboardPage() {
  const sesion = await sesionActual();
  if (!sesion) redirect("/admin/login");

  const store = await getStore();
  const [todosIntentos, todasEvaluaciones, todasAsistencias] = await Promise.all([
    store.listIntentos(),
    store.listEvaluaciones(),
    store.listAsistencias(),
  ]);

  const esAdmin = sesion.rol === "admin";
  const intentos = esAdmin ? todosIntentos : todosIntentos.filter((i) => i.area === sesion.area);
  const evaluaciones = esAdmin
    ? todasEvaluaciones
    : todasEvaluaciones.filter((e) => e.area === sesion.area);
  const asistencias = esAdmin
    ? todasAsistencias
    : todasAsistencias.filter((a) => a.area === sesion.area);

  return (
    <>
      <Topbar subtitulo="Panel de administración" sesion={sesion} />
      <main className="container-wide">
        <AdminNav activo="dashboard" sesion={sesion} />
        <h1>Dashboard</h1>
        <DashboardCharts
          intentos={intentos}
          evaluaciones={evaluaciones}
          asistencias={asistencias}
        />
      </main>
    </>
  );
}
