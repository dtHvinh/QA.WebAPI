using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.AuthResponses;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Provider;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateUserHandler(IUserRepository userRepository,
                               JwtTokenProvider tokenProvider)
    : ICommandHandler<CreateUserCommand, GenericResult<AuthResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly JwtTokenProvider _tokenProvider = tokenProvider;

    public async Task<GenericResult<AuthResponse>> Handle(
        CreateUserCommand request, CancellationToken cancellationToken)
    {
        var newUser = request.User.ToAppUser();

        var result = await _userRepository.AddUserAsync(newUser, request.User.Password, cancellationToken);

        if (!result.IsSuccess)
        {
            return GenericResult<AuthResponse>
                .Failure(result.Message);
        }

        newUser = result.Value!;

        var jwtToken = await _tokenProvider.CreateTokenAsync(newUser);
        var refreshToken = _tokenProvider.CreateRefreshToken();

        return GenericResult<AuthResponse>
            .Success(newUser.ToAuthResponseDto(jwtToken, refreshToken, "User"));
    }
}
