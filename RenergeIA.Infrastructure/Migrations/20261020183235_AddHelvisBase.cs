using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHelvisBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactosHelvis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Cargo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    PuedeConsultar = table.Column<bool>(type: "boolean", nullable: false),
                    PuedeReportar = table.Column<bool>(type: "boolean", nullable: false),
                    PuedeCorregir = table.Column<bool>(type: "boolean", nullable: false),
                    PuedeAprobar = table.Column<bool>(type: "boolean", nullable: false),
                    EsAdministrador = table.Column<bool>(type: "boolean", nullable: false),
                    PausadoHasta = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    MotivoPausa = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Notas = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactosHelvis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AsignacionesHelvis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContactoHelvisId = table.Column<int>(type: "integer", nullable: false),
                    ActividadWBSId = table.Column<int>(type: "integer", nullable: false),
                    Activa = table.Column<bool>(type: "boolean", nullable: false),
                    VigenteDesde = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    VigenteHasta = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Frente = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Notas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AsignacionesHelvis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AsignacionesHelvis_ActividadesWBS_ActividadWBSId",
                        column: x => x.ActividadWBSId,
                        principalTable: "ActividadesWBS",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AsignacionesHelvis_ContactosHelvis_ContactoHelvisId",
                        column: x => x.ContactoHelvisId,
                        principalTable: "ContactosHelvis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContactoHelvisProyectos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContactoHelvisId = table.Column<int>(type: "integer", nullable: false),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactoHelvisProyectos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactoHelvisProyectos_ContactosHelvis_ContactoHelvisId",
                        column: x => x.ContactoHelvisId,
                        principalTable: "ContactosHelvis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContactoHelvisProyectos_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MensajesHelvis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ContactoHelvisId = table.Column<int>(type: "integer", nullable: true),
                    Telefono = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Entrante = table.Column<bool>(type: "boolean", nullable: false),
                    WaMessageId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Texto = table.Column<string>(type: "text", nullable: true),
                    Tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    OpcionId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    DetalleError = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MensajesHelvis", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MensajesHelvis_ContactosHelvis_ContactoHelvisId",
                        column: x => x.ContactoHelvisId,
                        principalTable: "ContactosHelvis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesHelvis_ActividadWBSId",
                table: "AsignacionesHelvis",
                column: "ActividadWBSId");

            migrationBuilder.CreateIndex(
                name: "IX_AsignacionesHelvis_ContactoHelvisId_ActividadWBSId",
                table: "AsignacionesHelvis",
                columns: new[] { "ContactoHelvisId", "ActividadWBSId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactoHelvisProyectos_ContactoHelvisId_ProyectoId",
                table: "ContactoHelvisProyectos",
                columns: new[] { "ContactoHelvisId", "ProyectoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactoHelvisProyectos_ProyectoId",
                table: "ContactoHelvisProyectos",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactosHelvis_Telefono",
                table: "ContactosHelvis",
                column: "Telefono",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MensajesHelvis_ContactoHelvisId",
                table: "MensajesHelvis",
                column: "ContactoHelvisId");

            migrationBuilder.CreateIndex(
                name: "IX_MensajesHelvis_Telefono_FechaCreacion",
                table: "MensajesHelvis",
                columns: new[] { "Telefono", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_MensajesHelvis_WaMessageId",
                table: "MensajesHelvis",
                column: "WaMessageId",
                unique: true,
                filter: "\"WaMessageId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AsignacionesHelvis");

            migrationBuilder.DropTable(
                name: "ContactoHelvisProyectos");

            migrationBuilder.DropTable(
                name: "MensajesHelvis");

            migrationBuilder.DropTable(
                name: "ContactosHelvis");
        }
    }
}
