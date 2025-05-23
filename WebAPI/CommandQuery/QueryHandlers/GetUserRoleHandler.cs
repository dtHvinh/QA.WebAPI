using Microsoft.AspNetCore.Identity;
using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetUserRoleHandler(UserManager<ApplicationUser> userManager)
    : IQueryHandler<GetUserRoleQuery, GenericResult<List<RoleResponse>>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public async Task<GenericResult<List<RoleResponse>>> Handle(GetUserRoleQuery request, CancellationToken cancellationToken)
    {
        var roles = await _userManager.GetRolesAsync(new ApplicationUser { Id = request.UserId });

        return GenericResult<List<RoleResponse>>.Success(roles.Select(r => new RoleResponse(r)).ToList());
    }
}
