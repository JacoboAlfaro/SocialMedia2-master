using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SocialMedia.Api.Responses;
using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.DTOs;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Exceptions;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;
using SocialMedia.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SocialMedia.Api.Controllers
{
    //[Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;


        public PostController(IPostService postService, IMapper mapper, IUriService uriService)
        {
            _postService = postService;
            _mapper = mapper;
            _uriService = uriService;
        }

        /// <summary>
        /// Permite obtener todos los Posts
        /// </summary>
        /// <param name="filters">Filters to apply</param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetPosts))]
        [ProducesResponseType((int)HttpStatusCode.OK,Type = typeof(ApiResponse<IEnumerable<PostDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> GetPosts([FromQuery]PostQueryFilter filters)
        {
            var posts = await _postService.GetPosts(filters);
            var postsDtos = _mapper.Map<IEnumerable<PostDto>>(posts);

            var metadata = new MetaData
            {
                TotalCount = posts.TotalCount,
                PageSize = posts.PageSize,
                CurrentPage = posts.CurrentPage,
                TotalPages = posts.TotalPages,
                HasNextPage = posts.HasNextPage,
                HasPreviousPage = posts.HasPreviousPage,
                NextPageUrl = posts.HasNextPage ? (_uriService.GetPostPaginationUri(filters, Url.RouteUrl(nameof(GetPosts)), posts.CurrentPage + 1).ToString()): null,
                PreviousPageUrl = posts.HasPreviousPage ? (_uriService.GetPostPaginationUri(filters, Url.RouteUrl(nameof(GetPosts)), posts.CurrentPage - 1).ToString()): null

            };
            var response = new ApiResponse<IEnumerable<PostDto>>(postsDtos)
            {
                Meta = metadata

        };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(response);
        }

        /// <summary>
        /// Permite obtener un Post por su Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost(int id)
        {
            var post = await _postService.GetPost(id);
            var postDto = _mapper.Map<PostDto>(post);
            var response = new ApiResponse<PostDto>(postDto);
            return Ok(response);
        }

        /// <summary>
        /// Permite crear un Post
        /// </summary>
        /// <param name="postDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(PostDto postDto)
        {
            //var post = _mapper.Map<Post>(postDto);

            //await _postService.InsertPost(post);

            //postDto = _mapper.Map<PostDto>(post);
            //var response = new ApiResponse<PostDto>(postDto);
            //return Ok(response);

            var post = _mapper.Map<Post>(postDto);

            if(postDto.Image != null)
            {
            post.Image = $"{postDto.Image.Name}#{postDto.Image.Src}";
            }
            //if (postdto.image != null)
            //{
            //    var imagenmovida = _postservice.saveimage(postdto.image);
            //    if (imagenmovida.item1)
            //    {
            //        var imgprefix = imagenmovida.item2;
            //        post.image = imgprefix;


            //        await _postservice.insertpost(post);

            //        postdto = _mapper.map<postdto>(post);
            //        var responseimg = new apiresponse<postdto>(postdto);
            //        return ok(responseimg);
            //    }
            //    else
            //    {
            //        throw new businessexceptions("nombre de imagen ya existente, seleccione otro nombre o cambie la imagen");
            //    }
            //}
            await _postService.InsertPost(post);

            postDto = _mapper.Map<PostDto>(post);
            var response = new ApiResponse<PostDto>(postDto);
            return Ok(response);
        }

        /// <summary>
        /// Permite actualizar un Post por su Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="postDto"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Put(int id, PostDto postDto)
        {
            var post = _mapper.Map<Post>(postDto);
            post.Id = id;

            var result = await _postService.UpdatePost(post);
            var response = new ApiResponse<bool>(result);
            return Ok(response);
        }

        /// <summary>
        /// Permite dar like al post seleccionado
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> AddLike(int id)
        {
            var post = await _postService.GetPost(id);
            var result = await _postService.NewLike(post);
            var response = new ApiResponse<int>(result);
            return Ok(response);
        }

        /// <summary>
        /// Permite eliminar un Post por su Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _postService.DeletePost(id);
            var response = new ApiResponse<bool>(result);
            return Ok(response);
        }
    }
}
