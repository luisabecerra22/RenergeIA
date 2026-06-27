using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeguridadV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccionesGeneradas",
                table: "InspeccionesSST",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActosInseguros",
                table: "InspeccionesSST",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "InspeccionesSST",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CondicionesInseguras",
                table: "InspeccionesSST",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCompromiso",
                table: "InspeccionesSST",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Responsable",
                table: "InspeccionesSST",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ResponsableCierre",
                table: "InspeccionesSST",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "EntregasEPP",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Documento",
                table: "EntregasEPP",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Talla",
                table: "EntregasEPP",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoEntrega",
                table: "EntregasEPP",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "Capacitaciones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "Capacitaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Responsable",
                table: "Capacitaciones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CapacitacionesPlanificadas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tema = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublicoObjetivo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaPlanificada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DuracionEstimadaHoras = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    CapacitacionEjecutadaId = table.Column<int>(type: "int", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "PausasActivas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: false),
                    Trabajador = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Documento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cargo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    CantidadPausas = table.Column<int>(type: "int", nullable: false),
                    ResponsableRegistro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                name: "PlanesTrabajoHSE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: false),
                    Actividad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoIntervencion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Responsable = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Recursos = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EjecutadoPor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaPlanificada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaEjecutada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_CapacitacionesPlanificadas_ProyectoId",
                table: "CapacitacionesPlanificadas",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_PausasActivas_ProyectoId",
                table: "PausasActivas",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesTrabajoHSE_ProyectoId",
                table: "PlanesTrabajoHSE",
                column: "ProyectoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CapacitacionesPlanificadas");

            migrationBuilder.DropTable(
                name: "PausasActivas");

            migrationBuilder.DropTable(
                name: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "AccionesGeneradas",
                table: "InspeccionesSST");

            migrationBuilder.DropColumn(
                name: "ActosInseguros",
                table: "InspeccionesSST");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "InspeccionesSST");

            migrationBuilder.DropColumn(
                name: "CondicionesInseguras",
                table: "InspeccionesSST");

            migrationBuilder.DropColumn(
                name: "FechaCompromiso",
                table: "InspeccionesSST");

            migrationBuilder.DropColumn(
                name: "Responsable",
                table: "InspeccionesSST");

            migrationBuilder.DropColumn(
                name: "ResponsableCierre",
                table: "InspeccionesSST");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "EntregasEPP");

            migrationBuilder.DropColumn(
                name: "Documento",
                table: "EntregasEPP");

            migrationBuilder.DropColumn(
                name: "Talla",
                table: "EntregasEPP");

            migrationBuilder.DropColumn(
                name: "TipoEntrega",
                table: "EntregasEPP");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "Capacitaciones");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Capacitaciones");

            migrationBuilder.DropColumn(
                name: "Responsable",
                table: "Capacitaciones");
        }
    }
}
