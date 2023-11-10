using FluentValidation;
using SocialMedia.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Infrastructure.Validators
{
    public class SecurityValidator : AbstractValidator<SecurityDto>
    {
        public SecurityValidator()
        {
            RuleFor(Security => Security.User.FirstName)
                .NotNull()
                .WithMessage("El nombre no puede ser nulo");

            RuleFor(Security => Security.User.FirstName)
                .MaximumLength(50)
                .WithMessage("El Nombre puede tener como  maximo 50 caracteres");

            RuleFor(Security => Security.User.LastName)
                .NotNull()
                .WithMessage("El apellido no puede ser nulo");

            RuleFor(Security => Security.User.LastName)
                .MaximumLength(50)
                .WithMessage("El Apellido puede tener como  maximo 50 caracteres");

            RuleFor(Security => Security.User.Email)
                .NotNull()
                .Matches("@gmail.com")
                .WithMessage("El correo debe terminar en @gmail.com");

            RuleFor(Security => Security.User.Email)
                .MaximumLength(30)
                .WithMessage("El correo puede tener como  maximo 30 caracteres");

            RuleFor(Security => Security.User.Telephone)
               .NotNull()
               .WithMessage("El telefono es requerido");

            RuleFor(Security => Security.User.Telephone)
                .Length(10)
                .WithMessage("La longitud del telefono debe ser de 10 digitos");

            RuleFor(Security => Security.User.DateOfBirth)
                .NotNull()
                .LessThan(DateTime.Now)
                .WithMessage("La fecha de nacimiento no puede ser superior a la actual");

        }
    }
}
