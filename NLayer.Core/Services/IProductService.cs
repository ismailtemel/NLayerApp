using NLayer.Core.DTOs;
using NLayer.Core.Models;

namespace NLayer.Core.Services
{
    public interface IProductService : IService<Product>
    {
        // Burda product dönemeyeceğiz. Özelleştirilmiş bir response döneceğiz yani product'la beraber kategoriyi de döneceğiz.
        Task<CustomResponseDto<List<ProductWithCategoryDto>>> GetProductListWithCategory();
    }
}
