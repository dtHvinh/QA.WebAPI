using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Specification;
using WebAPI.Specification.Base;
using WebAPI.Utilities.Extensions;
using static WebAPI.Utilities.Enums;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IQuestionRepository))]
public class QuestionRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Question>(dbContext), IQuestionRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Question?> FindQuestionById(int questionId, CancellationToken cancellationToken)
    {
        return await Table.FirstOrDefaultAsync(e => e.Id == questionId, cancellationToken);
    }

    public async Task<List<Question>> FindQuestionByUserId(int userId, int skip, int take, QuestionSortOrder sortOrder,
        CancellationToken cancellationToken)
    {
        var query = Table.Where(e => e.AuthorId == userId)
            .EvaluateQuery(new ValidQuestionSpecification());

        query = sortOrder switch
        {
            QuestionSortOrder.Newest => query.OrderByDescending(e => e.CreatedAt),
            QuestionSortOrder.MostViewed => query.OrderByDescending(e => e.ViewCount),
            QuestionSortOrder.MostVoted => query.OrderByDescending(e => e.Upvotes - e.Downvotes),
            QuestionSortOrder.Solved => query.OrderByDescending(e => e.IsSolved),
            _ => throw new InvalidOperationException(),
        };

        return await query
            .Skip(skip)
            .Take(take)
            .Include(e => e.Author)
            .Include(e => e.Tags.Take(5))
            .ThenInclude(e => e.Description)
            .ToListAsync(cancellationToken);
    }


    public async Task<List<Question>> FindQuestion(int skip, int take, QuestionSortOrder sortOrder,
        CancellationToken cancellationToken)
    {
        var query = Table.EvaluateQuery(new ValidQuestionSpecification());

        query = sortOrder switch
        {
            QuestionSortOrder.Newest => query.OrderByDescending(e => e.CreatedAt),
            QuestionSortOrder.MostViewed => query.OrderByDescending(e => e.ViewCount),
            QuestionSortOrder.MostVoted => query.OrderByDescending(e => e.Upvotes),
            QuestionSortOrder.Solved => query.OrderByDescending(e => e.IsSolved),
            _ => throw new InvalidOperationException(),
        };

        var q = await query
            .Skip(skip)
            .Take(take)
            .Include(e => e.Author)
            .Include(e => e.Tags)
            .ThenInclude(e => e.Description)
            .ToListAsync(cancellationToken);

        return q;
    }

    /// <inheritdoc/>
    public async Task<Question?> FindQuestionDetailByIdAsync(int id, CancellationToken cancellationToken)
    {
        var specification = new ValidQuestionSpecification();

        var result = await Table.Where(e => e.Id == id)
                   .EvaluateQuery(specification)
                   .Include(e => e.Author)
                   .AsSplitQuery()
                   .Include(e => e.Answers.Where(e => !e.IsDeleted)
                                          .OrderByDescending(e => e.IsAccepted)
                                          .ThenByDescending(e => e.Upvote)
                                          .ThenByDescending(e => e.CreatedAt)
                                          .Take(10))
                   .ThenInclude(e => e.Author)
                   .AsSplitQuery()
                   .Include(e => e.Tags)
                   .ThenInclude(e => e.Description)
                   .FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task<List<Question>> FindQuestionsByTagId(
        int tagId, QuestionSortOrder sortOrder, int skip, int take, CancellationToken cancellationToken)
    {
        var query = Table.Where(e => e.Tags.Any(t => t.Id == tagId))
             .EvaluateQuery(new ValidQuestionSpecification());

        query = sortOrder switch
        {
            QuestionSortOrder.Newest => query.OrderByDescending(e => e.CreatedAt),
            QuestionSortOrder.MostViewed => query.OrderByDescending(e => e.ViewCount),
            QuestionSortOrder.MostVoted => query.OrderByDescending(e => e.Upvotes - e.Downvotes),
            QuestionSortOrder.Solved => query.OrderByDescending(e => e.IsSolved),
            _ => throw new InvalidOperationException(),
        };

        return await query.Skip(skip)
             .Take(take).ToListAsync(cancellationToken);

    }

    public async Task<Question?> FindQuestionWithAuthorByIdAsync(int id, CancellationToken cancellationToken)
    {
        var result = await Table.Where(e => e.Id == id)
            .Include(e => e.Author)
            .FirstOrDefaultAsync(cancellationToken);

        return result;
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
                    question.Upvotes += value;
                else if (value == -1)
                    question.Downvotes -= value; // - plus - eq +

                Entities.Update(question);
                break;

            case VoteUpdateTypes.ChangeVote:
                question.Upvotes += value;
                question.Downvotes -= value;
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

    public async Task<int> CountUserQuestion(int userId, CancellationToken cancellationToken)
    {
        return await Table.CountAsync(e => e.AuthorId == userId, cancellationToken);
    }

    public async Task<int> CountQuestionAskedByUser(int userId, CancellationToken cancellationToken)
    {
        return await Table.CountAsync(e => e.AuthorId == userId, cancellationToken);
    }

    public async Task<int> CountUserUpvote(int userId, CancellationToken cancellationToken)
    {
        return await Table.Where(e => e.AuthorId == userId)
            .SumAsync(e => e.Upvotes, cancellationToken);
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