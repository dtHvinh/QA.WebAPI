namespace WebAPI.Utilities.Params;

public record struct QuestionSearchParams(string Keyword, Guid TagId, int Skip, int Take)
{
    public static QuestionSearchParams From(string keyword, Guid tagId, int skip, int take)
    {
        return new QuestionSearchParams(keyword, tagId, skip, take);
    }
}

