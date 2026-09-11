using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCamposOrdenCompra : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescripcionServicio",
                table: "CompromisoCostos",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFactura",
                table: "CompromisoCostos",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroFactura",
                table: "CompromisoCostos",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SaldoPorPagar",
                table: "CompromisoCostos",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorFactura",
                table: "CompromisoCostos",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescripcionServicio",
                table: "CompromisoCostos");

            migrationBuilder.DropColumn(
                name: "FechaFactura",
                table: "CompromisoCostos");

            migrationBuilder.DropColumn(
                name: "NumeroFactura",
                table: "CompromisoCostos");

            migrationBuilder.DropColumn(
                name: "SaldoPorPagar",
                table: "CompromisoCostos");

            migrationBuilder.DropColumn(
                name: "ValorFactura",
                table: "CompromisoCostos");
        }
    }
}
