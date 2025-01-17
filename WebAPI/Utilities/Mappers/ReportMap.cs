using WebAPI.Dto;
using WebAPI.Model;

namespace WebAPI.Utilities.Mappers;

public static class ReportMap
{
    public static QuestionReport ToQuestionReport(this CreateReportDto dto, Guid reporterId)
    {
        return new QuestionReport
        {
            ReporterId = reporterId,
            QuestionId = dto.TargetId,
            Description = dto.Description
        };
    }

    public static AnswerReport ToAnswerReport(this CreateReportDto dto, Guid reporterId)
    {
        return new AnswerReport
        {
            ReporterId = reporterId,
            AnswerId = dto.TargetId,
            Description = dto.Description
        };
    }
}
