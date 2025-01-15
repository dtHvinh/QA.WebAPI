using Microsoft.EntityFrameworkCore;
using WebAPI.Model;
using WebAPI.Specification.Base;

namespace WebAPI.Specification;

public class AvailableQuestionWithTagSpec : SpecificationBase<Question>
{
    public AvailableQuestionWithTagSpec()
    {
        AddCriteria(question => !question.IsDeleted &&
                                !question.IsDraft);
    }

    public override IQueryable<Question> EvaluateQuery(IQueryable<Question> query)
    {
        return base.EvaluateQuery(query)
                   .Include(e => e.Answers)
                   .Include(e => e.Author)
                   .Include(e => e.Comments)
                   .Include(e => e.QuestionTags)
                   .ThenInclude(e => e.Tag);
    }
}
