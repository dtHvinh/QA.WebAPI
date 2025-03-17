using WebAPI.Model;
using WebAPI.Response.CommunityResponses;

namespace WebAPI.Utilities.Extensions;

public static class CommunityExtensions
{
    public static GetCommunityDetailResponse ToDetailResponse(this Community community)
    {
        return new GetCommunityDetailResponse()
        {
            Id = community.Id,
            Name = community.Name,
            Description = community.Description,
            IconImage = community.IconImage,
            IsPrivate = community.IsPrivate,
            Rooms = community.Rooms.Select(
                r => new ChatRoomResponse(
                    r.Id,
                    r.Name,
                    r.Messages.Select(e => new ChatMessageResponse(
                        e.Id,
                        e.Message,
                        e.CreatedAt,
                        e.UserId,
                        e.User.ToAuthorResponse()!)).ToList())).ToList()
        };
    }

    public static GetCommunityResponse ToResponse(this Community community)
    {
        return new GetCommunityResponse()
        {
            Id = community.Id,
            Name = community.Name,
            Description = community.Description,
            IconImage = community.IconImage,
            IsPrivate = community.IsPrivate
        };
    }

    public static GetCommunityDetailResponse WithMemberCount(this GetCommunityDetailResponse obj, int count)
    {
        obj.MemberCount = count;
        return obj;
    }

    public static GetCommunityDetailResponse WithIsJoined(this GetCommunityDetailResponse obj, bool isJoined)
    {
        obj.IsJoined = isJoined;
        return obj;
    }

    public static GetCommunityDetailResponse WithRoles(this GetCommunityDetailResponse obj, bool isOwner, bool isModerator)
    {
        obj.IsOwner = isOwner;
        obj.IsModerator = isModerator;
        return obj;
    }
}
