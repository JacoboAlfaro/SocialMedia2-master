using AutoMapper;
using SocialMedia.Core.DTOs;
using SocialMedia.Core.Entities;
using System.Linq;

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

            //CreateMap<User, UserPostCommentsCountDto>()
            // .ForMember(dest => dest.Posts, opt => opt.MapFrom(src => src.Posts.Select(p => Mapper.Map<PostCommentsCountDto>(p))));

            //CreateMap<User, UserPostCommentsCountDto>()
            //    .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Posts?.Count() ?? 0));
            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<Security, SecurityDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();

        }
    }
}
