using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarHistogramaReal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistogramasReales",
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
                    table.PrimaryKey("PK_HistogramasReales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistogramasReales_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsHistogramaReal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HistogramaRealId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                    table.PrimaryKey("PK_ItemsHistogramaReal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsHistogramaReal_HistogramasReales_HistogramaRealId",
                        column: x => x.HistogramaRealId,
                        principalTable: "HistogramasReales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistogramasReales_ProyectoId",
                table: "HistogramasReales",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsHistogramaReal_HistogramaRealId",
                table: "ItemsHistogramaReal",
                column: "HistogramaRealId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemsHistogramaReal");

            migrationBuilder.DropTable(
                name: "HistogramasReales");
        }
    }
}
