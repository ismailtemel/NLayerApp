using NLayer.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Core.Repositories
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        // Geriye task döndüğü zaman asenkron olduğunu belirtmek için methodlarımızın sonuna async ekleriz.
        Task<Category> GetSingleCategoryByIdWithProductsAsync(int categoryId);
    }
}
