namespace Tweetbook.Services
{
    public interface IResponseCacheService
    {
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan time);
        Task<string> GetCachedResponseAsync(string cacheKey);
    }
}
