using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using WebAPI.Attributes;
using WebAPI.Model;
using WebAPI.Utilities.Contract;
using static WebAPI.Utilities.Constants;

namespace WebAPI.Utilities.Services;

[Implement(typeof(ICacheService), ServiceLifetime.Singleton)]
public class CacheService(IDistributedCache cache, ICacheOptionProvider cacheOptionProvider) : ICacheService
{
    private readonly IDistributedCache cache = cache;
    private readonly ICacheOptionProvider _cacheOptionProvider = cacheOptionProvider;

    public async Task<AppUser?> GetAppUserAsync(Guid id)
    {
        var user = await cache.GetStringAsync(RedisKeyGen.AppUserKey(id));
        if (user is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<AppUser>(user);
    }

    public async Task UpdateAppUserAsync(AppUser user)
    {
        var options = _cacheOptionProvider.GetOptions(nameof(AppUser));
        await cache.SetStringAsync(RedisKeyGen.AppUserKey(user.Id), JsonSerializer.Serialize(user), options);
    }

    public async Task RemoveAppUserAsync(Guid id)
    {
        await cache.RemoveAsync(RedisKeyGen.AppUserKey(id));
    }

    public async Task SetUsedEmail(string email)
    {
        await cache.SetStringAsync(RedisKeyGen.UserEmail(email), "", _cacheOptionProvider.GetDefault());
    }

    public async Task<bool> IsEmailUsed(string email)
    {
        return await cache.GetStringAsync(RedisKeyGen.UserEmail(email)) is not null;
    }
}
