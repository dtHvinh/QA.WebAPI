using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using WebAPI.Data;
using WebAPI.Utilities.Options;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Utilities.Jobs;

public class GrantModeratorRoleJob(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<ApplicationProperties> applicationProperties,
    Serilog.ILogger logger)
{
    private readonly ApplicationProperties _applicationProperties = applicationProperties.Value;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly Serilog.ILogger _logger = logger;

    public async Task ExecuteJob()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.SetCommandTimeout(180);

        var moderatorRole = await dbContext.Roles
            .Where(e => e.Name == Roles.Moderator)
            .FirstAsync();

        int repRequireForModerator = _applicationProperties.ReputationRequireForRole.Moderator;

        var query = from user in dbContext.Users
                    join userRoleRel in dbContext.UserRoles on user.Id equals userRoleRel.UserId
                    where user.Reputation >= repRequireForModerator
                       && !dbContext.UserRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == moderatorRole.Id)
                    select user.Id;

        var ids = await query.ToListAsync();

        dbContext.UserRoles.AddRange(ids.Select(e =>
        {
            return new IdentityUserRole<int>
            {
                RoleId = moderatorRole.Id,
                UserId = e
            };
        }));

        dbContext.UserClaims.AddRange(ids.Select(e =>
        {
            return new IdentityUserClaim<int>
            {
                UserId = e,
                ClaimType = ClaimTypes.Role,
                ClaimValue = Roles.Moderator
            };
        }));

        var res = await dbContext.SaveChangesAsync();

        _logger.Information("Updated {Res} user to moderator", res);
    }

}
