using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Xunit;

namespace Tweetbook.IntegrationTests.ControllerTests
{
    public class IdentityControllerTests : IntegrationTest
    {
        [Fact]
        public async Task Register_ReturnsAuthSuccessResponseWithToken()
        {
            var response = await HttpClient.PostAsJsonAsync(
                ApiRoutes.Identity.Register,
                new UserRegistrationRequest
                {
                    Email = "vladymyr.bondarenko1@gmail.com",
                    Password = "1777897Vova."
                });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var authResponse = await response.Content.ReadFromJsonAsync<AuthSuccessResponse>();
            authResponse.Token.Should().NotBeNullOrWhiteSpace();
            authResponse.RefreshToken.Should().NotBeNullOrWhiteSpace();
        }
    }
}