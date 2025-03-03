using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IExternalLinkRepository))]
public class ExternalLinkRepository(
    ApplicationDbContext dbContext)
    : RepositoryBase<ExternalLinks>(dbContext), IExternalLinkRepository
{
    public async Task AddOrUpdate(int userId, string provider, string url)
    {
        var existing = await FindFirstAsync(x => x.AuthorId == userId && x.Provider == provider);

        if (existing == null)
        {
            Add(new()
            {
                AuthorId = userId,
                Provider = provider,
                Url = url
            });
        }
        else
        {
            existing.Url = url;
            Update(existing);
        }
    }

    public async Task AddOrUpdateMany(int userId, List<(string provider, string url)> links)
    {
        foreach (var (provider, url) in links)
        {
            await AddOrUpdate(userId, provider, url);
        }
    }

    public async Task<List<ExternalLinks>> GetUserLinks(int userId, CancellationToken cancellationToken = default)
    {
        return await Table.Where(e => e.AuthorId == userId).ToListAsync(cancellationToken);
    }
}
