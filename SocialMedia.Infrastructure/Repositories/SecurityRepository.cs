using Microsoft.EntityFrameworkCore;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Enumerations;
using SocialMedia.Core.Interfaces;
using SocialMedia.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Infrastructure.Repositories
{
    public class SecurityRepository : BaseRepository<Security>, ISecurityRepository
    {
        public SecurityRepository(SocialMediaContext context) : base(context) {}

        public async Task<Security> GetLoginByCredentials(UserLogin login)
        {
            return await _entities.FirstOrDefaultAsync(x => x.UserLogin == login.User);
        }

        //public IEnumerable<Security> GetAllLogins()
        //{
        //    return _entities.Select(p => new Security
        //    {
        //        Id = p.Id,
        //        User = p.User,
        //        UserName = p.UserName,
        //        Password = p.Password,
        //        Role = (RoleType)Enum.Parse(typeof(RoleType), p.Role.ToString())

        //    }).AsEnumerable();
        //}
    }
}
