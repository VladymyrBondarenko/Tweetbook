using StackExchange.Redis;

namespace Tweetbook.Options
{
    public class RedisCacheSettingsOptions
    {
        public bool Enabled { get; set; }

        public string InstanceName { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Password { get; set; }

        public int ConnectRetry { get; set; }

        public bool Ssl { get; set; }

        public bool AbortOnConnectFail { get; set; }

        public int ConnectTimeout { get; set; }

        public int SyncTimeout { get; set; }
    }
}