using WebAPI.Model;
using static WebAPI.Repositories.CommunityRepository;

namespace WebAPI.Repositories.Base;

public interface ICommunityRepository : IRepository<Community>
{
    void CreateChatRoom(CommunityChatRoom chatRoom);
    void CreateCommunity(Community community);
    void CreateCommunity(string name, string description, string iconImage, bool isPrivate);
    Task<List<CommunityWithJoinStatus>> GetCommunitiesWithJoinStatusAsync(int userId, int skip, int take, CancellationToken cancellationToken = default);
    Task<Community?> GetCommunityDetailByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Community?> GetCommunityDetailByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Community>> GetCommunityUserJoined(int userId, int skip, int take, CancellationToken cancellationToken);
    Task<int> GetMemberCount(int communityId, CancellationToken cancellationToken);
    Task<List<CommunityWithJoinStatus>> GetPopularCommunitiesWithJoinStatus(int userId, int skip, int take, CancellationToken cancellationToken);
    Task<bool> IsJoined(int userId, int communityId, CancellationToken cancellationToken = default);
    Task<bool> IsMember(int userId, int communityId, CancellationToken cancellationToken = default);
    Task<bool> IsModerator(int userId, int communityId, CancellationToken cancellationToken = default);
    Task<bool> IsOwner(int userId, int communityId, CancellationToken cancellationToken = default);
}
