using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SocialMedia.Api.Responses;
using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.DTOs;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;
using SocialMedia.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SocialMedia.Api.Controllers
{
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly IUriService _uriService;

        public CategoryController(ICategoryService categoryService, IMapper mapper, IUriService uriService)
        {
            _categoryService = categoryService;
            _mapper = mapper;
            _uriService = uriService;
        }

        /// <summary>
        /// Permite obtener todas las categorias
        /// </summary>
        /// <param name="filters">Filters to apply</param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetCategoriesAsync))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<CategoryDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]

        public async Task<IActionResult> GetCategoriesAsync([FromQuery] CategoryQueryFilter filters)
        {
            var categories = await _categoryService.GetCategoriesAsync(filters);
            var categoriesDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            var metadata = new MetaData
            {
                TotalCount = categories.TotalCount,
                PageSize = categories.PageSize,
                CurrentPage = categories.CurrentPage,
                TotalPages = categories.TotalPages,
                HasNextPage = categories.HasNextPage,
                HasPreviousPage = categories.HasPreviousPage,
                NextPageUrl = categories.HasNextPage ? (_uriService.GetCategoriesPaginationUri(filters, Url.RouteUrl(nameof(GetCategoriesAsync)), categories.CurrentPage + 1).ToString()) : null,
                PreviousPageUrl = categories.HasPreviousPage ? (_uriService.GetCategoriesPaginationUri(filters, Url.RouteUrl(nameof(GetCategoriesAsync)), categories.CurrentPage - 1).ToString()) : null

            };
            var response = new ApiResponse<IEnumerable<CategoryDto>>(categoriesDtos)
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
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _categoryService.GetCategory(id);
            var categoryDto = _mapper.Map<CategoryDto>(category);
            var response = new ApiResponse<CategoryDto>(categoryDto);
            return Ok(response);
        }


        /// <summary>
        /// Permite crear un Comentario 
        /// </summary>
        /// <param name="categoryDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostComment(CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);

            await _categoryService.InsertCategory(category);

            categoryDto = _mapper.Map<CategoryDto>(category);
            var response = new ApiResponse<CategoryDto>(categoryDto);
            return Ok(response);
        }

        /// <summary>
        /// Permite actualizar un Comentario por su Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="categoryDto"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IActionResult> Put(int id, CategoryDto categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            category.Id = id;

            var result = await _categoryService.UpdateCategory(category);
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
            var result = await _categoryService.DeleteCategory(id);
            var response = new ApiResponse<bool>(result);
            return Ok(response);
        }
    }

}
