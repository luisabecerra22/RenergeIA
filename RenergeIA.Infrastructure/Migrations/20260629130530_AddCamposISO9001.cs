using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposISO9001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Evidencia",
                table: "ItemsChecklist",
                newName: "OportunidadMejora");

            migrationBuilder.AlterColumn<string>(
                name: "Responsable",
                table: "ItemsChecklist",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Clausula",
                table: "ItemsChecklist",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EvidenciaUrl",
                table: "ItemsChecklist",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hallazgo",
                table: "ItemsChecklist",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Plazo",
                table: "ItemsChecklist",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Puntaje",
                table: "ItemsChecklist",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Seguimiento",
                table: "ItemsChecklist",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TituloClausula",
                table: "ItemsChecklist",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EstadoAuditoria",
                table: "ChecklistsAuditoria",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "ChecklistsAuditoria",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Clausula",
                table: "ItemsChecklist");

            migrationBuilder.DropColumn(
                name: "EvidenciaUrl",
                table: "ItemsChecklist");

            migrationBuilder.DropColumn(
                name: "Hallazgo",
                table: "ItemsChecklist");

            migrationBuilder.DropColumn(
                name: "Plazo",
                table: "ItemsChecklist");

            migrationBuilder.DropColumn(
                name: "Puntaje",
                table: "ItemsChecklist");

            migrationBuilder.DropColumn(
                name: "Seguimiento",
                table: "ItemsChecklist");

            migrationBuilder.DropColumn(
                name: "TituloClausula",
                table: "ItemsChecklist");

            migrationBuilder.DropColumn(
                name: "EstadoAuditoria",
                table: "ChecklistsAuditoria");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "ChecklistsAuditoria");

            migrationBuilder.RenameColumn(
                name: "OportunidadMejora",
                table: "ItemsChecklist",
                newName: "Evidencia");

            migrationBuilder.AlterColumn<string>(
                name: "Responsable",
                table: "ItemsChecklist",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }
    }
}
