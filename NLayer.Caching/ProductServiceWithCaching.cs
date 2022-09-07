using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Repositories;
using NLayer.Core.Services;
using NLayer.Core.UnitOfWorks;
using NLayer.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NLayer.Caching
{
    // Tüm endpointler cachden çalışacak.Product controller hangi interface'i implemente ediyor IProductService o zaman bizim de IProductService'yi implemente etmemiz lazım çünkü var olan yapıyı bozmamalıyız.
    // Buraya Decorator Design Pattern veya Decorator Design Pattern'a çok yakın olan Proxy Design Pattern 'ın biz bir implementasyonunu gerçekleştiriyoruz.Bu iki design patternları örnek vermemizin sebebi bu iki design pattern'ının birbirine çok yakın olan çok ufak bir implemntasyonunda bir farklılık olan 2 design pattern'dır.Kardeş design pattern da diyebiliriz.
    // Var olan yapıyı bozmamak için extradan kod yazacağız.
    // Dikkat edersek burda SOLID prensiplerinden olan open-closed'u kullanıyoruz.Yani gelişime kapalı ama değişime açık.
    // Var olan yapıyı bozmuyoruz ama yeni özellik eklediğim zamanda da kolay bir şekilde de implemente edebiliyoruz.
    // Şimdi burda bütün productları tutacağımız bir cach key tutacağız.
    // In-Memory cach de key-value şeklinde cachlerimizi tutabiliyoruz.
    public class ProductServiceWithCaching : IProductService
    {
        private const string CacheProductKey = "productsCache";
        // Arkasından bizim burda bir mapper'a ihtiyacımız var.Çünkü buralarda mapper'a döneceğimiz yerler var.
        private readonly IMapper _mapper;
        // Birde in-memory cach için IMemoryCach'e ihtiyacımız var.
        private readonly IMemoryCache _memoryCache;
        private readonly IProductRespository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductServiceWithCaching(IUnitOfWork unitOfWork, IProductRespository repository, IMemoryCache memoryCache, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
            _memoryCache = memoryCache;
            _mapper = mapper;

            // Burda productservicenin ilk nesne örneği oluşturuluduğu anda bir cachleme yapmamız gerekiyor.
            // TryGetValue yani değeri almaya çalış demektir.TryGetValue geriye boolean döner.Eğer bizim belirttiğimiz key'e sahip bir cach varsa geriye true döner yoksa false döner.Aynı zamanda geriye true döndüğünde out ile başlayan parametrede de cach de tutmuş olduğu datayı döner.Bir methodda birden fazla değer dönmek istiyorsak out keyword'ünü de dönebiliriz.Bunun best practicesi table lar dönebiliriz ama burda table yerine out kullanmış
            // Aşağıda key'i yazdıktan sonra out yazarız onu yazmamızın nedeni true false olduğunu öğrenmek.Aşağıda out'un yanına boş bir karakter bıraktık ki memory de yer tutmasın yani biz sadece aşağıda yazdığımız cache sahip yani cache key'ine sahip data varmı yokmu bunu öğrenmek isteriz.Cachdeki datayı almak istemiyoruz.Bu yüzden naptık memory de boşuna allow cate etmesin diye memory de allow cate yapmasını önlüyoruz.Yazdığımız alt tire birçok dilde de var.Eğer ki yok ise aşağıdaki memorycache ile başlyan yerin başına ünlem koyarız.
            if (!_memoryCache.TryGetValue(CacheProductKey, out _))
            {
                // Aşağıda productkey'e data'yı set et dedik hangi datayı _repoistory üzerinden git getall ile beraber tüm datayı al ve tolist deriz.
                // Eğer uygulama ilk ayağa kalktığında cach yoksa oluşturacak daha sonra zaten buraya girmeyecek çünkü artık cachde değer var yukardaki TryGetValue burdan true ya da false döner.Cache de varsa true döner.
                // Burada getall yerine getproductswithcategory ile de cachleyebiliriz. 
                // Programı çalıştırınca 500 hatası naldık nedeni aşağıdaki GetProductListWithCategory methodu async olarak geliyor fakat constructorun içine asenkron olarak gelemez bu yüzden sonuna result koyup senkrona çevirmemiz gerekiyor.Constructor içerisinde await kullanamayız.
                _memoryCache.Set(CacheProductKey, _repository.GetProductListWithCategory().Result);
                // Eğer aşağıdaki gibi bir kodlama yaparsak bu sefer de GetAll'ın bir anlamı kalmıyor.O zaman gelall da productla beraber bağlı olduğu kategoriler de dönüyor.O zaman product'a çok uygun olmuyor.
                //_memoryCache.Set(CacheProductKey, _repository.GetProductListWithCategory());
            }
        }

        // Bu yazdıklarımızı constructorda tek tek geçmemiz lazım.
        public async Task<Product> AddAsync(Product entity)
        {
            // Cachleyeceğimiz data çok değiştirmeyeceğimiz ama çok sık erişeceğimiz bir data en uygun cache adayıdır.Çok sık erişeceğiz ama çok sık güncellemediğimiz birşey olması lazım.Her seferinde cache'i yenilemek için ayrı ayrı yazmadan bir methoda cache kodları yazılıp ondan ulaşılabilir.
            await _repository.AddAsync(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
            // Entitymiz de db den verilen id ile beraber dönelim.
            return entity;
        }

        public async Task<IEnumerable<Product>> AddRangeAsync(IEnumerable<Product> entities)
        {
            await _repository.AddRangeAsync(entities);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
            return entities;
        }

       

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            var product = _memoryCache.Get<IEnumerable<Product>>(CacheProductKey);
            return Task.FromResult(product);
        }

        public Task<Product> GetByIdAsync(int id)
        {
            // Burdan bir async dönmemizi istiyor. Tamam o zaman bu sefer Task üzerinden fromresult diyoruz geriye bir asenkron dönmemiz gerekiyor.Bizden bir Task istiyor.Task'in yani Task sınıfının statik methodu var FromResult biz bu result ile beraber data dönebiliriz çünkü biz datayı cachden aldığımız için asenkron bir işlem yapmıyoruz bu yüzden memorycach'in yien Get'i ile beraber cachden bir list product bekliyoruz.Önce biz cach'e bir list product attık ve arkasından bir parantez açıyoruz ve bana bir key ver diyor o da CachProductKey'di. CachProductKey'imizi alıyoruz sonra "." koyuyoruz ve firstordefault'u artık kullanabiliriz.
            // Burda bir exception da fırlatacağız.
            var product = _memoryCache.Get<List<Product>>(CacheProductKey).FirstOrDefault(x => x.Id == id);

            if (product == null)
            {
                throw new NotFoundException($"{typeof(Product).Name}({id}) not found");
            }

            // Burda da istemiş olduğu product'ı asenkron olarak dönmüş olduk.
            // Burda fromresult dememizin sebebi await kullanmayışımız.Await kullanmadığımız için async de kullanmamıza gerek yok.O zaman bizden geriye bir task bekliyor mutlaka FromResult methodu üzerinden statik method üzerinden product'ımızı dönüyoruz.Üzerine geldiğimizde task.product dönüyor.
            return Task.FromResult(product);
        }

        // Cachlemede bir methodu nadir kullanıyorsak direk repodan dönebiliriz.
        public Task<List<ProductWithCategoryDto>> GetProductListWithCategory()
        {
            // Önceden sadece productsı vardı şimdi productlarını da al dedik biz burda getall da cach'i döndük.Aşağıda ise yine aynı methodu aldık ama methodumuz dto ve customreponse istediği için dto'yu çevirip customresponse ile beraber döndük.Bu methodun içindeki kodlarda category productları çekerken kategorileri de gelecek ama getall da productları ile beraber categorleri getallasync ile beraber productcontroller'daki All methoduna gelecek ama mapleme de dtoya dönüştüğü zaman productdto da category olmadığı için categoryler gelmez.Bu sayede bizim cach full time cach çalışacak ama bazen çok nadir kullandığımız bir method vardır bunu cachden çekmesin normal repodan çeksin dersek aşağıda yorum satırına aldığım ilk kod ile beraber diğer kodları dönebiliriz.
            // Eğer custom birşey döneceksek aşağıdaki gibi var products şeklinde bir değer oluştururuz.
            //var products = await _repository.GetProductListWithCategory();
            var products = _memoryCache.Get<IEnumerable<Product>>(CacheProductKey);
            //Burda mapper ile ilgili dto'ya mapliyoruz ve yukaridaki products'ı veriyoruz.
            var productsWithCategoryDto = _mapper.Map<List<ProductWithCategoryDto>>(products);

            // Ardından return customresponsedto dedikten sonra list döneceğiz ve durum koduyla productwithcategorydto'yu verip kod satırımızısonlandırdık.
            // Yukardaki GetProductListWithCategory isminin altının yeşil olmasının kod satırının içinde asenkronlukla ilgili bir kavram belitmemiş olmamız bunu da aşağıdaki gibi Task.FromResult ile çözeriz.Task.FromResult await yerine geçer.Await kullanılamayan yerlerde kullanılır.
            // Geriye bir Task dönememiz durumunda methodun içerisinde await kullanmadığımız durumlarda faydalandığımız bir methoddur.
            //return Task.FromResult (CustomResponseDto<List<ProductWithCategoryDto>>.Success(200, productsWithCategoryDto));
            return Task.FromResult(productsWithCategoryDto);
        }

        public async Task RemoveAsync(Product entity)
        {
            _repository.Remove(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public async Task RemoveRangeAsync(IEnumerable<Product> entities)
        {
            _repository.RemoveRange(entities);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public async Task UpdateAsync(Product entity)
        {
            _repository.Update(entity);
            await _unitOfWork.CommitAsync();
            await CacheAllProductsAsync();
        }

        public IQueryable<Product> Where(Expression<Func<Product, bool>> expression)
        {
            //Artık cachde sorgulama yapmamız gerekiyor get ile beraber nasıl bir data alacağız biz cach den bir list product almayı bekliyoruz CacheProductKey bu cach'i paranteze yazdıktan sonra bir where sorgusu yazacağız dikkat edersek bize yukarıdaki paramtreden bir expression geliyor.Where ise bir function istiyor.Burda gelen expression'u compile methodu üzerine geldiğimiz zaman Compile'ı fuctiona dönüştürüyor.Where de function istiyor nedeni artık burda bir link sorgusunda efcoredan çekmiyor artık cachden sorgu yapıyoruz.Complie dedikten sonra bunu AsQueryable'a dönüştürmemiz gerekiyor.
            return _memoryCache.Get<List<Product>>(CacheProductKey).Where(expression.Compile()).AsQueryable();
        }

        public async Task CacheAllProductsAsync()
        {
            // Burda memory cache üzerinden Set et diyoruz neyi set edeceğiz bizim CacheProductKey keyimiz vardı tüm datayı tekrar çeksin ve cachlesin.
            // Bu methodu her çağırdığımızda sıfırdan datayı çekip cacheliyor.
            _memoryCache.Set(CacheProductKey, await _repository.GetAll().ToListAsync());
        }

        public Task<bool> AnyAsync(Expression<Func<Product, bool>> expression)
        {
            throw new NotImplementedException();
        }
    }
}
