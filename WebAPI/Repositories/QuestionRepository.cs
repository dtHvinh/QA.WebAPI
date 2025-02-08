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

    public async Task<List<Question>> FindQuestionByUserId(int userId, int skip, int take, QuestionSortOrder sortOrder, CancellationToken cancellationToken)
    {
        var query = Table.Where(e => e.AuthorId == userId)
            .EvaluateQuery(new ValidQuestionSpecification());

        query = sortOrder switch
        {
            QuestionSortOrder.Newest => query.OrderByDescending(e => e.CreatedAt),
            QuestionSortOrder.MostViewed => query.OrderByDescending(e => e.ViewCount),
            QuestionSortOrder.MostVoted => query.OrderByDescending(e => e.Upvote - e.Downvote),
            QuestionSortOrder.Solved => query.OrderByDescending(e => e.IsSolved),
            QuestionSortOrder.Draft => query.OrderByDescending(e => e.IsDraft),
            _ => throw new InvalidOperationException(),
        };

        return await query
            .Skip(skip)
            .Take(take)
            .Include(e => e.Tags.Take(5))
            .ToListAsync(cancellationToken);
    }


    public async Task<List<Question>> FindQuestion(int skip, int take, QuestionSortOrder sortOrder, CancellationToken cancellationToken)
    {
        var query = Table.EvaluateQuery(new ValidQuestionSpecification());

        query = sortOrder switch
        {
            QuestionSortOrder.Newest => query.OrderByDescending(e => e.CreatedAt),
            QuestionSortOrder.MostViewed => query.OrderByDescending(e => e.ViewCount),
            QuestionSortOrder.MostVoted => query.OrderByDescending(e => e.Upvote - e.Downvote),
            QuestionSortOrder.Solved => query.OrderByDescending(e => e.IsSolved),
            QuestionSortOrder.Draft => query.OrderByDescending(e => e.IsDraft),
            _ => throw new InvalidOperationException(),
        };

        var q = await query
            .Skip(skip)
            .Take(take)
            .Include(e => e.Tags)
            .ThenInclude(e => e.Description)
            .ToListAsync(cancellationToken);

        return q;
    }

    /// <inheritdoc/>
    public async Task<Question?> FindQuestionDetailByIdAsync(int id, CancellationToken cancellationToken)
    {
        var specification = new QuestionFullDetailSpecification();
        var result = await Table.Where(e => e.Id == id)
                    .EvaluateQuery(specification)
                    .FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task<List<Question>> FindQuestionsByTagId(
        int tagId, QuestionSortOrder sortOrder, int skip, int take, CancellationToken cancellationToken)
    {
        Func<Question, object> orderByFunc;

        orderByFunc = sortOrder switch
        {
            QuestionSortOrder.Newest => (q => q.CreatedAt),
            QuestionSortOrder.MostVoted => (q => q.Upvote - q.Downvote),
            QuestionSortOrder.MostViewed => (q => q.ViewCount),
            QuestionSortOrder.Solved => (q => q.IsSolved),
            QuestionSortOrder.Draft => (q => q.IsDraft),
            _ => (q => q.CreatedAt),
        };

        var result = await _dbContext.Set<Tag>().Where(e => e.Id == tagId)
            .Select(e => new Tag()
            {
                Name = e.Name,
                Description = e.Description,
                Questions = e.Questions.OrderByDescending(orderByFunc).Skip(skip).Take(take).ToList()
            }).FirstAsync(cancellationToken);

        return [.. result.Questions];
    }

    public async Task<Question?> FindQuestionWithAuthorByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await Table.Where(e => e.Id == id)
                    .Include(e => e.Author)
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

    public void SoftDeleteQuestion(Question question)
    {
        question.SolftDelete();
        question.UpdatedAt = DateTime.UtcNow;
        Entities.Update(question);
    }

    public async Task<int> CountUserQuestion(int userId)
    {
        return await Table.CountAsync(e => e.AuthorId == userId);
    }

    public void MarkAsView(int questionId)
    {
        var q = Entities.FirstOrDefault(e => e.Id == questionId);
        if (q != null)
        {
            q.ViewCount++;
        }
    }
}