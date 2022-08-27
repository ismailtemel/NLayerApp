using System.Linq.Expressions;
namespace NLayer.Core.Services
{
    public interface IService<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        //Aşağıdaki methodu async belirlemedik çünkü biz geriye IQueryable döneceğiz bunun veritabanına yansımasını bu serivisi çağıran kodda ToList veya ToListAsync çağırdığımız zaman veritabanına yansıması gerçekleşecek
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        // IService de veritabanına bu değişiklikleri yansıtacağımız için ve savechangeasync methodunu kullanacağımız dan dolayı bunları async'ye dönüştürürüz.
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);
    }
}
