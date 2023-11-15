using Microsoft.Extensions.Options;
using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Exceptions;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia.Core.Services
{
    public class CommentService : ICommentService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOptions _paginationOptions;


        public CommentService(IUnitOfWork unitOfWork, IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }

        public async Task<Comment> GetComment(int id)
        {
            Comment comment = await _unitOfWork.CommentRepository.GetById(id);

            if (comment == null)
            {
                throw new BusinessExceptions("Commentario no encontrado");
            }

            comment.User = await _unitOfWork.UserRepository.GetSummaryUserByUserId(comment.UserId);
            comment.Post = await _unitOfWork.PostRepository.GetSummaryPostByPostId(comment.PostId);
            return comment;
        }

        public async Task<PagedList<Comment>> GetCommentsAsync(CommentQueryFilter filters)
        {
            filters.PageNumber = filters.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _paginationOptions.DefaultPageSize : filters.PageSize;

            var comments = _unitOfWork.CommentRepository.GetAll();

            if (filters.PostId != 0)
            {
                comments = comments.Where(x => x.PostId == filters.PostId);
            }
            if (filters.UserId != 0)
            {
                comments = comments.Where(x => x.UserId == filters.UserId);
            }
            if (filters.Date != null)
            {
                comments = comments.Where(x => x.Date.ToShortDateString() == filters.Date?.ToShortDateString());
            }
            if (filters.Description != null)
            {
                comments = comments.Where(x => x.Description.ToLower().Contains(filters.Description.ToLower()));
            }
            if (filters.IsActive != null)
            {
                comments = comments.Where(x => x.IsActive == filters.IsActive);
            }

            var pagedComments = PagedList<Comment>.Create(comments, filters.PageNumber, filters.PageSize);

            foreach (var comment in pagedComments)
            {
                comment.User = await _unitOfWork.UserRepository.GetSummaryUserByUserId(comment.UserId);
                comment.Post = await _unitOfWork.PostRepository.GetSummaryPostByPostId(comment.PostId);
            }

            return pagedComments;
        }

        public async Task InsertComment(Comment comment)
        {
            comment.IsActive = true;

            var user = await _unitOfWork.UserRepository.GetById(comment.UserId);
            var post = await _unitOfWork.PostRepository.GetById(comment.PostId);

            if (user == null)
            {
                throw new BusinessExceptions("Usuario no encontrado");
            }
            if (post == null )
            {
                throw new BusinessExceptions("Post no encontrado");
            }
            if (comment.Description.ToLower().Contains("sexo"))
            {
                throw new BusinessExceptions("Contenido no permitido o inadecuado");
            }
            if (comment.PostId == 0)
            {
                throw new BusinessExceptions("Post debe existir para realizar un comentario");
            }
            if (comment.UserId == 0)
            {
                throw new BusinessExceptions("Usuario necesario para realizar un comentario");
            }
            if ((DateTime.Now - comment.Date).TotalDays <= 0)
            {
                throw new BusinessExceptions("La fecha no puede ser mayor a la actual");
            }

            await _unitOfWork.CommentRepository.Add(comment);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task<bool> UpdateComment(Comment comment)
        {
            var existingComment = await _unitOfWork.CommentRepository.GetById(comment.Id);
            if(existingComment == null)
            {
                throw new BusinessExceptions("El Comentario que desea actualizar no existe");
            }
            existingComment.Description = comment.Description;
            existingComment.IsActive = comment.IsActive;

            _unitOfWork.CommentRepository.Update(existingComment);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> NewCommentLike(Comment comment)
        {
            var existingComment = await _unitOfWork.CommentRepository.GetById(comment.Id);

            if (existingComment == null)
            {
                throw new BusinessExceptions("No es posible dar like a este comentario");
            }

            existingComment.Likes = comment.Likes + 1;

            _unitOfWork.CommentRepository.Update(existingComment);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteComment(int id)
        {
            Comment comment = await _unitOfWork.CommentRepository.GetById(id);

            if (comment == null)
            {
                throw new BusinessExceptions("El comentario que desea eliminar no existe");
            }
            await _unitOfWork.CommentRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
