namespace WebAPI.Utilities.Response.TagResponses;

public record struct CreateTagResponse(Guid Id, string Name, string Description);
