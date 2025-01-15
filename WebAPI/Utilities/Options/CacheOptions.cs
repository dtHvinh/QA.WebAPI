namespace WebAPI.Utilities.Options;

public class CacheOptions
{
    public CacheOptionDetails AppUser { get; set; }
    public CacheOptionDetails Question { get; set; }
}

public record struct CacheOptionDetails
{
    public int AE { get; set; }
    public int SE { get; set; }
}
