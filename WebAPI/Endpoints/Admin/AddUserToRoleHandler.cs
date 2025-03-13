using Microsoft.AspNetCore.Identity;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Endpoints.Admin;

public class AddUserToRoleHandler(UserManager<AppUser> userManager, Serilog.ILogger logger)
    : ICommandHandler<AddUserToRoleCommand, GenericResult<TextResponse>>
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(AddUserToRoleCommand request, CancellationToken cancellationToken)
    {
        var res = await _userManager.AddToRoleAsync(new AppUser { Id = request.UserId }, request.Role);

        if (res.Succeeded)
        {
            return GenericResult<TextResponse>.Success("User added to role");
        }

        _logger.Error("Failed to add user to role: {@Errors}", res.Errors);

        return GenericResult<TextResponse>.Failure("Failed to add user to role");
    }
}
