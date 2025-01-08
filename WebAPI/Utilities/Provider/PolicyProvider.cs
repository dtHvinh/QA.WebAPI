using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Utilities.Provider;

public static class PolicyProvider
{
    /// <summary>
    /// User that authenticated by using JWT and contain <see cref="ClaimTypes.NameIdentifier"/> claim.
    /// </summary>
    public const string RequireNameIdClaim = "RequireNameIdClaim";
    public const string RequireAdminRole = "RequireAdminRole";

    public static AuthorizationPolicy Get(string name)
    {
        return name switch
        {
            RequireNameIdClaim => new AuthorizationPolicyBuilder()
            .RequireClaim(ClaimTypes.NameIdentifier)
            .Build(),

            RequireAdminRole => new AuthorizationPolicyBuilder()
            .RequireClaim(ClaimTypes.Role, Roles.Admin)
            .Build(),

            _ => throw new InvalidOperationException($"Do not recognize policy \'{name}\'"),
        };
    }
}