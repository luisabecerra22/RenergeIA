using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DocumentosControlRevisiones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Area",
                table: "Documentos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Categoria",
                table: "Documentos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CodigoCliente",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Fase",
                table: "Documentos",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaDevolucion1",
                table: "Documentos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaDevolucion2",
                table: "Documentos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaDevolucion3",
                table: "Documentos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaDevolucion4",
                table: "Documentos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEntrega1",
                table: "Documentos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEntrega2",
                table: "Documentos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEntrega3",
                table: "Documentos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEntrega4",
                table: "Documentos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaValidacion",
                table: "Documentos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Responsable",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Transmittal",
                table: "Documentos",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Version",
                table: "Documentos",
                type: "text",
                nullable: false,
                defaultValue: "0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Area",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "CodigoCliente",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Fase",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FechaDevolucion1",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FechaDevolucion2",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FechaDevolucion3",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FechaDevolucion4",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FechaEntrega1",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FechaEntrega2",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FechaEntrega3",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FechaEntrega4",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "FechaValidacion",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Responsable",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Transmittal",
                table: "Documentos");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Documentos");
        }
    }
}
