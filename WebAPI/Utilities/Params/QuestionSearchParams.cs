namespace WebAPI.Utilities.Params;

public record struct QuestionSearchParams(string Keyword, int TagId, int Skip, int Take)
{
    public static QuestionSearchParams From(string keyword, int tagId, int skip, int take)
    {
        return new QuestionSearchParams(keyword, tagId, skip, take);
    }
}

