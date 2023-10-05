using FluentValidation;
using SocialMedia.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Infrastructure.Validators
{
    public class UserValidator : AbstractValidator<UserDto>
    {
        public UserValidator()
        {
            RuleFor(User => User.FirstName)
                .NotNull()
                .WithMessage("El nombre no puede ser nulo");

            RuleFor(User => User.FirstName)
                .MaximumLength(50)
                .WithMessage("El Nombre puede tener como  maximo 50 caracteres");

            RuleFor(User => User.LastName)
                .NotNull()
                .WithMessage("El apellido no puede ser nulo");

            RuleFor(User => User.LastName)
                .MaximumLength(50)
                .WithMessage("El Apellido puede tener como  maximo 50 caracteres");

            RuleFor(User => User.Email)
                .NotNull()
                .Matches("@gmail.com")
                .WithMessage("El correo debe terminar en @gmail.com");

            RuleFor(User => User.Email)
                .MaximumLength(30)
                .WithMessage("El correo puede tener como  maximo 30 caracteres");

            RuleFor(User => User.Telephone)
               .NotNull()
               .WithMessage("El telefono es requerido");

            RuleFor(User => User.Telephone)
                .Length(10)
                .WithMessage("La longitud del telefono debe ser de 10 digitos");

            RuleFor(User => User.DateOfBirth)
                .NotNull()
                .LessThan(DateTime.Now)
                .WithMessage("La fehca de nacimiento no puede ser superior a la actual");

        }
    }
}
