using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace WebAPI.Helpers
{
    // Forma única de llamar a los procedimientos almacenados (SP) desde los controladores
    public static class SpHelper
    {
        // Crea un parámetro. Los null se envían como NULL de SQL
        public static SqlParameter Parametro(string nombre, object? valor)
        {
            return new SqlParameter(nombre, valor ?? DBNull.Value);
        }

        // Convierte una lista a texto JSON para los SP que la leen con OPENJSON
        public static string Json(object valor)
        {
            return JsonSerializer.Serialize(valor);
        }

        // SP que devuelve filas -> lista de T (T necesita una propiedad por cada columna del SELECT)
        public static Task<List<T>> ConsultarAsync<T>(this DatabaseFacade database, string sp, params SqlParameter[] parametros)
        {
            return database.SqlQueryRaw<T>(Comando(sp, parametros), parametros).ToListAsync();
        }

        // SP que devuelve filas de una tabla del DbContext (ej: Categoria, Almacen)
        public static Task<List<T>> ConsultarAsync<T>(this DbSet<T> tabla, string sp, params SqlParameter[] parametros) where T : class
        {
            return tabla.FromSqlRaw(Comando(sp, parametros), parametros).AsNoTracking().ToListAsync();
        }

        // SP que termina con "SELECT @ID AS Value": devuelve el ID del registro creado
        public static async Task<int> ObtenerIdAsync(this DatabaseFacade database, string sp, params SqlParameter[] parametros)
        {
            var resultado = await database.SqlQueryRaw<int>(Comando(sp, parametros), parametros).ToListAsync();
            return resultado.First();
        }

        // SP que no devuelve filas (actualizar, eliminar, anular...)
        public static Task<int> EjecutarAsync(this DatabaseFacade database, string sp, params SqlParameter[] parametros)
        {
            return database.ExecuteSqlRawAsync(Comando(sp, parametros), parametros);
        }

        // Arma el texto "EXEC sp_Nombre @A = @A, @B = @B" (parámetros por nombre: el orden no importa)
        private static string Comando(string sp, SqlParameter[] parametros)
        {
            if (parametros.Length == 0)
            {
                return $"EXEC {sp}";
            }

            return $"EXEC {sp} " + string.Join(", ", parametros.Select(p => $"{p.ParameterName} = {p.ParameterName}"));
        }
    }
}
