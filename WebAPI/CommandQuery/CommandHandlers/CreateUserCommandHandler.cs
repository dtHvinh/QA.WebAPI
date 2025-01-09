using CQRS;
using WebAPI.CommandQuery.Commands;
using WebAPI.Dto;
using WebAPI.Repositories.Base;
using WebAPI.Utilities;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Provider;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateUserCommandHandler(IUserRepository userRepository,
                                      JwtTokenProvider tokenProvider)
    : ICommandHandler<CreateUserCommand, HandlerResult<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly JwtTokenProvider _tokenProvider = tokenProvider;

    public async Task<HandlerResult<AuthResponseDto>> Handle(
        CreateUserCommand request, CancellationToken cancellationToken)
    {
        var newUser = request.User.ToAppUser();

        var result = await _userRepository.AddUserAsync(newUser, request.User.Password, cancellationToken);

        if (!result.IsSuccess)
        {
            return HandlerResult<AuthResponseDto>.Failure(result.Message);
        }

        newUser = result.Value!;

        var jwtToken = await _tokenProvider.CreateTokenAsync(newUser);

        var refreshToken = _tokenProvider.CreateRefreshToken();

        return HandlerResult<AuthResponseDto>.Success(newUser.ToLoginResponseDto(jwtToken, refreshToken));
    }
}
