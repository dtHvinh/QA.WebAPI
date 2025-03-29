using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record ChatCommand(ChatRequestDto Dto)
    : ICommand<GenericResult<ChatMessageResponse>>;
