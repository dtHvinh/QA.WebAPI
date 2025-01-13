using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class LoginQueryHandler(IAuthenticationService authenticationService)
    : IQueryHandler<LoginQuery, OperationResult<AuthResponse>>
{
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<OperationResult<AuthResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var authResult = await _authenticationService.LoginAsync(request.Dto.Email,
                                                                 request.Dto.Password,
                                                                 cancellationToken);

        return authResult;
    }
}
