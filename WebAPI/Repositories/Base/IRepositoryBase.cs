using System.Linq.Expressions;
using WebAPI.Specification.Base;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories.Base;

public interface IRepositoryBase<T> where T : class
{
    void Add(T entity);
    void AddRange(IEnumerable<T> entities);
    IEnumerable<T> FindAll(Func<T, bool> predicate);
    IEnumerable<T> FindAll(ISpecification<T> specification);
    Task<T?> FindFirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    void Remove(T entity);
    void Update(T entity);
    Task<OperationResult> SaveChangeAsync(CancellationToken cancellationToken);
}
