using SocialMedia.Core.Entities;
using System;
using System.Threading.Tasks;

namespace SocialMedia.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IPostRepository PostRepository { get; }
        IUserRepository UserRepository { get; }
        ICommentRepository CommentRepository { get; }
        ISecurityRepository SecurityRepository { get; }
        ICategoryRepository CategoryRepository { get; }

        void SaveChanges();

        Task SaveChangesAsync();


    }
}
