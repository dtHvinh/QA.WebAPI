using System.Linq.Expressions;

namespace WebAPI.ElasticSearch;

public record ElasticStringSearchParams<T>(
    Expression<Func<T, object>> Selector,
    Expression<Func<T, object>>? SortSelector,
    string Value, int From, int Size);