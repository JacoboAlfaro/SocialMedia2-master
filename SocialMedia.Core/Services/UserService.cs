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
        private readonly IPostService _postService;

        public UserService(IUnitOfWork unitOfWork, IOptions<PaginationOptions> options, IPostService postService)
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
            _postService = postService;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _unitOfWork.UserRepository.GetById(id);
            
            if (user == null)
            {
                throw new BusinessExceptions("Usuario no encontrado");
            }

            var posts = await _unitOfWork.PostRepository.GetSummaryPostsByUserId(id);
            user.Posts = posts.OrderByDescending(x => x.Date).ToList();

            foreach(var post in user.Posts)
            {
                var postComments = await _unitOfWork.CommentRepository.GetCommentsByPostId(post.Id);
                post.Comments = postComments.ToList();
                if (post.Image != null)
                {
                    post.Image = _postService.GetImageAsBase64(post.Image);
                }
            }

            var comments = await _unitOfWork.CommentRepository.GetSummaryCommentsByUserId(id);
            user.Comments = comments.OrderByDescending(x => x.Date).ToList();

            
            return user;

        }
        public async Task<PagedList<User>> GetUsers(UserQueryFilter filters)
        {
            filters.PageNumber = filters.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _paginationOptions.DefaultPageSize : filters.PageSize;

            var users = _unitOfWork.UserRepository.GetAll();


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
            if (filters.IsActive.HasValue)
            {
                users = users.Where(x => x.IsActive == filters.IsActive.Value);
            }
            var pagedUser = PagedList<User>.Create(users, filters.PageNumber, filters.PageSize);
            foreach(var user in pagedUser)
            {
                var posts = await _unitOfWork.PostRepository.GetSummaryPostsByUserId(user.Id);
                user.Posts = posts.ToList();
                var comments = await _unitOfWork.CommentRepository.GetSummaryCommentsByUserId(user.Id);
                user.Comments = comments.ToList();
            }
            return pagedUser;
        }

        //public async Task InsertUser(User user)
        //{
        //    user.IsActive = true;
        //    if ((DateTime.Now - user.DateOfBirth).TotalDays <= 0)
        //    {
        //        throw new BusinessExceptions("The date of birth can not pass the actual date");
        //    } else if((DateTime.Now - user.DateOfBirth).TotalDays < 6571)
        //    {
        //        throw new BusinessExceptions("You need to have at least 18 years to create a user");
        //    }

        //    if (!user.Email.EndsWith("@gmail.com"))
        //    {
        //        throw new BusinessExceptions("Email must end with @gmail.com");
        //    }

        //    if (user.Telephone.Length != 10)
        //    {
        //        throw new BusinessExceptions("Number must have exactly 10 numbers");
        //    }
            
        //    await _unitOfWork.UserRepository.Add(user);
        //    await _unitOfWork.SaveChangesAsync();
        //}

        public async Task<bool> UpdateUser(User user)
        {
            var existingUser = await _unitOfWork.UserRepository.GetById(user.Id);
            if (existingUser == null)
            {
                throw new BusinessExceptions("El Usuario que desea modificar no existe");
            }
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Telephone = user.Telephone;
            existingUser.Email = user.Email;
            existingUser.IsActive = user.IsActive;

            _unitOfWork.UserRepository.Update(existingUser);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<int> NewFollower(User user)
        {
            var existingUser = await _unitOfWork.UserRepository.GetById(user.Id);

            if (existingUser == null)
            {
                throw new BusinessExceptions("No es posible seguir a este usuario");
            }

            existingUser.Followers = user.Followers + 1;

            _unitOfWork.UserRepository.Update(existingUser);
            await _unitOfWork.SaveChangesAsync();
            return existingUser.Followers;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _unitOfWork.UserRepository.GetById(id);

            if (user == null)
            {
                throw new BusinessExceptions("El Usuario que desea eliminar no existe");
            }
            await _unitOfWork.UserRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}