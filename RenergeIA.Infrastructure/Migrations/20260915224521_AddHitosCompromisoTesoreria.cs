using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHitosCompromisoTesoreria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArchivoTesoreria",
                table: "Proyectos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCorteTesoreria",
                table: "Proyectos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClaveTesoreria",
                table: "CompromisoCostos",
                type: "character varying(400)",
                maxLength: 400,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Grupo",
                table: "CompromisoCostos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "OC");

            migrationBuilder.AddColumn<string>(
                name: "Origen",
                table: "CompromisoCostos",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Manual");

            migrationBuilder.AddColumn<decimal>(
                name: "ValorPagado",
                table: "CompromisoCostos",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "AsignacionesTesoreria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Clave = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: false),
                    Valor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionesTesoreria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HitosCompromiso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompromisoCostoId = table.Column<int>(type: "integer", nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Periodo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CodigoManual = table.Column<bool>(type: "boolean", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    Detalle = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Documento = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Subtotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Iva = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Importe = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    RetFuente = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    RetIca = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TotalPagar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Pagado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HitosCompromiso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HitosCompromiso_CompromisoCostos_CompromisoCostoId",
                        column: x => x.CompromisoCostoId,
                        principalTable: "CompromisoCostos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesTesoreria_ProyectoId_Tipo_Clave",
                table: "AsignacionesTesoreria",
                columns: new[] { "ProyectoId", "Tipo", "Clave" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HitosCompromiso_CompromisoCostoId",
                table: "HitosCompromiso",
                column: "CompromisoCostoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignacionesTesoreria");

            migrationBuilder.DropTable(
                name: "HitosCompromiso");

            migrationBuilder.DropColumn(
                name: "ArchivoTesoreria",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "FechaCorteTesoreria",
                table: "Proyectos");

            migrationBuilder.DropColumn(
                name: "ClaveTesoreria",
                table: "CompromisoCostos");

            migrationBuilder.DropColumn(
                name: "Grupo",
                table: "CompromisoCostos");

            migrationBuilder.DropColumn(
                name: "Origen",
                table: "CompromisoCostos");

            migrationBuilder.DropColumn(
                name: "ValorPagado",
                table: "CompromisoCostos");
        }
    }
}
