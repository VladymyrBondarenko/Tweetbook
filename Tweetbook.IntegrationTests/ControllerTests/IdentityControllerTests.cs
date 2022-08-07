using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Tweetbook.Contracts.Contracts.V1.Requests;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Options;
using Xunit;

namespace Tweetbook.IntegrationTests.ControllerTests
{
    public class IdentityControllerTests : IntegrationTest
    {
        [Fact]
        public async Task Register_ReturnsAuthSuccessResponseWithToken()
        {
            //Arrange
            var response = await HttpClient.PostAsJsonAsync(
                ApiRoutes.Identity.Register,
                new UserRegistrationRequest
                {
                    Email = "vladymyr.bondarenko1@gmail.com",
                    Password = "1777897Vova."
                });

            //Act
            var authResponse = await response.Content.ReadFromJsonAsync<AuthSuccessResponse>();

            //Assert
            authResponse.Token.Should().NotBeNullOrWhiteSpace();
            authResponse.RefreshToken.Should().NotBeNullOrWhiteSpace();
        }
    }
}