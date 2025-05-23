using Microsoft.AspNetCore.Identity;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class AddUserToRoleHandler(UserManager<ApplicationUser> userManager, AuthenticationContext authenticationContext, Serilog.ILogger logger)
    : ICommandHandler<AddUserToRoleCommand, GenericResult<TextResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(AddUserToRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
        {
            return GenericResult<TextResponse>.Failure("User not found");
        }

        if (_authenticationContext.UserId == request.UserId)
        {
            return GenericResult<TextResponse>.Failure("Cannot add yourself to a role");
        }

        var res = await _userManager.AddToRoleAsync(user, request.Role);

        _logger.UserAction(res.Succeeded
            ? Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error, _authenticationContext.UserId, LogOp.GrantRole, user);

        if (res.Succeeded)
            return GenericResult<TextResponse>.Success("User added to role");
        else
            return GenericResult<TextResponse>.Failure("Failed to add user to role");
    }
}
