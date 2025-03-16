using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Response.CommunityResponses;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record CreateCommunityCommand(CreateCommunityDto CreateDto)
    : ICommand<GenericResult<CreateCommunityResponse>>;
