using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFacturacionC3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContratosFacturacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    NumeroContrato = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FechaContrato = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Moneda = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    Cliente = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    ValorVenta = table.Column<decimal>(type: "numeric(20,2)", nullable: false),
                    PctAnticipo = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    PctAvanceObra = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    PctCulminacionMecanica = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    PctCulminacionSustancial = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    PctAceptacionProvisional = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    PctIva = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    PctAdministracion = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    PctImprevistos = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    PctUtilidad = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    PlazoPagoDias = table.Column<int>(type: "integer", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratosFacturacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContratosFacturacion_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CortesFacturacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoFacturacionId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    FechaCorte = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    NumeroActa = table.Column<int>(type: "integer", nullable: true),
                    FechaActa = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ActaGeneradaPor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CulminacionMecanica = table.Column<bool>(type: "boolean", nullable: false),
                    CulminacionSustancial = table.Column<bool>(type: "boolean", nullable: false),
                    AceptacionProvisional = table.Column<bool>(type: "boolean", nullable: false),
                    BaseAvance = table.Column<decimal>(type: "numeric(20,2)", nullable: false),
                    BaseHitos = table.Column<decimal>(type: "numeric(20,2)", nullable: false),
                    ValorAiu = table.Column<decimal>(type: "numeric(20,2)", nullable: false),
                    ValorIva = table.Column<decimal>(type: "numeric(20,2)", nullable: false),
                    Amortizacion = table.Column<decimal>(type: "numeric(20,2)", nullable: false),
                    Total = table.Column<decimal>(type: "numeric(20,2)", nullable: false),
                    NumeroFactura = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    FechaFactura = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PlazoPagoDias = table.Column<int>(type: "integer", nullable: true),
                    FechaPagoEstimada = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FacturaRegistradaPor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CortesFacturacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CortesFacturacion_ContratosFacturacion_ContratoFacturacionId",
                        column: x => x.ContratoFacturacionId,
                        principalTable: "ContratosFacturacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemsFacturacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContratoFacturacionId = table.Column<int>(type: "integer", nullable: false),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(20,2)", nullable: false),
                    Regimen = table.Column<int>(type: "integer", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    ActividadesWbs = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsFacturacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemsFacturacion_ContratosFacturacion_ContratoFacturacionId",
                        column: x => x.ContratoFacturacionId,
                        principalTable: "ContratosFacturacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesCorteFacturacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CorteFacturacionId = table.Column<int>(type: "integer", nullable: false),
                    ItemFacturacionId = table.Column<int>(type: "integer", nullable: false),
                    PctPlaneado = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    PctCronograma = table.Column<decimal>(type: "numeric(9,4)", nullable: true),
                    PctReal = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    AjustadoManual = table.Column<bool>(type: "boolean", nullable: false),
                    AjustadoPor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PctAnterior = table.Column<decimal>(type: "numeric(9,4)", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(20,2)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesCorteFacturacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetallesCorteFacturacion_CortesFacturacion_CorteFacturacion~",
                        column: x => x.CorteFacturacionId,
                        principalTable: "CortesFacturacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesCorteFacturacion_ItemsFacturacion_ItemFacturacionId",
                        column: x => x.ItemFacturacionId,
                        principalTable: "ItemsFacturacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContratosFacturacion_ProyectoId",
                table: "ContratosFacturacion",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_CortesFacturacion_ContratoFacturacionId_Tipo_FechaCorte",
                table: "CortesFacturacion",
                columns: new[] { "ContratoFacturacionId", "Tipo", "FechaCorte" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCorteFacturacion_CorteFacturacionId_ItemFacturacion~",
                table: "DetallesCorteFacturacion",
                columns: new[] { "CorteFacturacionId", "ItemFacturacionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DetallesCorteFacturacion_ItemFacturacionId",
                table: "DetallesCorteFacturacion",
                column: "ItemFacturacionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsFacturacion_ContratoFacturacionId",
                table: "ItemsFacturacion",
                column: "ContratoFacturacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetallesCorteFacturacion");

            migrationBuilder.DropTable(
                name: "CortesFacturacion");

            migrationBuilder.DropTable(
                name: "ItemsFacturacion");

            migrationBuilder.DropTable(
                name: "ContratosFacturacion");
        }
    }
}
