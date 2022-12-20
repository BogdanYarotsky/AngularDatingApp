using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public static class StartExtensions
{
    public static IServiceCollection AddAll(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("Default");
        services.AddDbContext<DataContext>(o => o.UseSqlite(connectionString));

        services.AddIdentityServices(config["TokenKey"]);
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<KeyedHashAlgorithm, HMACSHA512>();
        services.AddControllers();
        services.AddCors();
        return services;
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
        app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        return app;
    }
}