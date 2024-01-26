using Microsoft.EntityFrameworkCore;
using SocialMedia.Core.Entities;
using SocialMedia.Core.Interfaces;
using SocialMedia.Infrastructure.Data;
using System.Threading.Tasks;

namespace SocialMedia.Infrastructure.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(SocialMediaContext context) : base(context) { }

        public async Task<Category> GetCategoryByName(Category category)
        {
            var a = await _entities.FirstOrDefaultAsync(x => x.Name == category.Name);
            return a;
        }
        public async Task<string> GetNameById(int id)
        {
            var category = await _entities.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return "Sin cateogoria#FFF0F5";
            }
            return $"{category.Name}#{category.Color}";
        }
    }
}
