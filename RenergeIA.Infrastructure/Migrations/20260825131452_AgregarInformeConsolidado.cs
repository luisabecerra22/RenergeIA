using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarInformeConsolidado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InformesConsolidados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    NumeroInforme = table.Column<int>(type: "integer", nullable: false),
                    Version = table.Column<int>(type: "integer", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    TRM = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TRMBomInicial = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    CreadoPor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Responsable = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    VentaContractualCOP = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    VentaContractualUSD = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PresupuestoCOP = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EjecutadoCOP = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ComprometidoCOP = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PresupuestoUSD = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EjecutadoUSD = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ComprometidoUSD = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ImprevistosCOP = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ImprevistosUSD = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalPOsCOP = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalPOsUSD = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ConsolidadoAnteriorId = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InformesConsolidados", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InformesConsolidados_InformesConsolidados_ConsolidadoAnteri~",
                        column: x => x.ConsolidadoAnteriorId,
                        principalTable: "InformesConsolidados",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_InformesConsolidados_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LineasConsolidado",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InformeConsolidadoId = table.Column<int>(type: "integer", nullable: false),
                    Categoria = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CodigoCategoria = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    PresupuestoCOP = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EjecutadoCOP = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ComprometidoCOP = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PresupuestoUSD = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    EjecutadoUSD = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ComprometidoUSD = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineasConsolidado", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LineasConsolidado_InformesConsolidados_InformeConsolidadoId",
                        column: x => x.InformeConsolidadoId,
                        principalTable: "InformesConsolidados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InformesConsolidados_ConsolidadoAnteriorId",
                table: "InformesConsolidados",
                column: "ConsolidadoAnteriorId");

            migrationBuilder.CreateIndex(
                name: "IX_InformesConsolidados_ProyectoId",
                table: "InformesConsolidados",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_LineasConsolidado_InformeConsolidadoId",
                table: "LineasConsolidado",
                column: "InformeConsolidadoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LineasConsolidado");

            migrationBuilder.DropTable(
                name: "InformesConsolidados");
        }
    }
}
