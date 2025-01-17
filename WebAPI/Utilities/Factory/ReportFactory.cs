using WebAPI.Model;

namespace WebAPI.Utilities.Factory;

public class ReportFactory
{
    public static Report Create(string type)
    {
        return type.ToLowerInvariant() switch
        {
            "question" => new QuestionReport(),
            "answer" => new AnswerReport(),
            _ => throw new ArgumentException("Invalid report type")
        };
    }
}
