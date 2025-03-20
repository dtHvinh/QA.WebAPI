namespace WebAPI.Response.CommunityResponses;

public record CommunityMemberResponse(int Id, string Username, string ProfileImage, bool IsModerator);
