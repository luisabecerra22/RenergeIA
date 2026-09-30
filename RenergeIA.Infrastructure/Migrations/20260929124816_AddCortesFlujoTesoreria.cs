using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCortesFlujoTesoreria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CorteFlujoActual",
                table: "InformesConsolidados",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CorteFlujoAnterior",
                table: "InformesConsolidados",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CortesFlujoTesoreria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    FechaCorte = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Archivo = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Semana = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    PagosCOP = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PagosUSD = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CortesFlujoTesoreria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CortesFlujoTesoreria_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CortesFlujoTesoreria_ProyectoId_FechaCorte_Semana",
                table: "CortesFlujoTesoreria",
                columns: new[] { "ProyectoId", "FechaCorte", "Semana" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CortesFlujoTesoreria");

            migrationBuilder.DropColumn(
                name: "CorteFlujoActual",
                table: "InformesConsolidados");

            migrationBuilder.DropColumn(
                name: "CorteFlujoAnterior",
                table: "InformesConsolidados");
        }
    }
}
