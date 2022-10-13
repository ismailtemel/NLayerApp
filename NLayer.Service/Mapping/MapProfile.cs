using AutoMapper;
using NLayer.Core.DTOs;
using NLayer.Core.Models;

namespace NLayer.Service.Mapping
{
    // Mapping'imiz hazır bunu api'da haberdar etmemiz gerekiyor.
    // Servis katmanımız artık direk dto nesnelerimizi dönüyor ama genericler için değil çünkü orda ortak method olduğu için dto dönmüyorduk. Orayı da halledebiliriz ama bu kursun konusu değil. Token Bazlı kimlik doğrulama kursunda anlatılıyor. O kursta generic yaplar için dto dönüyoruz.
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            // Aşağıdaki kodda reversemap'e kadar product'ı productdto'ya dönüştürme yaparız ReverseMap ile ProductDto'yu product'a dönüştürürüz.
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<ProductFeature, ProductFeatureDto>().ReverseMap();
            // Bize herhangi bir yerde ProductUpdateDto gelirse güncelleme işlemi için bunu product'a dönüştür deriz ama ReverseMap yapmamıza gerek yok çünkü bize herhangi bir yerden productupdatedto gelirse bunu entity dönüştürmek için alacağız ama şuanda ihtiyacımız olmadığı için ReverseMap koymadık.
            CreateMap<ProductUpdateDto, Product>();
            CreateMap<Product, ProductWithCategoryDto>();
            CreateMap<Category, CategoryWithProductsDto>();
        }
    }
}
