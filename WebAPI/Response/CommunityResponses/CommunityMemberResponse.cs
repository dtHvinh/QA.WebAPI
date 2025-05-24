namespace WebAPI.Response.CommunityResponses;

public class CommunityMemberResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = default!;
    public string ProfileImage { get; set; } = default!;
    public bool IsOwner { get; set; }
    public bool IsModerator { get; set; }
}
