using WebAPI.Repositories;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;

namespace WebAPI.Filters.Requirement.Base;

public class ReputationRequirement(UserRepository userRepository,
                                   AuthenticationContext authContext,
                                   int minRequirement) : IEndpointFilter
{
    private readonly UserRepository _userRepository = userRepository;
    private readonly AuthenticationContext _authContext = authContext;
    private readonly int _minRequirement = minRequirement;

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var findUser = await _userRepository.FindByIdAsync(_authContext.UserId);

        if (!findUser.IsSuccess)
        {
            return ProblemResultExtensions.BadRequest("User not found");
        }

        var user = findUser.Value!;

        if (user.Reputation < _minRequirement)
        {
            string message = $"You need at least {_minRequirement} reputation to do that";
            return ProblemResultExtensions.Forbidden(message);
        }

        return await next(context);
    }
}
