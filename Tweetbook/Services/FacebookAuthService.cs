using Newtonsoft.Json;
using Tweetbook.External.Contracts;
using Tweetbook.Options;

namespace Tweetbook.Services
{
    public class FacebookAuthService : IFacebookAuthService
    {
        private const string TokenValidationUrl =
            "debug_token?input_token={0}&access_token={1}|{2}";

        private const string UserInfoUrl =
            @"me?fields=id,name,picture,first_name,last_name,email&access_token={0}";

        private readonly FacebookAuthSettingsOptions _facebookAuthSettings;
        private readonly FacebookGraphSettingsOptions _facebookGraphSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public FacebookAuthService(
            FacebookAuthSettingsOptions facebookAuthSettings,
            FacebookGraphSettingsOptions facebookGraphSettings, IHttpClientFactory httpClientFactory)
        {
            _facebookAuthSettings = facebookAuthSettings;
            _facebookGraphSettings = facebookGraphSettings;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accessToken)
        {
            var formattedUrl = _facebookGraphSettings.ApiBaseUrl + string.Format(
                TokenValidationUrl, accessToken, _facebookAuthSettings.AppId, _facebookAuthSettings.AppSecret);

            var result = await _httpClientFactory.CreateClient(_facebookGraphSettings.ClientName).GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();

            var responseAsString = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FacebookTokenValidationResult>(responseAsString);
        }

        public async Task<FacebookUserInfoResult> GetUserInfo(string accessToken)
        {
            var formattedUrl = _facebookGraphSettings.ApiBaseUrl + string.Format(UserInfoUrl, accessToken);

            var result = await _httpClientFactory.CreateClient(_facebookGraphSettings.ClientName).GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();

            var responseAsString = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FacebookUserInfoResult>(responseAsString);
        }
    }
}
