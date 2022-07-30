
using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public interface IUriService
    {
        Uri GetAllPostsUri(PaginationQuery paginationQuery = null);
        Uri GetPostUri(string postId);
    }
}