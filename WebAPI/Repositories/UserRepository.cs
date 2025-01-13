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
                            UserManager<AppUser> userManager,
                            ImageProvider imageProvider,
                            ICacheService cache)
    : RepositoryBase<AppUser>(dbContext), IUserRepository
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly ImageProvider _imgProv = imageProvider;
    private readonly ICacheService _cache = cache;

    public async Task<OperationResult<AppUser>> AddUserAsync(AppUser user, string password, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Set default image
            user.ProfilePicture = _imgProv.GetDefaultPFP($"{user.Email!.First()}");

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return OperationResult<AppUser>.Failure(
                    string.Join(',', result.Errors.Select(e => e.Description)));
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
            await _cache.SetUsedEmail(user.Email!);

            return OperationResult<AppUser>.Success(user);
        }
        catch (Exception ex)
        {
            return OperationResult<AppUser>.Failure(ex.Message);
        }
    }

    public async Task<AppUser?> FindByEmail(string email, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByEmailAsync(email);
        return user;
    }

    public async Task<AppUser?> FindUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await _userManager.FindByIdAsync(id.ToString());
        return user;
    }

    public async Task ChangeReputationAsync(Guid id, int amount, CancellationToken cancellationToken = default)
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
