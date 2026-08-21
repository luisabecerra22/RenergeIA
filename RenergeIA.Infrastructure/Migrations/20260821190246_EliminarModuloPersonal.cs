using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EliminarModuloPersonal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentosPersona");

            migrationBuilder.DropTable(
                name: "RegistrosAvancePersonal");

            migrationBuilder.DropTable(
                name: "PersonalProyecto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonalProyecto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    Apellido = table.Column<string>(type: "text", nullable: false),
                    Cargo = table.Column<string>(type: "text", nullable: false),
                    DocumentoIdentidad = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Empresa = table.Column<string>(type: "text", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaSalida = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Telefono = table.Column<string>(type: "text", nullable: true),
                    TipoPersonal = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalProyecto", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalProyecto_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosPersona",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PersonalProyectoId = table.Column<int>(type: "integer", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    NombreDocumento = table.Column<string>(type: "text", nullable: false),
                    RutaArchivo = table.Column<string>(type: "text", nullable: false),
                    TipoDocumento = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosPersona", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentosPersona_PersonalProyecto_PersonalProyectoId",
                        column: x => x.PersonalProyectoId,
                        principalTable: "PersonalProyecto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosAvancePersonal",
                columns: table => new
                {
                    RegistroAvanceDiarioId = table.Column<int>(type: "integer", nullable: false),
                    PersonalProyectoId = table.Column<int>(type: "integer", nullable: false),
                    HorasTrabajadas = table.Column<decimal>(type: "numeric(6,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosAvancePersonal", x => new { x.RegistroAvanceDiarioId, x.PersonalProyectoId });
                    table.ForeignKey(
                        name: "FK_RegistrosAvancePersonal_PersonalProyecto_PersonalProyectoId",
                        column: x => x.PersonalProyectoId,
                        principalTable: "PersonalProyecto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosAvancePersonal_RegistrosAvanceDiario_RegistroAvanc~",
                        column: x => x.RegistroAvanceDiarioId,
                        principalTable: "RegistrosAvanceDiario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosPersona_PersonalProyectoId",
                table: "DocumentosPersona",
                column: "PersonalProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalProyecto_ProyectoId",
                table: "PersonalProyecto",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAvancePersonal_PersonalProyectoId",
                table: "RegistrosAvancePersonal",
                column: "PersonalProyectoId");
        }
    }
}
