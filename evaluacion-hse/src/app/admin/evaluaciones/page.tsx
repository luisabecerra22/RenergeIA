import { redirect } from "next/navigation";
import Topbar from "@/components/Topbar";
import AdminNav from "@/components/AdminNav";
import ListaEvaluaciones from "@/components/ListaEvaluaciones";
import { getStore } from "@/lib/db";
import { sesionActual } from "@/lib/auth";

export const dynamic = "force-dynamic";

export default async function AdminEvaluaciones() {
  const sesion = await sesionActual();
  if (!sesion) redirect("/admin/login");

  const store = await getStore();
  const [todasEvaluaciones, areas] = await Promise.all([
    store.listEvaluaciones(),
    store.listAreas(),
  ]);

  const esAdmin = sesion.rol === "admin";
  const evaluaciones = esAdmin
    ? todasEvaluaciones
    : todasEvaluaciones.filter((e) => e.area === sesion.area);
  const areasMap = Object.fromEntries(areas.map((a) => [a.id, a.nombre]));

  return (
    <>
      <Topbar subtitulo="Panel de administración" sesion={sesion} />
      <main className="container-wide">
        <AdminNav activo="evaluaciones" sesion={sesion} />
        <h1>Evaluaciones</h1>
        <p className="muted">
          Crea, edita y activa las evaluaciones que verán los participantes.
        </p>
        <ListaEvaluaciones
          evaluaciones={evaluaciones.map((e) => ({
            id: e.id,
            titulo: e.titulo,
            tema: e.tema,
            activa: e.activa,
            area: e.area,
            numPreguntas: e.preguntas.length,
          }))}
          areasMap={areasMap}
          esAdmin={esAdmin}
        />
      </main>
    </>
  );
}
