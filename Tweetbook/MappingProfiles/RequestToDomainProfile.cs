using AutoMapper;
using Tweetbook.Contracts.Contracts.V1.Requests;
using Tweetbook.Domain;

namespace Tweetbook.MappingProfiles
{
    public class RequestToDomainProfile : Profile
    {
        public RequestToDomainProfile()
        {
            CreateMap<PaginationQueryRequest, PaginationQuery>();
        }
    }
}
