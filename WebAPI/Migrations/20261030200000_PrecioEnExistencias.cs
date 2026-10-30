using Microsoft.EntityFrameworkCore.Migrations;
using WebAPI.Helpers;

#nullable disable

namespace WebAPI.Migrations
{
    /// <summary>
    /// sp_Inventario_Existencias también devuelve el precio de venta del producto.
    /// Vuelve a ejecutar todos los scripts (CREATE OR ALTER). No cambia tablas.
    /// </summary>
    public partial class PrecioEnExistencias : Migration
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
