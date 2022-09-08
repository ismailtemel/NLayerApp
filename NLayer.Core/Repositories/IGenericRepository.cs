using System.Linq.Expressions;

namespace NLayer.Core.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);

        // Aşağıda geriye direk IEnumarable dönmememizin sebebi datayı aldıktan sonra ToList döndüğümüz anda veritabanına sorguyu atar IQueryable da dönen şeylerde direk veritabanına sorgu atılmaz bunlar memory de bizim yazmış olduğumuz lamda ifadelerini tek seferde veritabanına gönderir tabi ToList ve ToListAsync gibi methodlar çağırıldıktan sonra.
        // productRepository.GetAll(x=>x.id>5).Tolist(); 
        IQueryable<T> GetAll();

        // Burada bir expression tanımlayacağız amacımız 
        // productRepository.where(x=>x.id>5).OrderBy.TolistAsync();
        // Eğer tipini IQueryable değilde list olarak belirlersek productRepository.where(x=>x.id>5) bu sorguyu çalıştırdığımız anda gider db den datayı çeker datayı çektikten sonra memory'e alır sonra orderby yapar.
        // Ama IQueryable dönersek where ifadesinden sonra orderby'ıda alır veritabanından direk olarak A'dan Z'ye ya da Z den A'ya göre Id'si 5 ten büyük olan dataları alır
        // Delegate'ler methodları işaret eden yapılardır. 
        // productRepository.where(x=>x.id>5).OrderBy.TolistAsync();
        IQueryable<T> Where(Expression<Func<T, bool>> expression);
        // Burada T'den kastımız entity'i alacak arkasından x.id>5 burdaki ifade de ise true veya false dönecek.
        // T yukaridaki sorguda ilk x'e karşılık geliyor. Boolean ise dönüş tipine karşılık geliyor x.id>5 burdan geriye ya true ya false dönecek herbir satır için
        Task AddAsync(T entity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);

        // Update veya remove bunların async methodları yok ef core tarafında olmasına da gerek yok çünkü ef core memory'e alıp takip etmiş olduğu bir product'ın sadece stateni değiştiriyor bu uzun süren bir işlem olmadığı için async yapısı yok yani biz bir şeyi update edebilmek için bir entity vermemiz lazım bu entity zaten memory de ef core tarafından takip ediliyor update dediğimizde sadece o memory de takip etmiş olduğu entitynin state'ini modified olarak değiştiriyor yani uzun süren bir işlem değil ama add memory'e bir data ekliyor burada bir süreç var uzun süren bir işlem olduğu için async'si var ama update ve remove'un ef core tarafında async methodu yok çünkü uzun süren işlemler değil.Async methodu kullanmamızın sebebi var olan thread'leri bloklamamak için thread'leri daha efektif kullanmak.
        void Update(T entity);
        void Remove(T entity);
        // Aşağıda da bir list almıyoruz bir interface alıyoruz. Yazılımda mümkün olduğunca soyut nesnelerle çalışmak çok önemli interfaceler gibi abstract classlar gibi bunlar soyut nesnelerdir new anahtar sözcüğü kullanarak nesne örneği alamayız mümkün olduğunca soyut nesnelerle çalışmak önemlidir.Çünkü aşağıdaki IEnumerable<T> istediğimiz bir tipe dönüştürebiliriz. Yani IIEnumerable interface'ini implemente etmiş herhangi bir class'a cast yapabiliriz
        Task AddRangeAsync(IEnumerable<T> entities);
        void RemoveRange(IEnumerable<T> entities);
    }
}
