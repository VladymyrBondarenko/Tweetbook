using Microsoft.AspNetCore.WebUtilities;
using Tweetbook.Contracts.V1;
using Tweetbook.Domain;

namespace Tweetbook.Services
{
    public class UriService : IUriService
    {
        private readonly string _baseUri;

        public UriService(string baseUri)
        {
            _baseUri = baseUri;
        }

        public Uri GetPostUri(string postId)
        {
            return new Uri($"{_baseUri}/{ApiRoutes.Posts.Get.Replace("{postId}", postId)}");
        }

        public Uri GetAllPostsUri(PaginationQuery paginationQuery = null)
        {
            if(paginationQuery == null)
            {
                return new Uri(_baseUri);
            }

            var modifiedUri = QueryHelpers.AddQueryString(_baseUri, "pageNumber", paginationQuery.PageNumber.ToString());
            modifiedUri = QueryHelpers.AddQueryString(modifiedUri, "pageSize", paginationQuery.PageSize.ToString());

            return new Uri(modifiedUri);
        }
    }
}
