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
        public async Task<Security> GetLogin(int id)
        {
            var login = await _unitOfWork.SecurityRepository.GetById(id);

            if (login == null)
            {
                throw new BusinessExceptions("Login doesn't exist");
            }
            login.User = await _unitOfWork.UserRepository.GetSummaryUserByUserId(login.UserId);
            return login;
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
        public async Task<PagedList<Security>> GetLogins(SecurityQueryFilter filters)
        {
            filters.PageNumber = filters.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _paginationOptions.DefaultPageSize : filters.PageSize;

            var logins = _unitOfWork.SecurityRepository.GetAll();

            if (filters.UserLogin != null)
            {
                logins = logins.Where(x => x.UserLogin.ToLower() == filters.UserLogin.ToLower());
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
            var pagedLogins = PagedList<Security>.Create(logins, filters.PageNumber, filters.PageSize);
            foreach(var login in pagedLogins)
            {
                login.User = await _unitOfWork.UserRepository.GetSummaryUserByUserId(login.UserId);
            }
            return pagedLogins;
        }
    }
}
