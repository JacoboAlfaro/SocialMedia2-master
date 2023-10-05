using Microsoft.Extensions.Options;
using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Exceptions;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOptions _paginationOptions;

        public UserService(IUnitOfWork unitOfWork, IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _unitOfWork.UserRepository.GetById(id);
            var posts = await _unitOfWork.PostRepository.GetPostsByUser(id);
            user.Posts = posts.ToList();
            if (user == null)
            {
                throw new BusinessExceptions("User doesn't exist");
            }
            return user;

        }
        public PagedList<User> GetUsers(UserQueryFilter filters)
        {
            filters.PageNumber = filters.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _paginationOptions.DefaultPageSize : filters.PageSize;

            var users = _unitOfWork.UserRepository.GetAll();

            //foreach (var user in users){
            //    var posts =  _unitOfWork.PostRepository.GetPostsByUser(user.Id);
            //    user.Posts = posts.ToList();
            //}


            if (filters.FirstName != null)
            {
                users = users.Where(x => x.FirstName.ToLower() == filters.FirstName.ToLower());
            }
            if (filters.LastName != null)
            {
                users = users.Where(x => x.LastName.ToLower() == filters.LastName.ToLower());
            }
            if (filters.Email != null)
            {
                users = users.Where(x => x.Email == filters.Email);
            }
            if (filters.DateOfBirth != null)
            {
                users = users.Where(x => x.DateOfBirth.ToShortDateString() == filters.DateOfBirth?.ToShortDateString());
            }
            //if (filters.IsActive.ToString() != null)
            //{
            //    users = users.Where(x => x.IsActive.ToString().ToLower() == filters.IsActive.ToString().ToLower());
            //}

            var pagedUser = PagedList<User>.Create(users, filters.PageNumber, filters.PageSize);
            return pagedUser;
        }

        public async Task InsertUser(User user)
        {
            if ((DateTime.Now - user.DateOfBirth).TotalDays <= 0)
            {
                throw new BusinessExceptions("The date of birth can not pass the actual date");
            } else if((DateTime.Now - user.DateOfBirth).TotalDays < 6571)
            {
                throw new BusinessExceptions("You need to have at least 18 years to create a user");
            }

            if (!user.Email.EndsWith("@gmail.com"))
            {
                throw new BusinessExceptions("Email must end with @gmail.com");
            }

            if (user.Telephone.Length != 10)
            {
                throw new BusinessExceptions("Number must have exactly 10 numbers");
            }
            await _unitOfWork.UserRepository.Add(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateUser(User user)
        {
            var existingUser = await _unitOfWork.UserRepository.GetById(user.Id);
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Telephone = user.Telephone;
            existingUser.Email = user.Email;
            existingUser.IsActive = user.IsActive;

            _unitOfWork.UserRepository.Update(existingUser);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUser(int id)
        {
            await _unitOfWork.UserRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}