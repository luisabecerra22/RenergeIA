using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCronogramaVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CronogramaVersionId",
                table: "ActividadesWBS",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CronogramasVersion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroVersion = table.Column<int>(type: "int", nullable: false),
                    EsVigente = table.Column<bool>(type: "bit", nullable: false),
                    MotivoReprogramacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreadoPor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CronogramasVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CronogramasVersion_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesWBS_CronogramaVersionId",
                table: "ActividadesWBS",
                column: "CronogramaVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_CronogramasVersion_ProyectoId",
                table: "CronogramasVersion",
                column: "ProyectoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ActividadesWBS_CronogramasVersion_CronogramaVersionId",
                table: "ActividadesWBS",
                column: "CronogramaVersionId",
                principalTable: "CronogramasVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActividadesWBS_CronogramasVersion_CronogramaVersionId",
                table: "ActividadesWBS");

            migrationBuilder.DropTable(
                name: "CronogramasVersion");

            migrationBuilder.DropIndex(
                name: "IX_ActividadesWBS_CronogramaVersionId",
                table: "ActividadesWBS");

            migrationBuilder.DropColumn(
                name: "CronogramaVersionId",
                table: "ActividadesWBS");
        }
    }
}
