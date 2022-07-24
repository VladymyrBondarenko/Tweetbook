namespace Tweetbook.Contracts.V1.Requests
{
    public class UpdatePostRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}