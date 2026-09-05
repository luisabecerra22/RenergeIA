using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTipoFlujo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TipoFlujo",
                table: "FlujosCajaSemanal",
                type: "text",
                nullable: false,
                defaultValue: "USDCOP");

            migrationBuilder.Sql("UPDATE \"FlujosCajaSemanal\" SET \"TipoFlujo\" = 'USDCOP' WHERE \"TipoFlujo\" = ''");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TipoFlujo",
                table: "FlujosCajaSemanal");
        }
    }
}
