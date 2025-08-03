using Dsw2025Tpi.Domain.Interfaces;
using Dsw2025Tpi.Data.Repositories;
using Dsw2025Tpi.Application.Services;
using Dsw2025Tpi.Application.Interfaces;
using Dsw2025Tpi.Data;
using Dsw2025Tpi.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

namespace Dsw2025Tpi.Api.DependencyInyection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        // Repositorios
        services.AddScoped<IRepository, EfRepository>();

        // Servicios de aplicación
        services.AddScoped<IProductsManagementService, ProductsManagementService>();
        services.AddScoped<IOrdersManagementService, OrdersManagementService>();
        services.AddScoped<AuthenticateContext>();  //esto es para que se pueda inyectar el contexto de la base de datos en los servicios de aplicación
        services.AddScoped<Dsw2025TpiContext>();//esto es para que se pueda inyectar el contexto de la base de datos en los servicios de aplicación  
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthenticateService, AuthenticateService>();

        // Db Context
        services.AddDbContext<Dsw2025TpiContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Dsw2025TpiEntities")));
        services.AddDbContext<AuthenticateContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("Dsw2025TpiEntities"));
        });
        return services;
    }
    public static IServiceCollection AddJWTServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        var jwtConfig = configuration.GetSection("Jwt");
        var keyText = jwtConfig["Key"] ?? throw new ArgumentNullException("JWT Key");
        var key = Encoding.UTF8.GetBytes(keyText);
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtConfig["Issuer"],
                ValidAudience = jwtConfig["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        services.AddSingleton<JwtTokenService>();

        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddIdentity<IdentityUserExtension, IdentityRole>(options =>
        {
            options.Password = new PasswordOptions
            {
                RequiredLength = 8
            };

        })
        .AddEntityFrameworkStores<AuthenticateContext>()
        .AddDefaultTokenProviders();

        return services;
    }


}
