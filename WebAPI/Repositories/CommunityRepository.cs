using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Utilities.Contract;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(ICommunityRepository))]
public class CommunityRepository(ApplicationDbContext dbContext, ICacheService cacheService) : RepositoryBase<Community>(dbContext), ICommunityRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task CreateCommunity(Community community, CancellationToken cancellationToken = default)
    {
        Add(community);
        await _cacheService.SetUsedCommunity(community.Name, cancellationToken);
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

    public void CreateChatRoom(CommunityChatRoom chatRoom)
    {
        _dbContext.Set<CommunityChatRoom>().Add(chatRoom);
    }

    public async Task<bool> JoinCommunity(AppUser appUser, Community community, CancellationToken cancellationToken = default)
    {
        if (await _dbContext.Set<CommunityMember>().AnyAsync(
            cm => cm.UserId == appUser.Id
            && cm.CommunityId == community.Id,
            cancellationToken))
            return false;

        var member = new CommunityMember
        {
            User = appUser,
            Community = community
        };

        _dbContext.Set<CommunityMember>().Add(member);

        return true;
    }

    public async Task<List<CommunityWithJoinStatus>> Search(int userId, string searchTerm, int skip, int take, CancellationToken cancellationToken)
    {
        return await Table.Where(e => e.Name.Contains(searchTerm))
            .Skip(skip)
            .Take(take)
            .Select(e => new CommunityWithJoinStatus
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                IconImage = e.IconImage,
                IsPrivate = e.IsPrivate,
                MemberCount = e.MemberCount,
                IsJoined = e.Members.Any(m => m.UserId == userId)
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<CommunityChatRoom?> GetRoomAsync(int roomId, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<CommunityChatRoom>().FirstOrDefaultAsync(e => e.Id == roomId, cancellationToken);
    }

    public async Task<List<CommunityChatRoom>> GetRooms(int communityId, int skip, int take, CancellationToken cancellationToken)
    {
        return await _dbContext.Set<CommunityChatRoom>()
            .Where(e => e.CommunityId == communityId)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CommunityWithJoinStatus>> GetCommunitiesWithJoinStatusAsync(int userId, int skip, int take, CancellationToken cancellationToken = default)
    {
        return await Table.Where(e => !e.IsPrivate).Skip(skip).Take(take).Select(e => new CommunityWithJoinStatus
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
            .ThenInclude(e => e.Author)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Community?> GetCommunityDetailByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Table.Where(e => e.Name == name)
            .Include(e => e.Members.Take(20))
            .AsSplitQuery()
            .Include(e => e.Rooms.Take(20))
            .ThenInclude(e => e.Messages.Take(20))
            .ThenInclude(e => e.Author)
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

    public async Task<bool> IsMember(int userId, int communityId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<CommunityMember>().AnyAsync(c => c.CommunityId == communityId && c.UserId == userId, cancellationToken);
    }

    public async Task<bool> IsCommunityNameUsed(string name, CancellationToken cancellationToken = default)
    {
        return await _cacheService.IsCommunityNameUsed(name, cancellationToken);
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

    public async Task<List<CommunityWithJoinStatus>> GetPopularCommunitiesWithJoinStatus(int userId, int skip, int take, CancellationToken cancellationToken)
    {
        return await Table.OrderByDescending(e => e.MemberCount)
            .Where(e => !e.IsPrivate)
            .Skip(skip)
            .Take(take)
            .Select(e => new CommunityWithJoinStatus
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                IconImage = e.IconImage,
                IsPrivate = e.IsPrivate,
                MemberCount = e.MemberCount,
                IsJoined = e.Members.Any(m => m.UserId == userId)
            })
            .ToListAsync(cancellationToken);
    }

    public void DeleteChatRoom(CommunityChatRoom communityChatRoom)
    {
        _dbContext.Set<CommunityChatRoom>().Remove(communityChatRoom);
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
