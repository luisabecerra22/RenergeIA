using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAnalisisIA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AnalisisIA",
                table: "InformesConsolidados",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MostrarAnalisisEnPrint",
                table: "InformesConsolidados",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnalisisIA",
                table: "InformesConsolidados");

            migrationBuilder.DropColumn(
                name: "MostrarAnalisisEnPrint",
                table: "InformesConsolidados");
        }
    }
}
