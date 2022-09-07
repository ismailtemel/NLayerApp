using AutoMapper;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;

namespace NLayer.Service.Services
{
    public class ProductService : Service<Product>, IProductService
    {
        private readonly IProductRespository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IGenericRepository<Product> repository, IUnitOfWork unitOfWork, IProductRespository productRepository, IMapper mapper) : base(repository, unitOfWork)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        // Aşağıdaki değişkliği alıp productservice de ekliyoruz.
        public async Task<List<ProductWithCategoryDto>> GetProductListWithCategory()
        {
            // Yanlız bu method geriye bir list product dönüyor bizim dönüş tipimiz bizden dto bekliyor elimizde zaten mapper var biz burda dönüştürme işlemini başarılı bir şekilde yapabiliriz.Hatta biz burda bir tık daha ilerisi olan custom response dönebiliriz.Madem api'miz bizden customresponse bekliyor. Customresponse'ı direk ProductService de oluşturabiliriz sürekli yazmamak için ve best practices olması için.IProductService'imiz bize list dönüyordu burda sadece list değil biz orda direk customresponse'nin içerisinde döneceğiz.
            // Aşağıda tam olarak api'nin istemiş olduğu data'yı döndük.Çünkü bussines kodu service katmanında yazarız.Yani bir try catch kullanacaksak burda yazarız.Tüm bussiness service katmanında dönecek.E o zaman direk apinin istemiş olduğu datayı döneriz.
            // Dikkat edersek artık bizim repolarımızın dönüş tipiyle servislerimizin dönüş tipi farklılaşmaya başladı.Generic de aynı gibi duruyor ama işin içine custom sorgular girdiği zaman service katmanı gerçek görevini yapmaya başlıyor ve tam ihtiyaç olan datayı dönebiliyoruz.
            // Artık mvc projesi yaptığımız için bizim customresponsedto'ya ihtiyacımız yok
            var products = await _productRepository.GetProductListWithCategory();
            var productsDto = _mapper.Map<List<ProductWithCategoryDto>>(products);
            // Artık aşağıdaki kod satırını da dönememize gerek yok.
            //return CustomResponseDto<List<ProductWithCategoryDto>>.Success(200, productsDto);
            return productsDto;
        }
    }
}
