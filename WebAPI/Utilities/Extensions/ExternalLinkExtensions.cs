using WebAPI.Model;
using WebAPI.Response.ExternalLinkResponses;

namespace WebAPI.Utilities.Extensions;

public static class ExternalLinkExtensions
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
