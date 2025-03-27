namespace WebAPI.Utilities;

public static class MarkupText
{
    public static string Link(string url, string text = "")
    {
        return $"<a href=\"{url}\">{text}</a>";
    }
}
