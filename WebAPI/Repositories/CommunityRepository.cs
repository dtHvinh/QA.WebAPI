using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(ICommunityRepository))]
public class CommunityRepository(ApplicationDbContext dbContext) : RepositoryBase<Community>(dbContext), ICommunityRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

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
        return await _dbContext.Set<CommunityMember>().CountAsync(cm => cm.CommunityId == communityId, cancellationToken);
    }

    public async Task<Community?> GetCommunityDetailByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Table.Where(e => e.Id == id)
            .Include(e => e.Members.Take(20))
            .AsSplitQuery()
            .Include(e => e.Rooms.Take(20))
            .ThenInclude(e => e.Messages.Take(20))
            .ThenInclude(e => e.User)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Community?> GetCommunityDetailByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Table.Where(e => e.Name == name)
            .Include(e => e.Members.Take(20))
            .AsSplitQuery()
            .Include(e => e.Rooms.Take(20))
            .ThenInclude(e => e.Messages.Take(20))
            .ThenInclude(e => e.User)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsJoined(int userId, int communityId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<CommunityMember>().AnyAsync(cm => cm.UserId == userId && cm.CommunityId == communityId, cancellationToken);
    }

    public async Task<bool> IsModerator(int userId, int communityId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<CommunityMember>().AnyAsync(cm => cm.UserId == userId && cm.CommunityId == communityId && cm.IsModerator, cancellationToken);
    }

    public async Task<bool> IsOwner(int userId, int communityId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<CommunityMember>().AnyAsync(c => c.Id == communityId && c.UserId == userId && c.IsOwner, cancellationToken);
    }

    public async Task<List<Community>> GetCommunityUserJoined(int userId, int skip, int take, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<CommunityMember>()
            .Where(e => e.UserId == userId)
            .Skip(skip)
            .Take(take)
            .Include(e => e.Community)
            .Select(e => e.Community)
            .ToListAsync(cancellationToken);
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
