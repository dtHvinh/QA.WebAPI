namespace WebAPI.Response.TagResponses;

public record struct CreateTagResponse(int Id, string Name, string Description, string WikiBody);
