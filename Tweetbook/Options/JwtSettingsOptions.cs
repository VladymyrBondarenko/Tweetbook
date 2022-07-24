namespace Tweetbook.Options
{
    public class JwtSettingsOptions
    {
        public string Secret { get; set; }

        public TimeSpan TokenLifeTime { get; set; }
    }
}