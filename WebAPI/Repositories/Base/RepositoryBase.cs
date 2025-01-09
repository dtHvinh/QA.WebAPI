
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebAPI.Data;
using WebAPI.Specification.Base;
using WebAPI.Utilities;

namespace WebAPI.Repositories.Base;

public class RepositoryBase<T>(ApplicationDbContext dbContext) : IRepository<T> where T : class
{
    protected DbSet<T> Entities => dbContext.Set<T>();
    protected IQueryable<T> Table => Entities;

    public async Task<QueryResult<T>> AddAsync(T entity, CancellationToken cancellationToken)
    {
        try
        {
            var e = Entities.Add(entity).Entity;
            await dbContext.SaveChangesAsync(cancellationToken);
            return QueryResult<T>.Success(e);
        }
        catch (Exception ex)
        {
            return QueryResult<T>.Failure(ex.Message);
        }
    }

    public async Task<QueryResult> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken)
    {
        try
        {
            Entities.AddRange(entities);
            await dbContext.SaveChangesAsync(cancellationToken);
            return QueryResult.Success();
        }
        catch (Exception ex)
        {
            return QueryResult.Failure(ex.Message);
        }
    }

    public QueryResult<IEnumerable<T>> FindAll(Func<T, bool> predicate)
    {
        try
        {
            var result = Table.Where(predicate);
            return QueryResult<IEnumerable<T>>.Success(result);
        }
        catch (Exception ex)
        {
            return QueryResult<IEnumerable<T>>.Failure(ex.Message);
        }
    }

    public QueryResult<IEnumerable<T>> FindAll(ISpecification<T> specification)
    {
        try
        {
            var result = specification.EvaluateQuery(Table);
            if (!result.Any())
                return QueryResult<IEnumerable<T>>.Failure("No entities found.");

            return QueryResult<IEnumerable<T>>.Success(result);
        }
        catch (Exception ex)
        {
            return QueryResult<IEnumerable<T>>.Failure(ex.Message);
        }
    }

    public async Task<QueryResult<T>> FindFirstAsync(
        Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        try
        {
            var result = await Table.FirstOrDefaultAsync(predicate, cancellationToken);
            if (result == null)
            {
                return QueryResult<T>.Failure("Entity not found.");
            }

            return QueryResult<T>.Success(result);
        }
        catch (Exception ex)
        {
            return QueryResult<T>.Failure(ex.Message);
        }
    }

    public Task<QueryResult<T>> FindFirstAsync(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public async Task<QueryResult<T>> RemoveAsync(T entity, CancellationToken cancellationToken)
    {
        try
        {
            Entities.Remove(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return QueryResult<T>.Success(entity);
        }
        catch (Exception ex)
        {
            return QueryResult<T>.Failure(ex.Message);
        }
    }

    public async Task<QueryResult<T>> UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        try
        {
            Entities.Update(entity);
            await dbContext.SaveChangesAsync(cancellationToken);

            return QueryResult<T>.Success(entity);
        }
        catch (Exception ex)
        {
            return QueryResult<T>.Failure(ex.Message);
        }
    }
}
