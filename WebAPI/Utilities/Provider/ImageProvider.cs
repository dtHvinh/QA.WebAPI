namespace WebAPI.Utilities.Provider;

public class ImageProvider(string providerUrl)
{
    private readonly string _providerUrl = providerUrl;

    /// <summary>
    /// Get the default profile picture for a user.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public string GetDefaultPFP(string name)
    {
        return _providerUrl + "/?name=" + name + "&background=random";
    }
}
