using RenergeIA.Core.Enums;

namespace RenergeIA.Core.Helpers;

public static class EnumDisplay
{
    public static string Mostrar(EstadoProyecto estado) => estado switch
    {
        EstadoProyecto.Planificacion => "Planificación",
        EstadoProyecto.EnEjecucion   => "En Ejecución",
        EstadoProyecto.Suspendido    => "Suspendido",
        EstadoProyecto.Completado    => "Completado",
        EstadoProyecto.Cancelado     => "Cancelado",
        EstadoProyecto.OperacionYMantenimiento => "Operación y Mantenimiento",
        _                            => estado.ToString()
    };

    public static string Mostrar(EstadoActividad estado) => estado switch
    {
        EstadoActividad.Pendiente   => "Pendiente",
        EstadoActividad.EnProgreso  => "En Progreso",
        EstadoActividad.Completada  => "Completada",
        EstadoActividad.Retrasada   => "Retrasada",
        EstadoActividad.Bloqueada   => "Bloqueada",
        _                           => estado.ToString()
    };

    public static string Mostrar(TipoRecurso tipo) => tipo switch
    {
        TipoRecurso.Vehiculo   => "Vehículo",
        TipoRecurso.Maquinaria => "Maquinaria",
        TipoRecurso.Equipo     => "Equipo",
        TipoRecurso.Herramienta => "Herramienta",
        _                       => tipo.ToString()
    };

    public static string Mostrar(EtapaProceso etapa) => etapa switch
    {
        EtapaProceso.Compras  => "Compras",
        EtapaProceso.RRHH     => "RRHH (Conductor/Operador)",
        EtapaProceso.HSE      => "HSE (Máquina/Vehículo)",
        EtapaProceso.Cliente  => "Aprobación Cliente",
        EtapaProceso.Proyecto => "Aprobación Proyecto",
        _                     => etapa.ToString()
    };

    public static string Mostrar(EstadoEtapa estado) => estado switch
    {
        EstadoEtapa.Pendiente      => "Pendiente",
        EstadoEtapa.EnRevision     => "En Revisión",
        EstadoEtapa.ConComentarios => "Con Comentarios",
        EstadoEtapa.Aprobado       => "Aprobado",
        EstadoEtapa.NoAplica       => "N/A",
        _                          => estado.ToString()
    };

    public static string Mostrar(EstadoDocumento estado) => estado switch
    {
        EstadoDocumento.PendienteEmitir       => "Pendiente Emitir",
        EstadoDocumento.PendienteValidacion   => "Pendiente Validación",
        EstadoDocumento.ValidadoConComentarios => "Validado con Comentarios",
        EstadoDocumento.Validado              => "Validado",
        EstadoDocumento.Informativos          => "Informativos",
        EstadoDocumento.NoValidado            => "No Validado",
        _                                     => estado.ToString()
    };

    public static string Mostrar(CategoriaDocumento cat) => cat switch
    {
        CategoriaDocumento.Procedimientos => "Procedimientos",
        CategoriaDocumento.HSE            => "HSE",
        CategoriaDocumento.Ingenieria     => "Ingeniería",
        _                                 => cat.ToString()
    };

    public static string Mostrar(AreaDocumento area) => area switch
    {
        AreaDocumento.Civil     => "Civil",
        AreaDocumento.Mecanico  => "Mecánico",
        AreaDocumento.Electrico => "Eléctrico",
        AreaDocumento.General   => "General",
        _                       => area.ToString()
    };

    public static string Mostrar(EstadoInforme estado) => estado switch
    {
        EstadoInforme.Borrador   => "Borrador",
        EstadoInforme.EnRevision => "En Revisión",
        EstadoInforme.Aprobado   => "Aprobado",
        EstadoInforme.Rechazado  => "Rechazado",
        _                        => estado.ToString()
    };

    public static string Mostrar(EstadoAvance estado) => estado switch
    {
        EstadoAvance.Adelantado => "Adelantado",
        EstadoAvance.EnTiempo   => "En Tiempo",
        EstadoAvance.Atrasado   => "Atrasado",
        EstadoAvance.Critico    => "Crítico",
        _                       => estado.ToString()
    };

    public static string Mostrar(Disciplina disciplina) => disciplina switch
    {
        Disciplina.Mecanica         => "Mecánica",
        Disciplina.Civil            => "Civil",
        Disciplina.Electrica        => "Eléctrica",
        Disciplina.Contractual      => "Contractual",
        Disciplina.HotCommissioning => "Hot Commissioning",
        Disciplina.CierreProyecto   => "Dossier",
        Disciplina.General          => "General",
        _                           => disciplina.ToString()
    };

    public static string IconoDisciplina(Disciplina disciplina) => disciplina switch
    {
        Disciplina.Mecanica         => "⚙️",
        Disciplina.Civil            => "🏗️",
        Disciplina.Electrica        => "⚡",
        Disciplina.Contractual      => "📋",
        Disciplina.HotCommissioning => "🔥",
        Disciplina.CierreProyecto   => "✅",
        Disciplina.General          => "📌",
        _                           => "📌"
    };

    public static string Mostrar(EstadoRestriccion estado) => estado switch
    {
        EstadoRestriccion.Abierta   => "Abierta",
        EstadoRestriccion.EnGestion => "En Gestión",
        EstadoRestriccion.Levantada => "Levantada",
        EstadoRestriccion.Cancelada => "Cancelada",
        _                           => estado.ToString()
    };

    public static string Mostrar(SeveridadNoConformidad severidad) => severidad switch
    {
        SeveridadNoConformidad.Baja    => "Baja",
        SeveridadNoConformidad.Media   => "Media",
        SeveridadNoConformidad.Alta    => "Alta",
        SeveridadNoConformidad.Critica => "Crítica",
        _                              => severidad.ToString()
    };

    public static string Mostrar(EstadoNoConformidad estado) => estado switch
    {
        EstadoNoConformidad.Abierta          => "Abierta",
        EstadoNoConformidad.EnRevision       => "En Revisión",
        EstadoNoConformidad.EnImplementacion => "En Implementación",
        EstadoNoConformidad.Cerrada          => "Cerrada",
        EstadoNoConformidad.Cancelada        => "Cancelada",
        _                                    => estado.ToString()
    };

    public static string Mostrar(CondicionClimatica condicion) => condicion switch
    {
        CondicionClimatica.Soleado             => "Soleado",
        CondicionClimatica.ParcialmenteNublado => "Parcialmente Nublado",
        CondicionClimatica.Nublado             => "Nublado",
        CondicionClimatica.Lluvia              => "Lluvia",
        CondicionClimatica.LluviaFuerte        => "Lluvia Fuerte",
        CondicionClimatica.Tormenta            => "Tormenta",
        CondicionClimatica.Viento              => "Viento",
        CondicionClimatica.Niebla              => "Niebla",
        _                                      => condicion.ToString()
    };

    // ── HSEQ ─────────────────────────────────────────────────────────────────

    public static string Mostrar(EstadoHSEQ estado) => estado switch
    {
        EstadoHSEQ.Saludable => "Saludable",
        EstadoHSEQ.EnRiesgo  => "En Riesgo",
        EstadoHSEQ.Critico   => "Crítico",
        EstadoHSEQ.SinDatos  => "Sin Datos",
        _                    => estado.ToString()
    };

    public static string Mostrar(DivisionHSEQ division) => division switch
    {
        DivisionHSEQ.Calidad    => "Calidad",
        DivisionHSEQ.Seguridad  => "Seguridad",
        DivisionHSEQ.Ambiental  => "Ambiental",
        DivisionHSEQ.Social     => "Social",
        _                       => division.ToString()
    };

    public static string Mostrar(EstadoPPI estado) => estado switch
    {
        EstadoPPI.Pendiente          => "Pendiente",
        EstadoPPI.EnProceso          => "En Proceso",
        EstadoPPI.Aprobado           => "Aprobado",
        EstadoPPI.Rechazado          => "Rechazado",
        EstadoPPI.RequiereCorreccion => "Requiere Corrección",
        _                            => estado.ToString()
    };

    public static string Mostrar(EstadoCalibracion estado) => estado switch
    {
        EstadoCalibracion.Vigente         => "Vigente",
        EstadoCalibracion.ProximoAVencer  => "Próximo a Vencer",
        EstadoCalibracion.Vencido         => "Vencido",
        EstadoCalibracion.SinCertificado  => "Sin Certificado",
        _                                 => estado.ToString()
    };

    public static string Mostrar(EstadoDocumentoHSEQ estado) => estado switch
    {
        EstadoDocumentoHSEQ.Vigente    => "Vigente",
        EstadoDocumentoHSEQ.EnRevision => "En Revisión",
        EstadoDocumentoHSEQ.Pendiente  => "Pendiente",
        EstadoDocumentoHSEQ.Obsoleto   => "Obsoleto",
        EstadoDocumentoHSEQ.Vencido    => "Vencido",
        _                              => estado.ToString()
    };

    public static string Mostrar(TipoDocumentoHSEQ tipo) => tipo switch
    {
        TipoDocumentoHSEQ.Procedimiento => "Procedimiento",
        TipoDocumentoHSEQ.Instructivo   => "Instructivo",
        TipoDocumentoHSEQ.Formato       => "Formato",
        TipoDocumentoHSEQ.Plan          => "Plan",
        TipoDocumentoHSEQ.ITP           => "ITP",
        TipoDocumentoHSEQ.Matriz        => "Matriz",
        TipoDocumentoHSEQ.Dossier       => "Dossier",
        TipoDocumentoHSEQ.Manual        => "Manual",
        TipoDocumentoHSEQ.Registro      => "Registro",
        _                               => tipo.ToString()
    };

    public static string Mostrar(EstadoCumplimiento estado) => estado switch
    {
        EstadoCumplimiento.Cumple   => "Cumple",
        EstadoCumplimiento.NoCumple => "No Cumple",
        EstadoCumplimiento.Parcial  => "Parcial",
        EstadoCumplimiento.NoAplica => "No Aplica",
        _                           => estado.ToString()
    };

    public static string Mostrar(EstadoInspeccionHSEQ estado) => estado switch
    {
        EstadoInspeccionHSEQ.Programada => "Programada",
        EstadoInspeccionHSEQ.Realizada  => "Realizada",
        EstadoInspeccionHSEQ.Cancelada  => "Cancelada",
        EstadoInspeccionHSEQ.Vencida    => "Vencida",
        _                               => estado.ToString()
    };

    public static string Mostrar(EstadoPermisoTrabajo estado) => estado switch
    {
        EstadoPermisoTrabajo.Activo    => "Activo",
        EstadoPermisoTrabajo.Vencido   => "Vencido",
        EstadoPermisoTrabajo.Cerrado   => "Cerrado",
        EstadoPermisoTrabajo.Cancelado => "Cancelado",
        _                              => estado.ToString()
    };

    public static string Mostrar(TipoPermisoTrabajo tipo) => tipo switch
    {
        TipoPermisoTrabajo.TrabajoEnAltura    => "Trabajo en Altura",
        TipoPermisoTrabajo.EspacioConfinado   => "Espacio Confinado",
        TipoPermisoTrabajo.TrabajoEnCaliente  => "Trabajo en Caliente",
        TipoPermisoTrabajo.EnergiasPeligrosas => "Energías Peligrosas",
        TipoPermisoTrabajo.Electrico          => "Eléctrico",
        TipoPermisoTrabajo.Excavacion         => "Excavación",
        TipoPermisoTrabajo.General            => "General",
        _                                     => tipo.ToString()
    };

    public static string Mostrar(TipoIncidente tipo) => tipo switch
    {
        TipoIncidente.CasiAccidente    => "Casi Accidente",
        TipoIncidente.Incidente        => "Incidente",
        TipoIncidente.AccidenteLeve    => "Accidente Leve",
        TipoIncidente.AccidenteGrave   => "Accidente Grave",
        TipoIncidente.AccidenteFatal   => "Accidente Fatal",
        _                              => tipo.ToString()
    };

    public static string Mostrar(GravedadIncidente gravedad) => gravedad switch
    {
        GravedadIncidente.Leve     => "Leve",
        GravedadIncidente.Moderado => "Moderado",
        GravedadIncidente.Grave    => "Grave",
        GravedadIncidente.Fatal    => "Fatal",
        _                          => gravedad.ToString()
    };

    public static string Mostrar(EstadoPQR estado) => estado switch
    {
        EstadoPQR.Abierta   => "Abierta",
        EstadoPQR.EnGestion => "En Gestión",
        EstadoPQR.Cerrada   => "Cerrada",
        EstadoPQR.Vencida   => "Vencida",
        _                   => estado.ToString()
    };

    public static string Mostrar(TipoPQR tipo) => tipo switch
    {
        TipoPQR.Peticion   => "Petición",
        TipoPQR.Queja      => "Queja",
        TipoPQR.Reclamo    => "Reclamo",
        TipoPQR.Denuncia   => "Denuncia",
        TipoPQR.Solicitud  => "Solicitud",
        _                  => tipo.ToString()
    };

    public static string Mostrar(EstadoCompromisoSocial estado) => estado switch
    {
        EstadoCompromisoSocial.Pendiente      => "Pendiente",
        EstadoCompromisoSocial.EnCumplimiento => "En Cumplimiento",
        EstadoCompromisoSocial.Cumplido       => "Cumplido",
        EstadoCompromisoSocial.Vencido        => "Vencido",
        EstadoCompromisoSocial.Cancelado      => "Cancelado",
        _                                     => estado.ToString()
    };

    public static string Mostrar(TipoResiduos tipo) => tipo switch
    {
        TipoResiduos.Aprovechable    => "Aprovechable",
        TipoResiduos.Peligroso       => "Peligroso",
        TipoResiduos.Organico        => "Orgánico",
        TipoResiduos.Escombros       => "Escombros",
        TipoResiduos.NoAprovechable  => "No Aprovechable",
        TipoResiduos.Otro            => "Otro",
        _                            => tipo.ToString()
    };

    public static string Mostrar(EstadoPlanTrabajo estado) => estado switch
    {
        EstadoPlanTrabajo.Planificada  => "Planificada",
        EstadoPlanTrabajo.Ejecutada    => "Ejecutada",
        EstadoPlanTrabajo.Pendiente    => "Pendiente",
        EstadoPlanTrabajo.Vencida      => "Vencida",
        EstadoPlanTrabajo.Reprogramada => "Reprogramada",
        EstadoPlanTrabajo.Cancelada    => "Cancelada",
        _                              => estado.ToString()
    };
}
