namespace WebAPI.Specification.Base;

public interface ISpecification<T>
{
    IQueryable<T> EvaluateQuery(IQueryable<T> query);
}
