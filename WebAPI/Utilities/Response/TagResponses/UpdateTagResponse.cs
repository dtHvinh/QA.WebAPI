namespace WebAPI.Utilities.Response.TagResponses;

public record UpdateTagResponse(string Name, string Description)
{
    public static UpdateTagResponse Create(string name, string description)
    {
        return new UpdateTagResponse(name, description);
    }
}
