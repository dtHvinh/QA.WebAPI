using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using WebAPI.Attributes;
using WebAPI.Model;
using WebAPI.Utilities.Contract;
using WebAPI.Utilities.Options;

namespace WebAPI.Utilities.Provider;

[Implement(typeof(ICacheOptionProvider), ServiceLifetime.Singleton)]
public class CacheOptionProvider(IOptions<CacheOptions> options) : ICacheOptionProvider
{
    private readonly CacheOptions _options = options.Value;

    public DistributedCacheEntryOptions GetDefault()
    {
        return new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
        };
    }

    public DistributedCacheEntryOptions GetOptions(string type)
    {
        return type switch
        {
            nameof(AppUser) => new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.AppUser.AE),
                SlidingExpiration = TimeSpan.FromSeconds(_options.AppUser.SE),
            },
            nameof(Question) => new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.Question.AE),
                SlidingExpiration = TimeSpan.FromSeconds(_options.Question.SE),
            },
            _ => GetDefault()
        };
    }

}
