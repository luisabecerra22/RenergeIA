using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHelvisReportesYBloqueos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OpcionPendiente",
                table: "ContactosHelvis",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Flujo",
                table: "ContactosHelvis",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlujoDatos",
                table: "ContactosHelvis",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FlujoFecha",
                table: "ContactosHelvis",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlujoPaso",
                table: "ContactosHelvis",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RecibeBloqueos",
                table: "ContactosHelvis",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RecibeInformes",
                table: "ContactosHelvis",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FotosHelvis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContactoHelvisId = table.Column<int>(type: "integer", nullable: true),
                    ProyectoId = table.Column<int>(type: "integer", nullable: true),
                    ReporteAvanceHelvisId = table.Column<int>(type: "integer", nullable: true),
                    ActividadWBSId = table.Column<int>(type: "integer", nullable: true),
                    InformeDiarioId = table.Column<int>(type: "integer", nullable: true),
                    WaMediaId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    MimeType = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Contenido = table.Column<byte[]>(type: "bytea", nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotosHelvis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FotosHelvis_ContactosHelvis_ContactoHelvisId",
                        column: x => x.ContactoHelvisId,
                        principalTable: "ContactosHelvis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ReportesAvanceHelvis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContactoHelvisId = table.Column<int>(type: "integer", nullable: false),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    ActividadWBSId = table.Column<int>(type: "integer", nullable: true),
                    FechaTrabajo = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Paso = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TextoOriginal = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Cantidad = table.Column<decimal>(type: "numeric", nullable: true),
                    CantidadEsAcumulada = table.Column<bool>(type: "boolean", nullable: true),
                    Porcentaje = table.Column<decimal>(type: "numeric", nullable: true),
                    PorcentajeAnterior = table.Column<decimal>(type: "numeric", nullable: true),
                    PorcentajeNuevo = table.Column<decimal>(type: "numeric", nullable: true),
                    CantidadDia = table.Column<decimal>(type: "numeric", nullable: true),
                    SumarAlDeHoy = table.Column<bool>(type: "boolean", nullable: true),
                    Motivo = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Personas = table.Column<int>(type: "integer", nullable: true),
                    Horas = table.Column<decimal>(type: "numeric", nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RecursosRespondidos = table.Column<bool>(type: "boolean", nullable: false),
                    FotosRespondidas = table.Column<bool>(type: "boolean", nullable: false),
                    OpcionesActividad = table.Column<string>(type: "text", nullable: true),
                    FechaConfirmacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InformeDiarioId = table.Column<int>(type: "integer", nullable: true),
                    RegistroAvanceDiarioId = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesAvanceHelvis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportesAvanceHelvis_ActividadesWBS_ActividadWBSId",
                        column: x => x.ActividadWBSId,
                        principalTable: "ActividadesWBS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ReportesAvanceHelvis_ContactosHelvis_ContactoHelvisId",
                        column: x => x.ContactoHelvisId,
                        principalTable: "ContactosHelvis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FotosHelvis_ContactoHelvisId",
                table: "FotosHelvis",
                column: "ContactoHelvisId");

            migrationBuilder.CreateIndex(
                name: "IX_FotosHelvis_InformeDiarioId",
                table: "FotosHelvis",
                column: "InformeDiarioId");

            migrationBuilder.CreateIndex(
                name: "IX_FotosHelvis_ReporteAvanceHelvisId",
                table: "FotosHelvis",
                column: "ReporteAvanceHelvisId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesAvanceHelvis_ActividadWBSId",
                table: "ReportesAvanceHelvis",
                column: "ActividadWBSId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesAvanceHelvis_ContactoHelvisId_Estado",
                table: "ReportesAvanceHelvis",
                columns: new[] { "ContactoHelvisId", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportesAvanceHelvis_ProyectoId_FechaTrabajo",
                table: "ReportesAvanceHelvis",
                columns: new[] { "ProyectoId", "FechaTrabajo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FotosHelvis");

            migrationBuilder.DropTable(
                name: "ReportesAvanceHelvis");

            migrationBuilder.DropColumn(
                name: "Flujo",
                table: "ContactosHelvis");

            migrationBuilder.DropColumn(
                name: "FlujoDatos",
                table: "ContactosHelvis");

            migrationBuilder.DropColumn(
                name: "FlujoFecha",
                table: "ContactosHelvis");

            migrationBuilder.DropColumn(
                name: "FlujoPaso",
                table: "ContactosHelvis");

            migrationBuilder.DropColumn(
                name: "RecibeBloqueos",
                table: "ContactosHelvis");

            migrationBuilder.DropColumn(
                name: "RecibeInformes",
                table: "ContactosHelvis");

            migrationBuilder.AlterColumn<string>(
                name: "OpcionPendiente",
                table: "ContactosHelvis",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);
        }
    }
}
