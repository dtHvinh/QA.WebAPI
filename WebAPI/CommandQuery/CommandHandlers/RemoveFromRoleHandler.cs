using Microsoft.AspNetCore.Identity;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.CommandHandlers;

public class RemoveFromRoleHandler(UserManager<AppUser> userManager, Serilog.ILogger logger, AuthenticationContext authenticationContext)
    : ICommandHandler<RemoveFromRoleCommand, GenericResult<TextResponse>>
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly Serilog.ILogger _logger = logger;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<TextResponse>> Handle(RemoveFromRoleCommand request, CancellationToken cancellationToken)
    {
        if (request.Role == Roles.User)
            return GenericResult<TextResponse>.Failure("Cannot remove user from user role");

        var user = await _userManager.FindByIdAsync(request.UserId.ToString());

        if (user == null)
        {
            return GenericResult<TextResponse>.Failure("User not found");
        }

        if (_authenticationContext.UserId == request.UserId)
            return GenericResult<TextResponse>.Failure("Cannot remove yourself from a role");

        var result = await _userManager.RemoveFromRoleAsync(user, request.Role);

        if (!result.Succeeded)
        {
            _logger.Error("Failed to remove user from role: {Role}", request.Role);
            return GenericResult<TextResponse>.Failure("Failed to remove user from role");
        }

        return GenericResult<TextResponse>.Success("Successfully removed user from role");
    }
}
