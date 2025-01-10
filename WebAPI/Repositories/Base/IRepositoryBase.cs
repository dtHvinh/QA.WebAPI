using System.Linq.Expressions;
using WebAPI.Specification.Base;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories.Base;

public interface IRepositoryBase<T> where T : class
{
    Task<ResultBase<T>> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<ResultBase> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<ResultBase<T>> FindFirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    ResultBase<IEnumerable<T>> FindAll(Func<T, bool> predicate);
    ResultBase<IEnumerable<T>> FindAll(ISpecification<T> specification);
    Task<ResultBase<T>> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<ResultBase<T>> RemoveAsync(T entity, CancellationToken cancellationToken = default);
}
