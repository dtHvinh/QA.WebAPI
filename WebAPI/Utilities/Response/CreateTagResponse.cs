namespace WebAPI.Utilities.Response;

public record CreateTagResponse(string Name, string Description)
{
    public static CreateTagResponse Create(string name, string description)
    {
        return new CreateTagResponse(name, description);
    }
}
