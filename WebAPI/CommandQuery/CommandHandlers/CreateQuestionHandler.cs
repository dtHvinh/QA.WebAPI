using CQRS;
using WebAPI.CommandQuery.Commands;
using WebAPI.Dto;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateQuestionHandler : ICommandHandler<CreateQuestionCommand, ResultBase<CreateQuestionDto>>
{
    public Task<ResultBase<CreateQuestionDto>> Handle(CreateQuestionCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
