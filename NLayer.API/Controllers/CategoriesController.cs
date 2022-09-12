using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NLayer.Core.DTOs;
using NLayer.Core.Services;

namespace NLayer.API.Controllers
{

    public class CategoriesController : CustomBaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;


        public CategoriesController(ICategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();

            // Burayı bir dtoya çevirmemiz gerekiyor.Aşağıdaki method custom bir method olduğu için geriye direk olarak customresponsedto dönüyordu ama biz burda getall generic serviceden geldiği için kendimiz maplememiz lazım.
            // Aşağıda ilk önce çevireceği tipi yazarız categorydto ya dönüştürecek categorydto'ya dönüştürdükten sonra yukarıdaki categories'i bir toList'e çevirli ve dönüştürme işlemini yapalım.
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories.ToList());

            return CreateActionResult(CustomResponseDto<List<CategoryDto>>.Success(200, categoriesDto));
        }

        // api/categories/GetSingleCategoryByIdWithProducts/2
        // Yukarıda yazdığım yolun sonundaki 2'yi almak için aşağıdaki koda categoryId'de veririz. Methoddaki paramtre adı neyse action'ın yanına da onu yazmalıyız. Nedeni framework'ün mapleyebilmesi.
        // Bizim aslında burda tüm kategorileri alacağımız bir nedpointe ihtiyacımız var çünkü dropdownlist'i doldurmamız lazım.
        [HttpGet("[action]/{categoryId}")]
        public async Task<IActionResult> GetSingleCategoryByIdWithProducts(int categoryId)
        {
            return CreateActionResult(await _categoryService.GetSingleCategoryByIdWithProductsAsync(categoryId));
        }
    }
}
