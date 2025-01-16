using WebAPI.Model;
using WebAPI.Specification.Base;

namespace WebAPI.Specification;

public class QuestionSearchSpecification : SpecificationBase<Question>
{
    public QuestionSearchSpecification(string keyword, int? skip, int? take)
    {
        AddCriteria(question => (question.Title.Contains(keyword) || question.Content.Contains(keyword)) &&
                                !question.IsDeleted &&
                                !question.IsDraft);

        Skip = skip;
        Take = take;
    }

    public int? Skip { get; init; }
    public int? Take { get; init; }

    public override IQueryable<Question> EvaluateQuery(IQueryable<Question> query)
    {
        if (Skip == null || Take == null)
        {
            return base.EvaluateQuery(query)
                .OrderByDescending(e => new { e.Upvote, e.ViewCount, e.CreatedAt });
        }

        return base.EvaluateQuery(query)
            .OrderByDescending(e => new { e.Upvote, e.ViewCount, e.CreatedAt })
            .Skip(Skip.Value)
            .Take(Take.Value);
    }
}
