using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarPapeleraProyectos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Eliminado",
                table: "Proyectos",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEliminacion",
                table: "Proyectos",
                type: "timestamp without time zone",
                nullable: true);

            // Auto-asignar disciplina "Suministros" (valor 7) a actividades con nombre que contiene "suministro"
            migrationBuilder.Sql(
                """
                UPDATE "ActividadesWBS"
                SET "Disciplina" = 7
                WHERE "Disciplina" IS NULL
                  AND LOWER("Nombre") LIKE '%suministro%'
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Eliminado",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "FechaEliminacion",
                table: "Proyectos");
        }
    }
}
