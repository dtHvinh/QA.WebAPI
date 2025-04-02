using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Report>> FindAllReport(string? type, int skip, int take, CancellationToken cancellationToken = default)
    {
        if (type == null)
            return await Table.OrderByDescending(e => e.CreatedAt).Skip(skip).Take(take).ToListAsync(cancellationToken);

        return await Table.Where(e => e.Type == type).Skip(skip).Take(take).ToListAsync(cancellationToken);
    }
}
