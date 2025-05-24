using Riok.Mapperly.Abstractions;
using WebAPI.Model;
using WebAPI.Response.ExternalLinkResponses;

namespace WebAPI.Utilities.Extensions;

[Mapper]
public static partial class ExternalLinkExtensions
{
    public static partial ExternalLinkResponse ToExternalLinkResponse(this ExternalLinks source);
}
