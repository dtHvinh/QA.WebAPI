using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(ICommentRepository))]
public class CommentRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Comment>(dbContext), ICommentRepository
{
    public void AddComment(Comment comment)
    {
        Entities.Add(comment);
    }
}
