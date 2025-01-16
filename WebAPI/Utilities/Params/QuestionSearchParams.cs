namespace WebAPI.Utilities.Params;

public record struct QuestionSearchParams(string Keyword, string Tag, int Skip, int Take);

