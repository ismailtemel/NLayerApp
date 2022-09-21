using NLayer.Core.Models;

namespace NLayer.Core.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        // Geriye task döndüğü zaman asenkron olduğunu belirtmek için methodlarımızın sonuna async ekleriz.
        Task<Category> GetSingleCategoryByIdWithProductsAsync(int categoryId);
    }
}
