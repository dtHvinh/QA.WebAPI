using WebAPI.Model;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories.Base;

public interface IUserRepository : IRepository<AppUser>
{
    Task<GenericResult<AppUser>> AddUserAsync(AppUser user, string password, CancellationToken cancellationToken);
    Task ChangeReputationAsync(int id, int amount, CancellationToken cancellationToken = default);
    Task<AppUser?> FindByEmail(string email, CancellationToken cancellationToken = default);
    Task<AppUser?> FindUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AppUser?> FindUserWithLinks(int id, CancellationToken cancellationToken = default);
    Task<int> GetReputation(int id, CancellationToken cancellationToken = default);
}
