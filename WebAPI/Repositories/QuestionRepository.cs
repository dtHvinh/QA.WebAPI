using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Specification;
using WebAPI.Specification.Base;
using WebAPI.Utilities.Params;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IQuestionRepository))]
public class QuestionRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Question>(dbContext), IQuestionRepository
{
    /// <inheritdoc/>
    public async Task<Question?> FindAvailableQuestionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var specification = new QuestionFullDetailSpecification();
        var result = await Table.Where(e => e.Id == id)
                    .EvaluateQuery(specification)
                    .FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task<List<Question>> FindQuestionAsync(
        QuestionSearchParams searchParams, CancellationToken cancellationToken)
    {
        //var specification = new QuestionSearchSpecification(searchParams.Keyword, null, null);

        //Table.Include(e => e.Author)
        //     .Include(e => e.QuestionTags)
        //     .ThenInclude(e => e.Tag)
        //     .Where(e => e.QuestionTags.Any(e => e.Tag!.Name.Equals(searchParams.Tag, StringComparison.InvariantCultureIgnoreCase)));

        throw new NotImplementedException();
    }

    public async Task SetQuestionTag(Question question, List<Tag> tags)
    {
        await dbContext.Entry(question).Collection(e => e.Tags).LoadAsync();
        question.Tags = tags;
    }

    public void MarkAsView(Guid questionId)
    {
        var q = Entities.FirstOrDefault(e => e.Id == questionId);
        if (q != null)
        {
            q.ViewCount++;
        }
    }
}