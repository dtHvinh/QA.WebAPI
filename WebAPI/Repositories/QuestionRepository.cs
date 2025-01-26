using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Libraries.Collections;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Specification;
using WebAPI.Specification.Base;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Params;
using static WebAPI.Utilities.Enums;

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

    public async Task<List<Question>> FindQuestionByUserId(Guid userId, int skip, int take, QuestionSortOrder sortOrder, CancellationToken cancellationToken)
    {
        var query = Table.Where(e => e.AuthorId == userId)
                         .EvaluateQuery(new ValidQuestionSpecification());

        query = sortOrder switch
        {
            QuestionSortOrder.Newest => query.OrderByDescending(e => e.CreatedAt),
            QuestionSortOrder.MostViewed => query.OrderByDescending(e => e.ViewCount),
            QuestionSortOrder.MostVoted => query.OrderByDescending(e => e.Upvote - e.Downvote),
            QuestionSortOrder.Solved => query.OrderByDescending(e => e.IsSolved),
            _ => throw new InvalidOperationException(),
        };

        return await query.Skip(skip).Take(take).Include(e => e.Tags).ToListAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Question?> FindQuestionDetailByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var specification = new QuestionFullDetailSpecification();
        var result = await Table.Where(e => e.Id == id)
                    .EvaluateQuery(specification)
                    .FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task<Question?> FindQuestionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var result = await Table.Where(e => e.Id == id)
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

    public void UpdateQuestion(Question question)
    {
        question.UpdatedAt = DateTime.UtcNow;
        Entities.Update(question);
    }

    public void TryEditQuestion(Question question, out string? errMsg)
    {
        if (question.IsSolved)
        {
            errMsg = "Can not edit solved question";
            return;
        }

        if (question.Upvote > question.Downvote)
        {
            errMsg = "Can not edit question people have upvoted";
            return;
        }

        errMsg = null;
        question.UpdatedAt = DateTime.UtcNow;
        Entities.Update(question);
    }

    public void VoteChange(Question question, VoteUpdateTypes updateType, int value)
    {
        switch (updateType)
        {
            case VoteUpdateTypes.CreateNew:
                if (value == 1)
                    question.Upvote += value;
                else if (value == -1)
                    question.Downvote -= value; // - plus - eq +

                Entities.Update(question);
                break;

            case VoteUpdateTypes.ChangeVote:
                question.Upvote += value;
                question.Downvote -= value;
                Entities.Update(question);
                break;

            case VoteUpdateTypes.NoChange:
                break;

            default:
                throw new InvalidOperationException();
        }

        Entities.Update(question);
    }

    public void TrySoftDeleteQuestion(Question question, out string? errorMessage)
    {
        if (question.IsSolved)
        {
            errorMessage = "Can not delete solved question";
            return;
        }

        if (question.Upvote - question.Downvote > 0)
        {
            errorMessage = "Can not delete question people may find it valuable";
            return;
        }

        if (_dbContext.Set<Answer>().Where(e => e.QuestionId.Equals(question.Id))
            .Any(e => e.Upvote > e.Downvote))
        {
            errorMessage = "Can not delete question people may find it valuable";
            return;
        }

        errorMessage = null;
        question.SolftDelete();
        Entities.Update(question);
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