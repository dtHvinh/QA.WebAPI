using Microsoft.AspNetCore.Identity;
using WebAPI.Dto;
using WebAPI.Model;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Provider;
using WebAPI.Utilities.Result.Base;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Utilities.Services;

public class AuthenticationService(UserManager<AppUser> userManager,
                                   JwtTokenProvider tokenProvider) : IAuthenticationService
{
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly JwtTokenProvider _tokenProvider = tokenProvider;

    public async Task<OperationResult<AuthResponseDto>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        /// Check if user exists
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            var errorMessage = string.Format(EM.EMAIL_NOTFOUND, email);
            return OperationResult<AuthResponseDto>.Failure(errorMessage);
        }

        // Check password
        bool checkPass = await _userManager.CheckPasswordAsync(user, password);
        if (!checkPass)
        {
            await _userManager.AccessFailedAsync(user);

            return OperationResult<AuthResponseDto>.Failure("Password is wrong");
        }

        var at = await _tokenProvider.CreateTokenAsync(user);
        var rt = _tokenProvider.CreateRefreshToken();

        var authResponse = new AuthResponseDto(at, rt, user.UserName!, user.ProfilePicture);

        return OperationResult<AuthResponseDto>.Success(authResponse);
    }
}
