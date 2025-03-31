using WebAPI.CommandQuery.Queries;
using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Repositories.Base;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Extensions;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.QueryHandlers;

public class GetChatRoomMessageHandler(ICommunityRepository communityRepository)
    : IQueryHandler<GetChatRoomMessageQuery, GenericResult<PagedResponse<ChatMessageResponse>>>
{
    private readonly ICommunityRepository _communityRepository = communityRepository;

    public async Task<GenericResult<PagedResponse<ChatMessageResponse>>> Handle(GetChatRoomMessageQuery request, CancellationToken cancellationToken)
    {
        var messages = await _communityRepository.GetChatRoomMessagesWithAuthor(request.RoomId,
            request.PageArgs.CalculateSkip(), request.PageArgs.PageSize + 1, cancellationToken);

        var hasNext = messages.Count > request.PageArgs.PageSize;

        return GenericResult<PagedResponse<ChatMessageResponse>>.Success(new PagedResponse<ChatMessageResponse>(
            items: messages.TakeLast(request.PageArgs.PageSize).Select(e => e.ToResponseWithAuthor()),
            hasNext,
            request.PageArgs.PageIndex,
            request.PageArgs.PageSize));
    }
}
