using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Response.AuthResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record RefreshTokenCommand(RefreshTokenRequestDto Dto) : ICommand<GenericResult<AuthRefreshResponse>>;
