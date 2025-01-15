using WebAPI.Attributes;
using WebAPI.Data;
using WebAPI.Model;
using WebAPI.Repositories.Base;

namespace WebAPI.Repositories;

[RepositoryImpl(typeof(IUpvoteRepository))]
public class UpvoteRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Upvote>(dbContext), IUpvoteRepository
{
}
