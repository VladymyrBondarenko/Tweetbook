using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Tweetbook.Options;
using Tweetbook.Services;

namespace Tweetbook.Cache
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CacheAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;

        public CacheAttribute(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheSettings = context.HttpContext.RequestServices.GetService<RedisCacheSettingsOptions>();

            if (!cacheSettings.Enabled)
            {
                await next();
                return;
            }

            var cacheService = context.HttpContext.RequestServices.GetService<IResponseCacheService>();

            var cacheKey = generateCacheKeyFromResponse(context.HttpContext.Request);
            var cacheResponse = await cacheService.GetCachedResponseAsync(cacheKey);

            if(cacheResponse != null)
            {
                var contentResult = new ContentResult
                {
                    StatusCode = 200,
                    Content = cacheResponse,
                    ContentType = "application/json"
                };
                context.Result = contentResult;

                return;
            }

            var executedContext = await next();

            if(executedContext.Result is OkObjectResult okObjResult)
            {
                await cacheService.CacheResponseAsync(cacheKey, okObjResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
            }
        }

        private string generateCacheKeyFromResponse(HttpRequest request)
        {
            var sb = new StringBuilder();

            sb.Append(request.Path);

            foreach (var (key, value) in request.Query.OrderBy(x => x.Key))
            {
                sb.Append($"|{key}-{value}");
            }

            return sb.ToString();
        }
    }
}
