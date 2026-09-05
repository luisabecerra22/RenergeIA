using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFlujoCajaSemanal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFinFlujo",
                table: "InformesConsolidados",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicioFlujo",
                table: "InformesConsolidados",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FlujosCajaSemanal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InformeConsolidadoId = table.Column<int>(type: "integer", nullable: false),
                    Semana = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    OrdenSemana = table.Column<int>(type: "integer", nullable: false),
                    Ingresos = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Pagos = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlujosCajaSemanal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlujosCajaSemanal_InformesConsolidados_InformeConsolidadoId",
                        column: x => x.InformeConsolidadoId,
                        principalTable: "InformesConsolidados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlujosCajaSemanal_InformeConsolidadoId",
                table: "FlujosCajaSemanal",
                column: "InformeConsolidadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlujosCajaSemanal");

            migrationBuilder.DropColumn(
                name: "FechaFinFlujo",
                table: "InformesConsolidados");

            migrationBuilder.DropColumn(
                name: "FechaInicioFlujo",
                table: "InformesConsolidados");
        }
    }
}
