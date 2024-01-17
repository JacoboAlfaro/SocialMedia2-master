using FluentValidation;
using SocialMedia.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Infrastructure.Validators
{
    public class CommentValidator : AbstractValidator<CommentDto>
    {
        public CommentValidator()
        {
            RuleFor(Comment => Comment.Description)
                .NotNull()
                .WithMessage("La descripcion no puede ser nulo");

            RuleFor(Comment => Comment.Description)
                .Length(10, 500)
                .WithMessage("La longitud de la descripcion debe estar entre 10 y 500 caracteres");

            RuleFor(Comment => Comment.Categories)
                .MaximumLength(1000)
                .WithMessage("El post no puede tener mas de 50 categorias");

            RuleFor(Comment => Comment.Date)
                .NotNull()
                .LessThan(DateTime.Now)
                .WithMessage("La fecha no puede ser superior a la actual");
        }
    }
}
