using NLayer.Core.DTOs;
using NLayer.Core.Models;

namespace NLayer.Core.Services
{
    public interface ICategoryService : IService<Category>
    {
        // Bu artık bir servis olduğu için bizim burda task ile beraber CustomResponse'uda dönememiz gerekiyor.
        // Burda public'in başında async olduğu için hata alırız burda async'ye ihtiyacımız yoktur.
        // public async Task<CustomResponseDto<CategoryWithProductsDto>> GetSingleCategoryByWidthProductAsync(int categoryId)
        public Task<CustomResponseDto<CategoryWithProductsDto>> GetSingleCategoryByIdWithProductsAsync(int categoryId);
    }
}
