using WebAPI.Dto;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Utilities.Contract;

public interface IAuthenticationService
{
    Task<OperationResult<AuthResponseDto>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
}
