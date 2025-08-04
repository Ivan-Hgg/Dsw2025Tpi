using Dsw2025Tpi.Api.DependencyInyection;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
        builder.Services.AddSwaggerGen(o =>
        {
            o.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Desarrollo de Software",
                Version = "v1",
            });
            o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Ingresar el token",
                Type = SecuritySchemeType.ApiKey
            });
            o.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
        });
        builder.Services.AddHealthChecks();
        // Se pasa la configuración requerida al método AddDomainServices  
        builder.Services.AddIdentityServices(builder.Configuration);
        builder.Services.AddDomainServices(builder.Configuration);
        builder.Services.AddJWTServices(builder.Configuration);

        var app = builder.Build();

        // Ejecuta migraciones y seed de datos al iniciar la app  
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<Dsw2025TpiContext>();
            dbContext.Database.Migrate(); // Aplica migraciones pendientes  
            dbContext.SeedDatabase();     // Carga los datos desde los JSON  

        }/*
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await DbContextExtensions.SeedRolesAsync(roleManager);
        }*/

        // Configure the HTTP request pipeline.  
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        //app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/healthcheck");

        app.Run();
    }
}

