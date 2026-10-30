using Microsoft.Data.SqlClient;

namespace WebAPI.Helpers
{
    // Convierte los errores que lanzan los SP con THROW en respuestas claras para el frontend:
    //   THROW 50404       -> 404 Not Found     { "mensaje": "..." }
    //   THROW 50000-50999 -> 400 Bad Request   { "mensaje": "..." }
    public static class ManejadorErroresSql
    {
        public static async Task Manejar(HttpContext context, Func<Task> next)
        {
            try
            {
                await next();
            }
            catch (Exception ex) when (BuscarSqlException(ex) is SqlException sql && ObtenerRespuesta(sql).status != 0)
            {
                var (status, mensaje) = ObtenerRespuesta(sql);
                context.Response.StatusCode = status;
                await context.Response.WriteAsJsonAsync(new { mensaje });
            }
        }

        private static (int status, string mensaje) ObtenerRespuesta(SqlException sql)
        {
            return sql.Number switch
            {
                50404 => (StatusCodes.Status404NotFound, sql.Message),
                >= 50000 and < 51000 => (StatusCodes.Status400BadRequest, sql.Message),
                2627 or 2601 => (StatusCodes.Status400BadRequest, "Ya existe un registro con ese valor"),
                547 => (StatusCodes.Status400BadRequest, "El registro está relacionado con datos que no existen"),
                // El SP no existe en la base de datos: falta aplicar una migración
                2812 => (StatusCodes.Status500InternalServerError,
                    $"{sql.Message} Ejecute Update-Database en la Consola del Administrador de paquetes."),
                _ => (0, string.Empty)
            };
        }

        // El SqlException puede venir dentro de otra excepción de Entity Framework
        private static SqlException? BuscarSqlException(Exception? ex)
        {
            while (ex != null)
            {
                if (ex is SqlException sql)
                {
                    return sql;
                }
                ex = ex.InnerException;
            }
            return null;
        }
    }
}
