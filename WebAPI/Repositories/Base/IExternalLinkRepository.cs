using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface IExternalLinkRepository : IRepository<ExternalLinks>
{
    Task AddOrUpdate(int userId, string provider, string url);
    Task AddOrUpdateMany(int userId, List<(string provider, string url)> links);
    Task<List<ExternalLinks>> GetUserLinks(int userId, CancellationToken cancellationToken = default);
}
