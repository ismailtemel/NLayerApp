using System.Linq.Expressions;
namespace NLayer.Core.Services
{
    public interface IService<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(); // GetAll da expression ifadesini kaldırdık çünkü aynı ifade where de var.
        //Aşağıdaki methodu async belirlemedik çünkü biz geriye IQueryable döneceğiz bunun veritabanına yansımasını bu serivisi çağıran kodda ToList veya ToListAsync çağırdığımız zaman veritabanına yansıması gerçekleşecek
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task<T> AddAsync(T entity); // Bu geriye sadece task dönerken artık kendi tiplerini dönüyor.
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
        // IService de veritabanına bu değişiklikleri yansıtacağımız için ve savechangeasync methodunu kullanacağımız'dan dolayı bunları async'ye dönüştürürüz.
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
        Task RemoveRangeAsync(IEnumerable<T> entities);
    }
}
