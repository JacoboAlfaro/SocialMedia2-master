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
using SocialMedia.Core.Services;
using SocialMedia.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SocialMedia.Api.Controllers
{
    //[Authorize(Roles = nameof(RoleType.Administrator))]
    [Authorize]
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;
        private readonly IUriService _uriService;


        public SecurityController(ISecurityService securityService, IMapper mapper, IPasswordService passwordService, IUriService uriService, IUserService userService)
        {
            _securityService = securityService;
            _mapper = mapper;
            _passwordService = passwordService;
            _uriService = uriService;
            _userService = userService;
        }

        /// <summary>
        /// Permite obtener todos los Logins
        /// </summary>
        /// <param name="filters">Filters to apply</param>
        /// <returns></returns>
        [HttpGet(Name = nameof(GetLogins))]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<SecurityDto>>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetLogins([FromQuery] SecurityQueryFilter filters)
        {
            var logins = await _securityService.GetLogins(filters);
            var loginsDtos = _mapper.Map<IEnumerable<SecurityDto>>(logins);

            var metadata = new MetaData
            {
                TotalCount = logins.TotalCount,
                PageSize = logins.PageSize,
                CurrentPage = logins.CurrentPage,
                TotalPages = logins.TotalPages,
                HasNextPage = logins.HasNextPage,
                HasPreviousPage = logins.HasPreviousPage,
                NextPageUrl = logins.HasNextPage ? (_uriService.GetLoginsPaginationUri(filters, Url.RouteUrl(nameof(GetLogins)), logins.CurrentPage + 1).ToString()): null,
                PreviousPageUrl = logins.HasPreviousPage ? (_uriService.GetLoginsPaginationUri(filters, Url.RouteUrl(nameof(GetLogins)), logins.CurrentPage - 1).ToString()): null

            };
            var response = new ApiResponse<IEnumerable<SecurityDto>>(loginsDtos)
            {
                Meta = metadata

        };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(response);
        }

        /// <summary>
        /// Permite crear un Login
        /// </summary>
        /// <param name="securityDto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Permite obtener un Login por su Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
