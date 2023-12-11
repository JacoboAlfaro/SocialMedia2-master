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

            if (post == null)
            {
                throw new BusinessExceptions("Post no encontrado o no existe");
            }

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
            if (filters.Title != null)
            {
                posts = posts.Where(x => x.Title.ToLower().Contains(filters.Title.ToLower()));
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
            post.IsEdit = false;
            var user = await _unitOfWork.UserRepository.GetById(post.UserId);
            if (user == null)
            {
                throw new BusinessExceptions("No es posible publicar Post, usuario no existe");
            }

            if (post.Title == null)
            {
                throw new BusinessExceptions("El titulo del post es requerido para ser creado");
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
                    throw new BusinessExceptions("No tiene permitido publicar un post, no puede publicar más de 10 posts en un semana");
                }

            }

            if (post.Description.ToLower().Contains("sexo"))
            {
                throw new BusinessExceptions("Contenido no permitido o inadecuado");
            }
            await _unitOfWork.PostRepository.Add(post);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdatePost(Post post)
        {
            var existingPost = await _unitOfWork.PostRepository.GetById(post.Id);

            if (existingPost == null)
            {
                throw new BusinessExceptions("El Post que desea actualizar no existe");
            }

            existingPost.Image = post.Image;
            existingPost.Title = post.Title;
            existingPost.Description = post.Description;
            existingPost.IsEdit = true;


            _unitOfWork.PostRepository.Update(existingPost);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<int> NewLike(Post post)
        {
            var existingPost = await _unitOfWork.PostRepository.GetById(post.Id);

            if (existingPost == null)
            {
                throw new BusinessExceptions("No es posible dar like a este post");
            }

            existingPost.Likes = post.Likes + 1;

            _unitOfWork.PostRepository.Update(existingPost);
            await _unitOfWork.SaveChangesAsync();
            return existingPost.Likes;
        }

        public async Task<bool> DeletePost(int id)
        {
            Post post = await _unitOfWork.PostRepository.GetById(id);

            if (post == null)
            {
                throw new BusinessExceptions("El Post que desea eliminar no existe");
            }
            await _unitOfWork.PostRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
