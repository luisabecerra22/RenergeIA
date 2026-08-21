using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddControlIngresoDocumentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Proveedores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    NIT = table.Column<string>(type: "text", nullable: true),
                    EsRenergeia = table.Column<bool>(type: "boolean", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proveedores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proveedores_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TiposDocumentoControl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Categoria = table.Column<int>(type: "integer", nullable: false),
                    RequiereVencimiento = table.Column<bool>(type: "boolean", nullable: false),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposDocumentoControl", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonasExternas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    ProveedorId = table.Column<int>(type: "integer", nullable: true),
                    Nombre = table.Column<string>(type: "text", nullable: false),
                    Cedula = table.Column<string>(type: "text", nullable: true),
                    Rol = table.Column<string>(type: "text", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonasExternas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonasExternas_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonasExternas_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecursosEquipo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    ProveedorId = table.Column<int>(type: "integer", nullable: true),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Descripcion = table.Column<string>(type: "text", nullable: false),
                    PlacaSerial = table.Column<string>(type: "text", nullable: true),
                    ConductorOperadorId = table.Column<int>(type: "integer", nullable: true),
                    FechaInicioContrato = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaFinContrato = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecursosEquipo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecursosEquipo_PersonasExternas_ConductorOperadorId",
                        column: x => x.ConductorOperadorId,
                        principalTable: "PersonasExternas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecursosEquipo_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecursosEquipo_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosControl",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProyectoId = table.Column<int>(type: "integer", nullable: false),
                    TipoDocumentoControlId = table.Column<int>(type: "integer", nullable: false),
                    PersonaExternaId = table.Column<int>(type: "integer", nullable: true),
                    RecursoEquipoId = table.Column<int>(type: "integer", nullable: true),
                    ProveedorId = table.Column<int>(type: "integer", nullable: true),
                    FechaVencimiento = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Entregado = table.Column<bool>(type: "boolean", nullable: false),
                    Vigente = table.Column<bool>(type: "boolean", nullable: false),
                    RutaArchivo = table.Column<string>(type: "text", nullable: true),
                    NombreArchivo = table.Column<string>(type: "text", nullable: true),
                    ResponsableNombre = table.Column<string>(type: "text", nullable: true),
                    ResponsableEmail = table.Column<string>(type: "text", nullable: true),
                    Observaciones = table.Column<string>(type: "text", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosControl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentosControl_PersonasExternas_PersonaExternaId",
                        column: x => x.PersonaExternaId,
                        principalTable: "PersonasExternas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentosControl_Proveedores_ProveedorId",
                        column: x => x.ProveedorId,
                        principalTable: "Proveedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentosControl_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentosControl_RecursosEquipo_RecursoEquipoId",
                        column: x => x.RecursoEquipoId,
                        principalTable: "RecursosEquipo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentosControl_TiposDocumentoControl_TipoDocumentoContro~",
                        column: x => x.TipoDocumentoControlId,
                        principalTable: "TiposDocumentoControl",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosControl_PersonaExternaId",
                table: "DocumentosControl",
                column: "PersonaExternaId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosControl_ProveedorId",
                table: "DocumentosControl",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosControl_ProyectoId",
                table: "DocumentosControl",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosControl_RecursoEquipoId",
                table: "DocumentosControl",
                column: "RecursoEquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosControl_TipoDocumentoControlId",
                table: "DocumentosControl",
                column: "TipoDocumentoControlId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonasExternas_ProveedorId",
                table: "PersonasExternas",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonasExternas_ProyectoId",
                table: "PersonasExternas",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Proveedores_ProyectoId",
                table: "Proveedores",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_RecursosEquipo_ConductorOperadorId",
                table: "RecursosEquipo",
                column: "ConductorOperadorId");

            migrationBuilder.CreateIndex(
                name: "IX_RecursosEquipo_ProveedorId",
                table: "RecursosEquipo",
                column: "ProveedorId");

            migrationBuilder.CreateIndex(
                name: "IX_RecursosEquipo_ProyectoId",
                table: "RecursosEquipo",
                column: "ProyectoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentosControl");

            migrationBuilder.DropTable(
                name: "RecursosEquipo");

            migrationBuilder.DropTable(
                name: "TiposDocumentoControl");

            migrationBuilder.DropTable(
                name: "PersonasExternas");

            migrationBuilder.DropTable(
                name: "Proveedores");
        }
    }
}
