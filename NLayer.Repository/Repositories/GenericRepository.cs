using Microsoft.EntityFrameworkCore;
using NLayer.Core.Repositories;
using System.Linq.Expressions;

namespace NLayer.Repository.Repositories
{
    // Aşağıda T'yi class olarak belirtmessek  hata alırız aşağıda T ile beraber yazacağım ifade referans tipli olmalıdır.Biz aşağıda generic T ifadesine class vermessek biz buraya integer da gönderebiliriz ama efcore burada classlarla çalıştığı için açık açık belirtiriz.Olurda buraya başka birşey gönderirsek compile zamanında hata alırız.
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        // Aşağıda private yerine protected yapmamızın sebebi ilerde bazen özellikle product sınıfına ait aşağıdaki temel crud operasyonları dışında extra ayrı methodlara ihtiyacımız olursa diye private yerine protected yaparız.Protected erişim belirleyicisine miras aldığımız yerde erişebiliriz.
        protected readonly AppDbContext _context;


        // Aşağıda parametre olarak dbsete ihtiyacımız yok dbset bize context'in set'inden gelir 
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // Readonly keywordunu kullanıdğımızda bu değişkenlere ya o esnada değer atarız ya da constructor da değer atayabiliriz.Bu iki yer dışında değer atamaya kalkarsak compile zamanı hatası alırız.Biz mutlaka constructor da değer atayacağımız için daha sonra da kesinlikle set edilmemesi için readonly olarak belirtiyoruz.Yani değişkenlerimize farklı dbcontext'ler set edilmesin diye direk constructor da oluştururuz.
        // Biz genellikle constructora geçeceğimiz zaman değişkenlerimzi varsa bunları mutlaka readonyl yapıyoruz ki kazara başka şeyler set etmeyelim.
        private readonly DbSet<T> _dbSet;
        public async Task AddAsync(T entity)
        {
            // Readonly keywordunu kullanmış olduğumuz bir değişkeni gelip de burda dbsetine yeni birşeyler atamaya kalkarsak hata alırız örnek;
            // _dbSet = _context.Set<T>(); burda hata alırız.
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.AnyAsync(expression);
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> expression)
        {
            // Burda AsNoTracking dememizin sebebi efcore çekmiş olduğu dataları memory'e almasın Tracking etmesin ki daha performanslı çalışsın.Eğer kullanmassak 1000 tane data çekersek bu 1000 tane datayı memory'e alır ve anlık olarak durumlarını track eder izler.Bu da uygulamamnın performansını düşürür.
            return _dbSet.AsNoTracking().AsQueryable();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            // FindAsync methodu direk olarak bizden bir primary key bekler.Ama id'yi birden fazla da yazabiliriz.
            return await _dbSet.FindAsync(id);
        }

        public void Remove(T entity)
        {
            // Bunun asenkron methodunun olmamamsının sebebi çünkü bu remove dbden silmez sadece efcore'un memory de id'sini entity track ediyor biz remove dediğimizde aslında sadece o entity'nin state'ini deleted olarak işaretliyoruz yani bir flack koyuyoruz savechange methodunu çağırdığımızda efcore o deleted flackleri bulup db den siliyor.
            // Yani bizim burda remove dememizle aşağıdakini deleted olarak flack vermemiz aynı şey sadece property'sine set ediyoruz aşağıdaki gösterimde property'sine set ettiğimiz bir şeyin asenkron olmasına gerek yok.Burda ağır bir iş yapmıyoruz sadece memorydeki entity'nin state'ine bir enum değer atıyoruz.Yapmış olduğumuz işlem budur.Bu yüzden asnkronu yoktur.
            //_context.Entry(entity).State=EntityState.Deleted
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            // Bu da foreach ile entitylerde dönüp her bir entity'nin state'ini deleted olarak işaretliyor ne zaman savechange'ye basarsak otomatik bir şekilde gider ve o deleted flacke verilmiş entityleri veritabanından siliyor.
            _dbSet.RemoveRange();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }
    }
}
