using AutoMapper;
using Microsoft.Extensions.Options;
using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.DTOs;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Enumerations;
using SocialMedia.Core.Exceptions;
using SocialMedia.Core.Interfaces;
using SocialMedia.Core.QueryFilters;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SocialMedia.Core.Services
{
    public class PostService : IPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PaginationOptions _paginationOptions;


        public PostService(IUnitOfWork unitOfWork, IOptions<PaginationOptions> options, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
            _mapper = mapper;
        }

        public async Task<Post> GetPost(int id, bool isImageBase64)
        {
            Post post = await _unitOfWork.PostRepository.GetById(id);

            if (post == null)
            {
                throw new BusinessExceptions("Post no encontrado o no existe");
            }

            if (post.Image != null)
            {
                if (isImageBase64)
                {
                post.Image = GetImageAsBase64(post.Image);
                }
            }

            post.User = await _unitOfWork.UserRepository.GetSummaryUserByUserId(post.UserId);
            var comments = await _unitOfWork.CommentRepository.GetCommentsByPostId(id);
            post.Comments = comments.OrderByDescending(x => x.Date).ToList();
            return post;
        }

        public async Task<PagedList<Post>> GetPosts(PostQueryFilter filters)
        {
            filters.PageNumber = filters.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _paginationOptions.DefaultPageSize : filters.PageSize;

            var postOrdered = _unitOfWork.PostRepository.GetAll();
            var posts = postOrdered.OrderByDescending(x => x.Date);


            if (filters.UserId != null)
            {
                posts = posts.Where(x=> x.UserId == filters.UserId).OrderByDescending(x => x.Date);
            }
            if (filters.Date != null)
            {
                posts = posts.Where(x => x.Date.ToShortDateString() == filters.Date?.ToShortDateString()).OrderByDescending(x => x.Date);
            }
            if (filters.Title != null)
            {
                posts = posts.Where(x => x.Title.ToLower().Contains(filters.Title.ToLower())).OrderByDescending(x => x.Date);
            }
            if (filters.Description != null)
            {
                posts = posts.Where(x => x.Description.ToLower().Contains(filters.Description.ToLower())).OrderByDescending(x => x.Date);
            }

            var pagedPost = PagedList<Post>.Create(posts, filters.PageNumber, filters.PageSize);

            foreach (var post in pagedPost)
            {
                post.User = await _unitOfWork.UserRepository.GetSummaryUserByUserId(post.UserId);
                var comments = await _unitOfWork.CommentRepository.GetCommentsByPostId(post.Id);
                post.Comments = comments.ToList();
                if (post.Image != null)
                {
                    post.Image = GetImageAsBase64(post.Image.ToString());
                }
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

            var userPost = await _unitOfWork.PostRepository.GetPostsByUser(post.UserId);
            if (userPost.Count() < 10)
            {
                var lastPost = userPost.OrderByDescending(x => x.Date).FirstOrDefault();
                if ((DateTime.Now - lastPost.Date).TotalDays < 7)
                {
                    throw new BusinessExceptions("No tiene permitido publicar más de un Post por semana si no tiene más de 10 posts creados ("+ userPost.Count() + ")");
                }
            }
            if (typeof(NoValidContentWords).GetEnumNames().Any(word => post.Title.ToLower().Contains(word)))
            {
                throw new BusinessExceptions($"Contenido no permitido o inadecuado: '{typeof(NoValidContentWords).GetEnumNames().FirstOrDefault(word => post.Description.ToLower().Contains(word))}'");
            }

            if (typeof(NoValidContentWords).GetEnumNames().Any(word => post.Description.ToLower().Contains(word)))
            {
                throw new BusinessExceptions($"Contenido no permitido o inadecuado: '{typeof(NoValidContentWords).GetEnumNames().FirstOrDefault(word => post.Description.ToLower().Contains(word))}'" );
            }

            if (post.Image != null)
            {
                var image = new ImageFIle();
                var imageParts = post.Image.Split('#');
                if (imageParts.Length == 2)
                {
                    image.Name = imageParts[0];
                    image.Src = imageParts[1];
                }
                else
                {
                    image = null;
                }

                if (image != null)
                {
                    var imagenMovida = SaveImage(image);
                    if (imagenMovida.Item1)
                    {
                        var imgPrefix = imagenMovida.Item2;
                        post.Image = imgPrefix;

                        await _unitOfWork.PostRepository.Add(post);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        throw new BusinessExceptions("Imagen ya existe en la BD, cambie el nombre de la imagen o ingrese una nueva");
                    }
                }
                else
                {
                    await _unitOfWork.PostRepository.Add(post);
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            else
            {
                await _unitOfWork.PostRepository.Add(post);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<bool> UpdatePost(Post post)
        {
            var existingPost = await _unitOfWork.PostRepository.GetById(post.Id);

            if (existingPost == null)
            {
                throw new BusinessExceptions("El Post que desea actualizar no existe");
            }
            if(post.Image == null && existingPost.Image != null)
            {
                throw new BusinessExceptions("Error Gay");
            }
            if (post.Image != null)
            {
                var image = new ImageFIle();
                var imageParts = post.Image.Split('#');
                if (imageParts.Length == 2)
                {
                    image.Name = imageParts[0];
                    image.Src = imageParts[1];
                }
                else
                {
                    image = null;
                }

                if (image != null)
                {
                    var imagenMovida = SaveImage(image);
                    if (imagenMovida.Item1)
                    {
                        var imgPrefix = imagenMovida.Item2;
                        post.Image = imgPrefix;

                        _unitOfWork.PostRepository.Update(existingPost);
                        await _unitOfWork.SaveChangesAsync();
                    }
                    else
                    {
                        throw new BusinessExceptions("Imagen ya existe en la BD para actualizar, cambie el nombre de la imagen o ingrese una nueva");
                    }
                }
                else
                {
                     _unitOfWork.PostRepository.Update(existingPost);
                    await _unitOfWork.SaveChangesAsync();
                }
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

            var comments = await _unitOfWork.CommentRepository.GetCommentsByPostId(id);
            post.Comments = comments.ToList();

            if (post == null)
            {
                throw new BusinessExceptions("El Post que desea eliminar no existe");
            }
            if (post.Comments.Count != 0)
            {
                throw new BusinessExceptions("El Post no se puede eliminar ya que tiene comentarios asociados a él");
            }
            await _unitOfWork.PostRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public (bool,string) SaveImage(ImageFIle image)
        {
            var srcParts = image.Src.Split(',');
            var part1_2Base64 = srcParts[0];
            var a = part1_2Base64.Split('/');


            if (srcParts.Length != 2)
            {
                throw new BusinessExceptions("Formato base64 no coincide");
            }

            byte[] imageData = Convert.FromBase64String(srcParts[1]);
            var postImagePath = Path.Combine("..", "Images", "Posts", "PostImages");
            var imgPrefix = a[1] +"$post_Img_" + image.Name;
            //throw new BusinessExceptions(imgPrefix);

            //if (File.Exists(Path.Combine(postImagePath, imgPrefix)))
            //{
            //    return (false, null);
            //}

            File.WriteAllBytes(Path.Combine(postImagePath, imgPrefix), imageData);
            return (true, imgPrefix);
        }

        public string GetImageAsBase64(string imageName)
        {
            // Construye la ruta del archivo
            var imagePath = Path.Combine("..", "Images", "Posts", "PostImages", imageName);

            // Comprueba si el archivo existe
            if (!File.Exists(imagePath))
            {
                return null;
            }

            // Lee los bytes del archivo
            byte[] imageData = File.ReadAllBytes(imagePath);

            // Convierte los bytes a base64
            string imageBase64 = Convert.ToBase64String(imageData);

            var part1Base64 = imageName.Split('$');
            // Devuelve la base64
            return $"{part1Base64[0]},{imageBase64}";
        }
    }
}
