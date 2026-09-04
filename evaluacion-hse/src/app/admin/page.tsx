import { redirect } from "next/navigation";
import Topbar from "@/components/Topbar";
import AdminNav from "@/components/AdminNav";
import TablaResultados from "@/components/TablaResultados";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";

export const dynamic = "force-dynamic";

export default async function AdminDashboard() {
  const sesion = await sesionActual();
  if (!sesion) redirect("/admin/login");

  const store = await getStore();
  const [todosIntentos, todasEvaluaciones, areas] = await Promise.all([
    store.listIntentos(),
    store.listEvaluaciones(),
    store.listAreas(),
  ]);

  const esAdmin = sesion.rol === "admin";
  const intentos = esAdmin ? todosIntentos : todosIntentos.filter((i) => i.area === sesion.area);
  const evaluaciones = esAdmin
    ? todasEvaluaciones
    : todasEvaluaciones.filter((e) => e.area === sesion.area);
  const areasMap = Object.fromEntries(areas.map((a) => [a.id, a.nombre]));

  return (
    <>
      <Topbar subtitulo="Panel de administración" />
      <main className="container-wide">
        <AdminNav activo="resultados" sesion={sesion} />
        <h1>Resultados de evaluaciones</h1>
        <TablaResultados
          intentos={intentos}
          evaluaciones={evaluaciones}
          areasMap={areasMap}
          esAdmin={esAdmin}
        />
      </main>
    </>
  );
}
