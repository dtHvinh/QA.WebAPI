using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record DeleteQuestionCollectionCommand(int Id) : ICommand<GenericResult<GenericResponse>>;
