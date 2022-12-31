using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Interfaces;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public static class StartExtensions
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;
        var services = builder.Services;
        var connectionString = config.GetConnectionString("Default");
        services.AddDbContext<DataContext>(o => o.UseSqlite(connectionString));
        services.AddIdentityServices(config["TokenKey"]);
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<KeyedHashAlgorithm, HMACSHA512>();
        services.AddControllers();
        services.AddCors();
        return builder;
    }

    private static IServiceCollection AddIdentityServices(this IServiceCollection services, string tokenKey)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(tokenKey)
                ),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
        return services;
    }

    public static WebApplication AddMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseCors(policy =>
        {
            policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("https://localhost:4200");
        });
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        return app;
    }
}