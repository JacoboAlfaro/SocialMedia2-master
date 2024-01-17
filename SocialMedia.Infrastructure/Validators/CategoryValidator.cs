using FluentValidation;
using SocialMedia.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialMedia.Infrastructure.Validators
{
    class CategoryValidator: AbstractValidator<CategoryDto>
    {
        public CategoryValidator()
        {
            RuleFor(Category => Category.Name)
                .MaximumLength(50)
                .WithMessage("La longitud del nombre de la categoria no debe superar los 50 caracteres");

            RuleFor(Category => Category.Name)
                .NotNull()
                .WithMessage("El Nombre no puede ser nulo");

            RuleFor(Category => Category.Color)
                .MaximumLength(8)
                .WithMessage("La longitud del color no debe superar los 8 caracteres");

            RuleFor(Category => Category.Color)
                .NotNull()
                .WithMessage("El Color no puede ser nulo");
        }
    }
}
