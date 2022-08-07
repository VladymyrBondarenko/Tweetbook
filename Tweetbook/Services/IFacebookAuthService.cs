using Tweetbook.External.Contracts;

namespace Tweetbook.Services
{
    public interface IFacebookAuthService
    {
        Task<FacebookUserInfoResult> GetUserInfo(string accessToken);
        Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accessToken);
    }
}