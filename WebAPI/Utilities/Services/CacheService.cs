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

    public async Task<bool> BanUserAsync(int userId, DateTime to, string reason, CancellationToken cancellationToken)
    {
        if (to < DateTime.UtcNow)
        {
            return false;
        }

        await cache.SetStringAsync($"ban:{userId}", to.ToString() + "$$$" + reason, new()
        {
            AbsoluteExpiration = to
        }, cancellationToken);

        return true;
    }

    public async Task Unban(int userId, CancellationToken cancellationToken)
    {
        await cache.RemoveAsync($"ban:{userId}", cancellationToken);
    }

    public async Task<DateTime?> IsBanned(int userId, CancellationToken cancellationToken)
    {
        var banStr = await cache.GetStringAsync($"ban:{userId}", cancellationToken);

        if (banStr is null)
        {
            return null;
        }

        var banDate = banStr.Split("$$$")[0];
        return banDate is null ? null : DateTime.Parse(banDate, CultureInfo.CurrentCulture);
    }

    public bool IsBanned(int userId)
    {
        return cache.GetString($"ban:{userId}") != null;
    }

    public async Task<(DateTime, string)?> IsBannedWithReason(int userId, CancellationToken cancellationToken)
    {
        var banStr = await cache.GetStringAsync($"ban:{userId}", cancellationToken);
        if (banStr is null)
        {
            return null;
        }

        var banDate = banStr.Split("$$$")[0];
        var banReason = banStr.Split("$$$")[1];

        return (DateTime.Parse(banDate, CultureInfo.CurrentCulture), banReason);
    }

    public async Task<bool> IsCommunityNameUsed(string communityName, CancellationToken cancellationToken)
    {
        return (await cache.GetStringAsync($"community:{communityName}", cancellationToken)) != null;
    }

    public async Task FreeCommunityName(string communityName, CancellationToken cancellationToken)
    {
        await cache.RemoveAsync($"community:{communityName}", cancellationToken);
    }

    public async Task SetUsedCommunity(string communityName, CancellationToken cancellationToken)
    {
        await cache.SetStringAsync($"community:{communityName}", "1", cancellationToken);
    }

    public async Task SetUsedEmail(string email, CancellationToken cancellationToken)
    {
        await cache.SetStringAsync($"email:{email}", "1", _cacheOptionProvider.GetDefault(), cancellationToken);
    }
}
