using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Logging;
using WebAPI.Utilities.Result.Base;
using WebAPI.Utilities.Services;

namespace WebAPI.CommandQuery.CommandHandlers;

public class UpdateCommunityHandler(
    ICommunityRepository communityRepository,
    StorageService storageService,
    AuthenticationContext authenticationContext,
    Serilog.ILogger logger)
    : ICommandHandler<UpdateCommunityCommand, GenericResult<TextResponse>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly StorageService _storageService = storageService;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly Serilog.ILogger _logger = logger;

    public async Task<GenericResult<TextResponse>> Handle(UpdateCommunityCommand request, CancellationToken cancellationToken)
    {
        var community = await _communityRepository.FindFirstAsync(e => e.Id == request.Dto.Id, cancellationToken);

        if (community == null)
            return GenericResult<TextResponse>.Failure("Community not found");

        if (request.Dto.IconImage != null)
        {
            var imagePath = await _storageService.UploadCommunityIcon(community.Name, request.Dto.IconImage, cancellationToken);
            community.IconImage = imagePath;
        }
        community.Name = request.Dto.Name;
        community.Description = request.Dto.Description;
        community.IsPrivate = request.Dto.IsPrivate;
        community.ModificationDate = DateTime.UtcNow;

        _communityRepository.Update(community);

        var res = await _communityRepository.SaveChangesAsync(cancellationToken);

        _logger.UserAction(res.IsSuccess
            ? Serilog.Events.LogEventLevel.Information
            : Serilog.Events.LogEventLevel.Error, _authenticationContext.UserId, LogOp.Updated, community);

        return res.IsSuccess
            ? GenericResult<TextResponse>.Success(new TextResponse("Community updated successfully"))
            : GenericResult<TextResponse>.Failure(res.Message);
    }
}
