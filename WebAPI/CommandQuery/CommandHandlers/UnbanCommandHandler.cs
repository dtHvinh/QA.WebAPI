using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UnbanCommandHandler(ICacheService cacheService, Serilog.ILogger logger, AuthenticationContext authenticationContext)
    : ICommandHandler<UnbanCommand, GenericResult<TextResponse>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly Serilog.ILogger _logger = logger;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<TextResponse>> Handle(UnbanCommand request, CancellationToken cancellationToken)
    {
        if (!await _authenticationContext.IsAdmin())
            return GenericResult<TextResponse>.Failure(string.Format(EM.ROLE_NOT_MEET_REQ, Roles.Admin));

        await _cacheService.Unban(request.UserId, cancellationToken);

        _logger.Information("User #{UserId} was unbanned by Admin #{AdminId}", request.UserId, _authenticationContext.UserId);

        return GenericResult<TextResponse>.Success("User unbanned");
    }
}
