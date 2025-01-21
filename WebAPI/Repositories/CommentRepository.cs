using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(ICommentRepository))]
public class CommentRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Comment>(dbContext), ICommentRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public void AddComment(Comment comment)
    {
        Entities.Add(comment);
    }

    public int CountQuestionComment(Guid questionId)
    {
        return _dbContext.Set<QuestionComment>().Where(e => e.QuestionId.Equals(questionId))
             .Count();
    }
}
