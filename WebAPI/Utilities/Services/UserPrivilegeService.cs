using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebAPI.Data;
using WebAPI.Utilities.Options;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Utilities.Services;

public class UserPrivilegeService(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<ApplicationProperties> applicationProperties,
    ILogger<UserPrivilegeService> logger)
    : BackgroundService
{
    private readonly ApplicationProperties _applicationProperties = applicationProperties.Value;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;
    private readonly ILogger<UserPrivilegeService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await InnerTask(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task InnerTask(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.SetCommandTimeout(180);

        var moderatorRole = await dbContext.Roles
            .Where(e => e.Name == Roles.Moderator)
            .FirstAsync(stoppingToken);

        int repRequireForModerator = _applicationProperties.ReputationRequireForRole.Moderator;

        var query = from user in dbContext.Users
                    join userRoleRel in dbContext.UserRoles on user.Id equals userRoleRel.UserId
                    where user.Reputation >= repRequireForModerator
                       && !dbContext.UserRoles.Any(ur => ur.UserId == user.Id && ur.RoleId == moderatorRole.Id)
                    select user.Id;

        var ids = await query.ToListAsync(stoppingToken);

        dbContext.UserRoles.AddRange(ids.Select(e =>
        {
            return new IdentityUserRole<int>
            {
                RoleId = moderatorRole.Id,
                UserId = e
            };
        }));

        var res = await dbContext.SaveChangesAsync(stoppingToken);
        _logger.LogInformation("Updated {Res} user to moderator", res);
    }

}
