using Microsoft.Extensions.Options;
using SocialMedia.Core.CustomEntities;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Enumerations;
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
    public class CategoryService: ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PaginationOptions _paginationOptions;

        public CategoryService(IUnitOfWork unitOfWork, IOptions<PaginationOptions> options)
        {
            _unitOfWork = unitOfWork;
            _paginationOptions = options.Value;
        }


        public async Task<Category> GetCategory(int id)
        {
            Category category = await _unitOfWork.CategoryRepository.GetById(id);

            if (category == null)
            {
                throw new BusinessExceptions("Categoria no encontrada");
            }

            return category;
        }

        public async Task<PagedList<Category>> GetCategoriesAsync(CategoryQueryFilter filters)
        {
            filters.PageNumber = filters.PageNumber == 0 ? _paginationOptions.DefaultPageNumber : filters.PageNumber;
            filters.PageSize = filters.PageSize == 0 ? _paginationOptions.DefaultPageSize : filters.PageSize;

            var categories = _unitOfWork.CategoryRepository.GetAll();

           
            if (filters.Name != null)
            {
                categories = categories.Where(x => x.Name.ToLower().Contains(filters.Name.ToLower()));
            }


            var pagedCategories = PagedList<Category>.Create(categories, filters.PageNumber, filters.PageSize);

            return pagedCategories;
        }

        public async Task InsertCategory(Category category)
        {
            var existingNameCategory = await _unitOfWork.CategoryRepository.GetCategoryByName(category);

            if(category.Name == null)
            {
                throw new BusinessExceptions("El nombre no puede estar vacío");

            }

            if (category.Color == null)
            {
                throw new BusinessExceptions("EL color no puede estar vacío");

            }

            if (typeof(NoValidContentWords).GetEnumNames().Any(word => category.Name.ToLower().Contains(word)))
            {
                throw new BusinessExceptions($"Contenido no permitido o inadecuado: ' {typeof(NoValidContentWords).GetEnumNames().FirstOrDefault(word => category.Name.ToLower().Contains(word))} '");
            }

            if (category.Color.Length < 6)
            {
                throw new BusinessExceptions("Color no permitido");
            }

            if (existingNameCategory != null)
            {
                throw new BusinessExceptions($"Categoria ya existe, por favor cambiarle el nombre para poder crearla");
            }

            await _unitOfWork.CategoryRepository.Add(category);
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task<bool> UpdateCategory(Category category)
        {
            var existingCategory = await _unitOfWork.CategoryRepository.GetById(category.Id);
            if (existingCategory == null)
            {
                throw new BusinessExceptions("La categoria que desea actualizar no existe");
            }
            if (typeof(NoValidContentWords).GetEnumNames().Any(word => category.Name.ToLower().Contains(word)))
            {
                throw new BusinessExceptions($"Contenido no permitido o inadecuado: ' {typeof(NoValidContentWords).GetEnumNames().FirstOrDefault(word => category.Name.ToLower().Contains(word))} '");
            }
            existingCategory.Name = category.Name;
            existingCategory.Color = category.Color;

            _unitOfWork.CategoryRepository.Update(existingCategory);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteCategory(int id)
        {
            Category category = await _unitOfWork.CategoryRepository.GetById(id);

            if (category == null)
            {
                throw new BusinessExceptions("La categoria que desea eliminar no existe");
            }
            await _unitOfWork.CategoryRepository.Delete(id);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
