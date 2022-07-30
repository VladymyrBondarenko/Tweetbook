using Tweetbook.Contracts.Contracts.V1.Responses;
using Tweetbook.Domain;
using Tweetbook.Services;

namespace Tweetbook.Helpers
{
    public class PaginationHelpers
    {
        public static PagedResponse<T> CreatePaginatedResponse<T>(IUriService uriService, PaginationQuery paginationQuery, List<T> response)
        {
            var nextPage = paginationQuery.PageNumber >= 1 
                ? uriService.GetAllPostsUri(new PaginationQuery(paginationQuery.PageNumber + 1, paginationQuery.PageSize)).ToString() 
                : null;

            var previousPage = paginationQuery.PageNumber - 1 >= 1
                ? uriService.GetAllPostsUri(new PaginationQuery(paginationQuery.PageNumber - 1, paginationQuery.PageSize)).ToString()
                : null;

            return new PagedResponse<T>
            {
                Data = response,
                PageSize = paginationQuery.PageSize,
                PageNumber = paginationQuery.PageNumber,
                NextPage = nextPage,
                PreviousPage = previousPage
            };
        }
    }
}
