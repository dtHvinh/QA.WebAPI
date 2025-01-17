using WebAPI.Dto;
using WebAPI.Model;

namespace WebAPI.Utilities.Mappers;

public static class ReportMap
{
    public static QuestionReport ToQuestionReport(this CreateQuestionReportDto dto, Guid reporterId)
    {
        return new QuestionReport
        {
            ReporterId = reporterId,
            QuestionId = dto.QuestionId,
            Description = dto.Description
        };
    }
}
