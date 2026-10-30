using Microsoft.EntityFrameworkCore.Migrations;
using WebAPI.Helpers;

#nullable disable

namespace WebAPI.Migrations
{
    /// <summary>
    /// sp_Transaccion_Registrar recibe @DevuelveInventario: una nota de crédito mueve o no el inventario según su razón.
    /// sp_Transaccion_Anular solo revierte las líneas que sí movieron inventario.
    /// Vuelve a ejecutar todos los scripts (CREATE OR ALTER). No cambia tablas.
    /// </summary>
    public partial class NotaCreditoSegunRazon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var lote in ScriptsSql.LeerLotes())
            {
                migrationBuilder.Sql(lote);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Para regresar basta con ejecutar los scripts de la versión anterior del proyecto
        }
    }
}
