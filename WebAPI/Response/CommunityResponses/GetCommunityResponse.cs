namespace WebAPI.Response.CommunityResponses;

public class GetCommunityResponse
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public string? Description { get; set; }
    public string? IconImage { get; set; }

    public bool IsPrivate { get; set; } = false;
    public bool IsJoined { get; set; } = false;

    public int MemberCount { get; set; }
}
