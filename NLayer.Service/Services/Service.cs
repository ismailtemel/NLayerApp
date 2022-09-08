using Microsoft.EntityFrameworkCore;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;
using NLayer.Service.Exceptions;
using System.Linq.Expressions;

namespace NLayer.Service.Services
{
    public class Service<T> : IService<T> where T : class
    {
        private readonly IGenericRepository<T> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public Service(IGenericRepository<T> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }


        // Burda savechange'i kullanabilmek ve veritabanına yansıtabilmek için Unit Of Work interface'ini implemente etmemiz gerekiyor.
        // Bu service repo ile haberleşecek ki veritabanıyla ilgili işlemleri yapacak ve aynı zamanda bu repo katmanında savechange methodunu çağırmadık 
        public async Task<T> AddAsync(T entity)
        {
            // Biz burda savechange dediğimizde efcore sqlserver tarafında ilgili satırlar için verilen id'yi direk olarak 
            // aşağıda parantez de yazmış olduğumuz entitynin id property'sine atar.  
            // Yukarıda bir entity de döneriz nedeni yukadıdaki add metodumuz geriye sadece task dönüyordu. Sonuçta bir data kaydettiğimizde onun id'sinide kulllanmak isteyebiliriz bu yüzden Task<T> bu şekilde yaparız.
            // Yani biz burda geriye eklenen entitiy'nin dönemsini sağladık.
            await _repository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
        {
            // Veri tabanınada oluşan herbir satırda geri döneriz.
            // Burdaki addrange geriye bir IEnumarable dönmesi lazım çünkü artık burda veri tabanına yansıtıp id ler geliyor onları dönüyoruz.
            await _repository.AddRangeAsync(entities);
            await _unitOfWork.CommitAsync();
            return entities;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _repository.AnyAsync(expression);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // GetAll geriye IEnumarable dönüyor IQueryable dönmüyor burda tolist veya tolistasync'i çağırırız.
            // GetAll da bir daha expresison vermemize gerek yok çünkü where bu işi yapıyor
            // ToList ile beraber datayı aldık ve geri döndük.
            return await _repository.GetAll().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            // Başlangıçta service ve repo katmanları birbirine benzer görünüyor fakat ilerledikçe olayın şekli değişmeye başlıyor.İlk başta GetAllAsync de dönüş tipleri değişti. Şimdi if ile birlikte bir bussines kodu ekliyoruz bu aradki fark program büyüdükçe değişmeye devam edecek.Bazen servis katmanından bazı api'lere istek yapmamız gerekecek vs.
            // Aşağıda return demek yerine var hasProduct yaparız nedeni burası bizim bussines kodları yazdığımız yer.
            var hasProduct = await _repository.GetByIdAsync(id);

            // Eğer bir hata yoksa aşağıdaki if bloğuna girmez.Direk aşağıdaki return ile yoluna devam eder.
            if (hasProduct == null)
            {
                //ClientSideException bir mesaj alıyordu parantezlere dinamik olarak bu class T alıyordu.Biz product'mı geliyor stock mu geliyor hangi entitynin geldiğini bilmiyoruz.Bu yüzden mantıklı bir hata dönememiz lazım.
                // Aşağıdaki gibi NotFound dönersek artık 404 durum kodunu alırız.
                // Çalıştırdığımızda daha şık durmasını istiyorsak eğer aşağıdaki gibi parantez içinde gelen id'yi de yazdırabiliriz.Süslü parantezleri dinamik çalışması için koyarız.
                throw new NotFoundException($"{typeof(T).Name}({id}) not found");
            }
            return hasProduct;
        }

        public async Task RemoveAsync(T entity)
        {
            _repository.Remove(entity);
            await _unitOfWork.CommitAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<T> entities)
        {
            _repository.RemoveRange(entities);
            await _unitOfWork.CommitAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            // Where geriye tolist değil IQueryable dönüyor bu yüzden burada asenkron bir durum yok çünkü tolist veya tolistasync bu where methodunu çağırdığımız yerde yani api tarafında kullanacağız
            return _repository.Where(expression);
        }
    }
}
