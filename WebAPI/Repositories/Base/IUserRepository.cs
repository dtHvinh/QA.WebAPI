using WebAPI.Model;
using WebAPI.Utilities;

namespace WebAPI.Repositories.Base;

public interface IUserRepository : IRepository<AppUser>
{
    Task<QueryResult<AppUser>> AddUserAsync(AppUser user, string password, CancellationToken cancellationToken);
}
