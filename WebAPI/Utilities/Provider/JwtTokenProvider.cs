using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebAPI.Model;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Options;

namespace WebAPI.Utilities.Provider;

public class JwtTokenProvider(IOptions<JwtOptions> optionAccessor,
                              UserManager<AppUser> userManager) : ITokenProvider
{
    private readonly JwtOptions _options = optionAccessor.Value;
    private readonly UserManager<AppUser> _userManager = userManager;
    private readonly SymmetricSecurityKey _symmetricSecurityKey = new(Encoding.UTF8.GetBytes(optionAccessor.Value.SecretKey));

    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<string> CreateTokenAsync(AppUser user, IEnumerable<Claim>? externalClaims = null)
    {
        ArgumentNullException.ThrowIfNull(_options.SecretKey);

        SigningCredentials signingCredentials = new(_symmetricSecurityKey, _options.SecurityAlgorithm);

        JwtHeader header = new(signingCredentials);

        List<Claim> userClaims = [.. await _userManager.GetClaimsAsync(user)];

        if (externalClaims != null)
        {
            userClaims.AddRange(externalClaims);
        }

        JwtPayload payload = new(
            issuer: _options.Iss ?? throw new InvalidOperationException("\'Iss\' is null"),
            audience: _options.Aud ?? throw new InvalidOperationException("\'Aud\' is null"),
            claims: userClaims,
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddMinutes(_options.ExpiredAfterMin));

        JwtSecurityToken securityToken = new(header, payload);

        JwtSecurityTokenHandler handler = new();

        return handler.WriteToken(securityToken);
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
            expires: DateTime.Now.AddMinutes(_options.ExpiredAfterMin));

        JwtSecurityToken securityToken = new(header, payload);

        JwtSecurityTokenHandler handler = new();

        return handler.WriteToken(securityToken);
    }

    public string CreateRefreshToken()
    {
        var rand = new byte[32];

        using var rng = RandomNumberGenerator.Create();

        rng.GetBytes(rand);

        return Convert.ToBase64String(rand);
    }
}
