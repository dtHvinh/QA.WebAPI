using Microsoft.Extensions.Caching.Distributed;
using System.Globalization;
using System.Text.Json;
using WebAPI.Attributes;
using WebAPI.Utilities.Contract;

namespace WebAPI.Utilities.Services;

[Dependency(typeof(ICacheService), ServiceLifetime.Singleton)]
public class CacheService(IDistributedCache cache,
                          ICacheOptionProvider cacheOptionProvider,
                          JsonSerializerOptions options) : ICacheService
{
    private readonly IDistributedCache cache = cache;
    private readonly ICacheOptionProvider _cacheOptionProvider = cacheOptionProvider;
    private readonly JsonSerializerOptions _jsonOptions = options;

    public async Task<bool> BanUserAsync(int userId, DateTime to, CancellationToken cancellationToken)
    {
        if (to < DateTime.UtcNow)
        {
            return false;
        }

        await cache.SetStringAsync($"ban:{userId}", to.ToString(), new()
        {
            AbsoluteExpiration = to
        }, cancellationToken);

        return true;
    }

    public async Task<DateTime?> IsBanned(int userId, CancellationToken cancellationToken)
    {
        var banDate = await cache.GetStringAsync($"ban:{userId}", cancellationToken);
        return banDate is null ? null : DateTime.Parse(banDate, CultureInfo.InvariantCulture);
    }

    public async Task SetUsedEmail(string email, CancellationToken cancellationToken)
    {
        await cache.SetStringAsync($"email:{email}", "1", _cacheOptionProvider.GetDefault(), cancellationToken);
    }
}
