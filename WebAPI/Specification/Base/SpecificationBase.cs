using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace WebAPI.Specification.Base;

public class SpecificationBase<T> : ISpecification<T> where T : class
{
    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public Expression<Func<T, object>>? OrderByDesc { get; private set; }
    public Expression<Func<T, object>>? Include { get; private set; }

    public SpecificationBase()
    {

    }

    public SpecificationBase(Expression<Func<T, bool>>? criteria,
                             Expression<Func<T, object>>? include,
                             Expression<Func<T, object>>? orderBy,
                             Expression<Func<T, object>>? orderByDesc)
    {
        Criteria = criteria;
        Include = include;
        OrderBy = orderBy;
        OrderByDesc = orderByDesc;
    }

    public SpecificationBase<T> AddCriteria(Expression<Func<T, bool>> criteria)
    {
        Criteria = criteria;
        return this;
    }

    public SpecificationBase<T> AddInclude(Expression<Func<T, object>> selector)
    {
        Include = selector;
        return this;
    }

    public SpecificationBase<T> AddOrderBy(Expression<Func<T, object>> selector)
    {
        OrderBy = selector;
        return this;
    }

    public SpecificationBase<T> AddOrderByDesc(Expression<Func<T, object>> selector)
    {
        OrderByDesc = selector;
        return this;
    }

    public virtual IQueryable<T> EvaluateQuery(IQueryable<T> query)
    {
        if (Criteria != null)
        {
            query = query.Where(Criteria);
        }

        if (Include != null)
        {
            query = query.Include(Include);
        }

        if (OrderBy != null)
        {
            query = query.OrderBy(OrderBy);
        }
        else if (OrderByDesc != null)
        {
            query = query.OrderByDescending(OrderByDesc);
        }

        return query;
    }
}

public static class SpecificationExtensions
{
    public static IQueryable<T> EvaluateQuery<T>(this IQueryable<T> query, ISpecification<T> specification)
    {
        return specification.EvaluateQuery(query);
    }
}
