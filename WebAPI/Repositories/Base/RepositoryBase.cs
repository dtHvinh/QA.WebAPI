
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebAPI.Data;
using WebAPI.Specification.Base;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories.Base;

public class RepositoryBase<T>(ApplicationDbContext dbContext) : IRepositoryBase<T> where T : class
{
    protected DbSet<T> Entities => dbContext.Set<T>();
    protected IQueryable<T> Table => Entities;

    public async Task<OperationResult<T>> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            var e = Entities.Add(entity).Entity;
            return OperationResult<T>.Success(e);
        }
        catch (Exception ex)
        {
            return OperationResult<T>.Failure(ex.Message);
        }
    }

    public async Task<OperationResult> AddRangeAsync(IEnumerable<T> entities,
                                                CancellationToken cancellationToken = default)
    {
        try
        {
            Entities.AddRange(entities);
            await dbContext.SaveChangesAsync(cancellationToken);
            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ex.Message);
        }
    }

    public OperationResult<IEnumerable<T>> FindAll(Func<T, bool> predicate)
    {
        try
        {
            var result = Table.Where(predicate);
            return OperationResult<IEnumerable<T>>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<IEnumerable<T>>.Failure(ex.Message);
        }
    }

    public OperationResult<IEnumerable<T>> FindAll(ISpecification<T> specification)
    {
        try
        {
            var result = specification.EvaluateQuery(Table);
            if (!result.Any())
                return OperationResult<IEnumerable<T>>.Failure("No entities found.");

            return OperationResult<IEnumerable<T>>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<IEnumerable<T>>.Failure(ex.Message);
        }
    }

    public async Task<OperationResult<T>> FindFirstAsync(Expression<Func<T, bool>> predicate,
                                                    CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await Table.FirstOrDefaultAsync(predicate, cancellationToken);
            if (result == null)
            {
                return OperationResult<T>.Failure("Entity not found.");
            }

            return OperationResult<T>.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult<T>.Failure(ex.Message);
        }
    }

    public async Task<OperationResult<T>> RemoveAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            Entities.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return OperationResult<T>.Success(entity);
        }
        catch (Exception ex)
        {
            return OperationResult<T>.Failure(ex.Message);
        }
    }

    public async Task<OperationResult<T>> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            Entities.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return OperationResult<T>.Success(entity);
        }
        catch (Exception ex)
        {
            return OperationResult<T>.Failure(ex.Message);
        }
    }

    public async Task<OperationResult> SaveChangeAsync()
    {
        try
        {
            await dbContext.SaveChangesAsync();
            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ex.Message);
        }
    }
}
