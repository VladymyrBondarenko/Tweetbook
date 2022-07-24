using Swashbuckle.AspNetCore.Filters;
using Tweetbook.Contracts.V1.Requests;

namespace Tweetbook.SwaggerExamples.Requests
{
    public class CreatePostRequestExample : IExamplesProvider<CreatePostRequest>
    {
        public CreatePostRequest GetExamples()
        {
            return new CreatePostRequest 
            { 
                Name = "Post name", 
                Tags = new List<CreateTagRequest> 
                { 
                    new CreateTagRequest { Name = "Tag name" } 
                } 
            };
        }
    }
}
