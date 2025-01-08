using MediatR;

namespace CQRS;

public interface ICommand : IRequest;

public interface ICommand<TResponse> : IRequest<TResponse>;
