using WebAPI.Model;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories.Base;

public interface IUserRepository
{
    Task<OperationResult<AppUser>> AddUserAsync(AppUser user, string password, CancellationToken cancellationToken);
    Task ChangeReputationAsync(Guid id, int amount, CancellationToken cancellationToken = default);
    Task<AppUser?> FindByEmail(string email, CancellationToken cancellationToken = default);
    Task<AppUser?> FindUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
