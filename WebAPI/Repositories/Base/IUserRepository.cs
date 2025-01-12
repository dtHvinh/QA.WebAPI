using WebAPI.Model;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories.Base;

public interface IUserRepository
{
    Task<OperationResult<AppUser>> AddUserAsync(AppUser user, string password, CancellationToken cancellationToken);
    Task<OperationResult<AppUser>> FindByEmail(string email, CancellationToken cancellationToken = default);
    Task<OperationResult<AppUser>> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
