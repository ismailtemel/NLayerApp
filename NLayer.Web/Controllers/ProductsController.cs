using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLayer.Core.DTOs;
using NLayer.Core.Models;
using NLayer.Core.Services;

namespace NLayer.Web.Controllers
{
    // Biz productlarda baseentityde CreatedDate'i ve UpdatedDate'i otomatik bir şekilde merkezi bir yerden eklememiz lazım ileriki derslerde yapacağız.Kendimiz her bir ürün oluşturduğumuzda CreatedDate'i doldurmamıza gerek yok ya da her güncelleme yaptığımızda updateddate'i de kod tarafından bir tarih vermemize gerek yok SaveChangeyi ezip merkezi bir yerden yapacağız yani AppDbContext'imizin savechange methodunu override edeceğiz.
    public class ProductsController : Controller
    {
        // Eğer categorservice'imiz olmasaydı IService generic ile beraber kategoriyi alacaktık ama hali hazırda özelleştirilmiş bir kategori serivisimiz var.
        private readonly IProductService _services;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;

        public ProductsController(IProductService services, ICategoryService categoryService, IMapper mapper)
        {
            _services = services;
            _categoryService = categoryService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            // Bizim katmanlı mimari sadece bir web uygulaması dönecekse bir customresponse dönememize gerek yok.
            // Burda customResponsedto bizim ihtiyacımızı görmüyor.Bizim productwithcategorydto ya ihtiyacımız var.
            //var customResponse = await _services.GetProductListWithCategory();
            // Bizim asıl istediğimiz customresponse'ın datasıdır.GetProductWithCategory burda direk productwithcategory geliyor.Artık yukarıda yazdığımız kodu ayrı bir değişkene atamamaıza gerek kalmaz.
            // Aşağıda artık datayı almamıza gerek yok direk return View parantezleri içine gömebiliriz.
            // Servis katmanını mvc'ye uygun hale getirdik.
            return View(await _services.GetProductsWithCategory());
        }

        [HttpGet]
        public async Task<IActionResult> Save()
        {
            // Hatırlarsak product ile kategoriler arasında bire çok bir ilişki vardı biz ürün kaydederken name'ini stock'unu price'ını birde kategorisini dropdownlistten seçeceğiz bu yüzden bize kategoriler de lazım.
            // Aşağıda await vermeyi unutursak geriye task döner ve bu da hataya yol açar.
            var categories = await _categoryService.GetAllAsync();
            // Categories IEnumareable dönüyor aşağıda da list diyoruz aşağıdaki categories'in sonuna da tolist deriz ki uyum sağlasın.
            // Bunları istersek dto'ya da çevirebiliriz.
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories.ToList());
            // Dropdownlist'i doldurmak için aşağıya bir viewbag ekliyoruz.
            // Dropdowlist tip olarak selectlist alır.
            // SelectList'e parantez açtıktan sonra bizden IEnumarable interface'ini implemente etmiş bir liste bizde categoriesdto'yu veriyoruz. Ardından dropdowlistten birşey seçildiği zaman biz categorydto'dan "Id" sini göstereceğiz.Kullanıcılar category'nin Name'ini görecek.Yani kullanıcılar name'i seçtiği zaman arka planda Id'yi tutyor olacağız dropdownlist içerisinde.
            ViewBag.categories = new SelectList(categoriesDto, "Id", "Name");
            return View();
        }

        // Aşağıda ise kullanıcı butona bastığı zaman yapacağı işlemleri yazıyoruz.Aşağıdaki methodumuz productdto parametresini alır.
        [HttpPost]
        public async Task<IActionResult> Save(ProductDto productDto)
        {
            // IsValid'in anlamı Name'i price'ı stock'u geçerli ise aşağıdaki if bloklarının içinde kaydetme işlemi gerçekleştiririz.
            if (ModelState.IsValid)
            {
                // Aşağıda addasync'de bizden product ister fakat bizden productdto gelir.Bunu önlemek için mapper ile dönüştürme işlemi yaparız. Bu if blokları başarılıysa kaydetsin ve kaydettikten sonra da Redirecttoaction ile direk index sayfasında gitmesini sağlarız.Tip güvenli bir şekilde gitmesini istiyorsak nameof kullanırız.
                await _services.AddAsync(_mapper.Map<Product>(productDto));
                return RedirectToAction(nameof(Index));
            }
            // Eğer if bloğu başarısız olursa kategoriyi tekrar yüklesin aynı sayfayı tekrar dönsün.Aynı zamanda hata mesajları da dönecek onu da kodlayacağız zaten.
            // Aşağıdaki kategori kodlarını direk aldık burda da değişmeyeceği için.Değişmemesinin nedeni kullanıcı diyelim ki name alanını eksik bırakıp butona basarsa dropdownlist boş olur bu nedele aynı kodları buraya da yazdık.Çünkü yukarıdaki ve burdaki methodlar ayrı action methodlardır.Eğer başarılı devam ederse inedex'e yönelendiririz.Ama başarısızsa içinde bulunduğumuz methodda bu sayfa tekrar çalışacak yani kullanıcı name alanını boş bıraktı dropdownlisti doldurdu diyelim bastı butona kaydet diye Save methodu geldi eğer hata var ise bu sayfa tekrar çalışacak hata yok ise kaydedip index'e gidecek.Bu şartları sağlayan if bloğumuz yukrıdadiır.
            var categories = await _categoryService.GetAllAsync();
            // Bunları istersek dto'ya da çevirebiliriz.
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories.ToList());
            // Dropdownlist'i doldurmak için aşağıya bir viewbag ekliyoruz.
            // Dropdowlist tip olarak selectlist alır.
            // SelectList'e parantez açtıktan sonra bizden IEnumarable interface'ini implemente etmiş bir liste bizde categoriesdto'yu veriyoruz. Ardından dropdowlistten birşey seçildiği zaman biz categorydto'dan "Id" sini göstereceğiz.Kullanıcılar category'nin Name'ini görecek.Yani kullanıcılar name'i seçtiği zaman arka planda Id'yi tutyor olacağız dropdownlist içerisinde.
            ViewBag.categories = new SelectList(categoriesDto, "Id", "Name");
            return View();

        }

        // Update yaparken bize bir id gelir hangi id'ye sahip bir products'ı update yapacağız diye.
        [HttpGet]
        // Bu filter constructorunda bir parametre aldığı için servicefilter tanımlarız
        [ServiceFilter(typeof(NotFoundFilter<Product>))]
        public async Task<IActionResult> Update(int id)
        {
            var product = await _services.GetByIdAsync(id);
            // Bunun arkasından bizim dropdown list'i yine doldurmamız lazım mesela kalemler seçiliyse kalemler gözükmesi lazım kitaplar seçiliyse kitaplar gözükmesi lazım.
            var categories = await _categoryService.GetAllAsync();

            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories.ToList());

            // Aşağıda Name'in yanına bir parametre daha gelmesi lazım orda da bize seçilen değeri vermesini isteriz.Biz zaten id'sine ait products'ı bulduk. Bu id'ye sahip product'ın categoryId'sini veriyoruz.Artık kalemler kitaplar defterler derken gelen product defterlerle ilgili ise drop down listte defterlerle ilgili kayıtlar gözükecek ilk olarak.
            ViewBag.categories = new SelectList(categoriesDto, "Id", "Name", product.CategoryId);

            return View(_mapper.Map<ProductDto>(product));
        }
        [HttpPost]
        public async Task<IActionResult> Update(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                await _services.UpdateAsync(_mapper.Map<Product>(productDto));
                return RedirectToAction(nameof(Index));
            }

            var categories = await _categoryService.GetAllAsync();

            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories.ToList());

            ViewBag.categories = new SelectList(categoriesDto, "Id", "Name", productDto.CategoryId);
            return View(productDto);
        }

        public async Task<IActionResult> Remove(int id)
        {
            var products = await _services.GetByIdAsync(id);

            await _services.RemoveAsync(products);
            return RedirectToAction(nameof(Index));
        }
    }
}
