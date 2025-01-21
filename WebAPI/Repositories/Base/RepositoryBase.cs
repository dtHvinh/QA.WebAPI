
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebAPI.Data;
using WebAPI.Specification.Base;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.Repositories.Base;

public class RepositoryBase<T>(ApplicationDbContext dbContext) : IRepository<T> where T : class
{
    protected DbSet<T> Entities => dbContext.Set<T>();
    protected IQueryable<T> Table => Entities;

    public async Task<GenericResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return GenericResult.Success();
        }
        catch (Exception ex)
        {
            return GenericResult.Failure(ex.InnerException?.Message ?? ex.Message);
        }
    }

    public void Add(T entity)
    {
        Entities.Add(entity);
    }

    public void AddRange(IEnumerable<T> entities)
    {
        Entities.AddRange(entities);
    }

    public IEnumerable<T> FindAll(Func<T, bool> predicate)
    {
        return Table.Where(predicate);
    }

    public IEnumerable<T> FindAll(ISpecification<T> specification)
    {
        return specification.EvaluateQuery(Table);
    }

    public async Task<T?> FindFirstAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var entity = await Table.FirstOrDefaultAsync(predicate, cancellationToken);
        return entity;
    }

    public void Remove(T entity)
    {
        Entities.Remove(entity);
    }

    public void Update(T entity)
    {
        Entities.Update(entity);
    }

    public void UpdateRange(T[] entities)
    {
        Entities.UpdateRange(entities);
    }

    public void UpdateRange(IEnumerable<T> entities)
    {
        Entities.UpdateRange(entities);
    }

    public int CountAll()
    {
        return Entities.Count();
    }
}
