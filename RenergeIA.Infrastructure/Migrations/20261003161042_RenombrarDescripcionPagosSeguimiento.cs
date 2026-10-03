using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RenergeIA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenombrarDescripcionPagosSeguimiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Renombra en los registros existentes la descripción interna "Tesorería ..."
            // (que además podía incluir el nombre del archivo) por la etiqueta neutra
            // "Seguimiento de pagos". La clasificación real se conserva vía PartidaId.
            migrationBuilder.Sql(
                "UPDATE \"PagosCorteSemanal\" SET \"Descripcion\" = 'Seguimiento de pagos' " +
                "WHERE \"Descripcion\" LIKE 'Tesorería%';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
