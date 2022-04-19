using ElevenNoteWebApp.Server.Data;
using ElevenNoteWebApp.Server.Models;
using ElevenNoteWebApp.Shared.Models.Category;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElevenNoteWebApp.Server.Services.CategoryService
{
    public class CategoryService : ICategoryService

    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateCategoryAsync(CategoryCreate model)
        {
            if (model == null) return false;

            var categoryEntity = new Category
            {
                Name = model.Name,
            };

            _context.Categories.Add(categoryEntity);

            return await _context.SaveChangesAsync() == 1;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var categoryEntity = await _context.Categories.FindAsync(categoryId);
            _context.Categories.Remove(categoryEntity);
            return await _context.SaveChangesAsync() == 1;
        }

        public async Task<IEnumerable<CategoryListItem>> GetAllCategoriesAsync()
        {
            var categoryQuery = _context.Categories.Select(entity => new CategoryListItem
            {
                Id = entity.Id,
                Name = entity.Name,
            });

            return await categoryQuery.ToListAsync();
        }

        public async Task<CategoryDetail> GetCategoryByIdAsync(int categoryId)
        {
            var categoryEntity = await _context.Categories.FindAsync(categoryId);
            if (categoryEntity == null) return null;

            var categoryDetail = new CategoryDetail
            {
                Id =categoryEntity.Id,
                Name = categoryEntity.Name,
            };
            return categoryDetail;
        }

        public async Task<bool> UpdateCategoryAsync(CategoryEdit model)
        {
            if (model == null) return false;

            var category = await _context.Categories.FindAsync(model.Id);

            category.Name = model.Name;

            return await _context.SaveChangesAsync() == 1;

        }
    }
}
