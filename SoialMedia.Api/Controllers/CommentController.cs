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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SocialMedia.Api.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public CommentController(ICommentService commentService, IMapper mapper, IUriService uriService)
        {
            _commentService = commentService;
            _mapper = mapper;
            _uriService = uriService;
        }

        /// <summary>
        /// Permite obtener todos los Comentarios
        /// </summary>
        /// <param name="filters">Filters to apply</param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetCommentsAsync))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<CommentDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> GetCommentsAsync([FromQuery] CommentQueryFilter filters)
        {
            var comments = await _commentService.GetCommentsAsync(filters);
            var commentsDtos = _mapper.Map<IEnumerable<CommentDto>>(comments);

            var metadata = new MetaData
            {
                TotalCount = comments.TotalCount,
                PageSize = comments.PageSize,
                CurrentPage = comments.CurrentPage,
                TotalPages = comments.TotalPages,
                HasNextPage = comments.HasNextPage,
                HasPreviousPage = comments.HasPreviousPage,
                NextPageUrl = comments.HasNextPage ? (_uriService.GetCommentPaginationUri(filters, Url.RouteUrl(nameof(GetCommentsAsync)), comments.CurrentPage + 1).ToString()): null,
                PreviousPageUrl = comments.HasPreviousPage ? (_uriService.GetCommentPaginationUri(filters, Url.RouteUrl(nameof(GetCommentsAsync)), comments.CurrentPage - 1).ToString()): null

            };
            var response = new ApiResponse<IEnumerable<CommentDto>>(commentsDtos)
            {
                Meta = metadata
        };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(response);
        }

        /// <summary>
        /// Permite obtener un Comentario por su Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetComment(int id)
        {
            var comment = await _commentService.GetComment(id);
            var commentDto = _mapper.Map<CommentDto>(comment);
            var response = new ApiResponse<CommentDto>(commentDto);
            return Ok(response);
        }

        /// <summary>
        /// Permite crear un Comentario 
        /// </summary>
        /// <param name="commentDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostComment(CommentDto commentDto)
        {
            var comment = _mapper.Map<Comment>(commentDto);

            await _commentService.InsertComment(comment);

            commentDto = _mapper.Map<CommentDto>(comment);
            var response = new ApiResponse<CommentDto>(commentDto);
            return Ok(response);
        }

        /// <summary>
        /// Permite actualizar un Comentario por su Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="commentDto"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Put(int id, CommentDto commentDto)
        {
            var comment = _mapper.Map<Comment>(commentDto);
            comment.Id = id;

            var result = await _commentService.UpdateComment(comment);
            var response = new ApiResponse<bool>(result);
            return Ok(response);
        }
        /// <summary>
        /// Permite agregarle un like al comentario seleccionado
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<IActionResult> AddLike(int id)
        {
            var post = await _commentService.GetComment(id);
            var result = await _commentService.NewCommentLike(post);
            var response = new ApiResponse<bool>(result);
            return Ok(response);
        }

        /// <summary>
        /// Permite eliminar un Comentario por su Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _commentService.DeleteComment(id);
            var response = new ApiResponse<bool>(result);
            return Ok(response);
        }
    }
}
