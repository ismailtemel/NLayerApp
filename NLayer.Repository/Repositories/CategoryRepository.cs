using Microsoft.EntityFrameworkCore;
using NLayer.Core.Models;
using NLayer.Core.Repositories;

namespace NLayer.Repository.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Category> GetSingleCategoryByIdWithProductsAsync(int categoryId)
        {
           // x navigation property'imiz products'dı çünkü kategorinin birden fazla products'ı olabilir.
           // Id'si yukarıdaki categoryId'ye eşit olanın singleordefault'unu bul diyoruz.
           // SingleOrDefault ile firstordefaultun farkı eğer id'deb db de 4 5 tane varsa ilkini bulur döndürür ama single dersek eğer bu id'den aşağıdaki where koşulunu karşılayan birden fazla satır bulursa geriye hata döner.
           return await _context.Categories.Include(x=>x.Products).Where(x=>x.Id==categoryId).SingleOrDefaultAsync();
        }
    }
}
