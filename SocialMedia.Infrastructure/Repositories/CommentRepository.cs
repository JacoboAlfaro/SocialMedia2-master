using Microsoft.EntityFrameworkCore;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Infrastructure.Repositories
{
    public class CommentRepository: BaseRepository<Comment>, ICommentRepository
    {
        public CommentRepository(SocialMediaContext context) : base(context) { }

        public async Task<IEnumerable<Comment>> GetCommentsByPostId(int postId)
        {
            return await _entities.Where(x => x.PostId == postId).ToListAsync();
        }
        public async Task<IEnumerable<Comment>> GetSummaryCommentsByUserId(int userId)
        {
            return await _entities.Where(x => x.UserId == userId).Select(p => new Comment
            {
                Id = p.Id,
                UserId = p.UserId,
                PostId = p.PostId,
                Date = p.Date,
                Description = p.Description,
                IsActive = p.IsActive,
                IsEdit = p.IsEdit,
                Likes = p.Likes,
                Categories = p.Categories
            }).ToListAsync();
        }
    }
}
