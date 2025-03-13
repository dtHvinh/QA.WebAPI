
namespace WebAPI.Utilities.Contract;

public interface ICacheService
{
    Task<bool> BanUserAsync(int userId, DateTime to, string reason, CancellationToken cancellationToken);
    Task<DateTime?> IsBanned(int userId, CancellationToken cancellationToken);
    bool IsBanned(int userId);
    Task<(DateTime, string)?> IsBannedWithReason(int userId, CancellationToken cancellationToken);
    Task SetUsedEmail(string email, CancellationToken cancellationToken);
    Task Unban(int userId, CancellationToken cancellationToken);
}
