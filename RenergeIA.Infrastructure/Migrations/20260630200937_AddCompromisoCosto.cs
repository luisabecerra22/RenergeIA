using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCompromisoCosto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistsAuditoria_Proyectos_ProyectoId",
                table: "ChecklistsAuditoria");

            migrationBuilder.AddColumn<string>(
                name: "AdjuntoUrl",
                table: "CostosReales",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ProyectoId",
                table: "ChecklistsAuditoria",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "NormaISO",
                table: "ChecklistsAuditoria",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Division",
                table: "ChecklistsAuditoria",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ProcesoArea",
                table: "ChecklistsAuditoria",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoAuditoria",
                table: "ChecklistsAuditoria",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TipoNorma",
                table: "ChecklistsAuditoria",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CompromisoCostos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: false),
                    PartidaId = table.Column<int>(type: "int", nullable: true),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Proveedor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Prioridad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompromisoCostos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompromisoCostos_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CompromisoCostos_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompromisoCostos_PartidaId",
                table: "CompromisoCostos",
                column: "PartidaId");

            migrationBuilder.CreateIndex(
                name: "IX_CompromisoCostos_ProyectoId",
                table: "CompromisoCostos",
                column: "ProyectoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistsAuditoria_Proyectos_ProyectoId",
                table: "ChecklistsAuditoria",
                column: "ProyectoId",
                principalTable: "Proyectos",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChecklistsAuditoria_Proyectos_ProyectoId",
                table: "ChecklistsAuditoria");

            migrationBuilder.DropTable(
                name: "CompromisoCostos");

            migrationBuilder.DropColumn(
                name: "AdjuntoUrl",
                table: "CostosReales");

            migrationBuilder.DropColumn(
                name: "ProcesoArea",
                table: "ChecklistsAuditoria");

            migrationBuilder.DropColumn(
                name: "TipoAuditoria",
                table: "ChecklistsAuditoria");

            migrationBuilder.DropColumn(
                name: "TipoNorma",
                table: "ChecklistsAuditoria");

            migrationBuilder.AlterColumn<int>(
                name: "ProyectoId",
                table: "ChecklistsAuditoria",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NormaISO",
                table: "ChecklistsAuditoria",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "Division",
                table: "ChecklistsAuditoria",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChecklistsAuditoria_Proyectos_ProyectoId",
                table: "ChecklistsAuditoria",
                column: "ProyectoId",
                principalTable: "Proyectos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
