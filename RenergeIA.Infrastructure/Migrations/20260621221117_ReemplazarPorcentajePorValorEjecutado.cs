using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReemplazarPorcentajePorValorEjecutado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PorcentajeEjecutado",
                table: "Partidas");

            migrationBuilder.AddColumn<decimal>(
                name: "ValorEjecutado",
                table: "Partidas",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValorEjecutado",
                table: "Partidas");

            migrationBuilder.AddColumn<decimal>(
                name: "PorcentajeEjecutado",
                table: "Partidas",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
