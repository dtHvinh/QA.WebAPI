using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response.AppUserResponses;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetUserHandler(
    IUserRepository userRepository, AuthenticationContext authenticationContext, ICacheService cacheService)
    : IQueryHandler<GetUserQuery, GenericResult<UserResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly AuthenticationContext _authContext = authenticationContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<GenericResult<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        AppUser? user = null;

        if (!request.NoCache)
        {
            user = await _cacheService.GetAppUserAsync(_authContext.UserId);
        }

        if (user is null)
        {
            user = await _userRepository.FindUserByIdAsync(_authContext.UserId, cancellationToken);

            ArgumentNullException.ThrowIfNull(user);

            await _cacheService.SetAppUserAsync(user);
        }

        return user == null
            ? GenericResult<UserResponse>.Failure(string.Format(EM.USER_ID_NOTFOUND, _authContext.UserId))
            : GenericResult<UserResponse>.Success(user.ToUserResponse());
    }
}
