using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Services;

namespace NLayer.API.Controllers
{
    // Aşağıda controller seviyesine kadar gelmiş action ifadesini kullanmamış eğer ki action olsaydı aşağıda yazdığımız methodların ismini endpointlere istek yaparken mutlaka yazmamız gerekirdi.Ama action olmadığından dolayı bizim frameworkümüz istek yaparken methodun tipine göre eşleyecek eğer bir get isteği yaparsak aşağıda çalıştırmak istediğimiz methdolardan biri gelir. Ama nasıl bir get isteği yaparsak örneği ilk methodun üsütnde yazıyor.
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : CustomBaseController
    {
        private readonly IMapper _mapper;
        // Controllerlar sadece servisleri bilir. Constructorlarında kesinlikle repoyu referans almazlar.
        private readonly IService<Product> _service;
        public ProductsController(IMapper mapper, IService<Product> service)
        {
            _mapper = mapper;
            _service = service;
        }
        ///GET api/products
        [HttpGet]
        public async Task<IActionResult> All()
        {
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var products = await _service.GetByIdAsync(id);
            var productsDto = _mapper.Map<ProductDto>(products);
            //return Ok(CustomResponseDto<List<ProductDto>>.Success(200, productsDtos));
            // Artık Ok,badrequest gibi bildirimlerden kurtulduk.
            return CreateActionResult(CustomResponseDto<ProductDto>.Success(200, productsDto));
            // Geriye 200 mü 404 mü döneceğini nerden anlarız şurdan anlarız 200 gönderirsek geriye ok , 404 gönderirsek badrequest veya farklı birşey döner.3 satırda layı daha temiz bir hale getirdik.
        }
        [HttpPost()]
        public async Task<IActionResult> Save(ProductDto productDto)
        {
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
