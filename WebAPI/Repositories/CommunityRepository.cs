using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(ICommunityRepository))]
public class CommunityRepository(ApplicationDbContext dbContext) : RepositoryBase<Community>(dbContext), ICommunityRepository
{
    public void CreateCommunity(Community community)
    {
        Add(community);
    }

    public void CreateCommunity(string name, string description, string iconImage, bool isPrivate)
    {
        var community = new Community
        {
            Name = name,
            Description = description,
            IconImage = iconImage,
            IsPrivate = isPrivate
        };

        Add(community);
    }

    public async Task<List<CommunityWithJoinStatus>> GetCommunitiesWithJoinStatusAsync(int userId, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await Table.Skip(skip).Take(take).Select(e => new CommunityWithJoinStatus
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            IconImage = e.IconImage,
            IsPrivate = e.IsPrivate,
            MemberCount = e.MemberCount,
            IsJoined = e.Members.Any(m => m.UserId == userId)
        }).ToListAsync(cancellationToken);
    }

    public async Task<int> GetMemberCount(int communityId, CancellationToken cancellationToken)
    {
        return await dbContext.Set<CommunityMember>().CountAsync(cm => cm.CommunityId == communityId, cancellationToken);
    }

    public class CommunityWithJoinStatus
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? IconImage { get; set; }
        public bool IsPrivate { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public int MemberCount { get; set; } = 1;
        public bool IsJoined { get; set; }
    }
}
