﻿using Microsoft.AspNetCore.Identity;
using WebAPI.Model;
using WebAPI.Response.AuthResponses;
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

    public async Task<GenericResult<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        /// Check if user exists
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            var errorMessage = string.Format(EM.USER_EMAIL_NOTFOUND, email);
            return GenericResult<AuthResponse>.Failure(errorMessage);
        }

        // Check password
        bool checkPass = await _userManager.CheckPasswordAsync(user, password);
        if (!checkPass)
        {
            await _userManager.AccessFailedAsync(user);

            return GenericResult<AuthResponse>.Failure("Password is wrong");
        }

        var at = await _tokenProvider.CreateTokenAsync(user);

        var userRt = user.RefreshToken ?? await _tokenProvider.GenerateRefreshToken(user);

        var userRoles = await _userManager.GetRolesAsync(user);

        var authResponse = new AuthResponse(at, userRt, user.UserName!, user.ProfilePicture, userRoles);

        return GenericResult<AuthResponse>.Success(authResponse);
    }
}
