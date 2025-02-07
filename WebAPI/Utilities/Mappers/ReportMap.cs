using WebAPI.Dto;
using WebAPI.Model;

namespace WebAPI.Utilities.Mappers;

public static class ReportMap
{
    public static QuestionReport ToQuestionReport(this CreateReportDto dto, int reporterId)
    {
        return new QuestionReport
        {
            AuthorId = reporterId,
            QuestionId = dto.TargetId,
            Description = dto.Description
        };
    }

    public static AnswerReport ToAnswerReport(this CreateReportDto dto, int reporterId)
    {
        return new AnswerReport
        {
            AuthorId = reporterId,
            AnswerId = dto.TargetId,
            Description = dto.Description
        };
    }
}
