
namespace WebAPI.Utilities.Contract;

public interface ICacheService
{
    Task<bool> BanUserAsync(int userId, DateTime to, CancellationToken cancellationToken);
    Task<DateTime?> IsBanned(int userId, CancellationToken cancellationToken);
    Task SetUsedEmail(string email, CancellationToken cancellationToken);
}
