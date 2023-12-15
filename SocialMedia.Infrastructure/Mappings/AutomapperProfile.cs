using AutoMapper;
using SocialMedia.Core.DTOs;
using SocialMedia.Core.Entities;

namespace SocialMedia.Infrastructure.Mappings
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Post, PostDto>().ForMember(dest => dest.Image, opt => opt.MapFrom(src => new ImageFIle { Src = src.Image })); ;
            CreateMap<PostDto, Post>();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserSecurityInfoDto>().ReverseMap();
            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<Security, SecurityDto>().ReverseMap();

        }
    }
}
