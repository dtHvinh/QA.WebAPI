using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Model;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Options;
using WebAPI.Utilities.Result;

namespace WebAPI.Utilities.Provider;

public class JwtTokenProvider(IOptions<JwtOptions> optionAccessor,
                              UserManager<AppUser> userManager) : ITokenProvider
{
    private readonly JwtOptions _options = optionAccessor.Value;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SymmetricSecurityKey _symmetricSecurityKey = new(Encoding.UTF8.GetBytes(optionAccessor.Value.SecretKey));

    private string GenerateRefreshToken()
    {
        var signingCredentials = new SigningCredentials(_symmetricSecurityKey, _options.SecurityAlgorithm);

        var header = new JwtHeader(signingCredentials);

        JwtPayload payload = new(
            issuer: _options.Iss ?? throw new InvalidOperationException("\'Iss\' is null"),
            audience: _options.Aud ?? throw new InvalidOperationException("\'Aud\' is null"),
            claims: [],
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(_options.RefreshTokenExpiredAfterMin));

        JwtSecurityToken securityToken = new(header, payload);

        JwtSecurityTokenHandler handler = new();

        return handler.WriteToken(securityToken);
    }

    private async Task<string> CreateTokenInternalAsync(AppUser user, IEnumerable<Claim>? externalClaims = null)
    {
        ArgumentNullException.ThrowIfNull(_options.SecretKey);
        SigningCredentials signingCredentials = new(_symmetricSecurityKey, _options.SecurityAlgorithm);
        JwtHeader header = new(signingCredentials);
        List<Claim> userClaims = [.. await _userManager.GetClaimsAsync(user)];
        if (externalClaims != null)
            userClaims.AddRange(externalClaims);
        JwtPayload payload = new(
            issuer: _options.Iss ?? throw new InvalidOperationException("\'Iss\' is null"),
            audience: _options.Aud ?? throw new InvalidOperationException("\'Aud\' is null"),
            claims: userClaims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(_options.AccessTokenExpiredAfterMin));
        JwtSecurityToken securityToken = new(header, payload);
        JwtSecurityTokenHandler handler = new();
        return handler.WriteToken(securityToken);
    }

    public int GetIdFromToken(string accessToken)
    {
        JwtSecurityTokenHandler handler = new();

        var securityJwt = handler.ReadJwtToken(accessToken);

        var userId = int.Parse(securityJwt.Claims.First(e => e.Type.Equals(ClaimTypes.NameIdentifier)).Value);

        return userId;
    }

    public bool IsTokenExpired(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException(nameof(token), "Token cannot be null or empty.");
        }

        var handler = new JwtSecurityTokenHandler();

        try
        {
            var jwtToken = handler.ReadJwtToken(token);

            if (jwtToken.ValidTo == DateTime.MinValue)
            {
                return false;
            }

            return jwtToken.ValidTo < DateTime.UtcNow;
        }
        catch (Exception ex) when (ex is ArgumentException || ex is SecurityTokenMalformedException)
        {
            return true;
        }
    }

    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<string> CreateTokenAsync(AppUser user, IEnumerable<Claim>? externalClaims = null)
        => await CreateTokenInternalAsync(user, externalClaims);

    /// <summary>
    /// Create new access token by provided refresh token, if the refresh token is expired, perform update.
    /// </summary>
    public async Task<TokenResult> CreateTokenAndUpdateAsync(AppUser user, string refreshToken, IEnumerable<Claim>? externalClaims = null)
    {
        ArgumentNullException.ThrowIfNull(_options.PassPhrase);
        ArgumentNullException.ThrowIfNull(user.RefreshToken);

        if (user.RefreshToken != refreshToken)
        {
            return new TokenResult(false, null, null, ["Refresh token is invalid"]);
        }

        if (IsTokenExpired(user.RefreshToken))
        {
            user.RefreshToken = await GenerateRefreshToken(user);
        }

        var newAccessToken = await CreateTokenInternalAsync(user, externalClaims);

        return new TokenResult(true, newAccessToken, user.RefreshToken, null);
    }

    public string CreateToken(IEnumerable<Claim>? claims = null)
    {
        ArgumentNullException.ThrowIfNull(_options.SecretKey);

        SigningCredentials signingCredentials = new(_symmetricSecurityKey, _options.SecurityAlgorithm);

        JwtHeader header = new(signingCredentials);

        JwtPayload payload = new(
            issuer: _options.Iss ?? throw new InvalidOperationException("\'Iss\' is null"),
            audience: _options.Aud ?? throw new InvalidOperationException("\'Aud\' is null"),
            claims: claims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(_options.AccessTokenExpiredAfterMin));

        JwtSecurityToken securityToken = new(header, payload);

        JwtSecurityTokenHandler handler = new();

        return handler.WriteToken(securityToken);
    }

    /// <summary>
    /// Generate refresh token and save its encrypted version to the user.
    /// </summary>
    public async Task<string> GenerateRefreshToken(AppUser user)
    {
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;

        await _userManager.UpdateAsync(user);

        return refreshToken;
    }
}
