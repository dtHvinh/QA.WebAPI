using WebAPI.Utilities.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Utilities.Contract;

public interface IAuthenticationService
{
    Task<OperationResult<AuthResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}
