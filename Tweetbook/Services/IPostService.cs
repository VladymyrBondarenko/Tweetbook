using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public interface IPostService
    {
        Task<Post> CreateAsync(Post post);
        Task<bool> DeleteAsync(Guid Id);
        Task<Post> GetAsync(Guid Id);
        Task<List<Post>> GetAllAsync(PaginationQuery paginationFilter);
        Task<bool> UpdateAsync(Post post);
        Task<bool> UserOwnsPostAsync(Guid id, string userId);
    }
}