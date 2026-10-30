using Microsoft.EntityFrameworkCore.Migrations;
using WebAPI.Helpers;

#nullable disable

namespace WebAPI.Migrations
{
    /// <summary>
    /// Producto.Costo pasa a ser el costo de compra (no promedio): se puede editar en el producto y cada compra
    /// lo actualiza con el último costo pagado. Vuelve a ejecutar todos los scripts (CREATE OR ALTER). No cambia tablas.
    /// </summary>
    public partial class CostoDeCompra : Migration
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
            // Para regresar al costo promedio basta con ejecutar los scripts de la versión anterior del proyecto
        }
    }
}
