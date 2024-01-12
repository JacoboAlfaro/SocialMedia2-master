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
        //Metodo para obtener un login por su Id
        public async Task<Security> GetLogin(int id)
        {
            var login = await _unitOfWork.SecurityRepository.GetById(id);

            //Valida si el login existe
            if (login == null)
            {
                throw new BusinessExceptions("Login no encontrado");
            }

            //Agrega la informacion del usuario obteniendola por el id del usuario
            login.User = await _unitOfWork.UserRepository.GetSummaryUserByUserId(login.UserId);
            return login;
        }

        //Metodo para obtener un login por su userId
        public async Task<Security> GetLoginByUserId(int userId)
        {
            var login = await _unitOfWork.SecurityRepository.GetByUserId(userId);

            //Valida si el login existe
            if (login == null)
            {
                throw new BusinessExceptions("Login no encontrado");
            }

            //Agrega la informacion del usuario obteniendola por el id del usuario
            login.User = await _unitOfWork.UserRepository.GetSummaryUserByUserId(login.UserId);
            return login;
        }

        //Metodo para obtener un login por el userLogin y contrasenia
        public async Task<Security> GetLoginByCredentials(UserLogin userLogin)
        {
            if (userLogin.Password == null)
            {
                throw new BusinessExceptions("La contraseña es obligatoria");
            }
            var login = await _unitOfWork.SecurityRepository.GetLoginByCredentials(userLogin);
            if (login == null)
            {
                throw new BusinessExceptions("Login no encontrado, usuario o cotraseña incorrecta");
            }
            return login;
        }
        //Metodo para obtener el login de la persona que se quiere registrar
        private async Task<Security> isValid(UserLogin userLogin)
        {
            return await _unitOfWork.SecurityRepository.GetLoginByCredentials(userLogin);
        }

        //Metodo para registrar un usuario con info del usuario y del login
        public async Task RegisterUser(Security security)
        {
            ////Poner en db el usuario como unico luego de borrar todos los registros en equipo de casa
            ///
            //Se crea userLogin a partir del objeto security al registrar
            UserLogin userLogin = new UserLogin()
            {
                User = security.UserLogin,
                Password = security.Password
            };
            var validUser = await isValid(userLogin);

            //Se valida si el usuario no existe para poder crear el login
            if(validUser != null)
            {
                throw new BusinessExceptions("Login ya existe, utilice otro LoginUser");

            }
            
            //Se valida si el rol ingresado existe en la enumeracion de roles
            if (!Enum.IsDefined(typeof(RoleType), security.Role))
            {
                throw new BusinessExceptions("Rol no existente");
            }

            //Se validan reglas para el usuario que se crea
            security.User.IsActive = true;
            if ((DateTime.Now - security.User.DateOfBirth).TotalDays <= 0)
            {
                throw new BusinessExceptions("La fecha de nacimiento no puede ser superior a la fecha actual");
            }
            else if ((DateTime.Now - security.User.DateOfBirth).TotalDays < 6571)
            {
                throw new BusinessExceptions("Debe tener al menos 18 años para poder crear un usuario");
            }

            if (!security.User.Email.EndsWith("@gmail.com"))
            {
                throw new BusinessExceptions("Email debe terminar con @gmail.com");
            }

            if (security.User.Telephone.Length != 10)
            {
                throw new BusinessExceptions("El número debe ser de 10 digitos");
            }
            await _unitOfWork.SecurityRepository.Add(security);
            await _unitOfWork.SaveChangesAsync();
        }
        //Metodo para obtener todos los logins existentes
        public async Task<PagedList<Security>> GetLogins(SecurityQueryFilter filters)
        {
            filters.PageNumber = filters.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _paginationOptions.DefaultPageSize : filters.PageSize;

            var logins = _unitOfWork.SecurityRepository.GetAll();

            //Filtros para buscar logins
            if (filters.UserLogin != null)
            {
                logins = logins.Where(x => x.UserLogin.ToLower().Contains(filters.UserLogin.ToLower()));
            }
            if (filters.UserName != null)
            {
                logins = logins.Where(x => x.UserName.ToLower().Contains(filters.UserName.ToLower()));
            }
            if (filters.Role != null)
            {
                logins = logins.Where(x => x.Role.ToString() == filters.Role.ToString());
            }
            //if (filters.FirstName != null)
            //{
            //    logins = logins.Where(x => x.User.FirstName.ToLower().Contains(filters.FirstName.ToLower()));
            //}
            //logins = logins.Where(x => x.User != null && x.User.FirstName.ToLower() == filters.FirstName.ToLower());
            //if (filters.LastName != null)
            //{
            //    logins = logins.Where(x => x.User.LastName.ToLower().Contains(filters.LastName.ToLower()));
            //}
            //if (filters.Email != null)
            //{
            //    logins = logins.Where(x => x.User.Email.ToLower().Contains(filters.Email.ToLower()));
            //}
            //if (filters.Telephone != null)
            //{
            //    logins = logins.Where(x => x.User.Telephone.ToLower().Contains(filters.Telephone.ToLower()));
            //}
            //if (filters.DateOfBirth != null)
            //{
            //    logins = logins.Where(x => x.User?.DateOfBirth.ToShortDateString() == filters.DateOfBirth?.ToShortDateString());
            //}


            //if (filters.IsActive.HasValue)
            //{
            //    //throw new BusinessExceptions($"{filters.IsActive.Value} aaa");
            //    logins = logins.Where(s => s.User.IsActive == filters.IsActive.Value);
            //    throw new BusinessExceptions($"{logins}");
            //}


            //logins = logins.Where(x => x.User != null && x.User.IsActive == filters.IsActive.Value);

            //Recorre el paginado para agregar la informacion del usuario por cada registro 
            var pagedLogins = PagedList<Security>.Create(logins, filters.PageNumber, filters.PageSize);
            foreach(var login in pagedLogins)
            {
                login.User = await _unitOfWork.UserRepository.GetSummaryUserByUserId(login.UserId);
                var posts = await _unitOfWork.PostRepository.GetSummaryPostsByUserId(login.User.Id);
                login.User.Posts = posts.ToList();
                var comments = await _unitOfWork.CommentRepository.GetSummaryCommentsByUserId(login.User.Id);
                login.User.Comments = comments.ToList();
            }

            return pagedLogins;
        }
    }
}