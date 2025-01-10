using CQRS;
using WebAPI.CommandQuery.Commands;
using WebAPI.Dto;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Provider;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateUserCommandHandler(IUserRepository userRepository,
                                      JwtTokenProvider tokenProvider)
    : ICommandHandler<CreateUserCommand, ResultBase<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly JwtTokenProvider _tokenProvider = tokenProvider;

    public async Task<ResultBase<AuthResponseDto>> Handle(
        CreateUserCommand request, CancellationToken cancellationToken)
    {
        var newUser = request.User.ToAppUser();

        var result = await _userRepository.AddUserAsync(newUser, request.User.Password, cancellationToken);

        if (!result.IsSuccess)
        {
            return ResultBase<AuthResponseDto>
                .Failure(result.Message);
        }

        newUser = result.Value!;

        var jwtToken = await _tokenProvider.CreateTokenAsync(newUser);
        var refreshToken = _tokenProvider.CreateRefreshToken();

        return ResultBase<AuthResponseDto>
            .Success(newUser.ToLoginResponseDto(jwtToken, refreshToken));
    }
}
