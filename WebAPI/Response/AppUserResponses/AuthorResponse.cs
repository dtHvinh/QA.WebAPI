namespace WebAPI.Response.AppUserResponses;

public class AuthorResponse
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Username { get; set; } = default!;
    public int Reputation { get; set; }
    public string? ProfilePicture { get; set; }
}