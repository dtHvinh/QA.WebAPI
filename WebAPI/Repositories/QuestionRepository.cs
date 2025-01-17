using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Libraries.Collections;
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
    private readonly ApplicationDbContext _dbContext = dbContext;

    private static async Task<List<Question>> SearchQuestionWithKeyword(
        ICollection<Question> tagQuestions,
        string keyword,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var questions = await tagQuestions
            .Where(q =>
            {
                return
                q.Title.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                q.Content.Contains(keyword, StringComparison.InvariantCultureIgnoreCase);
            })
            .AsAsyncQueryable()
            .EvaluateQuery(new SearchQuestionSpecification())
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return questions ?? [];
    }

    private static async Task<List<Question>> SearchQuestionByTag(
        ICollection<Question> tagQuestions,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        var questions = await tagQuestions
            .AsAsyncQueryable()
            .EvaluateQuery(new SearchQuestionSpecification())
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return questions ?? [];
    }

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
        // Load questions of the tag
        var tagQuestions = await _dbContext
            .Set<Tag>()
            .Where(e => e.Id.Equals(searchParams.TagId))
            .AsSplitQuery()
            .Include(e => e.Questions)
            .ThenInclude(e => e.Tags)
            .Include(e => e.Questions)
            .ThenInclude(e => e.Comments
                               .OrderByDescending(o => o.CreatedAt)
                               .Take(10))
            .FirstAsync(cancellationToken);

        // Evaluate the query
        return
            string.IsNullOrEmpty(searchParams.Keyword)
            ? await SearchQuestionByTag(tagQuestions.Questions,
                                        searchParams.Skip,
                                        searchParams.Take,
                                        cancellationToken)
            : await SearchQuestionWithKeyword(tagQuestions.Questions,
                                              searchParams.Keyword,
                                              searchParams.Skip,
                                              searchParams.Take,
                                              cancellationToken);
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