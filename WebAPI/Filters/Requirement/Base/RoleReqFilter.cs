using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Filters.Requirement.Base;

public class RoleReqFilter(string role, AuthenticationContext authenticationContext)
    : IEndpointFilter
{
    private readonly string _role = role;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        switch (_role)
        {
            case Roles.Admin:
                if (await _authenticationContext.IsAdmin())
                {
                    return await next(context);
                }
                break;
            case Roles.Moderator:
                if (await _authenticationContext.IsModerator())
                {
                    return await next(context);
                }
                break;
            case Roles.User:
                if (await _authenticationContext.IsModerator())
                {
                    return await next(context);
                }
                break;
            default:
                string message = "Role not recognized";
                return ProblemResultExtensions.Forbidden(message);
        }

        return await next(context);
    }
}
