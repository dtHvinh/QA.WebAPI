using WebAPI.Repositories.Base;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;

namespace WebAPI.Filters.Requirement.Base;

public class ReputationRequirementFilter(IUserRepository userRepository,
                                   AuthenticationContext authContext,
                                   ICacheService cacheService,
                                   int minRequirement) : IEndpointFilter
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthenticationContext _authContext = authContext;
    private readonly ICacheService _cacheService = cacheService;
    private readonly int _minRequirement = minRequirement;

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {

        var user = await _userRepository.FindUserByIdAsync(_authContext.UserId);
        if (user is null)
        {
            return ProblemResultExtensions.NotFound("User not found");
        }

        if (user.Reputation < _minRequirement)
        {
            string message = $"You need at least {_minRequirement} reputation to do that";
            return ProblemResultExtensions.Forbidden(message);
        }

        return await next(context);
    }
}
