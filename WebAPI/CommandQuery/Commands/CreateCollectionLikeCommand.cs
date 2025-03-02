using System.Windows.Input;
using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CreateCollectionLikeCommand(int CollectionId) : ICommand<GenericResult<GenericResponse>>;