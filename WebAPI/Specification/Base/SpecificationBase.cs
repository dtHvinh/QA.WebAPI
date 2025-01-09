using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace WebAPI.Specification.Base;

public class SpecificationBase<T>(Expression<Func<T, bool>>? criteria,
                                  Expression<Func<T, object>>? include,
                                  Expression<Func<T, object>>? orderBy,
                                  Expression<Func<T, object>>? orderByDesc) : ISpecification<T> where T : class
{
    private readonly Expression<Func<T, bool>>? _criteria = criteria;
    private readonly Expression<Func<T, object>>? _include = include;
    private readonly Expression<Func<T, object>>? _orderBy = orderBy;
    private readonly Expression<Func<T, object>>? _orderByDesc = orderByDesc;

    public IQueryable<T> EvaluateQuery(IQueryable<T> query)
    {
        if (_criteria != null)
        {
            query = query.Where(_criteria);
        }

        if (_include != null)
        {
            query = query.Include(_include);
        }

        if (_orderBy != null)
        {
            query = query.OrderBy(_orderBy);
        }
        else if (_orderByDesc != null)
        {
            query = query.OrderByDescending(_orderByDesc);
        }

        return query;
    }
}
