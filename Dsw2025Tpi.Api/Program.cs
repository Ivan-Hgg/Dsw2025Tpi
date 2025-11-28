using Dsw2025Tpi.Api.DependencyInyection;
using Dsw2025Tpi.Api.MiddleareCustoms;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace Dsw2025Tpi.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLogging(config => {
            config.AddConsole();
            config.AddEventLog();
        });

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

        //mostrar los enum como sus valores de string
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowVite", policy =>
                policy.WithOrigins("http://localhost:5173")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials());
        });


        var app = builder.Build();

        // Ejecuta migraciones y seed de datos al iniciar la app  
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<Dsw2025TpiContext>();
            dbContext.Database.Migrate(); // Aplica migraciones pendientes  
            dbContext.SeedDatabase();     // Carga los datos desde los JSON  
            var dbContextAuthenticate = scope.ServiceProvider.GetRequiredService<AuthenticateContext>();
            dbContextAuthenticate.Database.Migrate();

        }
        using (var scope = app.Services.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await DbContextExtensions.SeedRolesAsync(roleManager);
        }

        // Configure the HTTP request pipeline.  
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseMiddleware<ExceptionHandlerCustom>();

        app.UseHttpsRedirection();

        //app.UseRouting();
        app.UseCors("AllowVite");


        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/healthcheck");

        app.Run();
    }
}

