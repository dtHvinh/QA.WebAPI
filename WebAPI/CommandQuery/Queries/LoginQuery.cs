using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Response.AuthResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record LoginQuery(LoginDto Dto) : IQuery<GenericResult<AuthResponse>>;
