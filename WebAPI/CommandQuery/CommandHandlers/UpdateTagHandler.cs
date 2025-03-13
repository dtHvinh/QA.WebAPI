using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateTagHandler(ITagRepository tagRepository, AuthenticationContext authenticationContext, Serilog.ILogger logger)
    : ICommandHandler<UpdateTagCommand, GenericResult<TextResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(
        UpdateTagCommand request, CancellationToken cancellationToken)
    {
        if (!await _authenticationContext.IsModerator())
            return GenericResult<TextResponse>.Failure(string.Format(Constants.EM.ROLE_NOT_MEET_REQ,
                nameof(Constants.Roles.Moderator)));

        var newTag = request.Tag.ToTag();

        _tagRepository.Update(newTag);
        var result = await _tagRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(result.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authenticationContext.UserId, LogOp.Created, newTag);

        return !result.IsSuccess
            ? GenericResult<TextResponse>.Failure(result.Message)
            : GenericResult<TextResponse>.Success("OK");
    }
}