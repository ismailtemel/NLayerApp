using Microsoft.AspNetCore.Mvc;
using NLayer.API.Filter;
using NLayer.Core.Services;

namespace NLayer.API.Controllers
{
    
    public class CategoriesController : CustomBaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // api/categories/GetSingleCategoryByIdWithProducts/2
        // Yukarıda yazdığım yolun sonundaki 2'yi almak için aşağıdaki koda categoryId'de veririz. Methoddaki paramtre adı neyse action'ın yanına da onu yazmalıyız. Nedeni framework'ün mapleyebilmesi.
        [HttpGet("[action]/{categoryId}")]
        public async Task<IActionResult> GetSingleCategoryByIdWithProducts(int categoryId)
        {
            return CreateActionResult(await _categoryService.GetSingleCategoryByIdWithProductsAsync(categoryId));
        }
    }
}
