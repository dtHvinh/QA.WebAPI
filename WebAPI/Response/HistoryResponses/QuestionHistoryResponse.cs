namespace WebAPI.Response.HistoryResponses;

public class QuestionHistoryResponse
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string QuestionHistoryType { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}
