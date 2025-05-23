using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebAPI.Model;
using WebAPI.Utilities.Contract;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Utilities.Context;

public class AuthenticationContext(IHttpContextAccessor hca, UserManager<ApplicationUser> userManager)
{
    private readonly HttpContext? _httpContext = hca.HttpContext;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public bool IsResourceOwnedByUser(IOwnedByUser<int> resource) => UserId == resource.AuthorId;

    public int UserId => int.Parse(_httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)!.Value!);
    public string Role => _httpContext?.User?.FindFirst(ClaimTypes.Role)!.Value!;

    public async Task<bool> IsAdmin() => await _userManager.IsInRoleAsync(new() { Id = UserId, }, Roles.Admin);
    public async Task<bool> IsUser() => await _userManager.IsInRoleAsync(new() { Id = UserId, }, Roles.User);
    public async Task<bool> IsModerator() => await _userManager.IsInRoleAsync(new() { Id = UserId, }, Roles.Moderator);
}
