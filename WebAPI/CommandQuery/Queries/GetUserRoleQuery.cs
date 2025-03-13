using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetUserRoleQuery(int UserId) : IQuery<GenericResult<List<RoleResponse>>>;
