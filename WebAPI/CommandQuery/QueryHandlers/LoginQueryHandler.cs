using CQRS;
using WebAPI.CommandQuery.Queries;
using WebAPI.Dto;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class LoginQueryHandler(IAuthenticationService authenticationService)
    : IQueryHandler<LoginQuery, ResultBase<AuthResponseDto>>
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<ResultBase<AuthResponseDto>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var authResult = await _authenticationService.LoginAsync(request.Dto.Email,
                                                                 request.Dto.Password,
                                                                 cancellationToken);

        return authResult;
    }
}
