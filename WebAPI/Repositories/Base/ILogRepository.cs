using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface ILogRepository
{
    Task<int> CountAsync();
    Task<List<SysLog>> GetLogs(int skip, int take, CancellationToken cancellationToken);
}
