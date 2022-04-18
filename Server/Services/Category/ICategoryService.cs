using ElevenNoteWebApp.Shared.Models.Category;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElevenNoteWebApp.Server.Services.CategoryService
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryListItem>> GetAllCategoriesAsync();
        Task<bool> CreateCategoryAsync(CategoryCreate model);
        Task<CategoryDetail> GetCategoryByIdAsync(int categoryId);
        Task<bool> UpdateCategoryAsync(CategoryEdit model);
        Task<bool> DeleteCategoryAsync(int categoryId);

    }
}
