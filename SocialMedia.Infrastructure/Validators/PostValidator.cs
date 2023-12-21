using FluentValidation;
using FluentValidation.Validators;
using SocialMedia.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Infrastructure.Validators
{
    public class PostValidator : AbstractValidator<PostDto>
    {
        public PostValidator()
        {
            RuleFor(Post => Post.Title)
                .MaximumLength(100)
                .WithMessage("La longitud del titulo no debe superar los 100 caracteres");

            RuleFor(Post => Post.Title)
                .NotNull()
                .WithMessage("El titulo no puede ser nulo");

            RuleFor(Post => Post.Description)
                .NotNull()
                .WithMessage("La descripcion no puede ser nulo");

            RuleFor(Post => Post.Description)
                .Length(10,1000)
                .WithMessage("La longitud de la descripcion debe estar entre 10 y 1000 caracteres");

            RuleFor(Post => Post.Date)
                .NotNull()
                .LessThan(DateTime.Now);
        }
    }
}
