using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanificacionDocumental : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArchivoPlanDocumental",
                table: "Proyectos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaActualizacionPlanDocumental",
                table: "Proyectos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AsBuiltAprobadoInterventoria",
                table: "Documentos",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AvanceAsBuilt",
                table: "Documentos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AvanceRedline",
                table: "Documentos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EditadoPor",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEdicionApp",
                table: "Documentos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaImportacion",
                table: "Documentos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionAsBuilt",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionRedline",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionRetraso",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RedlineAprobadoInterventoria",
                table: "Documentos",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RegistraCambios",
                table: "Documentos",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiereRedline",
                table: "Documentos",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsableAsBuilt",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponsableRedline",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ValidadoPor",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ResponsablesAreaDocumento",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Area = table.Column<int>(type: "integer", nullable: false),
                    Cargo = table.Column<string>(type: "text", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsablesAreaDocumento", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResponsablesAreaDocumento_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResponsablesAreaDocumento_ProyectoId_Area",
                table: "ResponsablesAreaDocumento",
                columns: new[] { "ProyectoId", "Area" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResponsablesAreaDocumento");

            migrationBuilder.DropColumn(
                name: "ArchivoPlanDocumental",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "FechaActualizacionPlanDocumental",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "AsBuiltAprobadoInterventoria",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "AvanceAsBuilt",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "AvanceRedline",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "EditadoPor",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FechaEdicionApp",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FechaUltimaImportacion",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "ObservacionAsBuilt",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "ObservacionRedline",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "ObservacionRetraso",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "RedlineAprobadoInterventoria",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "RegistraCambios",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "RequiereRedline",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "ResponsableAsBuilt",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "ResponsableRedline",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "ValidadoPor",
                table: "Documentos");
        }
    }
}
