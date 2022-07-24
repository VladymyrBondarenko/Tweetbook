namespace Tweetbook.Extensions
{
    public static class CommonExtensions
    {
        public static string GetUserId(this HttpContext context)
        {
            if(context.User == null)
            {
                return string.Empty;
            }

            return context.User.Claims.FirstOrDefault(x => x.Type == "id")?.Value;
        }
    }
}
