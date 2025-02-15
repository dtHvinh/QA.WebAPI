using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Serialization;
using Elastic.Transport;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Options;
using WebAPI.Utilities.Provider;
using WebAPI.Utilities.Reflection;
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

        services.AddIdentity<AppUser, IdentityRole<int>>(options =>
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
                });

        return services;
    }

    public static IServiceCollection ConfigureDependencies(this IServiceCollection services)
    {
        services.RegisterRepositories();
        services.AutoRegisterAllServices();
        services.AddScoped<JwtTokenProvider>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IValidationRuleProvider, ValidationRuleProvider>();
        services.AddSingleton(e =>
        {
            return new JsonSerializerOptions()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
            };
        });
        services.AddSingleton(e =>
        {
            var nodePool = new SingleNodePool(new Uri(Configuration["ElasticSearch:Uri"]!));

            var ess =
            new ElasticsearchClientSettings(
                nodePool: nodePool,
                sourceSerializer: (defaultSerializer, settings) =>
                    new DefaultSourceSerializer(settings,
                    (e) => e.ReferenceHandler = ReferenceHandler.IgnoreCycles)
            )
            .CertificateFingerprint(Configuration["ElasticSearch:CertFingerprint"]!)
            .Authentication(new BasicAuthentication(
                Configuration["ElasticSearch:Username"]!, Configuration["ElasticSearch:Password"]!));

            return ess;
        });
        services.AddSingleton<QuestionSearchService>();
        services.AddTransient<AuthenticationContext>();
        services.AddSingleton(new ImageProvider(
            Configuration["ImageProvider:DefaultProfileImage"]
            ?? throw new InvalidOperationException("Provider not found")));

        services.AddSingleton(sp =>
        {
            var endpoint = new Uri(Configuration["OpenAI:Endpoint"]
                ?? throw new InvalidOperationException("Endpoint not found"));
            var model = Configuration["OpenAI:Model"]
                ?? throw new InvalidOperationException("Model not found");

            return new AIService(endpoint, model);
        });

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

        services.Configure<ApplicationProperties>(Configuration.GetSection("ApplicationProperties"));

        services.Configure<CacheOptions>(Configuration.GetSection("CacheOptions"));

        return services;
    }
}
