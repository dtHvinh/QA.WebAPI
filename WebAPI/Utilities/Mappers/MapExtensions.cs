namespace WebAPI.Utilities.Mappers;

public static class MapExtensions
{
    public static string GenerateSlug(this string title)
    {
        return title.Trim().ToLower().Replace(" ", "-");
    }
}
