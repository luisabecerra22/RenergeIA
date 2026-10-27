using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHelvisProyectoActivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ProyectoActivoFecha",
                table: "ContactosHelvis",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProyectoActivoId",
                table: "ContactosHelvis",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProyectoActivoFecha",
                table: "ContactosHelvis");

            migrationBuilder.DropColumn(
                name: "ProyectoActivoId",
                table: "ContactosHelvis");
        }
    }
}
