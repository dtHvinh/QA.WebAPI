using CQRS;
using WebAPI.Dto;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record LoginQuery(LoginDto Dto) : IQuery<ResultBase<AuthResponseDto>>;
