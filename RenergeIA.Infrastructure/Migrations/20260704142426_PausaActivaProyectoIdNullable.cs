using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PausaActivaProyectoIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PausasActivas_Proyectos_ProyectoId",
                table: "PausasActivas");

            migrationBuilder.AlterColumn<int>(
                name: "ProyectoId",
                table: "PausasActivas",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_PausasActivas_Proyectos_ProyectoId",
                table: "PausasActivas",
                column: "ProyectoId",
                principalTable: "Proyectos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PausasActivas_Proyectos_ProyectoId",
                table: "PausasActivas");

            migrationBuilder.AlterColumn<int>(
                name: "ProyectoId",
                table: "PausasActivas",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PausasActivas_Proyectos_ProyectoId",
                table: "PausasActivas",
                column: "ProyectoId",
                principalTable: "Proyectos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
