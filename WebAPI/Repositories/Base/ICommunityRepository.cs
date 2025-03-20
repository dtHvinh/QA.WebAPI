using WebAPI.Model;
using static WebAPI.Repositories.CommunityRepository;

namespace WebAPI.Repositories.Base;

public interface ICommunityRepository : IRepository<Community>
{
    void CreateChatRoom(CommunityChatRoom chatRoom);

    /// <summary>
    /// Create community, then add its name to cache.
    /// </summary>
    Task CreateCommunity(Community community, CancellationToken cancellationToken = default);
    void CreateCommunity(string name, string description, string iconImage, bool isPrivate);
    Task<bool> JoinCommunity(AppUser appUser, Community community, CancellationToken cancellationToken = default);
    Task<List<CommunityWithJoinStatus>> Search(int userId, string searchTerm, int skip, int take, CancellationToken cancellationToken);
    Task<CommunityMember?> GetMemberAsync(int userId, int communityId, CancellationToken cancellationToken);
    Task<List<CommunityWithJoinStatus>> GetCommunitiesWithJoinStatusAsync(int userId, int skip, int take, CancellationToken cancellationToken = default);
    Task<Community?> GetCommunityDetailByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Community?> GetCommunityDetailByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Community>> GetCommunityUserJoined(int userId, int skip, int take, CancellationToken cancellationToken);
    Task<int> GetMemberCount(int communityId, CancellationToken cancellationToken);
    Task<List<CommunityWithJoinStatus>> GetPopularCommunitiesWithJoinStatus(int userId, int skip, int take, CancellationToken cancellationToken);
    Task<CommunityChatRoom?> GetRoomAsync(int roomId, CancellationToken cancellationToken);
    Task<List<CommunityChatRoom>> GetRooms(int communityId, int skip, int take, CancellationToken cancellationToken);
    Task<bool> IsCommunityNameUsed(string name, CancellationToken cancellationToken = default);
    Task<bool> IsJoined(int userId, int communityId, CancellationToken cancellationToken = default);
    Task<bool> IsMember(int userId, int communityId, CancellationToken cancellationToken = default);
    Task<bool> IsModerator(int userId, int communityId, CancellationToken cancellationToken = default);
    Task<bool> IsOwner(int userId, int communityId, CancellationToken cancellationToken = default);
    void DeleteChatRoom(CommunityChatRoom communityChatRoom);
    void UpdateRoom(CommunityChatRoom communityChatRoom);
    void RemoveMember(CommunityMember member);
}
