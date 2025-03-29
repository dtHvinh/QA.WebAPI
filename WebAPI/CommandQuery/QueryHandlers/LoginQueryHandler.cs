using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Response.AuthResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class LoginQueryHandler(
    IAuthenticationService authenticationService,
    Serilog.ILogger logger)
    : IQueryHandler<LoginQuery, GenericResult<AuthResponse>>
{
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<AuthResponse>> Handle(LoginQuery request, CancellationToken cancellationToken)
    {


        var authResult = await _authenticationService.LoginAsync(request.Dto.Email,
                                                                 request.Dto.Password,
                                                                 cancellationToken);

        if (authResult.IsSuccess)
            _logger.Information("User {Email} logged in.", request.Dto.Email);

        return authResult;
    }
}
