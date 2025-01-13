using MediatR;

namespace WebAPI.CQRS;

public interface ICommand : IRequest;

public interface ICommand<TResponse> : IRequest<TResponse>;
