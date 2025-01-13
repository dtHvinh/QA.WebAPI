using MediatR;

namespace WebAPI.CQRS;

public interface IQuery : IRequest;

public interface IQuery<TResponse> : IRequest<TResponse>;
