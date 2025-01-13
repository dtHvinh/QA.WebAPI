using WebAPI.Data;
using WebAPI.Utilities.Auto;
using WebAPI.Utilities.Extensions;

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

builder.Services.AddMediatR(
    configuration => configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

await ApplicationDbContext.Init(app, false);

app.RegisterEndpoints();
await app.RunAsync();
