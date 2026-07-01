using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeguridadIPERV : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BibliotecaPeligros",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Actividad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Peligro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Clasificacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EfectosPosibles = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlFuente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlMedio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlIndividuo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MedidasIntervencion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EPPRecomendado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DocumentosAsociados = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PermisosRequeridos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NivelRiesgoSugerido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BibliotecaPeligros", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InspeccionesIA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Actividad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ubicacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Inspector = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInspeccion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ObservacionManual = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvidenciaUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoValidacion = table.Column<int>(type: "int", nullable: false),
                    ValidadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaValidacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ObservacionValidacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResultadoIA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PeligrosIdentificados = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NivelRiesgoSugerido = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HallazgoRedactado = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccionCorrectivaSugerida = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "RiesgosIPERV",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: false),
                    FuenteOrigen = table.Column<int>(type: "int", nullable: false),
                    InspeccionIAId = table.Column<int>(type: "int", nullable: true),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Actividad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarea = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EsRutinaria = table.Column<bool>(type: "bit", nullable: false),
                    DescripcionPeligro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClasificacionPeligro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EfectosPosibles = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlFuente = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlMedio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlIndividuo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ND = table.Column<int>(type: "int", nullable: false),
                    NE = table.Column<int>(type: "int", nullable: false),
                    NP = table.Column<int>(type: "int", nullable: false),
                    NC = table.Column<int>(type: "int", nullable: false),
                    NR = table.Column<int>(type: "int", nullable: false),
                    Aceptabilidad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Eliminacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sustitucion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlIngenieria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ControlAdministrativo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Senalizacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EPP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Responsable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Plazo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EvidenciaUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hallazgo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccionCorrectiva = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    EstadoValidacion = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_InspeccionesIA_ProyectoId",
                table: "InspeccionesIA",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_RiesgosIPERV_InspeccionIAId",
                table: "RiesgosIPERV",
                column: "InspeccionIAId");

            migrationBuilder.CreateIndex(
                name: "IX_RiesgosIPERV_ProyectoId",
                table: "RiesgosIPERV",
                column: "ProyectoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BibliotecaPeligros");

            migrationBuilder.DropTable(
                name: "RiesgosIPERV");

            migrationBuilder.DropTable(
                name: "InspeccionesIA");
        }
    }
}
