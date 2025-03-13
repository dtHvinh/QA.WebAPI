using WebAPI.CQRS;
using WebAPI.Response;
using WebAPI.Utilities;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CollectionQuestionOperationCommand(int QuestionId, int CollectionId, Operations Operation)
    : ICommand<GenericResult<TextResponse>>;
