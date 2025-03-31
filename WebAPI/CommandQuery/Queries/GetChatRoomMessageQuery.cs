using WebAPI.CQRS;
using WebAPI.Pagination;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Queries;

public record GetChatRoomMessageQuery(int RoomId, PageArgs PageArgs)
    : IQuery<GenericResult<PagedResponse<ChatMessageResponse>>>;
