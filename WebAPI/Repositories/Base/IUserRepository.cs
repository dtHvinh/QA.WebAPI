using WebAPI.Model;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories.Base;

public interface IUserRepository : IRepository<ApplicationUser>
{
    Task<GenericResult<ApplicationUser>> AddUserAsync(ApplicationUser user, string password, CancellationToken cancellationToken);
    Task ChangeReputationAsync(int id, int amount, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> FindByEmail(string email, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> FindByUsername(string username, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> FindUserByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> FindUserWithLinks(int id, CancellationToken cancellationToken = default);
    Task<int> GetReputation(int id, CancellationToken cancellationToken = default);
}
