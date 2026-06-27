using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarHistogramas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlantillasHistograma",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillasHistograma", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlantillasHistograma_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsHistograma",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlantillaHistogramaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mes1 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Mes2 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Mes3 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Mes4 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Mes5 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Mes6 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Mes7 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Mes8 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Mes9 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Mes10 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Mes11 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Mes12 = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsHistograma", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsHistograma_PlantillasHistograma_PlantillaHistogramaId",
                        column: x => x.PlantillaHistogramaId,
                        principalTable: "PlantillasHistograma",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemsHistograma_PlantillaHistogramaId",
                table: "ItemsHistograma",
                column: "PlantillaHistogramaId");

            migrationBuilder.CreateIndex(
                name: "IX_PlantillasHistograma_ProyectoId",
                table: "PlantillasHistograma",
                column: "ProyectoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemsHistograma");

            migrationBuilder.DropTable(
                name: "PlantillasHistograma");
        }
    }
}
