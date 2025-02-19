using Microsoft.IdentityModel.Tokens;

namespace WebAPI.Utilities.Options;

public class JwtOptions
{
    /// <summary>
    /// JwtToken expired after specified minute(s). Default is 15;
    /// </summary>
    public int AccessTokenExpiredAfterMin { get; set; } = 15;
    public int RefreshTokenExpiredAfterMin { get; set; } = 60;

    public string Iss { get; set; } = null!;
    public string Aud { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public string PassPhrase { get; set; } = null!;

    public IEnumerable<string> ValidAudiences { get; set; } = null!;
    public IEnumerable<string> ValidIssuers { get; set; } = null!;

    public string SecurityAlgorithm { get; set; } = SecurityAlgorithms.HmacSha256Signature;
}
