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
    public void AddHistory(QuestionHistory history)
    {
        Entities.Add(history);
    }

    public void AddHistory(int questionId, int authorId, string questionHistoryType, string comment)
    {
        var history = new QuestionHistory
        {
            QuestionId = questionId,
            AuthorId = authorId,
            QuestionHistoryType = questionHistoryType,
            Comment = comment
        };

        Entities.Add(history);
    }

    public async Task<List<QuestionHistory>> FindHistoryWithAuthor(int questionId, CancellationToken cancellationToken)
    {
        return await Entities.Where(x => x.QuestionId == questionId).Include(e => e.Author)
            .OrderByDescending(e => e.CreatedAt).ToListAsync(cancellationToken);
    }
}
