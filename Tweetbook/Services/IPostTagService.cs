using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public interface IPostTagService
    {
        Task<Tag> CreateAsync(Tag post);
        Task<List<Tag>> GetAllAsync();
        Task<Tag> GetAsync(Guid Id);
    }
}