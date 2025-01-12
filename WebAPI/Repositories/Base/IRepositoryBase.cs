using System.Linq.Expressions;
using WebAPI.Specification.Base;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories.Base;

public interface IRepositoryBase<T> where T : class
{
    Task<OperationResult<T>> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<OperationResult> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task<OperationResult<T>> FindFirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    OperationResult<IEnumerable<T>> FindAll(Func<T, bool> predicate);
    OperationResult<IEnumerable<T>> FindAll(ISpecification<T> specification);
    Task<OperationResult<T>> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<OperationResult<T>> RemoveAsync(T entity, CancellationToken cancellationToken = default);
}
