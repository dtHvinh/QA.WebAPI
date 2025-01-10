using WebAPI.Model;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories.Base;

public interface IUserRepository
{
    Task<ResultBase<AppUser>> AddUserAsync(AppUser user, string password, CancellationToken cancellationToken);
    Task<ResultBase<AppUser>> FindByEmail(string email, CancellationToken cancellationToken);
}
