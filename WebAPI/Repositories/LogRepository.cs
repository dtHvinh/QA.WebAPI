using WebAPI.Attributes;
using WebAPI.DocumentDb;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(ILogRepository))]
public class LogRepository(DocumentCollection<SysLog> documentCollection) : ILogRepository
{
    private readonly DocumentCollection<SysLog> _documentCollection = documentCollection;

    public async Task<List<SysLog>> GetLogs(int skip, int take, CancellationToken cancellationToken)
    {
        var docs = await _documentCollection.GetDescAsync(e => e.UtcTimestamp, skip, take, cancellationToken);

        return docs;
    }

    public async Task<int> CountAsync() => (int)await _documentCollection.CountAsync();
}
