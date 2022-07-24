using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Tweetbook.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _cacheService;

        public ResponseCacheService(IDistributedCache cache)
        {
            _cacheService = cache;
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan time)
        {
            if(response == null)
            {
                return;
            }

            var serializedResponse = JsonConvert.SerializeObject(response);

            await _cacheService.SetStringAsync(cacheKey, serializedResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = time
            });
        }

        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            var cachedResponse = await _cacheService.GetStringAsync(cacheKey);
            return cachedResponse;
        }
    }
}