using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Entities;
using SocialMedia.Core.QueryFilters;
using System.Threading.Tasks;

namespace SocialMedia.Core.Interfaces
{
    public interface ISecurityService
    {
        Task<PagedList<Security>> GetLogins(SecurityQueryFilter filters);
        Task<Security> GetLogin(int id);
        Task<Security> GetLoginByUserId(int id);
        Task<Security> GetLoginByCredentials(UserLogin userLogin);
        Task RegisterUser(Security security);
        
    }
}