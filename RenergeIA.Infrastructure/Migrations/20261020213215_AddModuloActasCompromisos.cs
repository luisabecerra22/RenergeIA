using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModuloActasCompromisos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LotesImportacionActas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Archivo = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Usuario = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ResumenJson = table.Column<string>(type: "text", nullable: true),
                    RespaldoJson = table.Column<string>(type: "text", nullable: true),
                    Revertido = table.Column<bool>(type: "boolean", nullable: false),
                    RevertidoEl = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotesImportacionActas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProyectosActa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AreasJson = table.Column<string>(type: "text", nullable: true),
                    AliasJson = table.Column<string>(type: "text", nullable: true),
                    CodigosAnterioresJson = table.Column<string>(type: "text", nullable: true),
                    ProyectoId = table.Column<int>(type: "integer", nullable: true),
                    IdOrigen = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    LoteImportacionId = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectosActa", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProyectosActa_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosSeguimientoActa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    IdOrigen = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ProyectoTexto = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Tema = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: true),
                    Accion = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    EstadoRegistro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Evidencia = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ReferenciasJson = table.Column<string>(type: "text", nullable: true),
                    DatosOrigenJson = table.Column<string>(type: "text", nullable: true),
                    HistorialJson = table.Column<string>(type: "text", nullable: true),
                    Confirmada = table.Column<bool>(type: "boolean", nullable: false),
                    ConfirmadoPor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ActualizadoEl = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LoteImportacionId = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosSeguimientoActa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActasReunion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaTexto = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ProyectoTexto = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ProyectoActaId = table.Column<int>(type: "integer", nullable: true),
                    Area = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Asunto = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Modalidad = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Hora = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Resumen = table.Column<string>(type: "text", nullable: true),
                    AsistentesJson = table.Column<string>(type: "text", nullable: true),
                    PuntosJson = table.Column<string>(type: "text", nullable: true),
                    ActaAnteriorCodigo = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    TrazabilidadJson = table.Column<string>(type: "text", nullable: true),
                    SeguimientoPendienteConfirmacion = table.Column<bool>(type: "boolean", nullable: false),
                    FuenteUrl = table.Column<string>(type: "character varying(600)", maxLength: 600, nullable: true),
                    DatosOrigenJson = table.Column<string>(type: "text", nullable: true),
                    Origen = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    LoteImportacionId = table.Column<int>(type: "integer", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FechaEliminacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActasReunion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActasReunion_ProyectosActa_ProyectoActaId",
                        column: x => x.ProyectoActaId,
                        principalTable: "ProyectosActa",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ArchivosActaReunion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActaReunionId = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Filename = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Tamano = table.Column<int>(type: "integer", nullable: false),
                    Contenido = table.Column<byte[]>(type: "bytea", nullable: false),
                    LoteImportacionId = table.Column<int>(type: "integer", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivosActaReunion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivosActaReunion_ActasReunion_ActaReunionId",
                        column: x => x.ActaReunionId,
                        principalTable: "ActasReunion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ComentariosActaReunion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActaReunionId = table.Column<int>(type: "integer", nullable: false),
                    Texto = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: false),
                    Autor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Creado = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Atendido = table.Column<bool>(type: "boolean", nullable: false),
                    AtendidoPor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    AtendidoEl = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IdOrigen = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComentariosActaReunion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ComentariosActaReunion_ActasReunion_ActaReunionId",
                        column: x => x.ActaReunionId,
                        principalTable: "ActasReunion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompromisosReunion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActaReunionId = table.Column<int>(type: "integer", nullable: false),
                    IdOrigen = table.Column<string>(type: "character varying(170)", maxLength: 170, nullable: false),
                    Orden = table.Column<int>(type: "integer", nullable: false),
                    Texto = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: false),
                    Responsable = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    ResponsablePrincipal = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Empresa = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FechaTexto = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    FechaLimite = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Estado = table.Column<int>(type: "integer", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Categoria = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    NotaCierre = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CerradoPor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CerradoEl = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    RequiereConfirmacion = table.Column<bool>(type: "boolean", nullable: false),
                    ObservacionSeguimiento = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ContinuidadDeId = table.Column<int>(type: "integer", nullable: true),
                    ContinuidadDeOrigen = table.Column<string>(type: "character varying(170)", maxLength: 170, nullable: true),
                    ProyectoTexto = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Area = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FechaActa = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    OrigenOriginalJson = table.Column<string>(type: "text", nullable: true),
                    HistorialJson = table.Column<string>(type: "text", nullable: true),
                    CreadoOrigen = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EditadoPor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EditadoEl = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LoteImportacionId = table.Column<int>(type: "integer", nullable: true),
                    Eliminado = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    FechaEliminacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompromisosReunion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompromisosReunion_ActasReunion_ActaReunionId",
                        column: x => x.ActaReunionId,
                        principalTable: "ActasReunion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompromisosReunion_CompromisosReunion_ContinuidadDeId",
                        column: x => x.ContinuidadDeId,
                        principalTable: "CompromisosReunion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ActualizacionesCompromisoReunion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompromisoReunionId = table.Column<int>(type: "integer", nullable: false),
                    Fecha = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Autor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TipoActualizacion = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    EstadoAnterior = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    EstadoNuevo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Observacion = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: true),
                    ActaReferencia = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    CambiosJson = table.Column<string>(type: "text", nullable: true),
                    Origen = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActualizacionesCompromisoReunion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActualizacionesCompromisoReunion_CompromisosReunion_Comprom~",
                        column: x => x.CompromisoReunionId,
                        principalTable: "CompromisosReunion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActasReunion_Codigo",
                table: "ActasReunion",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActasReunion_ProyectoActaId",
                table: "ActasReunion",
                column: "ProyectoActaId");

            migrationBuilder.CreateIndex(
                name: "IX_ActualizacionesCompromisoReunion_CompromisoReunionId",
                table: "ActualizacionesCompromisoReunion",
                column: "CompromisoReunionId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivosActaReunion_ActaReunionId_Tipo",
                table: "ArchivosActaReunion",
                columns: new[] { "ActaReunionId", "Tipo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ComentariosActaReunion_ActaReunionId",
                table: "ComentariosActaReunion",
                column: "ActaReunionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosReunion_ActaReunionId",
                table: "CompromisosReunion",
                column: "ActaReunionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosReunion_ContinuidadDeId",
                table: "CompromisosReunion",
                column: "ContinuidadDeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisosReunion_IdOrigen",
                table: "CompromisosReunion",
                column: "IdOrigen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProyectosActa_ProyectoId",
                table: "ProyectosActa",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosSeguimientoActa_IdOrigen",
                table: "RegistrosSeguimientoActa",
                column: "IdOrigen",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActualizacionesCompromisoReunion");

            migrationBuilder.DropTable(
                name: "ArchivosActaReunion");

            migrationBuilder.DropTable(
                name: "ComentariosActaReunion");

            migrationBuilder.DropTable(
                name: "LotesImportacionActas");

            migrationBuilder.DropTable(
                name: "RegistrosSeguimientoActa");

            migrationBuilder.DropTable(
                name: "CompromisosReunion");

            migrationBuilder.DropTable(
                name: "ActasReunion");

            migrationBuilder.DropTable(
                name: "ProyectosActa");
        }
    }
}
