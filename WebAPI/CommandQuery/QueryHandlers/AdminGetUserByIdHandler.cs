using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.AppUserResponses;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class AdminGetUserByIdHandler(
    IUserRepository userRepository,
    ICacheService cacheService)
    : IQueryHandler<AdminGetUserByIdQuery, GenericResult<GetUserResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<GenericResult<GetUserResponse>> Handle(AdminGetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return GenericResult<GetUserResponse>.Failure("User not found");
        }

        return GenericResult<GetUserResponse>.Success(user.ToGetUserResponse()
            .SetIsBanned(_cacheService.IsBanned(user.Id)));
    }
}
