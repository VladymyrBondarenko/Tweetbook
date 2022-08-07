namespace Tweetbook.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Root = "api";

        public const string Version = "v1";

        public const string BaseUrl = $"{Root}/{Version}";

        public static class Posts
        {
            public const string GetAll = $"{BaseUrl}/posts";

            public const string Get = BaseUrl + "/posts/{postId}";

            public const string Create = $"{BaseUrl}/posts";

            public const string Update = $"{BaseUrl}/posts";

            public const string Delete = BaseUrl + "/posts/{postId}";
        }

        public static class Identity
        {
            public const string Register = $"{BaseUrl}/identity/register";

            public const string Login = $"{BaseUrl}/identity/login";

            public const string FacebookAuth = BaseUrl + "/identity/auth/fb";

            public const string Refresh = $"{BaseUrl}/identity/refresh";
        }

        public static class Tags
        {
            public const string GetAll = $"{BaseUrl}/tags";

            public const string Get = BaseUrl + "/tags/{tagId}";

            public const string Create = $"{BaseUrl}/tags";

            public const string Update = $"{BaseUrl}/tags";

            public const string Delete = BaseUrl + "/tags/{tagId}";
        }
    }
}
