using System.Text.RegularExpressions;

namespace WebAPI.Helpers
{
    // Lee los archivos .sql de Database/StoredProcedures (van incrustados en el .dll, ver WebAPI.csproj)
    // y los separa en lotes por cada línea "GO", porque cada CREATE PROCEDURE debe ir en su propio lote
    public static class ScriptsSql
    {
        public static IEnumerable<string> LeerLotes()
        {
            var ensamblado = typeof(ScriptsSql).Assembly;

            var recursos = ensamblado.GetManifestResourceNames()
                .Where(nombre => nombre.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                .OrderBy(nombre => nombre, StringComparer.Ordinal);

            foreach (var recurso in recursos)
            {
                using var stream = ensamblado.GetManifestResourceStream(recurso)!;
                using var lector = new StreamReader(stream);
                var script = lector.ReadToEnd();

                foreach (var lote in Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(lote))
                    {
                        yield return lote.Trim();
                    }
                }
            }
        }
    }
}
