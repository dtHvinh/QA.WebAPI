using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Utilities;
using WebAPI.Utilities.Provider;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Repositories;

public class UserRepository(ApplicationDbContext dbContext,
                            UserManager<AppUser> userManager,
                            ImageProvider imageProvider)
    : RepositoryBase<AppUser>(dbContext), IUserRepository
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly ImageProvider _imgProv = imageProvider;

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
    public async Task<QueryResult<AppUser>> AddUserAsync(AppUser user, string password, CancellationToken cancellationToken)
    {
        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Set default image
            user.ProfilePicture = _imgProv.GetDefaultPFP($"{user.FirstName}+{user.LastName}");

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return QueryResult<AppUser>.Failure(
                    string.Join(',', result.Errors.Select(e => e.Description)));
            }

            await _userManager.AddToRoleAsync(user, Roles.User);

            await _userManager.AddClaimsAsync(user,
            [
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, Roles.User),
            ]);

            return QueryResult<AppUser>.Success(user);
        }
        catch (Exception ex)
        {
            return QueryResult<AppUser>.Failure(ex.Message);
        }
    }
}
