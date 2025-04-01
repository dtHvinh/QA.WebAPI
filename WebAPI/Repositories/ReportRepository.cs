using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IReportRepository))]
public class ReportRepository(ApplicationDbContext dbContext) : RepositoryBase<Report>(dbContext), IReportRepository
{
    public void CreateReport(Report report)
    {
        Entities.Add(report);
    }
}
