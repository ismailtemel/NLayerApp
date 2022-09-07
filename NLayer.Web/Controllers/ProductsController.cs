using Microsoft.AspNetCore.Mvc;
using NLayer.Core.Services;

namespace NLayer.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _services;

        public ProductsController(IProductService services)
        {
            _services = services;
        }

        public async Task<IActionResult> Index()
        {
            // Bizim katmanlı mimari sadece bir web uygulaması dönecekse bir customresponse dönememize gerek yok.
            // Burda customResponsedto bizim ihtiyacımızı görmüyor.Bizim productwithcategorydto ya ihtiyacımız var.
            //var customResponse = await _services.GetProductListWithCategory();
            // Bizim asıl istediğimiz customresponse'ın datasıdır.GetProductListWithCategory burdak direk productwithcategory geliyor.Artık yukarıda yazdığımız kodu ayrı bir değişkene atamamaıza gerek kalmaz.
            // Aşağıda artık datayı almamıza gerek yok direk return View parantezleri içine gömebiliriz.
            // Servis katmanını mvc'ye uygun hale getirdik.
            return View(await _services.GetProductListWithCategory());
        }
    }
}
