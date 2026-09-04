import { notFound, redirect } from "next/navigation";
import Topbar from "@/components/Topbar";
import AdminNav from "@/components/AdminNav";
import EditorEvaluacion from "@/components/EditorEvaluacion";
import { getStore } from "@/lib/db";
import { puedeVerArea, sesionActual } from "@/lib/auth";

export const dynamic = "force-dynamic";

export default async function EditarEvaluacion({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const sesion = await sesionActual();
  if (!sesion) redirect("/admin/login");

  const { id } = await params;
  const store = await getStore();
  const [evaluacion, areas] = await Promise.all([
    store.getEvaluacion(id),
    store.listAreas(),
  ]);
  if (!evaluacion) notFound();
  if (!puedeVerArea(sesion, evaluacion.area)) notFound();

  return (
    <>
      <Topbar subtitulo="Panel de administración" />
      <main className="container">
        <AdminNav activo="evaluaciones" sesion={sesion} />
        <h1>Editar evaluación</h1>
        <EditorEvaluacion
          inicial={evaluacion}
          areas={areas}
          areaEditable={sesion.rol === "admin"}
        />
      </main>
    </>
  );
}
