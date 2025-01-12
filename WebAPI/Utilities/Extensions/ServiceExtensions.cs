using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Utilities.Auto;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Options;
using WebAPI.Utilities.Provider;
using WebAPI.Utilities.Services;

namespace WebAPI.Utilities.Extensions;

public static class ServiceExtensions
{
    public static IConfiguration Configuration { get; set; } = default!;

    public static IServiceCollection WithConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        Configuration = configuration;
        return services;
    }

    public static IServiceCollection ConfigureDatabase(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(Configuration.GetConnectionString("SqlServer"));
        });

        services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = Configuration.GetConnectionString("Redis");
            options.InstanceName = "QA_";
        });

        return services;
    }

    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services)
    {
        var secretKey = Configuration["JwtOptions:SecretKey"]
            ?? throw new ArgumentException("Secret key not found!");
        var aud = Configuration["JwtOptions:Aud"]?.Split(',');
        var iss = Configuration["JwtOptions:Iss"]?.Split(',');

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new()
                    {
                        ValidateAudience = true,
                        ValidAudiences = aud,
                        ValidateIssuer = true,
                        ValidIssuers = iss,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ClockSkew = TimeSpan.Zero
                    };

                    // Handling connection validation from SignalR hub
                    //options.Events = new()
                    //{
                    //    OnMessageReceived = context =>
                    //    {
                    //        try
                    //        {
                    //            var accessToken = context.Request.Query["access_token"];

                    //            var path = context.HttpContext.Request.Path;
                    //            if (!string.IsNullOrEmpty(accessToken) &&
                    //                path.StartsWithSegments(AppConstants.Hub.Notification))
                    //            {
                    //                context.Token = accessToken;
                    //            }
                    //            return Task.CompletedTask;
                    //        }
                    //        catch
                    //        {
                    //            throw new InvalidOperationException("No \'access_token\' query found");
                    //        }
                    //    }
                    //};
                });

        return services;
    }

    public static IServiceCollection ConfigureDependencies(this IServiceCollection services)
    {
        services.RegisterRepositories();
        services.AddScoped<JwtTokenProvider>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IValidationRuleProvider, ValidationRuleProvider>();
        services.AddTransient<AuthentcationContext>();

        services.AddSingleton(new ImageProvider(
            Configuration["ImageProvider:DefaultProfileImage"]
            ?? throw new InvalidOperationException("Provider not found")));

        services.AddValidatorsFromAssembly(typeof(ServiceExtensions).Assembly);

        return services;
    }

    public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddAuthorizationBuilder()
            .AddPolicy(
                name: PolicyProvider.RequireNameIdClaim,
                policy: PolicyProvider.Get(PolicyProvider.RequireNameIdClaim))
            .AddPolicy(
                name: PolicyProvider.RequireAdminRole,
                policy: PolicyProvider.Get(PolicyProvider.RequireAdminRole))
            .AddPolicy(
                name: PolicyProvider.RequireNameIdClaimAndRole,
                policy: PolicyProvider.Get(PolicyProvider.RequireNameIdClaimAndRole));

        return services;
    }

    public static IServiceCollection ConfigureApplicationOptions(this IServiceCollection services)
    {
        services.Configure<JwtOptions>(
            Configuration.GetSection("JwtOptions"));

        return services;
    }
}
