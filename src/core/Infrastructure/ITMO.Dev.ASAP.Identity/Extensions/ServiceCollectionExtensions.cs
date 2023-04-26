using ITMO.Dev.ASAP.Application.Abstractions.Identity;
using ITMO.Dev.ASAP.Identity.Entities;
using ITMO.Dev.ASAP.Identity.Services;
using ITMO.Dev.ASAP.Identity.Tools;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ITMO.Dev.ASAP.Identity.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddIdentityConfiguration(
        this IServiceCollection collection,
        IConfigurationSection identityConfigurationSection,
        Action<DbContextOptionsBuilder> dbContextAction)
    {
        IdentityConfiguration? identityConfiguration = identityConfigurationSection
            .Get<IdentityConfiguration>();

        IConfigurationSection identityOptionsSection = identityConfigurationSection.GetSection("Options");
        collection.Configure<IdentityOptions>(identityOptionsSection);

        collection.AddScoped<IAuthorizationService, AuthorizationService>();

        if (identityConfiguration is not null)
            collection.AddSingleton(identityConfiguration);

        collection.AddDbContext<AsapIdentityContext>(dbContextAction);

        collection.AddIdentity<AsapIdentityUser, AsapIdentityRole>()
            .AddEntityFrameworkStores<AsapIdentityContext>()
            .AddDefaultTokenProviders();

        collection.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudience = identityConfiguration?.Audience,
                ValidIssuer = identityConfiguration?.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(identityConfiguration?.Secret ?? string.Empty)),
            };
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    if (context.Principal is null)
                        return Task.CompletedTask;

                    ICurrentUserManager userManager = context.HttpContext.RequestServices
                        .GetRequiredService<ICurrentUserManager>();

                    userManager.Authenticate(context.Principal);

                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    string? accessToken = context.Request.Query["access_token"];

                    if (string.IsNullOrEmpty(accessToken) is false)
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                },
            };
        });
    }
}