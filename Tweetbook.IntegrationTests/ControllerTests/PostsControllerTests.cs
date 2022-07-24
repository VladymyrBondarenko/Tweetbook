using FluentAssertions;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Tweetbook.Contracts.V1;
using Tweetbook.Contracts.V1.Requests;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;
using Xunit;

namespace Tweetbook.IntegrationTests.ControllerTests
{
    public class PostsControllerTests : IntegrationTest
    {
        [Fact]
        public async Task GetAll_WithoutAnyPosts_ReturnsEmptyResponse()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await HttpClient.GetAsync(ApiRoutes.Posts.GetAll);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            (await response.Content.ReadFromJsonAsync<List<Post>>()).Should().BeEmpty();
        }

        [Fact]
        public async Task Get_ReturnsPost_WhenPostExistsInDb()
        {
            // Arrange
            await AuthenticateAsync();
            var createdPost = await CreatePostAsync(
                new CreatePostRequest { Name = "Post 1", Tags = new List<CreateTagRequest> { new CreateTagRequest { Name = "Tag 1" } }  });

            // Act
            var response = await HttpClient.GetAsync(ApiRoutes.Posts.Get.Replace("{postId}", createdPost.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var post = await response.Content.ReadFromJsonAsync<Post>();
            post.Id.Should().Be(createdPost.Id);
            post.Name.Should().Be(createdPost.Name);
        }

        [Fact]
        public async Task Edit_ReturnsPost_WhenPostExistsInDb()
        {
            // Arrange
            await AuthenticateAsync();
            var createdPost = await CreatePostAsync(
                new CreatePostRequest { Name = "Post 1", Tags = new List<CreateTagRequest> { new CreateTagRequest { Name = "Tag 1" } } });

            // Act
            var postToUpdate = new UpdatePostRequest
            {
                Id = createdPost.Id,
                Name = "Post 2"
            };
            var response = await HttpClient.PutAsJsonAsync(ApiRoutes.Posts.Update, postToUpdate);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var post = await response.Content.ReadFromJsonAsync<UpdatePostSuccessResponse>();
            post.Id.Should().Be(postToUpdate.Id);
            post.Name.Should().Be(postToUpdate.Name);
        }

        [Fact]
        public async Task Delete_ReturnsPost_WhenPostExistsInDb()
        {
            // Arrange
            await AuthenticateAsync();
            var createdPost = await CreatePostAsync(
                new CreatePostRequest { Name = "Post 1", Tags = new List<CreateTagRequest> { new CreateTagRequest { Name = "Tag 1" } } });

            // Act
            var response = await HttpClient.GetAsync(ApiRoutes.Posts.Delete.Replace("{postId}", createdPost.Id.ToString()));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
