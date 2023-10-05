using Microsoft.EntityFrameworkCore;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(SocialMediaContext context) : base(context) { }

        //public async Task<IEnumerable<Post>> GetPostsByUser(int userId)
        //{
        //    return await _entities.Where(x => x.Id == userId).ToListAsync();
        //}

    }
}
