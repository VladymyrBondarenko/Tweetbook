using AutoMapper;
using Tweetbook.Contracts.V1.Responses;
using Tweetbook.Domain;

namespace Tweetbook.MappingProfiles
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Post, GetPostResponse>()
                .ForMember(dest => dest.Tags, opt =>
                {
                    opt.MapFrom(src => src.Tags.Select(x => new GetTagResponse { Id = x.Id, Name = x.Name }));
                });

            CreateMap<Post, CreatePostResponse>()
                .ForMember(dest => dest.Tags, opt =>
                {
                    opt.MapFrom(src => src.Tags.Select(x => new CreateTagResponse { Id = x.Id, Name = x.Name }));
                });

            CreateMap<Post, UpdatePostSuccessResponse>()
               .ForMember(dest => dest.Tags, opt =>
               {
                   opt.MapFrom(src => src.Tags.Select(x => new GetTagResponse { Id = x.Id, Name = x.Name }));
               });

            CreateMap<Tag, GetTagResponse>();
        }
    }
}
