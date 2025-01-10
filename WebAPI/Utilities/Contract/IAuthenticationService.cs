using WebAPI.Dto;
using WebAPI.Utilities.Result;

namespace WebAPI.Utilities.Contract;

public interface IAuthenticationService
{
    Task<AuthResult<AuthResponseDto>> LoginAsync(string email, string password, CancellationToken cancellationToken);
}
