using NLayer.Core.Models;

namespace NLayer.Core.Repositories
{
    public interface IProductRespository : IGenericRepository<Product>
    {
        Task<List<Product>> GetProductListWithCategory();
    }
}
