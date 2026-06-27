using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposActividadWBS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_InformesDiarios_ProyectoId",
                table: "InformesDiarios");

            migrationBuilder.AddColumn<decimal>(
                name: "AvanceAcumulado",
                table: "RegistrosAvanceDiario",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "AvanceEsperado",
                table: "RegistrosAvanceDiario",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CantidadEjecutadaDia",
                table: "RegistrosAvanceDiario",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Desviacion",
                table: "RegistrosAvanceDiario",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DiasAtraso",
                table: "RegistrosAvanceDiario",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "RegistrosAvanceDiario",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "HorasAfectadasClima",
                table: "RegistrosAvanceDiario",
                type: "decimal(6,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Novedades",
                table: "RegistrosAvanceDiario",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComentariosGenerales",
                table: "InformesDiarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Estado",
                table: "InformesDiarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRevision",
                table: "InformesDiarios",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InformeDiarioAnteriorId",
                table: "InformesDiarios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazo",
                table: "InformesDiarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevisadoPor",
                table: "InformesDiarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "InformesDiarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CantidadEjecutadaAcumulada",
                table: "ActividadesWBS",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CantidadTotal",
                table: "ActividadesWBS",
                type: "decimal(18,4)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "EsCritica",
                table: "ActividadesWBS",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FrenteTrabajo",
                table: "ActividadesWBS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unidad",
                table: "ActividadesWBS",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RegistrosAvanceEquipo",
                columns: table => new
                {
                    RegistroAvanceDiarioId = table.Column<int>(type: "int", nullable: false),
                    EquipoId = table.Column<int>(type: "int", nullable: false),
                    HorasUtilizadas = table.Column<decimal>(type: "decimal(6,2)", nullable: false)
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
                        name: "FK_RegistrosAvanceEquipo_RegistrosAvanceDiario_RegistroAvanceDiarioId",
                        column: x => x.RegistroAvanceDiarioId,
                        principalTable: "RegistrosAvanceDiario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosAvancePersonal",
                columns: table => new
                {
                    RegistroAvanceDiarioId = table.Column<int>(type: "int", nullable: false),
                    PersonalProyectoId = table.Column<int>(type: "int", nullable: false),
                    HorasTrabajadas = table.Column<decimal>(type: "decimal(6,2)", nullable: false)
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
                        name: "FK_RegistrosAvancePersonal_RegistrosAvanceDiario_RegistroAvanceDiarioId",
                        column: x => x.RegistroAvanceDiarioId,
                        principalTable: "RegistrosAvanceDiario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosAvanceRestriccion",
                columns: table => new
                {
                    RegistroAvanceDiarioId = table.Column<int>(type: "int", nullable: false),
                    RestriccionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosAvanceRestriccion", x => new { x.RegistroAvanceDiarioId, x.RestriccionId });
                    table.ForeignKey(
                        name: "FK_RegistrosAvanceRestriccion_RegistrosAvanceDiario_RegistroAvanceDiarioId",
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
                name: "IX_InformesDiarios_InformeDiarioAnteriorId",
                table: "InformesDiarios",
                column: "InformeDiarioAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_InformesDiarios_ProyectoId_Fecha",
                table: "InformesDiarios",
                columns: new[] { "ProyectoId", "Fecha" },
                unique: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_InformesDiarios_InformesDiarios_InformeDiarioAnteriorId",
                table: "InformesDiarios",
                column: "InformeDiarioAnteriorId",
                principalTable: "InformesDiarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InformesDiarios_InformesDiarios_InformeDiarioAnteriorId",
                table: "InformesDiarios");

            migrationBuilder.DropTable(
                name: "RegistrosAvanceEquipo");

            migrationBuilder.DropTable(
                name: "RegistrosAvancePersonal");

            migrationBuilder.DropTable(
                name: "RegistrosAvanceRestriccion");

            migrationBuilder.DropIndex(
                name: "IX_InformesDiarios_InformeDiarioAnteriorId",
                table: "InformesDiarios");

            migrationBuilder.DropIndex(
                name: "IX_InformesDiarios_ProyectoId_Fecha",
                table: "InformesDiarios");

            migrationBuilder.DropColumn(
                name: "AvanceAcumulado",
                table: "RegistrosAvanceDiario");

            migrationBuilder.DropColumn(
                name: "AvanceEsperado",
                table: "RegistrosAvanceDiario");

            migrationBuilder.DropColumn(
                name: "CantidadEjecutadaDia",
                table: "RegistrosAvanceDiario");

            migrationBuilder.DropColumn(
                name: "Desviacion",
                table: "RegistrosAvanceDiario");

            migrationBuilder.DropColumn(
                name: "DiasAtraso",
                table: "RegistrosAvanceDiario");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "RegistrosAvanceDiario");

            migrationBuilder.DropColumn(
                name: "HorasAfectadasClima",
                table: "RegistrosAvanceDiario");

            migrationBuilder.DropColumn(
                name: "Novedades",
                table: "RegistrosAvanceDiario");

            migrationBuilder.DropColumn(
                name: "ComentariosGenerales",
                table: "InformesDiarios");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "InformesDiarios");

            migrationBuilder.DropColumn(
                name: "FechaRevision",
                table: "InformesDiarios");

            migrationBuilder.DropColumn(
                name: "InformeDiarioAnteriorId",
                table: "InformesDiarios");

            migrationBuilder.DropColumn(
                name: "MotivoRechazo",
                table: "InformesDiarios");

            migrationBuilder.DropColumn(
                name: "RevisadoPor",
                table: "InformesDiarios");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "InformesDiarios");

            migrationBuilder.DropColumn(
                name: "CantidadEjecutadaAcumulada",
                table: "ActividadesWBS");

            migrationBuilder.DropColumn(
                name: "CantidadTotal",
                table: "ActividadesWBS");

            migrationBuilder.DropColumn(
                name: "EsCritica",
                table: "ActividadesWBS");

            migrationBuilder.DropColumn(
                name: "FrenteTrabajo",
                table: "ActividadesWBS");

            migrationBuilder.DropColumn(
                name: "Unidad",
                table: "ActividadesWBS");

            migrationBuilder.CreateIndex(
                name: "IX_InformesDiarios_ProyectoId",
                table: "InformesDiarios",
                column: "ProyectoId");
        }
    }
}
