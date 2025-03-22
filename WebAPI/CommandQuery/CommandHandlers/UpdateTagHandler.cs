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

public class UpdateTagHandler(ITagRepository tagRepository, AuthenticationContext authenticationContext, Serilog.ILogger logger)
    : ICommandHandler<UpdateTagCommand, GenericResult<TextResponse>>
{
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(
        UpdateTagCommand request, CancellationToken cancellationToken)
    {
        var existTag = await _tagRepository.FindTagWithBodyById(request.Tag.Id, cancellationToken);

        if (existTag == null)
        {
            return GenericResult<TextResponse>.Failure("Tag not found");
        }

        ArgumentNullException.ThrowIfNull(existTag.WikiBody, nameof(existTag.WikiBody));
        ArgumentNullException.ThrowIfNull(existTag.Description, nameof(existTag.Description));

        existTag.Name = ObjectExtensions.ReturnIfNotNull(request.Tag.Name, existTag.Name);
        existTag.WikiBody.Content = ObjectExtensions.ReturnIfNotNull(request.Tag.WikiBody, existTag.WikiBody.Content);
        existTag.Description.Content = ObjectExtensions.ReturnIfNotNull(request.Tag.Description, existTag.Description.Content);

        _tagRepository.Update(existTag);
        var result = await _tagRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(result.IsSuccess ? LogEventLevel.Information : LogEventLevel.Error, _authenticationContext.UserId, LogOp.Created, existTag);

        return result.IsSuccess
            ? GenericResult<TextResponse>.Success("OK")
            : GenericResult<TextResponse>.Failure(result.Message);
    }
}