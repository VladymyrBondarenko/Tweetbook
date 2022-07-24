using Swashbuckle.AspNetCore.Filters;
using Tweetbook.Contracts.V1.Responses;

namespace Tweetbook.SwaggerExamples.Responses
{
    public class CreatePostResponseExample : IExamplesProvider<CreatePostResponse>
    {
        public CreatePostResponse GetExamples()
        {
            return new CreatePostResponse
            {
                Id = Guid.NewGuid(),
                Name = "Post name",
                Tags = new List<CreateTagResponse>
                {
                    new CreateTagResponse
                    {
                        Id = Guid.NewGuid(),
                        Name = "Tag name"
                    }
                }
            };
        }
    }
}
