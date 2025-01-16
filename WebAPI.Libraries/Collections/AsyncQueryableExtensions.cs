using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace WebAPI.Libraries.Collections;
#pragma warning disable S2953
#pragma warning disable CS1998 // Nothing to await
#pragma warning disable CS8603 
#pragma warning disable S125 
/**
 * Source: https://stackoverflow.com/questions/48743165/toarrayasync-throws-the-source-iqueryable-doesnt-implement-iasyncenumerable
 */
public static class AsyncQueryableExtensions
{
    /// <summary>
    /// Returns the input typed as IQueryable that can be queried asynchronously
    /// </summary>
    /// <typeparam name="TEntity">The item type</typeparam>
    /// <param name="source">The input</param>
    public static IQueryable<TEntity> AsAsyncQueryable<TEntity>(this IEnumerable<TEntity> source)
        => new DefaultAsyncQueryable<TEntity>(source ?? throw new ArgumentNullException(nameof(source)));
}

public class DefaultAsyncQueryable<TEntity>
    : EnumerableQuery<TEntity>, IAsyncEnumerable<TEntity>, IQueryable<TEntity>
{
    public DefaultAsyncQueryable(IEnumerable<TEntity> enumerable) : base(enumerable) { }
    public DefaultAsyncQueryable(Expression expression) : base(expression) { }
    public IAsyncEnumerator<TEntity> GetEnumerator() =>
        new AsyncEnumerator(this.AsEnumerable().GetEnumerator());
    public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new AsyncEnumerator(this.AsEnumerable().GetEnumerator());
    IQueryProvider IQueryable.Provider => new DefaultAsyncQueryProvider(this);


    internal class AsyncEnumerator(IEnumerator<TEntity> inner) : IAsyncEnumerator<TEntity>
    {
        private readonly IEnumerator<TEntity> inner = inner;
        public void Dispose() => inner.Dispose();
        public TEntity Current => inner.Current;
        public ValueTask<bool> MoveNextAsync() => new(inner.MoveNext());
        public async ValueTask DisposeAsync() => inner.Dispose();
    }

    internal class DefaultAsyncQueryProvider : IAsyncQueryProvider
    {
        private readonly IQueryProvider inner;
        internal DefaultAsyncQueryProvider(IQueryProvider inner) => this.inner = inner;
        public IQueryable CreateQuery(Expression expression) => new DefaultAsyncQueryable<TEntity>(expression);
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new DefaultAsyncQueryable<TElement>(expression);
        public object Execute(Expression expression) => inner.Execute(expression);
        public TResult Execute<TResult>(Expression expression) => inner.Execute<TResult>(expression);
        //public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) => new DefaultAsyncQueryable<TResult>(expression);
        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) => Execute<TResult>(expression);
    }
}
