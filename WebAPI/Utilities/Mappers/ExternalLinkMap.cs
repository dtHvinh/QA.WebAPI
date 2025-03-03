using WebAPI.Model;
using WebAPI.Response.ExternalLinkResponses;

namespace WebAPI.Utilities.Mappers;

public static class ExternalLinkMap
{
    public static ExternalLinkResponse ToExternalLinkResponse(this ExternalLinks obj)
    {
        return new()
        {
            Id = obj.Id,
            Provider = obj.Provider,
            Url = obj.Url
        };
    }
}
