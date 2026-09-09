import { redirect } from "next/navigation";
import Topbar from "@/components/Topbar";
import AdminNav from "@/components/AdminNav";
import GestionPersonal from "@/components/GestionPersonal";
import { getStore } from "@/lib/db";
import { sesionActual, puedeVerArea } from "@/lib/auth";

export const dynamic = "force-dynamic";

export default async function PersonalPage() {
  const sesion = await sesionActual();
  if (!sesion) redirect("/admin/login");

  const store = await getStore();
  const [personal, todasEvals, todasAsist, todosIntentos] = await Promise.all([
    store.listPersonal(),
    store.listEvaluaciones(),
    store.listAsistencias(),
    store.listIntentos(),
  ]);

  const esAdmin = sesion.rol === "admin";
  const evaluaciones = esAdmin
    ? todasEvals
    : todasEvals.filter((e) => puedeVerArea(sesion, e.area));
  const asistencias = esAdmin
    ? todasAsist
    : todasAsist.filter((a) => puedeVerArea(sesion, a.area));
  const intentos = esAdmin
    ? todosIntentos
    : todosIntentos.filter((i) => puedeVerArea(sesion, i.area));

  // Una persona cuenta como asistente a una evaluación si tiene asistencia registrada
  // O si presentó la evaluación (intento). Unificamos ambas fuentes por cédula + evaluación.
  const asistenciasUnificadas = [
    ...asistencias.map((a) => ({ cedula: a.cedula, evaluacionId: a.evaluacionId })),
    ...intentos.map((i) => ({ cedula: i.participante.cedula, evaluacionId: i.evaluacionId })),
  ];

  return (
    <>
      <Topbar subtitulo="Panel de administración" sesion={sesion} />
      <main className="container-wide">
        <AdminNav activo="personal" sesion={sesion} />
        <h1>Personal</h1>
        <p className="muted">
          Gestiona la planta de personal y consulta la matriz cruzada de asistencia a capacitaciones.
        </p>
        <GestionPersonal
          personalInicial={personal}
          evaluaciones={evaluaciones.map((e) => ({ id: e.id, titulo: e.titulo }))}
          asistencias={asistenciasUnificadas}
        />
      </main>
    </>
  );
}
