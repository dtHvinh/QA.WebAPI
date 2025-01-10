
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

    public async Task<ResultBase<T>> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            var e = Entities.Add(entity).Entity;
            await dbContext.SaveChangesAsync(cancellationToken);
            return ResultBase<T>.Success(e);
        }
        catch (Exception ex)
        {
            return ResultBase<T>.Failure(ex.Message);
        }
    }

    public async Task<ResultBase> AddRangeAsync(IEnumerable<T> entities,
                                                CancellationToken cancellationToken = default)
    {
        try
        {
            Entities.AddRange(entities);
            await dbContext.SaveChangesAsync(cancellationToken);
            return ResultBase.Success();
        }
        catch (Exception ex)
        {
            return ResultBase.Failure(ex.Message);
        }
    }

    public ResultBase<IEnumerable<T>> FindAll(Func<T, bool> predicate)
    {
        try
        {
            var result = Table.Where(predicate);
            return ResultBase<IEnumerable<T>>.Success(result);
        }
        catch (Exception ex)
        {
            return ResultBase<IEnumerable<T>>.Failure(ex.Message);
        }
    }

    public ResultBase<IEnumerable<T>> FindAll(ISpecification<T> specification)
    {
        try
        {
            var result = specification.EvaluateQuery(Table);
            if (!result.Any())
                return ResultBase<IEnumerable<T>>.Failure("No entities found.");

            return ResultBase<IEnumerable<T>>.Success(result);
        }
        catch (Exception ex)
        {
            return ResultBase<IEnumerable<T>>.Failure(ex.Message);
        }
    }

    public async Task<ResultBase<T>> FindFirstAsync(Expression<Func<T, bool>> predicate,
                                                    CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await Table.FirstOrDefaultAsync(predicate, cancellationToken);
            if (result == null)
            {
                return ResultBase<T>.Failure("Entity not found.");
            }

            return ResultBase<T>.Success(result);
        }
        catch (Exception ex)
        {
            return ResultBase<T>.Failure(ex.Message);
        }
    }

    public async Task<ResultBase<T>> RemoveAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            Entities.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return ResultBase<T>.Success(entity);
        }
        catch (Exception ex)
        {
            return ResultBase<T>.Failure(ex.Message);
        }
    }

    public async Task<ResultBase<T>> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        try
        {
            Entities.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return ResultBase<T>.Success(entity);
        }
        catch (Exception ex)
        {
            return ResultBase<T>.Failure(ex.Message);
        }
    }
}
