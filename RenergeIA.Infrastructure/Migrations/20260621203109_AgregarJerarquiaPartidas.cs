using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarJerarquiaPartidas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsPrincipal",
                table: "Partidas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Nivel",
                table: "Partidas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "Partidas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PadreId",
                table: "Partidas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Partidas_PadreId",
                table: "Partidas",
                column: "PadreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Partidas_Partidas_PadreId",
                table: "Partidas",
                column: "PadreId",
                principalTable: "Partidas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Partidas_Partidas_PadreId",
                table: "Partidas");

            migrationBuilder.DropIndex(
                name: "IX_Partidas_PadreId",
                table: "Partidas");

            migrationBuilder.DropColumn(
                name: "EsPrincipal",
                table: "Partidas");

            migrationBuilder.DropColumn(
                name: "Nivel",
                table: "Partidas");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Partidas");

            migrationBuilder.DropColumn(
                name: "PadreId",
                table: "Partidas");
        }
    }
}
