using WebAPI.Repositories.Base;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Extensions;

namespace WebAPI.Filters.Requirement.Base;

public class ReputationRequirementFilter(IUserRepository userRepository,
                                   AuthenticationContext authContext,
                                   ICacheService cache,
                                   int minRequirement) : IEndpointFilter
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthenticationContext _authContext = authContext;
    private readonly ICacheService _cache = cache;
    private readonly int _minRequirement = minRequirement;

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var user = await _cache.GetAppUserAsync(_authContext.UserId);

        if (user is null)
        {
            var findUser = await _userRepository.FindByIdAsync(_authContext.UserId);

            if (!findUser.IsSuccess)
            {
                return ProblemResultExtensions.BadRequest("User not found");
            }

            user = findUser.Value!;

            await _cache.UpdateAppUserAsync(user);
        }

        if (user.Reputation < _minRequirement)
        {
            string message = $"You need at least {_minRequirement} reputation to do that";
            return ProblemResultExtensions.Forbidden(message);
        }

        return await next(context);
    }
}
