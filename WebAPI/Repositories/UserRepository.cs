using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Provider;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IUserRepository))]
public class UserRepository(ApplicationDbContext dbContext,
                            UserManager<ApplicationUser> userManager,
                            ImageProvider imageProvider,
                            ICacheService cache)
    : RepositoryBase<ApplicationUser>(dbContext), IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ImageProvider _imgProv = imageProvider;
    private readonly ICacheService _cache = cache;

    public async Task<int> GetReputation(int id, CancellationToken cancellationToken = default)
    {
        var user = await Table.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        return user is null ? 0 : user.Reputation;

    }
    public async Task<GenericResult<ApplicationUser>> AddUserAsync(ApplicationUser user, string password, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return GenericResult<ApplicationUser>.Failure(result.Errors.Select(e => e.Description).FirstOrDefault()
                    ?? "Error");
            }

            // Add to role
            await _userManager.AddToRoleAsync(user, Roles.User);

            // Add Claim
            await _userManager.AddClaimsAsync(user,
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, Roles.User),
            ]);

            // Cache the user's email for fast look up
            await _cache.SetUsedEmail(user.Email!, cancellationToken);

            return GenericResult<ApplicationUser>.Success(user);
        }
        catch (Exception ex)
        {
            return GenericResult<ApplicationUser>.Failure(ex.Message);
        }
    }

    public async Task<ApplicationUser?> FindByEmail(string email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || user.IsDeleted)
            return null;

        return user;
    }

    public async Task<ApplicationUser?> FindByUsername(string username, CancellationToken cancellationToken = default)
        => await Table.Where(e => e.UserName!.Equals(username)).FirstOrDefaultAsync(cancellationToken);

    public async Task<ApplicationUser?> FindUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null || user.IsDeleted)
            return null;

        return user;
    }

    public async Task<ApplicationUser?> FindUserWithLinks(int id, CancellationToken cancellationToken = default)
    {
        return await Table.Where(e => e.Id == id).Include(e => e.ExternalLinks).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task ChangeReputationAsync(int id, int amount, CancellationToken cancellationToken = default)
    {
        var user = await Entities.FirstAsync(e => e.Id.Equals(id), cancellationToken);
        if (user is null)
        {
            return;
        }

        user.Reputation += amount;
        await _userManager.UpdateAsync(user);
    }
}
