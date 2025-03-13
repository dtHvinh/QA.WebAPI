using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class BanCommandHandler(
    ICacheService cacheService,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<BanCommand, GenericResult<TextResponse>>
{
    private readonly ICacheService _cacheService = cacheService;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(BanCommand request, CancellationToken cancellationToken)
    {
        if (_authenticationContext.UserId == request.UserId)
            return GenericResult<TextResponse>.Failure("You can't ban yourself");

        if (!await _authenticationContext.IsAdmin())
            return GenericResult<TextResponse>.Failure(string.Format(EM.ROLE_NOT_MEET_REQ, Roles.Admin));

        var banDueDate = await _cacheService.IsBanned(request.UserId, cancellationToken);

        if (banDueDate.HasValue)
        {
            return GenericResult<TextResponse>.Failure("User is already banned");
        }

        banDueDate =
            DateTime.UtcNow.AddDays(request.Ban.Days)
            .AddHours(request.Ban.Hours)
            .AddMinutes(request.Ban.Minutes);

        var result = await _cacheService.BanUserAsync(request.UserId, banDueDate.Value, request.Ban.Reason, cancellationToken);

        _logger.Information("User #{UserId} was banned by Admin #{AdminId} until {BanDueDate} with reason: {BanReason}", request.UserId, _authenticationContext.UserId, banDueDate, request.Ban.Reason);

        return result
            ? GenericResult<TextResponse>.Success("User is banned")
            : GenericResult<TextResponse>.Failure("Failed to ban user");
    }
}
