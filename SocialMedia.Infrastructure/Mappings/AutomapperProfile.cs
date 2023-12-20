using AutoMapper;
using SocialMedia.Core.DTOs;
using SocialMedia.Core.Entities;

namespace SocialMedia.Infrastructure.Mappings
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            //CreateMap<PostImageDto, Post>()
            //    .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image.Name + "#" + src.Image.Src));
            //CreateMap<Post, PostImageDto>()
            //    .ForMember(dest => dest.Image, opt => opt.MapFrom(src => ConvertToImageFile(src.Image))) ;
            CreateMap<Post, PostDto>().ForMember(dest => dest.Image, opt => opt.MapFrom(src => new ImageFIle { Src = src.Image })); ;
            CreateMap<PostDto, Post>();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, UserSecurityInfoDto>().ReverseMap();
            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<Security, SecurityDto>().ReverseMap();

        }
    }
}
