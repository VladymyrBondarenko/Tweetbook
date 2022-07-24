namespace Tweetbook.Contracts.V1.Requests
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; }

        public Guid RefreshToken { get; set; }
    }
}
