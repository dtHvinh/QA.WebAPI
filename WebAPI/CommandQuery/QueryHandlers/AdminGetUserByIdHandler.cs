using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.AppUserResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class AdminGetUserByIdHandler(IUserRepository userRepository)
    : IQueryHandler<AdminGetUserByIdQuery, GenericResult<GetUserResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<GenericResult<GetUserResponse>> Handle(AdminGetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindUserByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return GenericResult<GetUserResponse>.Failure("User not found");
        }

        return GenericResult<GetUserResponse>.Success(user.ToGetUserResponse());
    }
}
