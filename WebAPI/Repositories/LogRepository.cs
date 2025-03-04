using WebAPI.Attributes;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(ILogRepository))]
public class LogRepository : ILogRepository
{
}
