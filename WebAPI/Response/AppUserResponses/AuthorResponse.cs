namespace WebAPI.Response.AppUserResponses;

public class AuthorResponse
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Username { get; set; }
    public int Reputation { get; set; }
    public string ProfilePicture { get; set; }
}