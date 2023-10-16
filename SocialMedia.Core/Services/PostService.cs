using Microsoft.Extensions.Options;
using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Exceptions;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Core.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOptions _paginationOptions;


        public PostService(IUnitOfWork unitOfWork, IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<Post> GetPost(int id)
        {
            Post post = await _unitOfWork.PostRepository.GetById(id);
            post.User = await _unitOfWork.UserRepository.GetSummaryUserByUserId(post.UserId);
            var comments = await _unitOfWork.CommentRepository.GetCommentsByPostId(id);
            post.Comments = comments.ToList();
            return post;
        }

        public async Task<PagedList<Post>> GetPosts(PostQueryFilter filters)
        {
            filters.PageNumber = filters.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _paginationOptions.DefaultPageSize : filters.PageSize;

            var posts = _unitOfWork.PostRepository.GetAll();


            if (filters.UserId != null)
            {
                posts = posts.Where(x=> x.UserId == filters.UserId);
            }
            if (filters.Date != null)
            {
                posts = posts.Where(x => x.Date.ToShortDateString() == filters.Date?.ToShortDateString());
            }
            if (filters.Description != null)
            {
                posts = posts.Where(x => x.Description.ToLower().Contains(filters.Description.ToLower()));
            }

            var pagedPost = PagedList<Post>.Create(posts, filters.PageNumber, filters.PageSize);

            foreach (var post in pagedPost)
            {
                post.User = await _unitOfWork.UserRepository.GetSummaryUserByUserId(post.UserId);
                var comments = await _unitOfWork.CommentRepository.GetCommentsByPostId(post.Id);
                post.Comments = comments.ToList();
            }
            return pagedPost;
        }

        public async Task InsertPost(Post post)
        {
            var user = await _unitOfWork.UserRepository.GetById(post.UserId);
            if (user == null)
            {
                throw new BusinessExceptions("User doesn't exist");
            }
            //if(post.Date == null)
            //{
            //    string nuevoFormato = post.Date.ToString("yyyy-MM-ddTHH:mm:ss.ff");
            //    post.Date = DateTime.ParseExact(nuevoFormato, "yyyy-MM-ddTHH:mm:ss.ff", CultureInfo.InvariantCulture);
            //}

            var userPost = await _unitOfWork.PostRepository.GetPostsByUser(post.UserId);
            if (userPost.Count() < 10)
            {
                var lastPost = userPost.OrderByDescending(x=> x.Date).FirstOrDefault();
                if ((DateTime.Now - lastPost.Date).TotalDays < 7)
                {
                    throw new BusinessExceptions("You are no able to publish the post");
                }

            }

            if (post.Description.ToLower().Contains("sexo"))
            {
                throw new BusinessExceptions("Content not allowed");
            }
            await _unitOfWork.PostRepository.Add(post);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdatePost(Post post)
        {
            var existingPost = await _unitOfWork.PostRepository.GetById(post.Id);
            existingPost.Image = post.Image;
            existingPost.Description = post.Description;

            _unitOfWork.PostRepository.Update(existingPost);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePost(int id)
        {
            await _unitOfWork.PostRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
