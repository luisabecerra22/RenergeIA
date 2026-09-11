using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPagosCorteSemanal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFinPagos",
                table: "Proyectos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicioPagos",
                table: "Proyectos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PagosCorteSemanal",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    PartidaId = table.Column<int>(type: "integer", nullable: false),
                    FechaCorte = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Monto = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NumeroFactura = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Proveedor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagosCorteSemanal", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagosCorteSemanal_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PagosCorteSemanal_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PagosCorteSemanal_PartidaId",
                table: "PagosCorteSemanal",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_PagosCorteSemanal_ProyectoId",
                table: "PagosCorteSemanal",
                column: "ProyectoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagosCorteSemanal");

            migrationBuilder.DropColumn(
                name: "FechaFinPagos",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "FechaInicioPagos",
                table: "Proyectos");
        }
    }
}
