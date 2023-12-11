using Microsoft.EntityFrameworkCore;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Infrastructure.Repositories
{
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        public PostRepository(SocialMediaContext context) : base(context) { } 

        public async Task<IEnumerable<Post>> GetPostsByUser(int userId)
        {
            return await _entities.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetSummaryPostsByUserId(int userId)
        {
            return await _entities.Where(x => x.UserId == userId).Select(p => new Post
            {
                Id = p.Id,
                UserId = p.UserId,
                Date = p.Date,
                Title = p.Title,
                Description = p.Description,
                Image = p.Image,
                IsEdit = p.IsEdit,
                Likes = p.Likes
            }).ToListAsync();
        }
        public async Task<Post> GetSummaryPostByPostId(int id)
        {
            return await _entities.Where(x => x.Id == id).Select(p => new Post
            {
                Id = p.Id,
                UserId = p.UserId,
                Date = p.Date,
                Title = p.Title,
                Description = p.Description,
                Image = p.Image,
                IsEdit = p.IsEdit,
                Likes = p.Likes
            }).FirstOrDefaultAsync();
        }
    }
}
