using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Data.Helpers;


/*using System.Text.Json;


public static class DbContextExtensions
{
    public static void Seedwork<T>(this Dsw2025Ej15Context context, string dataSource) where T : class
    {
        if (context.Set<T>().Any()) return;
        var json = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, dataSource));
        var entities = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        });
        if (entities == null || entities.Count == 0) return;
        context.Set<T>().AddRange(entities);
        context.SaveChanges();
    }
}
*/