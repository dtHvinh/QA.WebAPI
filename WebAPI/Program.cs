using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer()
                .AddLogging()
                .AddHttpContextAccessor();


builder.Services.WithConfiguration(builder.Configuration)
                .ConfigureApplicationOptions()
                .ConfigureDatabase()
                .ConfigureAuthentication()
                .ConfigureAuthorization()
                .ConfigureDependencies();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowOrigins",
                      policy =>
                      {
                          policy.WithOrigins(
                              "https://localhost:7051",
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

app.UseCors("AllowOrigins");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.RegisterEndpoints();

await app.RunAsync();