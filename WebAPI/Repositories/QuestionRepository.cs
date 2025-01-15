using Microsoft.EntityFrameworkCore;
using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;
using WebAPI.Specification;
using WebAPI.Specification.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IQuestionRepository))]
public class QuestionRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Question>(dbContext), IQuestionRepository
{
    /// <inheritdoc/>
    public async Task<Question?> FindAvailableQuestionByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var specification = new AvailableQuestionWithTagSpec();
        var result = await Table.Where(e => e.Id == id)
                    .EvaluateQuery(specification)
                    .FirstOrDefaultAsync(cancellationToken);

        return result;
    }

    public async Task<Question?> FindQuestionBySlug(string slug, CancellationToken cancellationToken)
    {
        var specification = new AvailableQuestionWithTagSpec();
        var result = await Table.Where(e => e.Slug == slug)
                    .EvaluateQuery(specification)
                    .FirstOrDefaultAsync(cancellationToken);

        return result;
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
