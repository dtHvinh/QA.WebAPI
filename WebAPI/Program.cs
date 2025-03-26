using Hangfire;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;
using WebAPI.Middleware;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Jobs;
using WebAPI.Utilities.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
    options.ValidateOnBuild = true;
});

builder.Services.AddEndpointsApiExplorer()
                .AddOpenApi()
                .AddHttpContextAccessor();

builder.Services.WithConfiguration(builder.Configuration)
                .ConfigureLogging()
                .ConfigureApplicationOptions()
                .ConfigureDatabase()
                .ConfigureAuthentication()
                .ConfigureAuthorization()
                .ConfigureDependencies();

builder.Services.AddHangfire(configuration =>
{
    configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseSerilogLogProvider()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowOrigins",
                      policy =>
                      {
                          policy.WithOrigins(
                              "https://localhost:7051",
                              "http://localhost:7051",
                              "https://qa-web-mu.vercel.app",
                              "https://qa-web-dthvinhs-projects.vercel.app",
                              "https://qa-2vvl99o65-dthvinhs-projects.vercel.app");
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                      });
});

builder.Services.AddMediatR(
    configuration => configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.ConfigureFullTextSearch();

app.UseSerilogRequestLogging();

app.UseHangfireDashboard();

app.UseCors("AllowOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.RegisterEndpoints();
app.UseMiddleware<BanMiddleware>();


RecurringJob.AddOrUpdate<GrantModeratorRoleJob>(nameof(GrantModeratorRoleJob), (e) => e.ExecuteJob(), Cron.Hourly);

await app.RunAsync();