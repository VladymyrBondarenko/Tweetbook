
namespace Tweetbook.Contracts.V1.Responses
{
    public class CreatePostResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<CreateTagResponse> Tags { get; set; }
    }
}
