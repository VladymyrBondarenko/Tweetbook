using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tweetbook.External.Contracts;
using Tweetbook.Options;
using Tweetbook.Services;
using Xunit;

namespace Tweetbook.UnitTests
{
    public class FacebookAuthServiceTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock = new();
        private readonly Mock<HttpClientHandler> _handlerMock = new();
        private readonly IFacebookAuthService _facebookAuthService;
        private readonly FacebookAuthSettingsOptions _facebookAuthSettings;
        private readonly FacebookGraphSettingsOptions _facebookGraphSettings;

        public FacebookAuthServiceTests()
        {
            _facebookAuthSettings = new FacebookAuthSettingsOptions 
            {
                AppId = Guid.NewGuid().ToString(),
                AppSecret = Guid.NewGuid().ToString()
            };

            _facebookGraphSettings = new FacebookGraphSettingsOptions
            {
                ApiBaseUrl = "https://graph.facebook.com/v14.0/",
                ClientName = "GraphFacebook"
            };

            _facebookAuthService = new FacebookAuthService(_facebookAuthSettings, _facebookGraphSettings, _httpClientFactoryMock.Object);
        }

        [Fact]
        public async Task ValidateAccessTokenAsync_ReturnsFacebookTokenValidationResult()
        {
            //Arrange
            var token = Guid.NewGuid().ToString();

            var requestUrl = _facebookGraphSettings.ApiBaseUrl + string.Format("debug_token?input_token={0}&access_token={1}|{2}",
                token, _facebookAuthSettings.AppId, _facebookAuthSettings.AppSecret);

            var validationResult = new FacebookTokenValidationResult
            {
                Data = new ValidationResultData
                {
                    IsValid = true,
                    AppId = _facebookAuthSettings.AppId
                }
            };

            var response = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(validationResult)),
                StatusCode = HttpStatusCode.OK
            };

            _handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri(requestUrl)),
                ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync(response);

            _httpClientFactoryMock
                .Setup(x => x.CreateClient(_facebookGraphSettings.ClientName))
                .Returns(new HttpClient(_handlerMock.Object)
                {
                    BaseAddress = new Uri(_facebookGraphSettings.ApiBaseUrl)
                });

            //Act
            var authResult = await _facebookAuthService.ValidateAccessTokenAsync(token);

            //Assert
            authResult.Data.Should().NotBeNull();
            authResult.Data.IsValid.Should().BeTrue();
            authResult.Data.AppId.Should().Be(_facebookAuthSettings.AppId);
        }
    }
}
