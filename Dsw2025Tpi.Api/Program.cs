using Dsw2025Tpi.Api.DependencyInyection;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configura el DbContext (ajusta el proveedor y la cadena de conexión según tu entorno)  
        // Add services to the container.  
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHealthChecks();
        // Se pasa la configuración requerida al método AddDomainServices  
        builder.Services.AddDomainServices(builder.Configuration);

        var app = builder.Build();

        // Ejecuta migraciones y seed de datos al iniciar la app  
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<Dsw2025TpiContext>();
            dbContext.Database.Migrate(); // Aplica migraciones pendientes  
            dbContext.SeedDatabase();     // Carga los datos desde los JSON  
        }

        // Configure the HTTP request pipeline.  
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/healthcheck");

        app.Run();
    }
}

