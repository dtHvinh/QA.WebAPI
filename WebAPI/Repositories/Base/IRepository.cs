using System.Linq.Expressions;
using WebAPI.Specification.Base;
using WebAPI.Utilities;

namespace WebAPI.Repositories.Base;

public interface IRepository<T> where T : class
{
    Task<QueryResult<T>> AddAsync(T entity, CancellationToken cancellationToken);
    Task<QueryResult> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken);
    Task<QueryResult<T>> FindFirstAsync(Expression<Func<T, bool>> predicate);
    QueryResult<IEnumerable<T>> FindAll(Func<T, bool> predicate);
    QueryResult<IEnumerable<T>> FindAll(ISpecification<T> specification);
    Task<QueryResult<T>> UpdateAsync(T entity, CancellationToken cancellationToken);
    Task<QueryResult<T>> RemoveAsync(T entity, CancellationToken cancellationToken);
}
