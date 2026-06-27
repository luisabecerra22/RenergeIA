using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDisciplinaYRelacionClima : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InformeDiarioId",
                table: "RegistrosClima",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Disciplina",
                table: "ActividadesWBS",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosClima_InformeDiarioId",
                table: "RegistrosClima",
                column: "InformeDiarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_RegistrosClima_InformesDiarios_InformeDiarioId",
                table: "RegistrosClima",
                column: "InformeDiarioId",
                principalTable: "InformesDiarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RegistrosClima_InformesDiarios_InformeDiarioId",
                table: "RegistrosClima");

            migrationBuilder.DropIndex(
                name: "IX_RegistrosClima_InformeDiarioId",
                table: "RegistrosClima");

            migrationBuilder.DropColumn(
                name: "InformeDiarioId",
                table: "RegistrosClima");

            migrationBuilder.DropColumn(
                name: "Disciplina",
                table: "ActividadesWBS");
        }
    }
}
