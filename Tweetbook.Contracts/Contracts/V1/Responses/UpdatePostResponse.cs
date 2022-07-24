namespace Tweetbook.Contracts.V1.Responses
{
    public class UpdatePostSuccessResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public List<GetTagResponse> Tags { get; set; }
    }
}
