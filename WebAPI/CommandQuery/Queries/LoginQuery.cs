using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record LoginQuery(LoginDto Dto) : IQuery<OperationResult<AuthResponse>>;
