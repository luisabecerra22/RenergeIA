using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    NombreCompleto = table.Column<string>(type: "text", nullable: false),
                    Cargo = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BibliotecaPeligros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Area = table.Column<string>(type: "text", nullable: false),
                    Actividad = table.Column<string>(type: "text", nullable: false),
                    Tarea = table.Column<string>(type: "text", nullable: false),
                    Peligro = table.Column<string>(type: "text", nullable: false),
                    Clasificacion = table.Column<string>(type: "text", nullable: false),
                    EfectosPosibles = table.Column<string>(type: "text", nullable: true),
                    ControlFuente = table.Column<string>(type: "text", nullable: true),
                    ControlMedio = table.Column<string>(type: "text", nullable: true),
                    ControlIndividuo = table.Column<string>(type: "text", nullable: true),
                    MedidasIntervencion = table.Column<string>(type: "text", nullable: true),
                    EPPRecomendado = table.Column<string>(type: "text", nullable: true),
                    DocumentosAsociados = table.Column<string>(type: "text", nullable: true),
                    PermisosRequeridos = table.Column<string>(type: "text", nullable: true),
                    NivelRiesgoSugerido = table.Column<string>(type: "text", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BibliotecaPeligros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Proyectos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Cliente = table.Column<string>(type: "text", nullable: false),
                    Ubicacion = table.Column<string>(type: "text", nullable: false),
                    Pais = table.Column<string>(type: "text", nullable: false),
                    Departamento = table.Column<string>(type: "text", nullable: true),
                    Municipio = table.Column<string>(type: "text", nullable: true),
                    Latitud = table.Column<decimal>(type: "numeric(10,7)", nullable: true),
                    Longitud = table.Column<decimal>(type: "numeric(10,7)", nullable: true),
                    AccuWeatherLocationKey = table.Column<string>(type: "text", nullable: true),
                    CapacidadKWp = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    PresupuestoContractual = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaInicioPlaneada = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFinPlaneada = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaInicioReal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaFinReal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    MesInicialHistograma = table.Column<int>(type: "integer", nullable: false),
                    AnioInicialHistograma = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proyectos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActasEvidencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ComunidadOParte = table.Column<string>(type: "text", nullable: true),
                    ResponsableElaboracion = table.Column<string>(type: "text", nullable: true),
                    Firmantes = table.Column<string>(type: "text", nullable: true),
                    ArchivoUrl = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActasEvidencias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActasEvidencias_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alertas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Mensaje = table.Column<string>(type: "text", nullable: false),
                    Severidad = table.Column<string>(type: "text", nullable: false),
                    EsLeida = table.Column<bool>(type: "boolean", nullable: false),
                    DestinatarioId = table.Column<string>(type: "text", nullable: true),
                    Referencia = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alertas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Alertas_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnalisisTrabajoSeguro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Actividad = table.Column<string>(type: "text", nullable: false),
                    FrenteTrabajo = table.Column<string>(type: "text", nullable: false),
                    Responsable = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    NumeroTrabajadores = table.Column<int>(type: "integer", nullable: false),
                    RiesgosIdentificados = table.Column<string>(type: "text", nullable: true),
                    MedidasControl = table.Column<string>(type: "text", nullable: true),
                    EPPRequerido = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Firmas = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalisisTrabajoSeguro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnalisisTrabajoSeguro_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspectosImpactos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Actividad = table.Column<string>(type: "text", nullable: false),
                    Aspecto = table.Column<string>(type: "text", nullable: false),
                    Impacto = table.Column<string>(type: "text", nullable: false),
                    MedioAfectado = table.Column<string>(type: "text", nullable: false),
                    Magnitud = table.Column<int>(type: "integer", nullable: false),
                    Frecuencia = table.Column<int>(type: "integer", nullable: false),
                    Duracion = table.Column<int>(type: "integer", nullable: false),
                    NivelSignificancia = table.Column<int>(type: "integer", nullable: false),
                    EsSignificativo = table.Column<bool>(type: "boolean", nullable: false),
                    MedidasControl = table.Column<string>(type: "text", nullable: true),
                    Responsable = table.Column<string>(type: "text", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspectosImpactos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspectosImpactos_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Capacitaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Tema = table.Column<string>(type: "text", nullable: false),
                    Instructor = table.Column<string>(type: "text", nullable: false),
                    Responsable = table.Column<string>(type: "text", nullable: true),
                    Area = table.Column<string>(type: "text", nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DuracionHoras = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    NumeroAsistentes = table.Column<int>(type: "integer", nullable: false),
                    Participantes = table.Column<string>(type: "text", nullable: true),
                    Lugar = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Evidencias = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Capacitaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Capacitaciones_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CapacitacionesPlanificadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Tema = table.Column<string>(type: "text", nullable: false),
                    Area = table.Column<string>(type: "text", nullable: true),
                    PublicoObjetivo = table.Column<string>(type: "text", nullable: true),
                    FechaPlanificada = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DuracionEstimadaHoras = table.Column<decimal>(type: "numeric(6,2)", nullable: false),
                    Responsable = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    CapacitacionEjecutadaId = table.Column<int>(type: "integer", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapacitacionesPlanificadas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CapacitacionesPlanificadas_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChecklistsAuditoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: true),
                    Division = table.Column<int>(type: "integer", nullable: true),
                    NormaISO = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TipoNorma = table.Column<int>(type: "integer", nullable: true),
                    TipoAuditoria = table.Column<int>(type: "integer", nullable: false),
                    ProcesoArea = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Auditor = table.Column<string>(type: "text", nullable: false),
                    FechaAuditoria = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PorcentajeCumplimiento = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    EstadoAuditoria = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChecklistsAuditoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChecklistsAuditoria_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ComprasLocales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Proveedor = table.Column<string>(type: "text", nullable: false),
                    Municipio = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    ValorCOP = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    NumeroFactura = table.Column<string>(type: "text", nullable: true),
                    EsLocal = table.Column<bool>(type: "boolean", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComprasLocales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComprasLocales_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comunidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Municipio = table.Column<string>(type: "text", nullable: false),
                    Departamento = table.Column<string>(type: "text", nullable: false),
                    Resguardo = table.Column<string>(type: "text", nullable: true),
                    Corregimiento = table.Column<string>(type: "text", nullable: true),
                    LiderComunal = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    NumeroPobladores = table.Column<int>(type: "integer", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comunidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comunidades_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContratacionesLocales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    NombrePersona = table.Column<string>(type: "text", nullable: false),
                    Documento = table.Column<string>(type: "text", nullable: false),
                    Municipio = table.Column<string>(type: "text", nullable: false),
                    Cargo = table.Column<string>(type: "text", nullable: false),
                    Empresa = table.Column<string>(type: "text", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaRetiro = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EsLocal = table.Column<bool>(type: "boolean", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratacionesLocales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContratacionesLocales_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CronogramasVersion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    NumeroVersion = table.Column<int>(type: "integer", nullable: false),
                    EsVigente = table.Column<bool>(type: "boolean", nullable: false),
                    MotivoReprogramacion = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CronogramasVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CronogramasVersion_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Derrames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Sustancia = table.Column<string>(type: "text", nullable: false),
                    VolumenLitros = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    FrenteTrabajo = table.Column<string>(type: "text", nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ReportadoPor = table.Column<string>(type: "text", nullable: false),
                    CausaRaiz = table.Column<string>(type: "text", nullable: true),
                    AccionesInmediatas = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Evidencias = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Derrames", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Derrames_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    TipoDocumento = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Disciplina = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentos_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosHSEQ",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaAprobacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Responsable = table.Column<string>(type: "text", nullable: false),
                    AprobadoPor = table.Column<string>(type: "text", nullable: true),
                    UbicacionSharePoint = table.Column<string>(type: "text", nullable: true),
                    Disciplina = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosHSEQ", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentosHSEQ_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntregasEPP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Trabajador = table.Column<string>(type: "text", nullable: false),
                    Documento = table.Column<string>(type: "text", nullable: false),
                    Cargo = table.Column<string>(type: "text", nullable: false),
                    Area = table.Column<string>(type: "text", nullable: false),
                    TipoEntrega = table.Column<string>(type: "text", nullable: false),
                    ItemEPP = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Talla = table.Column<string>(type: "text", nullable: true),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    FechaEntrega = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EntregadoPor = table.Column<string>(type: "text", nullable: false),
                    FirmaConformidad = table.Column<bool>(type: "boolean", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntregasEPP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntregasEPP_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    TipoEquipo = table.Column<int>(type: "integer", nullable: false),
                    Marca = table.Column<string>(type: "text", nullable: true),
                    Modelo = table.Column<string>(type: "text", nullable: true),
                    NumeroSerie = table.Column<string>(type: "text", nullable: true),
                    Propietario = table.Column<string>(type: "text", nullable: true),
                    FechaIngreso = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaSalida = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipos_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquiposCalibracion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Serial = table.Column<string>(type: "text", nullable: false),
                    Marca = table.Column<string>(type: "text", nullable: false),
                    Modelo = table.Column<string>(type: "text", nullable: false),
                    NumeroCertificado = table.Column<string>(type: "text", nullable: true),
                    FechaCalibracion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Responsable = table.Column<string>(type: "text", nullable: false),
                    Ubicacion = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquiposCalibracion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EquiposCalibracion_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GestionResiduos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    CantidadKg = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    GestorAutorizado = table.Column<string>(type: "text", nullable: true),
                    ManifiestoNumero = table.Column<string>(type: "text", nullable: true),
                    FrenteTrabajo = table.Column<string>(type: "text", nullable: false),
                    Responsable = table.Column<string>(type: "text", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Evidencias = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GestionResiduos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GestionResiduos_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HallazgosHSEQ",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Division = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Causa = table.Column<string>(type: "text", nullable: true),
                    Clasificacion = table.Column<string>(type: "text", nullable: true),
                    Responsable = table.Column<string>(type: "text", nullable: false),
                    FechaDeteccion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaCompromiso = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Severidad = table.Column<int>(type: "integer", nullable: false),
                    Evidencias = table.Column<string>(type: "text", nullable: true),
                    Ubicacion = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HallazgosHSEQ", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HallazgosHSEQ_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistogramasReales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistogramasReales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistogramasReales_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncidentesAccidentes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Gravedad = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    FrenteTrabajo = table.Column<string>(type: "text", nullable: false),
                    PersonaInvolucrada = table.Column<string>(type: "text", nullable: false),
                    InvestigadoPor = table.Column<string>(type: "text", nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaInvestigacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaCierre = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CausaRaiz = table.Column<string>(type: "text", nullable: true),
                    AccionesInmediatas = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Evidencias = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncidentesAccidentes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IncidentesAccidentes_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InformesDiarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    NumeroCertificado = table.Column<string>(type: "text", nullable: true),
                    ResumenActividades = table.Column<string>(type: "text", nullable: true),
                    PersonalTotal = table.Column<int>(type: "integer", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    CreadoPor = table.Column<string>(type: "text", nullable: false),
                    Enviado = table.Column<bool>(type: "boolean", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    RevisadoPor = table.Column<string>(type: "text", nullable: true),
                    FechaRevision = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MotivoRechazo = table.Column<string>(type: "text", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    ComentariosGenerales = table.Column<string>(type: "text", nullable: true),
                    InformeDiarioAnteriorId = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformesDiarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InformesDiarios_InformesDiarios_InformeDiarioAnteriorId",
                        column: x => x.InformeDiarioAnteriorId,
                        principalTable: "InformesDiarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InformesDiarios_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspeccionesAmbientales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    FrenteTrabajo = table.Column<string>(type: "text", nullable: false),
                    Inspector = table.Column<string>(type: "text", nullable: false),
                    FechaInspeccion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    HallazgosEncontrados = table.Column<int>(type: "integer", nullable: false),
                    HallazgosCerrados = table.Column<int>(type: "integer", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Evidencias = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspeccionesAmbientales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspeccionesAmbientales_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspeccionesIA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Area = table.Column<string>(type: "text", nullable: false),
                    Actividad = table.Column<string>(type: "text", nullable: false),
                    Tarea = table.Column<string>(type: "text", nullable: false),
                    Responsable = table.Column<string>(type: "text", nullable: false),
                    Ubicacion = table.Column<string>(type: "text", nullable: true),
                    Inspector = table.Column<string>(type: "text", nullable: false),
                    FechaInspeccion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ObservacionManual = table.Column<string>(type: "text", nullable: true),
                    EvidenciaUrl = table.Column<string>(type: "text", nullable: true),
                    EstadoValidacion = table.Column<int>(type: "integer", nullable: false),
                    ValidadoPor = table.Column<string>(type: "text", nullable: true),
                    FechaValidacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ObservacionValidacion = table.Column<string>(type: "text", nullable: true),
                    ResultadoIA = table.Column<string>(type: "text", nullable: true),
                    PeligrosIdentificados = table.Column<string>(type: "text", nullable: true),
                    NivelRiesgoSugerido = table.Column<string>(type: "text", nullable: true),
                    HallazgoRedactado = table.Column<string>(type: "text", nullable: true),
                    AccionCorrectivaSugerida = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspeccionesIA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspeccionesIA_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InspeccionesSST",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Area = table.Column<string>(type: "text", nullable: false),
                    Frente = table.Column<string>(type: "text", nullable: false),
                    Responsable = table.Column<string>(type: "text", nullable: false),
                    Inspector = table.Column<string>(type: "text", nullable: false),
                    FechaInspeccion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    HallazgosEncontrados = table.Column<int>(type: "integer", nullable: false),
                    HallazgosCerrados = table.Column<int>(type: "integer", nullable: false),
                    ActosInseguros = table.Column<int>(type: "integer", nullable: false),
                    CondicionesInseguras = table.Column<int>(type: "integer", nullable: false),
                    AccionesGeneradas = table.Column<string>(type: "text", nullable: true),
                    ResponsableCierre = table.Column<string>(type: "text", nullable: true),
                    FechaCompromiso = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Evidencias = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InspeccionesSST", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InspeccionesSST_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoConformidades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Categoria = table.Column<string>(type: "text", nullable: true),
                    Severidad = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaDeteccion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DetectadoPor = table.Column<string>(type: "text", nullable: false),
                    Ubicacion = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoConformidades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoConformidades_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PausasActivas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Trabajador = table.Column<string>(type: "text", nullable: false),
                    Documento = table.Column<string>(type: "text", nullable: false),
                    Area = table.Column<string>(type: "text", nullable: false),
                    Cargo = table.Column<string>(type: "text", nullable: false),
                    Mes = table.Column<int>(type: "integer", nullable: false),
                    Anio = table.Column<int>(type: "integer", nullable: false),
                    CantidadPausas = table.Column<int>(type: "integer", nullable: false),
                    ResponsableRegistro = table.Column<string>(type: "text", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PausasActivas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PausasActivas_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermisosTrabajo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Actividad = table.Column<string>(type: "text", nullable: false),
                    FrenteTrabajo = table.Column<string>(type: "text", nullable: false),
                    ResponsableTrabajo = table.Column<string>(type: "text", nullable: false),
                    EmitidoPor = table.Column<string>(type: "text", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    MedidasControl = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermisosTrabajo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PermisosTrabajo_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalProyecto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Apellido = table.Column<string>(type: "text", nullable: false),
                    DocumentoIdentidad = table.Column<string>(type: "text", nullable: false),
                    Cargo = table.Column<string>(type: "text", nullable: false),
                    Empresa = table.Column<string>(type: "text", nullable: false),
                    TipoPersonal = table.Column<int>(type: "integer", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaSalida = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalProyecto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalProyecto_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanesTrabajoHSE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Actividad = table.Column<string>(type: "text", nullable: false),
                    TipoIntervencion = table.Column<string>(type: "text", nullable: false),
                    Responsable = table.Column<string>(type: "text", nullable: true),
                    Recursos = table.Column<string>(type: "text", nullable: true),
                    EjecutadoPor = table.Column<string>(type: "text", nullable: true),
                    FechaPlanificada = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaEjecutada = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Mes = table.Column<int>(type: "integer", nullable: false),
                    Anio = table.Column<int>(type: "integer", nullable: false),
                    Area = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanesTrabajoHSE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanesTrabajoHSE_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlantillasHistograma",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillasHistograma", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantillasHistograma_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PPIs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    ActividadWBS = table.Column<string>(type: "text", nullable: true),
                    FrenteTrabajo = table.Column<string>(type: "text", nullable: true),
                    Disciplina = table.Column<string>(type: "text", nullable: false),
                    Responsable = table.Column<string>(type: "text", nullable: false),
                    FechaPlaneada = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaRealizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Evidencias = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PPIs_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosFaunaFlora",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    Especie = table.Column<string>(type: "text", nullable: false),
                    NombreCientifico = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    Ubicacion = table.Column<string>(type: "text", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    RegistradoPor = table.Column<string>(type: "text", nullable: false),
                    AccionesManejo = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Evidencias = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosFaunaFlora", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosFaunaFlora_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Restricciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FechaIdentificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaCompromiso = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaLevantamiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Responsable = table.Column<string>(type: "text", nullable: true),
                    Impacto = table.Column<string>(type: "text", nullable: true),
                    Plan = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Restricciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Restricciones_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsChecklist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ChecklistAuditoriaId = table.Column<int>(type: "integer", nullable: false),
                    Clausula = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TituloClausula = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NumeroRequisito = table.Column<string>(type: "text", nullable: false),
                    DescripcionRequisito = table.Column<string>(type: "text", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Puntaje = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    EvidenciaUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Hallazgo = table.Column<string>(type: "text", nullable: true),
                    OportunidadMejora = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Responsable = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Plazo = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Seguimiento = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsChecklist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsChecklist_ChecklistsAuditoria_ChecklistAuditoriaId",
                        column: x => x.ChecklistAuditoriaId,
                        principalTable: "ChecklistsAuditoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompromissosSociales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    ComunidadId = table.Column<int>(type: "integer", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Beneficiario = table.Column<string>(type: "text", nullable: false),
                    Responsable = table.Column<string>(type: "text", nullable: false),
                    FechaCompromiso = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaCumplimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    FuenteCompromiso = table.Column<string>(type: "text", nullable: true),
                    Evidencias = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompromissosSociales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompromissosSociales_Comunidades_ComunidadId",
                        column: x => x.ComunidadId,
                        principalTable: "Comunidades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompromissosSociales_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PQRsHseq",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    ComunidadId = table.Column<int>(type: "integer", nullable: true),
                    Numero = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Solicitante = table.Column<string>(type: "text", nullable: false),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    FechaRecepcion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaLimiteRespuesta = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Responsable = table.Column<string>(type: "text", nullable: true),
                    Respuesta = table.Column<string>(type: "text", nullable: true),
                    Evidencias = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PQRsHseq", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PQRsHseq_Comunidades_ComunidadId",
                        column: x => x.ComunidadId,
                        principalTable: "Comunidades",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PQRsHseq_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReunionesComunitarias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    ComunidadId = table.Column<int>(type: "integer", nullable: false),
                    Titulo = table.Column<string>(type: "text", nullable: false),
                    Objetivo = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Lugar = table.Column<string>(type: "text", nullable: false),
                    Facilitador = table.Column<string>(type: "text", nullable: false),
                    NumeroAsistentes = table.Column<int>(type: "integer", nullable: false),
                    TemasTratatados = table.Column<string>(type: "text", nullable: true),
                    Acuerdos = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Evidencias = table.Column<string>(type: "text", nullable: true),
                    ActaUrl = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReunionesComunitarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReunionesComunitarias_Comunidades_ComunidadId",
                        column: x => x.ComunidadId,
                        principalTable: "Comunidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReunionesComunitarias_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ActividadesWBS",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Disciplina = table.Column<int>(type: "integer", nullable: true),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    ActividadPadreId = table.Column<int>(type: "integer", nullable: true),
                    CodigoWBS = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    NivelWBS = table.Column<int>(type: "integer", nullable: false),
                    Responsable = table.Column<string>(type: "text", nullable: true),
                    FechaInicioPlaneada = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFinPlaneada = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaInicioReal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaFinReal = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AvancePlanificado = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    AvanceReal = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    CronogramaVersionId = table.Column<int>(type: "integer", nullable: true),
                    CantidadTotal = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Unidad = table.Column<string>(type: "text", nullable: true),
                    CantidadEjecutadaAcumulada = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    EsCritica = table.Column<bool>(type: "boolean", nullable: false),
                    FrenteTrabajo = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActividadesWBS", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActividadesWBS_ActividadesWBS_ActividadPadreId",
                        column: x => x.ActividadPadreId,
                        principalTable: "ActividadesWBS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActividadesWBS_CronogramasVersion_CronogramaVersionId",
                        column: x => x.CronogramaVersionId,
                        principalTable: "CronogramasVersion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActividadesWBS_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VersionesDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentoId = table.Column<int>(type: "integer", nullable: false),
                    NumeroVersion = table.Column<string>(type: "text", nullable: false),
                    RutaArchivo = table.Column<string>(type: "text", nullable: false),
                    NombreArchivo = table.Column<string>(type: "text", nullable: false),
                    FechaSubida = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SubidoPor = table.Column<string>(type: "text", nullable: false),
                    TamanioBytes = table.Column<long>(type: "bigint", nullable: false),
                    Comentarios = table.Column<string>(type: "text", nullable: true),
                    EsVersionActual = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VersionesDocumento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VersionesDocumento_Documentos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "Documentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mantenimientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipoId = table.Column<int>(type: "integer", nullable: false),
                    TipoMantenimiento = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Costo = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    RealizadoPor = table.Column<string>(type: "text", nullable: true),
                    ProximoMantenimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantenimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mantenimientos_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosHorometro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipoId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    LecturaHorometro = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    HorasTrabajadas = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    Operador = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosHorometro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosHorometro_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccionesHSEQ",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HallazgoHSEQId = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Responsable = table.Column<string>(type: "text", nullable: false),
                    FechaCompromiso = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaCierre = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    AvancePorcentaje = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Evidencia = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccionesHSEQ", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccionesHSEQ_HallazgosHSEQ_HallazgoHSEQId",
                        column: x => x.HallazgoHSEQId,
                        principalTable: "HallazgosHSEQ",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsHistogramaReal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HistogramaRealId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Mes1 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes2 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes3 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes4 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes5 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes6 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes7 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes8 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes9 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes10 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes11 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes12 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsHistogramaReal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsHistogramaReal_HistogramasReales_HistogramaRealId",
                        column: x => x.HistogramaRealId,
                        principalTable: "HistogramasReales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fotografias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InformeDiarioId = table.Column<int>(type: "integer", nullable: false),
                    NombreArchivo = table.Column<string>(type: "text", nullable: false),
                    RutaArchivo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: true),
                    FechaToma = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Latitud = table.Column<double>(type: "double precision", nullable: true),
                    Longitud = table.Column<double>(type: "double precision", nullable: true),
                    Etiquetas = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fotografias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fotografias_InformesDiarios_InformeDiarioId",
                        column: x => x.InformeDiarioId,
                        principalTable: "InformesDiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosClima",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Condicion = table.Column<int>(type: "integer", nullable: false),
                    TemperaturaMaxima = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    TemperaturaMinima = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    HumedadRelativa = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    VelocidadViento = table.Column<decimal>(type: "numeric(6,2)", nullable: true),
                    PrecipitacionMm = table.Column<decimal>(type: "numeric(8,2)", nullable: true),
                    HorasDisponiblesTrabajar = table.Column<decimal>(type: "numeric(4,2)", nullable: true),
                    AfectoActividades = table.Column<bool>(type: "boolean", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    InformeDiarioId = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosClima", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosClima_InformesDiarios_InformeDiarioId",
                        column: x => x.InformeDiarioId,
                        principalTable: "InformesDiarios",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegistrosClima_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RiesgosIPERV",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    FuenteOrigen = table.Column<int>(type: "integer", nullable: false),
                    InspeccionIAId = table.Column<int>(type: "integer", nullable: true),
                    Area = table.Column<string>(type: "text", nullable: false),
                    Actividad = table.Column<string>(type: "text", nullable: false),
                    Tarea = table.Column<string>(type: "text", nullable: false),
                    EsRutinaria = table.Column<bool>(type: "boolean", nullable: false),
                    DescripcionPeligro = table.Column<string>(type: "text", nullable: false),
                    ClasificacionPeligro = table.Column<string>(type: "text", nullable: false),
                    EfectosPosibles = table.Column<string>(type: "text", nullable: true),
                    ControlFuente = table.Column<string>(type: "text", nullable: true),
                    ControlMedio = table.Column<string>(type: "text", nullable: true),
                    ControlIndividuo = table.Column<string>(type: "text", nullable: true),
                    ND = table.Column<int>(type: "integer", nullable: false),
                    NE = table.Column<int>(type: "integer", nullable: false),
                    NP = table.Column<int>(type: "integer", nullable: false),
                    NC = table.Column<int>(type: "integer", nullable: false),
                    NR = table.Column<int>(type: "integer", nullable: false),
                    Aceptabilidad = table.Column<string>(type: "text", nullable: false),
                    Eliminacion = table.Column<string>(type: "text", nullable: true),
                    Sustitucion = table.Column<string>(type: "text", nullable: true),
                    ControlIngenieria = table.Column<string>(type: "text", nullable: true),
                    ControlAdministrativo = table.Column<string>(type: "text", nullable: true),
                    Senalizacion = table.Column<string>(type: "text", nullable: true),
                    EPP = table.Column<string>(type: "text", nullable: true),
                    Responsable = table.Column<string>(type: "text", nullable: false),
                    Plazo = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EvidenciaUrl = table.Column<string>(type: "text", nullable: true),
                    Hallazgo = table.Column<string>(type: "text", nullable: true),
                    AccionCorrectiva = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    EstadoValidacion = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiesgosIPERV", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RiesgosIPERV_InspeccionesIA_InspeccionIAId",
                        column: x => x.InspeccionIAId,
                        principalTable: "InspeccionesIA",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RiesgosIPERV_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccionesCorrectivas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NoConformidadId = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Responsable = table.Column<string>(type: "text", nullable: false),
                    FechaCompromiso = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaImplementacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<string>(type: "text", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccionesCorrectivas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccionesCorrectivas_NoConformidades_NoConformidadId",
                        column: x => x.NoConformidadId,
                        principalTable: "NoConformidades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosPersona",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonalProyectoId = table.Column<int>(type: "integer", nullable: false),
                    TipoDocumento = table.Column<string>(type: "text", nullable: false),
                    NombreDocumento = table.Column<string>(type: "text", nullable: false),
                    RutaArchivo = table.Column<string>(type: "text", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosPersona", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentosPersona_PersonalProyecto_PersonalProyectoId",
                        column: x => x.PersonalProyectoId,
                        principalTable: "PersonalProyecto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsHistograma",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlantillaHistogramaId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Mes1 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes2 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes3 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes4 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes5 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes6 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes7 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes8 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes9 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes10 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes11 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Mes12 = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsHistograma", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsHistograma_PlantillasHistograma_PlantillaHistogramaId",
                        column: x => x.PlantillaHistogramaId,
                        principalTable: "PlantillasHistograma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Partidas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    ActividadWBSId = table.Column<int>(type: "integer", nullable: true),
                    Numero = table.Column<string>(type: "text", nullable: true),
                    Nivel = table.Column<int>(type: "integer", nullable: false),
                    EsPrincipal = table.Column<bool>(type: "boolean", nullable: false),
                    PadreId = table.Column<int>(type: "integer", nullable: true),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Unidad = table.Column<string>(type: "text", nullable: false),
                    Categoria = table.Column<string>(type: "text", nullable: true),
                    CantidadPresupuestada = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ValorEjecutado = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Partidas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Partidas_ActividadesWBS_ActividadWBSId",
                        column: x => x.ActividadWBSId,
                        principalTable: "ActividadesWBS",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Partidas_Partidas_PadreId",
                        column: x => x.PadreId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Partidas_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosAvanceDiario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    ActividadWBSId = table.Column<int>(type: "integer", nullable: false),
                    InformeDiarioId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PorcentajeAvance = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    HorasTrabajadas = table.Column<decimal>(type: "numeric(6,2)", nullable: true),
                    PersonalEnSitio = table.Column<int>(type: "integer", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    ReportadoPor = table.Column<string>(type: "text", nullable: false),
                    CantidadEjecutadaDia = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    AvanceEsperado = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    AvanceAcumulado = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    Desviacion = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    DiasAtraso = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    HorasAfectadasClima = table.Column<decimal>(type: "numeric(6,2)", nullable: true),
                    Novedades = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosAvanceDiario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosAvanceDiario_ActividadesWBS_ActividadWBSId",
                        column: x => x.ActividadWBSId,
                        principalTable: "ActividadesWBS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosAvanceDiario_InformesDiarios_InformeDiarioId",
                        column: x => x.InformeDiarioId,
                        principalTable: "InformesDiarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosAvanceDiario_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompromisoCostos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    PartidaId = table.Column<int>(type: "integer", nullable: true),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Proveedor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Prioridad = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompromisoCostos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompromisoCostos_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CompromisoCostos_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CostosReales",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PartidaId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    TipoCosto = table.Column<string>(type: "text", nullable: false),
                    Cantidad = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    PrecioUnitario = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    NumeroFactura = table.Column<string>(type: "text", nullable: true),
                    Proveedor = table.Column<string>(type: "text", nullable: true),
                    RegistradoPor = table.Column<string>(type: "text", nullable: false),
                    AdjuntoUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostosReales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CostosReales_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosAvanceEquipo",
                columns: table => new
                {
                    RegistroAvanceDiarioId = table.Column<int>(type: "integer", nullable: false),
                    EquipoId = table.Column<int>(type: "integer", nullable: false),
                    HorasUtilizadas = table.Column<decimal>(type: "numeric(6,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosAvanceEquipo", x => new { x.RegistroAvanceDiarioId, x.EquipoId });
                    table.ForeignKey(
                        name: "FK_RegistrosAvanceEquipo_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosAvanceEquipo_RegistrosAvanceDiario_RegistroAvanceD~",
                        column: x => x.RegistroAvanceDiarioId,
                        principalTable: "RegistrosAvanceDiario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosAvancePersonal",
                columns: table => new
                {
                    RegistroAvanceDiarioId = table.Column<int>(type: "integer", nullable: false),
                    PersonalProyectoId = table.Column<int>(type: "integer", nullable: false),
                    HorasTrabajadas = table.Column<decimal>(type: "numeric(6,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosAvancePersonal", x => new { x.RegistroAvanceDiarioId, x.PersonalProyectoId });
                    table.ForeignKey(
                        name: "FK_RegistrosAvancePersonal_PersonalProyecto_PersonalProyectoId",
                        column: x => x.PersonalProyectoId,
                        principalTable: "PersonalProyecto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosAvancePersonal_RegistrosAvanceDiario_RegistroAvanc~",
                        column: x => x.RegistroAvanceDiarioId,
                        principalTable: "RegistrosAvanceDiario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosAvanceRestriccion",
                columns: table => new
                {
                    RegistroAvanceDiarioId = table.Column<int>(type: "integer", nullable: false),
                    RestriccionId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosAvanceRestriccion", x => new { x.RegistroAvanceDiarioId, x.RestriccionId });
                    table.ForeignKey(
                        name: "FK_RegistrosAvanceRestriccion_RegistrosAvanceDiario_RegistroAv~",
                        column: x => x.RegistroAvanceDiarioId,
                        principalTable: "RegistrosAvanceDiario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrosAvanceRestriccion_Restricciones_RestriccionId",
                        column: x => x.RestriccionId,
                        principalTable: "Restricciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccionesCorrectivas_NoConformidadId",
                table: "AccionesCorrectivas",
                column: "NoConformidadId");

            migrationBuilder.CreateIndex(
                name: "IX_AccionesHSEQ_HallazgoHSEQId",
                table: "AccionesHSEQ",
                column: "HallazgoHSEQId");

            migrationBuilder.CreateIndex(
                name: "IX_ActasEvidencias_ProyectoId",
                table: "ActasEvidencias",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesWBS_ActividadPadreId",
                table: "ActividadesWBS",
                column: "ActividadPadreId");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesWBS_CronogramaVersionId",
                table: "ActividadesWBS",
                column: "CronogramaVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesWBS_ProyectoId",
                table: "ActividadesWBS",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Alertas_ProyectoId",
                table: "Alertas",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_AnalisisTrabajoSeguro_ProyectoId",
                table: "AnalisisTrabajoSeguro",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_AspectosImpactos_ProyectoId",
                table: "AspectosImpactos",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Capacitaciones_ProyectoId",
                table: "Capacitaciones",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_CapacitacionesPlanificadas_ProyectoId",
                table: "CapacitacionesPlanificadas",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChecklistsAuditoria_ProyectoId",
                table: "ChecklistsAuditoria",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_ComprasLocales_ProyectoId",
                table: "ComprasLocales",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisoCostos_PartidaId",
                table: "CompromisoCostos",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisoCostos_ProyectoId",
                table: "CompromisoCostos",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_CompromissosSociales_ComunidadId",
                table: "CompromissosSociales",
                column: "ComunidadId");

            migrationBuilder.CreateIndex(
                name: "IX_CompromissosSociales_ProyectoId",
                table: "CompromissosSociales",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Comunidades_ProyectoId",
                table: "Comunidades",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContratacionesLocales_ProyectoId",
                table: "ContratacionesLocales",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_CostosReales_PartidaId",
                table: "CostosReales",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_CronogramasVersion_ProyectoId",
                table: "CronogramasVersion",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Derrames_ProyectoId",
                table: "Derrames",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_ProyectoId",
                table: "Documentos",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosHSEQ_ProyectoId",
                table: "DocumentosHSEQ",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosPersona_PersonalProyectoId",
                table: "DocumentosPersona",
                column: "PersonalProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_EntregasEPP_ProyectoId",
                table: "EntregasEPP",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_ProyectoId",
                table: "Equipos",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_EquiposCalibracion_ProyectoId",
                table: "EquiposCalibracion",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Fotografias_InformeDiarioId",
                table: "Fotografias",
                column: "InformeDiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_GestionResiduos_ProyectoId",
                table: "GestionResiduos",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_HallazgosHSEQ_ProyectoId",
                table: "HallazgosHSEQ",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistogramasReales_ProyectoId",
                table: "HistogramasReales",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_IncidentesAccidentes_ProyectoId",
                table: "IncidentesAccidentes",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_InformesDiarios_InformeDiarioAnteriorId",
                table: "InformesDiarios",
                column: "InformeDiarioAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_InformesDiarios_ProyectoId_Fecha",
                table: "InformesDiarios",
                columns: new[] { "ProyectoId", "Fecha" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InspeccionesAmbientales_ProyectoId",
                table: "InspeccionesAmbientales",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_InspeccionesIA_ProyectoId",
                table: "InspeccionesIA",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_InspeccionesSST_ProyectoId",
                table: "InspeccionesSST",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsChecklist_ChecklistAuditoriaId",
                table: "ItemsChecklist",
                column: "ChecklistAuditoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsHistograma_PlantillaHistogramaId",
                table: "ItemsHistograma",
                column: "PlantillaHistogramaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsHistogramaReal_HistogramaRealId",
                table: "ItemsHistogramaReal",
                column: "HistogramaRealId");

            migrationBuilder.CreateIndex(
                name: "IX_Mantenimientos_EquipoId",
                table: "Mantenimientos",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_NoConformidades_ProyectoId",
                table: "NoConformidades",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_ActividadWBSId",
                table: "Partidas",
                column: "ActividadWBSId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_PadreId",
                table: "Partidas",
                column: "PadreId");

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_ProyectoId",
                table: "Partidas",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_PausasActivas_ProyectoId",
                table: "PausasActivas",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_PermisosTrabajo_ProyectoId",
                table: "PermisosTrabajo",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalProyecto_ProyectoId",
                table: "PersonalProyecto",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesTrabajoHSE_ProyectoId",
                table: "PlanesTrabajoHSE",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasHistograma_ProyectoId",
                table: "PlantillasHistograma",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_PPIs_ProyectoId",
                table: "PPIs",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_PQRsHseq_ComunidadId",
                table: "PQRsHseq",
                column: "ComunidadId");

            migrationBuilder.CreateIndex(
                name: "IX_PQRsHseq_ProyectoId",
                table: "PQRsHseq",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAvanceDiario_ActividadWBSId",
                table: "RegistrosAvanceDiario",
                column: "ActividadWBSId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAvanceDiario_InformeDiarioId",
                table: "RegistrosAvanceDiario",
                column: "InformeDiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAvanceDiario_ProyectoId",
                table: "RegistrosAvanceDiario",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAvanceEquipo_EquipoId",
                table: "RegistrosAvanceEquipo",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAvancePersonal_PersonalProyectoId",
                table: "RegistrosAvancePersonal",
                column: "PersonalProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAvanceRestriccion_RestriccionId",
                table: "RegistrosAvanceRestriccion",
                column: "RestriccionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosClima_InformeDiarioId",
                table: "RegistrosClima",
                column: "InformeDiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosClima_ProyectoId",
                table: "RegistrosClima",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosFaunaFlora_ProyectoId",
                table: "RegistrosFaunaFlora",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosHorometro_EquipoId",
                table: "RegistrosHorometro",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_Restricciones_ProyectoId",
                table: "Restricciones",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_ReunionesComunitarias_ComunidadId",
                table: "ReunionesComunitarias",
                column: "ComunidadId");

            migrationBuilder.CreateIndex(
                name: "IX_ReunionesComunitarias_ProyectoId",
                table: "ReunionesComunitarias",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_RiesgosIPERV_InspeccionIAId",
                table: "RiesgosIPERV",
                column: "InspeccionIAId");

            migrationBuilder.CreateIndex(
                name: "IX_RiesgosIPERV_ProyectoId",
                table: "RiesgosIPERV",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_VersionesDocumento_DocumentoId",
                table: "VersionesDocumento",
                column: "DocumentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccionesCorrectivas");

            migrationBuilder.DropTable(
                name: "AccionesHSEQ");

            migrationBuilder.DropTable(
                name: "ActasEvidencias");

            migrationBuilder.DropTable(
                name: "Alertas");

            migrationBuilder.DropTable(
                name: "AnalisisTrabajoSeguro");

            migrationBuilder.DropTable(
                name: "AspectosImpactos");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BibliotecaPeligros");

            migrationBuilder.DropTable(
                name: "Capacitaciones");

            migrationBuilder.DropTable(
                name: "CapacitacionesPlanificadas");

            migrationBuilder.DropTable(
                name: "ComprasLocales");

            migrationBuilder.DropTable(
                name: "CompromisoCostos");

            migrationBuilder.DropTable(
                name: "CompromissosSociales");

            migrationBuilder.DropTable(
                name: "ContratacionesLocales");

            migrationBuilder.DropTable(
                name: "CostosReales");

            migrationBuilder.DropTable(
                name: "Derrames");

            migrationBuilder.DropTable(
                name: "DocumentosHSEQ");

            migrationBuilder.DropTable(
                name: "DocumentosPersona");

            migrationBuilder.DropTable(
                name: "EntregasEPP");

            migrationBuilder.DropTable(
                name: "EquiposCalibracion");

            migrationBuilder.DropTable(
                name: "Fotografias");

            migrationBuilder.DropTable(
                name: "GestionResiduos");

            migrationBuilder.DropTable(
                name: "IncidentesAccidentes");

            migrationBuilder.DropTable(
                name: "InspeccionesAmbientales");

            migrationBuilder.DropTable(
                name: "InspeccionesSST");

            migrationBuilder.DropTable(
                name: "ItemsChecklist");

            migrationBuilder.DropTable(
                name: "ItemsHistograma");

            migrationBuilder.DropTable(
                name: "ItemsHistogramaReal");

            migrationBuilder.DropTable(
                name: "Mantenimientos");

            migrationBuilder.DropTable(
                name: "PausasActivas");

            migrationBuilder.DropTable(
                name: "PermisosTrabajo");

            migrationBuilder.DropTable(
                name: "PlanesTrabajoHSE");

            migrationBuilder.DropTable(
                name: "PPIs");

            migrationBuilder.DropTable(
                name: "PQRsHseq");

            migrationBuilder.DropTable(
                name: "RegistrosAvanceEquipo");

            migrationBuilder.DropTable(
                name: "RegistrosAvancePersonal");

            migrationBuilder.DropTable(
                name: "RegistrosAvanceRestriccion");

            migrationBuilder.DropTable(
                name: "RegistrosClima");

            migrationBuilder.DropTable(
                name: "RegistrosFaunaFlora");

            migrationBuilder.DropTable(
                name: "RegistrosHorometro");

            migrationBuilder.DropTable(
                name: "ReunionesComunitarias");

            migrationBuilder.DropTable(
                name: "RiesgosIPERV");

            migrationBuilder.DropTable(
                name: "VersionesDocumento");

            migrationBuilder.DropTable(
                name: "NoConformidades");

            migrationBuilder.DropTable(
                name: "HallazgosHSEQ");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Partidas");

            migrationBuilder.DropTable(
                name: "ChecklistsAuditoria");

            migrationBuilder.DropTable(
                name: "PlantillasHistograma");

            migrationBuilder.DropTable(
                name: "HistogramasReales");

            migrationBuilder.DropTable(
                name: "PersonalProyecto");

            migrationBuilder.DropTable(
                name: "RegistrosAvanceDiario");

            migrationBuilder.DropTable(
                name: "Restricciones");

            migrationBuilder.DropTable(
                name: "Equipos");

            migrationBuilder.DropTable(
                name: "Comunidades");

            migrationBuilder.DropTable(
                name: "InspeccionesIA");

            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "ActividadesWBS");

            migrationBuilder.DropTable(
                name: "InformesDiarios");

            migrationBuilder.DropTable(
                name: "CronogramasVersion");

            migrationBuilder.DropTable(
                name: "Proyectos");
        }
    }
}
