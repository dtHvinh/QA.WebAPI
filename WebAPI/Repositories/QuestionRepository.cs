using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Specification;
using WebAPI.Specification.Base;
using WebAPI.Utilities.Collections;
using WebAPI.Utilities.Params;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IQuestionRepository))]
public class QuestionRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Question>(dbContext), IQuestionRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    /// <inheritdoc/>
    public async Task<Question?> FindAvailableQuestionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var specification = new QuestionFullDetailSpecification();
        var result = await Table.Where(e => e.Id == id)
                    .EvaluateQuery(specification)
                    .FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task<List<Question>> SearchQuestionAsync(
        QuestionSearchParams searchParams, CancellationToken cancellationToken)
    {
        var searchQuestionSpec = new SearchQuestionSpecification();

        // Load questions of the tag
        var tagQuestions = await _dbContext
            .Set<Tag>()
            .Where(e => e.Id.Equals(searchParams.TagId))
            .Include(e => e.Questions)
            .FirstAsync(cancellationToken);

        // Evaluate the query
        var questions = await tagQuestions.Questions
            .Where(q =>
            {
                return
                q.Title.Contains(searchParams.Keyword, StringComparison.InvariantCultureIgnoreCase) ||
                q.Content.Contains(searchParams.Keyword, StringComparison.InvariantCultureIgnoreCase);
            })
            .Skip(searchParams.Skip)
            .Take(searchParams.Take)
            .AsAsyncQueryable()
            .EvaluateQuery(searchQuestionSpec)
            .ToListAsync(cancellationToken);

        return questions ?? [];
    }

    public async Task SetQuestionTag(Question question, List<Tag> tags)
    {
        await _dbContext.Entry(question).Collection(e => e.Tags).LoadAsync();
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