using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IQuestionHistoryRepository))]
public class QuestionHistoryRepository(ApplicationDbContext dbContext)
    : RepositoryBase<QuestionHistory>(dbContext), IQuestionHistoryRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public void AddHistory(QuestionHistory history)
    {
        Entities.Add(history);
    }

    public async Task AddHistory(int questionId, int authorId, string questionHistoryType, string comment, CancellationToken cancellationToken = default)
    {
        // The question history type should be in the database otherwise throw exception
        var historyType = await _dbContext.Set<QuestionHistoryType>()
            .FirstOrDefaultAsync(e => e.Name == questionHistoryType, cancellationToken)
            ?? throw new InvalidOperationException("History type not found");

        var history = new QuestionHistory
        {
            QuestionId = questionId,
            AuthorId = authorId,
            QuestionHistoryTypeId = historyType.Id,
            Comment = comment
        };

        Entities.Add(history);
    }

    public async Task<List<QuestionHistory>> FindHistoryWithAuthor(int questionId, CancellationToken cancellationToken)
    {
        return await Entities.Where(x => x.QuestionId == questionId).Include(e => e.Author)
            .OrderByDescending(e => e.CreationDate).ToListAsync(cancellationToken);
    }
}
