using Microsoft.Extensions.Options;
using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Enumerations;
using SocialMedia.Core.Exceptions;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace SocialMedia.Core.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOptions _paginationOptions;

        public SecurityService(IUnitOfWork unitOfWork, IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;

        }
        public async Task<Security> GetLoginByCredentials(UserLogin userLogin)
        {
            return await _unitOfWork.SecurityRepository.GetLoginByCredentials(userLogin);
        }

        public async Task RegisterUser(Security security)
        {
            var roles = Enum.GetValues(typeof(RoleType));

            if (security.Role.GetType() != roles.GetType())
            {
                throw new BusinessExceptions("Rol no existente");
            }
            await _unitOfWork.SecurityRepository.Add(security);
            await _unitOfWork.SaveChangesAsync();
        }
        public PagedList<Security> GetLogins(SecurityQueryFilter filters)
        {
            filters.PageNumber = filters.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _paginationOptions.DefaultPageSize : filters.PageSize;

            var logins = _unitOfWork.SecurityRepository.GetAll();

            if (filters.User != null)
            {
                logins = logins.Where(x => x.User.ToLower() == filters.User.ToLower());
            }
            if (filters.UserName != null)
            {
                logins = logins.Where(x => x.UserName.ToLower() == filters.UserName.ToLower());
            }
            //var roles = Enum.GetValues(typeof(RoleType));
            if (filters.Role != null)
            {
                logins = logins.Where(x => x.Role.ToString() == filters.Role.ToString());
            }
            var pagedUser = PagedList<Security>.Create(logins, filters.PageNumber, filters.PageSize);
            return pagedUser;
        }
    }
}
