using WebAPI.Model;
using static WebAPI.Repositories.CommunityRepository;

namespace WebAPI.Repositories.Base;

public interface ICommunityRepository : IRepository<Community>
{
    void CreateCommunity(Community community);
    void CreateCommunity(string name, string description, string iconImage, bool isPrivate);
    Task<List<CommunityWithJoinStatus>> GetCommunitiesWithJoinStatusAsync(int userId, int skip, int take, CancellationToken cancellationToken = default);
    Task<int> GetMemberCount(int communityId, CancellationToken cancellationToken);
}
