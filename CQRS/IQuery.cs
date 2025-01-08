using MediatR;

namespace CQRS;

public interface IQuery : IRequest;

public interface IQuery<TResponse> : IRequest<TResponse>;
