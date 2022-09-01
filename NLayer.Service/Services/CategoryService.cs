using AutoMapper;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;

namespace NLayer.Service.Services
{
    public class CategoryService : Service<Category>, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(IGenericRepository<Category> repository, IUnitOfWork unitOfWork, ICategoryRepository categoryRepository, IMapper mapper) : base(repository, unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CustomResponseDto<CategoryWithProductsDto>> GetSingleCategoryByIdWithProductsAsync(int categoryId)
        {
            // Aşağıdaki kodun anlamı : Bize tek bir category gelecek bu bir entity olacak await ile beraber categoryReposiyoryden getsinglecategory dön ama aynı zamanda productları dön ve bana bir id ver diyor methodda paramtere olarak geçmiş olduğumuz id'yi verdik. Bunu aynı zamanda dto'ya dönüştürmemiz gerekiyor.
            var category = await _categoryRepository.GetSingleCategoryByIdWidthProductsAsync(categoryId);

            var categoryDto = _mapper.Map<CategoryWithProductsDto>(category);

            // Return'un yanına eğer new koyarsak success methodumuz gelmez.
            return  CustomResponseDto<CategoryWithProductsDto>.Success(200,categoryDto);
        }
    }
}
