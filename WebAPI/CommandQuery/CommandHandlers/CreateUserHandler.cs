using Microsoft.AspNetCore.Identity;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response.AuthResponses;
using WebAPI.Utilities.Mappers;
using WebAPI.Utilities.Provider;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateUserHandler(IUserRepository userRepository,
                               JwtTokenProvider tokenProvider,
                               UserManager<AppUser> userManager,
                               Serilog.ILogger logger)
    : ICommandHandler<CreateUserCommand, GenericResult<AuthResponse>>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly JwtTokenProvider _tokenProvider = tokenProvider;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly Serilog.ILogger _logger = logger;

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
        var refreshToken = await _tokenProvider.GenerateRefreshToken(newUser);

        await _userRepository.SaveChangesAsync(cancellationToken);

        _logger.Information("User {Username} created a new account with id {UserId}", newUser.UserName, newUser.Id);

        return GenericResult<AuthResponse>
            .Success(newUser.ToAuthResponseDto(jwtToken, refreshToken, await _userManager.GetRolesAsync(newUser)));
    }
}
