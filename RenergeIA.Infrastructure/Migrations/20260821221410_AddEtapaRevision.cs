using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEtapaRevision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaIngresoSitio",
                table: "RecursosEquipo",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EtapasRevision",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RecursoEquipoId = table.Column<int>(type: "integer", nullable: false),
                    Etapa = table.Column<int>(type: "integer", nullable: false),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    ResponsableNombre = table.Column<string>(type: "text", nullable: true),
                    ResponsableEmail = table.Column<string>(type: "text", nullable: true),
                    FechaEnvio = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaRecepcion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaComentarios = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DetalleComentarios = table.Column<string>(type: "text", nullable: true),
                    FechaAprobacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EtapasRevision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EtapasRevision_RecursosEquipo_RecursoEquipoId",
                        column: x => x.RecursoEquipoId,
                        principalTable: "RecursosEquipo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EtapasRevision_RecursoEquipoId_Etapa",
                table: "EtapasRevision",
                columns: new[] { "RecursoEquipoId", "Etapa" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EtapasRevision");

            migrationBuilder.DropColumn(
                name: "FechaIngresoSitio",
                table: "RecursosEquipo");
        }
    }
}
