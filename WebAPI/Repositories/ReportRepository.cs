using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IReportRepository))]
public class ReportRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Report>(dbContext), IReportRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public void AddQuestionReport(QuestionReport report)
    {
        var table = _dbContext.Set<QuestionReport>();
        table.Add(report);
    }

    public void AddAnswerReport(AnswerReport report)
    {
        var table = _dbContext.Set<AnswerReport>();
        table.Add(report);
    }
}
