using WebAPI.Model;
using WebAPI.Specification.Base;

namespace WebAPI.Specification;

public class SearchQuestionSpecification : SpecificationBase<Question>
{
    public SearchQuestionSpecification()
    {
        AddCriteria(question => !question.IsDeleted &&
                                !question.IsDraft);

        AddOrderByDesc(e => new { e.Upvote, e.ViewCount, e.CreatedAt });
    }
}
