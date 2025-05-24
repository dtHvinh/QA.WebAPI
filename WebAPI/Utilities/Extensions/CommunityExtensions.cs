using Riok.Mapperly.Abstractions;
using WebAPI.Model;
using WebAPI.Response.CommunityResponses;
using static WebAPI.Repositories.CommunityRepository;

namespace WebAPI.Utilities.Extensions;

[Mapper]
public static partial class CommunityExtensions
{
    public static partial GetCommunityDetailResponse ToDetailResponse(this Community source);
    public static partial GetCommunityResponse ToResponse(this Community source);

    [MapProperty($"User.Id", nameof(CommunityMemberResponse.Id))]
    [MapProperty($"User.UserName", nameof(CommunityMemberResponse.UserName))]
    [MapProperty($"User.ProfilePicture", nameof(CommunityMemberResponse.ProfileImage))]
    public static partial CommunityMemberResponse ToResponse(this CommunityMember source);
    public static partial GetCommunityResponse ToResponse(this CommunityWithJoinStatus source);

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

    public static GetCommunityResponse WithIsJoined(this GetCommunityResponse obj, bool isJoined)
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
