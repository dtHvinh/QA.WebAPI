using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using WebAPI.Attributes;
using WebAPI.Model;
using WebAPI.Utilities.Contract;
using static WebAPI.Utilities.Constants;
using static WebAPI.Utilities.Provider.CacheOptionProvider;

namespace WebAPI.Utilities.Services;

[Implement(typeof(ICacheService), ServiceLifetime.Singleton)]
public class CacheService(IDistributedCache cache,
                          ICacheOptionProvider cacheOptionProvider,
                          JsonSerializerOptions options) : ICacheService
{
    private readonly IDistributedCache cache = cache;
    private readonly ICacheOptionProvider _cacheOptionProvider = cacheOptionProvider;
    private readonly JsonSerializerOptions _jsonOptions = options;

    public async Task<AppUser?> GetAppUserAsync(int id)
    {
        var user = await cache.GetStringAsync(RedisKeyGen.AppUserKey(id));
        if (user is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<AppUser>(user);
    }

    public async Task<Question?> GetQuestionAsync(int id)
    {
        var question = await cache.GetStringAsync(RedisKeyGen.Question(id));
        if (question is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Question>(question, _jsonOptions);
    }

    public async Task SetAppUserAsync(AppUser user)
    {
        var cacheOptions = _cacheOptionProvider.GetOptions(nameof(AppUser));
        await cache.SetStringAsync(
            RedisKeyGen.AppUserKey(user.Id), JsonSerializer.Serialize(user), cacheOptions);
    }

    public async Task SetUsedEmail(string email)
    {
        await cache.SetStringAsync(RedisKeyGen.UserEmail(email), "", _cacheOptionProvider.GetDefault());
    }

    public async Task SetQuestionAsync(Question question)
    {
        var cacheOptions = _cacheOptionProvider.GetOptions(nameof(Question));
        var str = JsonSerializer.Serialize(question, _jsonOptions);
        await cache.SetStringAsync(RedisKeyGen.Question(question.Id), str, cacheOptions);
    }

    public async Task RemoveAppUserAsync(int id)
    {
        await cache.RemoveAsync(RedisKeyGen.AppUserKey(id));
    }

    public async Task<bool> IsEmailUsed(string email)
    {
        return await cache.GetStringAsync(RedisKeyGen.UserEmail(email)) is not null;
    }

    public async Task<List<Tag>?> GetTags(string orderBy, int skip, int take)
    {
        var strValue = await cache.GetStringAsync(RedisKeyGen.GetTags(orderBy, skip, take));
        if (strValue is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<List<Tag>>(strValue, _jsonOptions);
    }

    public async Task SetTags(string orderBy, int skip, int take, List<Tag> values)
    {
        await cache.SetStringAsync(RedisKeyGen.GetTags(orderBy, skip, take),
            JsonSerializer.Serialize(values), _cacheOptionProvider.GetOptions(nameof(Tag)));
    }

    public async Task<Tag?> GetTagWithQuestion(int tagId, string orderBy, int questionPage, int questionPageSize)
    {
        var strValue = await cache.GetStringAsync(RedisKeyGen.GetTagDetail(tagId, orderBy, questionPage, questionPageSize));
        if (strValue is null)
        {
            return null;
        }

        return JsonSerializer.Deserialize<Tag>(strValue, _jsonOptions);
    }

    public async Task SetTagDetail(Tag tag, string orderBy, int questionPage, int questionPageSize)
    {
        await cache.SetStringAsync(RedisKeyGen.GetTagDetail(tag.Id, orderBy, questionPage, questionPageSize),
            JsonSerializer.Serialize(tag, _jsonOptions),
            _cacheOptionProvider.GetOptions(nameof(ExtensionCacheOptions.TagDetail)));
    }
}
