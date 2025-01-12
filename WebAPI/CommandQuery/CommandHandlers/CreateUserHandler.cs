using CQRS;
using WebAPI.CommandQuery.Commands;
using WebAPI.Dto;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Provider;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateUserHandler(IUserRepository userRepository,
                                      JwtTokenProvider tokenProvider)
    : ICommandHandler<CreateUserCommand, OperationResult<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly JwtTokenProvider _tokenProvider = tokenProvider;

    public async Task<OperationResult<AuthResponseDto>> Handle(
        CreateUserCommand request, CancellationToken cancellationToken)
    {
        var newUser = request.User.ToAppUser();

        var result = await _userRepository.AddUserAsync(newUser, request.User.Password, cancellationToken);

        if (!result.IsSuccess)
        {
            return OperationResult<AuthResponseDto>
                .Failure(result.Message);
        }

        newUser = result.Value!;

        var jwtToken = await _tokenProvider.CreateTokenAsync(newUser);
        var refreshToken = _tokenProvider.CreateRefreshToken();

        return OperationResult<AuthResponseDto>
            .Success(newUser.ToLoginResponseDto(jwtToken, refreshToken));
    }
}
