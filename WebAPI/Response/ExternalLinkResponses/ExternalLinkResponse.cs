namespace WebAPI.Response.ExternalLinkResponses;

public class ExternalLinkResponse
{
    public int Id { get; set; }
    public string Provider { get; set; } = default!;
    public string Url { get; set; } = default!;
}
