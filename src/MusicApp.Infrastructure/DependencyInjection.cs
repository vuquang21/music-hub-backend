using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MusicApp.Application.Common.Interfaces;
using MusicApp.Domain.Interfaces;
using MusicApp.Infrastructure.Auth;
using MusicApp.Infrastructure.Persistence;
using MusicApp.Infrastructure.Persistence.Repositories;
using MusicApp.Infrastructure.Services;

namespace MusicApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(config.GetConnectionString("Postgres")));

        services.AddScoped<ITrackRepository, TrackRepository>();
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<IPlaylistRepository, PlaylistRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IArtistRepository, ArtistRepository>();
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.Configure<JwtSettings>(config.GetSection("Jwt"));
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUserService>();

        services.AddSingleton<IStorageService, LocalStorageService>();

        var redisConnection = config.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection))
            services.AddStackExchangeRedisCache(opt => opt.Configuration = redisConnection);
        else
            services.AddDistributedMemoryCache();

        var jwtSettings = config.GetSection("Jwt").Get<JwtSettings>()!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true, ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true, ValidAudience = jwtSettings.Audience,
                ValidateIssuerSigningKey = true, IssuerSigningKey = key,
                ValidateLifetime = true, ClockSkew = TimeSpan.Zero,
            };

            opt.Events = new JwtBearerEvents
            {
                OnChallenge = async ctx =>
                {
                    ctx.HandleResponse();
                    ctx.Response.StatusCode = 401;
                    ctx.Response.ContentType = "application/json";
                    await ctx.Response.WriteAsJsonAsync(new { success = false, message = "Authentication required." });
                },
                OnForbidden = async ctx =>
                {
                    ctx.Response.StatusCode = 403;
                    ctx.Response.ContentType = "application/json";
                    await ctx.Response.WriteAsJsonAsync(new { success = false, message = "Access denied." });
                },
            };
        });

        services.AddAuthorization(opt =>
        {
            opt.AddPolicy("ArtistOrAdmin", p => p.RequireRole("Artist", "Admin"));
            opt.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
        });

        return services;
    }
}
