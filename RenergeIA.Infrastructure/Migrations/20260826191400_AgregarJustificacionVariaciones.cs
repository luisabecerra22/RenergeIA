using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarJustificacionVariaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JustificacionCosto",
                table: "InformesConsolidados",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JustificacionVenta",
                table: "InformesConsolidados",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JustificacionCosto",
                table: "InformesConsolidados");

            migrationBuilder.DropColumn(
                name: "JustificacionVenta",
                table: "InformesConsolidados");
        }
    }
}
