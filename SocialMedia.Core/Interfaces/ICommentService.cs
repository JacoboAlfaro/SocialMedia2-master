using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Entities;
using SocialMedia.Core.QueryFilters;
using System.Threading.Tasks;

namespace SocialMedia.Core.Interfaces
{
    public interface ICommentService
    {
        Task<PagedList<Comment>> GetCommentsAsync(CommentQueryFilter filters);
        Task<Comment> GetComment(int id);
        Task InsertComment(Comment comment);
        Task<bool> UpdateComment(Comment comment);
        Task<bool> NewCommentLike(Comment comment);
        Task<bool> DeleteComment(int id);
    }
}
