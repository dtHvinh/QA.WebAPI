using WebAPI.Utilities.Auto;
using WebAPI.Utilities.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddLogging();
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

//using var scope = app.Services.CreateScope();
//var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
//await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Admin));
//await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.User));

app.RegisterEndpoints();
await app.RunAsync();
