
namespace WebAPI.Utilities.Contract;

public interface ICacheService
{
    Task<bool> BanUserAsync(int userId, DateTime to, string reason, CancellationToken cancellationToken);
    Task FreeCommunityName(string communityName, CancellationToken cancellationToken);
    Task<int?> GetCommunityIdFromInvLink(string invitationLink, CancellationToken cancellationToken);
    Task<string?> GetInvitationLink(int communityId, CancellationToken cancellationToken);
    Task<DateTime?> IsBanned(int userId, CancellationToken cancellationToken);
    bool IsBanned(int userId);
    Task<(DateTime, string)?> IsBannedWithReason(int userId, CancellationToken cancellationToken);
    Task<bool> IsCommunityNameUsed(string communityName, CancellationToken cancellationToken);
    Task SetInvitationLink(string invitationLink, int communityId, CancellationToken cancellationToken);
    Task SetUsedCommunity(string communityName, CancellationToken cancellationToken);
    Task SetUsedEmail(string email, CancellationToken cancellationToken);
    Task Unban(int userId, CancellationToken cancellationToken);
}
