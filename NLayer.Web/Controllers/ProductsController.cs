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
            var customResponse = await _services.GetProductListWithCategory();
            // Bizim asıl istediğimiz customresponse'ın datasıdır.
            return View(customResponse.Data);
        }
    }
}
