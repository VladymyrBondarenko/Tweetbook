using Newtonsoft.Json;

namespace Tweetbook.External.Contracts
{
    public class FacebookTokenValidationResult
    {
        [JsonProperty("data")]
        public ValidationResultData Data { get; set; }
    }

    public class ValidationResultData
    {
        [JsonProperty("app_id")]
        public string AppId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("application")]
        public string Application { get; set; }

        [JsonProperty("data_access_expires_at")]
        public long DataAccessExpiresAt { get; set; }

        [JsonProperty("error")]
        public ValidationResultError Error { get; set; }

        [JsonProperty("expires_at")]
        public long ExpiresAt { get; set; }

        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }

        [JsonProperty("metadata")]
        public ValidationResultMetadata Metadata { get; set; }

        [JsonProperty("scopes")]
        public string[] Scopes { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }

    public class ValidationResultError
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("subcode")]
        public long Subcode { get; set; }
    }

    public class ValidationResultMetadata
    {
        [JsonProperty("auth_type")]
        public string AuthType { get; set; }
    }
}