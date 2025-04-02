using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface IReportRepository
    : IRepository<Report>
{
    void CreateReport(Report report);
    Task<List<Report>> FindAllReport(string? type, int skip, int take, CancellationToken cancellationToken = default);
}
