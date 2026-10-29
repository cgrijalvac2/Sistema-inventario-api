using Microsoft.EntityFrameworkCore.Migrations;
using WebAPI.Helpers;

#nullable disable

namespace WebAPI.Migrations
{
    /// <summary>
    /// Crea (o actualiza) todos los procedimientos almacenados de Database/StoredProcedures.
    /// No cambia tablas. Usa CREATE OR ALTER, así que se puede volver a ejecutar sin problema.
    /// Si después se modifica un SP: Add-Migration NombreCambio, copiar el contenido de Up() de esta
    /// migración en la nueva, y Update-Database.
    /// </summary>
    public partial class StoredProcedures : Migration
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
            // Elimina todos los procedimientos que empiezan con sp_
            migrationBuilder.Sql(@"
                DECLARE @sql NVARCHAR(MAX) = N'';
                SELECT @sql += N'DROP PROCEDURE ' + QUOTENAME(name) + N';'
                  FROM sys.procedures
                 WHERE name LIKE N'sp[_]%';
                EXEC sp_executesql @sql;");
        }
    }
}
