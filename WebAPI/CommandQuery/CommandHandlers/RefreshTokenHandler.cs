using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.AuthResponses;
using WebAPI.Utilities.Provider;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class RefreshTokenHandler(IUserRepository userRepository, JwtTokenProvider jwtTokenProvider) : ICommandHandler<RefreshTokenCommand, GenericResult<AuthRefreshResponse>>
{
    private readonly IUserRepository userRepository = userRepository;
    private readonly JwtTokenProvider jwtTokenProvider = jwtTokenProvider;

    public async Task<GenericResult<AuthRefreshResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var userId = jwtTokenProvider.GetIdFromToken(request.Dto.AccessToken);
        var user = await userRepository.FindUserByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return GenericResult<AuthRefreshResponse>.Failure("User not found");
        }

        var result = await jwtTokenProvider.CreateTokenAndUpdateAsync(user, request.Dto.RefreshToken);
        if (!result.IsSuccess)
        {
            return GenericResult<AuthRefreshResponse>.Failure(string.Join(',', result.Errors!));
        }

        return GenericResult<AuthRefreshResponse>.Success(new(result.AccessToken!, result.RefreshToken!));
    }
}
