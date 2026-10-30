using Microsoft.EntityFrameworkCore.Migrations;
using WebAPI.Helpers;

#nullable disable

namespace WebAPI.Migrations
{
    /// <summary>
    /// Productos por almacén con existencia: sp_InventarioAlmacen_Asignar recibe la cantidad actual y
    /// sp_InventarioAlmacen_Actualizar ajusta la existencia por conteo físico (ambos generan ajustes en el kardex).
    /// Vuelve a ejecutar todos los scripts (CREATE OR ALTER). No cambia tablas.
    /// </summary>
    public partial class ExistenciaInicial : Migration
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
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_InventarioAlmacen_Actualizar;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_MotivoAjuste_Buscar;");
        }
    }
}
