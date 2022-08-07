using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> LoginAsync(string email, string password);
        Task<AuthenticationResult> LoginWithFacebookAsync(string accessToken);
        Task<AuthenticationResult> RefreshTokenAsync(string token, Guid refreshToken);
        Task<AuthenticationResult> RegisterAsync(string email, string password);
    }
}