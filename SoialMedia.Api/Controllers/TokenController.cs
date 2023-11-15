using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Infrastructure.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;
        private readonly IPasswordService _passwordService;


        public TokenController(IConfiguration configuration, ISecurityService securityService, IPasswordService passwordService)
        {
            _configuration = configuration;
            _securityService = securityService;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Permite generar el token de acceso
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [HttpPost]
        public  async Task<IActionResult> Authentication(UserLogin login)
        {
            //If it is a valid user
            var validation = await IsValidUser(login);
            if (validation.Item1)
            {
                var token = GenerateToken(validation.Item2);
                var UsuarioLogado = validation.Item2;
                var response = new { token, UsuarioLogado };
                return Ok(response);
            }
            return NotFound();
        }
        private async Task<(bool, Security)> IsValidUser(UserLogin login)
        {
            var user  = await _securityService.GetLoginByCredentials(login);
            var isValid = _passwordService.Check(user.Password, login.Password);
            return (isValid, user);
        }
        private string GenerateToken(Security security)
        {
            //Header
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials);

            //Claims
            var claims = new[]
            {
                new Claim("UserName", security.UserName),
                new Claim("UserId", security.UserId.ToString()),
                new Claim("User", security.UserLogin),
                new Claim("Role", security.Role.ToString()),
            };

            //Payload
            var payload = new JwtPayload
            (
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claims,
                DateTime.Now,
                DateTime.UtcNow.AddHours(12)
            );

            //Token
            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    };
}
