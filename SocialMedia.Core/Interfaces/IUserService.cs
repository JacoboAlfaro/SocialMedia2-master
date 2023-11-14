using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Entities;
using SocialMedia.Core.QueryFilters;
using System.Threading.Tasks;

namespace SocialMedia.Core.Services
{
    public interface IUserService
    {
        Task<PagedList<User>> GetUsers(UserQueryFilter filters);
        Task<User> GetUser(int id);
        //Task InsertUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> NewFollower(User user);
        Task<bool> DeleteUser(int id);
    }
}