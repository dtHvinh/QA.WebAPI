using WebAPI.Model;

namespace WebAPI.Repositories.Base;

public interface IReportRepository : IRepository<Report>
{
    void AddAnswerReport(AnswerReport report);
    void AddQuestionReport(QuestionReport report);
}
