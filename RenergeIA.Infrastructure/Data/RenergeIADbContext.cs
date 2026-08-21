using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RenergeIA.Core.Entities;
using RenergeIA.Core.Entities.HSEQ;
using RenergeIA.Infrastructure.Identity;

namespace RenergeIA.Infrastructure.Data;

public class RenergeIADbContext : IdentityDbContext<ApplicationUser>
{
    public RenergeIADbContext(DbContextOptions<RenergeIADbContext> options) : base(options) { }

    public DbSet<Proyecto> Proyectos => Set<Proyecto>();
    public DbSet<CronogramaVersion> CronogramasVersion => Set<CronogramaVersion>();
    public DbSet<ActividadWBS> ActividadesWBS => Set<ActividadWBS>();
    public DbSet<InformeDiario> InformesDiarios => Set<InformeDiario>();
    public DbSet<RegistroAvanceDiario> RegistrosAvanceDiario => Set<RegistroAvanceDiario>();
    public DbSet<Fotografia> Fotografias => Set<Fotografia>();
    public DbSet<Documento> Documentos => Set<Documento>();
    public DbSet<VersionDocumento> VersionesDocumento => Set<VersionDocumento>();
    public DbSet<Partida> Partidas => Set<Partida>();
    public DbSet<CostoReal> CostosReales => Set<CostoReal>();
    public DbSet<CompromisoCosto> CompromisoCostos => Set<CompromisoCosto>();
    public DbSet<NoConformidad> NoConformidades => Set<NoConformidad>();
    public DbSet<AccionCorrectiva> AccionesCorrectivas => Set<AccionCorrectiva>();
    public DbSet<Restriccion> Restricciones => Set<Restriccion>();
    public DbSet<Alerta> Alertas => Set<Alerta>();
    public DbSet<RegistroClima> RegistrosClima => Set<RegistroClima>();
    public DbSet<RegistroAvanceRestriccion> RegistrosAvanceRestriccion => Set<RegistroAvanceRestriccion>();
    public DbSet<PlantillaHistograma> PlantillasHistograma => Set<PlantillaHistograma>();
    public DbSet<ItemHistograma> ItemsHistograma => Set<ItemHistograma>();
    public DbSet<HistogramaReal> HistogramasReales => Set<HistogramaReal>();
    public DbSet<ItemHistogramaReal> ItemsHistogramaReal => Set<ItemHistogramaReal>();
    public DbSet<Proveedor> Proveedores => Set<Proveedor>();
    public DbSet<PersonaExterna> PersonasExternas => Set<PersonaExterna>();
    public DbSet<RecursoEquipo> RecursosEquipo => Set<RecursoEquipo>();
    public DbSet<TipoDocumentoControl> TiposDocumentoControl => Set<TipoDocumentoControl>();
    public DbSet<DocumentoControl> DocumentosControl => Set<DocumentoControl>();
    public DbSet<EtapaRevision> EtapasRevision => Set<EtapaRevision>();

    // HSEQ
    public DbSet<ChecklistAuditoria> ChecklistsAuditoria => Set<ChecklistAuditoria>();
    public DbSet<ItemChecklist> ItemsChecklist => Set<ItemChecklist>();
    public DbSet<PPI> PPIs => Set<PPI>();
    public DbSet<EquipoCalibracion> EquiposCalibracion => Set<EquipoCalibracion>();
    public DbSet<DocumentoHSEQ> DocumentosHSEQ => Set<DocumentoHSEQ>();
    public DbSet<HallazgoHSEQ> HallazgosHSEQ => Set<HallazgoHSEQ>();
    public DbSet<AccionHSEQ> AccionesHSEQ => Set<AccionHSEQ>();
    public DbSet<InspeccionSST> InspeccionesSST => Set<InspeccionSST>();
    public DbSet<PermisoTrabajo> PermisosTrabajo => Set<PermisoTrabajo>();
    public DbSet<AnalisisTrabajoSeguro> AnalisisTrabajoSeguro => Set<AnalisisTrabajoSeguro>();
    public DbSet<IncidenteAccidente> IncidentesAccidentes => Set<IncidenteAccidente>();
    public DbSet<EntregaEPP> EntregasEPP => Set<EntregaEPP>();
    public DbSet<Capacitacion> Capacitaciones => Set<Capacitacion>();
    public DbSet<PlanTrabajoHSE> PlanesTrabajoHSE => Set<PlanTrabajoHSE>();
    public DbSet<PausaActiva> PausasActivas => Set<PausaActiva>();
    public DbSet<RegistroSTC> RegistrosSTC => Set<RegistroSTC>();
    public DbSet<RegistroOTS> RegistrosOTS => Set<RegistroOTS>();
    public DbSet<CapacitacionPlanificada> CapacitacionesPlanificadas => Set<CapacitacionPlanificada>();
    public DbSet<InspeccionAmbiental> InspeccionesAmbientales => Set<InspeccionAmbiental>();
    public DbSet<GestionResiduos> GestionResiduos => Set<GestionResiduos>();
    public DbSet<AspectoImpacto> AspectosImpactos => Set<AspectoImpacto>();
    public DbSet<Derrame> Derrames => Set<Derrame>();
    public DbSet<RegistroFaunaFlora> RegistrosFaunaFlora => Set<RegistroFaunaFlora>();
    public DbSet<Comunidad> Comunidades => Set<Comunidad>();
    public DbSet<ReunionComunitaria> ReunionesComunitarias => Set<ReunionComunitaria>();
    public DbSet<PQRHseq> PQRsHseq => Set<PQRHseq>();
    public DbSet<CompromisoSocial> CompromissosSociales => Set<CompromisoSocial>();
    public DbSet<ContratacionLocal> ContratacionesLocales => Set<ContratacionLocal>();
    public DbSet<CompraLocal> ComprasLocales => Set<CompraLocal>();
    public DbSet<ActaEvidencia> ActasEvidencias => Set<ActaEvidencia>();

    // Matriz de Riesgos IPERV
    public DbSet<InspeccionIA> InspeccionesIA => Set<InspeccionIA>();
    public DbSet<RiesgoIPERV> RiesgosIPERV => Set<RiesgoIPERV>();
    public DbSet<BibliotecaPeligro> BibliotecaPeligros => Set<BibliotecaPeligro>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Proyecto
        modelBuilder.Entity<Proyecto>(e =>
        {
            e.ToTable("Proyectos");
            e.Property(p => p.CapacidadKWp).HasColumnType("decimal(10,2)");
            e.Property(p => p.PresupuestoContractual).HasColumnType("decimal(18,2)");
            e.Property(p => p.Latitud).HasColumnType("decimal(10,7)");
            e.Property(p => p.Longitud).HasColumnType("decimal(10,7)");
        });

        // CronogramaVersion
        modelBuilder.Entity<CronogramaVersion>(e =>
        {
            e.ToTable("CronogramasVersion");
            e.HasOne(v => v.Proyecto)
             .WithMany()
             .HasForeignKey(v => v.ProyectoId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ActividadWBS — auto-referencia con Restrict para evitar ciclos
        modelBuilder.Entity<ActividadWBS>(e =>
        {
            e.ToTable("ActividadesWBS");
            e.Property(p => p.AvancePlanificado).HasColumnType("decimal(5,2)");
            e.Property(p => p.AvanceReal).HasColumnType("decimal(5,2)");
            e.HasOne(a => a.ActividadPadre)
             .WithMany(a => a.SubActividades)
             .HasForeignKey(a => a.ActividadPadreId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(a => a.CronogramaVersion)
             .WithMany(v => v.Actividades)
             .HasForeignKey(a => a.CronogramaVersionId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // RegistroAvanceDiario — múltiples FK a Proyectos, usar Restrict en 2 de 3
        modelBuilder.Entity<RegistroAvanceDiario>(e =>
        {
            e.ToTable("RegistrosAvanceDiario");
            e.Property(p => p.PorcentajeAvance).HasColumnType("decimal(5,2)");
            e.Property(p => p.HorasTrabajadas).HasColumnType("decimal(6,2)");
            e.HasOne(r => r.ActividadWBS)
             .WithMany(a => a.RegistrosAvance)
             .HasForeignKey(r => r.ActividadWBSId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.InformeDiario)
             .WithMany(i => i.RegistrosAvance)
             .HasForeignKey(r => r.InformeDiarioId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Proyecto — TRM COP/USD con valor por defecto
        modelBuilder.Entity<Proyecto>(e =>
        {
            e.Property(p => p.TasaCambioCOPUSD).HasColumnType("decimal(18,2)").HasDefaultValue(4000m);
        });

        // Partida — jerarquía WBS + propiedad calculada ignorada
        modelBuilder.Entity<Partida>(e =>
        {
            e.ToTable("Partidas");
            e.Property(p => p.CantidadPresupuestada).HasColumnType("decimal(18,4)");
            e.Property(p => p.PrecioUnitario).HasColumnType("decimal(18,2)");
            e.Ignore(p => p.MontoPresupuestado);
            e.Property(p => p.ValorEjecutado).HasColumnType("decimal(18,2)");
            e.Property(p => p.MontoComprometido).HasColumnType("decimal(18,2)").HasDefaultValue(0m);
            e.HasOne(p => p.Padre)
             .WithMany(p => p.SubPartidas)
             .HasForeignKey(p => p.PadreId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // CostoReal — propiedad calculada ignorada
        modelBuilder.Entity<CostoReal>(e =>
        {
            e.ToTable("CostosReales");
            e.Property(p => p.Cantidad).HasColumnType("decimal(18,4)");
            e.Property(p => p.PrecioUnitario).HasColumnType("decimal(18,2)");
            e.Property(p => p.AdjuntoUrl).HasMaxLength(500);
            e.Ignore(p => p.Monto);
        });

        // CompromisoCosto
        modelBuilder.Entity<CompromisoCosto>(e =>
        {
            e.ToTable("CompromisoCostos");
            e.Property(c => c.Valor).HasColumnType("decimal(18,2)");
            e.Property(c => c.Estado).HasConversion<int>();
            e.Property(c => c.Codigo).HasMaxLength(50);
            e.Property(c => c.Proveedor).HasMaxLength(200);
            e.Property(c => c.Prioridad).HasMaxLength(20);
            e.HasOne(c => c.Proyecto).WithMany()
             .HasForeignKey(c => c.ProyectoId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(c => c.Partida).WithMany()
             .HasForeignKey(c => c.PartidaId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        });

        // RegistroClima
        modelBuilder.Entity<RegistroClima>(e =>
        {
            e.ToTable("RegistrosClima");
            e.Property(p => p.TemperaturaMaxima).HasColumnType("decimal(5,2)");
            e.Property(p => p.TemperaturaMinima).HasColumnType("decimal(5,2)");
            e.Property(p => p.HumedadRelativa).HasColumnType("decimal(5,2)");
            e.Property(p => p.VelocidadViento).HasColumnType("decimal(6,2)");
            e.Property(p => p.PrecipitacionMm).HasColumnType("decimal(8,2)");
            e.Property(p => p.HorasDisponiblesTrabajar).HasColumnType("decimal(4,2)");
            e.HasOne(c => c.InformeDiario)
             .WithMany(i => i.RegistrosClima)
             .HasForeignKey(c => c.InformeDiarioId)
             .OnDelete(DeleteBehavior.NoAction);
        });

        // InformeDiario — auto-referencia + unique constraint ProyectoId+Fecha
        modelBuilder.Entity<InformeDiario>(e =>
        {
            e.ToTable("InformesDiarios");
            e.HasIndex(i => new { i.ProyectoId, i.Fecha }).IsUnique();
            e.HasOne(i => i.InformeDiarioAnterior)
             .WithMany()
             .HasForeignKey(i => i.InformeDiarioAnteriorId)
             .OnDelete(DeleteBehavior.NoAction);
        });

        // RegistroAvanceDiario — columnas decimales nuevas
        modelBuilder.Entity<RegistroAvanceDiario>(e =>
        {
            e.Property(r => r.CantidadEjecutadaDia).HasColumnType("decimal(18,4)");
            e.Property(r => r.AvanceEsperado).HasColumnType("decimal(5,2)");
            e.Property(r => r.AvanceAcumulado).HasColumnType("decimal(5,2)");
            e.Property(r => r.Desviacion).HasColumnType("decimal(5,2)");
            e.Property(r => r.HorasAfectadasClima).HasColumnType("decimal(6,2)");
        });

        // ActividadWBS — columnas decimales nuevas
        modelBuilder.Entity<ActividadWBS>(e =>
        {
            e.Property(a => a.CantidadTotal).HasColumnType("decimal(18,4)");
            e.Property(a => a.CantidadEjecutadaAcumulada).HasColumnType("decimal(18,4)");
        });

        // M:N — claves compuestas
        modelBuilder.Entity<RegistroAvanceRestriccion>(e =>
        {
            e.ToTable("RegistrosAvanceRestriccion");
            e.HasKey(r => new { r.RegistroAvanceDiarioId, r.RestriccionId });
            e.HasOne(r => r.RegistroAvanceDiario)
             .WithMany(rd => rd.RestriccionesRelacionadas)
             .HasForeignKey(r => r.RegistroAvanceDiarioId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(r => r.Restriccion)
             .WithMany()
             .HasForeignKey(r => r.RestriccionId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // Nombres de tablas para las demás entidades
        modelBuilder.Entity<Fotografia>().ToTable("Fotografias");
        modelBuilder.Entity<Documento>(e =>
        {
            e.ToTable("Documentos");
            e.Property(d => d.Disciplina).HasConversion<int>();
        });
        modelBuilder.Entity<VersionDocumento>().ToTable("VersionesDocumento");
        modelBuilder.Entity<NoConformidad>().ToTable("NoConformidades");
        modelBuilder.Entity<AccionCorrectiva>().ToTable("AccionesCorrectivas");
        modelBuilder.Entity<Restriccion>().ToTable("Restricciones");
        modelBuilder.Entity<Alerta>().ToTable("Alertas");

        // Control de Ingreso — Proveedores, Recursos, Personas y Documentos con vencimiento
        modelBuilder.Entity<Proveedor>(e =>
        {
            e.ToTable("Proveedores");
            e.HasOne(p => p.Proyecto)
             .WithMany(pr => pr.Proveedores)
             .HasForeignKey(p => p.ProyectoId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PersonaExterna>(e =>
        {
            e.ToTable("PersonasExternas");
            e.HasOne(p => p.Proyecto)
             .WithMany(pr => pr.PersonasExternas)
             .HasForeignKey(p => p.ProyectoId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(p => p.Proveedor)
             .WithMany(pv => pv.Personas)
             .HasForeignKey(p => p.ProveedorId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RecursoEquipo>(e =>
        {
            e.ToTable("RecursosEquipo");
            e.HasOne(r => r.Proyecto)
             .WithMany(pr => pr.RecursosEquipo)
             .HasForeignKey(r => r.ProyectoId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(r => r.Proveedor)
             .WithMany(pv => pv.Recursos)
             .HasForeignKey(r => r.ProveedorId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.ConductorOperador)
             .WithMany()
             .HasForeignKey(r => r.ConductorOperadorId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TipoDocumentoControl>().ToTable("TiposDocumentoControl");

        modelBuilder.Entity<DocumentoControl>(e =>
        {
            e.ToTable("DocumentosControl");
            e.HasOne(d => d.Proyecto)
             .WithMany(pr => pr.DocumentosControl)
             .HasForeignKey(d => d.ProyectoId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(d => d.TipoDocumentoControl)
             .WithMany()
             .HasForeignKey(d => d.TipoDocumentoControlId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.PersonaExterna)
             .WithMany(p => p.Documentos)
             .HasForeignKey(d => d.PersonaExternaId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.RecursoEquipo)
             .WithMany(r => r.Documentos)
             .HasForeignKey(d => d.RecursoEquipoId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(d => d.Proveedor)
             .WithMany(pv => pv.Documentos)
             .HasForeignKey(d => d.ProveedorId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<EtapaRevision>(e =>
        {
            e.ToTable("EtapasRevision");
            e.HasIndex(x => new { x.RecursoEquipoId, x.Etapa }).IsUnique();
            e.HasOne(x => x.RecursoEquipo)
             .WithMany(r => r.Etapas)
             .HasForeignKey(x => x.RecursoEquipoId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // Histogramas
        modelBuilder.Entity<PlantillaHistograma>(e =>
        {
            e.ToTable("PlantillasHistograma");
            e.HasMany(p => p.Items)
             .WithOne(i => i.PlantillaHistograma)
             .HasForeignKey(i => i.PlantillaHistogramaId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ItemHistograma>(e =>
        {
            e.ToTable("ItemsHistograma");
            e.Property(i => i.Mes1).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes2).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes3).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes4).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes5).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes6).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes7).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes8).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes9).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes10).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes11).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes12).HasColumnType("decimal(10,2)");
        });

        // HistogramaReal
        modelBuilder.Entity<HistogramaReal>(e =>
        {
            e.ToTable("HistogramasReales");
            e.HasMany(h => h.Items)
             .WithOne(i => i.HistogramaReal)
             .HasForeignKey(i => i.HistogramaRealId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ItemHistogramaReal>(e =>
        {
            e.ToTable("ItemsHistogramaReal");
            e.Property(i => i.Mes1).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes2).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes3).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes4).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes5).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes6).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes7).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes8).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes9).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes10).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes11).HasColumnType("decimal(10,2)");
            e.Property(i => i.Mes12).HasColumnType("decimal(10,2)");
        });

        // HSEQ
        modelBuilder.Entity<ChecklistAuditoria>(e =>
        {
            e.ToTable("ChecklistsAuditoria");
            e.Property(c => c.PorcentajeCumplimiento).HasColumnType("decimal(5,2)");
            e.Property(c => c.EstadoAuditoria).HasConversion<int>();
            e.Property(c => c.UsuarioId).HasMaxLength(450);
            e.Property(c => c.NormaISO).HasMaxLength(100);
            e.Property(c => c.ProcesoArea).HasMaxLength(200);
            e.Property(c => c.TipoNorma).HasConversion<int>();
            e.Property(c => c.TipoAuditoria).HasConversion<int>();
            e.HasOne(c => c.Proyecto).WithMany()
             .HasForeignKey(c => c.ProyectoId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
            e.HasMany(c => c.Items).WithOne(i => i.ChecklistAuditoria)
             .HasForeignKey(i => i.ChecklistAuditoriaId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<ItemChecklist>(e =>
        {
            e.ToTable("ItemsChecklist");
            e.Property(i => i.Puntaje).HasColumnType("decimal(5,2)");
            e.Property(i => i.Clausula).HasMaxLength(20);
            e.Property(i => i.TituloClausula).HasMaxLength(200);
            e.Property(i => i.EvidenciaUrl).HasMaxLength(500);
            e.Property(i => i.Responsable).HasMaxLength(200);
            e.Property(i => i.Seguimiento).HasConversion<int>();
            e.Property(i => i.Estado).HasConversion<int>();
        });
        modelBuilder.Entity<PPI>().ToTable("PPIs");
        modelBuilder.Entity<EquipoCalibracion>(e =>
        {
            e.ToTable("EquiposCalibracion");
            e.Ignore(eq => eq.Estado);
        });
        modelBuilder.Entity<DocumentoHSEQ>().ToTable("DocumentosHSEQ");
        modelBuilder.Entity<HallazgoHSEQ>(e =>
        {
            e.ToTable("HallazgosHSEQ");
            e.HasMany(h => h.Acciones).WithOne(a => a.HallazgoHSEQ)
             .HasForeignKey(a => a.HallazgoHSEQId).OnDelete(DeleteBehavior.Cascade);
        });
        modelBuilder.Entity<AccionHSEQ>().ToTable("AccionesHSEQ");
        modelBuilder.Entity<InspeccionSST>().ToTable("InspeccionesSST");
        modelBuilder.Entity<PermisoTrabajo>().ToTable("PermisosTrabajo");
        modelBuilder.Entity<AnalisisTrabajoSeguro>().ToTable("AnalisisTrabajoSeguro");
        modelBuilder.Entity<IncidenteAccidente>().ToTable("IncidentesAccidentes");
        modelBuilder.Entity<EntregaEPP>().ToTable("EntregasEPP");
        modelBuilder.Entity<Capacitacion>(e =>
        {
            e.ToTable("Capacitaciones");
            e.Property(c => c.DuracionHoras).HasColumnType("decimal(6,2)");
        });
        modelBuilder.Entity<PlanTrabajoHSE>().ToTable("PlanesTrabajoHSE");
        modelBuilder.Entity<PausaActiva>().ToTable("PausasActivas");
        modelBuilder.Entity<RegistroSTC>().ToTable("RegistrosSTC");
        modelBuilder.Entity<RegistroOTS>().ToTable("RegistrosOTS");
        modelBuilder.Entity<CapacitacionPlanificada>(e =>
        {
            e.ToTable("CapacitacionesPlanificadas");
            e.Property(c => c.DuracionEstimadaHoras).HasColumnType("decimal(6,2)");
        });
        modelBuilder.Entity<InspeccionAmbiental>().ToTable("InspeccionesAmbientales");
        modelBuilder.Entity<GestionResiduos>(e =>
        {
            e.ToTable("GestionResiduos");
            e.Property(g => g.CantidadKg).HasColumnType("decimal(10,2)");
        });
        modelBuilder.Entity<AspectoImpacto>().ToTable("AspectosImpactos");
        modelBuilder.Entity<Derrame>(e =>
        {
            e.ToTable("Derrames");
            e.Property(d => d.VolumenLitros).HasColumnType("decimal(10,2)");
        });
        modelBuilder.Entity<RegistroFaunaFlora>().ToTable("RegistrosFaunaFlora");
        modelBuilder.Entity<Comunidad>(e =>
        {
            e.ToTable("Comunidades");
            e.HasMany(c => c.Reuniones).WithOne(r => r.Comunidad)
             .HasForeignKey(r => r.ComunidadId).OnDelete(DeleteBehavior.Restrict);
            e.HasMany(c => c.PQRs).WithOne(p => p.Comunidad)
             .HasForeignKey(p => p.ComunidadId).OnDelete(DeleteBehavior.ClientSetNull);
        });
        modelBuilder.Entity<ReunionComunitaria>().ToTable("ReunionesComunitarias");
        modelBuilder.Entity<PQRHseq>().ToTable("PQRsHseq");
        modelBuilder.Entity<CompromisoSocial>(e =>
        {
            e.ToTable("CompromissosSociales");
            e.HasOne(c => c.Comunidad).WithMany()
             .HasForeignKey(c => c.ComunidadId).OnDelete(DeleteBehavior.ClientSetNull);
        });
        modelBuilder.Entity<ContratacionLocal>().ToTable("ContratacionesLocales");
        modelBuilder.Entity<CompraLocal>(e =>
        {
            e.ToTable("ComprasLocales");
            e.Property(c => c.ValorCOP).HasColumnType("decimal(18,2)");
        });
        modelBuilder.Entity<ActaEvidencia>().ToTable("ActasEvidencias");
    }
}
