#pragma warning disable CA1822 // Mark members as static

using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Specification;
using WebAPI.Specification.Base;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Services;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IQuestionRepository))]
public class QuestionRepository(
    ApplicationDbContext dbContext,
    QuestionSearchService questionSearchService)
    : RepositoryBase<Question>(dbContext), IQuestionRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;
    private readonly QuestionSearchService _questionSearchService = questionSearchService;

    public async Task<SearchResult<Question>> SearchQuestionWithNoTagAsync(string keyword, int skip, int take,
        CancellationToken cancellationToken)
    {
        var query = Table
            .Where(e => EF.Functions.FreeText(e.Title, keyword) || EF.Functions.FreeText(e.Content, keyword))
            .EvaluateQuery(new ValidQuestionSpecification());

        return await InternalSearchQuestion(query, skip, take, cancellationToken);
    }

    public async Task<SearchResult<Question>> SearchQuestionWithTagAsync(string keyword, int tagId, int skip, int take,
    CancellationToken cancellationToken)
    {
        var query = Table
            .Where(e => e.Tags.Any(t => t.Id == tagId))
            .Where(e => EF.Functions.FreeText(e.Title, keyword) || EF.Functions.FreeText(e.Content, keyword))
            .EvaluateQuery(new ValidQuestionSpecification());

        return await InternalSearchQuestion(query, skip, take, cancellationToken);
    }

    private async Task<SearchResult<Question>> InternalSearchQuestion(IQueryable<Question> query, int skip, int take, CancellationToken cancellationToken)
    {
        var result = await query
            .OrderBy(e => e.Id)
            .Include(e => e.Author)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        var total = await query.CountAsync(cancellationToken);

        return new SearchResult<Question>(result, total);
    }

    public async Task<SearchResult<Question>> SearchSimilarQuestionAsync(int questionId, int skip, int take, CancellationToken cancellationToken)
    {
        //var question = await Table.FirstOrDefaultAsync(e => e.Id == questionId, cancellationToken);
        //if (question == null)
        //{
        //    return new SearchResult<Question>([], 0);
        //}

        //var tokens = question.Title
        //                     .Split([' '], StringSplitOptions.RemoveEmptyEntries)
        //                     .Select(t => t.Trim().ToLowerInvariant())
        //                     .Distinct()
        //                     .ToList();

        //if (tokens.Count == 0)
        //{
        //    return new SearchResult<Question>([], 0);
        //}

        //var query = Table.Where(q => q.Id != questionId &&
        //                            tokens.Any(token => EF.Functions.Contains(q.Title.ToLower(), $"%{token}%")));

        var results = await Table.EvaluateQuery(new ValidQuestionSpecification())
                                 .OrderBy(q => Guid.NewGuid())
                                 .Skip(skip)
                                 .Take(take)
                                 .ToListAsync(cancellationToken);

        return new SearchResult<Question>(results, -1);
    }

    public async Task<SearchResult<Question>> SearchQuestionYouMayLikeAsync(int skip, int take,
    CancellationToken cancellationToken)
    {
        var response = await Table
            .EvaluateQuery(new ValidQuestionSpecification())
            .Include(e => e.Author)
            .OrderBy(q => Guid.NewGuid())
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);

        return new SearchResult<Question>(response, -1);
    }

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
            QuestionSortOrder.MostVoted => query.OrderByDescending(e => e.Score),
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

    public async Task<Question?> FindQuestionWithTags(int questionId, CancellationToken cancellationToken)
    {
        return await Table.Where(e => e.Id == questionId)
            .Include(e => e.Tags)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<Question>> FindQuestionNoException(int skip, int take, CancellationToken cancellationToken = default)
    {
        return await Table.Skip(skip).Take(take).Include(e => e.Author).ToListAsync(cancellationToken);
    }

    public async Task<List<Question>> FindQuestion(int skip, int take, QuestionSortOrder sortOrder,
        CancellationToken cancellationToken)
    {
        var query = Table.EvaluateQuery(new ValidQuestionSpecification());

        query = sortOrder switch
        {
            QuestionSortOrder.Newest => query.OrderByDescending(e => e.CreatedAt),
            QuestionSortOrder.MostViewed => query.OrderByDescending(e => e.ViewCount),
            QuestionSortOrder.MostVoted => query.OrderByDescending(e => e.Score),
            QuestionSortOrder.Solved => query.OrderByDescending(e => e.IsSolved),
            _ => throw new InvalidOperationException(),
        };

        var q = await query
            .Skip(skip)
            .Take(take)
            .AsSplitQuery()
            .Include(e => e.Author)
            .AsSplitQuery()
            .Include(e => e.Tags)
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
                   .Include(e => e.Answers.Where(e => !e.IsDeleted)
                                          .OrderByDescending(e => e.IsAccepted)
                                          .ThenByDescending(e => e.Score)
                                          .ThenByDescending(e => e.CreatedAt)
                                          .Take(10))
                   .ThenInclude(e => e.Author)
                   .AsSplitQuery()
                   .Include(e => e.Comments.Where(e => !e.IsDeleted)
                                          .OrderByDescending(e => e.CreatedAt)
                                          .Take(5))
                   .ThenInclude(e => e.Author)
                   .AsSplitQuery()
                   .Include(e => e.Tags)
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
            QuestionSortOrder.MostVoted => query.OrderByDescending(e => e.Score),
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

    public async Task UpdateQuestion(Question question)
    {
        question.UpdatedAt = DateTime.UtcNow;
        await _questionSearchService.IndexOrUpdateAsync(question, default);
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

    public async Task<int> CountUserScore(int userId, CancellationToken cancellationToken)
    {
        return await Table.Where(e => e.AuthorId == userId)
            .SumAsync(e => e.Score, cancellationToken);
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