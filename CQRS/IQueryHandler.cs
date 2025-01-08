using MediatR;

namespace CQRS;

public interface IQueryHandler<TQuery> : IRequestHandler<TQuery> where TQuery : IQuery;

public interface IQueryHandler<TQuery, TResponse>
    : IRequestHandler<TQuery, TResponse> where TQuery : IQuery<TResponse>;
