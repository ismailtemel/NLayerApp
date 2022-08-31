using AutoMapper;
using NLayer.Core.DTOs;
using NLayer.Core.Models;

namespace NLayer.Service.Mapping
{
    // Mapping'imiz hazır bunu api'da haberdar etmemiz gerekiyor.
    public class MapProfile :Profile
    {
        public MapProfile()
        {
            // Aşağıdaki kodda reversemap'e kadar product'ı productdto'ya döüştürme yaparız ReverseMap ile ProductDto'yu product'a dönüştürürüz.
            CreateMap<Product,ProductDto>().ReverseMap();
            CreateMap<Category,CategoryDto>().ReverseMap();
            CreateMap<ProductFeature,ProductFeatureDto>().ReverseMap();
            // Bize herhangi bir yerde ProductUpdateDto gelirse güncelleme işlemi için bunu product'a dönüştür deriz ama ReverseMap yapmamıza gerek yok çünkü biz herhangi bir yerden productupdatedto gelirse bunu entity dönüştürmek için alacağız ama şuanda ihtiyacımız olmadığı için ReverseMap koymadık.
            CreateMap<ProductUpdateDto,Product>();
        }
    }
}
