using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecomendacionManualActividad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RecomendacionManual",
                table: "ActividadesWBS",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RecomendacionManualFecha",
                table: "ActividadesWBS",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecomendacionManualPor",
                table: "ActividadesWBS",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecomendacionManual",
                table: "ActividadesWBS");

            migrationBuilder.DropColumn(
                name: "RecomendacionManualFecha",
                table: "ActividadesWBS");

            migrationBuilder.DropColumn(
                name: "RecomendacionManualPor",
                table: "ActividadesWBS");
        }
    }
}
