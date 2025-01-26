using Microsoft.EntityFrameworkCore;
using WebAPI.Model;
using WebAPI.Specification.Base;

namespace WebAPI.Specification;

public class QuestionFullDetailSpecification : SpecificationBase<Question>
{
    /// <summary>
    /// Question with full detail.
    /// </summary>
    /// <remarks>
    /// The question to be able to be included in the query must be 
    /// <strong>descending ordered by created date</strong> and:
    /// 
    /// <list type="bullet">
    /// <item>Not Deleted</item>
    /// <item>Not Draft</item>
    /// </list>
    /// 
    /// The each item in the query will include:
    /// <list type="bullet">
    /// <item>Author</item>
    /// <item>Answer</item>
    /// <item>Question comment</item>
    /// <item>Tag</item>
    /// </list>
    /// 
    /// Answer and comment will be limited to 10 newest items base on theirs created date
    /// 
    /// </remarks>
    public QuestionFullDetailSpecification()
    {
        AddCriteria(question => !question.IsDeleted &&
                                !question.IsDraft);

        AddOrderByDesc(question => question.CreatedAt);
    }

    public override IQueryable<Question> EvaluateQuery(IQueryable<Question> query)
    {
        return base.EvaluateQuery(query)
                   .Include(e => e.Author)
                   .Include(e => e.Answers.Where(e => !e.IsDeleted)
                                          .OrderByDescending(e => e.IsAccepted)
                                          .ThenByDescending(e => e.Upvote)
                                          .ThenByDescending(e => e.CreatedAt))
                   .ThenInclude(e => e.Author)
                   .Include(e => e.Comments.OrderByDescending(e => e.CreatedAt))
                   .ThenInclude(e => e.Author)
                   .Include(e => e.Tags);
    }
}
