using Microsoft.EntityFrameworkCore.Migrations;
using WebAPI.Helpers;

#nullable disable

namespace WebAPI.Migrations
{
    /// <summary>
    /// Los SP de insertar y actualizar reciben @Estado (activo / inactivo) para que se pueda elegir
    /// desde el formulario al crear y al editar. Vuelve a ejecutar todos los scripts (CREATE OR ALTER). No cambia tablas.
    /// </summary>
    public partial class EstadoEnFormularios : Migration
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
            // Los SP anteriores aceptan los mismos parámetros sin @Estado; para regresar basta con
            // ejecutar los scripts de la versión anterior del proyecto
        }
    }
}
