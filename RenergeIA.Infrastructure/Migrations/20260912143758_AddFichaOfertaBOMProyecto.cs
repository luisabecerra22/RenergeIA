using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFichaOfertaBOMProyecto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BomAlcance",
                table: "Proyectos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BomCostoCOP",
                table: "Proyectos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BomCostoPlenoCostoCOP",
                table: "Proyectos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BomCostoPlenoPrecioCOP",
                table: "Proyectos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BomCostoUSD",
                table: "Proyectos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BomMargenCOP",
                table: "Proyectos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BomMargenPct",
                table: "Proyectos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BomMargenUSD",
                table: "Proyectos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BomObrasCivilesCostoCOP",
                table: "Proyectos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BomObrasCivilesPrecioCOP",
                table: "Proyectos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BomPrecioCOP",
                table: "Proyectos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BomPrecioUSD",
                table: "Proyectos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BomTotalIvaCostoCOP",
                table: "Proyectos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BomTotalIvaPrecioCOP",
                table: "Proyectos",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BomTrm",
                table: "Proyectos",
                type: "numeric",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BomAlcance",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "BomCostoCOP",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "BomCostoPlenoCostoCOP",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "BomCostoPlenoPrecioCOP",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "BomCostoUSD",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "BomMargenCOP",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "BomMargenPct",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "BomMargenUSD",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "BomObrasCivilesCostoCOP",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "BomObrasCivilesPrecioCOP",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "BomPrecioCOP",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "BomPrecioUSD",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "BomTotalIvaCostoCOP",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "BomTotalIvaPrecioCOP",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "BomTrm",
                table: "Proyectos");
        }
    }
}
