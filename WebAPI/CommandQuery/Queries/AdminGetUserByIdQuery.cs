using WebAPI.CQRS;
using WebAPI.Response.AppUserResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record AdminGetUserByIdQuery(int UserId) : IQuery<GenericResult<GetUserResponse>>;
