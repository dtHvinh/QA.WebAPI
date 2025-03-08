using Serilog.Events;
using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.CommandHandlers;

public class CreateTagHandler(
    ITagRepository tagRepository,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<CreateTagCommand, GenericResult<GenericResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<GenericResponse>> Handle(
        CreateTagCommand request, CancellationToken cancellationToken)
    {
        var newTag = request.Tag.ToTag();

        _tagRepository.CreateTag(newTag);

        var createTag = await _tagRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(createTag.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authenticationContext.UserId, LogOp.Created, newTag);

        return createTag.IsSuccess
            ? GenericResult<GenericResponse>.Success("Ok")
            : GenericResult<GenericResponse>.Failure(createTag.Message);
    }
}