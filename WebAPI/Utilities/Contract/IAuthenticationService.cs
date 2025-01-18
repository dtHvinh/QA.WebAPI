using WebAPI.Utilities.Response.AuthResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Utilities.Contract;

public interface IAuthenticationService
{
    Task<GenericResult<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}
