using Microsoft.Extensions.Caching.Distributed;

namespace WebAPI.Utilities.Contract;

public interface ICacheOptionProvider
{
    DistributedCacheEntryOptions GetOptions(string type);
    DistributedCacheEntryOptions GetDefault();
}
