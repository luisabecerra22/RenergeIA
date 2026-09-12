using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLineasBOM : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LineasBOM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Fuente = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Concepto = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Unidad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CantidadBOM = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CostoUnitarioBOM = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    MonedaCosto = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    CostoTotalBOM = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CantidadReal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ValorReal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineasBOM", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineasBOM_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LineasBOM_ProyectoId_Codigo",
                table: "LineasBOM",
                columns: new[] { "ProyectoId", "Codigo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineasBOM");
        }
    }
}
