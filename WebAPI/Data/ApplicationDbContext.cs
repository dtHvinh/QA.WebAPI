using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;
using WebAPI.Model;
using WebAPI.Utilities;
using WebAPI.Utilities.Contract;

namespace WebAPI.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var entity in GetEntities())
        {
            builder.Entity(entity);
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    private static IEnumerable<Type> GetEntities()
    {
        return typeof(ApplicationDbContext).Assembly.GetTypes()
            .Where(type => type.IsClass && type.GetInterfaces()
                .Any(i => i == typeof(IKeylessEntityWithTime)
                || (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityWithTime<>))
                || (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntity<>))));
    }

    public static async Task Init(WebApplication app, bool run)
    {
        if (!run)
            return;

        using var scope = app.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        await roleManager.CreateAsync(new IdentityRole<Guid>(WebAPI.Utilities.Constants.Roles.Admin));
        await roleManager.CreateAsync(new IdentityRole<Guid>(WebAPI.Utilities.Constants.Roles.User));

        var tagRepository = scope.ServiceProvider.GetRequiredService<WebAPI.Repositories.Base.ITagRepository>();

        tagRepository.CreateTags(Enum.GetNames<Tags>().Select(e => new Tag
        {
            Name = e,
            Description = e
        }).ToList());

        var user = new AppUser()
        {
            Email = "admin@email.com",
            UserName = "admin@email.com",
            Reputation = 99999999,
            ProfilePicture = "avc"
        };
        await userManager.CreateAsync(user, "0123456789");
        await userManager.AddClaimsAsync(user,
        [
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, Constants.Roles.Admin),
        ]);
        await userManager.AddToRoleAsync(user, Constants.Roles.Admin);
    }
}


