using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTasaCambioDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TasaCambioCOPUSD",
                table: "Proyectos",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 4000m,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.Sql(
                "UPDATE \"Proyectos\" SET \"TasaCambioCOPUSD\" = 4000 WHERE \"TasaCambioCOPUSD\" = 0;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TasaCambioCOPUSD",
                table: "Proyectos",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldDefaultValue: 4000m);
        }
    }
}
