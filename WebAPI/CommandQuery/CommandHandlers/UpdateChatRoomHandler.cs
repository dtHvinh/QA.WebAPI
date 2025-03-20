using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateChatRoomHandler(
    ICommunityRepository repository,
    Serilog.ILogger logger,
    AuthenticationContext authenticationContext)
    : ICommandHandler<UpdateChatRoomCommand, GenericResult<TextResponse>>
{
    private readonly ICommunityRepository _repository = repository;
    private readonly Serilog.ILogger _logger = logger;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;

    public async Task<GenericResult<TextResponse>> Handle(UpdateChatRoomCommand request, CancellationToken cancellationToken)
    {
        var chatRoom = await _repository.GetRoomAsync(request.Dto.Id, cancellationToken);

        if (chatRoom is null)
            return GenericResult<TextResponse>.Failure("Chat room not found.");

        chatRoom.Name = request.Dto.Name;

        _repository.UpdateRoom(chatRoom);

        var res = await _repository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess
            ? Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error,
            _authenticationContext.UserId,
            LogOp.Updated,
            chatRoom);

        return GenericResult<TextResponse>.Success(new TextResponse("Chat room updated."));
    }
}
