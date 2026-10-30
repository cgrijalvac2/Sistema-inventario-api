using Microsoft.EntityFrameworkCore.Migrations;
using WebAPI.Helpers;

#nullable disable

namespace WebAPI.Migrations
{
    /// <summary>
    /// Agrega los SP del mantenimiento Rol - Permiso (10_RolPermiso.sql) y actualiza sp_Rol_Actualizar.
    /// Vuelve a ejecutar todos los scripts: como usan CREATE OR ALTER, los que ya existían solo se actualizan.
    /// No cambia tablas.
    /// </summary>
    public partial class RolPermisoSP : Migration
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
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_RolPermiso_Asignar;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_RolPermiso_Quitar;");
        }
    }
}
