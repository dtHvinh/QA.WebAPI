using WebAPI.CommandQuery.Commands;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response;
using WebAPI.Utilities.Context;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Result.Base;
using WebAPI.Utilities.Services;

namespace WebAPI.CommandQuery.CommandHandlers;

public class DeleteCommunityHandler(
    ICommunityRepository communityRepository,
    AuthenticationContext authenticationContext,
    ICacheService cacheService,
    StorageService storageService)
    : ICommandHandler<DeleteCommunityCommand, GenericResult<TextResponse>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;
    private readonly AuthenticationContext _authenticationContext = authenticationContext;
    private readonly ICacheService _cacheService = cacheService;
    private readonly StorageService _storageService = storageService;

    public async Task<GenericResult<TextResponse>> Handle(DeleteCommunityCommand request, CancellationToken cancellationToken)
    {
        var community = await _communityRepository.FindFirstAsync(e => e.Id == request.CommunityId, cancellationToken);

        if (community is null)
            return GenericResult<TextResponse>.Failure("Community not found");

        if (!await _communityRepository.IsOwner(_authenticationContext.UserId, community.Id, cancellationToken))
            return GenericResult<TextResponse>.Failure("You are not the owner of this community");

        _communityRepository.Remove(community);

        if (community.IconImage is not null)
            await _storageService.Delete(community.IconImage);

        await _cacheService.FreeCommunityName(community.Name, cancellationToken);

        var result = await _communityRepository.SaveChangesAsync(cancellationToken);

        return result.IsSuccess
            ? GenericResult<TextResponse>.Success("Community deleted successfully")
            : GenericResult<TextResponse>.Failure("Failed to delete community");
    }
}
