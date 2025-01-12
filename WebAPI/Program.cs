using Microsoft.AspNetCore.Identity;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Auto;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

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

await Init(false);

async Task Init(bool run)
{
    if (!run)
        return;

    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.Admin));
    await roleManager.CreateAsync(new IdentityRole<Guid>(Roles.User));

    var tagRepository = scope.ServiceProvider.GetRequiredService<ITagRepository>();

    await tagRepository.AddTagsAsync(Enum.GetNames<Tags>().Select(e => new Tag { Name = e }).ToList());
}


app.RegisterEndpoints();
await app.RunAsync();
