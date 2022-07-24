using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Tweetbook.Contracts.V1;
using Tweetbook.Domain;
using Xunit;

namespace Tweetbook.IntegrationTests.ControllerTests
{
    public class TagsControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithoutAnyTags_ReturnsEmptyResponse()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await HttpClient.GetAsync(ApiRoutes.Tags.GetAll);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadFromJsonAsync<List<Tag>>()).Should().BeEmpty();
        }
    }
}
