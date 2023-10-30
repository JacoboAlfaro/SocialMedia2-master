using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SocialMedia.Api.Responses;
using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.DTOs;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Enumerations;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;
using SocialMedia.Core.Services;
using SocialMedia.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SocialMedia.Api.Controllers
{
    [Authorize(Roles = nameof(RoleType.Administrator))]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;
        private readonly IUriService _uriService;


        public SecurityController(ISecurityService securityService, IMapper mapper, IPasswordService passwordService, IUriService uriService)
        {
            _securityService = securityService;
            _mapper = mapper;
            _passwordService = passwordService;
            _uriService = uriService;
        }

        /// <summary>
        /// Retrieve all posts
        /// </summary>
        /// <param name="filters">Filters to apply</param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetLogins))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<PostDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public IActionResult GetLogins([FromQuery] SecurityQueryFilter filters)
        {
            var users = _securityService.GetLogins(filters);
            var loginsDtos = _mapper.Map<IEnumerable<SecurityDto>>(users);

            var metadata = new MetaData
            {
                TotalCount = users.TotalCount,
                PageSize = users.PageSize,
                CurrentPage = users.CurrentPage,
                TotalPages = users.TotalPages,
                HasNextPage = users.HasNextPage,
                HasPreviousPage = users.HasPreviousPage,
                NextPageUrl = _uriService.GetLoginsPaginationUri(filters, Url.RouteUrl(nameof(GetLogins)), users.CurrentPage + 1).ToString(),
                PreviousPageUrl = _uriService.GetLoginsPaginationUri(filters, Url.RouteUrl(nameof(GetLogins)), users.CurrentPage - 1).ToString()

            };
            var response = new ApiResponse<IEnumerable<SecurityDto>>(loginsDtos)
            {
                Meta = metadata
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Post(SecurityDto securityDto)
        {
            var security = _mapper.Map<Security>(securityDto);

            security.Password = _passwordService.Hash(security.Password);
            await _securityService.RegisterUser(security);

            securityDto = _mapper.Map<SecurityDto>(security);
            var response = new ApiResponse<SecurityDto>(securityDto);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> getLoginById(int id)
        {
            var login = await _securityService.GetLogin(id);
            var SecurityDto = _mapper.Map<SecurityDto>(login);
            var response = new ApiResponse<SecurityDto>(SecurityDto);
            return Ok(response);
        }
    }
}
