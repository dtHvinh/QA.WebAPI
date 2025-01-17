using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface IReportRepository : IRepository<Report>
{
    void AddQuestionReport(QuestionReport report);
}
