namespace WebAPI.Response.CommunityResponses;

public class GetCommunityDetailResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? IconImage { get; set; }
    public bool IsPrivate { get; set; }
    public int MemberCount { get; set; }
    public bool IsJoined { get; set; }
    public List<ChatRoomResponse> Rooms { get; set; }
    public bool IsOwner { get; set; }
    public bool IsModerator { get; set; }
}
