using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPlanTrabajoHSECampos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AbrEjec",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AbrProg",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AgoEjec",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AgoProg",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DicEjec",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DicProg",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EneEjec",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EneProg",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EtapaPHVA",
                table: "PlanesTrabajoHSE",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FebEjec",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FebProg",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FrecuenciaVerificacion",
                table: "PlanesTrabajoHSE",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "JulEjec",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "JulProg",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "JunEjec",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "JunProg",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MarEjec",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MarProg",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MayEjec",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MayProg",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NovEjec",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NovProg",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OctEjec",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OctProg",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Orden",
                table: "PlanesTrabajoHSE",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SepEjec",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SepProg",
                table: "PlanesTrabajoHSE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PlanesTrabajoHSEEncabezados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Anio = table.Column<int>(type: "integer", nullable: false),
                    ResponsableNombre = table.Column<string>(type: "text", nullable: true),
                    Cargo = table.Column<string>(type: "text", nullable: true),
                    Ubicacion = table.Column<string>(type: "text", nullable: true),
                    ObjetivoGeneral = table.Column<string>(type: "text", nullable: true),
                    IndicadorCumplimiento = table.Column<string>(type: "text", nullable: true),
                    IndicadorEficacia = table.Column<string>(type: "text", nullable: true),
                    IndicadorCobertura = table.Column<string>(type: "text", nullable: true),
                    FechaElaboracion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanesTrabajoHSEEncabezados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanesTrabajoHSEEncabezados_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanesTrabajoHSEEncabezados_ProyectoId",
                table: "PlanesTrabajoHSEEncabezados",
                column: "ProyectoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanesTrabajoHSEEncabezados");

            migrationBuilder.DropColumn(
                name: "AbrEjec",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "AbrProg",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "AgoEjec",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "AgoProg",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "DicEjec",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "DicProg",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "EneEjec",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "EneProg",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "EtapaPHVA",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "FebEjec",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "FebProg",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "FrecuenciaVerificacion",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "JulEjec",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "JulProg",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "JunEjec",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "JunProg",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "MarEjec",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "MarProg",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "MayEjec",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "MayProg",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "NovEjec",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "NovProg",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "OctEjec",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "OctProg",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "Orden",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "SepEjec",
                table: "PlanesTrabajoHSE");

            migrationBuilder.DropColumn(
                name: "SepProg",
                table: "PlanesTrabajoHSE");
        }
    }
}
