using WebAPI.Model;
using WebAPI.Specification.Base;

namespace WebAPI.Specification;

public class ValidQuestionSpecification : SpecificationBase<Question>
{
    public ValidQuestionSpecification()
    {
        AddCriteria(question => !question.IsDeleted);
    }
}
