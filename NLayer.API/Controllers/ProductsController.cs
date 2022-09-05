using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NLayer.API.Filter;
using NLayer.API.Filters;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Services;

namespace NLayer.API.Controllers
{
    // Aşağıda controller seviyesine kadar gelmiş action ifadesini kullanmamış eğer ki action olsaydı aşağıda yazdığımız methodların ismini endpointlere istek yaparken mutlaka yazmamız gerekirdi.Ama action olmadığından dolayı bizim frameworkümüz istek yaparken methodun tipine göre eşleyecek eğer bir get isteği yaparsak aşağıda çalıştırmak istediğimiz methdolardan biri gelir. Ama nasıl bir get isteği yaparsak örneği ilk methodun üsütnde yazıyor.
    
    public class ProductsController : CustomBaseController
    {
        // Controllerlar sadece servisleri bilir. Constructorlarında kesinlikle repoyu referans almazlar.
        // Artık aşağıdaki  service'e ihtiyacımız kalmadı. IProductService üzerinden IServicedeki herşeye erişebiliriz.
        // Çünkü IProductService interface'i IService interfacesinden miras alır.

        private readonly IMapper _mapper;
        private readonly IProductService _service;

        public ProductsController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _service = productService;
        }

        //Method isimleri önemli değildir methodun tipine göre eşleşme vardır.
        //GET api/products/GetProductsWithCategory
        //Her seferinde aşağıdaki gibi isim belirtmek zorunda değiliz.Direk action yazarak da kullanabiliriz.
        //İlerde methodun ismini değiştirirsek getin yanına action kodyuğumuz için bir sıkıntı yaşamayız.
        [HttpGet("[action]")]
        public async Task<IActionResult> GetProductsWithCategory()
        {
            // Amacımız controllerlar içerisinde action methodlarda minimum kod barındırmak.Tek satır kodla beraber create action result'ın istemiş olduğu datayı döndük.
            return CreateActionResult(await _service.GetProductListWithCategory());
        }

        //GET api/products
        [HttpGet]
        public async Task<IActionResult> All()
        {
            // Burda iki satır fazlamız var.Çünkü bunlar generic olduğu için dtoyu dönüştürmüyor.Dto yu bu scopelar içerisinde yapmak zorundayız.Artık özelleştirilmiş bir repo ve özelleştirilmiş bir servisimiz olduğu için artık direk olarak api'nin istemiş olduğu datayı dönüyoruz ve tek satıra indirmiş olduk.
            // Aşağıda bulunan 2 satırdaki işlemi servis katmamnında yaparız yani gerçekten olması gereken yerde yaparız.
            var products = await _service.GetAllAsync();
            var productsDtos = _mapper.Map<List<ProductDto>>(products.ToList());
            //return Ok(CustomResponseDto<List<ProductDto>>.Success(200, productsDtos));
            // Artık Ok,badrequest gibi bildirimlerden kurtulduk.
            return CreateActionResult(CustomResponseDto<List<ProductDto>>.Success(200, productsDtos));
            // Geriye 200 mü 404 mü döneceğini nerden anlarız şurdan anlarız 200 gönderirsek geriye ok , 404 gönderirsek badrequest veya farklı birşey döner.3 satırda layı daha temiz bir hale getirdik.
        }
        // Eğer aşağıdaki gibi httpget'de id belirtmezsek id'yi querystringten bekler.Ama aşağıdaki gibi belirtirsek 
        // www.mysite.com/api/products/5 böyle bir sonuç alırız.
        // Get /api/products/5 bunu dersek eğer aşağıdaki method çalışır.Methodun tipine göre ve paramtresine göre bir eşleşme var.
        // O zaman aşağıda getallasync çalışmaz 
        // Burda attribute şekilnde filter tanımlayamayız bunun iki tane sebebi vardır. 1.NotFoundFilter bir attribute sınıfını miras almıyor.Ama validatefilter,action filter attribute'unun dibine gittiğimiz zaman attribute'u miras alıyor ve IAsyncActionFilter interface'ini implemente ediyor.NotFoundFilter ise sadece IAsyncActionFilter'ı implemente ediyor.Nedeni biz dinamik kullanmak istiyoruz ve constructorunda bir parametre geçiyoruz eğer biz bir attribute'a bir filter'a constructorun da parametre geçiyorsak direk kapalı parantezlerle kullanamayız bunu yerine servisfilter üzerinden kullanmamız gerekir.Örnek olarak
        // [ServiceFilter(typeof(NotFoundFilter<Product>))] Burda dikkat edersek biz servicefilter attribute'una bizim filterımızı tip olarak veriyoruz bu yüzden bizim IAsyncActionFilter'ımın validatefilterdaki gibi attribute sınıfını miras almasına gerek yok
        // Son olarak bir filtremiz constructorunda parametre alıyorsa mutlaka servicefilter üzerinden kullanmamız lazım ve servicefilter da belirtmiş olduğumuz tipi de program.cs tarafında servise eklememiz lazım.
        // Kullandığımız bu filter'ın avantajı normalde getbyıd'deki bizim exceptionumuz getbyid'de oluyordu.Gelen request aşağıdaki methodun içerisine girmiş oluyordu.Filter da ise gelen request methodun içerisine girmeden çalışır.İkisinin de kullanım yeri vardır.Her zaman en best practices çalışır.
        [ServiceFilter(typeof(NotFoundFilter<Product>))]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Burda db de olan bir datayı çekmek istediğimizde null gelmez fakat db de olmayan bir datayı çekmek istediğimizde null gelir fakat biz null gelmesini istemiyoruz bu yüzden ilk önce kötü yöntem olan controller içinde if bloklarını kullanırız aşağıdaki gibi. 
            var products = await _service.GetByIdAsync(id);

            //if (products==null)
            //{
                // Bir data bulunamadığı zaman geriye genellikle 404 notfound döneriz.
                // Burda ise null yerine gelmesini istediğimiz sonucu yazdık.Bu if bloklarını burda yazdık fakat buna her bloğun içinde ihtiyacımız olabilir.Bu da controller içinde ciddi bir kirliliğe sebep olur.
            //    return CreateActionResult(CustomResponseDto<ProductDto>.Fail(400, "Bu id'ye sahip ürün bulunamadı"));
            //}

            var productsDto = _mapper.Map<ProductDto>(products);
            //return Ok(CustomResponseDto<List<ProductDto>>.Success(200, productsDtos));
            // Artık Ok,badrequest gibi bildirimlerden kurtulduk.
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(200, productsDto));
            // Geriye 200 mü 404 mü döneceğini nerden anlarız şurdan anlarız 200 gönderirsek geriye ok , 404 gönderirsek badrequest veya farklı birşey döner.3 satırda layı daha temiz bir hale getirdik.
        }
        [HttpPost()]
        public async Task<IActionResult> Save(ProductDto productDto)
        {
            // Methodumuz validaton'a takılırsa scopeların içine yazdığımız kodları çalıştırmaz.
            var products = await _service.AddAsync(_mapper.Map<Product>(productDto));
            var productsDto = _mapper.Map<ProductDto>(products);
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(201, productsDto));
        }

        [HttpPut]
        public async Task<IActionResult> Update(ProductUpdateDto productUpdateDto)
        {
            // Update geriye birşey dönmez. Geriye birşey dönmeyeceği için CustomResponse controllerındaki NoContentDto sınfını kullanırız.
            await _service.UpdateAsync(_mapper.Map<Product>(productUpdateDto));
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
        }

        // DELETE api/products/5 dersek id'si 5 olan kayıt silinir. Eğer delete method tipi yerine get yaparsak yuykarıdaki get methodu çalışıyor delete method tipiyle gönderirsek aynı url'i aşağıdaki method çalışır.Çünkü methodun türüne göre eşleşme var.
        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var product = await _service.GetByIdAsync(id);
            await _service.RemoveAsync(product);
            //Burda birşey dönememize gerek yok aşağıdaki kod satırı gibi
            //var productsDto = _mapper.Map<ProductDto>(products);
            // Burda da geriye birşey dönememize gerek yok
            return CreateActionResult(CustomResponseDto<NoContentDto>.Success(204));
            
        }
    }
}
