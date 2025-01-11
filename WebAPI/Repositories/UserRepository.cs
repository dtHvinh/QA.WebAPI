using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Claims;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Attributes;
using WebAPI.Utilities.Provider;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Repositories;

[RepositoryImplOf(Type = typeof(IUserRepository))]
public class UserRepository(ApplicationDbContext dbContext,
                            UserManager<AppUser> userManager,
                            ImageProvider imageProvider,
                            IDistributedCache cache)
    : RepositoryBase<AppUser>(dbContext), IUserRepository
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly ImageProvider _imgProv = imageProvider;
    private readonly IDistributedCache _cache = cache;

    /// <summary>
    /// Add a new user to the database.
    /// 
    /// </summary>
    /// 
    /// <remarks>
    /// 
    /// <para>
    /// Set the user's role to <see cref="Roles.User"/> and add the user's claims.
    /// </para>
    /// </remarks>
    public async Task<ResultBase<AppUser>> AddUserAsync(AppUser user, string password, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Set default image
            user.ProfilePicture = _imgProv.GetDefaultPFP($"{user.Email!.First()}");

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return ResultBase<AppUser>.Failure(
                    string.Join(',', result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, Roles.User);

            await _userManager.AddClaimsAsync(user,
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, Roles.User),
            ]);

            // Cache the user's email for fast look up
            await _cache.SetStringAsync(RedisKeyGen.ForEmailDuplicate(user.Email!), "", cancellationToken);

            return ResultBase<AppUser>.Success(user);
        }
        catch (Exception ex)
        {
            return ResultBase<AppUser>.Failure(ex.Message);
        }
    }

    public async Task<ResultBase<AppUser>> FindByEmail(string email, CancellationToken cancellationToken)
    {
        return await FindFirstAsync(e => e.Email!.Equals(email), cancellationToken);
    }
}
