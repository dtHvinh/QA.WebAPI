namespace WebAPI.Utilities.Extensions;

public static class EndpointExtensions
{
    public static string ToEndpointPrefix(this string group)
        => $"/api/{group}";
}
