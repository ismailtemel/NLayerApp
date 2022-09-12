using NLayer.Core.DTOs;
using NLayer.Core.Models;

namespace NLayer.Core.Services
{
    public interface IProductService : IService<Product>
    {
        // Burda product dönemeyeceğiz. Özelleştirilmiş bir response döneceğiz yani product'la beraber kategoriyi de döneceğiz.
        // Mvc tarafında burayı kullanırken mvc de dto dönmemiz uygun değil customresponse dönüyorduk.Direk olarak dto nesnesini dönmüştük biz bu yüzden tekrar eski haline getirdik.
        Task<CustomResponseDto<List<ProductWithCategoryDto>>> GetProductsWithCategory();
    }
}
