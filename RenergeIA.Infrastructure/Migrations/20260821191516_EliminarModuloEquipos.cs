using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EliminarModuloEquipos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mantenimientos");

            migrationBuilder.DropTable(
                name: "RegistrosAvanceEquipo");

            migrationBuilder.DropTable(
                name: "RegistrosHorometro");

            migrationBuilder.DropTable(
                name: "Equipos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    Codigo = table.Column<string>(type: "text", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaIngreso = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaSalida = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Marca = table.Column<string>(type: "text", nullable: true),
                    Modelo = table.Column<string>(type: "text", nullable: true),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    NumeroSerie = table.Column<string>(type: "text", nullable: true),
                    Propietario = table.Column<string>(type: "text", nullable: true),
                    TipoEquipo = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Equipos_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mantenimientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipoId = table.Column<int>(type: "integer", nullable: false),
                    Costo = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ProximoMantenimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RealizadoPor = table.Column<string>(type: "text", nullable: true),
                    TipoMantenimiento = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mantenimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mantenimientos_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosAvanceEquipo",
                columns: table => new
                {
                    RegistroAvanceDiarioId = table.Column<int>(type: "integer", nullable: false),
                    EquipoId = table.Column<int>(type: "integer", nullable: false),
                    HorasUtilizadas = table.Column<decimal>(type: "numeric(6,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosAvanceEquipo", x => new { x.RegistroAvanceDiarioId, x.EquipoId });
                    table.ForeignKey(
                        name: "FK_RegistrosAvanceEquipo_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RegistrosAvanceEquipo_RegistrosAvanceDiario_RegistroAvanceD~",
                        column: x => x.RegistroAvanceDiarioId,
                        principalTable: "RegistrosAvanceDiario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosHorometro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EquipoId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    HorasTrabajadas = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    LecturaHorometro = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    Operador = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosHorometro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosHorometro_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_ProyectoId",
                table: "Equipos",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Mantenimientos_EquipoId",
                table: "Mantenimientos",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosAvanceEquipo_EquipoId",
                table: "RegistrosAvanceEquipo",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosHorometro_EquipoId",
                table: "RegistrosHorometro",
                column: "EquipoId");
        }
    }
}
