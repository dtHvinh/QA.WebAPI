namespace WebAPI.Response.CollectionResponses;

public class GetCollectionWithAddStatusResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsPublic { get; set; }
    public bool IsAdded { get; set; }
}
