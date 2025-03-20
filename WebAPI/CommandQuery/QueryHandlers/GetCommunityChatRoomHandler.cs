using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetCommunityChatRoomHandler(ICommunityRepository communityRepository)
    : IQueryHandler<GetCommunityChatRoomQuery, GenericResult<List<ChatRoomResponse>>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;

    public async Task<GenericResult<List<ChatRoomResponse>>> Handle(GetCommunityChatRoomQuery request, CancellationToken cancellationToken)
    {
        var room = await _communityRepository.GetRooms(request.CommunityId, request.PageArgs.CalculateSkip(), request.PageArgs.PageSize, cancellationToken);

        return GenericResult<List<ChatRoomResponse>>.Success(
            room.Select(e => new ChatRoomResponse(e.Id, e.Name, [])).ToList());
    }
}
